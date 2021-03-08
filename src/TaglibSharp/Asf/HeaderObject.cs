//
// HeaderObject.cs: Provides a representation of an ASF Header object which can
// be read from and written to disk.
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2006-2007 Brian Nickel
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

namespace TagLib.Asf
{
	/// <summary>
	///    This class extends <see cref="Object" /> to provide a
	///    representation of an ASF Header object which can be read from and
	///    written to disk.
	/// </summary>
	public class HeaderObject : Object
	{
		#region Private Fields

		/// <summary>
		///    Contains the reserved header data.
		/// </summary>
		readonly ByteVector reserved;

		/// <summary>
		///    Contains the child objects.
		/// </summary>
		readonly List<Object> children;

		#endregion



		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="HeaderObject" /> by reading the contents from a
		///    specified position in a specified file.
		/// </summary>
		/// <param name="file">
		///    A <see cref="Asf.File" /> object containing the file from
		///    which the contents of the new instance are to be read.
		/// </param>
		/// <param name="position">
		///    A <see cref="long" /> value specify at what position to
		///    read the object.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="file" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///    <paramref name="position" /> is less than zero or greater
		///    than the size of the file.
		/// </exception>
		/// <exception cref="CorruptFileException">
		///    The object read from disk does not have the correct GUID
		///    or smaller than the minimum size.
		/// </exception>
		public HeaderObject (File file, long position)
			: base (file, position)
		{
			if (!Guid.Equals (Asf.Guid.AsfHeaderObject))
				throw new CorruptFileException ("Object GUID incorrect.");

			if (OriginalSize < 26)
				throw new CorruptFileException ("Object size too small.");

			children = new List<Object> ();

			uint child_count = file.ReadDWord ();

			reserved = file.ReadBlock (2);

			children.AddRange (file.ReadObjects (child_count,
				file.Tell));
		}

		#endregion



		#region Public Properties

		/// <summary>
		///    Gets the header extension object contained in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="HeaderExtensionObject" /> object containing
		///    the header extension object.
		/// </value>
		public HeaderExtensionObject Extension {
			get {
				foreach (Object child in children)
					if (child is HeaderExtensionObject)
						return child as HeaderExtensionObject;
				return null;
			}
		}

		/// <summary>
		///    Gets the child objects contained in the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="T:System.Collections.Generic.IEnumerable`1" /> object enumerating
		///    through the children of the current instance.
		/// </value>
		public IEnumerable<Object> Children {
			get { return children; }
		}

		/// <summary>
		///    Gets the media properties contained within the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="Properties" /> object containing the media
		///    properties of the current instance.
		/// </value>
		public Properties Properties {
			get {
				TimeSpan duration = TimeSpan.Zero;
				var codecs = new List<ICodec> ();

				foreach (var obj in Children) {
					if (obj is FilePropertiesObject fpobj) {
						duration = fpobj.PlayDuration - TimeSpan.FromMilliseconds (fpobj.Preroll);
						continue;
					}

					if (obj is StreamPropertiesObject spobj) {
						codecs.Add (spobj.Codec);
					}
				}

				return new Properties (duration, codecs);
			}
		}

		/// <summary>
		///    Gets whether or not the current instance contains either
		///    type of content descriptiors.
		/// </summary>
		/// <value>
		///    <see langword="true" /> if the current instance contains
		///    a <see cref="ContentDescriptionObject" /> or a <see
		///    cref="ExtendedContentDescriptionObject" />. Otherwise
		///    <see langword="false" />.
		/// </value>
		public bool HasContentDescriptors {
			get {
				foreach (Object child in children)
					if (child.Guid == Asf.Guid.AsfContentDescriptionObject ||
						child.Guid == Asf.Guid.AsfExtendedContentDescriptionObject)
						return true;

				return false;
			}
		}

		#endregion



		#region Public Methods

		/// <summary>
		///    Renders the current instance as a raw ASF object.
		/// </summary>
		/// <returns>
		///    A <see cref="ByteVector" /> object containing the
		///    rendered version of the current instance.
		/// </returns>
		public override ByteVector Render ()
		{
			var output = new ByteVector ();
			uint child_count = 0;

			foreach (var child in children)
				if (child.Guid != Asf.Guid.AsfPaddingObject) {
					output.Add (child.Render ());
					child_count++;
				}

			long size_diff = (long)output.Count + 30 -
				(long)OriginalSize;

			if (size_diff != 0) {
				var obj = new PaddingObject ((uint)(size_diff > 0 ? 4096 : -size_diff));

				output.Add (obj.Render ());
				child_count++;
			}

			output.Insert (0, reserved);
			output.Insert (0, RenderDWord (child_count));
			return Render (output);
		}

		/// <summary>
		///    Adds a child object to the current instance.
		/// </summary>
		/// <param name="obj">
		///    A <see cref="Object" /> object to add to the current
		///    instance.
		/// </param>
		public void AddObject (Object obj)
		{
			children.Add (obj);
		}

		/// <summary>
		///    Adds a child unique child object to the current instance,
		///    replacing and existing child if present.
		/// </summary>
		/// <param name="obj">
		///    A <see cref="Object" /> object to add to the current
		///    instance.
		/// </param>
		public void AddUniqueObject (Object obj)
		{
			for (int i = 0; i < children.Count; i++)
				if (children[i].Guid == obj.Guid) {
					children[i] = obj;
					return;
				}

			children.Add (obj);
		}

		/// <summary>
		///    Removes the content description objects from the current
		///    instance.
		/// </summary>
		public void RemoveContentDescriptors ()
		{
			for (int i = children.Count - 1; i >= 0; i--)
				if (children[i].Guid == Asf.Guid.AsfContentDescriptionObject ||
					children[i].Guid == Asf.Guid.AsfExtendedContentDescriptionObject)
					children.RemoveAt (i);
		}

		#endregion
	}
}
