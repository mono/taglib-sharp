using System;

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
         
         Mode = AccessMode.Read;
         Read (properties_style);
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
         tag.Save (this);
      }
      
      // Get the Apple Tag.
      public override TagLib.Tag GetTag (TagTypes type, bool create)
      {
         if (type == TagTypes.Apple)
         {
            if (tag == null && create)
               tag = new AppleTag (null);
            
            return tag;
         }
         
         return null;
      }
      
      // Read the file.
      private void Read (AudioProperties.ReadStyle properties_style)
      {
         FileParser parser = new FileParser (this);
         
         if (properties_style == Properties.ReadStyle.None)
            parser.ParseTag ();
         else
            parser.ParseTagAndProperties ();
         
         tag = new AppleTag (parser.UserDataBox);
         
         // If we're not reading properties, we're done.
         if (properties_style == Properties.ReadStyle.None)
            return;
         
         // Get the movie header box.
         IsoMovieHeaderBox mvhd_box = parser.MovieHeaderBox;
         if(mvhd_box == null)
            throw new CorruptFileException ("mvhd box not found.");
         
         IsoAudioSampleEntry sample_entry = parser.AudioSampleEntry;
         
         // Read the properties.
         properties = new Properties (mvhd_box, sample_entry, properties_style);
      }
   }
}
