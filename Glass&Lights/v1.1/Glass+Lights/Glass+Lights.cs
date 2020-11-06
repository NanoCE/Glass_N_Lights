using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.AI;
using static Glass_Lights.HarmonyPatches;

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

        [HarmonyPatch(typeof(SickPawnVisitUtility), "CanVisit")]
        static class Harmony_FindRandomSickPawn
        {
            static bool Prefix(Pawn pawn, Pawn sick, JoyCategory maxPatientJoy, ref bool __result)
            {
                if(sick.needs.rest != null)
                {                    
                    return true;
                }

                Log.Message("Needs Null");
                __result = sick.IsColonist && !sick.Dead && pawn != sick && sick.InBed() 
                    && sick.Awake() && !sick.IsForbidden(pawn) && sick.needs.joy != null 
                    && sick.needs.joy.CurCategory <= maxPatientJoy 
                    && InteractionUtility.CanReceiveInteraction(sick) && !sick.needs.food.Starving
                    && pawn.CanReserveAndReach(sick, PathEndMode.InteractionCell, Danger.None, 1, -1, null, false) 
                    && !SickPawnVisitUtility.AboutToRecover(sick);
                return false;
            }
        }        
    }    
}
