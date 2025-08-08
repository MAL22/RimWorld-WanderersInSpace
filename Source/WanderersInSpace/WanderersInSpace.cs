using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using Random = UnityEngine.Random;

namespace WanderersInSpace
{
    [StaticConstructorOnStartup]
    public static class WanderersInSpace
    {
        static WanderersInSpace()
        {
            var harmony = new Harmony("MAL22.WanderersInSpace");
            harmony.PatchAll();

            Log.Message($"[WanderersInSpace] Harmony patch initialized");
        }
    }

    [HarmonyPatch]
    public static class ChoiceLetter_AcceptJoiner_Patch
    {
        static MethodBase TargetMethod()
        {
            var PredicateClass = typeof(ChoiceLetter_AcceptJoiner).GetNestedTypes(AccessTools.all).FirstOrDefault(t => t.FullName.Contains("d__10"));
            return PredicateClass.GetMethods(AccessTools.all).FirstOrDefault(m => m.Name.Contains("MoveNext"));
        }
        
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var isSpaceField = AccessTools.Field(typeof(PlanetLayerDef), nameof(PlanetLayerDef.isSpace));
            
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand.Equals(isSpaceField))
                {
                    codes[i] = new CodeInstruction(OpCodes.Pop);
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldc_I4_0));
                    Log.Message($"[WanderersInSpace] Harmony patch applied");
                    break;
                }
            }
            
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.SetFaction), new[] { typeof(Faction), typeof(Pawn) })]
    public static class Pawn_SetFaction_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Faction newFaction, Pawn recruiter, Pawn __instance)
        {
            if (newFaction == Faction.OfPlayer && !__instance.IsPrisoner && !__instance.IsSlave && __instance.ConcernedByVacuum && Find.CurrentMap.Tile.LayerDef.isSpace)
            {
                var vacsuitDef = DefDatabase<ThingDef>.GetNamed("Apparel_Vacsuit");
                var vacsuitHelmetDef = DefDatabase<ThingDef>.GetNamed("Apparel_VacsuitHelmet");
                
                if (ThingMaker.MakeThing(vacsuitDef) is Apparel vacsuitApparel)
                {
                    vacsuitApparel.HitPoints = (int)(vacsuitApparel.MaxHitPoints * Random.Range(0.13f, 0.47f));
                    __instance.apparel.Wear(vacsuitApparel, false);
                }

                if (ThingMaker.MakeThing(vacsuitHelmetDef) is Apparel vacsuitHelmetApparel)
                {
                    vacsuitHelmetApparel.HitPoints = (int)(vacsuitHelmetApparel.MaxHitPoints * Random.Range(0.13f, 0.47f));
                    __instance.apparel.Wear(vacsuitHelmetApparel, false);
                }
            }
        }
    }
}