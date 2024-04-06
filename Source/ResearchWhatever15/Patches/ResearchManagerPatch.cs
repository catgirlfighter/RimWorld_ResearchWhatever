using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;

namespace ResearchWhatever.Patches
{
    [HarmonyPatch(typeof(ResearchManager), "FinishProject")]
    public static class ResearchManager_FinishProject_ResearchWhateverPatch
    {
        public static void Prefix(ResearchManager __instance, ref bool doCompletionDialog)
        {
            if (doCompletionDialog)
            {
                var comp = Current.Game.GetComponent<ResearchWhateverGameComp>();
                if (comp.NotifyMode == ResearchWhateverNotifyMode.rwnDefault)
                    return;

                doCompletionDialog = false;

                var currentProj = __instance.GetProject();
                switch (comp.NotifyMode)
                {
                    case ResearchWhateverNotifyMode.rwnLetter: 
                        Find.LetterStack.ReceiveLetter("ResearchFinished".Translate(currentProj.LabelCap), currentProj.description, LetterDefOf.PositiveEvent, null, null, null, null, null);
                        break;
                    case ResearchWhateverNotifyMode.rwnNotice:
                        Messages.Message("ResearchFinished".Translate(currentProj.LabelCap).CapitalizeFirst(), MessageTypeDefOf.SilentInput);
                        break;
                    default:
                        break;
                } 
            }
        }
    }
}
