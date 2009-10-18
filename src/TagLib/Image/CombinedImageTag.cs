//
// CombinedImageTag.cs: The class provides an abstraction to combine
// ImageTags.
//
// Author:
//   Mike Gemuende (mike@gemuende.de)
//
// Copyright (C) 2009 Mike Gemuende
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

using TagLib.Xmp;
using TagLib.Exif;

namespace TagLib.Image
{

	public class CombinedImageTag : ImageTag
	{

#region Private Fields

		/// <summary>
		///    Direct access to the Exif tag (if any)
		/// </summary>
		public ImageTag Exif { get; private set; }

		/// <summary>
		///    Direct access to the Xmp tag (if any)
		/// </summary>
		public ImageTag Xmp { get; private set; }

		/// <summary>
		///    Other image tags available in this tag.
		/// </summary>
		public List<ImageTag> OtherTags { get; private set; }

		private TagTypes allowed_types;

		/// <summary>
		///    Returns all image tags in this tag, with XMP
		///    and Exif first.
		/// </summary>
		public List<ImageTag> AllTags {
			get {
				if (all_tags == null) {
					all_tags = new List<ImageTag> ();
					if (Xmp != null)
						all_tags.Add (Xmp);
					if (Exif != null)
						all_tags.Add (Exif);
					all_tags.AddRange (OtherTags);
				}

				return all_tags;
			}
		}

		private List<ImageTag> all_tags = null;

#endregion

#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="CombinedImageTag" /> with a restriction on the
		///    allowed tag types contained in this combined tag.
		/// </summary>
		/// <param name="allowed_types">
		///    A <see cref="TagTypes" /> value, which restricts the
		///    types of metadata that can be contained in this
		///    combined tag.
		/// </param>
		public CombinedImageTag (TagTypes allowed_types)
		{
			this.allowed_types = allowed_types;
			OtherTags = new List<ImageTag> ();
		}

#endregion

#region Protected Methods

		internal void AddTag (ImageTag tag)
		{
			if ((tag.TagTypes & allowed_types) != tag.TagTypes)
				throw new Exception (String.Format ("Attempted to add {0} to an image, but the only allowed types are {1}", tag.TagTypes, allowed_types));

			if (tag is ExifTag)
				Exif = tag;
			else if (tag is XmpTag)
				Xmp = tag;
			else
				OtherTags.Add (tag);

			all_tags = null;
		}

		internal void RemoveTag (ImageTag tag)
		{
			if (tag is ExifTag)
				Exif = null;
			else if (tag is XmpTag)
				Xmp = null;
			else
				OtherTags.Remove (tag);

			all_tags = null;
		}

#endregion

#region Public Properties

		public override TagTypes TagTypes {
			get {
				TagTypes types = TagTypes.None;

				if (Exif != null)
					types |= Exif.TagTypes;
				if (Xmp != null)
					types |= Xmp.TagTypes;

				foreach (ImageTag tag in OtherTags)
					types |= tag.TagTypes;

				return types;
			}
		}

		/// <summary>
		///    Clears all of the child tags.
		/// </summary>
		public override void Clear ()
		{
			foreach (ImageTag tag in AllTags)
				tag.Clear ();
		}

		/// <summary>
		///    Gets or sets the comment for the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the comment of the
		///    current instace.
		/// </value>
		public override string Comment {
			get {
				foreach (ImageTag tag in AllTags) {
					string value = tag.Comment;
					if (!string.IsNullOrEmpty (value))
						return value;
				}

				return null;
			}
			set {
				foreach (ImageTag tag in AllTags)
					tag.Comment = value;
			}
		}

		/// <summary>
		///    Gets or sets the keywords for the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the keywords of the
		///    current instace.
		/// </value>
		public override string[] Keywords {
			get {
				foreach (ImageTag tag in AllTags) {
					string[] value = tag.Keywords;
					if (value != null && value.Length > 0)
						return value;
				}

				return new string[] {};
			}
			set {
				foreach (ImageTag tag in AllTags)
					tag.Keywords = value;
			}
		}

		/// <summary>
		///    Gets or sets the rating for the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the rating of the
		///    current instace.
		/// </value>
		public override uint Rating {
			get {
				foreach (ImageTag tag in AllTags) {
					uint value = tag.Rating;

					if (value != 0)
						return value;
				}

				return 0;
			}
			set {
				foreach (ImageTag tag in AllTags)
					tag.Rating = value;
			}
		}

		/// <summary>
		///    Gets or sets the time when the image, the current instance
		///    belongs to, was taken.
		/// </summary>
		/// <value>
		///    A <see cref="DateTime" /> with the time the image was taken.
		/// </value>
		public override DateTime DateTime {
			get {
				foreach (ImageTag tag in AllTags) {
					DateTime value = tag.DateTime;

					if (value != DateTime.MinValue)
						return value;
				}

				return DateTime.MinValue;
			}
			set {
				foreach (ImageTag tag in AllTags)
					tag.DateTime = value;
			}
		}

		/// <summary>
		///    Gets or sets the orientation of the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the orienatation of the
		///    image
		/// </value>
		public override uint Orientation {
			get {
				foreach (ImageTag tag in AllTags) {
					uint value = tag.Orientation;

					if (value != 0)
						return value;
				}

				return 0;
			}
			set {
				foreach (ImageTag tag in AllTags)
					tag.Orientation = value;
			}
		}

		/// <summary>
		///    Gets or sets the software the image, the current instance
		///    belongs to, was created with.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the name of the
		///    software the current instace was created with.
		/// </value>
		public override string Software {
			get {
				foreach (ImageTag tag in AllTags) {
					string value = tag.Software;

					if (!string.IsNullOrEmpty(value))
						return value;
				}

				return null;
			}
			set {
				foreach (ImageTag tag in AllTags)
					tag.Software = value;
			}
		}

		/// <summary>
		///    Gets the exposure time the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the exposure time in seconds.
		/// </value>
		public override double ExposureTime {
			get {
				foreach (ImageTag tag in AllTags) {
					double value = tag.ExposureTime;

					if (value > 0.0d)
						return value;
				}

				return 0.0d;
			}
		}

		//// <summary>
		///    Gets the FNumber the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the FNumber.
		/// </value>
		public override double FNumber {
			get {
				foreach (ImageTag tag in AllTags) {
					double value = tag.FNumber;

					if (value > 0.0d)
						return value;
				}

				return 0.0d;
			}
		}

		/// <summary>
		///    Gets the ISO speed the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> with the ISO speed as defined in ISO 12232.
		/// </value>
		public override uint ISOSpeedRatings {
			get {
				foreach (ImageTag tag in AllTags) {
					uint value = tag.ISOSpeedRatings;

					if (value > 0)
						return value;
				}

				return 0;
			}
		}

		/// <summary>
		///    Gets the focal length the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the focal length in millimeters.
		/// </value>
		public override double FocalLength {
			get {
				foreach (ImageTag tag in AllTags) {
					double value = tag.FocalLength;

					if (value > 0.0d)
						return value;
				}

				return 0.0d;
			}
		}

		/// <summary>
		///    Gets the manufacture of the recording equipment the image, the
		///    current instance belongs to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> with the manufacture name.
		/// </value>
		public override string Make {
			get {
				foreach (ImageTag tag in AllTags) {
					string value = tag.Make;

					if (!string.IsNullOrEmpty(value))
						return value;
				}

				return null;
			}
		}

		/// <summary>
		///    Gets the model name of the recording equipment the image, the
		///    current instance belongs to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> with the model name.
		/// </value>
		public override string Model {
			get {
				foreach (ImageTag tag in AllTags) {
					string value = tag.Model;

					if (value != null)
					if (!string.IsNullOrEmpty(value))
						return value;
				}

				return null;
			}
		}
#endregion

	}
}
