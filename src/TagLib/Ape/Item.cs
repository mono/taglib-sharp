/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : apeitem.cpp from TagLib
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

namespace TagLib.Ape {
	/// <summary>
	///    Indicates the type of data stored in a <see cref="Item" />
	///    object.
	/// </summary>
	public enum ItemType {
		/// <summary>
		///    The item contains Unicode text.
		/// </summary>
		Text = 0,
		
		/// <summary>
		///    The item contains binary data.
		/// </summary>
		Binary = 1,
		
		/// <summary>
		///    The item contains a locator (file path/URL) for external
		///    information.
		/// </summary>
		Locator = 2
	}
	
	public class Item
	{
		#region Private Fields
		
		/// <summary>
		///    Contains the type of data stored in the item.
		/// </summary>
		private ItemType type = ItemType.Text;
		
		/// <summary>
		///    Contains the item key.
		/// </summary>
		private string key = null;
		
		/// <summary>
		///    Contains the item value.
		/// </summary>
		private ReadOnlyByteVector data = null;
		
		/// <summary>
		///    Contains the item text.
		/// </summary>
		private string [] text = null;
		
		/// <summary>
		///    Indicates whether or not the item is read only.
		/// </summary>
		private bool read_only = false;
		
		/// <summary>
		///    Contains the size of the item on disk.
		/// </summary>
		private int size_on_disk;
		
		#endregion
		
		
		
		#region Constructors
		
		public Item (ByteVector data, int offset)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			Parse (data, offset);
		}
		
		public Item (string key, string value)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			if (value == null)
				throw new ArgumentNullException ("value");
			
			this.key = key;
			this.text = new string [] {value};
		}
		
		public Item (string key, params string [] value)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			if (value == null)
				throw new ArgumentNullException ("value");
			
			this.key = key;
			this.text = (string[]) value.Clone ();
		}
		
		[Obsolete("Use Item(string,string[])")]
		public Item (string key, StringCollection value)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			if (value == null)
				throw new ArgumentNullException ("value");
			
			this.key = key;
			this.text = value.ToArray ();
		}
		
		public Item (string key, ByteVector value)
		{
			this.key = key;
			this.type = ItemType.Binary;
			
			data = value as ReadOnlyByteVector;
			if (data == null)
				data = new ReadOnlyByteVector (value);
		}
		
		#endregion
		
		
		
		#region Public Properties
		
		public string Key {
			get {return key;}
		}
		
		public ByteVector Value {
			get {return (type == ItemType.Binary) ? data : null;}
		}
		
		public int Size {
			get {return size_on_disk;}
		}
		
		public ItemType Type {
			get {return type;}
			set {type = value;}
		}
		
		public bool ReadOnly {
			get {return read_only;}
			set {read_only = value;}
		}
		
		public bool IsEmpty {
			get {
				if (type != ItemType.Binary)
					return text == null || text.Length == 0;
				else
					return data == null || data.IsEmpty;
			}
		}
		
		#endregion
		
		
		
		#region Public Methods
		public override string ToString ()
		{
			if (type == ItemType.Binary)
				return "[BINARY DATA]";
			else if (text == null)
				return null;
			else
				return string.Join (", ", text);
		}
		
		public string [] ToStringArray ()
		{
			if (type == ItemType.Binary || text == null)
				return new string [0];
			
			return text;
		}
		
		public ByteVector Render ()
		{
			uint flags = (uint) ((ReadOnly) ? 1 : 0) |
				((uint) Type << 1);
			
			if (IsEmpty)
				return new ByteVector ();
			
			ByteVector result = null;
			
			if (type == ItemType.Binary) {
				result = data;
			} else if (text != null) {
				result = new ByteVector ();
				
				for (int i = 0; i < text.Length; i ++) {
					if (i != 0)
						result.Add ((byte) 0);
					
					result.Add (ByteVector.FromString (
						text [i], StringType.UTF8));
				}
			}
			
			// If no data is stored, don't write the item.
			if (result == null || result.Count == 0)
				return new ByteVector ();
			
			ByteVector output = new ByteVector ();
			output.Add (ByteVector.FromUInt ((uint) result.Count,
				false));
			output.Add (ByteVector.FromUInt (flags, false));
			output.Add (ByteVector.FromString (key, StringType.UTF8));
			output.Add ((byte) 0);
			output.Add (result);
			
			size_on_disk = output.Count;
			
			return output;
		}
		
		#endregion
		
		#region Protected Methods
		protected void Parse (ByteVector data, int offset)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			// 11 bytes is the minimum size for an APE item
			if(data.Count < offset + 11)
				throw new CorruptFileException ("Not enough data for APE Item");
			
			uint value_length = data.Mid (offset, 4).ToUInt (false);
			uint flags = data.Mid (offset + 4, 4).ToUInt (false);
			
			ReadOnly = (flags & 1) == 1;
			Type = (ItemType) ((flags >> 1) & 3);
			
			int pos = data.Find (ByteVector.TextDelimiter (
				StringType.UTF8), offset + 8);
			
			key = data.Mid (offset + 8, pos - offset - 8)
				.ToString (StringType.UTF8);
			
			if (value_length > data.Count - pos - 1)
				throw new CorruptFileException (
					"Invalid data length.");
			
			size_on_disk = pos + 1 + (int) value_length - offset;
			
			if (Type == ItemType.Binary)
				this.data = new ReadOnlyByteVector (
					data.Mid (pos + 1, (int) value_length));
			else
				this.text = data.Mid (pos + 1,
					(int) value_length).ToStrings (
						StringType.UTF8, 0);
		}
		
		#endregion
	}
}
