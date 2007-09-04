//
// ListTag.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
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

namespace TagLib.Riff
{
   public abstract class ListTag : Tag
   {
      List fields;
      
      protected ListTag ()
      {
         fields = new List ();
      }
      
      protected ListTag (List fields)
      {
         if (fields == null)
            throw new System.ArgumentNullException ("fields");
         
         this.fields = fields;
      }
      
      protected ListTag (ByteVector data)
      {
         fields = new List (data);
      }
      
      protected ListTag (TagLib.File file, long position, int length)
      {
         if (file == null)
            throw new System.ArgumentNullException ("file");
         
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
      
      public ByteVectorCollection GetValues (ByteVector id)
      {
         return fields.GetValues (id);
      }
      
		public string [] GetValuesAsStrings (ByteVector id)
		{
			return fields.GetValuesAsStrings (id);
		}
		
		[Obsolete("Use GetValuesAsStrings(ByteVector)")]
		public StringCollection GetValuesAsStringCollection (ByteVector id)
		{
			return new StringCollection (
				fields.GetValuesAsStrings (id));
		}
      
      public uint GetValueAsUInt (ByteVector id)
      {
         return fields.GetValueAsUInt (id);
      }
      
      public void SetValue (ByteVector id, params ByteVector [] value)
      {
         fields.SetValue (id, value);
      }
      
      public void SetValue (ByteVector id, ByteVectorCollection value)
      {
         fields.SetValue (id, value);
      }
      
      public void SetValue (ByteVector id, uint value)
      {
         fields.SetValue (id, value);
      }
      
		[Obsolete("Use SetValue(ByteVector,string[])")]
      public void SetValue (ByteVector id, StringCollection value)
      {
         fields.SetValue (id, value);
      }
      
      public void SetValue (ByteVector id, params string [] value)
      {
         fields.SetValue (id, value);
      }
      
      public void RemoveValue (ByteVector id)
      {
         fields.RemoveValue (id);
      }
      
      public override void Clear ()
      {
         fields.Clear ();
      }
      
      public override bool IsEmpty {
         get {
            return fields.Count == 0;
         }
      }
   }
}