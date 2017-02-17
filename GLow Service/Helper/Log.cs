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

using System.Diagnostics;

namespace GLowService.Helper
{
    /// <summary>
    /// Logger to the event log.
    /// </summary>
    public sealed class Log
    {
        /// <summary>
        /// The source for the system log.
        /// </summary>
        private const string EVENTLOG_SOURCE = "GLow Screensaver Service";

        /// <summary>
        /// Log name.
        /// </summary>
        private const string LOG_NAME = "GlowLog";

        /// <summary>
        /// The inner log.
        /// </summary>
        private static EventLog _myLog;

        /// <summary>
        /// Static constructor to initialize the logger.
        /// </summary>
        static Log()
        {
            // Create the source if it doesn't exist
            if (!EventLog.SourceExists(EVENTLOG_SOURCE)) EventLog.CreateEventSource(EVENTLOG_SOURCE, LOG_NAME);

            // Create an EventLog instance and assign its source
            _myLog = new EventLog();
            _myLog.Source = EVENTLOG_SOURCE;
        }

        /// <summary>
        /// Static calls only.
        /// </summary>
        private Log() { }

        /// <summary>
        /// Write a message in the event log.
        /// </summary>
        /// <param name="eventID">The ID of the event.</param>
        /// <param name="msg">The message.</param>
        public static void Info(int eventID, string msg)
        {
            _myLog.WriteEntry(msg, EventLogEntryType.Information, eventID);
        }
    }
}
