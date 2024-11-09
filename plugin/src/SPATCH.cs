using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using HarmonyLib;
using FistVR;

namespace Ethiom101
{
	static public class SPATCH
	{
		[HarmonyPatch(typeof(ClosedBolt), "Awake")]
		[HarmonyPostfix]
		public static void EnableBoltHoldOpen(ClosedBolt __instance)
		{
			// Are we a SPAS-15?
			switch (__instance.Weapon.ObjectWrapper.ItemID)
			{
				case "SPAS15":
				case "SPAS15Compact":
				case "SPAS15Tactical":
					break;
				default:
					return;
			}

			__instance.HasLastRoundBoltHoldOpen = true;
		}

		[HarmonyPatch(typeof(ClosedBolt), "UpdateBolt")]
		[HarmonyPostfix]
		public static void OverrrideSafety(ClosedBolt __instance)
		{
			// Are we a SPAS-15?
			switch (__instance.Weapon.ObjectWrapper.ItemID)
			{
				case "SPAS15":
				case "SPAS15Compact":
				case "SPAS15Tactical":
					break;
				default:
					return;
			}

			// Are we already on fire?
			if (!__instance.Weapon.IsWeaponOnSafe())
				return; // Nothing to do

			// Check to see if the bolt has traveled at least half way
			float boltTravel = Vector3.Distance(__instance.Point_Bolt_Forward.position, __instance.Point_Bolt_Rear.position);
			float currentBoltPos = Vector3.Distance(__instance.Point_Bolt_Forward.position, __instance.transform.position);
			if (currentBoltPos / boltTravel >= 0.5)
				__instance.Weapon.ToggleFireSelector(); // Switch from Safe to Fire (we verified we're on safe already above)
		}

		[HarmonyReversePatch]
		[HarmonyPatch(typeof(FVRInteractiveObject), "FVRUpdate")]
		public static void OvrrideFVRUpdate(object instance)
		{

		}

		[HarmonyPatch(typeof(ClosedBoltForeHandle), "FVRUpdate")]
		[HarmonyPrefix]
		public static bool LockUnlockPump(ClosedBoltForeHandle __instance)
		{
			// Are we a SPAS-15?
			switch (__instance.Weapon.ObjectWrapper.ItemID)
			{
				case "SPAS15":
				case "SPAS15Compact":
				case "SPAS15Tactical":
					break;
				default:
					return true;
			}

			OvrrideFVRUpdate(__instance);

			if (__instance.IsHeld && __instance.Mode == ClosedBoltForeHandle.ForeHandleMode.Racking)
			{
				Vector3 closestValidPoint = __instance.GetClosestValidPoint(__instance.ForeHandlePoint_Forward.position, __instance.ForeHandlePoint_Rear.position, __instance.m_hand.Input.Pos);
				Vector3 vector = __instance.Weapon.transform.InverseTransformPoint(closestValidPoint);
				float num = Mathf.InverseLerp(__instance.ForeHandlePoint_Forward.localPosition.z, __instance.ForeHandlePoint_Rear.localPosition.z, vector.z);
				if (__instance.Pos == ClosedBoltForeHandle.ForeHandlePos.Forward && num > 0.7f && __instance.Weapon.IsWeaponOnSafe())
				{
					__instance.m_tarPos = ClosedBoltForeHandle.ForeHandlePos.Rear;
				}
				else if (__instance.Pos == ClosedBoltForeHandle.ForeHandlePos.Rear && num < 0.3f && __instance.Weapon.Magazine != null && __instance.Weapon.Magazine.HasARound())
				{
					__instance.m_tarPos = ClosedBoltForeHandle.ForeHandlePos.Forward;
				}
			}
			if (__instance.Mode == ClosedBoltForeHandle.ForeHandleMode.Racking)
			{
				if (__instance.m_tarPos == ClosedBoltForeHandle.ForeHandlePos.Rear && __instance.Pos != ClosedBoltForeHandle.ForeHandlePos.Rear)
				{
					__instance.posLerp += Time.deltaTime * 20f;
					__instance.transform.localPosition = Vector3.Lerp(__instance.ForeHandlePoint_Forward.localPosition, __instance.ForeHandlePoint_Rear.localPosition, __instance.posLerp);
					if (__instance.posLerp >= 1f)
					{
						__instance.posLerp = 1f;
						__instance.ArriveAtRear();
					}
					else
					{
						__instance.Pos = ClosedBoltForeHandle.ForeHandlePos.Mid;
					}
				}
				else if (__instance.m_tarPos == ClosedBoltForeHandle.ForeHandlePos.Forward && __instance.Pos != ClosedBoltForeHandle.ForeHandlePos.Forward)
				{
					__instance.posLerp -= Time.deltaTime * 20f;
					__instance.transform.localPosition = Vector3.Lerp(__instance.ForeHandlePoint_Forward.localPosition, __instance.ForeHandlePoint_Rear.localPosition, __instance.posLerp);
					if (__instance.posLerp <= 0f)
					{
						__instance.posLerp = 0f;
						__instance.ArriveAtFront();
					}
					else
					{
						__instance.Pos = ClosedBoltForeHandle.ForeHandlePos.Mid;
					}
				}
			}
			bool flag = false;
			if (__instance.Mode == ClosedBoltForeHandle.ForeHandleMode.Racking)
			{
				flag = true;
			}
			float num2 = Mathf.InverseLerp(__instance.ForeHandlePoint_Forward.localPosition.z, __instance.ForeHandlePoint_Rear.localPosition.z, __instance.transform.localPosition.z);
			__instance.Weapon.Bolt.UpdateHandleHeldState(flag, num2);

			return false;
		}

		[HarmonyPatch(typeof(ClosedBoltWeapon), "Fire")]
		[HarmonyPostfix]
		public static void SafeWhenFire(ClosedBoltWeapon __instance)
		{
			// Are we a SPAS-15?
			switch (__instance.ObjectWrapper.ItemID)
			{
				case "SPAS15":
				case "SPAS15Compact":
				case "SPAS15Tactical":
					break;
				default:
					return;
			}

			// A SPAS-15 can only be fired when toggled to fire so
			__instance.ToggleFireSelector(); // Switch to safe
		}


		//[HarmonyPatch(typeof(ClosedBolt), "UpdateBolt")]
		//[HarmonyPostfix]
		//public static void UnlockBoltWhenSafe(ClosedBolt __instance)
		//{
		//	// Are we a SPAS-15?
		//	switch (__instance.Weapon.ObjectWrapper.ItemID)
		//	{
		//		case "SPAS15":
		//		case "SPAS15Compact":
		//		case "SPAS15Tactical":
		//			break;
		//		default:
		//			return;
		//	}

		//	// Are we on safe?
		//	if (__instance.Weapon.IsWeaponOnSafe())
		//		return;

		//	// TODO - Unlock the bolt
		//}

		//[HarmonyPatch(typeof(ClosedBoltWeapon), "Fire")]
		//[HarmonyPrefix]
		//public static void UnlockBoltWhenFire(ClosedBoltWeapon __instance)
		//{
		//	// Are we a SPAS-15?
		//	switch (__instance.ObjectWrapper.ItemID)
		//	{
		//		case "SPAS15":
		//		case "SPAS15Compact":
		//		case "SPAS15Tactical":
		//			break;
		//		default:
		//			return;
		//	}

		//	// When action has been cycled the bolt is locked, firing the gun will unlock the bolt
		//	// TODO - Unlock the bolt
		//}

		//[HarmonyPatch(typeof(ClosedBolt), "BoltEvent_ArriveAtFore")]
		//[HarmonyPostfix]
		//public static void LockBoltWhenChamberRound(ClosedBolt __instance)
		//{
		//	// Are we a SPAS-15?
		//	switch (__instance.Weapon.ObjectWrapper.ItemID)
		//	{
		//		case "SPAS15":
		//		case "SPAS15Compact":
		//		case "SPAS15Tactical":
		//			break;
		//		default:
		//			return;
		//	}

		//	//if (!__instance.Weapon.IsWeaponOnSafe()) // If the gun is on fire when the round is chambered (they may drop the bolt slowly and engage the safety part way)

		//}
	}
}

// TODO - Lock bolt to rear on empty mag for pump action
