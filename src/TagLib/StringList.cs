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
   public class StringList : IList
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
         set {this [index] = (string)value;}
      }
      
      public string this [int index]
      {
         get {return (string) data [index];}
         set {data [index] = value;}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // IList methods
      //////////////////////////////////////////////////////////////////////////
      
      int  IList.Add (object value)     {return Add ((string)value);}
      public int Add (string value) {return data.Add (value);}
      
      public void Clear () {data.Clear ();}
      
      bool  IList.Contains (object value)     {return Contains ((string)value);}
      public bool Contains (string value) {return IndexOf (value) != -1;}
      
      int  IList.IndexOf (object value)     {return IndexOf ((string)value);}
      public int IndexOf (string v)
      {
         for (int i = 0; i < data.Count; i ++)
            if (v == (string) data [i])
               return i;
         
         return -1;
      }
      
      void  IList.Insert (int index, object value) {Insert (index, (string)value);}
      public void Insert (int index, string value) {data.Insert (index, value);}
      
      void  IList.Remove (object value) {Remove ((string)value);}
      public void Remove (string value) {data.Remove (value);}
      
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
      
      public StringList () {}
      
      public StringList (string s)
      {
         Add (s);
      }
      public StringList (StringList s)
      {
         Add (s);
      }
      public StringList (string [] s)
      {
         Add (s);
      }
      
      public StringList (ByteVectorList l, StringType type)
      {
         foreach (ByteVector v in l)
            Add (v.ToString (type));
      }
      
      public StringList (ByteVectorList l) : this (l, StringType.UTF8) {}
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      
      public bool IsEmpty {get {return Count == 0;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      
      public void Add (StringList l)
      {
         if (l != null)
            data.AddRange (l);
      }
      
      public void Add (string [] l)
      {
         if (l != null)
            data.AddRange (l);
      }
      
      public void SortedInsert (string s, bool unique)
      {
         int i = 0;
         for (; i < data.Count; i ++)
         {
            if (s == (string) data [i] && unique)
               return;
            
            if (s.CompareTo ((string) data [i]) <= 0)
               break;
         }
         
         Insert (i, s);
      }
      
      public void SortedInsert (string s)
      {
         SortedInsert (s, false);
      }
      
      public string ToString (string separator)
      {
         string s = "";
         
         for (int i = 0; i < Count; i ++)
         {
            if (i != 0)
               s += separator;
            
            s += this [i];
         }
         
         return s;
      }
      
      public override string ToString ()
      {
         return ToString (", ");
      }
      
      public string [] ToArray ()
      {
         return (string []) data.ToArray (typeof (string));
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public static methods
      //////////////////////////////////////////////////////////////////////////
      
      public static StringList Split (string s, string pattern)
      {
         StringList l = new StringList ();
         
         int previous_position = 0;
         int position = s.IndexOf (pattern, 0);
         
         while (position != -1)
         {
            l.Add (s.Substring (previous_position, position - previous_position));
            previous_position = position + pattern.Length;
            
            position = s.IndexOf (pattern, previous_position);
         }
         
         l.Add (s.Substring (previous_position));
         return l;
      }
   }
}
