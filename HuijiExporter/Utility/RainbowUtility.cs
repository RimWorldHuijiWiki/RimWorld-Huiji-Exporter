using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuijiExporter.Utility {
    public static class RainbowUtility {
        public static IEnumerable<string> RainbowHex(int count = 7, string startColorHex = "#ff6666") {
            ColorHSL hsl = ColorRGB.FromHtmlHex(startColorHex).ToColorHSL();
            double startHue = hsl.H;
            for (int i = 0; i < count + 1; i++) {
                hsl.H = startHue + (double)i / (double)count;
                yield return hsl.ToColorRGB().ToStringHex();
            }
        }

        public static IEnumerable<string> RainbowHex360(string startColorHex = "#ff6666") {
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
