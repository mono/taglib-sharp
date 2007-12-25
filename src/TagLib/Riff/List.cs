//
// List.cs:
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Runtime.Serialization;

namespace TagLib.Riff {
	[Serializable]
	[ComVisible(false)]
	public class List : Dictionary <ByteVector,ByteVectorCollection>
	{
		public List ()
		{
		}
		
		public List (ByteVector data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			Parse (data);
		}
		
		public List (TagLib.File file, long position, int length)
		{
			if (file == null)
				throw new ArgumentNullException ("file");
			
			if (length < 0)
				throw new ArgumentOutOfRangeException (
					"length");
			
			if (position < 0 || position > file.Length - length)
				throw new ArgumentOutOfRangeException (
					"position");
			
			file.Seek (position);
			Parse (file.ReadBlock (length));
		}
		
		protected List (SerializationInfo info,
		                StreamingContext context)
			: base (info, context)
		{
		}
		
		private void Parse (ByteVector data)
		{
			int offset = 0;
			while (offset + 8 < data.Count) {
				ByteVector id = data.Mid (offset, 4);
				int length = (int) data.Mid (offset + 4, 4)
					.ToUInt (false);
				
				if (!ContainsKey (id))
					Add (id, new ByteVectorCollection ());
				
				this [id].Add (data.Mid (offset + 8, length));
				
				if (length % 2 == 1)
					length ++;
				
				offset += 8 + length;
			}
		}
		
		public ByteVector Render ()
		{
			ByteVector data = new ByteVector ();
			
			foreach (ByteVector id in Keys)
				foreach (ByteVector value in this [id]) {
					if (value.Count == 0)
						continue;
					
					data.Add (id);
					data.Add (ByteVector.FromUInt (
						(uint) value.Count, false));
					data.Add (value);
					
					if (value.Count % 2 == 1)
						data.Add (0);
				}
			
			return data;
		}
		
		public ByteVector RenderEnclosed (ByteVector id)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			
			if (id.Count != 4)
				throw new ArgumentException (
					"ID must be 4 bytes long.", "id");
			
			ByteVector data = Render ();
			
			if (data.Count <= 8)
				return new ByteVector ();
			
			ByteVector header = new ByteVector ("LIST");
			header.Add (ByteVector.FromUInt (
				(uint) (data.Count + 4), false));
			header.Add (id);
			data.Insert (0, header);
			return data;
		}
		
		public ByteVectorCollection GetValues (ByteVector id)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			
			if (id.Count != 4)
				throw new ArgumentException (
					"ID must be 4 bytes long.", "id");
			
			ByteVectorCollection value;
			
			return TryGetValue (id, out value) ?
				value : new ByteVectorCollection ();
		}
		
		public string [] GetValuesAsStrings (ByteVector id)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			
			if (id.Count != 4)
				throw new ArgumentException (
					"ID must be 4 bytes long.", "id");
			
			ByteVectorCollection values = GetValues (id);
			
			string [] result = new string [values.Count];
			
			for (int i = 0; i < result.Length; i ++) {
				ByteVector data = values [i];
				
				if (data == null) {
					result [i] = string.Empty;
					continue;
				}
				
				int length = data.Count;
				while (length > 0 && data [length - 1] == 0)
					length --;
				
				result [i] = data
					.ToString (StringType.UTF8, 0, length);
			}
			
			return result;
		}
		
		[Obsolete("Use GetValuesAsStrings(ByteVector)")]
		public StringCollection GetValuesAsStringCollection (ByteVector id)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			
			if (id.Count != 4)
				throw new ArgumentException (
					"ID must be 4 bytes long.", "id");
			
			return new StringCollection (GetValuesAsStrings (id));
		}
		
		public uint GetValueAsUInt (ByteVector id)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			
			if (id.Count != 4)
				throw new ArgumentException (
					"ID must be 4 bytes long.", "id");
			
			foreach (string text in GetValuesAsStrings (id)) {
				uint value;
				if (uint.TryParse (text, out value))
					return value;
			}
			
			return 0;
		}
		
		public void SetValue (ByteVector id,
		                      IEnumerable<ByteVector> values)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			
			if (id.Count != 4)
				throw new ArgumentException (
					"ID must be 4 bytes long.", "id");
			
			if (values == null)
				RemoveValue (id);
			else if (ContainsKey (id))
				this [id] = new ByteVectorCollection (values);
			else
				Add (id, new ByteVectorCollection (values));
		}
		
		public void SetValue (ByteVector id, params ByteVector [] values)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			
			if (id.Count != 4)
				throw new ArgumentException (
					"ID must be 4 bytes long.", "id");
			
			if (values == null || values.Length == 0)
				RemoveValue (id);
			else
				SetValue (id, values as IEnumerable<ByteVector>);
		}
		
		public void SetValue (ByteVector id, uint value)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			
			if (id.Count != 4)
				throw new ArgumentException (
					"ID must be 4 bytes long.", "id");
			
			if (value == 0)
				RemoveValue (id);
			else
				SetValue (id, value.ToString (
					CultureInfo.InvariantCulture));
		}
		
		public void SetValue (ByteVector id, IEnumerable<string> values)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			
			if (id.Count != 4)
				throw new ArgumentException (
					"ID must be 4 bytes long.", "id");
			
			if (values == null) {
				RemoveValue (id);
				return;
			}
			
			ByteVectorCollection l = new ByteVectorCollection ();
			foreach (string value in values) {
				if (string.IsNullOrEmpty (value))
					continue;
				
				ByteVector data = ByteVector.FromString (value,
					StringType.UTF8);
				data.Add (0);
				l.Add (data);
			}
			
			if (l.Count == 0)
				RemoveValue (id);
			else
				SetValue (id, l);
		}
		
		public void SetValue (ByteVector id, params string [] values)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			
			if (id.Count != 4)
				throw new ArgumentException (
					"ID must be 4 bytes long.", "id");
			
			if (values == null || values.Length == 0)
				RemoveValue (id);
			else
				SetValue (id, values as IEnumerable<string>);
		}
		
		public void RemoveValue (ByteVector id)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			
			if (id.Count != 4)
				throw new ArgumentException (
					"ID must be 4 bytes long.", "id");
			
			if (ContainsKey (id))
				Remove (id);
		}
	}
}
