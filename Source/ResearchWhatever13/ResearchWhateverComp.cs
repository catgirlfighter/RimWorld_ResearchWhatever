using System.Collections.Generic;
using Verse;
using RimWorld;

namespace ResearchWhatever
{
    public class ResearchWhateverComp : ThingComp
    {
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                if (value == active)
                {
                    return;
                }
                this.active = value;
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref active, "active", true, false);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Command_Toggle command_Toggle = new Command_Toggle();
            command_Toggle.hotKey = KeyBindingDefOf.Command_TogglePower;
            command_Toggle.defaultLabel = "CommandResearchWhateverToggleLabel".Translate();
            command_Toggle.icon = TexCommand.OpenLinkedQuestTex;
            command_Toggle.isActive = (() => this.Active);
            command_Toggle.toggleAction = delegate ()
            {
                this.Active = !this.Active;
            };
            if (this.Active)
            {
                command_Toggle.defaultDesc = "CommandResearchWhateverToggleDescActive".Translate();
            }
            else
            {
                command_Toggle.defaultDesc = "CommandResearchWhateverToggleDescInactive".Translate();
            }
            yield return command_Toggle;
            yield break;
        }

        private bool active = true;
    }
    
}
