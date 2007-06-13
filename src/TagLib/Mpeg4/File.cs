using System;

namespace TagLib.Mpeg4
{
   [SupportedMimeType("taglib/m4a", "m4a")]
   [SupportedMimeType("taglib/m4v", "m4v")]
   [SupportedMimeType("taglib/m4p", "m4p")]
   [SupportedMimeType("taglib/mp4", "mp4")]
   [SupportedMimeType("audio/mp4")]
   [SupportedMimeType("audio/x-m4a")]
   [SupportedMimeType("video/mp4")]
   [SupportedMimeType("video/x-m4v")]
   public class File : TagLib.File
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private AppleTag    apple_tag;
      private CombinedTag tag;
      private Properties  properties;
      private IsoUserDataBox udta_box;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public File (string path, ReadStyle propertiesStyle) : this (new File.LocalFileAbstraction (path), propertiesStyle)
      {}
      
      public File (string path) : this (path, ReadStyle.Average)
      {}
      
      public File (File.IFileAbstraction abstraction, ReadStyle propertiesStyle) : base (abstraction)
      {
         // TODO: Support Id3v2 boxes!!!
         tag = new CombinedTag ();
         
         Mode = AccessMode.Read;
         Read (propertiesStyle);
         Mode = AccessMode.Closed;
      }
      
      public File (File.IFileAbstraction abstraction) : this (abstraction, ReadStyle.Average)
      {}
      
      // Read the tag. If it doesn't exist, create.
      public override TagLib.Tag Tag {get {return tag;}}
      
      // Read the properties.
      public override TagLib.Properties Properties {get {return properties;}}
      
      // Save. This work is done in the AppleTag.
      public override void Save () 
      {
         if (udta_box == null)
            udta_box = new IsoUserDataBox ();
         
         // Try to get into write mode.
         Mode = File.AccessMode.Write;
         
         FileParser parser = new FileParser (this);
         parser.ParseBoxHeaders ();
         
         long size_change = 0;
         long write_position = 0;
         
         ByteVector tag_data = udta_box.Render ();
         
         // If we don't have a "udta" box to overwrite...
         if (parser.UdtaTree == null || parser.UdtaTree.Length == 0 || parser.UdtaTree [parser.UdtaTree.Length - 1].BoxType != BoxType.Udta)
         {
            // Stick the box at the end of the moov box.
            BoxHeader moov_header = parser.MoovTree [parser.MoovTree.Length - 1];
            size_change = tag_data.Count;
            write_position = moov_header.Position + moov_header.TotalBoxSize;
            Insert (tag_data, write_position, 0);
            
            // Overwrite the parent box sizes.
            for (int i = parser.MoovTree.Length - 1; i >= 0; i --)
               size_change = parser.MoovTree [i].Overwrite (this, size_change);
         }
         else
         {
            // Overwrite the old box.
            BoxHeader udta_header = parser.UdtaTree [parser.UdtaTree.Length - 1];
            size_change = tag_data.Count - udta_header.TotalBoxSize;
            write_position = udta_header.Position;
            Insert (tag_data, write_position, udta_header.TotalBoxSize);
            
            // Overwrite the parent box sizes.
            for (int i = parser.UdtaTree.Length - 2; i >= 0; i --)
               size_change = parser.UdtaTree [i].Overwrite (this, size_change);
         }
         
         // If we've had a size change, we may need to adjust chunk offsets.
         if (size_change != 0)
         {
            // We may have moved the offset boxes, so we need to reread.
            parser.ParseChunkOffsets ();
            
            foreach (Box box in parser.ChunkOffsetBoxes)
            {
               if (box is IsoChunkLargeOffsetBox)
                  (box as IsoChunkLargeOffsetBox).Overwrite (this, size_change, write_position);
               
               if (box is IsoChunkOffsetBox)
                  (box as IsoChunkOffsetBox).Overwrite (this, size_change, write_position);
            }
         }
         
         Mode = File.AccessMode.Closed;
         TagTypesOnDisk = TagTypes;
      }
      
      // Get the Apple Tag.
      public override TagLib.Tag GetTag (TagTypes type, bool create)
      {
         if (type == TagTypes.Apple)
         {
            if (apple_tag == null && create)
            {
               apple_tag = new AppleTag (udta_box);
               tag.SetTags (apple_tag);
            }
            
            return apple_tag;
         }
         
         return null;
      }
      
      // Remove the Apple Tag.
      public override void RemoveTags (TagTypes types)
      {
         if ((types & TagTypes.Apple) != TagTypes.Apple || apple_tag == null)
            return;
         
         apple_tag.DetachIlst ();
         apple_tag = null;
         tag.SetTags ();
      }
      
      // Read the file.
      private void Read (ReadStyle properties_style)
      {
         FileParser parser = new FileParser (this);
         
         if (properties_style == ReadStyle.None)
            parser.ParseTag ();
         else
            parser.ParseTagAndProperties ();
         
         udta_box = parser.UserDataBox;
         
         if (udta_box != null && udta_box.GetChild (BoxType.Meta) != null && udta_box.GetChild (BoxType.Meta).GetChild (BoxType.Ilst) != null)
            TagTypesOnDisk |= TagTypes.Apple;
         
         if (udta_box == null)
            udta_box = new IsoUserDataBox ();

         apple_tag = new AppleTag (udta_box);
         tag.SetTags (apple_tag);
         
         // If we're not reading properties, we're done.
         if (properties_style == ReadStyle.None)
            return;
         
         // Get the movie header box.
         IsoMovieHeaderBox mvhd_box = parser.MovieHeaderBox;
         if(mvhd_box == null)
            throw new CorruptFileException ("mvhd box not found.");
         
         IsoAudioSampleEntry  audio_sample_entry  = parser.AudioSampleEntry;
         IsoVisualSampleEntry visual_sample_entry = parser.VisualSampleEntry;
         
         // Read the properties.
         properties = new Properties (mvhd_box.Duration, audio_sample_entry, visual_sample_entry);
      }
   }
}
