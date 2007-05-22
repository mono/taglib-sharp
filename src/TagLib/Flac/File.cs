/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : flacfile.cpp from TagLib
 ***************************************************************************/

/***************************************************************************
 *   This library is free software; you can redistribute it and/or modify  *
 *   it  under the terms of the GNU Lesser General Public License version  *
 *   2.1 as published by the Free Software Foundation.                     *
 *                                                                         *
 *   This library is distributed in the hope that it will be useful, but   *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   Lesser General Public License for more details.                       *
 *                                                                         *
 *   You should have received a copy of the GNU Lesser General Public      *
 *   License along with this library; if not, write to the Free Software   *
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 *   USA                                                                   *
 ***************************************************************************/

using System.Collections.Generic;
using System;

namespace TagLib.Flac
{
   [SupportedMimeType("taglib/flac", "flac")]
   [SupportedMimeType("audio/x-flac")]
   [SupportedMimeType("application/x-flac")]
   [SupportedMimeType("audio/flac")]
   public class File : TagLib.NonContainer.File
   {
#region Properties
      private Ogg.XiphComment comment      = null;
      private PictureTag      picture_tag  = null;
      private CombinedTag     tag          = null;
      private ByteVector      header_block = null;
      private long            stream_start = 0;
#endregion Properties
      
#region Constructors
      public File (string file, ReadStyle properties_style) : base (file, properties_style)
      {
      }
      
      public File (string file) : this (file, ReadStyle.Average)
      {
      }
#endregion
      
      public override void Save ()
      {
         Mode = AccessMode.Write;
         
         // Update the tags at the beginning of the file.
         long start = StartTag.Write ();
         
         // Get all the blocks, but don't read the data for ones we're filling
         // with stored data.
         List<Block> old_blocks = ReadBlocks (start, BlockMode.Blacklist, BlockType.VorbisComment, BlockType.Picture);
         
         // Find the range currently holding the blocks.
         long metadata_start = old_blocks [0].Position;
         long metadata_end   = old_blocks [old_blocks.Count - 1].NextBlockPosition;
         
         // Create new vorbis comments is they don't exist.
         GetTag (TagTypes.Xiph, true);
         
         // Create new blocks and add the basics.
         List<Block> new_blocks = new List<Block> ();
         new_blocks.Add (old_blocks [0]);
         
         // Add blocks we don't deal with from the file.
         foreach (Block block in old_blocks)
            if (block.Type != BlockType.StreamInfo    &&
                block.Type != BlockType.VorbisComment &&
                block.Type != BlockType.Picture       &&
                block.Type != BlockType.Padding)
               new_blocks.Add (block);
         
         new_blocks.Add (new Block (BlockType.VorbisComment, comment.Render (false)));
         
         foreach (IPicture picture in picture_tag.Pictures)
            new_blocks.Add (new Block (BlockType.Picture, Picture.Render (picture)));
         
         // Get the length of the blocks.
         long length = 0;
         foreach (Block block in new_blocks)
            length += block.TotalLength;
         
         // Find the padding size to avoid trouble. If that fails make some.
         long padding_size = metadata_end - metadata_start - BlockHeader.Length - length;
         if (padding_size < 0)
            padding_size = 1024 * 4;
         
         // Add a padding block.
         if (padding_size != 0)
            new_blocks.Add (new Block (BlockType.Padding, new ByteVector ((int) padding_size)));
         
         // Render the blocks.
         ByteVector block_data = new ByteVector ();
         for (int i = 0; i < new_blocks.Count; i ++)
            block_data.Add (new_blocks [i].Render (i == new_blocks.Count - 1));
         
         // Update the blocks.
         Insert (block_data, metadata_start, metadata_end - metadata_start);
         
         // Update the tags at the end of the file.
         EndTag.Write ();
         Mode = AccessMode.Closed;
         TagTypesOnDisk = TagTypes;
      }
      
      public override TagLib.Tag GetTag (TagTypes type, bool create)
      {
         Tag t = (type == TagTypes.Xiph) ? comment : (Tag as TagLib.NonContainer.Tag).GetTag (type);
         
         if (t != null || !create)
            return t;
         
         switch (type)
         {
         case TagTypes.Id3v1:
            return EndTag.AddTag (type, Tag);
         
         case TagTypes.Id3v2:
            return StartTag.AddTag (type, Tag);
         
         case TagTypes.Ape:
            return EndTag.AddTag (type, Tag);
            
         case TagTypes.Xiph:
            comment = new Ogg.XiphComment ();
            TagLib.Tag.Duplicate (tag, comment, true);
            tag.SetTags (picture_tag, comment, base.Tag);
            return comment;
         
         default:
            return null;
         }
      }
      
      public override TagTypes TagTypes
      {
         get{
            TagTypes types = base.TagTypes;
            if (comment != null && !comment.IsEmpty)
               types |= TagTypes.Xiph;
            return types;
         }
      }

      public override TagLib.Tag Tag {get {return tag;}}
      
      protected override void ReadStart (long start, ReadStyle style)
      {
         List<Block> blocks = ReadBlocks (start, BlockMode.Whitelist,
            BlockType.StreamInfo, BlockType.VorbisComment, BlockType.Picture);
         
         // Find the first vorbis comment inside the blocks.
         foreach (Block block in blocks)
            if (block.Type == BlockType.VorbisComment && block.Data.Count > 0)
            {
               comment = new Ogg.XiphComment (block.Data);
               if (!comment.IsEmpty)
                  TagTypesOnDisk |= TagTypes.Xiph;
               break;
            }
         
         // Find the images.
         List<IPicture> pictures = new List<IPicture>();
         foreach (Block block in blocks)
            if (block.Type == BlockType.Picture && block.Data.Count > 0)
            {
               try
               {
                  pictures.Add (new Picture (block.Data));
               } catch {}
            }
         
         picture_tag = new PictureTag (pictures.ToArray ());
         
         if (style != ReadStyle.None)
         {
            // The stream exists from the end of the last block to the end of the file.
            stream_start = blocks [blocks.Count - 1].NextBlockPosition;
            header_block = blocks [0].Data;
         }
      }
      
      protected override void ReadEnd (long end, ReadStyle style)
      {
         tag = new CombinedTag (picture_tag, comment, base.Tag);
         
         // Make sure we have a Vorbis Comment.
         GetTag (TagTypes.Xiph, true);
      }
      
      protected override TagLib.Properties ReadProperties (long start, long end, ReadStyle style)
      {
         StreamHeader header = new StreamHeader (header_block, end - stream_start);
         return new Properties (TimeSpan.Zero, header);
      }
      

      private enum BlockMode
      {
         Blacklist,
         Whitelist
      }
      
      private List<Block> ReadBlocks (long start, BlockMode mode, params BlockType[] types)
      {
         List<Block> blocks = new List<Block>();
         
         long block_position = Find ("fLaC", start);
         if (block_position < 0)
            throw new CorruptFileException ("FLAC stream not found at starting position.");
         
         block_position += 4;
         
         Block block;
         do
         {
            Seek (block_position);
            
            block = ReadMetadataBlock (types, mode);
            blocks.Add (block);
            
            if (block.NextBlockPosition > Length)
               throw new CorruptFileException ("Next block position exceeds length of stream.");
            
            block_position = block.NextBlockPosition;
         }
         while (!block.IsLastBlock);

         // Check that the first block is a METADATA_BLOCK_STREAMINFO.
         if (blocks [0].Type != BlockType.StreamInfo)
            throw new CorruptFileException ("FLAC stream does not begin with StreamInfo.");
         
         return blocks;
      }
      
      private Block ReadMetadataBlock (BlockType[] types, BlockMode mode)
      {
         long position = Tell;
         BlockHeader header = new BlockHeader (ReadBlock (4));
         ByteVector data = null;
         
         if (types != null)
         {
            bool found = false;
            foreach (BlockType type in types)
               if (header.BlockType == type)
                  found = true;
            
            if ((mode == BlockMode.Whitelist &&  found) ||
                (mode == BlockMode.Blacklist && !found))
               data = ReadBlock ((int) header.BlockLength);
         }
         
         return new Block (header, data, position);
      }
      
      //////////////////////////////////////////////////////////////////////////
      // PictureTag class
      //////////////////////////////////////////////////////////////////////////
      private class PictureTag : Tag
      {
         private IPicture[] pictures;
         public PictureTag (IPicture[] pictures)
         {
            this.pictures = pictures;
         }
         
         public override IPicture[] Pictures
         {
            get {return pictures != null ? pictures : new IPicture [] {};}
            set {pictures = value;}
         }
      }
   }
}
