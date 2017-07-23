using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace HuijiExporter {
    [StaticConstructorOnStartup]
    public class Controller {
        static Controller() {
            LongEventHandler.ExecuteWhenFinished(ExportAll);
        }

        public static readonly string Identifier = "[RimWorld-Huiji-Exporter]";
        public static readonly string Path_Defs = @"D:\Git\RW\RimWorld-Huiji-Data\Defs";

        static void ExportAll() {
            Log.Message(Identifier);
            if (!Directory.Exists(Path_Defs)) {
                Directory.CreateDirectory(Path_Defs);
            }
            foreach (Type t_SubEx in typeof(ExDefs.Ex_Def).AllSubclasses()) {
                var curSubEx = t_SubEx.Assembly.CreateInstance(t_SubEx.FullName) as ExDefs.Ex_Def;
                Log.Message($"{Identifier} Exporting {curSubEx.DefType}");
                curSubEx.Export();
            }

            // Debug
            // Test();
        }

        static void Test() {
            Utility.RainbowUtility.RainbowHexDemo();
        }
    }
}
