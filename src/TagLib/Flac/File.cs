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
      private Metadata        metadata     = null;
      private CombinedTag     tag          = null;
      private ByteVector      header_block = null;
      private long            stream_start = 0;
#endregion Properties
      
      #region Constructors
      public File (string path, ReadStyle propertiesStyle) : base (path, propertiesStyle)
      {}
      
      public File (string path) : base (path)
      {}
      
      public File (File.IFileAbstraction abstraction, ReadStyle propertiesStyle) : base (abstraction, propertiesStyle)
      {}
      
      public File (File.IFileAbstraction abstraction) : base (abstraction)
      {}
      #endregion
      
      public override void Save ()
      {
         Mode = AccessMode.Write;
         
         // Update the tags at the beginning of the file.
         long metadata_start = StartTag.Write ();
         long metadata_end;
         
         // Get all the blocks, but don't read the data for ones we're filling
         // with stored data.
         List<Block> old_blocks = ReadBlocks (ref metadata_start, out metadata_end, BlockMode.Blacklist, BlockType.XiphComment, BlockType.Picture);
         
         // Create new vorbis comments is they don't exist.
         GetTag (TagTypes.Xiph, true);
         
         // Create new blocks and add the basics.
         List<Block> new_blocks = new List<Block> ();
         new_blocks.Add (old_blocks [0]);
         
         // Add blocks we don't deal with from the file.
         foreach (Block block in old_blocks)
            if (block.Type != BlockType.StreamInfo  &&
                block.Type != BlockType.XiphComment &&
                block.Type != BlockType.Picture     &&
                block.Type != BlockType.Padding)
               new_blocks.Add (block);
         
         new_blocks.Add (new Block (BlockType.XiphComment, (GetTag (TagTypes.Xiph, true) as Ogg.XiphComment).Render (false)));
         
         foreach (IPicture picture in metadata.Pictures)
            if (picture != null)
               new_blocks.Add (new Block (BlockType.Picture, new Picture (picture).Render ()));
         
         // Get the length of the blocks.
         long length = 0;
         foreach (Block block in new_blocks)
            length += block.TotalSize;
         
         // Find the padding size to avoid trouble. If that fails make some.
         long padding_size = metadata_end - metadata_start - BlockHeader.Size - length;
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
         switch (type)
         {
         case TagTypes.Xiph:
            return metadata.GetComment (create, tag);
         case TagTypes.FlacMetadata:
            return metadata;
         }
         
            
            
         Tag t = (Tag as TagLib.NonContainer.Tag).GetTag (type);
         
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
            
         default:
            return null;
         }
      }
      
      public override TagLib.Tag Tag {get {return tag;}}
      
      public override void RemoveTags (TagTypes types)
      {
         if ((types & TagTypes.Xiph) != 0)
            metadata.RemoveComment ();
         
         if ((types & TagTypes.FlacMetadata) != 0)
            metadata.Clear ();
         
         base.RemoveTags (types);
      }
      
      protected override void ReadStart (long start, ReadStyle propertiesStyle)
      {
         long end;
         List<Block> blocks = ReadBlocks (ref start, out end, BlockMode.Whitelist, BlockType.StreamInfo, BlockType.XiphComment, BlockType.Picture);
         
         metadata = new Metadata (blocks);
         TagTypesOnDisk |= metadata.TagTypes;
         
         if (propertiesStyle != ReadStyle.None)
         {
            // Check that the first block is a METADATA_BLOCK_STREAMINFO.
            if (blocks.Count == 0 || blocks [0].Type != BlockType.StreamInfo)
               throw new CorruptFileException ("FLAC stream does not begin with StreamInfo.");
            
            // The stream exists from the end of the last block to the end of the file.
            stream_start = end;
            header_block = blocks [0].Data;
         }
      }
      
      protected override void ReadEnd (long end, ReadStyle propertiesStyle)
      {
         tag = new CombinedTag (metadata, base.Tag);
         GetTag (TagTypes.Xiph, true);
      }
      
      protected override TagLib.Properties ReadProperties (long start, long end, ReadStyle propertiesStyle)
      {
         StreamHeader header = new StreamHeader (header_block, end - stream_start);
         return new Properties (TimeSpan.Zero, header);
      }
      
      private enum BlockMode
      {
         Blacklist,
         Whitelist
      }
      
      private List<Block> ReadBlocks (ref long start, out long end, BlockMode mode, params BlockType[] types)
      {
         List<Block> blocks = new List<Block>();
         
         long start_position = Find ("fLaC", start);
         
         if (start_position < 0)
            throw new CorruptFileException ("FLAC stream not found at starting position.");
         
         end = start = start_position + 4;
         
         Seek (start);
         
         BlockHeader header;
         do
         {
            header = new BlockHeader (ReadBlock ((int)BlockHeader.Size));
            
            bool found = false;
            foreach (BlockType type in types)
               if (header.BlockType == type)
               {
                  found = true;
                  break;
               }
            
            if ((mode == BlockMode.Whitelist &&  found) || (mode == BlockMode.Blacklist && !found))
               blocks.Add (new Block (header, ReadBlock ((int) header.BlockSize)));
            else
               Seek (header.BlockSize, System.IO.SeekOrigin.Current);
            
            end += header.BlockSize + BlockHeader.Size;
         }
         while (!header.IsLastBlock);

         return blocks;
      }
   }
   
   
   
   public class Metadata : CombinedTag
   {
      public override TagTypes TagTypes {get {return TagTypes.FlacMetadata | base.TagTypes;}}
      
      private List<IPicture> pictures = new List<IPicture>();
      
      public Metadata (List<Block> blocks)
      {
         foreach (Block block in blocks)
         {
            if (block.Data.Count == 0)
               continue;
            
            if (block.Type == BlockType.XiphComment)
               AddTag (new Ogg.XiphComment (block.Data));
            else if (block.Type == BlockType.Picture)
               pictures.Add (new Picture (block.Data));
         }
      }
      
      public override IPicture[] Pictures
      {
         get {return pictures.ToArray ();}
         set {pictures.Clear (); if (value != null) pictures.AddRange (value);}
      }
      
      public Ogg.XiphComment GetComment (bool create, Tag copy)
      {
         foreach (Tag t in Tags)
            if (t is Ogg.XiphComment)
               return t as Ogg.XiphComment;
         
         if (!create)
            return null;
         
         Ogg.XiphComment c = new Ogg.XiphComment ();
         if (copy != null)
            TagLib.Tag.Duplicate (copy, c, true);
         AddTag (c);
         return c;
      }
      
      public void RemoveComment ()
      {
         Ogg.XiphComment c;
         while ((c = GetComment (false, null)) != null)
            RemoveTag (c);
      }
      
      public override void Clear ()
      {
         pictures.Clear ();
      }
   }
}