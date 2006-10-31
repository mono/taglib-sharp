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

namespace TagLib
{
    public class StringList : ListBase<string>
    {
        public StringList() 
        {
        }

        public StringList(string str)
        {
            Add(str);
        }
        
        public StringList(StringList list)
        {
            Add(list);
        }
        public StringList(string [] array)
        {
            Add(array);
        }

        public StringList(ByteVectorList vectorList, StringType type)
        {
            foreach(ByteVector vector in vectorList) {
                Add(vector.ToString(type));
            }
        }

        public StringList(ByteVectorList vectorList) : this(vectorList, StringType.UTF8) 
        {
        }
        
        public static StringList Split(string str, string pattern)
        {
            StringList list = new StringList();

            int previous_position = 0;
            int position = str.IndexOf(pattern, 0);
            int pattern_length = pattern.Length;
            
            while(position != -1) {
                list.Add(str.Substring(previous_position, position - previous_position));
                previous_position = position + pattern_length;
                position = str.IndexOf(pattern, previous_position);
            }

            list.Add(str.Substring(previous_position));
            
            return list;
        }
    }
}
