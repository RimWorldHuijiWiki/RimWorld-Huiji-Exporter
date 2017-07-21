using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace HuijiExporter.ExDefs {
    class Ex_Def {
        public virtual string DefType => typeof(Def).Name;

        public virtual void Export() {
            //List<JObject> allDatas = new List<JObject>();
            //foreach (Def curDef in DefDatabase<Def>.AllDefs) {
            //    JObject curData = new JObject(
            //        new JProperty("defType", this.DefType),
            //        new JProperty("defName", curDef.defName),
            //        new JProperty("label", curDef.label),
            //        new JProperty("description", curDef.description)
            //    );
            //    allDatas.Add(curData);
            //    using (StreamWriter sw = new StreamWriter(Path.Combine(Controller.Path_Defs, $"{this.DefType}_{curDef.defName}.json"))) {
            //        string text = curData.ToString();
            //        sw.Write(text);
            //    }
            //}
            Log.Error($"{Controller.Identifier} The class '{GetType().Name}' does not realized method '{System.Reflection.MethodBase.GetCurrentMethod().Name}()'.");
        }
    }
}
