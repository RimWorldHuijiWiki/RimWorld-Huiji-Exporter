using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuijiExporter.Utility {
    public class ColorHSL {

        #region Fields

        private double hue = 0.0;
        private double saturation = 0.0;
        private double lightness = 1.0;

        #endregion

        #region Construtors

        public ColorHSL() { }

        public ColorHSL(ColorHSL other) {
            this.hue = other.hue;
            this.saturation = other.saturation;
            this.lightness = other.lightness;
        }

        public ColorHSL(double hue, double saturation, double lightness) {
            this.hue = Cycle01(hue);
            this.saturation = Clamp01(saturation);
            this.lightness = Clamp01(lightness);
        }

        public static ColorHSL FromHtmlHsl(int hueDegree, int saturationPercent, int lightnessPercent) {
            return new ColorHSL((double)hueDegree / 360.0, (double)saturationPercent / 100.0, (double)lightnessPercent / 100.0);
        }

        #endregion

        #region Properties

        public double H {
            get { return hue; }
            set { hue = Cycle01(value); }
        }
        public double S {
            get { return saturation; }
            set { saturation = Clamp01(value); }
        }
        public double L {
            get { return lightness; }
            set { lightness = Clamp01(value); }
        }
        public int HueDegree {
            get { return Convert.ToInt32(hue * 360.0); }
            set { hue = Cycle01((int)value / 360.0); }
        }
        public int SaturationPercent {
            get { return Convert.ToInt32(saturation * 100.0); }
            set { saturation = Clamp01((double)value / 100.0); }
        }
        public int LightnessPercent {
            get { return Convert.ToInt32(lightness * 100.0); }
            set { lightness = Clamp01((double)value / 100.0); }
        }

        #endregion

        #region Methods

        public override string ToString() {
            return string.Format("hsl({0}, {1}%, {2}%)",
                Convert.ToInt32(hue * 360.0),
                Convert.ToInt32(saturation * 100.0),
                Convert.ToInt32(lightness * 100.0));
        }

        public ColorRGB ToColorRGB() {
            ClampColor();
            double m2 = lightness <= 0.5 ? lightness * (saturation + 1.0) : lightness + saturation - lightness * saturation;
            double m1 = lightness * 2.0 - m2;
            return new ColorRGB(
                HueToRGB(m1, m2, hue + 1.0 / 3.0),
                HueToRGB(m1, m2, hue),
                HueToRGB(m1, m2, hue - 1.0 / 3.0));
        }

        private static double HueToRGB(double m1, double m2, double hue) {
            if (hue < 0.0)
                hue = hue + 1.0;
            if (hue > 1.0)
                hue = hue - 1.0;
            if (hue * 6.0 < 1.0)
                return m1 + (m2 - m1) * hue * 6.0;
            if (hue * 2.0 < 1.0)
                return m2;
            if (hue * 3.0 < 2.0)
                return m1 + (m2 - m1) * (2.0 / 3.0 - hue) * 6.0;
            return m1;
        }

        #endregion

        #region Helpers

        private static double Cycle01(double value) {
            if (value < 0.0 || value > 1.0)
                value -= (double)Math.Floor(value);
            return value;
        }

        private static double Clamp01(double value) {
            if (value < 0.0)
                value = 0.0;
            if (value > 1.0)
                value = 1.0;
            return value;
        }

        private void ClampColor() {
            hue = Cycle01(hue);
            saturation = Clamp01(saturation);
            lightness = Clamp01(lightness);
        }

        #endregion
    }
}
