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

namespace TagLib.Image
{

	public class CombinedImageTag : ImageTag
	{

#region Private Fields

		private List<ImageTag> image_tags = new List<ImageTag> ();

		private File file;

#endregion

#region Constructors

		public CombinedImageTag (File file)
		{
			this.file = file;
		}

#endregion

#region Protected Methods

		protected void AddTag (ImageTag tag)
		{
			image_tags.Add (tag);
		}

		protected void RemoveTag (ImageTag tag)
		{
			image_tags.Remove (tag);
		}

#endregion

#region Public Properties

		public File File {
			get { return file; }
		}

		public override TagTypes TagTypes {
			get {
				TagTypes types = TagTypes.None;
				foreach (ImageTag tag in image_tags)
					if (tag != null)
						types |= tag.TagTypes;

				return types;
			}
		}

		public ImageTag[] ImageTags {
			get { return image_tags.ToArray (); }
		}

		/// <summary>
		///    Clears all of the child tags.
		/// </summary>
		public override void Clear ()
		{
			foreach (ImageTag tag in image_tags)
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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

					string value = tag.Comment;

					if (value != null)
						return value;
				}

				return null;
			}

			set {
				foreach (ImageTag tag in image_tags)
					if (tag != null)
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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

					string[] value = tag.Keywords;

					if (value != null && value.Length > 0)
						return value;
				}

				return new string[] {};
			}

			set {
				foreach (ImageTag tag in image_tags)
					if (tag != null)
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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

					uint value = tag.Rating;

					if (value != 0)
						return value;
				}

				return 0;
			}

			set {
				foreach (ImageTag tag in image_tags)
					if (tag != null)
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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

					DateTime value = tag.DateTime;

					if (value != DateTime.MinValue)
						return value;
				}

				return DateTime.MinValue;
			}

			set {
				foreach (ImageTag tag in image_tags)
					if (tag != null)
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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

					uint value = tag.Orientation;

					if (value != 0)
						return value;
				}

				return 0;
			}

			set {
				foreach (ImageTag tag in image_tags)
					if (tag != null)
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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

					string value = tag.Software;

					if (value != null)
						return value;
				}

				return null;
			}

			set {
				foreach (ImageTag tag in image_tags)
					if (tag != null)
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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

					string value = tag.Make;

					if (value != null)
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
				foreach (ImageTag tag in image_tags) {
					if (tag == null)
						continue;

					string value = tag.Model;

					if (value != null)
						return value;
				}

				return null;
			}
		}
#endregion

	}
}
