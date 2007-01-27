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

namespace TagLib.Asf
{
   public class ExtendedContentDescriptionObject : Object
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ArrayList descriptors;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public ExtendedContentDescriptionObject (Asf.File file, long position) : base (file, position)
      {
         if (!Guid.Equals (Asf.Guid.AsfExtendedContentDescriptionObject))
            throw new CorruptFileException ("Object GUID incorrect.");
         
         if (OriginalSize < 26)
            throw new CorruptFileException ("Object size too small.");
         
         descriptors = new ArrayList ();
         
         short count = file.ReadWord ();
         
         for (short i = 0; i < count; i ++)
         {
            ContentDescriptor desc = new ContentDescriptor (file);
            AddDescriptor (desc);
         }
      }
      
      public ExtendedContentDescriptionObject () : base (Asf.Guid.AsfExtendedContentDescriptionObject)
      {
         descriptors = new ArrayList ();
      }
      
      public override ByteVector Render ()
      {
         ByteVector output = new ByteVector ();
         short count = 0;
         
         foreach (ContentDescriptor desc in descriptors)
         {
            count ++;
            output.Add (desc.Render ());
         }
         
         return Render (RenderWord (count) + output);
      }
      
      public void RemoveDescriptors (string name)
      {
         for (int i = descriptors.Count - 1; i >= 0; i --)
            if (name == ((ContentDescriptor) descriptors [i]).Name)
               descriptors.RemoveAt (i);
      }
      
      public ContentDescriptor [] GetDescriptors (string name)
      {
         ArrayList l = new ArrayList ();
         
         foreach (ContentDescriptor desc in descriptors)
            if (desc.Name == name)
               l.Add (desc);
         
         return (ContentDescriptor []) l.ToArray (typeof (ContentDescriptor));
      }

      public void AddDescriptor (ContentDescriptor descriptor)
      {
         descriptors.Add (descriptor);
      }
      
      public void SetDescriptors (string name, params ContentDescriptor [] descriptors)
      {
         int i;
         for (i = 0; i < this.descriptors.Count; i ++)
            if (name == ((ContentDescriptor) this.descriptors [i]).Name)
               break;
         
         RemoveDescriptors (name);
         
         this.descriptors.InsertRange (i, descriptors);
      }
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
   }
}
