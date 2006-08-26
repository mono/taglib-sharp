/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
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

using System.Collections;
using System;

namespace TagLib
{
   public class ByteVectorList : IList
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      
      private ArrayList data = new ArrayList ();
      
      
      //////////////////////////////////////////////////////////////////////////
      // IList properties
      //////////////////////////////////////////////////////////////////////////
      
      public bool IsReadOnly {get {return false;}}
      public bool IsFixedSize {get {return false;}}
      
      object IList.this [int index]
      {
         get {return this [index];}
         set {this [index] = (ByteVector)value;}
      }
      
      public ByteVector this [int index]
      {
         get {return (ByteVector) data [index];}
         set {data [index] = value;}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // IList methods
      //////////////////////////////////////////////////////////////////////////
      
      int  IList.Add (object value)     {return Add ((ByteVector)value);}
      public int Add (ByteVector value) {return data.Add (value);}
      
      public void Clear () {data.Clear ();}
      
      bool  IList.Contains (object value)     {return Contains ((ByteVector)value);}
      public bool Contains (ByteVector value) {return IndexOf (value) != -1;}
      
      int  IList.IndexOf (object value)     {return IndexOf ((ByteVector)value);}
      public int IndexOf (ByteVector v)
      {
         for (int i = 0; i < data.Count; i ++)
            if (v == (ByteVector) data [i])
               return i;
         
         return -1;
      }
      
      void  IList.Insert (int index, object value)     {Insert (index, (ByteVector)value);}
      public void Insert (int index, ByteVector value) {data.Insert (index, value);}
      
      void  IList.Remove (object value)     {Remove ((ByteVector)value);}
      public void Remove (ByteVector value) {data.Remove (value);}
      
      public void RemoveAt (int index) {data.RemoveAt (index);}
      
      
      //////////////////////////////////////////////////////////////////////////
      // ICollection properties
      //////////////////////////////////////////////////////////////////////////
      
      public int Count {get {return data.Count;}}
      
      public bool IsSynchronized {get {return false;}}
      
      public object SyncRoot {get {return this;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // ICollection methods
      //////////////////////////////////////////////////////////////////////////
      
      void ICollection.CopyTo (System.Array array, int index) {data.CopyTo (array, index);}
      
      
      //////////////////////////////////////////////////////////////////////////
      // IEnumerable methods
      //////////////////////////////////////////////////////////////////////////
      
      IEnumerator IEnumerable.GetEnumerator () {return data.GetEnumerator ();}
      
      
      //////////////////////////////////////////////////////////////////////////
      // public constructors
      //////////////////////////////////////////////////////////////////////////
      
      public ByteVectorList () {}
      
      public ByteVectorList (ByteVector v) {Add (v);}
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      
      public bool IsEmpty {get {return Count == 0;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      
      public void SortedInsert (ByteVector v, bool unique)
      {
         int i = 0;
         for (; i < data.Count; i ++)
         {
            if (v == (ByteVector) data [i] && unique)
               return;
            
            if (v >= (ByteVector) data [i])
               break;
         }
         
         Insert (i + 1, v);
      }
      
      public void SortedInsert (ByteVector v)
      {
         SortedInsert (v, false);
      }
      
      public ByteVector ToByteVector (ByteVector separator)
      {
         ByteVector v = new ByteVector ();
         
         for (int i = 0; i < Count; i ++)
         {
            if (i != 0)
               v.Add (separator);
            
            v.Add (this [i]);
         }
         
         return v;
      }
      
      public ByteVector ToByteVector ()
      {
         return ToByteVector (" ");
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public static methods
      //////////////////////////////////////////////////////////////////////////
      
      public static ByteVectorList Split (ByteVector v, ByteVector pattern,
                                     int byte_align, int max)
      {
         ByteVectorList l = new ByteVectorList ();
         int previous_offset = 0;
         for (int offset = v.Find (pattern, 0, byte_align);
              offset != -1 && (max == 0 || max > l.Count + 1);
              offset = v.Find (pattern, offset + pattern.Count, byte_align))
         {
            l.Add (v.Mid (previous_offset, offset - previous_offset));
            previous_offset = offset + pattern.Count;
         }

         if (previous_offset < v.Count)
            l.Add (v.Mid (previous_offset, v.Count - previous_offset));

         return l;
      }

      public static ByteVectorList Split (ByteVector v, ByteVector pattern,
                                     int byte_align)
      {
         return Split (v, pattern, byte_align, 0);
      }
      
      public static ByteVectorList Split (ByteVector v, ByteVector pattern)
      {
         return Split (v, pattern, 1);
      }
   }
}
