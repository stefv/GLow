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

using Microsoft.Win32;
using OpenTK.Graphics.OpenGL;
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
    /// Control displaying the OpenGL animation.
    /// </summary>
    public partial class GLScreensaverControl : UserControl
    {
        #region Properties & attributes
        /// <summary>
        /// Activate or deactivate the preview mode. With this mode activated, the CTRL key will not display the frame rate.
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

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsPreview.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(GLScreensaverControl), new PropertyMetadata(null, new PropertyChangedCallback(Source_PropertyChanged)));

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

        /// <summary>
        /// true if the design mode is actually used.
        /// </summary>
        private bool _isDesignMode = false;

        /// <summary>
        /// Start time used with the iGlobalTime variable used in the shaders.
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        /// true if the initialization of OpenGL is done.
        /// </summary>
        private bool _glInitialized = false;

        /// <summary>
        /// Mouse position used in the shaders with the variable iMouse.
        /// </summary>
        private Point _mousePosition = new Point(0, 0);

        /// <summary>
        /// Frame rate counter. This value is the number of frames during one second.
        /// </summary>
        private uint _fps = 0;

        /// <summary>
        /// The time when the frame rate counter has been set to 0. 
        /// </summary>
        private DateTime _timeFPS = DateTime.Now;

        /// <summary>
        /// The time when the mouse has been moved for the last time. This is
        /// used to hide the cursor after a delay.
        /// </summary>
        private DateTime _timeVisibleMouse = DateTime.Now;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GLScreensaverControl()
        {
            _isDesignMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

            InitializeComponent();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }
        #endregion

        #region Initialize the control
        /// <summary>
        /// Do the settings of the OpenGL control.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Argument for this event.</param>
        private void GlControl_Load(object sender, EventArgs e)
        {
            if (!_isDesignMode)
            {
                // Set the default background color of the OpenGL control
                GL.ClearColor(0.0f, 1.0f, 0.0f, 1.0f);

                // Starting time for iGlobalTime variable
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

                SetupViewport();

                _glInitialized = true;

                if (Source != null && Source != "") InitializeFragmentShader(Source);
            }
        }
        #endregion
        #region Render the content
        /// <summary>
        /// Force the control to update the content displayed.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">The argument for this event.</param>
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (_glInitialized) glControl.Invalidate();
        }

        /// <summary>
        /// Initialize the fragment shader using the current settings.
        /// </summary>
        public void InitializeFragmentShader()
        {
            RegistryKey folder = Registry.CurrentUser.CreateSubKey(@"Software\GLow Screensaver\");
            /*if (folder.GetValue("ShaderId") != null)
            {
                int shaderId = (int)folder.GetValue("ShaderId");
                SQLiteConnection db = Database.Instance.GetConnection();
                GLow_Screensaver.Data.ImageSource source = (from s in db.Table<GLow_Screensaver.Data.ImageSource>() where s.Id == shaderId select s).FirstOrDefault();
                if (source != null) InitializeFragmentShader(source.SourceCode);
            }*/
        }

        /// <summary>
        /// Initialize the given fragment shader.
        /// </summary>
        /// <param name="imageSourceCode">Source code of the fragment shader to initialize.</param>
        public void InitializeFragmentShader(string imageSourceCode)
        {
            if (_glProgram > 0)
            {
                // Insert in the first line the global variables used by the shader
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

                // Check if some errors are existing. This method is not clean but i can't use the extensions of OpenGL.
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

        /// <summary>
        /// When the control is painted, render the content of the OpenGL control.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (!_isDesignMode) Render();
        }

        /// <summary>
        /// Render the animation.
        /// </summary>
        private void Render()
        {
            if (!_isDesignMode)
            {
                glControl.MakeCurrent();

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                // Set the resolution iResolution used by the shader
                float w = glControl.Width;
                float h = glControl.Height;
                int iResolution = GL.GetUniformLocation(_glProgram, "iResolution");
                if (iResolution != -1) GL.Uniform3(iResolution, (float)w, (float)h, (float)0);

                // Set the iGlobalTime variable used by the shader
                float timespan = (float)(DateTime.Now - _startTime).TotalSeconds;
                int iGlobalTime = GL.GetUniformLocation(_glProgram, "iGlobalTime");
                if (iGlobalTime != -1) GL.Uniform1(iGlobalTime, (float)timespan);

                // Set the iDate variable used by some shaders
                int iDate = GL.GetUniformLocation(_glProgram, "iDate");
                if (iDate != -1) GL.Uniform4(iDate, (float)DateTime.Now.Year, (float)DateTime.Now.Month, (float)DateTime.Now.Day, (float)DateTime.Now.TimeOfDay.TotalSeconds);

                // Set the position of the mouse iMouse used by the shader
                int iMouse = GL.GetUniformLocation(_glProgram, "iMouse");
                if (iMouse != -1) GL.Uniform4(iMouse, (float)_mousePosition.X, (float)((double)h - _mousePosition.Y), (float)_mousePosition.X, (float)((double)h - _mousePosition.Y));

                // Create 2 triangles where the fragment shader will be displayed
                GL.Begin(BeginMode.Quads);
                GL.Color3(0, 0, 0);
                GL.Vertex2(0, 0);
                GL.Vertex2(glControl.Width, 0);
                GL.Vertex2(glControl.Width, glControl.Height);
                GL.Vertex2(0, glControl.Height);
                GL.End();

                // Show the back buffer
                glControl.SwapBuffers();

                // Update the FPS
                if ((DateTime.Now - _timeFPS).TotalSeconds >= 1)
                {
                    _timeFPS = DateTime.Now;
                    //FPS.Text = "FPS:" + _fps; // FIXME Display the FPS
                    _fps = 0;
                }
                _fps++;

                // Hide the mouse after a delay if it's not a preview
                if ((!IsPreview) && ((DateTime.Now - _timeVisibleMouse).TotalSeconds >= 5))
                {
                    Mouse.OverrideCursor = Cursors.None;
                }
            }
        }
        #endregion
        #region Update the viewport when the size change
        /// <summary>
        /// When a resize of the control is fired, the size of the viewport of
        /// the OpenGL is changed before to invalide the content to force the 
        /// refresh.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Argument for this event.</param>
        private void GlControl_Resize(object sender, EventArgs e)
        {
            if (!_isDesignMode)
            {
                SetupViewport();
                glControl.Invalidate();
            }
        }

        /// <summary>
        /// Resize the viewport in function of the size of the control.
        /// </summary>
        private void SetupViewport()
        {
            int w = glControl.Width;
            int h = glControl.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1);    //Bottom left corner pixel has corrdinate 0,0
            GL.Viewport(0, 0, w, h);        //Use all of the MyGlControl painting area
        }
        #endregion
        #region Mouse and keyboard events
        /// <summary>
        /// Update the position of the mouse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// If a left click happened, close the application.
        /// </summary>
        /// <param name="sender">Object sending this event.</param>
        /// <param name="e">Argument for this event.</param>
        private void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // FIXME when a click happen, just fire an event that the window can catch
            if (!IsPreview)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left) Application.Current.Shutdown();
            }
        }
        #endregion

        private static void IsShowFPS_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GLScreensaverControl ctrl = (GLScreensaverControl)sender;
            //Debug.WriteLine("ctrl.IsShowFPS:" + ctrl.IsShowFPS);
            //ctrl.FPSPopup.IsOpen = ctrl.IsShowFPS; // FIXME Activate again this FPS counter
        }

        private static void Source_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GLScreensaverControl ctrl = (GLScreensaverControl)sender;
            if (ctrl.Source != null && ctrl.Source != "") ctrl.InitializeFragmentShader(ctrl.Source);
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