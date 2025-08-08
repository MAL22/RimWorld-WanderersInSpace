using HarmonyLib;
using Verse;

namespace WanderersInSpace
{
    public class WanderersInSpace_ModSettings : ModSettings
    {
        public bool IsSuitTainted;
        public float SuitMinHitpoints = 0.2f;
        public float SuitMaxHitpoints = 0.6f;
        
        public override void ExposeData()
        {
            Scribe_Values.Look(ref IsSuitTainted, "IsSuitTainted", false);
            Scribe_Values.Look(ref SuitMinHitpoints, "SuitMinHitpoints", 0.2f);
            Scribe_Values.Look(ref SuitMaxHitpoints, "SuitMaxHitpoints", 0.6f);
            
            base.ExposeData();
        }
    }
}