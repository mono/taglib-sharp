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
   public enum BlockType
   {
      StreamInfo = 0,
      Padding,
      Application,
      SeekTable,
      VorbisComment,
      CueSheet,
      Picture
   }

   [SupportedMimeType("taglib/flac", "flac")]
   [SupportedMimeType("audio/x-flac")]
   [SupportedMimeType("application/x-flac")]
   [SupportedMimeType("audio/flac")]
   public class File : TagLib.File
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Id3v2.Tag          id3v2_tag;
      private Id3v1.Tag          id3v1_tag;
      private Ogg.XiphComment    comment;
      private PictureTag         picture_tag;
      private CombinedTag        tag;

      private Properties         properties;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public File (string file, Properties.ReadStyle properties_style) : base (file)
      {
         comment             = null;
         properties          = null;
         tag                 = new CombinedTag ();
         
         Mode = AccessMode.Read;
         Read (properties_style);
         Mode = AccessMode.Closed;
      }

      public File (string file) : this (file, Properties.ReadStyle.Average)
      {
      }

      public override void Save ()
      {
         Mode = AccessMode.Write;
         
         long tag_start, tag_end;
         
         // Update ID3v2 tag
         FindId3v2 (out tag_start, out tag_end);
         if(id3v2_tag != null)
         {
            ByteVector id3v2_tag_data = id3v2_tag.Render ();
            Insert (id3v2_tag_data, tag_start, tag_end);
            tag_end = tag_start + id3v2_tag_data.Count;
         }
         
         // Get all the blocks, but don't read the data for ones we're filling
         // with stored data.
         BlockType[] types = {BlockType.VorbisComment, BlockType.Picture};
         List<Block> old_blocks = ReadBlocks (tag_end, Length, types, BlockMode.Blacklist);
         
         // Find the range currently holding the blocks.
         tag_start = old_blocks [0].Position;
         tag_end   = old_blocks [old_blocks.Count - 1].NextBlockPosition;
         
         // Create new vorbis comments is they don't exist.
         GetTag (TagTypes.Xiph, true);
         
         // Create new blocks and add the basics.
         List<Block> new_blocks = new List<Block> ();
         new_blocks.Add (old_blocks [0]);
         new_blocks.Add (new Block (BlockType.VorbisComment, comment.Render (false)));
         
         foreach (IPicture picture in picture_tag.Pictures)
            new_blocks.Add (new Block (BlockType.Picture, Picture.Render (picture)));
         
         // Add all other blocks from the file.
         foreach (Block block in old_blocks)
            if (block.Type != BlockType.StreamInfo    &&
                block.Type != BlockType.VorbisComment &&
                block.Type != BlockType.Picture)
               new_blocks.Add (block);
         
         // Get the length of the blocks.
         long length = 0;
         foreach (Block block in new_blocks)
            length += block.TotalLength;
         
         // Find the padding size to avoid trouble. If that fails make some.
         long padding_size = tag_end - tag_start - BlockHeader.Length - length;
         if (padding_size < 0)
            padding_size = 1024 * 4;
         
         // Add a padding block.
         if (padding_size != 0)
         {
            new_blocks.Add (new Block (BlockType.Padding, new ByteVector ((int) length)));
         }
         
         // Render the blocks.
         ByteVector block_data = new ByteVector ();
         for (int i = 0; i < new_blocks.Count; i ++)
            block_data.Add (new_blocks [i].Render (i == new_blocks.Count - 1));
         
         // Update the blocks.
         Insert (block_data, tag_start, tag_end);
         
         // Update ID3v1 tag
         FindId3v1 (out tag_start, out tag_end);
         if(id3v1_tag != null)
         {
            Insert (id3v1_tag.Render (), tag_start, tag_end);
         }
         Mode = AccessMode.Closed;
      }
      
      public override TagLib.Tag GetTag (TagTypes type, bool create)
      {
         switch (type)
         {
            case TagTypes.Id3v1:
            {
               if (create && id3v1_tag == null)
               {
                  id3v1_tag = new Id3v1.Tag ();
                  
                  if (tag != null)
                     TagLib.Tag.Duplicate (tag, id3v1_tag, true);
                  
                  tag.SetTags (picture_tag, comment, id3v2_tag, id3v1_tag);
               }
               return id3v1_tag;
            }
            
            case TagTypes.Id3v2:
            {
               if (create && id3v2_tag == null)
               {
                  id3v2_tag = new Id3v2.Tag ();
                  
                  if (tag != null)
                     TagLib.Tag.Duplicate (tag, id3v2_tag, true);
                  
                  tag.SetTags (picture_tag, comment, id3v2_tag, id3v1_tag);
               }
               return id3v2_tag;
            }
            
            case TagTypes.Xiph:
            {
               if (create && comment == null)
               {
                  comment = new Ogg.XiphComment ();
                  
                  if (tag != null)
                     TagLib.Tag.Duplicate (tag, comment, true);
                  
                  tag.SetTags (picture_tag, comment, id3v2_tag, id3v1_tag);
               }
               return comment;
            }
            
            default:
               return null;
         }
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TagLib.Tag Tag {get {return tag;}}

      public override AudioProperties AudioProperties {get {return properties;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Read (Properties.ReadStyle properties_style)
      {
         long flac_data_begin, flac_data_end, dummy;
         
         
         id3v2_tag = ReadId3v2Tag (out dummy, out flac_data_begin);
         id3v1_tag = ReadId3v1Tag (out flac_data_end, out dummy);
         
         BlockType[] types = {BlockType.StreamInfo, BlockType.VorbisComment, BlockType.Picture};
         List<Block> blocks = ReadBlocks (flac_data_begin, flac_data_end, types, BlockMode.Whitelist);
         
         // Find the first vorbis comment inside the blocks.
         foreach (Block block in blocks)
            if (block.Type == BlockType.VorbisComment && block.Data.Count > 0)
            {
               comment = new Ogg.XiphComment (block.Data);
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
         
         // Set the tags in the CombinedTag.
         tag.SetTags (picture_tag, comment, id3v2_tag, id3v1_tag);
         
         // Make sure we have a Vorbis Comment.
         GetTag (TagTypes.Xiph, true);
         
         // The stream exists from the end of the last block to the end of the file.
         long stream_length = flac_data_end - blocks [blocks.Count - 1].NextBlockPosition;
         if(properties_style != Properties.ReadStyle.None)
            properties = new Properties (blocks [0].Data, stream_length, properties_style);
      }
      
      private Id3v2.Tag ReadId3v2Tag (out long start, out long end)
      {
         return FindId3v2 (out start, out end) ? new Id3v2.Tag (this, start) : null;
      }
      
      private Id3v1.Tag ReadId3v1Tag (out long start, out long end)
      {
         return FindId3v1 (out start, out end) ? new Id3v1.Tag (this, start) : null;
      }
      
      private enum BlockMode
      {
         Blacklist,
         Whitelist
      }
      
      private List<Block> ReadBlocks (long start, long end, BlockType[] types, BlockMode mode)
      {
         List<Block> blocks = new List<Block>();
         
         long block_position = Find ("fLaC", start);
         if (block_position < 0)
            throw new ArgumentException ("FLAC stream not found at starting position.", "start");
         
         block_position += 4;
         
         Block block;
         do
         {
            Seek (block_position);
            
            block = ReadMetadataBlock (types, mode);
            blocks.Add (block);
            
            if (block.NextBlockPosition > end)
               throw new Exception ("Next block position exceeds length of stream.");
            
            block_position = block.NextBlockPosition;
         }
         while (!block.IsLastBlock);

         // Check that the first block is a METADATA_BLOCK_STREAMINFO.
         if (blocks [0].Type != BlockType.StreamInfo)
            throw new Exception ("FLAC stream does not begin with StreamInfo.");
         
         return blocks;
      }
      
      private BlockHeader ReadMetadataBlockHeader ()
      {
         return new BlockHeader (ReadBlock (4));
      }
      
      private Block ReadMetadataBlock (BlockType[] types, BlockMode mode)
      {
         long position = Tell;
         BlockHeader header = ReadMetadataBlockHeader ();
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
      
      private bool FindId3v1 (out long start, out long end)
      {
         Seek (-128, System.IO.SeekOrigin.End);
         
         start = Tell;
         end = Length;
         
         if (ReadBlock (3) == Id3v1.Tag.FileIdentifier)
            return true;
         
         start = end;
         return false;
      }

      private bool FindId3v2 (out long start, out long end)
      {
         start = end = 0;
         
         Seek (0);
         ByteVector data = ReadBlock (10);
         
         if (data.Mid (0, 3) == Id3v2.Header.FileIdentifier)
         {
            Id3v2.Header header = new Id3v2.Header (data);
            if (header.TagSize != 0)
            {
               end = header.CompleteTagSize;
               return true;
            }
         }
         
         return false;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // BlockHeader class
      //////////////////////////////////////////////////////////////////////////
      private class BlockHeader
      {
         private BlockType block_type;
         private bool      is_last_block;
         private uint      block_length;
         
         public BlockHeader (ByteVector data)
         {
            block_type    = (BlockType) (data[0] & 0x7f);
            is_last_block = (data[0] & 0x80) != 0;
            block_length  = data.Mid (1,3).ToUInt ();
         }
         
         public BlockHeader (BlockType type, uint length)
         {
            block_type    = type;
            is_last_block = false;
            block_length  = length;
         }
         
         public ByteVector Render (bool is_last_block)
         {
            ByteVector data = ByteVector.FromUInt (block_length);
            data [0] = (byte)(block_type + (is_last_block ? 0x80 : 0));
            return data;
         }
         
         public static uint Length    {get {return 4;}}
         
         public BlockType BlockType   {get {return block_type;}}
         public bool      IsLastBlock {get {return is_last_block;}}
         public uint      BlockLength {get {return block_length;}}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // Block class
      //////////////////////////////////////////////////////////////////////////
      private class Block
      {
         private BlockHeader header;
         private ByteVector  data;
         private long        position;
         
         public Block (BlockHeader header, ByteVector data, long position)
         {
            this.header   = header;
            this.data     = data;
            this.position = position;
         }
         
         public Block (BlockType type, ByteVector data)
         {
            this.header   = new BlockHeader (type, (uint) data.Count);
            this.data     = data;
            this.position = 0;
         }
         
         public ByteVector Render (bool is_last_block)
         {
            ByteVector data = header.Render (is_last_block);
            data.Add (this.data);
            return data;
         }
         
         public BlockType   Type              {get {return header.BlockType;}}
         public bool        IsLastBlock       {get {return header.IsLastBlock;}}
         public uint        Length            {get {return header.BlockLength;}}
         public uint        TotalLength       {get {return Length + 4;}}
         public ByteVector  Data              {get {return data;}}
         public long        Position          {get {return position;}}
         public long        NextBlockPosition {get {return Position + BlockHeader.Length + header.BlockLength;}}
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
