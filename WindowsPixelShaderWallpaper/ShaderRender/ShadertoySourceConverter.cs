using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPixelShaderWallpaper.ShaderRender
{
    static class ShadertoySourceConverter
    {

        public static string ConvertToCommonGLSL(string source)
        {
            string Prefix = EmbeddedResourceHelper.GetResource("ShaderRender.ShaderResource.ShaderToyPrefix.glsl");
            string Suffix = EmbeddedResourceHelper.GetResource("ShaderRender.ShaderResource.ShaderToySuffix.glsl");
            return Prefix + source + Suffix;
        }
    }
}
