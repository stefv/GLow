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

using GLowCommon.Services;
using GLowService.Data;
using GLowService.Services;
using System;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace GLowService
{
    /// <summary>
    /// Service for GLow screensaver to download the shaders from ShaderToy.
    /// </summary>
    public partial class GlowService : ServiceBase
    {
        #region Constants
        /// <summary>
        /// Number of hours before to download the shaders.
        /// </summary>
        private const int HOURS_BEFORE_EVENT = 5;
        #endregion
        #region Fields
        /// <summary>
        /// The timer.
        /// </summary>
        private System.Timers.Timer _timer;

        /// <summary>
        /// true if the service must stop.
        /// </summary>
        private bool _requiredStop = false;

        /// <summary>
        /// The service to download the shaders.
        /// </summary>
        private ShaderDownloader _shaderDownloader;

        /// <summary>
        /// The host for the service service.
        /// </summary>
        private ServiceHost serviceHost;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GlowService()
        {
            InitializeComponent();
        }
        #endregion

        /// <summary>
        /// Start the service.
        /// </summary>
        /// <param name="args">Arguments of the service.</param>
        protected override void OnStart(string[] args)
        {
#if DEBUG
            //System.Diagnostics.Debugger.Launch();
#endif

            CreateDatabase();

            _shaderDownloader = new ShaderDownloader();

            ThreadPool.QueueUserWorkItem((_) => _shaderDownloader.Download(IsStopRequired));

            _timer = new System.Timers.Timer(1000 * 3600 * HOURS_BEFORE_EVENT);
            _timer.Elapsed += timer_Elapsed;

            serviceHost = new ServiceHost(typeof(ShaderService), new Uri[] {
                new Uri("net.pipe://localhost")
            });
            if (serviceHost != null)
            {
                NetNamedPipeBinding binding = new NetNamedPipeBinding();

                serviceHost.AddServiceEndpoint(typeof(IShaderService), binding, ShaderServiceConst.SERVICE_NAME);
                serviceHost.Open();
            }
        }

        /// <summary>
        /// Stop the service.
        /// </summary>
        protected override void OnStop()
        {
            if (serviceHost != null) serviceHost.Close();
            _requiredStop = true;
        }

        /// <summary>
        /// Create the database if it doesn't exist.
        /// </summary>
        private void CreateDatabase()
        {
            Database.Instance.GetConnection();
        }

        /// <summary>
        /// Download the shaders.
        /// </summary>
        /// <param name="sender">The object sending the event.</param>
        /// <param name="e">Argument for this event.</param>
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _shaderDownloader.Download(IsStopRequired);
        }

        private bool IsStopRequired()
        {
            return _requiredStop;
        }
    }
}
