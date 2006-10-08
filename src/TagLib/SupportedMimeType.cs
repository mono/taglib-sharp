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
using System.Collections;

namespace TagLib
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class SupportedMimeType : Attribute 
    {
        private static ArrayList mime_types = new ArrayList();
    
        private string mime_type;
        private string extension;
    
        public string MimeType {
            get { return mime_type; }
        }

        public string Extension {
            get { return extension; }
        }

        public SupportedMimeType(string mime_type)
        {
            this.mime_type = mime_type;
            mime_types.Add(this);
        }

        public SupportedMimeType(string mime_type, string extension) : this(mime_type) 
        {
            this.extension = extension;
        }

        public IEnumerator AllMimeTypes {
            get { 
                foreach(SupportedMimeType type in mime_types) {
                    yield return type.MimeType;
                }
            }
        }

        public IEnumerator AllExtensions {
            get {
                foreach(SupportedMimeType type in mime_types) {
                    if(type.Extension != null) {
                        yield return type.Extension;
                    }
                }
            }
        }
    }
}
