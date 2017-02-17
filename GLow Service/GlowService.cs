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
using System.Diagnostics;

namespace GLowService
{
    public partial class GlowService : ServiceBase
    {
        private const string EVENTLOG_SOURCE = "GLow Screensaver Service";

        public GlowService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();

            if (!EventLog.SourceExists(EVENTLOG_SOURCE)) EventLog.CreateEventSource(EVENTLOG_SOURCE, "GlowLog");
            
            // Create an EventLog instance and assign its source.
            EventLog myLog = new EventLog();
            myLog.Source = EVENTLOG_SOURCE;

            // Write an informational entry to the event log.    
            myLog.WriteEntry("Ca marche !");

            CreateDatabase();
        }

        protected override void OnStop()
        {
        }

        /// <summary>
        /// Create the database if it doesn't exist.
        /// </summary>
        private void CreateDatabase()
        {
            Database.Instance.GetConnection();
        }
    }
}
