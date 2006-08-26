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

using System.Collections;
using System;

namespace TagLib.Flac
{
   [SupportedMimeType("taglib/flac")]
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
      private CombinedTag        tag;

      private Properties         properties;
      private ByteVector         stream_info_data;
      private ByteVector         xiph_comment_data;
      private long               flac_start;
      private long               stream_start;
      private long               stream_length;
      private bool               scanned;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public File (string file, Properties.ReadStyle properties_style) : base (file)
      {
         id3v2_tag           = null;
         id3v1_tag           = null;
         comment             = null;
         properties          = null;
         flac_start          = 0;
         stream_start        = 0;
         stream_length       = 0;
         scanned             = false;
         tag                 = new CombinedTag ();
         try {Mode = AccessMode.Read;}
         catch {return;}
         
         Read (properties_style);
         
         Mode = AccessMode.Closed;
      }

      public File (string file) : this (file, Properties.ReadStyle.Average)
      {
      }




      public override bool Save ()
      {
         if (IsReadOnly)
         {
            Debugger.Debug ("Flac.File.Save() - Cannot save to a read only file.");
            return false;
         }
         
         try {Mode = AccessMode.Write;}
         catch
         {
            Debugger.Debug ("Flac.File.Save() - Cannot save to a read only file.");
            return false;
         }


         long flac_data_begin;
         long flac_data_end;
         

         // Update ID3 tags
         if(id3v2_tag != null)
         {
            ByteVector id3v2_tag_data = id3v2_tag.Render ();
            
            long id3v2_location = FindId3v2 ();
            if (id3v2_location >= 0)
            {
               int id3v2_size = 0;
               
               Seek (id3v2_location);
               Id3v2.Header header = new Id3v2.Header (ReadBlock ((int) Id3v2.Header.Size));
            
               if (header.TagSize == 0)
                  Debugger.Debug ("Flac.File.Save() -- Id3v2 header is broken. Ignoring.");
               else
                  id3v2_size = (int) header.CompleteTagSize;

               Insert (id3v2_tag_data, id3v2_location, id3v2_size);
               System.Console.WriteLine ("ID3v2: " + id3v2_size + " " + id3v2_tag_data.Count);
               flac_data_begin = id3v2_location + id3v2_tag_data.Count;
            }
            else
            {
               Insert (id3v2_tag_data, 0, 0);
               flac_data_begin = id3v2_tag_data.Count;
            }
         }
         else 
            flac_data_begin = 0;

         if(id3v1_tag != null)
         {
            long id3v1_location = FindId3v1 ();
            
            if (id3v1_location >= 0)
               Seek (id3v1_location);
            else
               Seek (0, System.IO.SeekOrigin.End);
            
            flac_data_end = Tell;
            WriteBlock (id3v1_tag.Render ());
         }
         else
            flac_data_end = Length;


         // Create new vorbis comments is they don't exist.
         GetTag (TagTypes.Xiph, true);

         xiph_comment_data = comment.Render (false);

         ByteVector v = ByteVector.FromUInt((uint) xiph_comment_data.Count);

         // Set the type of the comment to be a Xiph / Vorbis comment
         // (See scan() for comments on header-format)
         v [0] = 4;
         v.Add (xiph_comment_data);


         // If file already have comment => find and update it
         //                       if not => insert one

         scanned = false;
         
         if (Scan (flac_data_begin, flac_data_end) != null)
         {
            long next_page_offset = flac_start;
            Seek (next_page_offset);
            ByteVector header = ReadBlock (4);
            uint length = header.Mid(1, 3).ToUInt ();

            next_page_offset += length + 4;

            // Search through the remaining metadata

            byte block_type = (byte) (header[0] & 0x7f);
            bool last_block = (header[0] & 0x80) != 0;

            while (!last_block)
            {
               Seek (next_page_offset);

               header = ReadBlock (4);
               block_type = (byte) (header[0] & 0x7f);
               last_block = (header[0] & 0x80) != 0;
               length = header.Mid(1, 3).ToUInt ();
               
               // Type is vorbiscomment
               if(block_type == 4)
               {
                  long next_keep = (last_block ? 0 : FindPaddingBreak (next_page_offset + length + 4,
                                                                       next_page_offset + XiphCommentData.Count + 8,
                                                                       ref last_block));
                  uint padding_length;
                  if(next_keep != 0)
                  {
                     // There is space for comment and padding blocks without rewriting the whole file.
                     // Note this can not overflow.
                     padding_length = (uint) (next_keep - (next_page_offset + XiphCommentData.Count + 8));
                  }
                  else
                  {
                     // Not enough space, so we will have to rewrite the whole file following this block
                     padding_length = (uint) XiphCommentData.Count;
                     if (padding_length < 4096)
                        padding_length = 4096;
                     next_keep = next_page_offset + length + 4;
                  }
                  
                  ByteVector padding = ByteVector.FromUInt (padding_length);
                  padding [0] = 1;
                  if (last_block)
                     padding [0] = (byte) (padding [0] | 0x80);
                  padding.Resize ((int)(padding_length + 4));
                  
                  Insert (v + padding, next_page_offset, next_keep - next_page_offset);
               System.Console.WriteLine ("OGG: " + (next_keep - next_page_offset) + " " + (v.Count + padding.Count));
                  
                  break;
               }

               next_page_offset += length + 4;
            }
         }
         else
         {
            long next_page_offset = flac_start;

            Seek (next_page_offset);

            ByteVector header = ReadBlock (4);
            bool last_block = (header[0] & 0x80) != 0;
            uint length = header.Mid(1, 3).ToUInt ();

            // If last block was last, make this one last

            if(last_block)
            {
               // Copy the bottom seven bits into the new value

               ByteVector h = (byte)(header[0] & 0x7F);
               Insert (h, next_page_offset, 1);

               // Set the last bit
               v [0] = (byte) (v [0] | 0x80);
            }

            Insert (v, next_page_offset + length + 4, 0);
         }

         Mode = AccessMode.Closed;
         return true;
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
                  
                  tag.SetTags (comment, id3v2_tag, id3v1_tag);
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
                  
                  tag.SetTags (comment, id3v2_tag, id3v1_tag);
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
                  
                  tag.SetTags (comment, id3v2_tag, id3v1_tag);
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
         long flac_data_begin;
         long flac_data_end;
         
         // Look for an ID3v2 tag
         long id3v2_location = FindId3v2 ();
         
         if(id3v2_location >= 0)
         {
            id3v2_tag = new Id3v2.Tag (this, id3v2_location);
            flac_data_begin = id3v2_location + id3v2_tag.Header.CompleteTagSize;
         }
         else
            flac_data_begin = 0;

         // Look for an ID3v1 tag
         long id3v1_location = FindId3v1 ();

         if(id3v1_location >= 0)
         {
            id3v1_tag = new Id3v1.Tag (this, id3v1_location);
            flac_data_end = id3v1_location;
         }
         else
            flac_data_end = Length;

         // Look for FLAC metadata, including vorbis comments

         xiph_comment_data = Scan (flac_data_begin, flac_data_end);

         if (!IsValid) return;

         if (XiphCommentData.Count > 0)
            comment = new Ogg.XiphComment (XiphCommentData);

         tag.SetTags (comment, id3v2_tag, id3v1_tag);
         
         GetTag (TagTypes.Xiph, true);

         if(properties_style != Properties.ReadStyle.None)
            properties = new Properties (stream_info_data, stream_length, properties_style);
      }

      private ByteVector Scan (long begin, long end)
      {
         ByteVector xiph_comment_data = null;
         
         // Scan the metadata pages

         if(scanned || !IsValid)
            return null;
         
         long next_page_offset;
         long file_size = Length;

         next_page_offset = Find ("fLaC", begin);

         if (next_page_offset < 0)
         {
            Debugger.Debug ("Flac.File.Scan () -- FLAC stream not found");
            SetValid (false);
            return null;
         }

         next_page_offset += 4;
         flac_start = next_page_offset;

         Seek (next_page_offset);

         ByteVector header = ReadBlock (4);

         // Header format (from spec):
         // <1> Last-metadata-block flag
         // <7> BLOCK_TYPE
         //	0 : STREAMINFO
         //    1 : PADDING
         //    ..
         //    4 : VORBIS_COMMENT
         //    ..
         // <24> Length of metadata to follow

         byte block_type = (byte) (header [0] & 0x7f);
         bool last_block = (header [0] & 0x80) != 0;
         uint length = header.Mid (1, 3).ToUInt ();

         // First block should be the stream_info metadata
         if (block_type != 0)
         {
            Debugger.Debug ("Flac.File.Scan() -- invalid FLAC stream");
            SetValid (false);
            return null;
         }
         stream_info_data  = ReadBlock ((int) length);
         next_page_offset += length + 4;

         // Search through the remaining metadata

         while (!last_block)
         {
            header = ReadBlock (4);
            block_type = (byte) (header [0] & 0x7f);
            last_block = (header [0] & 0x80) != 0;
            length = header.Mid (1, 3).ToUInt ();

            // Found the vorbis-comment
            if(block_type == 4)
               xiph_comment_data  = ReadBlock ((int) length);

            next_page_offset += length + 4;
            if (next_page_offset >= file_size)
            {
               Debugger.Debug ("Flac.File.Scan() -- FLAC stream corrupted");
               SetValid (false);
               return null;
            }
            Seek (next_page_offset);
         }

         // End of metadata, now comes the datastream
         stream_start = next_page_offset;
         stream_length = end - stream_start;

         scanned = true;
         
         return xiph_comment_data;
      }

      private long FindId3v1 ()
      {
         if (IsValid)
         {
            Seek (-128, System.IO.SeekOrigin.End);
            long p = Tell;

            if (ReadBlock (3) == Id3v1.Tag.FileIdentifier)
            return p;
         }
         
         return -1;
      }

      private long FindId3v2 ()
      {
         if (IsValid)
         {
            Seek (0);

            if (ReadBlock (3) == Id3v2.Header.FileIdentifier)
               return 0;
         }
         
         return -1;
      }

      private long FindPaddingBreak (long next_page_offset, long target_offset, ref bool last)
      {
         // Starting from nextPageOffset, step over padding blocks to find the
         // address of a block which is after targetOffset. Return zero if
         // a non-padding block occurs before that point
         
         while (true)
         {
            Seek (next_page_offset);
         
            ByteVector header = ReadBlock (4);
            byte block_type   = (byte) (header[0] & 0x7f);
            bool last_block   = (header[0] & 0x80) != 0;
            uint length       = header.Mid(1, 3).ToUInt ();
         
            if (block_type != 1)
               break;
         
            next_page_offset += 4 + length;
         
            if(next_page_offset >= target_offset)
            {
               last = last_block;
               return next_page_offset;
            }
            
            if (last_block)
               break;
         }
         return 0;
      }

      private ByteVector XiphCommentData
      {
         get
         {
            return (IsValid && xiph_comment_data != null) ? xiph_comment_data : new ByteVector ();
         }
      }
   }
}
