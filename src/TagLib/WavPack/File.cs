/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : wvfile.cpp from libtunepimp
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

namespace TagLib.WavPack
{
   [SupportedMimeType("taglib/wv", "wv")]
   [SupportedMimeType("audio/x-wavpack")]
   public class File : TagLib.File
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Ape.Tag     ape_tag;
      private Id3v1.Tag   id3v1_tag;
      private CombinedTag tag;
      private Properties  properties;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public File (string file, Properties.ReadStyle properties_style) : base (file)
      {
         ape_tag        = null;
         id3v1_tag      = null;
         tag            = new CombinedTag ();
         properties     = null;
         
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
         
         // Update ID3v1 tag
         FindId3v1 (out tag_start, out tag_end);
         if (id3v1_tag == null)
            RemoveBlock (tag_start, tag_end - tag_start);
         else
            Insert (id3v1_tag.Render (), tag_start, tag_end - tag_start);
         
         // Update Ape tag
         FindApe (id3v1_tag != null, out tag_start, out tag_end);
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
                  
                  if (tag != null)
                     TagLib.Tag.Duplicate (tag, id3v1_tag, true);
                  
                  tag.SetTags (ape_tag, id3v1_tag);
               }
               return id3v1_tag;
            }
            
            case TagTypes.Ape:
            {
               if (create && ape_tag == null)
               {
                  ape_tag = new Ape.Tag ();
                  
                  if (tag != null)
                     TagLib.Tag.Duplicate (tag, ape_tag, true);
                  
                  tag.SetTags (ape_tag, id3v1_tag);
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

         if ((types & TagTypes.Ape) != 0)
            ape_tag = null;

         tag.SetTags (ape_tag, id3v1_tag);
      }
      
      public void Remove ()
      {
         Remove (TagTypes.AllTags);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TagLib.Tag Tag {get {return tag;}}
      
      public override TagLib.AudioProperties AudioProperties {get {return properties;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Read (Properties.ReadStyle properties_style)
      {
         long tag_start, tag_end, ape_tag_start;
         
         bool has_id3v1 = FindId3v1 (out tag_start, out tag_end);
         
         if (has_id3v1)
            id3v1_tag = new Id3v1.Tag (this, tag_start);
         
         if (FindApe (has_id3v1, out ape_tag_start, out tag_end))
            ape_tag = new Ape.Tag (this, ape_tag_start);

         tag.SetTags (ape_tag, id3v1_tag);
         GetTag (TagTypes.Ape, true);
         
         // Look for MPC metadata
         if (properties_style != Properties.ReadStyle.None)
         {
            Seek (0);
            properties = new Properties (ReadBlock ((int) Properties.HeaderSize),
               ape_tag_start);
         }
      }
      
      private bool FindApe (bool has_id3v1, out long start, out long end)
      {
         Seek (has_id3v1 ? -160 : -32, System.IO.SeekOrigin.End);
         
         start = end = Tell + 32;
         
         ByteVector data = ReadBlock (32);
         if (data.StartsWith (Ape.Tag.FileIdentifier))
         {
            Ape.Footer footer = new Ape.Footer (data);
            start = end - footer.CompleteTagSize;
            return true;
         }
         return false;
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
   }
}
