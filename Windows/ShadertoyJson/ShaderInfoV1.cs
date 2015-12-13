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

using System.Collections.Generic;

namespace GLow_Screensaver.Windows.ShadertoyJson
{
    /// <summary>
    /// Informations for one shader.
    /// </summary>
    public class ShaderInfoV1
    {
        /// <summary>
        /// The ID of the shader.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The date.
        /// </summary>
        public long date { get; set; }

        /// <summary>
        /// Number of views.
        /// </summary>
        public long viewed { get; set; }

        /// <summary>
        /// The name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The username of the author.
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// The description.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Number of likes.
        /// </summary>
        public int likes { get; set; }

        /// <summary>
        /// Published of not.
        /// </summary>
        public int published { get; set; }

        /// <summary>
        /// Some flags.
        /// </summary>
        public int flags { get; set; }

        /// <summary>
        /// Tags.
        /// </summary>
        public List<string> tags { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShaderInfoV1()
        {
        }
    }
}
