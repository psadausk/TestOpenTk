// This code was written for the OpenTK library and has been released
// to the Public Domain.
// It is provided "as is" without express or implied warranty of any kind.

using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Examples.Tutorial {
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>

    public class SimpleWindow : GameWindow {
        public float angle { get; set; }

        public float CameraXAngle { get; set; }
        public float CameraYAngle { get; set; }
        public float CameraXPos { get; set; }
        public float CameraYPos { get; set; }
        public float DeltaX { get; set; }
        public float DeltaY { get; set; }

        private float m_facing = 0.0f;
        private float m_pitch = 0.0f;
        private Vector2 m_mouseSpeed;
        private Vector2 m_mouseDelta;
        private Vector3 m_cameraLocation;
        private Matrix4 m_cameraMatrix;
        private Vector3 m_upVector = Vector3.UnitY;




        public bool RotateLeft { get; set; }
        public bool RotateRight { get; set; }
        public bool RotateUp { get; set; }
        public bool RotateDown { get; set; }
        public bool IsDragging { get; set; }

        public SimpleWindow()
            : base(800, 600) {
            //Keyboard.KeyDown += Keyboard_KeyDown;
            //Keyboard.KeyUp += (Keyboard_KeyUp);
            this.m_cameraMatrix = Matrix4.Identity;
            this.m_cameraLocation = new Vector3(0, 20, 25);
            this.m_facing = 5.0f;
            //this.m_pitch = 5.0f;
            Mouse.Move += MouseOnMove;
            Mouse.ButtonDown += MouseOnButtonDown;
            Mouse.ButtonUp += MouseOnButtonUp;

        }

        private void MouseOnButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
            if (mouseButtonEventArgs.Button == MouseButton.Right) {
                this.IsDragging = false;
            }
        }

        private void MouseOnMove(object sender, MouseMoveEventArgs e) {
            if (!this.IsDragging) return;
            this.m_mouseDelta = new Vector2(e.XDelta, e.YDelta);
            //this.CameraXAngle += ((float)e.XDelta / this.Width * 360);
            //this.CameraYAngle += ((float)e.YDelta / this.Height * 360);
        }

        private void MouseOnButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
            if (mouseButtonEventArgs.Button == MouseButton.Right) {
                this.IsDragging = true;
            }
        }

        #region Keyboard Events

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The KeyboardDevice which generated this event.</param>
        /// <param name="e">The key that was pressed.</param>
        //void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e) {
        //    if (e.Key == Key.Escape)
        //        this.Exit();
        //    if (e.Key == Key.Left) {
        //        this.RotateLeft = true;
        //    } else if (e.Key == Key.Right) {
        //        this.RotateRight = true;
        //    } else if (e.Key == Key.Up) {
        //        this.RotateUp = true;
        //    } else if (e.Key == Key.Down) {
        //        this.RotateDown = true;
        //    }


        //    if (e.Key == Key.F11)
        //        if (this.WindowState == WindowState.Fullscreen)
        //            this.WindowState = WindowState.Normal;
        //        else
        //            this.WindowState = WindowState.Fullscreen;
        //}
        //void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e) {
        //    if (e.Key == Key.Left) {
        //        this.RotateLeft = false;
        //    } else if (e.Key == Key.Right) {
        //        this.RotateRight = false;
        //    } else if (e.Key == Key.Up) {
        //        this.RotateUp = false;
        //    } else if (e.Key == Key.Down) {
        //        this.RotateDown = false;
        //    }
        //}

        #endregion

        #region OnLoad

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e) {
            GL.ClearColor(Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);

        }

        #endregion

        #region OnResize

        /// <summary>
        /// Respond to resize events here.
        /// </summary>
        /// <param name="e">Contains information on the new GameWindow size.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnResize(EventArgs e) {


            GL.Viewport(0, 0, this.Width, this.Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }

        #endregion

        #region OnUpdateFrame

        /// <summary>
        /// Add your game logic here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnUpdateFrame(FrameEventArgs e) {
            this.UpdateKeyboardEvents();
            this.m_mouseSpeed.X *= 0.9f;
            this.m_mouseSpeed.Y *= 0.9f;
            this.m_mouseSpeed.X += this.m_mouseDelta.X / 1000f;
            this.m_mouseSpeed.Y += this.m_mouseDelta.Y / this.Height;
            this.m_mouseDelta = new Vector2();

            this.m_facing += this.m_mouseSpeed.X;
            this.m_pitch -= this.m_mouseSpeed.Y;

            //Limit the Y camera to a certain pitch, to avoid losing focus
            if (this.m_pitch >= 0.0f) {
                this.m_pitch = 0.0f;
            } else if (this.m_pitch <= -2.0f) {
                this.m_pitch = -2.0f;
            }

            //TODO The Camera location should always be looking at at least some of the board, but I'm not sure how to make that check.


            var lookatPoint = new Vector3((float)Math.Cos(this.m_facing), this.m_pitch, (float)Math.Sin(this.m_facing));
            this.m_cameraMatrix = Matrix4.LookAt(this.m_cameraLocation, this.m_cameraLocation + lookatPoint, this.m_upVector);

        }

        private void UpdateKeyboardEvents() {
            if (Keyboard[Key.Escape]) {
                this.Exit();
            }

            if (Keyboard[Key.W]) {
                this.m_cameraLocation.X += (float)Math.Cos(this.m_facing) * 0.5f;
                this.m_cameraLocation.Z += (float)Math.Sin(this.m_facing) * 0.5f;
            }
            if (Keyboard[Key.A]) {
                this.m_cameraLocation.X -= (float)Math.Cos(this.m_facing + Math.PI / 2) * 0.1f;
                this.m_cameraLocation.Z -= (float)Math.Sin(this.m_facing + Math.PI / 2) * 0.1f;
            }
            if (Keyboard[Key.S]) {
                this.m_cameraLocation.X -= (float)Math.Cos(this.m_facing) * 0.1f;
                this.m_cameraLocation.Z -= (float)Math.Sin(this.m_facing) * 0.1f;
            }
            if (Keyboard[Key.D]) {
                this.m_cameraLocation.X += (float)Math.Cos(this.m_facing + Math.PI / 2) * 0.1f;
                this.m_cameraLocation.Z += (float)Math.Sin(this.m_facing + Math.PI / 2) * 0.1f;
            }

        }

        #endregion

        #region OnRenderFrame

        /// <summary>
        /// Add your game rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e) {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadMatrix(ref this.m_cameraMatrix);

            for (int r = -26; r < 26; r++) {
                for (int c = -26; c < 26; c++) {
                    this.DrawCube(r, c, Math.Abs(r * c) % 10);
                }
            }


            this.SwapBuffers();
        }


        private void DrawCube(float r, float c, float h) {

            GL.Begin(BeginMode.Quads);

            //Top
            GL.Color3(Color.Green);
            GL.Vertex3(r + 0.0f, h + 1.0f, c + 0.0f);
            GL.Vertex3(r + 1.0f, h + 1.0f, c + 0.0f);
            GL.Vertex3(r + 1.0f, h + 1.0f, c + -1.0f);
            GL.Vertex3(r + 0.0f, h + 1.0f, c + -1.0f);


            //Right
            GL.Color3(Color.Yellow);
            GL.Vertex3(r + 1.0f, 0.0f, c + 0.0f);
            GL.Vertex3(r + 1.0f, h + 1.0f, c + 0.0f);
            GL.Vertex3(r + 1.0f, h + 1.0f, c + -1.0f);
            GL.Vertex3(r + 1.0f, 0.0f, c + -1.0f);


            //Front
            GL.Color3(Color.Black);
            GL.Vertex3(r + 0.0f, 0.0f, c + 0.0f);
            GL.Vertex3(r + 1.0f, 0.0f, c + 0.0f);
            GL.Vertex3(r + 1.0f, h + 1.0f, c + 0.0f);
            GL.Vertex3(r + 0.0f, h + 1.0f, c + 0.0f);

            //Left
            GL.Color3(Color.Red);
            GL.Vertex3(r + 0.0f, 0.0f, c + 0.0f);
            GL.Vertex3(r + 0.0f, h + 1.0f, c + 0.0f);
            GL.Vertex3(r + 0.0f, h + 1.0f, c + -1.0f);
            GL.Vertex3(r + 0.0f, 0.0f, c + -1.0f);

            //Back
            GL.Color3(Color.Blue);
            GL.Vertex3(r + 0.0f, 0.0f, c + -1.0f);
            GL.Vertex3(r + 0.0f, h + 1.0f, c + -1.0f);
            GL.Vertex3(r + 1.0f, h + 1.0f, c + -1.0f);
            GL.Vertex3(r + 1.0f, 0.0f, c + -1.0f);

            //Bottom
            GL.Color3(Color.White);
            GL.Vertex3(r + 0.0f, 0.0f, c + 0.0f);
            GL.Vertex3(r + 1.0f, 0.0f, c + 0.0f);
            GL.Vertex3(r + 1.0f, 0.0f, c + -1.0f);
            GL.Vertex3(r + 0.0f, 0.0f, c + -1.0f);
            GL.End();
        }
        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main() {
            using (SimpleWindow example = new SimpleWindow()) {
                // Get the title and category  of this example using reflection.

                example.Run(30.0, 0.0);
            }
        }

        #endregion
    }
}
