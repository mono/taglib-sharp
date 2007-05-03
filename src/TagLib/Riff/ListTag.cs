/***************************************************************************
    copyright            : (C) 2007 by Brian Nickel
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

namespace TagLib.Riff
{
   public abstract class ListTag : Tag
   {
      List fields;
      
      public ListTag ()
      {
         fields = new List ();
      }
      
      public ListTag (List fields)
      {
         this.fields = fields;
      }
      
      public ListTag (ByteVector data)
      {
         fields = new List (data);
      }
      
      public ListTag (TagLib.File file, long position, int length)
      {
         file.Seek (position);
         fields = new List (file.ReadBlock (length));
      }
      
      public abstract ByteVector RenderEnclosed ();
      
      protected ByteVector RenderEnclosed (ByteVector id)
      {
         return fields.RenderEnclosed (id);
      }
      
      public ByteVector Render ()
      {
         return fields.Render ();
      }
      
      public ByteVectorList GetValues (ByteVector id)
      {
         return fields.GetValues (id);
      }
      
      public StringList GetValuesAsStringList (ByteVector id)
      {
         return fields.GetValuesAsStringList (id);
      }
      
      public uint GetValueAsUInt (ByteVector id)
      {
         return fields.GetValueAsUInt (id);
      }
      
      public void SetValue (ByteVector id, ByteVector value)
      {
         fields.SetValue (id, value);
      }
      
      public void SetValue (ByteVector id, string value)
      {
         fields.SetValue (id, value);
      }
      
      public void SetValue (ByteVector id, uint value)
      {
         fields.SetValue (id, value);
      }
      
      public void SetValue (ByteVector id, StringList value)
      {
         fields.SetValue (id, value);
      }
      
      public void SetValue (ByteVector id, string [] value)
      {
         fields.SetValue (id, value);
      }
      
      public void RemoveValue (ByteVector id)
      {
         fields.RemoveValue (id);
      }
   }
}