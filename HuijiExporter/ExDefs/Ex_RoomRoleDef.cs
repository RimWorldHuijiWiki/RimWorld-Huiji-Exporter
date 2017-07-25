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
    class Ex_RoomRoleDef : Ex_Def {
        public override string DefType => typeof(RoomRoleDef).Name;

        public override void Export() {
            // Instance 实例
            List<JObject> allDatas = new List<JObject>();
            List<string> allDefs = new List<string>();
            foreach (RoomRoleDef curDef in DefDatabase<RoomRoleDef>.AllDefsListForReading) {
                JObject curData = new JObject(
                    new JProperty("defType", this.DefType),
                    new JProperty("defName", curDef.defName),
                    new JProperty("label", curDef.label),
                    //new JProperty("description", curDef.description)
                    new JProperty("relatedStats", from s in DefDatabase<RoomStatDef>.AllDefsListForReading
                                                  where curDef.IsStatRelated(s)
                                                  select new JObject(
                                                      new JProperty("defName", s.defName),
                                                      new JProperty("name", s.label)
                                                  ))

                );
                allDatas.Add(curData);
                allDefs.Add($"{this.DefType}_{curDef.defName}.json");
                using (StreamWriter sw = new StreamWriter(Path.Combine(Controller.Path_Defs, $"{this.DefType}_{curDef.defName}.json"))) {
                    string text = curData.ToString();
                    sw.Write(text);
                }
            }
            // Collection 集合
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
        }
    }
}
