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

using SQLite;
using System;
using System.ComponentModel;

namespace GLow_Screensaver.Data
{
    /// <summary>
    /// Description of the shader.
    /// </summary>
    [Table("shader")]
    public class Shader : INotifyPropertyChanged
    {
        /// <summary>
        /// Identity.
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
        [Column("id")]
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        private int _id;

        /// <summary>
        /// Identity in Shadertoy.
        /// </summary>
        [Unique]
        [Column("shadertoyid")]
        public string ShadertoyID { get; set; }

        /// <summary>
        /// Name of shader.
        /// </summary>
        [Column("name")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private string _name;

        /// <summary>
        /// Type of shader (HLSL or GLSL).
        /// </summary>
        [Column("type")]
        public string Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    RaisePropertyChanged("Type");
                }
            }
        }

        private string _type;

        /// <summary>
        /// Name of shader.
        /// </summary>
        [Column("description")]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        private string _description;

        /// <summary>
        /// Author.
        /// </summary>
        [Column("author")]
        public string Author { get; set; }

        /// <summary>
        /// Last update from Shadertoy.
        /// </summary>
        [Column("lastupdate")]
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Read only shader. Only copies can be done.
        /// </summary>
        [Column("readonly")]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// true if it's a favorite.
        /// </summary>
        [Column("favorite")]
        public bool Favorite
        {
            get { return _favorite; }
            set
            {
                if (_favorite != value)
                {
                    _favorite = value;
                    RaisePropertyChanged("Favorite");
                }
            }
        }

        private bool _favorite;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Shader()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Shader(Shader src)
        {
            Id = src.Id;
            ShadertoyID = src.ShadertoyID;
            Name = src.Name;
            Description = src.Description;
            Author = src.Author;
            LastUpdate = src.LastUpdate;
            ReadOnly = src.ReadOnly;
            Favorite = src.Favorite;
        }

        /// <summary>
        /// Constructor with the name.
        /// </summary>
        /// <param name="name">Name of the shader.</param>
        public Shader(string name)
        {
            this.Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is Shader)) return false;

            Shader shader2 = (Shader)obj;
            if (shader2.Id != this.Id) return false;
            if (shader2.ShadertoyID != this.ShadertoyID) return false;
            if (shader2.Name != this.Name) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (ShadertoyID + "|" + Name + "|" + Author).GetHashCode();
        }
    }
}
