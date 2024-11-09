using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using HarmonyLib;
using FistVR;

namespace Ethiom101
{
	[HarmonyPatch(typeof(ClosedBolt))]
	static public class SPATCH
	{
		[HarmonyPatch("UpdateBolt")]
		[HarmonyPostfix]
		static public void OverrrideSafety(ClosedBolt __instance)
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

			// Are we on safe?
			if (__instance.Weapon.FireSelector_Modes[__instance.Weapon.m_fireSelectorMode].ModeType != ClosedBoltWeapon.FireSelectorModeType.Safe)
				return;

			// Check to see if the bolt has traveled at least half way
			float boltTravel = Vector3.Distance(__instance.Point_Bolt_Forward.position, __instance.Point_Bolt_Rear.position);
			float currentBoltPos = Vector3.Distance(__instance.Point_Bolt_Forward.position, __instance.transform.position);
			if(currentBoltPos/boltTravel >= 0.5)
				__instance.Weapon.ToggleFireSelector(); // Switch from Safe to Fire (we verified we're on safe already above)
		}
	}
}
