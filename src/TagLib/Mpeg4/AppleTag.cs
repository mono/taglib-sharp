using System;
using System.Collections;
using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class AppleTag : TagLib.Tag, IEnumerable
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private IsoUserDataBox udta_box;
      private IsoMetaBox meta_box;
      private AppleItemListBox ilst_box;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public AppleTag (ref IsoUserDataBox box) : base ()
      {
         if (box == null)
            box = new IsoUserDataBox ();
         
         udta_box = box;
         
         meta_box = udta_box.Children.Get (BoxTypes.Meta) as IsoMetaBox;
         if (meta_box == null)
         {
            meta_box = new IsoMetaBox ("mdir", null);
            udta_box.Children.Add (meta_box);
         }
         
         ilst_box = meta_box.Children.Get (BoxTypes.Ilst) as AppleItemListBox;
         
         if (ilst_box == null)
         {
            ilst_box = new AppleItemListBox ();
            meta_box.Children.Add (ilst_box);
         }
      }
      
      public IEnumerator GetEnumerator()
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
            if (box.BoxType == "----")
            {
               // Get the mean and name boxes, make sure they're legit, and make
               // sure that they match what we want. Then loop through and add
               // all the data box children to our output.
               AppleAdditionalInfoBox mean_box = (AppleAdditionalInfoBox) box.Children.Get (BoxTypes.Mean);
               AppleAdditionalInfoBox name_box = (AppleAdditionalInfoBox) box.Children.Get (BoxTypes.Name);
               if (mean_box != null && name_box != null && mean_box.Text == mean && name_box.Text == name)
                  foreach (Box data_box in box.Children)
                     if (data_box is AppleDataBox)
                        yield return data_box as AppleDataBox;
            }
      }
      
      // Get all the text data boxes from a given box type.
      public string [] GetText (ByteVector type)
      {
         StringList l = new StringList ();
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
                  box.Children.Clear ();
                  foreach (AppleDataBox b in boxes)
                     box.Children.Add (b);
                  first = false;
               }
            }
         
         // If we didn't find the box..
         if (first == true)
         {
            // Add the box and try again.
            Box box = new AppleAnnotationBox (type);
            ilst_box.Children.Add (box);
            SetData (type, boxes);
         }
      }
      
      public void SetData (ByteVector type, ByteVectorList data, uint flags)
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
	         SetData (type, new ByteVectorList (data), flags);
      }
      
      // Set the data with the given box type, strings, and flags.
      public void SetText (ByteVector type, string [] text)
      {
         // Remove empty data and return.
         if (text == null)
         {
            ilst_box.Children.RemoveByType (FixId (type));
            return;
         }
         
         // Create a list...
         ByteVectorList l = new ByteVectorList ();
         
         // and populate it with the ByteVectorized strings.
         foreach (string value in text)
            l.Add (ByteVector.FromString (value, StringType.UTF8));
         
         // Send our final byte vectors to SetData
         SetData (type, l, (uint) AppleDataBox.FlagTypes.ContainsText);
      }
      
      // Set the data with the given box type, string, and flags.
      public void SetText (ByteVector type, string text)
      {
         // Remove empty data and return.
         if (text == null || text == string.Empty)
         {
            ilst_box.Children.RemoveByType (FixId (type));
            return;
         }
         
         SetText (type, new string [] {text});
      }
      
      // Clear all data associated with a box type.
      public void ClearData (ByteVector type)
      {
         ilst_box.Children.RemoveByType (FixId (type));
      }
      
      public void DetachIlst ()
      {
         meta_box.Children.Remove (ilst_box);
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override string Title
      {
         get
         {
            string [] text = GetText (BoxTypes.Nam);
            return text.Length == 0 ? null : text [0];
         }
         set {SetText (BoxTypes.Nam, value);}
      }
      
      public override string [] AlbumArtists
      {
         get { return GetText(BoxTypes.Aart); }
         set { SetText(BoxTypes.Aart, value); }
      }
      
      public bool IsCompilation
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxTypes.Cpil))
               return box.Data.ToUInt () != 0;
            
            return false;
         }
         set
         {
            SetData (BoxTypes.Cpil, ByteVector.FromUInt ((uint)(value ? 1 : 0)), (uint)AppleDataBox.FlagTypes.ForTempo);
         }
      }
      
      public override string [] Performers
      {
         get {return GetText (BoxTypes.Art);}
         set {SetText (BoxTypes.Art, value);}
      }
      
      public override string [] Composers
      {
         get {return GetText (BoxTypes.Wrt);}
         set {SetText (BoxTypes.Wrt, value);}
      }
      
      public override string Album
      {
         get
         {
            string [] text = GetText (BoxTypes.Alb);
            return text.Length == 0 ? null : text [0];
         }
         set {SetText (BoxTypes.Alb, value);}
      }
      
      public override string Comment
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxTypes.Cmt))
               return box.Text;
            return null;
         }
         set
         {
            SetText (BoxTypes.Cmt, value);
         }
      }
      
      public override string [] Genres
      {
         get
         {
            StringList l = new StringList ();
            ByteVectorList names = new ByteVectorList ();
            names.Add (BoxTypes.Gen);
            names.Add (BoxTypes.Gnre);
            foreach (AppleDataBox box in DataBoxes (names))
               if (box.Text != null)
                  l.Add (box.Text);
               else if (box.Flags == (int) AppleDataBox.FlagTypes.ContainsData)
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
            ClearData (BoxTypes.Gnre);
            SetText (BoxTypes.Gen, value);
         }
      }
      
      public override uint Year
      {
         get
         {
            uint value;
            foreach (AppleDataBox box in DataBoxes (BoxTypes.Day))
               if (box.Text != null && uint.TryParse (box.Text.Length > 4 ? box.Text.Substring (0, 4) : box.Text, out value))
                  return value;
            
            return 0;
         }
         set
         {
            SetText (BoxTypes.Day, value.ToString ());
         }
      }
      
      public override uint Track
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxTypes.Trkn))
               if (box.Flags == (int) AppleDataBox.FlagTypes.ContainsData && box.Data.Count >=4)
                  return box.Data.Mid (2, 2).ToUShort ();
            
            return 0;
         }
         set
         {
            ByteVector v = ByteVector.FromUShort (0);
            v.Add (ByteVector.FromUShort ((ushort) value));
            v.Add (ByteVector.FromUShort ((ushort) TrackCount));
            v.Add (ByteVector.FromUShort (0));
            
            SetData (BoxTypes.Trkn, v, (int) AppleDataBox.FlagTypes.ContainsData);
         }
      }
      
      public override uint TrackCount
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxTypes.Trkn))
               if (box.Flags == (int) AppleDataBox.FlagTypes.ContainsData && box.Data.Count >=6)
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
            
            SetData (BoxTypes.Trkn, v, (int) AppleDataBox.FlagTypes.ContainsData);
         }
      }
      
      public override uint Disc
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxTypes.Disk))
               if (box.Flags == (int) AppleDataBox.FlagTypes.ContainsData && box.Data.Count >=4)
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
            
            SetData (BoxTypes.Disk, v, (int) AppleDataBox.FlagTypes.ContainsData);
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxTypes.Disk))
               if (box.Flags == (int) AppleDataBox.FlagTypes.ContainsData && box.Data.Count >=6)
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
            
            SetData (BoxTypes.Disk, v, (int) AppleDataBox.FlagTypes.ContainsData);
         }
      }
      
      public override string Lyrics
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxTypes.Lyr))
               return box.Text;
            return null;
         }
         set
         {
            SetText (BoxTypes.Lyr, value);
         }
      }
      
      public override uint BeatsPerMinute
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes (BoxTypes.Tmpo))
               if (box.Flags == (uint)AppleDataBox.FlagTypes.ForTempo)
                  return box.Data.ToUInt ();
            
            return 0;
         }
         set
         {
            SetData (BoxTypes.Tmpo, ByteVector.FromUInt (value), (uint)AppleDataBox.FlagTypes.ForTempo);
         }
      }
      
      public override string Grouping
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes(BoxTypes.Grp))
               return box.Text;
            
            return null;
         }
         set
         {
            SetText(BoxTypes.Grp, value);
         }
      }
      
      public override string Conductor
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes(BoxTypes.Cond))
               return box.Text;
            return null;
         }
         set
         {
            SetText(BoxTypes.Cond, value);
         }
      }
      
      public override string Copyright
      {
         get
         {
            foreach (AppleDataBox box in DataBoxes(BoxTypes.Cprt))
               return box.Text;
            return null;
         }
         set
         {
            SetText(BoxTypes.Cprt, value);
         }
      }
      
      public override IPicture [] Pictures
      {
         get
         {
         	List<Picture> l = new List<Picture> ();
         	
         	foreach (AppleDataBox box in  DataBoxes(BoxTypes.Covr))
         	{
         		string type = null;
         		string desc = null;
            	if (box.Flags == (int) AppleDataBox.FlagTypes.ContainsJpegData)
            	{
            		type = "image/jpeg";
            		desc = "cover.jpg";
            	}
            	else if (box.Flags == (int) AppleDataBox.FlagTypes.ContainsPngData)
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
         		ClearData (BoxTypes.Covr);
         		return;
         	}
         	
         	AppleDataBox [] boxes = new AppleDataBox [value.Length];
         	for (int i = 0; i < value.Length; i ++)
         	{
         		uint type = (uint) AppleDataBox.FlagTypes.ContainsData;
         		
            	if (value [i].MimeType == "image/jpeg")
            		type = (uint) AppleDataBox.FlagTypes.ContainsJpegData;
            	else if (value [i].MimeType == "image/png")
            		type = (uint) AppleDataBox.FlagTypes.ContainsPngData;
            	
            	boxes [i] = new AppleDataBox (value [i].Data, type);
         	}
         	
            SetData(BoxTypes.Covr, boxes);
         }
      }
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private ByteVector FixId (ByteVector v)
      {
         // IF we have a three byte type (like "wrt"), add the extra byte.
         if (v.Count == 3)
            v.Insert (0, 0xa9);
         return v;
      }
   }
}
