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
    class Ex_SkillDef : Ex_Def {
        public override string DefType => typeof(SkillDef).Name;

        public override void Export() {
            // Instance
            List<JObject> allDatas = new List<JObject>();
            List<string> allDefs = new List<string>();
            foreach (SkillDef curDef in DefDatabase<SkillDef>.AllDefsListForReading) {
                JObject curData = new JObject(
                    new JProperty("defType", this.DefType),
                    new JProperty("defName", curDef.defName),
                    new JProperty("label", curDef.skillLabel), // Special
                    new JProperty("skillLabel", curDef.skillLabel), // Special
                    new JProperty("description", curDef.description),
                    new JProperty("disablingWorkTags", GetWorkTags(curDef))
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
                            new JProperty("record", GenerateSkillRecord())
                        )
                    )
                );
                sw.Write(collection.ToString());
            }
        }

        #region Methods

        internal static IEnumerable<WorkTags> GetWorkTags(SkillDef skill) {
            if (skill.disablingWorkTags == WorkTags.None) {
                yield return WorkTags.None;
            }
            foreach (WorkTags curTag in Enum.GetValues(typeof(WorkTags))) {
                if (curTag == WorkTags.None)
                    continue;
                if ((skill.disablingWorkTags & curTag) == curTag) {
                    yield return curTag;
                }
            }
        }

        #endregion

        #region ECharts Data Generators

        /// <summary>
        /// Data of pession, experience lost, etc.
        /// 兴趣度，经验流失等的数据
        /// Class: RimWorld.SkillRecord
        /// </summary>
        /// <returns></returns>
        protected JObject GenerateSkillRecord() {
            List<JObject> LevelsData = new List<JObject>();
            for (int curLevel = SkillRecord.MinLevel; curLevel <= SkillRecord.MaxLevel; curLevel++) {
                LevelsData.Add(new JObject(
                    new JProperty("Level", curLevel),
                    new JProperty("Descriptor", $"Skill{curLevel}".Translate()),
                    new JProperty("XpRequiredToLevelUp", 1000 + curLevel * 1000),
                    new JProperty("XpLostSpeed", GetXpLostSpeedFor(curLevel))
                ));
            }

            return new JObject(
                new JProperty("IntervalTicks", SkillRecord.IntervalTicks),
                new JProperty("MinLevel", SkillRecord.MinLevel),
                new JProperty("MaxLevel", SkillRecord.MaxLevel),
                new JProperty("MaxFullRateXpPerDay", SkillRecord.MaxFullRateXpPerDay),
                new JProperty("SaturatedLearningFactor", SkillRecord.SaturatedLearningFactor),
                new JProperty("PassionsData", new JArray(
                    new JObject(
                        new JProperty("Passion", Passion.None),
                        new JProperty("Descriptor", "PassionNone".Translate(SkillRecord.LearnFactorPassionNone.ToStringPercent("F0"))),
                        new JProperty("LearningFactor", SkillRecord.LearnFactorPassionNone),
                        new JProperty("GainJoyAmount", 0f),
                        new JProperty("Icon", null)
                    ),
                    new JObject(
                        new JProperty("Passion", Passion.Minor),
                        new JProperty("Descriptor", "PassionMinor".Translate(SkillRecord.LearnFactorPassionMinor.ToStringPercent("F0"))),
                        new JProperty("LearningFactor", SkillRecord.LearnFactorPassionMinor),
                        new JProperty("GainJoyAmount", 2E-05f),
                        new JProperty("Icon", TextureUtility.PathToFile("UI/Icons/PassionMinor"))
                    ),
                    new JObject(
                        new JProperty("Passion", Passion.Major),
                        new JProperty("Descriptor", "PassionMajor".Translate(SkillRecord.LearnFactorPassionMajor.ToStringPercent("F0"))),
                        new JProperty("LearningFactor", SkillRecord.LearnFactorPassionMajor),
                        new JProperty("GainJoyAmount", 4E-05f),
                        new JProperty("Icon", TextureUtility.PathToFile("UI/Icons/PassionMajor"))
                    )
                )),
                new JProperty("LevelsData", LevelsData)
            );
        }

        private float GetXpLostSpeedFor(int level) {
            switch (level) {
                case 10:
                    return -0.1f;
                case 11:
                    return -0.2f;
                case 12:
                    return -0.4f;
                case 13:
                    return -0.65f;
                case 14:
                    return -1f;
                case 15:
                    return -1.5f;
                case 16:
                    return -2f;
                case 17:
                    return -3f;
                case 18:
                    return -4f;
                case 19:
                    return -6f;
                case 20:
                    return -8f;
                default:
                    return 0f;
            }
        }

        #endregion

    }
}
