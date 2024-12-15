using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Maths;
using System.Drawing;
using System.Drawing.Design;
using Silk.NET.GLFW;
using System.Threading;
using System.Runtime.ExceptionServices;

namespace WindowsWallpaper
{
    enum ShaderSourceType
    {
        Vertex = 0,
        Fragment = 1
    }
    class ShaderWindow
    {
        public IWindow WindowHandler;
        public GL OpenGLHandler;

        private uint VAO;
        private uint VBO;
        private uint EBO;

        private uint Program;
        private Dictionary<ShaderSourceType, string> ShaderData = new Dictionary<ShaderSourceType, string>();

        private float WindowTime;
        private float TimeSpeed = 0.01f;

        private uint iTimePointer;
        private uint iResolutionPointer;
        private uint iMousePointer;

        private uint FragmentShader;
        private uint VertexShader;

        public ShaderWindow(Vector2D<int> windowSize, string windowTitle)
        {
            WindowOptions options = WindowOptions.Default;
            options.Size = windowSize;
            options.Title = windowTitle;
            options.WindowBorder = WindowBorder.Fixed;
            WindowHandler = Window.Create(options);

            WindowHandler.Load += OnLoad;
            WindowHandler.Update += OnUpdate;
            WindowHandler.Render += OnRender;

        }


        public void AdjustSpeed(float newSpeed)
        {
            Console.WriteLine(newSpeed);
            TimeSpeed = newSpeed;
        }
        public void AddShader(ShaderSourceType shaderType, string ShaderSource)
        {
            if (ShaderData.ContainsKey(shaderType)) 
            {
                ShaderData.Remove(shaderType);
            }
            ShaderData.Add(shaderType, ShaderSource); 
        }
        public IntPtr GetHandle()
        {
            return (IntPtr)WindowHandler.Handle;
        }


        public void StartWindow()
        {
            Console.WriteLine("Starting Window!");
            WindowHandler.Run();
        }

        [HandleProcessCorruptedStateExceptions]
        public void StopWindow() 
        {
            Console.WriteLine("Closing Windows!");
            WindowHandler.Close();
            while (true)
            {
                try
                {
                    Console.WriteLine(WindowHandler.IsClosing);
                    
                }
                catch
                {
                    break;
                }
            }
        }

        public unsafe void SwitchShader(ShaderSourceType shaderType, string shaderSource)
        {
        }

        private unsafe void OnLoad()
        {
            Console.WriteLine("Loaded SHADERWINDOW!");
            OpenGLHandler = WindowHandler.CreateOpenGL();
            OpenGLHandler.ClearColor(Color.AliceBlue);
            Console.WriteLine("Cleared!");

            VAO = OpenGLHandler.GenVertexArray();
            OpenGLHandler.BindVertexArray(VAO);

            float[] vertices =
            {
                 1f,  1f, 0.0f,
                 1f, -1f, 0.0f,
                -1f, -1f, 0.0f,
                -1f,  1f, 0.0f
            };

            VBO = OpenGLHandler.GenBuffer();
            OpenGLHandler.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);

            fixed (float* buf = vertices)
                OpenGLHandler.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            
            uint[] indices =
            {
                0u, 1u, 3u,
                1u, 2u, 3u
            };

            EBO = OpenGLHandler.GenBuffer();
            OpenGLHandler.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);

            fixed (uint* buf = indices)
                OpenGLHandler.BufferData(BufferTargetARB.ElementArrayBuffer, (uint)(indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);

            uint vertexShader = OpenGLHandler.CreateShader(ShaderType.VertexShader);
            OpenGLHandler.ShaderSource(vertexShader, ShaderData[ShaderSourceType.Vertex]);

            OpenGLHandler.CompileShader(vertexShader);

            OpenGLHandler.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + OpenGLHandler.GetShaderInfoLog(vertexShader));

            uint fragmentShader = OpenGLHandler.CreateShader(ShaderType.FragmentShader);
            OpenGLHandler.ShaderSource(fragmentShader, ShaderData[ShaderSourceType.Fragment]);

            OpenGLHandler.CompileShader(fragmentShader);

            OpenGLHandler.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + OpenGLHandler.GetShaderInfoLog(fragmentShader));

            Program = OpenGLHandler.CreateProgram();

            OpenGLHandler.AttachShader(Program, vertexShader);
            OpenGLHandler.AttachShader(Program, fragmentShader);

            FragmentShader = fragmentShader;
            VertexShader = vertexShader;

            OpenGLHandler.LinkProgram(Program);

            OpenGLHandler.GetProgram(Program, ProgramPropertyARB.LinkStatus, out int lStatus);
            if (lStatus != (int)GLEnum.True)
                throw new Exception("Program failed to link: " + OpenGLHandler.GetProgramInfoLog(Program));

            uint positionLoc = (uint) OpenGLHandler.GetAttribLocation(Program, "aPosition");
            OpenGLHandler.EnableVertexAttribArray(positionLoc);
            OpenGLHandler.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);

            iTimePointer = (uint) OpenGLHandler.GetUniformLocation(Program, "iTime");
            iResolutionPointer = (uint)OpenGLHandler.GetUniformLocation(Program, "iResolution");
            iMousePointer = (uint)OpenGLHandler.GetUniformLocation(Program, "iMouse");
        }

        private unsafe void OnUpdate(double time)
        {
            
        }

        private unsafe void OnRender(double time)
        {
            WindowTime += TimeSpeed;
            U32.POINT MousePosition;
            U32.GetCursorPos(out MousePosition);

            

            OpenGLHandler.Uniform1((int)iTimePointer, WindowTime);
            OpenGLHandler.Uniform3((int)iResolutionPointer, WindowHandler.Size.X, WindowHandler.Size.Y, 0f);
            OpenGLHandler.Uniform4((int)iMousePointer, MousePosition.X, MousePosition.Y, MousePosition.X, MousePosition.Y);
            OpenGLHandler.Clear(ClearBufferMask.ColorBufferBit);

            OpenGLHandler.BindVertexArray(VAO);
            OpenGLHandler.UseProgram(Program);
            OpenGLHandler.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);
        }
    }
}
