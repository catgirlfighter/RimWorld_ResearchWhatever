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

            proj = DefDatabase<ResearchProjectDef>.AllDefsListForReading.FirstOrDefault((ResearchProjectDef x) => x.CanStartNow && x.knowledgeCategory == knowledgeCategory);
            if (proj == null && knowledgeCategory.overflowCategory != null)
                proj = DefDatabase<ResearchProjectDef>.AllDefsListForReading.FirstOrDefault((ResearchProjectDef x) => x.CanStartNow && x.knowledgeCategory == knowledgeCategory.overflowCategory);

            if (proj == null)
            {
                //CompStudiable compStudiable = studiedThing.TryGetComp<CompStudiable>();
                //if (compStudiable == null)
                //    return;
                //
                //if (compStudiable.Completed) ;
                return;
            }
            SoundDefOf.ResearchStart.PlayOneShotOnCamera(null);
            researchManager.SetCurrentProject(proj);
            Thing thing = studiedThing;
            if (thing.Map == null) thing = thing.ParentHolder as Thing;
            TutorSystem.Notify_Event("StartResearchProject");
            Messages.Message("ResearchWhateverNewResearch".Translate(studier.Name.ToStringFull, proj.label).CapitalizeFirst(), new TargetInfo(thing.Position, thing.Map, false), MessageTypeDefOf.SilentInput);
        }
    }
}
