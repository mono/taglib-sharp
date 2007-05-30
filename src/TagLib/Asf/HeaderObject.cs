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

using System.Collections.Generic;
using System;

namespace TagLib.Asf
{
   public class HeaderObject : Object
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ByteVector reserved;
      private List<Object> children;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public HeaderObject (Asf.File file, long position) : base (file, position)
      {
         if (!Guid.Equals (Asf.Guid.AsfHeaderObject))
            throw new CorruptFileException ("Object GUID incorrect.");
         
         children = new List<Object> ();
         
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
         
         long size_diff = (long) output.Count + 30 - (long) OriginalSize;
         
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
      
      public bool HasContentDescriptors
      {
         get
         {
            foreach (Asf.Object child in children)
               if (child.Guid == TagLib.Asf.Guid.AsfContentDescriptionObject ||
                   child.Guid == TagLib.Asf.Guid.AsfExtendedContentDescriptionObject)
                  return true;
            
            return false;
         }
      }
      
      public void RemoveContentDescriptors ()
      {
         for (int i = children.Count - 1; i >= 0; i --)
            if (children [i].Guid == TagLib.Asf.Guid.AsfContentDescriptionObject ||
                children [i].Guid == TagLib.Asf.Guid.AsfExtendedContentDescriptionObject)
               children.RemoveAt (i);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public IEnumerable<Object> Children {get {return children;}}
      
      public Properties Properties
      {
         get
         {
            TimeSpan duration = TimeSpan.Zero;
            List<ICodec> codecs = new List<ICodec> ();
            
            foreach (Object obj in Children)
            {
               if (obj is FilePropertiesObject)
                  duration = (obj as FilePropertiesObject).PlayDuration;
               
               if (obj is StreamPropertiesObject)
                  codecs.Add ((obj as StreamPropertiesObject).Codec);
            }
            
            return new Properties (duration, codecs);
         }
      }
   }
}
