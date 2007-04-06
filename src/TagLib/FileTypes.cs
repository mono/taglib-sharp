/***************************************************************************
    copyright            : (C) 2006 Novell, Inc.
    email                : abockover@novell.com
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
    public static class FileTypes
    {
        private static Dictionary<string, Type> file_types;

        // A static Type array is used instead of getting types by
        // reflecting the executing assembly as Assembly.GetTypes is very
        // inefficient and leaks every type instance under Mono.
        // Not reflecting taglib-sharp.dll saves about 120KB of heap
        private static Type [] static_file_types = new Type [] {
            typeof(TagLib.Asf.File),
            typeof(TagLib.Flac.File),
            typeof(TagLib.Mpc.File),
            typeof(TagLib.Mpeg4.File),
            typeof(TagLib.Mpeg.File),
            typeof(TagLib.Ogg.File),
            typeof(TagLib.WavPack.File)
        };

        static FileTypes()
        {
            Init();
        }

        internal static void Init()
        {
            if(file_types != null) {
                return;
            }
            
            file_types = new Dictionary<string, Type>();
            
            foreach(Type type in static_file_types) {
                Attribute [] attrs = Attribute.GetCustomAttributes(type, typeof(SupportedMimeType));
                if(attrs == null || attrs.Length == 0) {
                    continue;
                }

                foreach(SupportedMimeType attr in attrs) {
                    file_types.Add(attr.MimeType, type);
                } 
            }
        }

        public static IDictionary<string, Type> AvailableTypes {
            get { return file_types; }
        }
    }
}

