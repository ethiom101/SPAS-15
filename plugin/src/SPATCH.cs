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

		[HarmonyPatch(typeof(ClosedBoltForeHandle), "FVRUpdate")]
		[HarmonyPrefix]
		public static void UnlockPump(ClosedBoltForeHandle __instance)
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

			if (__instance.IsHeld && __instance.Mode == ClosedBoltForeHandle.ForeHandleMode.Racking)
			{
				Vector3 closestValidPoint = __instance.GetClosestValidPoint(__instance.ForeHandlePoint_Forward.position, __instance.ForeHandlePoint_Rear.position, __instance.m_hand.Input.Pos);
				Vector3 vector = __instance.Weapon.transform.InverseTransformPoint(closestValidPoint);
				float num = Mathf.InverseLerp(__instance.ForeHandlePoint_Forward.localPosition.z, __instance.ForeHandlePoint_Rear.localPosition.z, vector.z);
				if (__instance.Pos == ClosedBoltForeHandle.ForeHandlePos.Forward && num > 0.7f && __instance.Weapon.IsWeaponOnSafe())
				{
					__instance.m_tarPos = ClosedBoltForeHandle.ForeHandlePos.Rear;
				}
			}
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

		//	// Switching the gun from fire to safe will unlock the bolt allowing it to be cycled freely
		//	__instance.ReleaseBolt();
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
		//	__instance.Bolt.ReleaseBolt();
		//}

		//[HarmonyPatch(typeof(ClosedBoltWeapon), "ChamberRound")]
		//[HarmonyPostfix]
		//public static void LockBoltWhenChamberRound(ClosedBoltWeapon __instance)
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

		//	if(!__instance.IsWeaponOnSafe()) // If the gun is on fire when the round is chambered (they may drop the bolt slowly and engage the safety part way)
		//		__instance.Bolt.LockBolt();
		//}
	}
}

// TODO - Lock bolt to rear on empty mag for pump action
