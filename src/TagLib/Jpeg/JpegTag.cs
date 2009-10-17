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

using System;

using TagLib.Image;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Exif;
using TagLib.Xmp;


namespace TagLib.Jpeg
{
	internal class JpegTag : CombinedImageTag
	{
		internal File File { get; private set; }

#region Constructors

		internal JpegTag (File file)
			: base (file) {}

#endregion

#region Internal Methods

		internal void AddJpegComment (JpegCommentTag tag)
		{
			AddTag (tag);
		}

		internal void AddIFDTag (IFDTag tag)
		{
			AddTag (tag);
			AddSubIFDTags (tag);
		}

		internal void AddXmpTag (XmpTag tag)
		{
			AddTag (tag);
		}

		internal void RemoveJpegComment ()
		{
			foreach (ImageTag tag in ImageTags)
				if (tag is JpegCommentTag)
					RemoveTag (tag);
		}

#endregion

#region Private Methods

		private void AddSubIFDTags (IFDTag tag)
		{
			foreach (IFDTag sub_ifd in tag.SubIFDs) {
				AddTag (sub_ifd);
				AddSubIFDTags (sub_ifd);
			}
		}

#endregion
	}
}
