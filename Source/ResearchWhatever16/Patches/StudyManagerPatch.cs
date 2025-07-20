using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        //private static readonly PropertyInfo LStudyCompleted = AccessTools.Property(typeof(ITab_StudyNotes), "StudyCompleted");

        internal static bool CheckForBullshit(Thing studiedThing)
        {
            return (!(studiedThing is UnnaturalCorpse)) || (((UnnaturalCorpse)studiedThing).Tracker.CanDestroyViaResearch);
            //return !(studiedThing UnnaturalCorpse) ((UnnaturalCorpse)studiedThing).tarcker
        }

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
                var compStudiable = studiedThing.TryGetComp<CompStudiable>();
                CompHoldingPlatformTarget compHoldingPlatformTarget = studiedThing.TryGetComp<CompHoldingPlatformTarget>();
                bool unlockComplete = false;
                bool studiableComplete = false;
                bool buillshitComplete = CheckForBullshit(studiedThing);
                //bool tabComplete = false;
                //I'm so pissed, why the fuck research progress has so many channels, and only fucking one that unites them ALL is a fucking TAB??? (ITab_StudyNotes) WHAT THE FUCK???
                //What the fuck am I supposed to do to take data from that scuffed thing without VISUALLY selecting shit? Fuck me!
                //var tabType = studiedThing.def.inspectorTabs.FirstOrDefault(x => x.SameOrSubclassOf<ITab_StudyNotes>());

                var compStudyUnlocks = studiedThing.TryGetComp<CompStudyUnlocks>();
                //Log.Message($"unlocks = {compStudyUnlocks}, completed = {compStudyUnlocks?.Completed}");
                if (compStudyUnlocks == null || compStudyUnlocks.Completed)
                {
                    //compStudiable.studyEnabled = false;
                    unlockComplete = true;
                }

                if (compStudiable != null)
                {
                    //Log.Message($"studiable, {compStudiable.studyPoints}/{compStudiable.Props.studyAmountToComplete} completable = {compStudiable.Props.Completable}, completed = {compStudiable.Completed}");
                    if (!compStudiable.Props.Completable || compStudiable.Completed)
                        {
                        //compStudiable.studyEnabled = false;
                        studiableComplete = true;
                        }
                    //    else { }
                    //else
                    //{
                    //}
                    if (buillshitComplete && unlockComplete && studiableComplete) compStudiable.studyEnabled = false;
                }

                if (buillshitComplete && unlockComplete && studiableComplete)
                {
                    if (compHoldingPlatformTarget?.containmentMode == EntityContainmentMode.Study) compHoldingPlatformTarget.containmentMode = EntityContainmentMode.MaintainOnly;
                    Messages.Message("ResearchWhateverNothingLeftToResearch".Translate(studiedThing.Label).CapitalizeFirst(), new TargetInfo(studiedThing.PositionHeld, studiedThing.MapHeld, false), MessageTypeDefOf.NeutralEvent);
                }

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
