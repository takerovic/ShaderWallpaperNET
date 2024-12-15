using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPixelShaderWallpaper
{
    static class EmbeddedResourceHelper
    {
        public static string GetResource(string resourceName) 
        {
            var info = Assembly.GetExecutingAssembly().GetName();
            var name = info.Name;
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{name}.{resourceName}");
            var streamReader = new StreamReader(stream, Encoding.UTF8);
            return streamReader.ReadToEnd();
        }
    }
}
