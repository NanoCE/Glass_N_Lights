using Harmony;
using RimWorld;
using System.Reflection;
using Verse;

namespace Glass_Lights
{
    [StaticConstructorOnStartup]
    internal static class HarmonyPatches
    {
        
        static HarmonyPatches()
        {
            var harmony = HarmonyInstance.Create("Glass_Lights");
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
    }
}
