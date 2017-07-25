using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuijiExporter.Utility {
    public static class StringExtension {
        public static string Wrap(this string text, int length = 30) {
            if (string.IsNullOrEmpty(text) || text.Length <= length) {
                return text;
            }
            return string.Join("<br/>", text.SplitByLength(length));
        }

        public static string[] SplitByLength(this string text, int length = 30) {
            if (string.IsNullOrEmpty(text) || text.Length <= length) {
                return new string[] { text };
            }
            List<string> list = new List<string>();
            for (int i = 0; i < text.Length; i += length) {
                if (i + length > text.Length) {
                    list.Add(text.Substring(i));
                } else {
                    list.Add(text.Substring(i, length));
                }
            }
            return list.ToArray();
        }
    }
}
