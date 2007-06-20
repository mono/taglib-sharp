using System;
using System.Collections;
using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class AppleTag : TagLib.Tag, IEnumerable<Box>
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private IsoMetaBox meta_box;
      private AppleItemListBox ilst_box;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public AppleTag (IsoUserDataBox box) : base ()
      {
         if (box == null)
            throw new ArgumentNullException ("box");
         
         meta_box = box.GetChild (BoxType.Meta) as IsoMetaBox;
         if (meta_box == null)
         {
            meta_box = new IsoMetaBox ("mdir", null);
            box.AddChild (meta_box);
         }
         
         ilst_box = meta_box.GetChild (BoxType.Ilst) as AppleItemListBox;
         
         if (ilst_box == null)
         {
            ilst_box = new AppleItemListBox ();
            meta_box.AddChild (ilst_box);
         }
      }
      
      public IEnumerator<Box> GetEnumerator()
      {
         return ilst_box.Children.GetEnumerator();
      }
      
      IEnumerator IEnumerable.GetEnumerator()
      {
         return ilst_box.Children.GetEnumerator();
      }
      
      // Get all the data boxes with the provided types.
      public IEnumerable<AppleDataBox> DataBoxes (IEnumerable<ByteVector> list)
      {
         // Check each box to see if the match any of the provided types.
         // If a match is found, loop through the children and add any data box.
         foreach (Box box in ilst_box.Children)
            foreach (ByteVector v in list)
               if (FixId (v) == box.BoxType)
                  foreach (Box data_box in box.Children)
                     if (data_box is AppleDataBox)
                        yield return data_box as AppleDataBox;
      }
      
      // Get all the data boxes with a given type.
      public IEnumerable<AppleDataBox> DataBoxes (params ByteVector [] vects)
      {
         return DataBoxes (vects as IEnumerable<ByteVector>);
      }
      
      // Find all the data boxes with a given mean and name.
      public IEnumerable<AppleDataBox> DataBoxes (string mean, string name)
      {
         // These children will have a box type of "----"
         foreach (Box box in ilst_box.Children)
            if (box.BoxType == BoxType.DASH)
            {
               // Get the mean and name boxes, make sure they're legit, and make
               // sure that they match what we want. Then loop through and add
               // all the data box children to our output.
               AppleAdditionalInfoBox mean_box = (AppleAdditionalInfoBox) box.GetChild (BoxType.Mean);
               AppleAdditionalInfoBox name_box = (AppleAdditionalInfoBox) box.GetChild (BoxType.Name);
               if (mean_box != null && name_box != null && mean_box.Text == mean && name_box.Text == name)
                  foreach (Box data_box in box.Children)
                     if (data_box is AppleDataBox)
                        yield return data_box as AppleDataBox;
            }
      }
      
      // Get all the text data boxes from a given box type.
      public string [] GetText (ByteVector type)
      {
         StringCollection l = new StringCollection ();
         foreach (AppleDataBox box in DataBoxes (type))
         {
            if (box.Text != null)
               l.Add (box.Text);
         }
         return l.ToArray ();
      }
      
      // Set the data with the given box type, data, and flags.
      public void SetData (ByteVector type, AppleDataBox [] boxes)
      {
         // Fix the type.
         type = FixId (type);
         
         bool first = true;
         
         // Loop through the children and find all with the type.
         foreach (Box box in ilst_box.Children)
            if (type == box.BoxType)
            {
               // If this is our first child...
               if (first)
               {
                  // clear its children and add our data boxes.
                  box.ClearChildren ();
                  foreach (AppleDataBox b in boxes)
                     box.AddChild (b);
                  first = false;
               }
            }
         
         // If we didn't find the box..
         if (first == true)
         {
            // Add the box and try again.
            Box box = new AppleAnnotationBox (type);
            ilst_box.AddChild (box);
            SetData (type, boxes);
         }
      }
      
      public void SetData (ByteVector type, ByteVectorCollection data, uint flags)
      {
      	if (data == null || data.Count == 0)
      	{
	      	ClearData (type);
	      	return;
	      }
	      	
      	AppleDataBox [] boxes = new AppleDataBox [data.Count];
      	for (int i = 0; i < data.Count; i ++)
      		boxes [i] = new AppleDataBox (data [i], flags);
      	
      	SetData (type, boxes);
      }
      
      // Set the data with the given box type, data, and flags.
      public void SetData (ByteVector type, ByteVector data, uint flags)
      {
      	if (data == null || data.Count == 0)
	      	ClearData (type);
	      else
	         SetData (type, new ByteVectorCollection (data), flags);
      }
      
      // Set the data with the given box type, strings, and flags.
      public void SetText (ByteVector type, string [] text)
      {
         // Remove empty data and return.
         if (text == null)
         {
            ilst_box.RemoveChild (FixId (type));
            return;
         }
         
         // Create a list...
         ByteVectorCollection l = new ByteVectorCollection ();
         
         // and populate it with the ByteVectorized strings.
         foreach (string value in text)
            l.Add (ByteVector.FromString (value, StringType.UTF8));
         
         // Send our final byte vectors to SetData
         SetData (type, l, (uint) AppleDataBox.FlagType.ContainsText);
      }
      
      // Set the data with the given box type, string, and flags.
      public void SetText (ByteVector type, string text)
      {
         // Remove empty data and return.
         if (string.IsNullOrEmpty (text))
         {
            ilst_box.RemoveChild (FixId (type));
            return;
         }
         
         SetText (type, new string [] {text});
      }
      
      // Clear all data associated with a box type.
      public void ClearData (ByteVector type)
      {
         ilst_box.RemoveChild (FixId (type));
      }
      
      public void DetachIlst ()
      {
         meta_box.RemoveChild (ilst_box);
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TagTypes TagTypes {get {return TagTypes.Apple;}}
      
      public override string Title
      {
         get
         {
            string [] text = GetText (BoxType.Nam);
            return text.Length == 0 ? null : text [0];
         }
         set {SetText (BoxType.Nam, value);}
      }
      
      public override string [] AlbumArtists
      {
         get {return GetText (BoxType.Aart);}
         set { SetText(BoxType.Aart, value); }
      }
      
      public bool IsCompilation
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxType.Cpil))
               return box.Data.ToUInt () != 0;
            
            return false;
         }
         set
         {
            SetData (BoxType.Cpil, ByteVector.FromUInt ((uint)(value ? 1 : 0)), (uint)AppleDataBox.FlagType.ForTempo);
         }
      }
      
      public override string [] Performers
      {
         get {return GetText (BoxType.Art);}
         set {SetText (BoxType.Art, value);}
      }
      
      public override string [] Composers
      {
         get {return GetText (BoxType.Wrt);}
         set {SetText (BoxType.Wrt, value);}
      }
      
      public override string Album
      {
         get
         {
            string [] text = GetText (BoxType.Alb);
            return text.Length == 0 ? null : text [0];
         }
         set {SetText (BoxType.Alb, value);}
      }
      
      public override string Comment
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxType.Cmt))
               return box.Text;
            return null;
         }
         set
         {
            SetText (BoxType.Cmt, value);
         }
      }
      
      public override string [] Genres
      {
         get
         {
            StringCollection l = new StringCollection ();
            ByteVectorCollection names = new ByteVectorCollection ();
            names.Add (BoxType.Gen);
            names.Add (BoxType.Gnre);
            foreach (AppleDataBox box in DataBoxes (names))
               if (box.Text != null)
                  l.Add (box.Text);
               else if (box.Flags == (int) AppleDataBox.FlagType.ContainsData)
               {
                  // iTunes stores genre's in the GNRE box as (ID3# + 1).
                  string genre = TagLib.Genres.IndexToAudio ((byte) (box.Data.ToUShort (true) - 1));
                  if (genre != null)
                     l.Add (genre);
               }
            return l.ToArray ();
         }
         set
         {
            ClearData (BoxType.Gnre);
            SetText (BoxType.Gen, value);
         }
      }
      
      public override uint Year
      {
         get
         {
            uint value;
            foreach (AppleDataBox box in DataBoxes (BoxType.Day))
               if (box.Text != null && uint.TryParse (box.Text.Length > 4 ? box.Text.Substring (0, 4) : box.Text, out value))
                  return value;
            
            return 0;
         }
         set
         {
            SetText (BoxType.Day, value.ToString (System.Globalization.CultureInfo.InvariantCulture));
         }
      }
      
      public override uint Track
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxType.Trkn))
               if (box.Flags == (int) AppleDataBox.FlagType.ContainsData && box.Data.Count >=4)
                  return box.Data.Mid (2, 2).ToUShort ();
            
            return 0;
         }
         set
         {
            ByteVector v = ByteVector.FromUShort (0);
            v.Add (ByteVector.FromUShort ((ushort) value));
            v.Add (ByteVector.FromUShort ((ushort) TrackCount));
            v.Add (ByteVector.FromUShort (0));
            
            SetData (BoxType.Trkn, v, (int) AppleDataBox.FlagType.ContainsData);
         }
      }
      
      public override uint TrackCount
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxType.Trkn))
               if (box.Flags == (int) AppleDataBox.FlagType.ContainsData && box.Data.Count >=6)
                  return box.Data.Mid (4, 2).ToUShort ();
            
            return 0;
         }
         set
         {
            ByteVector v = new ByteVector ();
            v += ByteVector.FromUShort (0);
            v += ByteVector.FromUShort ((ushort) Track);
            v += ByteVector.FromUShort ((ushort) value);
            v += ByteVector.FromUShort (0);
            
            SetData (BoxType.Trkn, v, (int) AppleDataBox.FlagType.ContainsData);
         }
      }
      
      public override uint Disc
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxType.Disk))
               if (box.Flags == (int) AppleDataBox.FlagType.ContainsData && box.Data.Count >=4)
                  return box.Data.Mid (2, 2).ToUShort ();
            
            return 0;
         }
         set
         {
            ByteVector v = new ByteVector ();
            v += ByteVector.FromUShort (0);
            v += ByteVector.FromUShort ((ushort) value);
            v += ByteVector.FromUShort ((ushort) DiscCount);
            v += ByteVector.FromUShort (0);
            
            SetData (BoxType.Disk, v, (int) AppleDataBox.FlagType.ContainsData);
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxType.Disk))
               if (box.Flags == (int) AppleDataBox.FlagType.ContainsData && box.Data.Count >=6)
                  return box.Data.Mid (4, 2).ToUShort ();
            
            return 0;
         }
         set
         {
            ByteVector v = new ByteVector ();
            v += ByteVector.FromUShort (0);
            v += ByteVector.FromUShort ((ushort) Disc);
            v += ByteVector.FromUShort ((ushort) value);
            v += ByteVector.FromUShort (0);
            
            SetData (BoxType.Disk, v, (int) AppleDataBox.FlagType.ContainsData);
         }
      }
      
      public override string Lyrics
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxType.Lyr))
               return box.Text;
            return null;
         }
         set
         {
            SetText (BoxType.Lyr, value);
         }
      }
      
      public override uint BeatsPerMinute
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxType.Tmpo))
               if (box.Flags == (uint)AppleDataBox.FlagType.ForTempo)
                  return box.Data.ToUInt ();
            
            return 0;
         }
         set
         {
            SetData (BoxType.Tmpo, ByteVector.FromUInt (value), (uint)AppleDataBox.FlagType.ForTempo);
         }
      }
      
      public override string Grouping
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes(BoxType.Grp))
               return box.Text;
            
            return null;
         }
         set
         {
            SetText(BoxType.Grp, value);
         }
      }
      
      public override string Conductor
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes(BoxType.Cond))
               return box.Text;
            return null;
         }
         set
         {
            SetText(BoxType.Cond, value);
         }
      }
      
      public override string Copyright
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes(BoxType.Cprt))
               return box.Text;
            return null;
         }
         set
         {
            SetText(BoxType.Cprt, value);
         }
      }
      
      public override IPicture [] Pictures
      {
         get
         {
         	List<Picture> l = new List<Picture> ();
         	
         	foreach (AppleDataBox box in  DataBoxes(BoxType.Covr))
         	{
         		string type = null;
         		string desc = null;
            	if (box.Flags == (int) AppleDataBox.FlagType.ContainsJpegData)
            	{
            		type = "image/jpeg";
            		desc = "cover.jpg";
            	}
            	else if (box.Flags == (int) AppleDataBox.FlagType.ContainsPngData)
            	{
            		type = "image/png";
            		desc = "cover.png";
            	}
            	else continue;
            	
            	Picture p = new Picture ();
            	p.Type = PictureType.FrontCover;
            	p.Data = box.Data;
            	p.MimeType = type;
            	p.Description = desc;
            	
            	l.Add (p);
            }
            
            return (Picture []) l.ToArray ();
         }
         
         set
         {
         	if (value == null || value.Length == 0)
         	{
         		ClearData (BoxType.Covr);
         		return;
         	}
         	
         	AppleDataBox [] boxes = new AppleDataBox [value.Length];
         	for (int i = 0; i < value.Length; i ++)
         	{
         		uint type = (uint) AppleDataBox.FlagType.ContainsData;
         		
            	if (value [i].MimeType == "image/jpeg")
            		type = (uint) AppleDataBox.FlagType.ContainsJpegData;
            	else if (value [i].MimeType == "image/png")
            		type = (uint) AppleDataBox.FlagType.ContainsPngData;
            	
            	boxes [i] = new AppleDataBox (value [i].Data, type);
         	}
         	
            SetData(BoxType.Covr, boxes);
         }
      }
      
      public override bool IsEmpty {
         get {
            return !ilst_box.HasChildren;
         }
      }

      
      public override void Clear ()
      {
         ilst_box.ClearChildren ();
      }

      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      internal static ReadOnlyByteVector FixId (ByteVector v)
      {
      	if (v.Count == 4)
         {
            if (v is ReadOnlyByteVector)
               return v as ReadOnlyByteVector;
      		return new ReadOnlyByteVector (v);
      	}
      	if (v.Count == 3)
            return new ReadOnlyByteVector (0xa9, v [0], v [1], v [2]);
         
         return null;
      }
   }
}
