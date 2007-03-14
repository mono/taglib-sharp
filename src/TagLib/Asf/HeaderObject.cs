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

namespace TagLib.Asf
{
   public class HeaderObject : Object
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ByteVector reserved;
      private ArrayList children;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public HeaderObject (Asf.File file, long position) : base (file, position)
      {
         if (!Guid.Equals (Asf.Guid.AsfHeaderObject))
            throw new CorruptFileException ("Object GUID incorrect.");
         
         children = new ArrayList ();
         
         uint child_count = file.ReadDWord ();
         
         reserved = file.ReadBlock (2);
         children.AddRange (file.ReadObjects (child_count, file.Tell));
      }
      
      public override ByteVector Render ()
      {
         ByteVector output = new ByteVector ();
         uint child_count = 0;
         foreach (Object child in children)
            if (child.Guid != Asf.Guid.AsfPaddingObject)
            {
               output.Add (child.Render ());
               child_count ++;
            }
         
         int size_diff = (int) (output.Count + 30 - OriginalSize);
         
         if (size_diff != 0)
         {
            PaddingObject obj = new PaddingObject ((uint) (size_diff > 0 ? 4096 : - size_diff));
            output.Add (obj.Render ());
            child_count ++;
         }
         
         output.Insert (0, reserved);
         output.Insert (0, RenderDWord (child_count));
         return Render (output);
      }
      
      public void AddObject (Object obj)
      {
         children.Add (obj);
      }
      
      public void AddUniqueObject (Object obj)
      {
         for (int i = 0; i < children.Count; i ++)
            if (((Object) children [i]).Guid == obj.Guid)
            {
               children [i] = obj;
               return;
            }
         
         children.Add (obj);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public byte [] Reserved {get {return reserved.Data;}}
      
      public Object [] Children {get {return (Object []) children.ToArray (typeof (Object));}}
   }
}
