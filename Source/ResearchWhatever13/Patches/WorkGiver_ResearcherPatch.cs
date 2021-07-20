using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;

namespace ResearchWhatever.Patches
{
    [HarmonyPatch(typeof(WorkGiver_Researcher), "ShouldSkip")]
    static class WorkGiver_Researcher_ShouldSkip_ResearchWhateverPatch
    {
        static bool Prefix(ref bool __result)
        {
            __result = false;
            //Log.Message($"ShouldSkip={__result}");
            return false;
        }
    }

    [HarmonyPatch(typeof(WorkGiver_Researcher), "PotentialWorkThingRequest", MethodType.Getter)]
    static class WorkGiver_PotentialWorkThingRequest_ResearchWhateverPatch
    {
        static bool Prefix(ref ThingRequest __result)
        {
            __result = ThingRequest.ForGroup(ThingRequestGroup.ResearchBench);
            return false;
        }
    }

    [HarmonyPatch(typeof(WorkGiver_Researcher), "HasJobOnThing")]
    static class WorkGiver_Researcher_HasJobOnThing_ResearchWhateverPatch
    {
        private static bool hasFacilities(this Building_ResearchBench bench, List<ThingDef> requiredFacilities)
        {
            if (requiredFacilities.NullOrEmpty()) return true;
            CompAffectedByFacilities comp = bench.TryGetComp<CompAffectedByFacilities>();
            if (comp == null) return false;

            foreach (var rf in requiredFacilities)
                if (comp.LinkedFacilitiesListForReading.FirstOrDefault(x => x.def == rf && comp.IsFacilityActive(x)) == null)
                    return false;

            return true;
        }

        static void Prefix(Pawn pawn, Thing t, bool forced)
        {
            
            ResearchProjectDef currentProj = Find.ResearchManager.currentProj;
            if (currentProj != null) return;
            Building_ResearchBench bench = t as Building_ResearchBench;
            if (bench == null) return;

            ResearchWhateverComp comp = bench.TryGetComp<ResearchWhateverComp>();

            if (comp == null || !comp.Active) return;

            List<ResearchProjectDef> projects = new List<ResearchProjectDef>(
                from x in DefDatabase<ResearchProjectDef>.AllDefsListForReading
                where Find.Storyteller.difficulty.AllowedBy(x.hideWhen)
                && !x.IsFinished
                && x.TechprintRequirementMet
                && x.PrerequisitesCompleted 
                && (x.requiredResearchBuilding == null || x.requiredResearchBuilding == bench.def && bench.hasFacilities(x.requiredResearchFacilities))
                select x);

            if (projects.NullOrEmpty())
            {
                comp.Active = false;
                Messages.Message("ResearchWhateverNothingLeftToResearch".Translate(bench.Label).CapitalizeFirst(), new TargetInfo(bench.Position, bench.Map, false), MessageTypeDefOf.NeutralEvent);
                return;
            }
            projects.SortBy(x => x.CostApparent);

            ResearchProjectDef def = projects.First();
            projects.TryRandomElementByWeight(x => x.CostApparent == def.CostApparent ? 1f : 0f, out def);
            SoundDefOf.ResearchStart.PlayOneShotOnCamera(null);
            Find.ResearchManager.currentProj = def;
            TutorSystem.Notify_Event("StartResearchProject");
            Messages.Message( "ResearchWhateverNewResearch".Translate(pawn.Name.ToStringFull, def.label).CapitalizeFirst(), new TargetInfo(bench.Position, bench.Map, false), MessageTypeDefOf.SilentInput);
        }
    }
}
