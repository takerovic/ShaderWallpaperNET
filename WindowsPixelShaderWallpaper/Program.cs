using Silk.NET.GLFW;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using WindowsPixelShaderWallpaper;
using WindowsPixelShaderWallpaper.ShaderRender;

namespace WindowsWallpaper
{
    internal class Program
    {
        public static ShaderWindow MainShaderWindow;

        private static void Main(string[] args)
        {
            MainShaderWindow = new ShaderWindow(new Vector2D<int>(1920, 1080), "winshaderpaper");
            MainShaderWindow.WindowHandler.Position = new Vector2D<int>(0, 0);
            ProgmanHook WallpaperHook = new ProgmanHook();

            string DefaultVertShader = EmbeddedResourceHelper.GetResource("ShaderRender.ShaderResource.DefaultVert.vert");
            MainShaderWindow.AddShader(ShaderSourceType.Vertex, DefaultVertShader);
            string FragShader = ShadertoySourceConverter.ConvertToCommonGLSL(File.ReadAllText(".frag"));
            MainShaderWindow.AddShader(ShaderSourceType.Fragment, FragShader);
            MainShaderWindow.WindowHandler.Load += () =>
            {
                Console.WriteLine($"ShaderWindow hWnd: {MainShaderWindow.GetHandle().ToString()}");
                Console.WriteLine($"ShaderWindow Native: {MainShaderWindow.WindowHandler.Native.Win32}");
                WallpaperHook.HookHandle(MainShaderWindow.WindowHandler.Native.Win32.Value.Hwnd);
            };
            new Thread(new ThreadStart(MainShaderWindow.StartWindow)).Start();
        }
    }
}
