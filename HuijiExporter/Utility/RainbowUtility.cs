using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuijiExporter.Utility {
    public static class RainbowUtility {

        public static IEnumerable<ColorHSL> RainbowHSL(int count = 7, string startColorHex = "#f66") {
            ColorHSL startColorHSL = ColorRGB.FromHtmlHex(startColorHex).ToColorHSL();
            double statHue = startColorHSL.H;
            for (int i = 0; i < count; i++) {
                yield return new ColorHSL(statHue + (double)i / (double)count, startColorHSL.S, startColorHSL.L);
            }
        }

        public static IEnumerable<ColorRGB> RainbowRGB(int count = 7, string startColorHex = "#f66") {
            return from hsl in RainbowHSL(count, startColorHex)
                   select hsl.ToColorRGB();
        }

        public static IEnumerable<string> RainbowHex(int count = 7, string startColorHex = "#f66") {
            return from hsl in RainbowHSL(count, startColorHex)
                   select hsl.ToColorRGB().ToStringHex();
        }

        public static IEnumerable<string> RainbowHex360(string startColorHex = "#f66") {
            return RainbowHex(360, startColorHex);
        }

        public static void RainbowHexDemo() {
            StringBuilder sb = new StringBuilder();
            int ten = 0;
            foreach (string curColor in RainbowHex360()) {
                sb.Append(curColor);
                sb.Append("    ");
                ten++;
                if (ten == 10) {
                    ten = 0;
                    sb.AppendLine();
                }
            }
            Verse.Log.Message(sb.ToString());
        }
    }
}
