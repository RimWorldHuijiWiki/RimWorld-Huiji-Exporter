﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;
using Newtonsoft.Json.Linq;
using HuijiExporter.Utility;

namespace HuijiExporter.ExDefs {
    class Ex_BiomeDef : Ex_Def {
        public override string DefType => typeof(BiomeDef).Name;

        public override void Export() {
            // Instance
            List<JObject> allDatas = new List<JObject>();
            List<string> allDefs = new List<string>();
            foreach (BiomeDef curDef in DefDatabase<BiomeDef>.AllDefsListForReading) {
                JObject curData = new JObject(
                    new JProperty("defType", this.DefType),
                    new JProperty("defName", curDef.defName),
                    new JProperty("label", curDef.label),
                    new JProperty("description", curDef.description),
                    new JProperty("animalDensity", curDef.animalDensity),
                    new JProperty("plantDensity", curDef.plantDensity),
                    new JProperty("diseaseMtbDays", curDef.diseaseMtbDays),
                    new JProperty("hasVirtualPlants", curDef.hasVirtualPlants),
                    new JProperty("animalWeights", GetAnimalWeights(curDef)),
                    new JProperty("plantWeights", GetPlantWeights(curDef)),
                    new JProperty("diseaseWeights", GetDiseaseWeights(curDef)),
                    new JProperty("texture", TextureUtility.PathToFile(curDef.texture))
                );
                allDatas.Add(curData);
                allDefs.Add($"{this.DefType}_{curDef.defName}.json");
                using (StreamWriter sw = new StreamWriter(Path.Combine(Controller.Path_Defs, $"{this.DefType}_{curDef.defName}.json"))) {
                    sw.Write(curData.ToString());
                }
            }
            // Collection
            using (StreamWriter sw = new StreamWriter(Path.Combine(Controller.Path_Defs, $"{this.DefType}.json"))) {
                JObject collection = new JObject(
                    new JProperty("defCollection", this.DefType),
                    new JProperty("allDefs", allDefs),
                    new JProperty("charts",
                        new JObject(
                            new JProperty("map", GenerateMap()),
                            new JProperty("compare", GenerateCompare())
                        )
                    )
                );
                sw.Write(collection.ToString());
            }
        }

        #region Methods

        static IEnumerable<JObject> GetAnimalWeights(BiomeDef biome) {
            List<JObject> result = new List<JObject>();
            foreach (PawnKindDef curAnimal in biome.AllWildAnimals) {
                result.Add(new JObject(
                    new JProperty("defName", curAnimal.defName),
                    new JProperty("name", curAnimal.label),
                    new JProperty("value", biome.CommonalityOfAnimal(curAnimal) / curAnimal.wildSpawn_GroupSizeRange.Average)
                ));
            }
            return result.Count > 0 ? result : null;
        }

        static IEnumerable<JObject> GetPlantWeights(BiomeDef biome) {
            List<JObject> result = new List<JObject>();
            foreach (ThingDef curPlant in biome.AllWildPlants) {
                result.Add(new JObject(
                    new JProperty("defName", curPlant.defName),
                    new JProperty("name", curPlant.label),
                    new JProperty("value", biome.CommonalityOfPlant(curPlant))
                ));
            }
            return result.Count > 0 ? result : null;
        }

        static IEnumerable<JObject> GetDiseaseWeights(BiomeDef biome) {
            List<JObject> result = new List<JObject>();
            foreach (IncidentDef curDisease in from inc in DefDatabase<IncidentDef>.AllDefs
                                               where inc.diseaseIncident != null
                                               select inc) {
                float commonality = biome.CommonalityOfDisease(curDisease);
                if (commonality > 0) {
                    result.Add(new JObject(
                    new JProperty("defName", curDisease.diseaseIncident.defName),
                    new JProperty("name", curDisease.diseaseIncident.label),
                    new JProperty("value", commonality)
                    ));
                }
            }
            return result.Count > 0 ? result : null;
        }

        #endregion

        #region ECharts Data Generators

        /// <summary>
        /// Distribution map of all biomes.
        /// 所有生态区的平均温度与降雨量分布图。
        /// </summary>
        /// <returns></returns>
        protected JObject GenerateMap() {
            List<BiomeDef> allBiomes = DefDatabase<BiomeDef>.AllDefsListForReading;
            // { AridShrubland, Desert, ExtremeDesert, BorealForest, Tundra, IceSheet, SeaIce, TemperateForest, TropicalRainforest, Ocean, Lake }
            // { "#896647", "#9a7a56", "#9e815f", "#64522e", "#836a59", "#a09fa0", "#939394", "#766c39", "#656633", "#3c475c", "#3c475c"}
            Dictionary<BiomeDef, string> colors = new Dictionary<BiomeDef, string> {
                { BiomeDef.Named("AridShrubland"), "#896647" },
                { BiomeDef.Named("Desert"), "#9a7a56" },
                { BiomeDef.Named("ExtremeDesert"), "#9e815f" },
                { BiomeDef.Named("BorealForest"), "#64522e" },
                { BiomeDef.Named("Tundra"), "#836a59" },
                { BiomeDef.Named("IceSheet"), "#a09fa0" },
                { BiomeDef.Named("SeaIce"), "#939394" },
                { BiomeDef.Named("TemperateForest"), "#766c39" },
                { BiomeDef.Named("TropicalRainforest"), "#656633" },
                { BiomeDef.Named("Ocean"), "#3c475c" },
                { BiomeDef.Named("Lake"), "#3c475c" },
            };
            List<JObject> visualMap_pieces = new List<JObject>();
            for (int i = 0; i < allBiomes.Count; i++) {
                visualMap_pieces.Add(new JObject(
                    new JProperty("value", i),
                    new JProperty("label", allBiomes[i].label),
                    new JProperty("color", colors[allBiomes[i]])
                ));
            }
            List<float> x1_data = new List<float>();
            for (int temperature = -40; temperature < 60; temperature++) {
                x1_data.Add(temperature);
            }
            List<float> y1_data = new List<float>();
            for (int rainfall = 0; rainfall < 4000; rainfall += 40) {
                y1_data.Add(rainfall);
            }
            List<JArray> s1_data = new List<JArray>();
            Tile ws = new Tile() { elevation = 1f };
            foreach (float temperature in x1_data) {
                ws.temperature = temperature;
                foreach (float rainfall in y1_data) {
                    ws.rainfall = rainfall;
                    BiomeDef biomeDef = null;
                    float num = 0f;
                    for (int i = 0; i < allBiomes.Count; i++) {
                        BiomeDef biomeDef2 = allBiomes[i];
                        if (biomeDef2.implemented) {
                            float score = biomeDef2.Worker.GetScore(ws);
                            if (score > num || biomeDef == null) {
                                biomeDef = biomeDef2;
                                num = score;
                            }
                        }
                    }
                    s1_data.Add(new JArray(x1_data.IndexOf(temperature), y1_data.IndexOf(rainfall), allBiomes.IndexOf(biomeDef)));
                }
            }

            return new JObject(
                new JProperty("visualMap", new JObject(new JProperty("pieces", visualMap_pieces))),
                new JProperty("xAxis", new JArray(new JObject(new JProperty("data", x1_data)))),
                new JProperty("yAxis", new JArray(new JObject(new JProperty("data", y1_data)))),
                new JProperty("series", new JArray(new JObject(new JProperty("data", s1_data))))
            );
        }

        /// <summary>
        /// Comparison for all biomes.
        /// 所有生态区的对比。
        /// </summary>
        /// <returns></returns>
        protected override JObject GenerateCompare() {
            List<BiomeDef> allBiomes = DefDatabase<BiomeDef>.AllDefsListForReading;
            List<string> x1_data = new List<string>();
            List<float> animalDensity_data = new List<float>();
            List<float> plantDensity_data = new List<float>();
            List<float> diseaseMtbDays_data = new List<float>();
            foreach (BiomeDef curBiome in allBiomes) {
                x1_data.Add(curBiome.label);
                animalDensity_data.Add(curBiome.animalDensity);
                plantDensity_data.Add(curBiome.plantDensity);
                diseaseMtbDays_data.Add(curBiome.diseaseMtbDays);
            }

            return new JObject(
                new JProperty("color", RainbowUtility.RainbowHex(4)),
                new JProperty("xAxis", new JArray(new JObject(new JProperty("data", x1_data)))),
                new JProperty("series", new JArray(
                    new JObject(new JProperty("data", animalDensity_data)),
                    new JObject(new JProperty("data", plantDensity_data)),
                    new JObject(new JProperty("data", diseaseMtbDays_data))
                ))
            );
        }

        #endregion
    }
}
