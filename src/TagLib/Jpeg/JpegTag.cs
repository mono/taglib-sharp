//
// JpegTag.cs:
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

using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Exif;
using TagLib.Xmp;


namespace TagLib.Jpeg
{
	internal class JpegTag : TagLib.CombinedTag
	{
		internal File File { get; private set; }

#region Constructors

		internal JpegTag (File file)
		{
			File = file;
		}

#endregion


#region Internal Methods

		internal void AddJpegComment (JpegCommentTag tag)
		{
			AddTag (tag);
		}

		internal void AddIFDTag (IFD.IFDTag tag)
		{
			AddTag (tag);

			var exif_tag = FindExifTag (tag);
			if (exif_tag != null)
				AddExifTag (exif_tag);
		}

		internal void AddXmpTag (XmpTag tag)
		{
			AddTag (tag);
		}

		internal void AddExifTag (ExifTag tag)
		{
			AddTag (tag);
		}


		internal void RemoveJpegComment ()
		{
			foreach (JpegCommentTag tag in Tags)
				if (tag != null)
					RemoveTag (tag);
		}

#endregion

#region Private Methods

		private ExifTag FindExifTag (IFD.IFDTag ifd_tag)
		{
			if (ifd_tag == null)
				return null;

			var exif_entry = ifd_tag.GetEntry ((ushort) IFDEntryTag.ExifIFD) as SubIFDEntry;
			if (exif_entry == null)
				return null;

			return exif_entry.IFDTag as ExifTag;
		}

#endregion
	}
}
