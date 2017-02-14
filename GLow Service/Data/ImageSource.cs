﻿//
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

using SQLite;

namespace GLow_Screensaver.Data
{
    /// <summary>
    /// Source for the image.
    /// </summary>
    [Table("imagesource")]
	public class ImageSource
	{
        /// <summary>
        /// Identity.
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
        [Column("id")]
		public int Id {get; set;}
		
		/// <summary>
		/// Source code of the shader.
		/// </summary>
		[Column("sourceImage")]
		public string SourceCode {get;set;}

        /// <summary>
        /// Source for the image.
        /// </summary>
        [Column("shader")]
        public int Shader { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ImageSource()
		{
		}
	}
}
