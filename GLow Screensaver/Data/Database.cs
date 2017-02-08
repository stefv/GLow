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

using GLow_Screensaver.Data;
using SQLite;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace GLowScreensaver
{
    /// <summary>
    /// Access class to the database.
    /// </summary>
    public sealed class Database
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        //public static string DBConnectionString = "Data Source=GLow.sqlite; Version=3;";
        public static string DBConnectionString = "GLow.sqlite";

        /// <summary>
        /// Create an unique instance of the database object.
        /// </summary>
        private static readonly Lazy<Database> _instance = new Lazy<Database>(() => new Database(), LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Return the unique instance.
        /// </summary>
        public static Database Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// Hidden constructor to use Instance.
        /// </summary>
        private Database()
        {
        }

        /// <summary>
        /// Return the connection to the database.
        /// </summary>
        /// <returns>The connection to the database.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public SQLiteConnection GetConnection()
        {
            DirectoryInfo appdata = Directory.GetParent(Application.UserAppDataPath);
            SQLiteConnection connection = new SQLiteConnection(appdata + "\\" + DBConnectionString);

            connection.CreateTable<Shader>();
            connection.CreateTable<ImageSource>();

            return connection;
        }
    }
}
