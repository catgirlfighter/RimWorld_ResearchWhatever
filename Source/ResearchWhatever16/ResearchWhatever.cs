using HarmonyLib;
using System.Reflection;
using Verse;
using RimWorld;

namespace ResearchWhatever
{
    [StaticConstructorOnStartup]
    public class ResearchWhatever : Mod
    {
        public ResearchWhatever(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("net.avilmask.rimworld.mod.ResearchWhatever");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
