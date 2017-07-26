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
    class Ex_WeatherDef : Ex_Def {
        public override string DefType => typeof(WeatherDef).Name;

        public override void Export() {
            // Instance
            List<JObject> allDatas = new List<JObject>();
            List<string> allDefs = new List<string>();
            foreach (WeatherDef curDef in DefDatabase<WeatherDef>.AllDefsListForReading) {
                JObject curData = new JObject(
                    new JProperty("defType", this.DefType),
                    new JProperty("defName", curDef.defName),
                    new JProperty("label", curDef.label),
                    new JProperty("description", curDef.description),
                    new JProperty("windSpeedFactor", curDef.windSpeedFactor),
                    new JProperty("moveSpeedMultiplier", curDef.moveSpeedMultiplier),
                    new JProperty("accuracyMultiplier", curDef.accuracyMultiplier)
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
        }

        #region ECharts Data Generators

        /// <summary>
        ///  Comparison for all weathers.
        ///  所有天气的对比。
        /// </summary>
        /// <returns></returns>
        protected override JObject GenerateCompare() {
            List<WeatherDef> allWeather = DefDatabase<WeatherDef>.AllDefsListForReading;
            List<string> x0_data = new List<string>();
            List<float> windSpeedFactor_data = new List<float>();
            List<float> moveSpeedMultiplier_data = new List<float>();
            List<float> accuracyMultiplier_data = new List<float>();
            foreach (WeatherDef curWeather in allWeather) {
                x0_data.Add(curWeather.label); // 天气名称
                windSpeedFactor_data.Add(curWeather.windSpeedFactor); // 风速系数
                moveSpeedMultiplier_data.Add(curWeather.moveSpeedMultiplier); // 移动速度乘数
                accuracyMultiplier_data.Add(curWeather.accuracyMultiplier); // 射击精度乘数
            }

            return new JObject(
                new JProperty("color", RainbowUtility.RainbowHex(4)),
                new JProperty("xAxis", new JArray(new JObject(new JProperty("data", x0_data)))),
                new JProperty("series", new JArray(
                    new JObject(new JProperty("data", windSpeedFactor_data)),
                    new JObject(new JProperty("data", moveSpeedMultiplier_data)),
                    new JObject(new JProperty("data", accuracyMultiplier_data))
                ))
            );
        }

        #endregion
    }
}
