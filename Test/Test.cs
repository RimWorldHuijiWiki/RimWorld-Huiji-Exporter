using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test {
    class Test {
        static void Main(string[] args) {
            double hue = -10.6;
            double hueClamped;
            for (int i = 0; i < 100; i++) {
                hue += 0.3;
                if (hue < 0.0 || hue > 1.0) {
                    hueClamped = hue - (double)Math.Floor(hue);
                } else {
                    hueClamped = hue;
                }
                Console.WriteLine($"{hue}\t{hueClamped}");
            }
        }

        private static double Loop01(double value) {
            if (value < 0.0 || value > 1.0)
                value -= Math.Floor(value);
            return value;
        }
    }
}
