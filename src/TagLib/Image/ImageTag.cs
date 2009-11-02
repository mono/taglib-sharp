//
// ImageTag.cs: This abstract class extends the Tag class by basic Image
// properties.
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

namespace TagLib.Image
{

	public abstract class ImageTag : Tag
	{

#region Public Properties

		/// <summary>
		///    Gets or sets the comment for the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the comment of the
		///    current instace.
		/// </value>
		public override string Comment {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets or sets the keywords for the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the keywords of the
		///    current instace.
		/// </value>
		public virtual string[] Keywords {
			get { return new string [] {}; }
			set {}
		}

		/// <summary>
		///    Gets or sets the rating for the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the rating of the
		///    current instace.
		/// </value>
		public virtual uint Rating {
			get { return 0; }
			set {}
		}

		/// <summary>
		///    Gets or sets the time when the image, the current instance
		///    belongs to, was taken.
		/// </summary>
		/// <value>
		///    A <see cref="DateTime" /> with the time the image was taken.
		/// </value>
		public virtual DateTime DateTime {
			get { return DateTime.MinValue; }
			set {}
		}

		/// <summary>
		///    Gets or sets the orientation of the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the orienatation of the
		///    image
		/// </value>
		public virtual uint Orientation {
			get { return 0; }
			set {}
		}

		/// <summary>
		///    Gets or sets the software the image, the current instance
		///    belongs to, was created with.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the name of the
		///    software the current instace was created with.
		/// </value>
		public virtual string Software {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets or sets the latitude of the GPS coordinate the current
		///    image was taken.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the latitude ranging from -90.0
		///    to +90.0 degrees.
		/// </value>
		public virtual double Latitude {
			get { return 0.0d; }
			set {}
		}

		/// <summary>
		///    Gets or sets the longitude of the GPS coordinate the current
		///    image was taken.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the longitude ranging from 0
		///    to +180.0 degrees.
		/// </value>
		public virtual double Longitude {
			get { return 0.0d; }
			set {}
		}

		/// <summary>
		///    Gets or sets the altitude of the GPS coordinate the current
		///    image was taken.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the altitude ranging from -90.0
		///    to +90.0 degrees.
		/// </value>
		public virtual double Altitude {
			get { return 0.0d; }
			set {}
		}

		/// <summary>
		///    Gets the exposure time the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the exposure time in seconds.
		/// </value>
		public virtual double ExposureTime {
			get { return 0.0d; }
		}

		/// <summary>
		///    Gets the FNumber the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the FNumber.
		/// </value>
		public virtual double FNumber {
			get { return 0.0d; }
		}

		/// <summary>
		///    Gets the ISO speed the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> with the ISO speed as defined in ISO 12232.
		/// </value>
		public virtual uint ISOSpeedRatings {
			get { return 0; }
		}

		/// <summary>
		///    Gets the focal length the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the focal length in millimeters.
		/// </value>
		public virtual double FocalLength {
			get { return 0.0d; }
		}

		/// <summary>
		///    Gets the manufacture of the recording equipment the image, the
		///    current instance belongs to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> with the manufacture name.
		/// </value>
		public virtual string Make {
			get { return null; }
		}

		/// <summary>
		///    Gets the model name of the recording equipment the image, the
		///    current instance belongs to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> with the model name.
		/// </value>
		public virtual string Model {
			get { return null; }
		}
#endregion

	}
}
