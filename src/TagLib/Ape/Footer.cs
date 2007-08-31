/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : apefooter.cpp from TagLib
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

namespace TagLib.Ape {
	#region Enums
	
	[Flags]
	/// <summary>
	///    Indicates the flags applied to a <see cref="Footer" /> object.
	/// </summary>
	public enum FooterFlags : uint {
		/// <summary>
		///    The tag lacks a footer object.
		/// </summary>
		FooterAbsent  = 0x40000000,
		
		/// <summary>
		///    The footer is actually a header.
		/// </summary>
		IsHeader      = 0x20000000,
		
		/// <summary>
		///    The tag contains a header.
		/// </summary>
		HeaderPresent = 0x80000000
	}
	
	#endregion
	
	
	
	/// <summary>
	///    This structure provides support for reading and writing APEv2
	///    footers.
	/// </summary>
	public struct Footer : IEquatable<Footer>
	{
		#region Private Properties
		
		/// <summary>
		///    Contains the APE tag version.
		/// </summary>
		private uint _version;
		
		/// <summary>
		///    Contains the footer flags.
		/// </summary>
		private uint _flags;
		
		/// <summary>
		///    Contains the number of items in the tag.
		/// </summary>
		private uint _item_count;
		
		/// <summary>
		///    Contains the tag size including the footer but excluding
		///    the header.
		/// </summary>
		private uint _tag_size;
		
		#endregion
		
		
		
		#region Public Static Fields
		
		/// <summary>
		///    Specifies the size of an APEv2 footer.
		/// </summary>
		public const uint Size = 32;
		
		/// <summary>
		///    Specifies the identifier used find an APEv2 footer in a
		///    file.
		/// </summary>
		/// <value>
		///    "<c>APETAGEX</c>"
		/// </value>
		public static readonly ReadOnlyByteVector FileIdentifier = "APETAGEX";
		
		#endregion
		
		
		
		#region Constructors
		
		public Footer (ByteVector data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			if (data.Count < Size)
				throw new CorruptFileException (
					"Provided data is smaller than object size.");
			
			if (!data.StartsWith (FileIdentifier))
				throw new CorruptFileException (
					"Provided data does not start with File Identifier");
			
			_version = data.Mid (8, 4).ToUInt (false);
			_tag_size = data.Mid (12, 4).ToUInt (false);
			_item_count = data.Mid (16, 4).ToUInt (false);
			_flags = data.Mid (20, 4).ToUInt (false);
		}
		
		#endregion
		
		
		
		#region Public Properties
		
		public uint Version {
			get {return _version == 0 ? 2000 : _version;}
		}
		
		public FooterFlags Flags {
			get {return (FooterFlags) _flags;}
			set {_flags = (uint) value;}
		}
		
		public uint ItemCount {
			get {return _item_count;}
			set {_item_count = value;}
		}
		
		public uint TagSize {
			get {return _tag_size;}
			set {_tag_size = value;}
		}
		
		public uint CompleteTagSize {
			get {
				return TagSize + ((Flags &
					FooterFlags.HeaderPresent) != 0 ?
					Size : 0);
			}
		}
		
		#endregion
		
		
		
		#region Public Methods
		
		public ByteVector RenderFooter ()
		{
			return Render (false);
		}
		
		public ByteVector RenderHeader ()
		{
			return (Flags & FooterFlags.HeaderPresent) != 0 ?
				Render (true) : new ByteVector ();
		}
		
		#endregion
		
		
		
		#region Private Methods
		private ByteVector Render (bool isHeader)
		{
			ByteVector v = new ByteVector ();
			
			// add the file identifier -- "APETAGEX"
			v.Add (FileIdentifier);
			
			// add the version number -- we always render a 2.000
			// tag regardless of what the tag originally was.
			v.Add (ByteVector.FromUInt (2000, false));
			
			// add the tag size
			v.Add (ByteVector.FromUInt (_tag_size, false));
			
			// add the item count
			v.Add (ByteVector.FromUInt (_item_count, false));
			
			// render and add the flags
			uint flags = 0;
			
			if ((Flags & FooterFlags.HeaderPresent) != 0)
				flags |= (uint) FooterFlags.HeaderPresent;
			
			// footer is always present
			if (isHeader) flags |= (uint) FooterFlags.IsHeader;
			
			v.Add (ByteVector.FromUInt (flags, false));
			
			// add the reserved 64bit
			v.Add (ByteVector.FromULong (0));
			
			return v;
		}
		
		#endregion
      
      
      
      #region IEquatable
      public override int GetHashCode ()
      {
         unchecked
         {            return (int) (_flags ^ _tag_size ^ _item_count ^ _version);
         }
      }
      
      public override bool Equals (object obj)
      {
         if (!(obj is Footer))
            return false;
         
         return Equals ((Footer) obj);
      }
      
      public bool Equals (Footer other)
      {
         return _flags == other._flags && _tag_size == other._tag_size &&
         _item_count == other._item_count && _version == other._version;
      }
      
      public static bool operator == (Footer first, Footer second)
      {
         return first.Equals (second);
      }
      
      public static bool operator != (Footer first, Footer second)
      {
         return !first.Equals (second);
      }
      #endregion
   }
}
