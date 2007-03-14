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
      private AppleItemListBox ilst_box;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public AppleTag (IsoUserDataBox box) : base ()
      {
         udta_box = box;
         
         if (udta_box == null)
            udta_box = new IsoUserDataBox ();
         
         IsoMetaBox meta_box = udta_box.Children.Get (BoxTypes.Meta) as IsoMetaBox;
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
      public AppleDataBox [] DataBoxes (ByteVectorList list)
      {
         List<AppleDataBox> l = new List<AppleDataBox> ();
         
         // Check each box to see if the match any of the provided types.
         // If a match is found, loop through the children and add any data box.
         foreach (Box box in ilst_box.Children)
            foreach (ByteVector v in list)
               if (FixId (v) == box.BoxType)
                  foreach (Box data_box in box.Children)
                     if (data_box.GetType () == typeof (AppleDataBox))
                        l.Add ((AppleDataBox)data_box);
         
         // Return the results as an array.
         return (AppleDataBox []) l.ToArray ();
      }
      
      // Get all the data boxes with a given type.
      public AppleDataBox [] DataBoxes (ByteVector v)
      {
         return DataBoxes (new ByteVectorList (FixId (v)));
      }
      
      // Find all the data boxes with a given mean and name.
      public AppleDataBox [] DataBoxes (string mean, string name)
      {
         List<AppleDataBox> l = new List<AppleDataBox> ();
         
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
                     if (data_box.GetType () == typeof (AppleDataBox))
                        l.Add ((AppleDataBox)data_box);
            }
         
         // Return the results as an array.
         return (AppleDataBox []) l.ToArray ();
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

      // Save the file.
      public void Save (File file)
      {
         if (udta_box == null)
            throw new NullReferenceException();
         
         // Try to get into write mode.
         file.Mode = File.AccessMode.Write;
         
         FileParser parser = new FileParser (file);
         parser.ParseBoxHeaders ();
         
         long size_change = 0;
         long write_position = 0;
         
         ByteVector tag_data = udta_box.Render ();
         
         // If we don't have a "udta" box to overwrite...
         if (parser.UdtaTree.Length == 0 || parser.UdtaTree [parser.UdtaTree.Length - 1].BoxType != BoxTypes.Udta)
         {
            // Stick the box at the end of the moov box.
            BoxHeader moov_header = parser.MoovTree [parser.MoovTree.Length - 1];
            size_change = tag_data.Count;
            write_position = 0;
            file.Insert (tag_data, moov_header.Position + moov_header.TotalBoxSize, 0);
            
            // Overwrite the parent box sizes.
            for (int i = parser.MoovTree.Length - 1; i >= 0; i --)
               size_change = parser.MoovTree [i].Overwrite (file, size_change);
         }
         else
         {
            // Overwrite the old box.
            BoxHeader udta_header = parser.UdtaTree [parser.UdtaTree.Length - 1];
            size_change = tag_data.Count - udta_header.TotalBoxSize;
            write_position = udta_header.Position;
            file.Insert (tag_data, udta_header.Position, udta_header.TotalBoxSize);
            
            // Overwrite the parent box sizes.
            for (int i = parser.UdtaTree.Length - 2; i >= 0; i --)
               size_change = parser.UdtaTree [i].Overwrite (file, size_change);
         }
         
         // If we've had a size change, we may need to adjust chunk offsets.
         if (size_change != 0)
         {
            // We may have moved the offset boxes, so we need to reread.
            parser.ParseChunkOffsets ();
            
            foreach (Box box in parser.ChunkOffsetBoxes)
            {
               if (box is IsoChunkLargeOffsetBox)
                  (box as IsoChunkLargeOffsetBox).Overwrite (file, size_change, write_position);
               
               if (box is IsoChunkOffsetBox)
                  (box as IsoChunkOffsetBox).Overwrite (file, size_change, write_position);
            }
         }
         
         file.Mode = File.AccessMode.Closed;
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
         get {return IsCompilation ? new string [] {"Various Artists"} : Performers;}
         set
         {
            if (value.Length == 1 && value [0].ToLower () == "various artists")
               IsCompilation = true;
            else
            {
               IsCompilation = false;
               Performers = value;
            }
         }
      }
      
      public bool IsCompilation
      {
         get
         {
            AppleDataBox [] boxes = DataBoxes (BoxTypes.Cpil);
            return boxes.Length != 0 && boxes [0].Data.ToUInt () != 0;
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
            AppleDataBox [] boxes = DataBoxes (BoxTypes.Cmt);
            return boxes.Length == 0 ? null : boxes [0].Text;
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
                  string genre = Id3v1.GenreList.Genre ((byte) (box.Data.ToShort (true) - 1));
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
            AppleDataBox [] boxes = DataBoxes (BoxTypes.Day);
            try
            {
               return boxes.Length == 0 || boxes [0].Text == null ? 0 : System.UInt32.Parse (boxes [0].Text.Substring (0, boxes [0].Text.Length < 4 ? boxes [0].Text.Length : 4));
            }
            catch {return 0;}
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
            AppleDataBox [] boxes = DataBoxes (BoxTypes.Trkn);
            if (boxes.Length != 0 && boxes [0].Flags == (int) AppleDataBox.FlagTypes.ContainsData && boxes [0].Data.Count >=4)
               return (uint) boxes [0].Data.Mid (2, 2).ToShort ();
            return 0;
         }
         set
         {
            ByteVector v = ByteVector.FromShort (0);
            v.Add (ByteVector.FromShort ((short) value));
            v.Add (ByteVector.FromShort ((short) TrackCount));
            v.Add (ByteVector.FromShort (0));
            
            SetData (BoxTypes.Trkn, v, (int) AppleDataBox.FlagTypes.ContainsData);
         }
      }
      
      public override uint TrackCount
      {
         get
         {
            AppleDataBox [] boxes = DataBoxes (BoxTypes.Trkn);
            if (boxes.Length != 0 && boxes [0].Flags == (int) AppleDataBox.FlagTypes.ContainsData && boxes [0].Data.Count >=6)
               return (uint) boxes [0].Data.Mid (4, 2).ToShort ();
            return 0;
         }
         set
         {
            ByteVector v = new ByteVector ();
            v += ByteVector.FromShort (0);
            v += ByteVector.FromShort ((short) Track);
            v += ByteVector.FromShort ((short) value);
            v += ByteVector.FromShort (0);
            
            SetData (BoxTypes.Trkn, v, (int) AppleDataBox.FlagTypes.ContainsData);
         }
      }
      
      public override uint Disc
      {
         get
         {
            AppleDataBox [] boxes = DataBoxes (BoxTypes.Disk);
            if (boxes.Length != 0 && boxes [0].Flags == (int) AppleDataBox.FlagTypes.ContainsData && boxes [0].Data.Count >=4)
               return (uint) boxes [0].Data.Mid (2, 2).ToShort ();
            return 0;
         }
         set
         {
            ByteVector v = new ByteVector ();
            v += ByteVector.FromShort (0);
            v += ByteVector.FromShort ((short) value);
            v += ByteVector.FromShort ((short) DiscCount);
            v += ByteVector.FromShort (0);
            
            SetData (BoxTypes.Disk, v, (int) AppleDataBox.FlagTypes.ContainsData);
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            AppleDataBox [] boxes = DataBoxes (BoxTypes.Disk);
            if (boxes.Length != 0 && boxes [0].Flags == (int) AppleDataBox.FlagTypes.ContainsData && boxes [0].Data.Count >=6)
               return (uint) boxes [0].Data.Mid (4, 2).ToShort ();
            return 0;
         }
         set
         {
            ByteVector v = new ByteVector ();
            v += ByteVector.FromShort (0);
            v += ByteVector.FromShort ((short) Disc);
            v += ByteVector.FromShort ((short) value);
            v += ByteVector.FromShort (0);
            
            SetData (BoxTypes.Disk, v, (int) AppleDataBox.FlagTypes.ContainsData);
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
