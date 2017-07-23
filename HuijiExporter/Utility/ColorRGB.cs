using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuijiExporter.Utility {
    public class ColorRGB {

        #region Fields

        private double red = 1.0;
        private double green = 1.0;
        private double blue = 1.0;

        #endregion

        #region Constructors

        public ColorRGB() { }

        public ColorRGB(ColorRGB other) {
            this.red = other.red;
            this.green = other.green;
            this.blue = other.blue;
        }

        public ColorRGB(double red, double green, double blue) {
            this.red = Clamp01(red);
            this.green = Clamp01(green);
            this.blue = Clamp01(blue);
        }

        public static ColorRGB FromHtmlHex(string htmlHex) {
            if (htmlHex.StartsWith("#"))
                htmlHex = htmlHex.Substring(1);
            if (htmlHex.Length != 3 && htmlHex.Length != 6)
                throw new ArgumentException($"The htmlHex '#{htmlHex}' is invalid.");
            if (htmlHex.Length == 3)
                htmlHex = string.Format("{0}{1}{2}", htmlHex.Substring(0, 1), htmlHex.Substring(1, 1), htmlHex.Substring(2, 1));
            int red = int.Parse(htmlHex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int green = int.Parse(htmlHex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int blue = int.Parse(htmlHex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new ColorRGB() {
                red = (double)red / 255.0,
                green = (double)green / 255.0,
                blue = (double)blue / 255.0
            };
        }

        public static ColorRGB FromInt32(int red, int green, int blue) {
            return new ColorRGB(
                (double)red / 255.0,
                (double)green / 255.0,
                (double)blue / 255.0);
        }

        public static ColorRGB FromByte(byte red, byte green, byte blue) {
            return new ColorRGB(
                (double)red / 255.0,
                (double)green / 255.0,
                (double)blue / 255.0);
        }

        #endregion

        #region Properties

        public double R {
            get { return red; }
            set { red = Clamp01(value); }
        }
        public double G {
            get { return green; }
            set { green = Clamp01(value); }
        }
        public double B {
            get { return blue; }
            set { blue = Clamp01(value); }
        }

        #endregion

        #region Methods

        public override string ToString() {
            return string.Format("rgb({0}, {1}, {2})",
                Convert.ToInt32(red * 255.0),
                Convert.ToInt32(green * 255.0),
                Convert.ToInt32(blue * 255.0));
        }

        public string ToStringHex() {
            return string.Format("#{0:X2}{1:X2}{2:X2}",
                Convert.ToInt32(red * 255.0),
                Convert.ToInt32(green * 255.0),
                Convert.ToInt32(blue * 255.0));
        }

        public ColorHSL ToColorHSL() {
            ClampColor();
            double min = Math.Min(red, Math.Min(green, blue));
            double max = Math.Max(red, Math.Max(green, blue));

            double delta = max - min;
            double hue = 0.0;
            double saturation = 0.0;

            double lightness = (min + max) / 2.0;
            if (lightness > 0.0 && lightness < 0.5)
                saturation = delta / (max + min);
            if (lightness >= 0.5 && lightness < 1.0)
                saturation = delta / (2.0 - max - min);

            if (delta > 0.0) {
                if (max == red && max != green)
                    hue += (green - blue) / delta;
                if (max == green && max != blue)
                    hue += 2.0 + (blue - red) / delta;
                if (max == blue && max != red)
                    hue += 4.0 + (red - green) / delta;
                hue /= 6.0;
            }

            if (hue < 0)
                hue += 1.0;
            if (hue > 0)
                hue -= 1.0;

            return new ColorHSL(hue, saturation, lightness);
        }

        #endregion

        #region Helpers

        private static double Clamp01(double value) {
            if (value < 0.0)
                value = 0.0;
            if (value > 1.0)
                value = 1.0;
            return value;
        }

        private void ClampColor() {
            red = Clamp01(red);
            green = Clamp01(green);
            blue = Clamp01(blue);
        }

        #endregion
    }
}
