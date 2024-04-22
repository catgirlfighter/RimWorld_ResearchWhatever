using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;

namespace ResearchWhatever.Patches
{
    [HarmonyPatch(typeof(ResearchManager), "FinishProject")]
    static class ResearchManager_FinishProject_ResearchWhateverPatch
    {
        internal static void Prefix(ResearchProjectDef proj, ref bool doCompletionDialog)
        {
            if (doCompletionDialog)
            {
                var comp = Current.Game.GetComponent<ResearchWhateverGameComp>();
                if (comp.NotifyMode == ResearchWhateverNotifyMode.rwnDefault)
                    return;

                doCompletionDialog = false;

                //var currentProj = __instance.GetProject();
                switch (comp.NotifyMode)
                {
                    case ResearchWhateverNotifyMode.rwnLetter:
                        Find.LetterStack.ReceiveLetter("ResearchFinished".Translate(proj.LabelCap), proj.description, LetterDefOf.PositiveEvent, null, null, null, null, null);
                        break;
                    case ResearchWhateverNotifyMode.rwnNotice:
                        Messages.Message("ResearchFinished".Translate(proj.LabelCap).CapitalizeFirst(), MessageTypeDefOf.SilentInput);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
