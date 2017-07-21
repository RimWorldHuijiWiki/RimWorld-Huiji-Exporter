using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld_Huiji_Exporter.Def;
using RimWorld;
using Verse;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace RimWorld_Huiji_Exporter {
    [StaticConstructorOnStartup]
    public class Controller {
        static Controller() {
            LongEventHandler.ExecuteWhenFinished(ExportAll);
        }

        public static readonly string Identifier = "[RimWorld-Huiji-Exporter]";

        public static void ExportAll() {
            //List<Ex_Def> allEx = new List<Ex_Def> {
            //    new Ex_BiomeDef()
            //};
            //foreach (Ex_Def curEx in allEx) {
            //    curEx.Export();
            //}
            foreach (Type t_Ex in typeof(Ex_Def).AllSubclasses()) {
                var curEx = t_Ex.Assembly.CreateInstance(t_Ex.FullName) as Ex_Def;
                curEx.Export();
            }
        }
    }
}
