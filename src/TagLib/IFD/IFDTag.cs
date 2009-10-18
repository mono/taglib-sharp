//
// IFDTag.cs: Basic Tag-class to handle an IFD (Image File Directory) with
// its image-tags.
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
using System.Collections.Generic;
using System.IO;

using TagLib.Image;
using TagLib.IFD.Entries;

namespace TagLib.IFD
{
	/// <summary>
	///    Contains the metadata for one IFD (Image File Directory).
	/// </summary>
	public class IFDTag : ImageTag
	{
		public static readonly string DATETIME_FORMAT = "yyyy:MM:dd HH:mm:ss";

#region Private Fields

		/// <summary>
		///    Contains the IFD directories in this tag.
		/// </summary>
		internal readonly List<IFDDirectory> directories = new List<IFDDirectory> ();

#endregion

#region Constructors

		/// <summary>
		///    Constructor. Creates an empty IFD tag. Can be populated manually, or via
		///    <see cref="IFDReader"/>.
		/// </summary>
		public IFDTag ()
		{
		}

#endregion

#region Public Properties

		/// <summary>
		///    Gets the IFD directories contained in the current instance.
		/// </summary>
		/// <value>
		///    An array of <see cref="IFDirectory"/> instances.
		/// </value>
		public IFDDirectory [] Directories {
			get { return directories.ToArray (); }
		}

		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    Always <see cref="TagTypes.TiffIFD" />.
		/// </value>
		public override TagTypes TagTypes {
			get { return TagTypes.TiffIFD; }
		}

#endregion

#region Public Methods

		/// <summary>
		///    Clears the values stored in the current instance.
		/// </summary>
		public override void Clear ()
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		///    Checks, if a value for the given tag is contained in the IFD.
		/// </summary>
		/// <param name="directory">
		///    A <see cref="System.Int32"/> value with the directory index that
		///    contains the tag.
		/// </param>
		/// <param name="tag">
		///    A <see cref="System.UInt32"/> value with the tag.
		/// </param>
		/// <returns>
		///    A <see cref="System.Boolean"/>, which is true, if the tag is already
		///    contained in the IFD, otherwise false.
		/// </returns>
		public bool ContainsTag (int directory, uint tag)
		{
			if (directory >= directories.Count)
				return false;
			return directories [directory].ContainsKey (tag);
		}

		/// <summary>
		///    Removes a given tag from the IFD.
		/// </summary>
		/// <param name="directory">
		///    A <see cref="System.Int32"/> value with the directory index that
		///    contains the tag to remove.
		/// </param>
		/// <param name="tag">
		///    A <see cref="System.UInt32"/> value with the tag to remove.
		/// </param>
		public void RemoveTag (int directory, uint tag)
		{
			directories [directory].Remove (tag);
		}

		/// <summary>
		///    Removes a given tag from the IFD.
		/// </summary>
		/// <param name="directory">
		///    A <see cref="System.Int32"/> value with the directory index that
		///    contains the tag to remove.
		/// </param>
		/// <param name="entry_tag">
		///    A <see cref="IFDEntryTag"/> value with the tag to remove.
		/// </param>
		public void RemoveTag (int directory, IFDEntryTag entry_tag)
		{
			RemoveTag (directory, (uint) entry_tag);
		}

		/// <summary>
		///    Adds an <see cref="IFDEntry"/> to the IFD, if it is not already
		///    contained in, it fails otherwise.
		/// </summary>
		/// <param name="directory">
		///    A <see cref="System.Int32"/> value with the directory index that
		///    should contain the tag that will be added.
		/// </param>
		/// <param name="entry">
		///    A <see cref="IFDEntry"/> to add to the IFD.
		/// </param>
		public void AddEntry (int directory, IFDEntry entry)
		{
			while (directory >= directories.Count)
				directories.Add (new IFDDirectory ());

			directories [directory].Add (entry.Tag, entry);
		}

		/// <summary>
		///    Adds an <see cref="IFDEntry"/> to the IFD. If it is already contained
		///    in the IFD, it is overwritten.
		/// </summary>
		/// <param name="directory">
		///    A <see cref="System.Int32"/> value with the directory index that
		///    contains the tag that will be set.
		/// </param>
		/// <param name="entry">
		///    A <see cref="IFDEntry"/> to add to the IFD.
		/// </param>
		public void SetEntry (int directory, IFDEntry entry)
		{
			if (ContainsTag (directory, entry.Tag))
				RemoveTag (directory, entry.Tag);

			AddEntry (directory, entry);
		}

		/// <summary>
		///   Returns the <see cref="IFDEntry"/> belonging to the given tag.
		/// </summary>
		/// <param name="directory">
		///    A <see cref="System.Int32"/> with the directory that contains
		///    the wanted tag.
		/// </param>
		/// <param name="entry_tag">
		///    A <see cref="System.UInt16"/> with the tag to get.
		/// </param>
		/// <returns>
		///    A <see cref="IFDEntry"/> belonging to the given tag, or
		///    null, if no such tag is contained in the IFD.
		/// </returns>
		public IFDEntry GetEntry (int directory, ushort entry_tag)
		{
			if (!ContainsTag (directory, entry_tag))
				return null;

			return directories [directory] [entry_tag];
		}

		/// <summary>
		///   Returns the <see cref="IFDEntry"/> belonging to the given tag.
		/// </summary>
		/// <param name="directory">
		///    A <see cref="System.Int32"/> with the directory that contains
		///    the wanted tag.
		/// </param>
		/// <param name="tag">
		///    A <see cref="IFDEntryTag"/> with the tag to get.
		/// </param>
		/// <returns>
		///    A <see cref="IFDEntry"/> belonging to the given tag, or
		///    null, if no such tag is contained in the IFD.
		/// </returns>
		public IFDEntry GetEntry (int directory, IFDEntryTag entry_tag)
		{
			return GetEntry (directory, (ushort) entry_tag);
		}

		public string GetStringValue (int directory, ushort entry_tag)
		{
			var entry = GetEntry (directory, entry_tag);

			if (entry != null && entry is StringIFDEntry)
				return (entry as StringIFDEntry).Value;

			return null;
		}

		public uint GetLongValue (int directory, ushort entry_tag)
		{
			var entry = GetEntry (directory, entry_tag);

			if (entry != null) {
				if (entry is LongIFDEntry)
					return (entry as LongIFDEntry).Value;

				if (entry is ShortIFDEntry)
					return (entry as ShortIFDEntry).Value;
			}

			return 0;
		}

		public double GetRationalValue (int directory, ushort entry_tag)
		{
			var entry = GetEntry (directory, entry_tag);

			if (entry != null) {

				if (entry is RationalIFDEntry)
					return (entry as RationalIFDEntry).Value;

				if (entry is SRationalIFDEntry)
					return (entry as SRationalIFDEntry).Value;
			}

			return 0.0d;
		}

		public DateTime GetDateTimeValue (int directory, ushort entry_tag)
		{
			string date_string = GetStringValue (directory, entry_tag);

			if (date_string == null)
				return DateTime.MinValue;

			try {
				DateTime date_time = DateTime.ParseExact (date_string, DATETIME_FORMAT, System.Globalization.CultureInfo.InvariantCulture);

				return date_time;
			} catch {}

			return DateTime.MinValue;
		}

		public void SetStringValue (int directory, ushort entry_tag, string value)
		{
			SetEntry (directory, new StringIFDEntry (entry_tag, value));
		}

		public void SetDateTimeValue (int directory, ushort entry_tag, DateTime value)
		{
			string date_string = value.ToString (DATETIME_FORMAT);

			SetStringValue (directory, entry_tag, date_string);
		}

#endregion

	}
}
