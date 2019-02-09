using System.IO;
using System.Text;

namespace Tests {
    public static class Utils {
        public static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }
}