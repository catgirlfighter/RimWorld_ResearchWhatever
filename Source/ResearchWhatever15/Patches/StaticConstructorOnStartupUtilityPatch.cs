﻿using System;
using System.Linq;
using System.Collections.Generic;
using Verse;
using HarmonyLib;
using System.Reflection;
using RimWorld;


namespace ResearchWhatever
{
    public static class StaticConstructorOnStartupUtility_Patch
    {
        //[HarmonyPatch(typeof(StaticConstructorOnStartupUtility), "CallAll")]
        [HarmonyPatch]
        public static class StaticConstructorOnStartupUtility_CallAll_ResearchWhateverPatch
        {
            internal static MethodBase TargetMethod()
            {
                MethodBase LCallAll = AccessTools.Method("BetterLoading.Stage.InitialLoad.StageRunStaticCctors:PreCallAll");
                if (LCallAll == null)
                {
                    LCallAll = AccessTools.Method("Verse.StaticConstructorOnStartupUtility:CallAll");
                    if (LCallAll == null)
                        throw new Exception("Couldn't find StaticConstructorOnStartupUtility.CallAll()");
                }
                else
                    Log.Message("[ResearchWhatever] BetterLoading detected, workaround initiated");
                return LCallAll;
            }
            //
            public static void Postfix()
            {
                List<ThingDef> list = new List<ThingDef>(
                    from x in DefDatabase<ThingDef>.AllDefsListForReading
                    where x.thingClass == typeof(Building_ResearchBench) || x.thingClass != null && x.thingClass.IsSubclassOf(typeof(Building_ResearchBench))
                    select x);

                if (list.NullOrEmpty())
                    return;
                else
                    foreach (var thing in list)
                    {
                        thing?.comps?.Add(new CompProperties(typeof(ResearchWhateverComp)));
                    }
            }
        }
    }
}
