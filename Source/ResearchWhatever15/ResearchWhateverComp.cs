using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace ResearchWhatever
{
    public class ResearchWhateverComp : ThingComp
    {
        public bool Active
        {
            get { return parent?.Faction == Faction.OfPlayer && active; }
            set { if (value == active) return; active = value; }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref active, "active", true, false);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent?.Faction != Faction.OfPlayer)
                yield break;
            //
            Command_Toggle command_Toggle = new Command_Toggle();
            command_Toggle.hotKey = KeyBindingDefOf.Command_TogglePower;
            command_Toggle.defaultLabel = "CommandResearchWhateverToggleLabel".Translate();
            command_Toggle.icon = TexCommand.OpenLinkedQuestTex;
            command_Toggle.isActive = (() => active);
            command_Toggle.toggleAction = delegate ()
            {
                if (Event.current.button == 0)
                    Active = !Active;
                else if (Event.current.button == 1)
                {
                    var comp = Current.Game.GetComponent<ResearchWhateverGameComp>();
                    //var lable = Mute ? "CommandResearchWhateverToggleMute".Translate().CapitalizeFirst() : "CommandResearchWhateverToggleUnmute".Translate().CapitalizeFirst();
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    list.Add(new FloatMenuOption("CommandResearchWhateverToggleDefault".Translate().CapitalizeFirst(), delegate ()
                    {
                        comp.NotifyMode = ResearchWhateverNotifyMode.rwnDefault;
                    }));
                    list.Add(new FloatMenuOption("CommandResearchWhateverToggleLetter".Translate().CapitalizeFirst(), delegate ()
                    {
                        comp.NotifyMode = ResearchWhateverNotifyMode.rwnLetter;
                    }));
                    list.Add(new FloatMenuOption("CommandResearchWhateverToggleNotice".Translate().CapitalizeFirst(), delegate ()
                    {
                        comp.NotifyMode = ResearchWhateverNotifyMode.rwnNotice;
                    }));
                    list.Add(new FloatMenuOption("CommandResearchWhateverToggleMute".Translate().CapitalizeFirst(), delegate ()
                    {
                        comp.NotifyMode = ResearchWhateverNotifyMode.rwnMute;
                    }));
                    FloatMenu floatMenu = new FloatMenu(list);
                    floatMenu.vanishIfMouseDistant = true;
                    Find.WindowStack.Add(floatMenu);
                }
            };
            if (Active)
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
