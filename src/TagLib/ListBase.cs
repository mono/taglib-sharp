//
// ListBase.cs:
//
// Author:
//   Aaron Bockover (abockover@novell.com)
//
// Original Source:
//   tbytevectorlist.cpp from TagLib
//
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
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace TagLib {
	public class ListBase<T> : IList<T> where T : IComparable<T>
	{
		private List<T> data = new List<T>();

		#region Constructors

		public ListBase() 
		{
		}

		public ListBase(ListBase<T> list)
		{
			Add(list);
		}

		public ListBase(params T [] list)
		{
			Add(list);
		}

		#endregion

		#region Properties

		public bool IsEmpty {
			get {return Count == 0;}
		}

		#endregion

		#region Methods

		public void Add(ListBase<T> list)
		{
			if(list != null) {
				data.AddRange(list);
			}
		}

		public void Add(IEnumerable<T> list)
		{
			if(list != null) {
				data.AddRange(list);
			}
		}

		public void Add(T [] list)
		{
			if(list != null) {
				data.AddRange(list);
			}
		}

		public virtual void SortedInsert(T item, bool unique)
		{
			int i = 0;
			for(; i < data.Count; i++) {
				if(item.CompareTo(data[i]) == 0 && unique) {
					return;
				}

				if(item.CompareTo(data[i]) <= 0) {
					break;
				}
			}

			Insert(i, item);
		}

		public void SortedInsert(T item)
		{
			SortedInsert(item, false);
		}

		public T [] ToArray()
		{
			return data.ToArray();
		}

		#endregion

		#region IList<T>

		public bool IsReadOnly {
			get { return false; }
		}

		public bool IsFixedSize {
			get { return false; }
		}

		public T this[int index] {
			get { return data[index]; }
			set { data[index] = value; }
		}

		public void Add(T item)
		{
			data.Add(item);
		}

		public void Clear()
		{
			data.Clear();
		}

		public bool Contains(T item)
		{
			return data.Contains(item);
		}

		public int IndexOf(T item)
		{
			return data.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			data.Insert(index, item);
		}

		public bool Remove(T item)
		{
			return data.Remove(item);
		}

		public void RemoveAt(int index)
		{
			data.RemoveAt(index);
		}

		public string ToString(string separator)
		{
			StringBuilder builder = new StringBuilder();

			for(int i = 0; i < Count; i++) {
				if(i != 0) {
					builder.Append(separator);
				}

				builder.Append(this[i].ToString());
			}

			return builder.ToString ();
		}

		public override string ToString ()
		{
			return ToString(", ");
		}

		#endregion

		#region ICollection<T>

		public int Count {
			get {return data.Count;}
		}

		public bool IsSynchronized { 
			get {return false;}
		}

		public object SyncRoot { 
			get {return this;}
		}

		public void CopyTo(T [] array, int arrayIndex)
		{
			data.CopyTo(array, arrayIndex);
		}

		#endregion

		#region IEnumerable<T>

		IEnumerator IEnumerable.GetEnumerator()
		{
			return data.GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		#endregion
	}
}