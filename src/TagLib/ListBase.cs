/***************************************************************************
    copyright            : (C) 2006 Novell, Inc.
    email                : Aaron Bockover <abockover@novell.com>
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
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace TagLib
{
    public class ListBase<T> : IList<T> where T : IComparable<T>
    {
        protected List<T> data = new List<T>();
        
        #region Constructors
        
        public ListBase() 
        {
        }

        public ListBase(T s)
        {
            Add(s);
        }
        
        public ListBase(ListBase<T> s)
        {
            Add(s);
        }
        
        public ListBase(T [] s)
        {
            Add(s);
        }

        #endregion
        
        #region Properties
        
        public bool IsEmpty {
            get { return Count == 0; }
        }
      
        #endregion
        
        #region Methods

        public void Add(ListBase<T> list)
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
      
        public virtual void SortedInsert(T s, bool unique)
        {
            int i = 0;
            for(; i < data.Count; i++) {
                if(s.CompareTo(data[i]) == 0 && unique) {
                    return;
                }
                
                if(s.CompareTo(data[i]) <= 0) {
                    break;
                }
            }
            
            Insert(i, s);
        }
      
        public void SortedInsert(T s)
        {
            SortedInsert(s, false);
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

        public void Add(T value)
        {
            data.Add(value);
        }
        
        public void Clear()
        {
            data.Clear();
        }
        
        public bool Contains(T value)
        {
            return data.Contains(value);
        }
        
        public int IndexOf(T value)
        {
            return data.IndexOf(value);
        }
        
        public void Insert(int index, T value)
        {
            data.Insert(index, value);
        }
        
        public bool Remove(T value)
        {
            return data.Remove(value);
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

            return builder.ToString();
        }

        public override string ToString()
        {
            return ToString(", ");
        }

        #endregion
        
        #region ICollection<T>
        
        public int Count {
            get { return data.Count; }
        }
        
        public bool IsSynchronized { 
            get { return false; }
        }
        
        public object SyncRoot { 
            get { return this; }
        }
        
        public void CopyTo(T [] array, int index)
        {
            data.CopyTo(array, index);
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
