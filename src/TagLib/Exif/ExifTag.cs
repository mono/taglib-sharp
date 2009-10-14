//
// ExifTag.cs:
//
// Author:
//   Ruben Vermeersch (ruben@savanne.be)
//   Mike Gemuende (mike@gemuende.de)
//
// Copyright (C) 2009 Ruben Vermeersch
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

using TagLib.IFD;
using TagLib.IFD.Entries;

namespace TagLib.Exif
{

	public class ExifTag : IFDTag
	{
		public static readonly ByteVector COMMENT_ASCCI_CODE = new byte[] {0x41, 0x53, 0x43, 0x49, 0x49, 0x00, 0x00, 0x00};
		public static readonly ByteVector COMMENT_JIS_CODE = new byte[] {0x4A, 0x49, 0x53, 0x00, 0x00, 0x00, 0x00, 0x00};
		public static readonly ByteVector COMMENT_UNICODE_CODE = new byte[] {0x55, 0x4E, 0x49, 0x43, 0x4F, 0x44, 0x45, 0x00};
		public static readonly ByteVector COMMENT_UNDEFINED_CODE = new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

#region Constructors

		public ExifTag (File file, long base_offset, uint ifd_offset, bool is_bigendian, out uint next_offset)
			: base (file, base_offset, ifd_offset, is_bigendian, out next_offset) {}


		public ExifTag (File file, uint ifd_offset, bool is_bigendian, out uint next_offset)
			: base (file, ifd_offset, is_bigendian, out next_offset) {}

		public ExifTag (File file) : base (file) {}

#endregion


#region Public Properties

		public string UserComment {
			get {
				UndefinedIFDEntry comment_entry =
					GetEntry (IFDEntryTag.UserComment) as UndefinedIFDEntry;

				if (comment_entry == null)
					return null;

				ByteVector data = comment_entry.Data;

				if (data.StartsWith (COMMENT_ASCCI_CODE))
					return data.ToString (StringType.Latin1, COMMENT_ASCCI_CODE.Count);

				if (data.StartsWith (COMMENT_UNICODE_CODE))
					return data.ToString (StringType.UTF8, COMMENT_UNICODE_CODE.Count);

				throw new NotImplementedException ("UserComment with other encoding than Latin1 or Unicode");
			}
			set {

				ByteVector data = new ByteVector ();
				data.Add (COMMENT_UNICODE_CODE);
				data.Add (ByteVector.FromString (value, StringType.UTF8));

				UndefinedIFDEntry comment_entry =
					new UndefinedIFDEntry ((uint)IFDEntryTag.UserComment, data);

				SetEntry (comment_entry);
			}
		}

		public override string Comment {
			get { return UserComment; }
			set { UserComment = value; }
		}


#endregion

#region Public Methods

		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    Always <see cref="TagTypes.Exif" />.
		/// </value>
		public override TagTypes TagTypes {
			get { return TagTypes.Exif; }
		}

#endregion

	}
}
