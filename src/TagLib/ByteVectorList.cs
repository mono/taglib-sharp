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
using System.Collections;
using System.Collections.Generic;

namespace TagLib
{
   public class ByteVectorList : ListBase<ByteVector>
   {
        public ByteVectorList() 
        {
        }

        public ByteVectorList(ByteVector vector) 
        {
            Add(vector);
        }

        public override void SortedInsert(ByteVector vector, bool unique)
        {
            int i = 0;
            for(; i < data.Count; i++) {
                if(vector == data[i] && unique) {
                    return;
                }
                
                if(vector >= data[i]) {
                    break;
                }
            }
            
            Insert(i + 1, vector);
        }
        
        public ByteVector ToByteVector(ByteVector separator)
        {
            ByteVector vector = new ByteVector();

            for(int i = 0; i < Count; i++) {
                if(i != 0 && separator.Count > 0) {
                    vector.Add(separator);
                }
                
                vector.Add(this[i]);
            }

            return vector;
        }

        public ByteVector ToByteVector()
        {
            return ToByteVector(" ");
        }

        public static ByteVectorList Split(ByteVector vector, ByteVector pattern,
            int byteAlign, int max)
        {
            ByteVectorList list = new ByteVectorList();
            int previous_offset = 0;
            
            for(int offset = vector.Find(pattern, 0, byteAlign);
                offset != -1 && (max == 0 || max > list.Count + 1);
                offset = vector.Find(pattern, offset + pattern.Count, byteAlign)) {
                list.Add(vector.Mid(previous_offset, offset - previous_offset));
                previous_offset = offset + pattern.Count;
            }

            if(previous_offset < vector.Count) {
                list.Add(vector.Mid(previous_offset, vector.Count - previous_offset));
            }

            return list;
        }

        public static ByteVectorList Split(ByteVector vector, ByteVector pattern, int byteAlign)
        {
            return Split(vector, pattern, byteAlign, 0);
        }

        public static ByteVectorList Split(ByteVector vector, ByteVector pattern)
        {
            return Split(vector, pattern, 1);
        }
    }
}
