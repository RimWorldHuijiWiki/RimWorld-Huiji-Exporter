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
    class Ex_ResearchProjectDef : Ex_Def {
        public override string DefType => typeof(ResearchProjectDef).Name;

        public override void Export() {
            // Instance 实例
            List<JObject> allDatas = new List<JObject>();
            List<string> allDefs = new List<string>();
            foreach (ResearchProjectDef curDef in DefDatabase<ResearchProjectDef>.AllDefsListForReading) {
                JObject curData = new JObject(
                    new JProperty("defType", this.DefType),
                    new JProperty("defName", curDef.defName),
                    new JProperty("label", curDef.label),
                    new JProperty("description", curDef.description),
                    new JProperty("techLevel", curDef.techLevel),
                    new JProperty("techLevelLabel", curDef.techLevel.ToStringHuman()),
                    new JProperty("baseCost", curDef.baseCost),
                    //new JProperty("prerequisites", curDef.prerequisites != null && curDef.prerequisites.Count > 0 ? from p in curDef.prerequisites
                    //                                                                                                select p.defName
                    //                                                                                              : null),
                    //new JProperty("requiredByThis", curDef.requiredByThis != null && curDef.requiredByThis.Count > 0 ? from r in curDef.requiredByThis
                    //                                                                                                   select r
                    //                                                                                                 : null),
                    //new JProperty("requiredResearchBuilding", new JObject(
                    //    new JProperty("defName", curDef.requiredResearchBuilding.defName),
                    //    new JProperty("name", curDef.requiredResearchBuilding.label)
                    //)),
                    //new JProperty("requiredResearchFacilities", curDef.requiredResearchFacilities != null && curDef.requiredResearchFacilities.Count > 0 ? from f in curDef.requiredResearchFacilities
                    //                                                                                                                                       select new JObject(
                    //                                                                                                                                           new JProperty("defName", f.defName),
                    //                                                                                                                                           new JProperty("name", f.label)
                    //                                                                                                                                       )
                    //                                                                                                                                     : null
                    //),
                    new JProperty("tags", curDef.tags),
                    new JProperty("researchViewX", curDef.researchViewX),
                    new JProperty("researchViewY", curDef.researchViewY)
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
                            new JProperty("compare", GenerateCompare()),
                            new JProperty("graph", GenerateGraph())
                        )
                    )
                );
                sw.Write(collection.ToString());
            }
        }

        #region Methods

        static float GetCostFactor(TechLevel projectTechLevel, TechLevel factionTechLevel) {
            if (factionTechLevel >= projectTechLevel) {
                return 1f;
            }
            return 1f + (float)(projectTechLevel - factionTechLevel);
        }
        
        static string GetExplain(ResearchProjectDef research) {
            StringBuilder sb = new StringBuilder();
            sb.Append(research.LabelCap);
            sb.Append("<br/><br/>");

            if (research.tags != null) {
                foreach (string curTag in research.tags) {
                    switch (curTag) {
                        case "TribalStart":
                            sb.Append("（部落开局科技）");
                            break;
                        case "ClassicStart":
                            sb.Append("（殖民地开局科技）");
                            break;
                        default:
                            break;
                    }
                }
                sb.Append("<br/>");
            }
            sb.Append(research.description.Wrap());
            sb.Append("<br/><br/>");
            
            sb.Append("ProjectTechLevel".Translate().CapitalizeFirst());
            sb.Append("：");
            sb.Append(research.techLevel.ToStringHuman());
            sb.Append("<br/>");
            
            sb.Append("WorkAmount".Translate().CapitalizeFirst());
            sb.Append("：");
            sb.Append(research.baseCost.ToString());
            sb.Append("<br/>");
            
            if (research.requiredResearchBuilding != null) {
                sb.Append("RequiredResearchBench".Translate().CapitalizeFirst());
                sb.Append("：");
                sb.Append(research.requiredResearchBuilding.label);
                sb.Append("<br/>");
            }
            
            if (research.requiredResearchFacilities != null) {
                sb.Append("RequiredResearchBenchFacilities".Translate().CapitalizeFirst());
                sb.Append("：");
                sb.Append(string.Join("、", (from fac in research.requiredResearchFacilities
                                                select fac.label).ToArray()));
                sb.Append("<br/>");
            }
            
            if (research.prerequisites != null) {
                sb.Append("ResearchPrerequisites".Translate().CapitalizeFirst());
                sb.Append("：");
                sb.Append(string.Join("、", (from pre in research.prerequisites
                                            select pre.label).ToArray()));
                sb.Append("<br/>");
            }
            
            sb.Append("<br/>");
            
            var unlockBuildings = from bdg in DefDatabase<ThingDef>.AllDefs
                                  where bdg.category == ThingCategory.Building && bdg.researchPrerequisites != null && bdg.researchPrerequisites.Contains(research)
                                  select bdg.label;
            if (unlockBuildings.Count() > 0) {
                sb.Append(("解锁建筑：" + string.Join("、", unlockBuildings.ToArray())).Wrap());
                sb.Append("<br/>");
            }

            var unlockPlant = from plt in DefDatabase<ThingDef>.AllDefs
                              where plt.category == ThingCategory.Plant && plt.plant.sowResearchPrerequisites != null && plt.plant.sowResearchPrerequisites.Contains(research)
                              select plt.label;
            if (unlockPlant.Count() > 0) {
                sb.Append(("解锁植物：" + string.Join("、", unlockPlant.ToArray())).Wrap());
                sb.Append("<br/>");
            }

            var unlockTerrains = from trn in DefDatabase<TerrainDef>.AllDefs
                                 where trn.researchPrerequisites != null && trn.researchPrerequisites.Contains(research)
                                 select trn.label;
            if (unlockTerrains.Count() > 0) {
                sb.Append(("解锁地面：" + string.Join("、", unlockTerrains.ToArray())).Wrap());
                sb.Append("<br/>");
            }
            
            var unlockRecipes = from rcp in DefDatabase<RecipeDef>.AllDefs
                                where rcp.researchPrerequisite == research
                                select rcp.label;
            if (unlockRecipes.Count() > 0) {
                sb.Append(("解锁配方：" + string.Join("、", unlockRecipes.ToArray())).Wrap());
                sb.Append("<br/>");
            }

            return sb.ToString();
        }

        #endregion

        #region ECharts Data Generators

        /// <summary>
        /// Generate research work amount multipliers.
        /// 生成研究工作量乘数。
        /// </summary>
        /// <returns></returns>
        protected override JObject GenerateCompare() {
            List<TechLevel> allTechLevels = new List<TechLevel>();
            foreach (TechLevel curLevel in Enum.GetValues(typeof(TechLevel))) {
                if (curLevel != TechLevel.Undefined) allTechLevels.Add(curLevel);
            }
            List<JObject> rows = new List<JObject>();
            foreach (TechLevel factionTechLevel in allTechLevels) {
                List<JObject> cols = new List<JObject>();
                foreach (TechLevel projectTechLevel in allTechLevels) {
                    cols.Add(new JObject(
                        new JProperty("level", projectTechLevel),
                        new JProperty("enum", projectTechLevel.ToString()),
                        new JProperty("name", projectTechLevel.ToStringHuman()),
                        new JProperty("factor", GetCostFactor(projectTechLevel, factionTechLevel))
                    ));
                }
                rows.Add(new JObject(
                    new JProperty("level", factionTechLevel),
                    new JProperty("enum", factionTechLevel.ToString()),
                    new JProperty("name", factionTechLevel.ToStringHuman()),
                    new JProperty("projects", cols)
                ));
            }

            return new JObject(
                new JProperty("factions", rows),
                new JProperty("color", RainbowUtility.RainbowHex(allTechLevels.Count))
            );
        }

        /// <summary>
        /// Generate the graph for all research projects。
        /// 生成所有研究项目的关系图。
        /// </summary>
        /// <returns></returns>
        protected JObject GenerateGraph() {
            List<JObject> s1_data = new List<JObject>();
            List<JObject> s1_links = new List<JObject>();
            foreach (ResearchProjectDef curResearch in DefDatabase<ResearchProjectDef>.AllDefsListForReading) {
                s1_data.Add(new JObject(
                    new JProperty("defName", curResearch.defName),
                    new JProperty("name", curResearch.label),
                    new JProperty("x", curResearch.researchViewX),
                    new JProperty("y", curResearch.researchViewY),
                    new JProperty("tooltip", new JObject(
                        new JProperty("formatter", GetExplain(curResearch))
                    ))
                ));
                if (curResearch.prerequisites != null) {
                    foreach (ResearchProjectDef curPre in curResearch.prerequisites) {
                        s1_links.Add(new JObject(
                            new JProperty("source", curPre.label),
                            new JProperty("target", curResearch.label)
                        ));
                    }
                }
            }

            return new JObject(
                new JProperty("series", new JArray(new JObject(
                    new JProperty("data", s1_data),
                    new JProperty("links", s1_links)
                )))
            );
        }

        #endregion
    }
}
