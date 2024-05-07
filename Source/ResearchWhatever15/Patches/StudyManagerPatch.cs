using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace ResearchWhatever.Patches
{
    [HarmonyPatch(typeof(StudyManager), "StudyAnomaly")]
    static class StudyManager_StudyAnomaly_ResearchWhateverPatch
    {
        internal static void Prefix(Thing studiedThing, Pawn studier, float knowledgeAmount, KnowledgeCategoryDef knowledgeCategory)
        {
            if (!ModsConfig.AnomalyActive)
            {
                return;
            }
            if (knowledgeAmount <= 0f)
            {
                return;
            }
            //
            var researchManager = Find.ResearchManager;
            var proj = researchManager.GetProject(knowledgeCategory);

            if (proj != null)
                return;

            if (knowledgeCategory.overflowCategory != null)
                proj = researchManager.GetProject(knowledgeCategory.overflowCategory);
            //
            if (proj != null)
                return;

            var projs = DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where((ResearchProjectDef x) => x.CanStartNow && x.knowledgeCategory == knowledgeCategory).ToList();
            if (projs.NullOrEmpty() && knowledgeCategory.overflowCategory != null)
                projs = DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where((ResearchProjectDef x) => x.CanStartNow && x.knowledgeCategory == knowledgeCategory.overflowCategory).ToList();
            
            if (projs.NullOrEmpty())
            {
                CompStudiable compStudiable = studiedThing.TryGetComp<CompStudiable>();
                CompHoldingPlatformTarget compHoldingPlatformTarget = studiedThing.TryGetComp<CompHoldingPlatformTarget>();

                bool b = false;
                if (compStudiable != null && compStudiable.Completed)
                {
                    compStudiable.studyEnabled = false;
                    b = true;
                }
                //
                if (compHoldingPlatformTarget != null)
                {
                    compHoldingPlatformTarget.containmentMode = EntityContainmentMode.MaintainOnly;
                    b = b || compStudiable != null;
                }

                if (b) Messages.Message("ResearchWhateverNothingLeftToResearch".Translate(studiedThing.Label).CapitalizeFirst(), new TargetInfo(studiedThing.PositionHeld, studiedThing.MapHeld, false), MessageTypeDefOf.NeutralEvent);

                return;
            }
            projs.SortBy(x => x.CostApparent - x.ProgressApparent);
            proj = projs.First();
            projs.TryRandomElementByWeight(x => x.CostApparent == proj.CostApparent ? 1f : 0f, out proj);

            SoundDefOf.ResearchStart.PlayOneShotOnCamera(null);
            researchManager.SetCurrentProject(proj);
            Thing thing = studiedThing;
            if (thing.Map == null) thing = thing.ParentHolder as Thing;
            TutorSystem.Notify_Event("StartResearchProject");
            Messages.Message("ResearchWhateverNewResearch".Translate(studier.Name.ToStringFull, proj.label).CapitalizeFirst(), new TargetInfo(thing.Position, thing.Map, false), MessageTypeDefOf.SilentInput);
        }
    }
}
