//
// GLow screensaver
// Copyright(C) Stéphane VANPOPERYNGHE
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or(at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//

using GLowScreensaver;
using Microsoft.Win32;
using OpenTK.Graphics.OpenGL;
using SQLite;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GLow_Screensaver.Controls
{
    /// <summary>
    /// Control displaying the OpenGL animation version.
    /// </summary>
    public partial class GLScreensaverControl : UserControl
    {
        /// <summary>
        /// Activate or deactivate the preview mode. With this mode, the CTRL key will not display the frame rate.
        /// </summary>
        public bool IsPreview
        {
            get { return (bool)GetValue(IsPreviewProperty); }
            set { SetValue(IsPreviewProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsPreview.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsPreviewProperty = DependencyProperty.Register("IsPreview", typeof(bool), typeof(GLScreensaverControl), new PropertyMetadata(false));

        /// <summary>
        /// Display or not the FPS counter.
        /// </summary>
        public bool IsShowFPS
        {
            get { return (bool)GetValue(IsShowFPSProperty); }
            set { SetValue(IsShowFPSProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowFPS.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowFPSProperty = DependencyProperty.Register("IsShowFPS", typeof(bool), typeof(GLScreensaverControl), new PropertyMetadata(false, new PropertyChangedCallback(IsShowFPS_PropertyChanged)));

        /// <summary>
        /// OpenGL program.
        /// </summary>
        private int _glProgram = 0;

        /// <summary>
        /// OpenGL fragment shader.
        /// </summary>
        private int _glFramentShader = 0;

        private bool _isDesignMode = false;

        private DateTime _startTime;

        private bool _glInitialized = false;

        private Point _mousePosition = new Point(0, 0);

        private uint _fps = 0;

        private DateTime _timeFPS = DateTime.Now;

        private DateTime _timeVisibleMouse = DateTime.Now;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GLScreensaverControl()
        {
            _isDesignMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

            InitializeComponent();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private static void IsShowFPS_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GLScreensaverControl ctrl = (GLScreensaverControl)sender;
            //Debug.WriteLine("ctrl.IsShowFPS:" + ctrl.IsShowFPS);
            //ctrl.FPSPopup.IsOpen = ctrl.IsShowFPS; // FIXME Activate again this FPS counter
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (_glInitialized) glControl.Invalidate();
        }

        /*private bool InitializeShader()
        {
            // Prépare le pixel shader			
            string source = GetPixelShaderSource();
            int pixelShader = GL.CreateShader(ShaderType.FragmentShader);

            source = "uniform vec3  iResolution;\r\n" + source;
            source = "uniform float iGlobalTime;" + source;
            source = "uniform vec4 iMouse;" + source;
            source = "out vec4 out_frag_color;" + source;
            source = source + "\r\nvoid main(void) {vec4 fragColor;mainImage(fragColor,gl_FragCoord.xy);out_frag_color=fragColor;}";

            GL.ShaderSource(pixelShader, source);
            GL.CompileShader(pixelShader);
            string result = GL.GetShaderInfoLog(pixelShader);
            GL.DeleteShader(pixelShader);
            if (result.Contains("error"))
            {
                Debug.WriteLine(result);
                Debug.WriteLine("--------------------------------------------------------------------");
                Debug.WriteLine(source);
                //Application.Current.Dispatcher.Invoke(new Action(()=>{MessageBox.Show(result);}));
                return false;
            }

            return true;
        }*/

        /*private string GetPixelShaderSource()
        {

            foreach (string s in Assembly.GetExecutingAssembly().GetManifestResourceNames()) Debug.WriteLine(s);

            Stream fragmentShaderStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(PIXEL_SHADER_FILE);

            // Ajoute les variables
            // uniform vec3      iResolution;           // viewport resolution (in pixels)
            // uniform float     iGlobalTime;           // shader playback time (in seconds)
            // uniform vec4      iMouse;                // mouse pixel coords. xy: current (if MLB down), zw: click
            // uniform float     iChannelTime[4];       // channel playback time (in seconds)
            // uniform vec3      iChannelResolution[4]; // channel resolution (in pixels)
            // uniform samplerXX iChannel0..3;          // input channel. XX = 2D/Cube
            // uniform vec4      iDate;                 // (year, month, day, time in seconds)
            // uniform float     iSampleRate;     
            string fragmentShaderSource = new StreamReader(fragmentShaderStream).ReadToEnd();

            return fragmentShaderSource;
        }*/

        private void GlControl_Load(object sender, EventArgs e)
        {
            if (!_isDesignMode)
            {
                GL.ClearColor(0.0f, 1.0f, 0.0f, 1.0f);

                // Lit le contenu du shader
                //if (!InitializeShader()) return;

                // Heure de début pour iGlobalTime
                _startTime = Process.GetCurrentProcess().StartTime;

                // Prépare le vertex shader			
                int vertexShader = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(vertexShader, "void main(void) {gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;}");
                GL.CompileShader(vertexShader);

                // Crée le programme
                _glProgram = GL.CreateProgram();
                GL.AttachShader(_glProgram, vertexShader);
                GL.LinkProgram(_glProgram);
                GL.UseProgram(_glProgram);

                //InitializeFragmentShader(GetPixelShaderSource());

                SetupViewport();

                _glInitialized = true;
            }
        }

        /// <summary>
        /// Initialize the fragment shader using the current settings.
        /// </summary>
        public void InitializeFragmentShader()
        {
            RegistryKey folder = Registry.CurrentUser.CreateSubKey(@"Software\GLow Screensaver\");
            if (folder.GetValue("ShaderId") != null)
            {
                int shaderId = (int)folder.GetValue("ShaderId");
                SQLiteConnection db = Database.Instance.GetConnection();
                GLow_Screensaver.Data.ImageSource source = (from s in db.Table<GLow_Screensaver.Data.ImageSource>() where s.Id == shaderId select s).FirstOrDefault();
                if (source != null) InitializeFragmentShader(source.SourceCode);
            }
        }

        public void InitializeFragmentShader(string imageSourceCode)
        {
            if (_glProgram > 0)
            {
                imageSourceCode = "uniform vec3  iResolution;\r\n" + imageSourceCode;
                imageSourceCode = "uniform float iGlobalTime;" + imageSourceCode;
                imageSourceCode = "uniform vec4 iMouse;" + imageSourceCode;
                imageSourceCode = "uniform vec4 iDate;" + imageSourceCode;
                imageSourceCode = "out vec4 out_frag_color;" + imageSourceCode;
                imageSourceCode = "#version 150\r\n" + imageSourceCode;

                imageSourceCode = imageSourceCode + "\r\nvoid main(void) {vec4 fragColor;mainImage(fragColor,gl_FragCoord.xy);out_frag_color=fragColor;}";

                // Delete the current fragment shader
                if (_glFramentShader > 0)
                {
                    GL.DetachShader(_glProgram, _glFramentShader);
                    GL.DeleteShader(_glFramentShader);
                    _glFramentShader = 0;
                }

                // Create the new fragment shader
                _glFramentShader = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(_glFramentShader, imageSourceCode);
                GL.CompileShader(_glFramentShader);
                string result = GL.GetShaderInfoLog(_glFramentShader).Trim();

                if (result != "" && result.ToLower() != "no errors.")
                {
                    Debug.WriteLine(result);
                    MessageBox.Show(result);
                }


                // Attach the fragment shader to the program
                GL.AttachShader(_glProgram, _glFramentShader);
                GL.LinkProgram(_glProgram);
                GL.UseProgram(_glProgram);
            }
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            if (!_isDesignMode)
            {
                SetupViewport();
                glControl.Invalidate();
            }
        }

        private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (!_isDesignMode) Render();
        }

        private void Render()
        {
            if (!_isDesignMode)
            {
                glControl.MakeCurrent();

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                // Définit le paramètre iResolution
                float w = (float)ActualWidth;
                float h = (float)ActualHeight;
                int iResolution = GL.GetUniformLocation(_glProgram, "iResolution");
                if (iResolution != -1) GL.Uniform3(iResolution, (float)w, (float)h, (float)0);

                // Debug.WriteLine(iResolution+" - "+w+" - "+h);

                // Définit le paramètre iGlobalTime			
                float timespan = (float)(DateTime.Now - _startTime).TotalSeconds;
                int iGlobalTime = GL.GetUniformLocation(_glProgram, "iGlobalTime");
                if (iGlobalTime != -1) GL.Uniform1(iGlobalTime, (float)timespan);

                // Définit le paramètre iDate			
                int iDate = GL.GetUniformLocation(_glProgram, "iDate");
                if (iDate != -1) GL.Uniform4(iDate, (float)DateTime.Now.Year, (float)DateTime.Now.Month, (float)DateTime.Now.Day, (float)DateTime.Now.TimeOfDay.TotalSeconds);

                // Définit le paramètre iMouse			
                int iMouse = GL.GetUniformLocation(_glProgram, "iMouse");
                if (iMouse != -1) GL.Uniform4(iMouse, (float)_mousePosition.X, (float)((double)h - _mousePosition.Y), (float)_mousePosition.X, (float)((double)h - _mousePosition.Y));

                GL.Begin(BeginMode.Quads);
                GL.Color3(0, 0, 0);
                GL.Vertex2(0, 0);
                GL.Vertex2(glControl.Width, 0);
                GL.Vertex2(glControl.Width, glControl.Height);
                GL.Vertex2(0, glControl.Height);

                GL.End();

                glControl.SwapBuffers();

                // Met à jour le FPS
                if ((DateTime.Now - _timeFPS).TotalSeconds >= 1)
                {
                    _timeFPS = DateTime.Now;
                    //FPS.Text = "FPS:" + _fps; // FIXME Display the FPS
                    _fps = 0;
                }
                _fps++;

                // Hide the mouse after a delay if it's not a preview
                if ((!IsPreview) && ((DateTime.Now - _timeVisibleMouse).TotalSeconds >= 5)) Mouse.OverrideCursor = Cursors.None;
            }
        }

        private void SetupViewport()
        {
            int w = glControl.Width;
            int h = glControl.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); //Bottom left corner pixel has corrdinate 0,0
            GL.Viewport(0, 0, w, h); //Use all of the MyGlControl painting area
        }

        private void glControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) _mousePosition = new Point(e.X, e.Y);
            Mouse.OverrideCursor = Cursors.Arrow;
            _timeVisibleMouse = DateTime.Now;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsPreview)
            {
                //if (e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl)
                //Application.Current.Shutdown();
                //else FPSPopup.IsOpen = true;
            }
        }

        private void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!IsPreview)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left) Application.Current.Shutdown();
            }
        }

        //private void Window_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (!IsPreview)
        //    {
        //        if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) FPSPopup.IsOpen = false;
        //    }
        //}
    }
}