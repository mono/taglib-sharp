//
// AppleTag.cs: Provides support for processing Apple "ilst" tags.
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace TagLib.Mpeg4 {
	/// <summary>
	///    This class extends <see cref="TagLib.Tag" /> to provide support
	///    for processing Apple "ilst" tags.
	/// </summary>
	public class AppleTag : TagLib.Tag, IEnumerable<Box>
	{
		#region Private Fields
		
		/// <summary>
		///    Contains the ISO meta box in which that tag will be
		///    stored.
		/// </summary>
		private IsoMetaBox meta_box;
		
		/// <summary>
		///    Contains the ILST box which holds all the values.
		/// </summary>
		private AppleItemListBox ilst_box;
		
		#endregion
		
		
		
		#region Constructors
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AppleTag" /> for a specified ISO user data box.
		/// </summary>
		/// <param name="box">
		///    A <see cref="IsoUserDataBox" /> from which the tag is to
		///    be read.
		/// </param>
		public AppleTag (IsoUserDataBox box)
		{
			if (box == null)
				throw new ArgumentNullException ("box");
			
			meta_box = box.GetChild (BoxType.Meta) as IsoMetaBox;
			if (meta_box == null) {
				meta_box = new IsoMetaBox ("mdir", null);
				box.AddChild (meta_box);
			}
			
			ilst_box = meta_box.GetChild (BoxType.Ilst)
				as AppleItemListBox;
			
			if (ilst_box == null) {
				ilst_box = new AppleItemListBox ();
				meta_box.AddChild (ilst_box);
			}
		}
		
		#endregion
		
		
		
		#region Public Methods
		
		/// <summary>
		///    Gets and sets whether or not the album described by the
		///    current instance is a compilation.
		/// </summary>
		/// <value>
		///    A <see cref="bool" /> value indicating whether or not the
		///    album described by the current instance is a compilation.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "cpil" data box.
		/// </remarks>
		public bool IsCompilation {
			get {
				foreach (AppleDataBox box in DataBoxes (
					BoxType.Cpil))
					return box.Data.ToUInt () != 0;
				
				return false;
			}
			set {
				SetData (BoxType.Cpil, new ByteVector(
					(byte) (value ? 1 : 0)),
					(uint) AppleDataBox.FlagType.ForTempo);
			}
		}
		
		#endregion
		
		
		
		#region Public Methods
		
		/// <summary>
		///    Gets all data boxes that match any of the provided types.
		/// </summary>
		/// <param name="types">
		///    A <see cref="T:System.Collections.Generic.IEnumerable`1" /> object enumerating a list
		///    of box types to match.
		/// </param>
		/// <returns>
		///    A <see cref="T:System.Collections.Generic.IEnumerable`1" /> object enumerating the
		///    matching boxes.
		/// </returns>
		public IEnumerable<AppleDataBox> DataBoxes (IEnumerable<ByteVector> types)
		{
			// Check each box to see if the match any of the
			// provided types. If a match is found, loop through the
			// children and add any data box.
			foreach (Box box in ilst_box.Children)
				foreach (ByteVector v in types)
					if (FixId (v) == box.BoxType)
						foreach (Box data_box in box.Children)
							if (data_box is AppleDataBox)
								yield return data_box as AppleDataBox;
		}
		
		/// <summary>
		///    Gets all data boxes that match any of the provided types.
		/// </summary>
		/// <param name="types">
		///    A <see cref="ByteVector[]" /> containing list of box
		///    types to match.
		/// </param>
		/// <returns>
		///    A <see cref="T:System.Collections.Generic.IEnumerable`1" /> object enumerating the
		///    matching boxes.
		/// </returns>
		public IEnumerable<AppleDataBox> DataBoxes (params ByteVector [] types)
		{
			return DataBoxes (types as IEnumerable<ByteVector>);
		}
		
		/// <summary>
		///    Gets all custom data boxes that match the specified mean
		///    and name pair.
		/// </summary>
		/// <param name="mean">
		///    A <see cref="string" /> object containing the "mean" to
		///    match.
		/// </param>
		/// <param name="name">
		///    A <see cref="string" /> object containing the name to
		///    match.
		/// </param>
		/// <returns>
		///    A <see cref="T:System.Collections.Generic.IEnumerable`1" /> object enumerating the
		///    matching boxes.
		/// </returns>
		public IEnumerable<AppleDataBox> DataBoxes (string mean,
		                                            string name)
		{
			// These children will have a box type of "----"
			foreach (Box box in ilst_box.Children) {
				if (box.BoxType != BoxType.DASH)
					continue;
				
				// Get the mean and name boxes, make sure
				// they're legit, and make sure that they match
				// what we want. Then loop through and add all
				// the data box children to our output.
				AppleAdditionalInfoBox mean_box =
					(AppleAdditionalInfoBox)
					box.GetChild (BoxType.Mean);
				AppleAdditionalInfoBox name_box =
					(AppleAdditionalInfoBox)
					box.GetChild (BoxType.Name);
				
				if (mean_box == null || name_box == null ||
					mean_box.Text != mean ||
					name_box.Text != name)
					continue;
				
				foreach (Box data_box in box.Children) {
					AppleDataBox adb =
						data_box as AppleDataBox;
					
					if (adb != null)
						yield return adb;
				}
			}
		}
		
		/// <summary>
		///    Gets all text values contained in a specified box type.
		/// </summary>
		/// <param name="type">
		///    A <see cref="ByteVector" /> object containing the box
		///    type to match.
		/// </param>
		/// <returns>
		///    A <see cref="string[]" /> containing text from all
		///    matching boxes.
		/// </returns>
		public string [] GetText (ByteVector type) {
			List<string> result = new List<string> ();
			foreach (AppleDataBox box in DataBoxes (type))
				if (box.Text != null)
					result.Add (box.Text);
			return result.ToArray ();
		}
		
		/// <summary>
		///    Sets the data for a specified box type to a collection of
		///    boxes.
		/// </summary>
		/// <param name="type">
		///    A <see cref="ByteVector" /> object containing the type to
		///    add to the new instance.
		/// </param>
		/// <param name="boxes">
		///    A <see cref="AppleDataBox[]" /> containing boxes to add
		///    for the specified type.
		/// </param>
		public void SetData (ByteVector type, AppleDataBox [] boxes)
		{
			// Fix the type.
			type = FixId (type);
			
			bool added = false;
			
			foreach (Box box in ilst_box.Children)
				if (type == box.BoxType) {
					
					// Clear the box's children.
					box.ClearChildren ();
					
					// If we've already added new childen,
					// continue.
					if (added)
						continue;
					
					added = true;
					
					// Add the children.
					foreach (AppleDataBox b in boxes)
						box.AddChild (b);
				}
			
			if (added)
				return;
			
			Box box2 = new AppleAnnotationBox (type);
			ilst_box.AddChild (box2);
			
			foreach (AppleDataBox b in boxes)
				box2.AddChild (b);
		}
		
		/// <summary>
		///    Sets the data for a specified box type using values from
		///    a <see cref="ByteVectorCollection" /> object.
		/// </summary>
		/// <param name="type">
		///    A <see cref="ByteVector" /> object containing the type to
		///    add to the new instance.
		/// </param>
		/// <param name="data">
		///    A <see cref="ByteVectorCollection" /> object containing
		///    data to add for the specified type.
		/// </param>
		/// <param name="flags">
		///    A <see cref="uint" /> value containing flags to use for
		///    the added boxes.
		/// </param>
		public void SetData (ByteVector type, ByteVectorCollection data,
		                     uint flags)
		{
			if (data == null || data.Count == 0) {
				ClearData (type);
				return;
			}
			
			AppleDataBox [] boxes = new AppleDataBox [data.Count];
			for (int i = 0; i < data.Count; i ++)
				boxes [i] = new AppleDataBox (data [i], flags);
			
			SetData (type, boxes);
		}
		
		/// <summary>
		///    Sets the data for a specified box type using a single
		///    <see cref="ByteVector" /> object.
		/// </summary>
		/// <param name="type">
		///    A <see cref="ByteVector" /> object containing the type to
		///    add to the new instance.
		/// </param>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object containing data to add
		///    for the specified type.
		/// </param>
		/// <param name="flags">
		///    A <see cref="uint" /> value containing flags to use for
		///    the added box.
		/// </param>
		public void SetData (ByteVector type, ByteVector data,
		                     uint flags)
		{
			if (data == null || data.Count == 0)
				ClearData (type);
			else
				SetData (type, new ByteVectorCollection (data),
					flags);
		}
      
		/// <summary>
		///    Sets the text for a specified box type.
		/// </summary>
		/// <param name="type">
		///    A <see cref="ByteVector" /> object containing the type to
		///    add to the new instance.
		/// </param>
		/// <param name="text">
		///    A <see cref="string[]" /> containing text to store.
		/// </param>
		public void SetText (ByteVector type, string [] text)
		{
			// Remove empty data and return.
			if (text == null) {
				ilst_box.RemoveChild (FixId (type));
				return;
			}
			
			// Create a list...
			ByteVectorCollection l = new ByteVectorCollection ();
			
			// and populate it with the ByteVectorized strings.
			foreach (string value in text)
				l.Add (ByteVector.FromString (value,
					StringType.UTF8));
			
			// Send our final byte vectors to SetData
			SetData (type, l, (uint)
				AppleDataBox.FlagType.ContainsText);
		}
		
		/// <summary>
		///    Sets the text for a specified box type.
		/// </summary>
		/// <param name="type">
		///    A <see cref="ByteVector" /> object containing the type to
		///    add to the new instance.
		/// </param>
		/// <param name="text">
		///    A <see cref="string" /> object containing text to store.
		/// </param>
		public void SetText (ByteVector type, string text)
		{
			// Remove empty data and return.
			if (string.IsNullOrEmpty (text)) {
				ilst_box.RemoveChild (FixId (type));
				return;
			}
			
			SetText (type, new string [] {text});
		}
		
		/// <summary>
		///    Clears all data for a specified box type.
		/// </summary>
		/// <param name="type">
		///    A <see cref="ByteVector" /> object containing the type of
		///    box to remove from the current instance.
		/// </param>
		public void ClearData (ByteVector type)
		{
			ilst_box.RemoveChild (FixId (type));
		}
		
		/// <summary>
		///    Detaches the internal "ilst" box from its parent element.
		/// </summary>
		public void DetachIlst ()
		{
			meta_box.RemoveChild (ilst_box);
		}
		
		#endregion
		
		
		
		#region Internal Methods
		
		/// <summary>
		///    Converts the provided ID into a readonly ID and fixes a
		///    3 byte ID.
		/// </summary>
		/// <param name="id">
		///    A <see cref="ByteVector" /> object containing an ID to
		///    fix.
		/// </param>
		/// <returns>
		///    A fixed <see cref="ReadOnlyByteVector" /> or <see
		///    langword="null" /> if the ID could not be fixed.
		/// </returns>
		internal static ReadOnlyByteVector FixId (ByteVector id)
		{
			if (id.Count == 4) {
				ReadOnlyByteVector roid =
					id as ReadOnlyByteVector;
				if (roid != null)
					return roid;
				
				return new ReadOnlyByteVector (id);
			}
			
			if (id.Count == 3)
				return new ReadOnlyByteVector (
					0xa9, id [0], id [1], id [2]);
			
			return null;
		}
		
		#endregion
		
		
		
		#region IEnumerable<Box>
		
		public IEnumerator<Box> GetEnumerator()
		{
			return ilst_box.Children.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ilst_box.Children.GetEnumerator();
		}
		
		#endregion
		
		
		
		#region TagLib.Tag
		
		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    Always <see cref="TagTypes.Apple" />.
		/// </value>
		public override TagTypes TagTypes {
			get {return TagTypes.Apple;}
		}
		
		/// <summary>
		///    Gets and sets the title for the media described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the title for
		///    the media described by the current instance or <see
		///    langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "@nam" data box.
		/// </remarks>
		public override string Title {
			get {
				string [] text = GetText (BoxType.Nam);
				return text.Length == 0 ? null : text [0];
			}
			set {
				SetText (BoxType.Nam, value);
			}
		}
		
		/// <summary>
		///    Gets and sets the performers or artists who performed in
		///    the media described by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the performers or
		///    artists who performed in the media described by the
		///    current instance or an empty array if no value is
		///    present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "@ART" data box.
		/// </remarks>
		public override string [] Performers {
			get {return GetText (BoxType.Art);}
			set {SetText (BoxType.Art, value);}
		}
		
		/// <summary>
		///    Gets and sets the band or artist who is credited in the
		///    creation of the entire album or collection containing the
		///    media described by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the band or artist
		///    who is credited in the creation of the entire album or
		///    collection containing the media described by the current
		///    instance or an empty array if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "aART" data box.
		/// </remarks>
		public override string [] AlbumArtists {
			get {return GetText (BoxType.Aart);}
			set {SetText(BoxType.Aart, value);}
		}
		
		/// <summary>
		///    Gets and sets the composers of the media represented by
		///    the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the composers of the
		///    media represented by the current instance or an empty
		///    array if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "@wrt" data box.
		/// </remarks>
		public override string [] Composers {
			get {return GetText (BoxType.Wrt);}
			set {SetText (BoxType.Wrt, value);}
		}
		
		/// <summary>
		///    Gets and sets the album of the media represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the album of
		///    the media represented by the current instance or <see
		///    langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "@alb" data box.
		/// </remarks>
		public override string Album {
			get {
				string [] text = GetText (BoxType.Alb);
				return text.Length == 0 ? null : text [0];
			}
			set {SetText (BoxType.Alb, value);}
		}
		
		/// <summary>
		///    Gets and sets a user comment on the media represented by
		///    the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing user comments
		///    on the media represented by the current instance or <see
		///    langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "@cmt" data box.
		/// </remarks>
		public override string Comment {
			get {
				string [] text = GetText (BoxType.Cmt);
				return text.Length == 0 ? null : text [0];
			}
			set {SetText (BoxType.Cmt, value);}
		}
		
		/// <summary>
		///    Gets and sets the genres of the media represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the genres of the
		///    media represented by the current instance or an empty
		///    array if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "@gen" and "gnre"
		///    data boxes.
		/// </remarks>
		public override string [] Genres {
			get {
				List<string> result = new List<string> ();
				ByteVector [] names = new ByteVector [] {
					BoxType.Gen, BoxType.Gnre
				};
				
				foreach (AppleDataBox box in DataBoxes (names)) {
					if (box.Text != null) {
						result.Add (box.Text);
						continue;
					}
					
					if (box.Flags != (int)
						AppleDataBox.FlagType.ContainsData)
						continue;
					
					// iTunes stores genre's in the GNRE box
					// as (ID3# + 1).
					string genre = TagLib.Genres.IndexToAudio (
						(byte) (box.Data.ToUShort (true) - 1));
					
					if (genre != null)
						result.Add (genre);
				}
				return result.ToArray ();
			}
			set {
				ClearData (BoxType.Gnre);
				SetText (BoxType.Gen, value);
			}
		}
		
		/// <summary>
		///    Gets and sets the year that the media represented by the
		///    current instance was recorded.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the year that the media
		///    represented by the current instance was created or zero
		///    if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "@day" data box.
		/// </remarks>
		public override uint Year {
			get {
				uint value;
				foreach (AppleDataBox box in DataBoxes (BoxType.Day))
					if (box.Text != null && (uint.TryParse (
						box.Text, out value) ||
						uint.TryParse (
							box.Text.Length > 4 ?
							box.Text.Substring (0, 4)
							: box.Text, out value)))
						return value;
				
				return 0;
			}
			set {
				if (value == 0)
					ClearData (BoxType.Day);
				else
					SetText (BoxType.Day, value.ToString (
						CultureInfo.InvariantCulture));
			}
		}
		
		/// <summary>
		///    Gets and sets the position of the media represented by
		///    the current instance in its containing album.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the position of the
		///    media represented by the current instance in its
		///    containing album or zero if not specified.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "trkn" data box.
		/// </remarks>
		public override uint Track {
			get {
				foreach (AppleDataBox box in DataBoxes (BoxType.Trkn))
					if (box.Flags == (int)
						AppleDataBox.FlagType.ContainsData &&
						box.Data.Count >= 4)
						return box.Data.Mid (2, 2).ToUShort ();
				
				return 0;
			}
			set {
				uint count = TrackCount;
				if (value == 0 && count == 0) {
					ClearData (BoxType.Trkn);
					return;
				}
				
				ByteVector v = ByteVector.FromUShort (0);
				v.Add (ByteVector.FromUShort ((ushort) value));
				v.Add (ByteVector.FromUShort ((ushort) count));
				v.Add (ByteVector.FromUShort (0));
				
				SetData (BoxType.Trkn, v, (int)
					AppleDataBox.FlagType.ContainsData);
			}
		}
		
		/// <summary>
		///    Gets and sets the number of tracks in the album
		///    containing the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of tracks in
		///    the album containing the media represented by the current
		///    instance or zero if not specified.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "trkn" data box.
		/// </remarks>
		public override uint TrackCount {
			get {
				foreach (AppleDataBox box in DataBoxes (BoxType.Trkn))
					if (box.Flags == (int)
						AppleDataBox.FlagType.ContainsData &&
						box.Data.Count >= 6)
						return box.Data.Mid (4, 2).ToUShort ();
				
				return 0;
			}
			set {
				uint track = Track;
				if (value == 0 && track == 0) {
					ClearData (BoxType.Trkn);
					return;
				}
				
				ByteVector v = ByteVector.FromUShort (0);
				v.Add (ByteVector.FromUShort ((ushort) track));
				v.Add (ByteVector.FromUShort ((ushort) value));
				v.Add (ByteVector.FromUShort (0));
				SetData (BoxType.Trkn, v, (int)
					AppleDataBox.FlagType.ContainsData);
			}
		}
		
		/// <summary>
		///    Gets and sets the number of the disc containing the media
		///    represented by the current instance in the boxed set.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of the disc
		///    containing the media represented by the current instance
		///    in the boxed set.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "disk" data box.
		/// </remarks>
		public override uint Disc {
			get {
				foreach (AppleDataBox box in DataBoxes (BoxType.Disk))
					if (box.Flags == (int)
						AppleDataBox.FlagType.ContainsData &&
						box.Data.Count >= 4)
						return box.Data.Mid (2, 2).ToUShort ();
				
				return 0;
			}
			set {
				uint count = DiscCount;
				if (value == 0 && count == 0) {
					ClearData (BoxType.Disk);
					return;
				}
				
				ByteVector v = ByteVector.FromUShort (0);
				v.Add (ByteVector.FromUShort ((ushort) value));
				v.Add (ByteVector.FromUShort ((ushort) count));
				v.Add (ByteVector.FromUShort (0));
				
				SetData (BoxType.Disk, v, (int)
					AppleDataBox.FlagType.ContainsData);
			}
		}
		
		/// <summary>
		///    Gets and sets the number of discs in the boxed set
		///    containing the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of discs in
		///    the boxed set containing the media represented by the
		///    current instance or zero if not specified.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "disk" data box.
		/// </remarks>
		public override uint DiscCount {
			get {
				foreach (AppleDataBox box in DataBoxes (BoxType.Disk))
					if (box.Flags == (int)
						AppleDataBox.FlagType.ContainsData &&
						box.Data.Count >= 6)
						return box.Data.Mid (4, 2).ToUShort ();
				
				return 0;
			}
			set {
				uint disc = Disc;
				if (value == 0 && disc == 0) {
					ClearData (BoxType.Disk);
					return;
				}
				
				ByteVector v = ByteVector.FromUShort (0);
				v.Add (ByteVector.FromUShort ((ushort) disc));
				v.Add (ByteVector.FromUShort ((ushort) value));
				v.Add (ByteVector.FromUShort (0));
				SetData (BoxType.Disk, v, (int)
					AppleDataBox.FlagType.ContainsData);
			}
		}
		
		/// <summary>
		///    Gets and sets the lyrics or script of the media
		///    represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the lyrics or
		///    script of the media represented by the current instance
		///    or <see langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "@lyr" data box.
		/// </remarks>
		public override string Lyrics {
			get {
				foreach (AppleDataBox box in DataBoxes (BoxType.Lyr))
					return box.Text;
				return null;
			}
			set {
				SetText (BoxType.Lyr, value);
			}
		}
		
		/// <summary>
		///    Gets and sets the grouping on the album which the media
		///    in the current instance belongs to.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the grouping on
		///    the album which the media in the current instance belongs
		///    to or <see langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "@grp" data box.
		/// </remarks>
		public override string Grouping {
			get {
				foreach (AppleDataBox box in DataBoxes(BoxType.Grp))
					return box.Text;
				
				return null;
			}
			set {SetText(BoxType.Grp, value);}
		}
		
		/// <summary>
		///    Gets and sets the number of beats per minute in the audio
		///    of the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of beats per
		///    minute in the audio of the media represented by the
		///    current instance, or zero if not specified.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "tmpo" data box.
		/// </remarks>
		public override uint BeatsPerMinute {
			get {
				foreach (AppleDataBox box in DataBoxes (BoxType.Tmpo))
					if (box.Flags == (uint)
						AppleDataBox.FlagType.ForTempo)
						return box.Data.ToUInt ();
				
				return 0;
			}
			set {
				if (value == 0) {
					ClearData (BoxType.Tmpo);
					return;
				}
				
				SetData (BoxType.Tmpo,
					ByteVector.FromUInt (value),
					(uint) AppleDataBox.FlagType.ForTempo);
			}
		}
      
		/// <summary>
		///    Gets and sets the conductor or director of the media
		///    represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the conductor
		///    or director of the media represented by the current
		///    instance or <see langref="null" /> if no value present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "cond" data box.
		/// </remarks>
		public override string Conductor {
			get {
				foreach (AppleDataBox box in DataBoxes(BoxType.Cond))
					return box.Text;
				
				return null;
			}
			set {SetText(BoxType.Cond, value);}
		}
		
		/// <summary>
		///    Gets and sets the copyright information for the media
		///    represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the copyright
		///    information for the media represented by the current
		///    instance or <see langword="null" /> if no value present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "cprt" data box.
		/// </remarks>
		public override string Copyright {
			get {
				foreach (AppleDataBox box in DataBoxes(BoxType.Cprt))
					return box.Text;
				
				return null;
			}
			set {SetText(BoxType.Cprt, value);}
		}
		
		/// <summary>
		///    Gets and sets a collection of pictures associated with
		///    the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="IPicture[]" /> containing a collection of
		///    pictures associated with the media represented by the
		///    current instance or an empty array if none are present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "covr" data box.
		/// </remarks>
		public override IPicture [] Pictures {
			get {
				List<Picture> l = new List<Picture> ();
				
				foreach (AppleDataBox box in DataBoxes(BoxType.Covr)) {
					Picture p = new Picture (box.Data);
					p.Type = PictureType.FrontCover;
					l.Add (p);
				}
				
				return (Picture []) l.ToArray ();
			}
			set {
				if (value == null || value.Length == 0) {
					ClearData (BoxType.Covr);
					return;
				}
				
				AppleDataBox [] boxes =
					new AppleDataBox [value.Length];
				for (int i = 0; i < value.Length; i ++) {
					uint type = (uint)
						AppleDataBox.FlagType.ContainsData;
					
					if (value [i].MimeType == "image/jpeg")
						type = (uint)
							AppleDataBox.FlagType.ContainsJpegData;
					else if (value [i].MimeType == "image/png")
						type = (uint)
							AppleDataBox.FlagType.ContainsPngData;
					
					boxes [i] = new AppleDataBox (value [i].Data, type);
				}
				
				SetData(BoxType.Covr, boxes);
			}
		}
		
		/// <summary>
		///    Gets whether or not the current instance is empty.
		/// </summary>
		/// <value>
		///    <see langword="true" /> if the current instance does not
		///    any values. Otherwise <see langword="false" />.
		/// </value>
		public override bool IsEmpty {
			get {return !ilst_box.HasChildren;}
		}
		
		/// <summary>
		///    Clears the values stored in the current instance.
		/// </summary>
		public override void Clear ()
		{
			ilst_box.ClearChildren ();
		}
		
		#endregion
	}
}
