using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace RimWorld_Huiji_Exporter.ExDef {
    public class Ex_Def {
        public virtual void Export() {
            //List<JObject> allDatas = new List<JObject>();
            //foreach (Def curDef in DefDatabase<Def>.AllDefs) {
            //    JObject data = JObject.FromObject(new {
            //        defType = "Def",
            //        defName = curDef.defName,
            //        label = curDef.label,
            //        description = curDef.description,
            //    });
            //}
            Log.Error($"{Controller.Identifier} Class '{GetType().Name}' dose not override method '{System.Reflection.MethodBase.GetCurrentMethod().Name}()'.");
        }
    }
}
