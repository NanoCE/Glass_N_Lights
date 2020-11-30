using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace Glass_Lights
{
	[StaticConstructorOnStartup]
	internal static class HarmonyInit
	{
		static HarmonyInit()
		{
			var harmony = new Harmony("NanoCE.Glass_Lights");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		[HarmonyPatch(typeof(PowerNetGraphics), "PrintWirePieceConnecting")]
		static class Harmony_PrintWirePieceConnecting
		{
			static bool Prefix(SectionLayer layer, ref Thing A, Thing B, bool forPowerOverlay)
			{
				if (A.def.building != null && !A.def.building.allowWireConnection && A.TryGetComp<CompPowerTrader>() != null)
				{
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(RoofGrid), "SetRoof")]
		private static class Patch_SetRoof
		{
			private static void Postfix(Map ___map, ref IntVec3 c, ref RoofDef def)
			{
				if (def == null)
				{
					c.GetThingList(___map).OfType<Glass_Lights.IRoofMounted>().ToList<Glass_Lights.IRoofMounted>().ForEach(x =>
					{
						x.Poof();
					});
				}
			}
		}

		[HarmonyPatch(typeof(Building), "Destroy")]
		private static class Patch_BuildingDestroy
		{
			private static void Prefix(Building __instance)
			{				
				if (__instance != null && __instance.def != null &&__instance.def.passability == Traversability.Impassable && __instance.Map != null)
				{
					//Log.Message(string.Format("{0} item is impassble, checking poof", __instance.def.defName));

					(from m in __instance.Position.GetThingList(__instance.Map).OfType<Glass_Lights.IWallMounted>()
					 where m != __instance
					 select m).ToList<Glass_Lights.IWallMounted>().ForEach(x =>
					 {
						 //test
						 //Log.Message("Wall Mounted thing found, poof");
						 x.Poof();
					 });
				}
			}
		}

		//Test Ignore Code

		////Patchs for thingwithcomps that are not apparel that exist in recipes with apparel
		//[HarmonyPatch(typeof(SpecialThingFilterWorker_NonDeadmansApparel), "Matches")]
		//public static class Patch_SpecialThingFilterWorkerMatches
		//{
		//	//Defaults all ThingWithComp objects to return true if the special filter is called for Non-DeadmansApparel. Used for niche situations when
		//	//an ThingWithComp is put in the Apparel Category which causes these items to appear in recipes that use the Apparel category tag. 
		//	//ex: Recyling Apparel mods
		//	public static void Postfix(ref bool __result, ref Thing t)
		//	{
		//		if(t != null && t is ThingWithComps && !(t is Apparel))
		//		{
		//			__result = true;
		//		}
		//	}
		//}

		//[HarmonyPatch(typeof(SpecialThingFilterWorker_DeadmansApparel), "Matches")]
		//public static class Patch_SpecialThingFilterWorker_DeadmansApparel
		//{
		//	//Defaults all ThingWithComp objects to return true if the special filter is called for Deadmans Apparel. Used for niche situations when
		//	//an ThingWithComp is put in the Apparel Category which causes these items to appear in recipes that use the Apparel category tag. 
		//	//ex: Recyling Apparel mods
		//	public static void Postfix(ref bool __result, ref Thing t)
		//	{
		//		if (t != null && t is ThingWithComps && !(t is Apparel))
		//		{
		//			__result = false;
		//		}
		//	}
		//}

		//[HarmonyPatch(typeof(CostListCalculator), "CostListAdjusted", new Type[] { typeof(BuildableDef), typeof(ThingDef), typeof(bool) })]
		//public static class Patch_CostListAdjusted
		//{
		//	public static void Postfix(ref BuildableDef entDef,ref  ThingDef stuff,ref bool errorOnNullStuff, ref List<ThingDefCountClass> __result)
		//	{
		//		int volumeMultiplier;

		//		if (stuff != null && stuff.HasModExtension<Glass_LightsExtension>() && __result != null)
		//		{
		//			volumeMultiplier = stuff.GetModExtension<Glass_LightsExtension>().volumeMultiplier;

		//			if(volumeMultiplier < 1)
		//			{
		//				Log.Warning("VolumeMultipler for thing: " + stuff.defName + " is 0 or a negative number. Unable to perform calculation. Defaulting to original stuff cost.");
		//			}
		//			else
		//			{
		//				int temp = __result.Last().count * volumeMultiplier;
		//				Log.Message(temp.ToString());
		//				__result.Last().count = temp;
		//			}
		//		}
		//	}
		//}

		//[HarmonyPatch(typeof(ThingDef), "get_VolumePerUnit")]
		//public static class Patch_ThingDef_VolumePerUnit
		//{
		//	//Defaults all ThingWithComp objects to return true if the special filter is called for Deadmans Apparel. Used for niche situations when
		//	//an ThingWithComp is put in the Apparel Category which causes these items to appear in recipes that use the Apparel category tag. 
		//	//ex: Recyling Apparel mods
		//	public static void Postfix(ThingDef __instance, ref float __result)
		//	{
		//		if (!__instance.smallVolume && __instance != null && __instance.HasModExtension<Glass_LightsExtension>())
		//		{
		//			__result = __instance.GetModExtension<Glass_LightsExtension>().customVolume;
		//		}				
		//	}
		//}
	}
}
