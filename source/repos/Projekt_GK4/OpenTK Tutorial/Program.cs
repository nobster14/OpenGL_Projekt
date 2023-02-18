

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using LearnOpenTK;
using Window = LearnOpenTK.Window;

namespace Main
{
    public class MainClass
    {
        static void Main(string[] args)
        {

                var nativeWindowSettings = new NativeWindowSettings()
                {
                    Size = new Vector2i(800, 600),
                    Title = "OpenGL Window",
                    // This is needed to run on macos
                    Flags = ContextFlags.ForwardCompatible,

                };

                using (Window window = new Window(GameWindowSettings.Default, nativeWindowSettings))
                {
                    window.Run();
                }

        }
    }
}
