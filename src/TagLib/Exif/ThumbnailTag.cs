//
// ThumbnailTag.cs:
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

using System.IO;

using TagLib.IFD;
using TagLib.IFD.Entries;

namespace TagLib.Exif
{

	public class ThumbnailTag : IFDTag
	{

#region Private Fields

		private ByteVector thumbnail_data;

#endregion


#region Constructors

		public ThumbnailTag (File file, long base_offset, uint ifd_offset, bool is_bigendian)
			: base (file, base_offset, ifd_offset, is_bigendian)
		{
			ReadThumbnail (base_offset);
		}

#endregion


#region Public Methods

		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    Always <see cref="TagTypes.ThumbnailTag" />.
		/// </value>
		public override TagTypes TagTypes {
			get { return TagTypes.Thumbnail; }
		}

#endregion


#region Public Properties

		public ByteVector Thumbnail {
			get { return thumbnail_data; }
		}

#endregion

		protected override void RenderEntryData (IFDEntry entry, ByteVector entry_data, ByteVector offset_data, uint data_offset)
		{
			if (entry.Tag != (uint) IFDEntryTag.JPEGInterchangeFormat) {
				base.RenderEntryData (entry, entry_data, offset_data, data_offset);
				return;
			}

			RenderEntry (entry_data, (ushort)entry.Tag, (ushort)IFDEntryType.Long, 1, (uint) (data_offset + offset_data.Count));
			offset_data.Add (thumbnail_data);
		}

#region Private Methods

		private void ReadThumbnail (long base_offset)
		{
			LongIFDEntry format_entry =
				GetEntry (IFDEntryTag.JPEGInterchangeFormat) as LongIFDEntry;

			LongIFDEntry length_entry =
				GetEntry (IFDEntryTag.JPEGInterchangeFormatLength) as LongIFDEntry;

			if (format_entry == null || length_entry == null) {
				RemoveTag (IFDEntryTag.JPEGInterchangeFormat);
				RemoveTag (IFDEntryTag.JPEGInterchangeFormatLength);
				return;
			}

			File.Seek (base_offset + format_entry.Value, SeekOrigin.Begin);
			thumbnail_data = File.ReadBlock ((int) length_entry.Value);
		}

#endregion

	}
}
