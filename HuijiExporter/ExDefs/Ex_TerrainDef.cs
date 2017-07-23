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
    class Ex_TerrainDef : Ex_Def {
        public override string DefType => typeof(TerrainDef).Name;

        public override void Export() {
            // Instance
            List<JObject> allDatas = new List<JObject>();
            List<string> allDefs = new List<string>();
            foreach (TerrainDef curDef in DefDatabase<TerrainDef>.AllDefsListForReading) {
                JObject curData = new JObject(
                    new JProperty("defType", this.DefType),
                    new JProperty("defName", curDef.defName),
                    new JProperty("label", curDef.label),
                    new JProperty("description", curDef.description),
                    new JProperty("statBases", new JArray(
                        GetStat(curDef, StatDefOf.MarketValue),
                        GetStat(curDef, StatDefOf.WorkToBuild),
                        GetStat(curDef, StatDefOf.Beauty),
                        GetStat(curDef, StatDefOf.Cleanliness),
                        GetStat(curDef, StatDefOf.Flammability)
                    )),
                    new JProperty("passability", curDef.passability),
                    new JProperty("walkSpeed", (curDef.passability == Traversability.Impassable) ? 0f : (13f / curDef.pathCost + 13f)),
                    new JProperty("fertility", curDef.fertility),
                    new JProperty("costList", GetCostList(curDef)),
                    new JProperty("researchPrerequisites", GetResearchPrerequisites(curDef)),
                    new JProperty("resourcesFractionWhenDeconstructed", curDef.resourcesFractionWhenDeconstructed),
                    new JProperty("texture", TextureUtility.PathToFile(curDef.texturePath)),
                    new JProperty("layerable", curDef.layerable),
                    new JProperty("color", curDef.color.ToStringHexExceptWhite()),
                    new JProperty("driesTo", curDef.driesTo?.label)

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

        #region Methods

        static JObject GetStat(TerrainDef terrain, StatDef stat) {
            return new JObject(
                new JProperty("defName", stat.defName),
                new JProperty("name", stat.label),
                new JProperty("value", terrain.GetStatValueAbstract(stat))
            );
        }

        static IEnumerable<JObject> GetCostList(TerrainDef terrain) {
            if (terrain.costList != null) {
                List<JObject> result = new List<JObject>();
                foreach (ThingCountClass thingCount in terrain.costList) {
                    result.Add(new JObject(
                        new JProperty("defName", thingCount.thingDef.defName),
                        new JProperty("name", thingCount.thingDef.label),
                        new JProperty("value", thingCount.count)
                    ));
                }
                return result;
            }
            return null;
        }

        static IEnumerable<JObject> GetResearchPrerequisites(TerrainDef terrain) {
            if (terrain.researchPrerequisites != null) {
                List<JObject> result = new List<JObject>();
                foreach (ResearchProjectDef research in terrain.researchPrerequisites) {
                    result.Add(new JObject(
                        new JProperty("defName", research.defName),
                        new JProperty("label", research.label)
                    ));
                }
                return result;
            }
            return null;
        }

        #endregion

        #region ECharts Data Generators

        protected override JObject GenerateCompare() {
            List<string> y1_data = new List<string>();
            List<float> MarketValue_data = new List<float>();
            List<float> fertility_data = new List<float>();
            List<float> Cleanliness_data = new List<float>();
            List<float> Beauty_data = new List<float>();
            foreach (TerrainDef curTerrain in DefDatabase<TerrainDef>.AllDefsListForReading) {
                y1_data.Add(curTerrain.label);
                MarketValue_data.Add(curTerrain.GetStatValueAbstract(StatDefOf.MarketValue));
                fertility_data.Add(curTerrain.fertility);
                Cleanliness_data.Add(curTerrain.GetStatValueAbstract(StatDefOf.Cleanliness));
                Beauty_data.Add(curTerrain.GetStatValueAbstract(StatDefOf.Beauty));
            }

            return new JObject(
                new JProperty("color", RainbowUtility.RainbowHex(4)),
                new JProperty("yAxis", new JArray(new JObject(new JProperty("data", y1_data)))),
                new JProperty("series", new JArray(
                    new JObject(
                        new JProperty("name", StatDefOf.MarketValue.label),
                        new JProperty("data", MarketValue_data)
                    ),
                    new JObject(
                        new JProperty("name", "Fertility".Translate()),
                        new JProperty("data", fertility_data)
                    ),
                    new JObject(
                        new JProperty("name", StatDefOf.Cleanliness.label),
                        new JProperty("data", Cleanliness_data)
                    ),
                    new JObject(
                        new JProperty("name", StatDefOf.Beauty.label),
                        new JProperty("data", Beauty_data)
                    )
                ))
            );
        }

        #endregion
    }
}
