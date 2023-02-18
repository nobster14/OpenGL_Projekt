using System;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;
using ObjLoader.Loader.Loaders;
using OpenTK_Tutorial.DataStructures;
using OpenTK_Tutorial;

namespace LearnOpenTK
{
    // In this tutorial we take the code from the last tutorial and create some level of abstraction over it allowing more
    // control of the interaction between the light and the material.
    // At the end of the web version of the tutorial we also had a bit of fun creating a disco light that changes
    // color of the cube over time.
    public class Window : GameWindow
    {



        #region Options
        public ProgramOptions options = new ProgramOptions();
        #endregion

        #region Shaders
        private Shader phongShader;
        private Shader gouraudShader;
        private Shader constantShader;
        private Shader lightShader;
        #endregion

        #region Lights
        private readonly List<Vector3> pointLights = new List<Vector3> { new Vector3(-2.0f, 4.0f, 0.0f), new Vector3(3.0f, 5.0f, 3.0f) };
        private DirectLight sun = new DirectLight();
        private List<SpotLight> spotLights = new List<SpotLight> { new SpotLight(new Vector3(-0.5f, 2f, 0f)), new SpotLight(new Vector3(0.5f, 2f, 0f)) };
        #endregion

        #region Cameras
        private Camera _movingObjectCamera;
        private Camera _staticCamera;
        private Camera _staticCameraFollowingObj;
        #endregion

        private bool _firstMove = true;
        #region Object to draw
        private List<MyObject> data;
        #endregion

        private Vector2 _lastPos;

        private float _time = 0;

        #region VAOs and VBOs
        private List<int> VBOs = new List<int>();
        private List<int> ModelVAOs = new List<int>();
        private int _vaoLamp;
        #endregion

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            data = MyLoader.Load("../../../InputData/chess_board_all.obj");
            phongShader = new Shader("../../../Shaders/shaderPhong.vert", "../../../Shaders/lightingPhong.frag");
            gouraudShader = new Shader("../../../Shaders/shaderGourad.vert", "../../../Shaders/lightingGourad.frag");
            constantShader = new Shader("../../../Shaders/shaderConstant.vert", "../../../Shaders/lightingConstant.frag");
            lightShader = new Shader("../../../Shaders/shaderPhong.vert", "../../../Shaders/shader.frag");
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            for (int i = 0; i < data.Count; i++)
            {
                var vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, data[i].ConvertVerticiesToFlatArray().Length * sizeof(float), data[i].ConvertVerticiesToFlatArray(), BufferUsageHint.StaticDraw);
                VBOs.Add(vbo);

                {
                    var vaoModel = GL.GenVertexArray();
                    ModelVAOs.Add(vaoModel);

                    GL.BindVertexArray(vaoModel);
                    var positionLocation = phongShader.GetAttribLocation("aPos");
                    GL.EnableVertexAttribArray(positionLocation);
                    GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                    var normalLocation = phongShader.GetAttribLocation("aNormal");
                    GL.EnableVertexAttribArray(normalLocation);
                    GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

                    GL.BindVertexArray(vaoModel);
                    var positionLocation2 = gouraudShader.GetAttribLocation("aPos");
                    GL.EnableVertexAttribArray(positionLocation2);
                    GL.VertexAttribPointer(positionLocation2, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                    var normalLocation2 = gouraudShader.GetAttribLocation("aNormal");
                    GL.EnableVertexAttribArray(normalLocation2);
                    GL.VertexAttribPointer(normalLocation2, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

                    GL.BindVertexArray(vaoModel);
                    var positionLocation3 = constantShader.GetAttribLocation("aPos");
                    GL.EnableVertexAttribArray(positionLocation3);
                    GL.VertexAttribPointer(positionLocation3, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                    var normalLocation3 = constantShader.GetAttribLocation("aNormal");
                    GL.EnableVertexAttribArray(normalLocation3);
                    GL.VertexAttribPointer(normalLocation3, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                }
            }

            var vbo2 = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo2);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Last().ConvertVerticiesToFlatArray().Length * sizeof(float), data.Last().ConvertVerticiesToFlatArray(), BufferUsageHint.StaticDraw);

            _vaoLamp = GL.GenVertexArray();
            GL.BindVertexArray(_vaoLamp);

            var positionLocation4 = lightShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation4);
            GL.VertexAttribPointer(positionLocation4, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);


            _movingObjectCamera = new Camera(new Vector3((-0.117447294f, 3.3331909f, -0.26576048f)), Size.X / (float)Size.Y);
            _staticCamera = new Camera(new Vector3((-9.700516f, 6.6947083f, 2.220917f)), Size.X / (float)Size.Y);
            _staticCamera.Pitch = -23.59285f;
            _staticCamera.Yaw = -36.787426f;
            _staticCameraFollowingObj = new Camera(new Vector3(9.808256f, 7.169347f, 0.17111626f), Size.X / (float)Size.Y);


            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int i = 0; i < data.Count; i++)
            {
                GL.BindVertexArray(ModelVAOs[i]);

                UseShader(data[i], GetActiveCamera(), pointLights);

                GL.DrawArrays(PrimitiveType.Triangles, 0, data[i].verticiesAndNormals.Count / 2);
            }

            GL.BindVertexArray(_vaoLamp);

            lightShader.Use();

            lightShader.SetMatrix4("view", GetActiveCamera().GetViewMatrix());
            lightShader.SetMatrix4("projection", GetActiveCamera().GetProjectionMatrix());

            for (int i = 0; i < pointLights.Count; i++)
            {
                Matrix4 lampMatrix = Matrix4.CreateScale(0.5f);
                lampMatrix = lampMatrix * Matrix4.CreateTranslation(pointLights[i]);

                lightShader.SetMatrix4("model", lampMatrix);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }

            SwapBuffers();
        }

        private void UseShader(MyObject data, Camera camera, List<Vector3> pointLights)
        {
            switch (options.ShadersOptions)
            {
                case ShadersOptions.phong:
                    phongShader.Use();
                    Shaders.Render(phongShader, camera, data, pointLights, sun, options.fogDensity, spotLights);
                    break;
                case ShadersOptions.gouraud:
                    gouraudShader.Use();
                    Shaders.Render(gouraudShader, camera, data, pointLights, sun, options.fogDensity, spotLights);
                    break;
                case ShadersOptions.constant:
                    constantShader.Use();
                    Shaders.Render(constantShader, camera, data, pointLights, sun, options.fogDensity, spotLights);
                    break;
            }
        }

        private Camera GetActiveCamera()
        {
            switch (options.CameraOptions)
            {
                case CameraOptions.staticCamera:
                    return _staticCamera;
                case CameraOptions.movingObjectCamera:
                    return _movingObjectCamera;
                case CameraOptions.staticCameraFollowingObj:
                    return _staticCameraFollowingObj;
                default:
                    return null;
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _time += (float)e.Time;

            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float speed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                if (options.CameraOptions == CameraOptions.movingObjectCamera)
                {
                    _movingObjectCamera.Position += _movingObjectCamera.Front * speed * (float)e.Time; // Forward
                    ((MovingObject)data.Last()).MoveObject(_movingObjectCamera.Front * speed * (float)e.Time);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(_movingObjectCamera.Front * speed * (float)e.Time);
                }
                else
                {
                    ((MovingObject)data.Last()).MoveObject(new Vector3(speed * (float)e.Time, 0, 0));
                    _movingObjectCamera.Position += new Vector3(speed * (float)e.Time, 0, 0);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(new Vector3(speed * (float)e.Time, 0, 0));
                }
            }
            if (input.IsKeyDown(Keys.S))
            {
                if (options.CameraOptions == CameraOptions.movingObjectCamera)
                {
                    _movingObjectCamera.Position -= _movingObjectCamera.Front * speed * (float)e.Time; // Backwards
                    ((MovingObject)data.Last()).MoveObject(_movingObjectCamera.Front * speed * -(float)e.Time);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(_movingObjectCamera.Front * speed * -(float)e.Time);
                }
                else
                {
                    ((MovingObject)data.Last()).MoveObject(new Vector3(speed * -(float)e.Time, 0, 0));
                    _movingObjectCamera.Position += new Vector3(speed * -(float)e.Time, 0, 0);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(new Vector3(speed * -(float)e.Time, 0, 0));
                }
            }
            if (input.IsKeyDown(Keys.A))
            {
                if (options.CameraOptions == CameraOptions.movingObjectCamera)
                {
                    _movingObjectCamera.Position -= _movingObjectCamera.Right * speed * (float)e.Time; // Left
                    ((MovingObject)data.Last()).MoveObject(_movingObjectCamera.Right * speed * -(float)e.Time);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(_movingObjectCamera.Right * speed * -(float)e.Time);
                }
                else
                {
                    ((MovingObject)data.Last()).MoveObject(new Vector3(0, 0, speed * (float)e.Time));
                    _movingObjectCamera.Position += new Vector3(0, 0, speed * (float)e.Time);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(new Vector3(0, 0, speed * (float)e.Time));
                }
            }
            if (input.IsKeyDown(Keys.D))
            {
                if (options.CameraOptions == CameraOptions.movingObjectCamera)
                {
                    _movingObjectCamera.Position += _movingObjectCamera.Right * speed * (float)e.Time; // Right
                    ((MovingObject)data.Last()).MoveObject(_movingObjectCamera.Right * speed * (float)e.Time);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(_movingObjectCamera.Right * speed * (float)e.Time);
                }
                else
                {
                    ((MovingObject)data.Last()).MoveObject(new Vector3(0, 0, speed * -(float)e.Time));
                    _movingObjectCamera.Position += new Vector3(0, 0, speed * -(float)e.Time);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(new Vector3(0, 0, speed * -(float)e.Time));
                }
            }
            if (input.IsKeyDown(Keys.Space))
            {
                if (options.CameraOptions == CameraOptions.movingObjectCamera)
                {
                    _movingObjectCamera.Position += _movingObjectCamera.Up * speed * (float)e.Time; // Up
                    ((MovingObject)data.Last()).MoveObject(_movingObjectCamera.Up * speed * (float)e.Time);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(_movingObjectCamera.Up * speed * (float)e.Time);
                }
                else
                {
                    ((MovingObject)data.Last()).MoveObject(new Vector3(0, speed * (float)e.Time, 0));
                    _movingObjectCamera.Position += new Vector3(0, speed * (float)e.Time, 0);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(new Vector3(0, speed * (float)e.Time, 0));
                }
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                if (options.CameraOptions == CameraOptions.movingObjectCamera)
                {
                    _movingObjectCamera.Position -= _movingObjectCamera.Up * speed * (float)e.Time; // Down
                    ((MovingObject)data.Last()).MoveObject(_movingObjectCamera.Up * speed * -(float)e.Time);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(_movingObjectCamera.Up * speed * -(float)e.Time);
                }
                else
                {
                    ((MovingObject)data.Last()).MoveObject(new Vector3(0, speed * -(float)e.Time, 0));
                    _movingObjectCamera.Position += new Vector3(0, speed * -(float)e.Time, 0);
                    foreach (var spotLight in spotLights)
                        spotLight.MoveLight(new Vector3(0, speed * -(float)e.Time, 0));
                }
            }
            if (input.IsKeyDown(Keys.B))
            {
                options.ShadersOptions = ShadersOptions.phong;
            }
            if (input.IsKeyDown(Keys.N))
            {
                options.ShadersOptions = ShadersOptions.gouraud;
            }
            if (input.IsKeyDown(Keys.M))
            {
                options.ShadersOptions = ShadersOptions.constant;
            }
            if (input.IsKeyDown(Keys.D1))
            {
                options.CameraOptions = CameraOptions.movingObjectCamera;
            }
            if (input.IsKeyDown(Keys.D2))
            {
                options.CameraOptions = CameraOptions.staticCamera;
            }
            if (input.IsKeyDown(Keys.D3))
            {
                options.CameraOptions = CameraOptions.staticCameraFollowingObj;
            }
            if (input.IsKeyDown(Keys.L))
            {
                options.VibrationsEnabled = true;
            }
            if (input.IsKeyDown(Keys.K))
            {
                options.VibrationsEnabled = false;
            }
            if (input.IsKeyDown(Keys.O))
            {
                options.whichSpotLightToModify = 0;
            }
            if (input.IsKeyDown(Keys.P))
            {
                options.whichSpotLightToModify = 1;
            }
            if (input.IsKeyDown(Keys.Left))
            {
                spotLights[options.whichSpotLightToModify].ChangeDirection(new Vector3(speed * -(float)e.Time, 0, 0));
            }
            if (input.IsKeyDown(Keys.Right))
            {
                spotLights[options.whichSpotLightToModify].ChangeDirection(new Vector3(speed * (float)e.Time, 0, 0));
            }
            if (input.IsKeyDown(Keys.Up))
            {
                spotLights[options.whichSpotLightToModify].ChangeDirection(new Vector3(0, speed * (float)e.Time, 0));
            }
            if (input.IsKeyDown(Keys.Down))
            {
                spotLights[options.whichSpotLightToModify].ChangeDirection(new Vector3(0, speed * -(float)e.Time, 0));
            }

            ((MovingObject)data.Last()).model.Row3.Deconstruct(out var x, out var y, out var z, out var w);
            var cameraDirection = Vector3.Normalize(_staticCameraFollowingObj.Position - new Vector3(x, y, z));
            _staticCameraFollowingObj.UpdateFront(-cameraDirection);
            ((MovingObject)data.Last()).RotateObject(_time);
            if (options.VibrationsEnabled)
                ((MovingObject)data.Last()).VibrateObject();

            sun.ChangeDirection();
            options.UpdateFogDensity();

            var mouse = MouseState;

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _movingObjectCamera.Yaw += deltaX * sensitivity;
                _movingObjectCamera.Pitch -= deltaY * sensitivity;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _movingObjectCamera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _movingObjectCamera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}