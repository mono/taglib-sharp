//
// ByteVectorList.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//   Aaron Bockover (abockover@novell.com)
//
// Original Source:
//   tbytevectorlist.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
// Copyright (C) 2006 Novell, Inc.
// Copyright (C) 2002,2003 Scott Wheeler (Original Implementation)
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TagLib {
	[ComVisible(false)]
	public class ByteVectorCollection : ListBase<ByteVector>
	{
		public ByteVectorCollection() 
		{
		}

		public ByteVectorCollection(IEnumerable<ByteVector> list)
		{
			Add (list);
		}

		public ByteVectorCollection (params ByteVector[] list)
		{
			Add (list);
		}

		public override void SortedInsert(ByteVector item, bool unique)
		{
			int i = 0;
			for(; i < Count; i++) {
				if (item == this[i] && unique)
					return;

				if (item >= this[i])
					break;
			}

			Insert (i + 1, item);
		}

		public ByteVector ToByteVector(ByteVector separator)
		{
			if (separator == null)
				throw new ArgumentNullException ("separator");
			
			ByteVector vector = new ByteVector();
			
			for(int i = 0; i < Count; i++) {
				if(i != 0 && separator.Count > 0)
					vector.Add(separator);
				
				vector.Add(this[i]);
			}
			
			return vector;
		}

		public static ByteVectorCollection Split (ByteVector vector,
		                                          ByteVector pattern,
		                                          int byteAlign,
		                                          int max)
		{
			if (vector == null)
				throw new ArgumentNullException ("vector");
			
			if (pattern == null)
				throw new ArgumentNullException ("pattern");
			
			ByteVectorCollection list = new ByteVectorCollection();
			int previous_offset = 0;
			
			for (int offset = vector.Find(pattern, 0, byteAlign);
				offset != -1 && (max == 0 ||
					max > list.Count + 1);
				offset = vector.Find (pattern,
					offset + pattern.Count, byteAlign)) {
				list.Add (vector.Mid (previous_offset,
					offset - previous_offset));
				previous_offset = offset + pattern.Count;
			}
			
			if (previous_offset < vector.Count)
				list.Add (vector.Mid (previous_offset,
					vector.Count - previous_offset));
			
			return list;
		}

		public static ByteVectorCollection Split (ByteVector vector,
		                                          ByteVector pattern,
		                                          int byteAlign)
		{
			return Split(vector, pattern, byteAlign, 0);
		}
		
		public static ByteVectorCollection Split (ByteVector vector,
		                                          ByteVector pattern)
		{
			return Split(vector, pattern, 1);
		}
	}
}

