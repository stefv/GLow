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

using System.ServiceProcess;
using System.Timers;

namespace GLowService
{
    public partial class GlowService : ServiceBase
    {
        /// <summary>
        /// Number of hours before to download the shaders.
        /// </summary>
        private const int HOURS_BEFORE_EVENT = 5;

        /// <summary>
        /// The timer.
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// true if the service must stop.
        /// </summary>
        private bool _requiredStop = false;

        /// <summary>
        /// The service to download the shaders.
        /// </summary>
        private ShaderDownloader _shaderDownloader;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GlowService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Start the service.
        /// </summary>
        /// <param name="args">Arguments of the service.</param>
        protected override void OnStart(string[] args)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif

            CreateDatabase();

            _shaderDownloader = new ShaderDownloader();

            _timer = new Timer(1000 * 3600 * HOURS_BEFORE_EVENT);
            _timer.Elapsed += timer_Elapsed;
        }

        /// <summary>
        /// Stop the service.
        /// </summary>
        protected override void OnStop()
        {
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
            _shaderDownloader.download();
        }
    }
}
