using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuijiExporter.Utility {
    public static class TextureUtility {
        public static string PathToFile(string path) {
            return $"Texture_{path.ToLower().Replace('/', '_')}.png";
        }
    }
}
