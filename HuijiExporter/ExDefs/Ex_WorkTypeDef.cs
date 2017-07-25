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
    class Ex_WorkTypeDef : Ex_Def {
        public override string DefType => typeof(WorkTypeDef).Name;

        public override void Export() {
            // Instance
            List<JObject> allDatas = new List<JObject>();
            List<string> allDefs = new List<string>();
            foreach (WorkTypeDef curDef in WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder) {
                JObject curData = new JObject(
                    new JProperty("defType", this.DefType),
                    new JProperty("defName", curDef.defName),
                    new JProperty("label", curDef.label),
                    new JProperty("description", curDef.description),
                    new JProperty("workTags", from t in GetWorkTags(curDef)
                                              select t.LabelTranslated()),
                    new JProperty("relevantSkills", from s in curDef.relevantSkills
                                                    select s.label),
                    new JProperty("workGiversByPriority", from g in curDef.workGiversByPriority
                                                          select g.label)
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
                            new JProperty("compare", GenerateCompare()),
                            new JProperty("tagsTable", GenerateTagsTable())
                        )
                    )
                );
                sw.Write(collection.ToString());
            }
        }

        #region Methods

        static IEnumerable<WorkTags> GetWorkTags(WorkTypeDef work) {
            if (work.workTags == WorkTags.None) {
                yield return WorkTags.None;
            }
            foreach (WorkTags curTag in Enum.GetValues(typeof(WorkTags))) {
                if (curTag == WorkTags.None)
                    continue;
                if ((work.workTags & curTag) == curTag) {
                    yield return curTag;
                }
            }
        }

        static IEnumerable<WorkTags> GetWorkTags(WorkGiverDef giver) {
            if (giver.workTags == WorkTags.None) {
                yield return WorkTags.None;
            }
            foreach (WorkTags curTag in Enum.GetValues(typeof(WorkTags))) {
                if (curTag == WorkTags.None)
                    continue;
                if ((giver.workTags & curTag) == curTag) {
                    yield return curTag;
                }
            }
        }

        #endregion

        #region ECharts Data Generators

        /// <summary>
        /// Category tags table for all WorkTypeDefs, WorkGiverDefs and SkillDefs.
        /// 所有工作类型定义、工作分配器定义和技能定义的分类标签表。
        /// </summary>
        /// <returns></returns>
        protected JObject GenerateTagsTable () {
            Dictionary<WorkTags, HashSet<WorkTypeDef>> tableWorks = new Dictionary<WorkTags, HashSet<WorkTypeDef>>();
            Dictionary<WorkTags, HashSet<WorkGiverDef>> tableGivers = new Dictionary<WorkTags, HashSet<WorkGiverDef>>();
            Dictionary<WorkTags, HashSet<SkillDef>> tableSkills = new Dictionary<WorkTags, HashSet<SkillDef>>();
            foreach (WorkTags curTag in Enum.GetValues(typeof(WorkTags))) {
                tableWorks.Add(curTag, new HashSet<WorkTypeDef>());
                tableGivers.Add(curTag, new HashSet<WorkGiverDef>());
                tableSkills.Add(curTag, new HashSet<SkillDef>());
            }
            foreach (WorkTypeDef curWork in WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder) {
                foreach (WorkTags curTag in GetWorkTags(curWork)) {
                    tableWorks[curTag].Add(curWork);
                    if (curTag == WorkTags.None) continue;
                    tableGivers[curTag].AddRange(curWork.workGiversByPriority);
                    tableSkills[curTag].AddRange(curWork.relevantSkills);
                }
            }
            foreach (WorkGiverDef curGiver in DefDatabase<WorkGiverDef>.AllDefsListForReading) {
                foreach (WorkTags curTag in GetWorkTags(curGiver)) {
                    if (curTag == WorkTags.None) continue;
                    tableGivers[curTag].Add(curGiver);
                }
            }
            foreach (SkillDef curSkill in DefDatabase<SkillDef>.AllDefsListForReading) {
                foreach (WorkTags curTag in Ex_SkillDef.GetWorkTags(curSkill)) {
                    if (curTag == WorkTags.None) continue;
                    tableSkills[curTag].Add(curSkill);
                }
            }

            return new JObject(new JProperty("data",
                from t in tableWorks.Keys
                select new JObject(
                    new JProperty("tag", t.LabelTranslated()),
                    new JProperty("works", from w in tableWorks[t]
                                           select w.label),
                    new JProperty("givers", from g in tableGivers[t]
                                            select g.label),
                    new JProperty("skills", from s in tableSkills[t]
                                            select s.skillLabel)
                )
            ));
        }

        #endregion
    }
}
