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
   public class IntList : IList
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
         set {this [index] = (int)value;}
      }
      
      public int this [int index]
      {
         get {return (int) data [index];}
         set {data [index] = value;}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // IList methods
      //////////////////////////////////////////////////////////////////////////
      
      int  IList.Add (object value)     {return Add ((int)value);}
      public int Add (int value) {return data.Add (value);}
      
      public void Clear () {data.Clear ();}
      
      bool  IList.Contains (object value)     {return Contains ((int)value);}
      public bool Contains (int value) {return IndexOf (value) != -1;}
      
      int  IList.IndexOf (object value)     {return IndexOf ((int)value);}
      public int IndexOf (int v)
      {
         for (int i = 0; i < data.Count; i ++)
            if (v == (int) data [i])
               return i;
         
         return -1;
      }
      
      void  IList.Insert (int index, object value)     {Insert (index, (int)value);}
      public void Insert (int index, int value) {data.Insert (index, value);}
      
      void  IList.Remove (object value)     {Remove ((int)value);}
      public void Remove (int value) {data.Remove (value);}
      
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
      
      public IntList () {}
      
      public IntList (int s)
      {
         Add (s);
      }
      public IntList (IntList s)
      {
         Add (s);
      }
      public IntList (int [] s)
      {
         Add (s);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      
      public bool IsEmpty {get {return Count == 0;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      
      public void Add (IntList l)
      {
         if (l != null)
            data.AddRange (l);
      }
      
      public void Add (int [] l)
      {
         if (l != null)
            data.AddRange (l);
      }
      
      public void SortedInsert (int s, bool unique)
      {
         int i = 0;
         for (; i < data.Count; i ++)
         {
            if (s == (int) data [i] && unique)
               return;
            
            if (s <= (int) data [i])
               break;
         }
         
         Insert (i, s);
      }
      
      public void SortedInsert (int s)
      {
         SortedInsert (s, false);
      }
      
      public int [] ToArray ()
      {
         return (int []) data.ToArray (typeof (int));
      }
   }
}
