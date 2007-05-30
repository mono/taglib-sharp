/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
                         : (C) 2006 Novell, Inc.
    email                : brian.nickel@gmail.com
                         : Aaron Bockover <abockover@novell.com>
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
using System.Runtime.InteropServices;

namespace TagLib
{
    [ComVisible(false)]
    public class StringCollection : ListBase<string>
    {
        public StringCollection () 
        {
        }

        public StringCollection(StringCollection values)
        {
            Add (values);
        }
        
        public StringCollection(params string [] values)
        {
            Add (values);
        }

        public StringCollection(ByteVectorCollection vectorList, StringType type)
        {
            foreach(ByteVector vector in vectorList) {
                Add(vector.ToString(type));
            }
        }

        public StringCollection(ByteVectorCollection vectorList) : this(vectorList, StringType.UTF8) 
        {
        }
        
        public static StringCollection Split (string value, string pattern)
        {
           if (value == null)
              throw new ArgumentNullException ("value");
           
           if (pattern == null)
              throw new ArgumentNullException ("pattern");
           
           StringCollection list = new StringCollection ();

           int previous_position = 0;
           int position = value.IndexOf (pattern, 0);
           int pattern_length = pattern.Length;
            
           while (position != -1) {
              list.Add (value.Substring(previous_position, position - previous_position));
              previous_position = position + pattern_length;
              position = value.IndexOf (pattern, previous_position);
           }

           list.Add (value.Substring (previous_position));
            
           return list;
        }
    }
}
