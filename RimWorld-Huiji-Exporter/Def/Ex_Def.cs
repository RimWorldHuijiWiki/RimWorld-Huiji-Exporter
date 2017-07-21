using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace RimWorld_Huiji_Exporter.Def {
    public class Ex_Def {
        public virtual void Export() {
            Log.Error($"{Controller.Identifier} Class '{GetType().Name}' dose not override method '{System.Reflection.MethodBase.GetCurrentMethod().Name}()'.");
        }
    }
}
