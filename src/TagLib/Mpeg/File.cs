/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : mpegfile.cpp from TagLib
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

namespace TagLib.Mpeg
{
   [SupportedMimeType("taglib/mp3", "mp3")]
   [SupportedMimeType("audio/x-mp3")]
   [SupportedMimeType("application/x-id3")]
   [SupportedMimeType("audio/mpeg")]
   [SupportedMimeType("audio/x-mpeg")]
   [SupportedMimeType("audio/x-mpeg-3")]
   [SupportedMimeType("audio/mpeg3")]
   [SupportedMimeType("audio/mp3")]
   public class File : TagLib.File
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Id3v2.Tag    id3v2_tag;
      private Ape.Tag      ape_tag;
      private Id3v1.Tag    id3v1_tag;
      private CombinedTag  tag;
      private Properties   properties;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public File (string file, Properties.ReadStyle properties_style) : base (file)
      {
         id3v2_tag  = null;
         ape_tag    = null;
         id3v1_tag  = null;
         tag        = new CombinedTag ();
         properties = null;
         
         try {Mode = AccessMode.Read;}
         catch {return;}
         
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
         if (id3v2_tag == null)
            RemoveBlock (tag_start, tag_end - tag_start);
         else
            Insert (id3v2_tag.Render (), tag_start, tag_end - tag_start);
         
         // Update ID3v1 tag
         FindId3v1 (out tag_start, out tag_end);
         if (id3v1_tag == null)
            RemoveBlock (tag_start, tag_end - tag_start);
         else
            Insert (id3v1_tag.Render (), tag_start, tag_end - tag_start);
         
         // Update Ape tag
         FindApe (id3v2_tag != null, out tag_start, out tag_end);
         if (ape_tag == null)
            RemoveBlock (tag_start, tag_end - tag_start);
         else
            Insert (ape_tag.Render (), tag_start, tag_end - tag_start);

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
                  TagLib.Tag.Duplicate (tag, id3v1_tag, true);
                  tag.SetTags (id3v2_tag, ape_tag, id3v1_tag);
               }
               
               return id3v1_tag;
            }
            
            case TagTypes.Id3v2:
            {
               if (create && id3v2_tag == null)
               {
                  id3v2_tag = new Id3v2.Tag ();
                  TagLib.Tag.Duplicate (tag, id3v2_tag, true);
                  tag.SetTags (id3v2_tag, ape_tag, id3v1_tag);
               }
               
               return id3v2_tag;
            }
            
            case TagTypes.Ape:
            {
               if (create && ape_tag == null)
               {
                  ape_tag = new Ape.Tag ();
                  TagLib.Tag.Duplicate (tag, ape_tag, true);
                  tag.SetTags (id3v2_tag, ape_tag, id3v1_tag);
               }
               
               return ape_tag;
            }
            
            default:
               return null;
         }
      }

      public void Remove (TagTypes types)
      {
         if ((types & TagTypes.Id3v1) != 0)
            id3v1_tag = null;

         if ((types & TagTypes.Id3v2) != 0)
            id3v2_tag = null;

         if ((types & TagTypes.Ape) != 0)
            ape_tag = null;

         tag.SetTags (id3v2_tag, ape_tag, id3v1_tag);
      }
      
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      private Header FindFirstFrameHeader (long start)
      {
         long position = start;
         Seek (position);
         ByteVector buffer = ReadBlock (3);
         
         if (buffer.Count < 3)
            return null;
         
         do
         {
            Seek (position + 3);
            buffer = buffer.Mid (buffer.Count - 3);
            buffer.Add (ReadBlock ((int) BufferSize));
            
            for (int i = 0; i < buffer.Count - 3; i++)
               if (buffer [i] == 0xFF && SecondSynchByte (buffer [i + 1]))
                  try {return new Header (buffer.Mid (i, 4), position + i);} catch {}
            
            position += BufferSize;
         }
         while (buffer.Count > 3);
        
         return null;
      }
      
      private Header FindLastFrameHeader (long end)
      {
         long position = end - 3;
         Seek (position);
         ByteVector buffer = ReadBlock (3);
         position -= BufferSize;
         
         while (position >= 0)
         {
            Seek (position);
            buffer.Insert (0, ReadBlock ((int) BufferSize));
            
            for (int i = buffer.Count - 4; i >= 0; i--)
               if (buffer [i] == 0xFF && SecondSynchByte (buffer [i + 1]))
                  try {return new Header (buffer.Mid (i, 4), position + i);} catch {}
            
            position -= BufferSize;
            buffer = buffer.Mid (0, 3);
         }
         
         return null;
      }
      
      public override TagLib.Tag Tag {get {return tag;}}
      
      public override AudioProperties AudioProperties {get {return properties;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Read (Properties.ReadStyle properties_style)
      {
         long start, end;
         Header first_header = null;
         
         // Look for an ID3v2 tag.
         if (FindId3v2 (out start, out end))
            id3v2_tag = new Id3v2.Tag (this, start);
         
         // If we're reading properties, grab the first one for good measure.
         if (properties_style != Properties.ReadStyle.None)
            first_header = FindFirstFrameHeader (start > 2048 ? 0 : end);
         
         // Look for an ID3v1 tag.
         if (FindId3v1 (out start, out end))
            id3v1_tag = new Id3v1.Tag (this, start);

         // Look for an APE tag.
         if (FindApe (id3v1_tag != null, out start, out end))
            ape_tag = new Ape.Tag (this, start);
         
         // Set the tags and create Id3v2.
         tag.SetTags (id3v2_tag, ape_tag, id3v1_tag);
         GetTag (TagTypes.Id3v2, true);
         
         // Now read the properties.
         if (properties_style != Properties.ReadStyle.None)
            properties = new Properties (this, first_header, properties_style);
      }

      private bool FindId3v2 (out long start, out long end)
      {
         Seek (0);
         start = end = Find (Id3v2.Header.FileIdentifier);
         
         if (start >= 0)
         {
            Seek (start + 3);
            Id3v2.Header header = new Id3v2.Header (Id3v2.Header.FileIdentifier + ReadBlock (7));
            if (header.TagSize != 0)
            {
               end = start + header.CompleteTagSize;
               return true;
            }
         }
         
         start = end = 0;
         return false;
      }

      private bool FindApe (bool has_id3v1, out long start, out long end)
      {
         start = end = Length - (has_id3v1 ? 128 : 0);
         
         if (end >= 32)
         {
            Seek (end - 32);
            ByteVector data = ReadBlock (32);
            
            if (data.StartsWith (Ape.Tag.FileIdentifier))
            {
               Ape.Footer footer = new Ape.Footer (data);
               start = end - footer.CompleteTagSize;
               return true;
            }
         }
         return false;
      }
      
      private bool FindId3v1 (out long start, out long end)
      {
         start = end = Length;
         
         if (end >= 128)
         {
            Seek (end - 128);
            start = Tell;
            
            if (ReadBlock (3) == Id3v1.Tag.FileIdentifier)
               return true;
         }
         
         start = end;
         return false;
      }
      
      private bool SecondSynchByte (byte b)
      {
         return b >= 0xE0;
      }
   }
}
