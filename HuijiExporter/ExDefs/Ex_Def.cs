using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Newtonsoft.Json.Linq;
using HuijiExporter.Utility;

namespace HuijiExporter.ExDefs {
    class Ex_Def {
        public virtual string DefType => typeof(Def).Name;

        public virtual void Export() {
            // Instance
            List<JObject> allDatas = new List<JObject>();
            List<string> allDefs = new List<string>();
            foreach (Def curDef in DefDatabase<Def>.AllDefsListForReading) {
                JObject curData = new JObject(
                    new JProperty("defType", this.DefType),
                    new JProperty("defName", curDef.defName),
                    new JProperty("label", curDef.label),
                    new JProperty("description", curDef.description)
                );
                allDatas.Add(curData);
                allDefs.Add($"{this.DefType}_{curDef.defName}.json");
                using (StreamWriter sw = new StreamWriter(Path.Combine(Controller.Path_Defs, $"{this.DefType}_{curDef.defName}.json"))) {
                    string text = curData.ToString();
                    sw.Write(text);
                }
            }
            // Collection
            using (StreamWriter sw = new StreamWriter(Path.Combine(Controller.Path_Defs, $"{this.DefType}.json"))) {
                JObject collection = new JObject(
                    new JProperty("collection", this.DefType),
                    new JProperty("allDefs", allDefs),
                    new JProperty("charts",
                        new JObject(
                            new JProperty("compare", GenerateCompare())
                        )
                    )
                );
                sw.Write(collection.ToString());
            }
            Log.Error($"{Controller.Identifier} The class '{GetType().Name}' does not realized method '{System.Reflection.MethodBase.GetCurrentMethod().Name}()'.");
        }

        #region ECharts Data Generators

        protected virtual JObject GenerateCompare() {
            return new JObject();
        }

        #endregion
    }
}
