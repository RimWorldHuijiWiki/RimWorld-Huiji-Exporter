using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HuijiExporter.Utility {
    public static class ColorExtension {
        public static string ToStringHexExceptWhite(this Color color) {
            if (color == null || color == Color.white)
                return null;
            return (new ColorRGB(color.r, color.g, color.b)).ToStringHex();
        }
    }
}
