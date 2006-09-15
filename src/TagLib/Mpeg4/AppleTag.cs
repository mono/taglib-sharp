using System;
using System.Collections;

namespace TagLib.Mpeg4
{
   public class AppleTag : TagLib.Tag, IEnumerable
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private AppleItemListBox ilst_box;
      private File file;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public AppleTag (AppleItemListBox box, File file) : base ()
      {
         // Hold onto the ilst_box and file. If the box doesn't exist, create
         // one.
         ilst_box = (box == null) ? new AppleItemListBox () : box;
         this.file = file;
      }
      
      public AppleTag (File file) : this (null, file)
      {
      }
      
      public IEnumerator GetEnumerator()
      {
         return ilst_box.Children.GetEnumerator();
      }
      
      // Get all the data boxes with the provided types.
      public AppleDataBox [] DataBoxes (ByteVectorList list)
      {
         ArrayList l = new ArrayList ();
         
         // Check each box to see if the match any of the provided types.
         // If a match is found, loop through the children and add any data box.
         foreach (Box box in ilst_box.Children)
            foreach (ByteVector v in list)
               if (FixId (v) == box.BoxType)
                  foreach (Box data_box in box.Children)
                     if (data_box.GetType () == typeof (AppleDataBox))
                        l.Add (data_box);
         
         // Return the results as an array.
         return (AppleDataBox []) l.ToArray (typeof (AppleDataBox));
      }
      
      // Get all the data boxes with a given type.
      public AppleDataBox [] DataBoxes (ByteVector v)
      {
         return DataBoxes (new ByteVectorList (FixId (v)));
      }
      
      // Find all the data boxes with a given mean and name.
      public AppleDataBox [] DataBoxes (string mean, string name)
      {
         ArrayList l = new ArrayList ();
         
         // These children will have a box type of "----"
         foreach (Box box in ilst_box.Children)
            if (box.BoxType == "----")
            {
               // Get the mean and name boxes, make sure they're legit, and make
               // sure that they match what we want. Then loop through and add
               // all the data box children to our output.
               AppleAdditionalInfoBox mean_box = (AppleAdditionalInfoBox) box.FindChild ("meta");
               AppleAdditionalInfoBox name_box = (AppleAdditionalInfoBox) box.FindChild ("name");
               if (mean_box != null && name_box != null && mean_box.Text == mean && name_box.Text == name)
                  foreach (Box data_box in box.Children)
                     if (data_box.GetType () == typeof (AppleDataBox))
                        l.Add (data_box);
            }
         
         // Return the results as an array.
         return (AppleDataBox []) l.ToArray (typeof (AppleDataBox));
      }
      
      // Get all the text data boxes from a given box type.
      public string [] GetText (ByteVector type)
      {
         StringList l = new StringList ();
         foreach (AppleDataBox box in DataBoxes (type))
            if (box.Text != null)
               l.Add (box.Text);
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
               // Otherwise, it is dead to us.
               else
                  box.RemoveFromParent ();
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
            ilst_box.RemoveChildren (FixId (type));
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
         if (text == null || text == "")
         {
            ilst_box.RemoveChildren (FixId (type));
            return;
         }
         
         SetText (type, new string [] {text});
      }
      
      // Clear all data associated with a box type.
      public void ClearData (ByteVector type)
      {
         ilst_box.RemoveChildren (FixId (type));
      }

      // Save the file.
      public void Save ()
      {
         if (ilst_box == null) {
            throw new NullReferenceException();
         }
         
         // Try to get into write mode.
         file.Mode = File.AccessMode.Write;
         
         // Make a file box.
         FileBox file_box = new FileBox (file);
         
         // Get the MovieBox.
         IsoMovieBox moov_box = (IsoMovieBox) file_box.FindChildDeep ("moov");
         
         // If we have a movie box...
         if (moov_box != null)
         {
            // Set up how much, where, and what to replace, and who to tell
            // about it.
            ulong      original_size = 0;
            long       position      = -1;
            ByteVector data          = null;
            Box        parent        = null;
            
            // Get the old ItemList (the one we're replacing.
            AppleItemListBox old_ilst_box = (AppleItemListBox) moov_box.FindChildDeep ("ilst");
            
            // If it exists.
            if (old_ilst_box != null)
            {
               // We stick ourself in the meta box and slate to overwrite it.
               parent = old_ilst_box.Parent;
               original_size = parent.BoxSize;
               position = parent.NextBoxPosition - (long) original_size;
               
               parent.ReplaceChild (old_ilst_box, ilst_box);
               data = parent.Render ();
               
               parent = parent.Parent;
            }
            else
            {
               // There is not old ItemList. See if we can get a MetaBox.
               IsoMetaBox meta_box = (IsoMetaBox) moov_box.FindChildDeep ("meta");
               
               //If we can...
               if (meta_box != null)
               {
                  // Stick the child in here and slate to overwrite...
                  meta_box.AddChild (ilst_box);
                  
                  original_size = meta_box.BoxSize;
                  position      = meta_box.NextBoxPosition - (long) original_size;
                  data          = meta_box.Render ();
                  parent        = meta_box.Parent;
               }
               else
               {
                  // There's no MetaBox. Create one and add the ItemList.
                  meta_box = new IsoMetaBox ("hdlr", null);
                  meta_box.AddChild (ilst_box);
                  
                  // See if we can get a UserDataBox.
                  IsoUserDataBox udta_box = (IsoUserDataBox) moov_box.FindChildDeep ("udta");
                  
                  // If we can...
                  if (udta_box != null)
                  {
                     // We'll stick the MetaBox at the end and overwrite it.
                     original_size = 0;
                     position = udta_box.NextBoxPosition;
                     data = meta_box.Render ();
                     parent = udta_box;
                  }
                  else
                  {
                     // If not even the UserDataBox exists, create it and add
                     // our MetaBox.
                     udta_box = new IsoUserDataBox ();
                     udta_box.AddChild (meta_box);
                     
                     // Since UserDataBox is a child of MovieBox, we'll just
                     // insert it at the end.
                     original_size = 0;
                     position = moov_box.NextBoxPosition;
                     data = udta_box.Render ();
                     parent = moov_box;
                  }
               }
            }
            
            // If we have data and somewhere to put it..
            if (data != null && position >= 0)
            {
               // Figure out the size difference.
               long size_difference = (long) data.Count - (long) original_size;
               
               // Insert the new data.
               file.Insert (data, position, (long) (original_size));
               
               // If there is a size difference, resize all parent headers.
               if (size_difference != 0)
               {
                  while (parent != null)
                  {
                     parent.OverwriteHeader (size_difference);
                     parent = parent.Parent;
                  }
                  
                  // ALSO, VERY IMPORTANTLY, YOU MUST UPDATE EVERY 'stco'.
                  
                  foreach (Box box in moov_box.Children)
                     if (box.BoxType == "trak")
                     {
                     	IsoChunkLargeOffsetBox co64_box = (IsoChunkLargeOffsetBox) box.FindChildDeep ("co64");
                        if (co64_box != null)
                           co64_box.UpdateOffset (size_difference);
                        
                     	IsoChunkOffsetBox stco_box = (IsoChunkOffsetBox) box.FindChildDeep ("stco");
                        if (stco_box != null)
                           stco_box.UpdateOffset ((int) size_difference);
                     }
               }
               
               // Be nice and close the stream.
               file.Mode = File.AccessMode.Closed;
               return;
            }
         }
         else
            throw new ApplicationException("stream does not have MOOV tag");
         
         // We're at the end. Close the stream and admit defeat.
         file.Mode = File.AccessMode.Closed;
         throw new ApplicationException("Could not complete AppleTag save");
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override string Title
      {
         get
         {
            string [] text = GetText (FixId ("nam"));
            return text.Length == 0 ? null : text [0];
         }
         set {SetText (FixId ("nam"), value);}
      }
      
      public override string [] Artists
      {
         get {return GetText (FixId ("ART"));}
         set {SetText (FixId ("ART"), value);}
      }
      
      // FIXME: If we can figure out the performers box, we'll migrate.
      public override string [] Performers
      {
         get {return GetText (FixId ("prf"));}
         set {SetText (FixId ("prf"), value);}
      }
      
      public override string [] Composers
      {
         get {return GetText (FixId ("wrt"));}
         set {SetText (FixId ("wrt"), value);}
      }
      
      public override string Album
      {
         get
         {
            string [] text = GetText (FixId ("alb"));
            return text.Length == 0 ? null : text [0];
         }
         set {SetText (FixId ("alb"), value);}
      }
      
      public override string Comment
      {
         get
         {
            AppleDataBox [] boxes = DataBoxes (FixId ("cmt"));
            return boxes.Length == 0 ? null : boxes [0].Text;
         }
         set
         {
            SetText (FixId ("cmt"), value);
         }
      }
      
      public override string [] Genres
      {
         get
         {
            StringList l = new StringList ();
            ByteVectorList names = new ByteVectorList ();
            names.Add (FixId ("gen"));
            names.Add (FixId ("gnre"));
            foreach (AppleDataBox box in DataBoxes (names))
               if (box.Text != null)
                  l.Add (box.Text);
               else if (box.Flags == (int) AppleDataBox.FlagTypes.ContainsData)
               {
                  string genre = Id3v1.GenreList.Genre (box.Data [0]);
                  if (genre != null)
                     l.Add (genre);
               }
            return l.ToArray ();
         }
         set
         {
            ClearData (FixId ("gnre"));
            SetText (FixId ("gen"), value);
         }
      }
      
      public override uint Year
      {
         get
         {
            AppleDataBox [] boxes = DataBoxes (FixId ("day"));
            try
            {
               return boxes.Length == 0 || boxes [0].Text == null ? 0 : System.UInt32.Parse (boxes [0].Text.Substring (0, boxes [0].Text.Length < 4 ? boxes [0].Text.Length : 4));
            }
            catch {return 0;}
         }
         set
         {
            SetText (FixId ("day"), value.ToString ());
         }
      }
      
      public override uint Track
      {
         get
         {
            AppleDataBox [] boxes = DataBoxes (FixId ("trkn"));
            if (boxes.Length != 0 && boxes [0].Flags == (int) AppleDataBox.FlagTypes.ContainsData && boxes [0].Data.Count >=4)
               return (uint) boxes [0].Data.Mid (2, 2).ToShort ();
            return 0;
         }
         set
         {
            ByteVector v = new ByteVector ();
            v += ByteVector.FromShort (0);
            v += ByteVector.FromShort ((short) value);
            v += ByteVector.FromShort ((short) TrackCount);
            v += ByteVector.FromShort (0);
            
            SetData (FixId ("trkn"), v, (int) AppleDataBox.FlagTypes.ContainsData);
         }
      }
      
      public override uint TrackCount
      {
         get
         {
            AppleDataBox [] boxes = DataBoxes (FixId ("trkn"));
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
            
            SetData (FixId ("trkn"), v, (int) AppleDataBox.FlagTypes.ContainsData);
         }
      }
      
      public override uint Disc
      {
         get
         {
            AppleDataBox [] boxes = DataBoxes (FixId ("disk"));
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
            
            SetData (FixId ("disk"), v, (int) AppleDataBox.FlagTypes.ContainsData);
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            AppleDataBox [] boxes = DataBoxes (FixId ("disk"));
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
            
            SetData (FixId ("disk"), v, (int) AppleDataBox.FlagTypes.ContainsData);
         }
      }
      
 
      public override IPicture [] Pictures
      {
         get
         {
         	ArrayList l = new ArrayList ();
         	
         	foreach (AppleDataBox box in  DataBoxes(FixId("covr")))
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
            
            return (Picture []) l.ToArray (typeof (Picture));
         }
         
         set
         {
         	if (value == null || value.Length == 0)
         	{
         		ClearData ("covr");
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
         	
            SetData("covr", boxes);
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
