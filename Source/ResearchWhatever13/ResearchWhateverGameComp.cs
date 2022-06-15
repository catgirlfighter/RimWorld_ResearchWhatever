using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace ResearchWhatever
{
    public enum ResearchWhateverNotifyMode { rwnDefault, rwnLetter, rwnNotice, rwnMute }
    public class ResearchWhateverGameComp : GameComponent
    {
        public ResearchWhateverGameComp()
        {
        }
        public ResearchWhateverGameComp(Game game)
        {
        }
        public override void ExposeData()
        {
            Scribe_Values.Look(ref notifyMode, "NotifyMode");
        }
        private ResearchWhateverNotifyMode notifyMode = ResearchWhateverNotifyMode.rwnDefault;
        public ResearchWhateverNotifyMode NotifyMode { get { return notifyMode; } set { notifyMode = value; } }
    }
}
