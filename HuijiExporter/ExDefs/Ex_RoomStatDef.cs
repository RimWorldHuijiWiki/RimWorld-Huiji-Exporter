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
    class Ex_RoomStatDef : Ex_Def {
        public override string DefType => typeof(RoomStatDef).Name;

        public override void Export() {
            // Instance 实例
            List<JObject> allDatas = new List<JObject>();
            List<string> allDefs = new List<string>();
            foreach (RoomStatDef curDef in DefDatabase<RoomStatDef>.AllDefsListForReading) {
                JObject curData = new JObject(
                    new JProperty("defType", this.DefType),
                    new JProperty("defName", curDef.defName),
                    new JProperty("label", curDef.label),
                    //new JProperty("description", curDef.description)
                    new JProperty("isHidden", curDef.isHidden),
                    new JProperty("defaultScore", curDef.defaultScore),
                    new JProperty("scoreStages", GetScoreStages(curDef)),
                    new JProperty("color", curDef.scoreStages == null ? null : RainbowUtility.RainbowHex(curDef.scoreStages.Count))
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

        #region Methods

        static IEnumerable<JObject> GetScoreStages(RoomStatDef roomStat) {
            if (roomStat.scoreStages == null)
                return null;
            
            return from ss in roomStat.scoreStages
                   select new JObject(
                       new JProperty("name", ss.label),
                       new JProperty("minScore", ss.minScore)
                   );
        }

        #endregion

        #region ECharts Data Generators

        /// <summary>
        /// Generate data of hidden room stats those effected by cleanless.
        /// 生成受到清洁影响的隐藏房间属性。
        /// </summary>
        /// <returns></returns>
        protected override JObject GenerateCompare() {
            return new JObject(new JProperty("color", RainbowUtility.RainbowHex(4)));
        }

        #endregion
    }
}
