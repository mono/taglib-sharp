namespace TagLib.Mpeg4
{
   [SupportedMimeType("taglib/m4a", "m4a")]
   [SupportedMimeType("taglib/m4p", "m4p")]
   [SupportedMimeType("taglib/mp4", "mp4")]
   [SupportedMimeType("audio/mp4")]
   [SupportedMimeType("audio/x-m4a")]
   public class File : TagLib.File
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private AppleTag   tag;
      private Properties properties;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public File (string file, AudioProperties.ReadStyle properties_style) : base (file)
      {
         // Nullify for safety.
         tag = null;
         properties = null;
         
         // Try to open read support.
         try {Mode = AccessMode.Read;}
         catch {return;}
         
         // Read
         Read (properties_style);
         
         // Be nice and close.
         Mode = AccessMode.Closed;
      }
      
      // Assume average speed.
      public File (string file) : this (file, AudioProperties.ReadStyle.Average)
      {
      }
      
      // Read the tag. If it doesn't exist, create.
      public override TagLib.Tag Tag {get {return GetTag (TagTypes.Apple, true);}}
      
      // Read the properties.
      public override TagLib.AudioProperties AudioProperties {get {return properties;}}
      
      // Save. This work is done in the AppleTag.
      public override void Save () 
      {
        tag.Save();
      }
      
      // Get the Apple Tag.
      public override TagLib.Tag GetTag (TagTypes type, bool create)
      {
         if (type == TagTypes.Apple)
         {
            if (tag == null && create)
               tag = new AppleTag (this);
            
            return tag;
         }
         
         return null;
      }
      
      // Read the file.
      private void Read (AudioProperties.ReadStyle properties_style)
      {
         // Create a dummie outer box, as perscribed by the specs.
         FileBox file_box = new FileBox (this);
         
         // Find the movie box and item list. If the movie box doen't exist, an
         // exception will be thrown on the next call, but if there is no movie 
         // box, the file can't possibly be valid.
         IsoMovieBox moov_box = (IsoMovieBox) file_box.FindChildDeep ("moov");
         AppleItemListBox ilst_box = (AppleItemListBox) moov_box.FindChildDeep ("ilst");
         
         // If we have a ItemListBox, deparent it.
         if (ilst_box != null)
            ilst_box.RemoveFromParent ();
         
         // Create the tag.
         tag = new AppleTag (ilst_box, this);
         
         // If we're not reading properties, we're done.
         if(properties_style == Properties.ReadStyle.None)
            return;
         
         // Get the movie header box.
         IsoMovieHeaderBox   mvhd_box = (IsoMovieHeaderBox) moov_box.FindChildDeep ("mvhd");
         IsoAudioSampleEntry sample_entry = null;
         
         // Find a TrackBox with a sound Handler.
         foreach (Box box in moov_box.Children)
            if (box.BoxType == "trak")
            {
               // If the handler isn't sound, it could be metadata or video or
               // any number of other things.
               IsoHandlerBox hdlr_box = (IsoHandlerBox) box.FindChildDeep ("hdlr");
               if (hdlr_box == null || hdlr_box.HandlerType != "soun")
                  continue;
               
               // This track SHOULD contain at least one sample entry.
               sample_entry = (IsoAudioSampleEntry) box.FindChildDeep (typeof (IsoAudioSampleEntry));
               break;
            }
         
         // If we have a MovieHeaderBox, deparent it.
         if (mvhd_box != null)
            mvhd_box.RemoveFromParent ();
         
         // If we have a SampleEntry, deparent it.
         if (sample_entry != null)
            sample_entry.RemoveFromParent ();
         
         // Read the properties.
         properties = new Properties (mvhd_box, sample_entry, properties_style);
      }
      
      /*
      private void ShowAllTags (Box box, string spa)
      {
         if (box.GetType () == typeof (AppleDataBox))
            System.Console.WriteLine (spa + box.BoxType.Mid (box.BoxType [0] == 0xa9 ? 1 : 0).ToString () + " - " + box.ToString () + " - " + ((AppleDataBox)box).Text);
         else if (box.GetType () == typeof (AppleAdditionalInfoBox))
            System.Console.WriteLine (spa + box.BoxType.Mid (box.BoxType [0] == 0xa9 ? 1 : 0).ToString () + " - " + box.ToString () + " - " + ((AppleAdditionalInfoBox)box).Text);
         else
            System.Console.WriteLine (spa + box.BoxType.Mid (box.BoxType [0] == 0xa9 ? 1 : 0).ToString () + " - " + box.ToString ());
         
         foreach (Box child in box.Children)
         {
            ShowAllTags (child, spa + " ");
         }
      }
      */
   }
}
