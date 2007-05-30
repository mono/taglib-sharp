/***************************************************************************
    copyright            : (C) 2006 Novell, Inc.
    email                : abockover@novell.com
    based on             : Entagged#
 ***************************************************************************/

/***************************************************************************
 *   This library is free software; you can redistribute it and/or modify  *
 *   it  under the terms of the GNU Lesser General Public License version  *
 *   2.1 as published by the Free Software Foundation.                     *
 *                                                                         *
 *   This library is distributed in the hope that it will be useful, but   *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   Lesser General Public License for more details.                       *
 *                                                                         *
 *   You should have received a copy of the GNU Lesser General Public      *
 *   License along with this library; if not, write to the Free Software   *
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 *   USA                                                                   *
 ***************************************************************************/
 
using System;
using System.Collections.Generic;

namespace TagLib
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class SupportedMimeType : Attribute 
    {
        private static List<SupportedMimeType> _mimetypes = new List<SupportedMimeType>();
 
        private string _mimetype;
        private string _extension;

        static SupportedMimeType()
        {
            FileTypes.Init();
        }

        public SupportedMimeType(string mimetype)
        {
            _mimetype = mimetype;
            _mimetypes.Add(this);
        }

        public SupportedMimeType(string mimetype, string extension) : this(mimetype) 
        {
            _extension = extension;
        }
    
        public string MimeType {
            get { return _mimetype; }
        }

        public string Extension {
            get { return _extension; }
        }
        
        public static IEnumerable<string> AllMimeTypes {
            get { 
                foreach(SupportedMimeType type in _mimetypes) {
                    yield return type.MimeType;
                }
            }
        }

        public static IEnumerable<string> AllExtensions {
            get {
                foreach(SupportedMimeType type in _mimetypes) {
                    if(type.Extension != null) {
                        yield return type.Extension;
                    }
                }
            }
        }
    }
}
