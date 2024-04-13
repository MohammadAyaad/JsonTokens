using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTokens
{
    public static class Base64Converter
    {
        public static byte[] FromBase64String(string base64)
        {
            return Convert.FromBase64String(base64.PadRight(4 * ((base64.Length + 3) / 4), '='));
        }
        public static string ToBase64String(byte[] bytearray)
        {
            return Convert.ToBase64String(bytearray).Replace("=", "");
        }
    }
}
