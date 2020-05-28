using System.IO;
using System.Text;
using DCasm;

namespace Tests {
    public static class Utils {
        public static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
        
        public static bool isCorrectType<T>(INode node) {
            return node.GetType() == typeof(T);
        }
    }
}