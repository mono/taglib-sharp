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
   [SupportedMimeType("taglib/wv")]
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
         if(IsReadOnly) {
            throw new ReadOnlyException();
         }
         
         Mode = AccessMode.Write;
         
         // Update ID3v1 tag
         long id3v1_location = FindId3v1 ();

         if (id3v1_tag != null)
         {
            if (id3v1_location >= 0)
               Insert (id3v1_tag.Render (), id3v1_location, 128);
            else
            {
               Seek (0, System.IO.SeekOrigin.End);
               id3v1_location = Tell;
               WriteBlock (id3v1_tag.Render ());
            }
         }
         else if(id3v1_location >= 0)
         {
            RemoveBlock (id3v1_location, 128);
            id3v1_location = -1;
         }


         // Update APE tag
         long ape_location = FindApe (id3v1_location != -1);
         long ape_size     = 0;
         
         if (ape_location != -1)
         {
            Seek (ape_location);
            ape_size = (new Ape.Footer (ReadBlock ((int) Ape.Footer.Size))).CompleteTagSize;
            ape_location = ape_location + Ape.Footer.Size - ape_size;
         }
         
         if (ape_tag != null)
         {
            if (ape_location >= 0)
               Insert (ape_tag.Render (), ape_location, ape_size);
            else
            {
               if (id3v1_location >= 0)
                  Insert (ape_tag.Render (), id3v1_location, 0);
               else
               {
                  Seek (0, System.IO.SeekOrigin.End);
                  WriteBlock (ape_tag.Render ());
               }
            }
         }
         else if (ape_location >= 0)
            RemoveBlock (ape_location, ape_size);

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
         // Look for an ID3v1 tag

         long id3v1_location = FindId3v1 ();

         if (id3v1_location >= 0)
            id3v1_tag = new Id3v1.Tag (this, id3v1_location);

         // Look for an APE tag

         long ape_location = FindApe (id3v1_location != -1);

         if (ape_location >= 0)
            ape_tag = new Ape.Tag (this, ape_location);

         tag.SetTags (ape_tag, id3v1_tag);
         GetTag (TagTypes.Ape, true);
         
         // Look for MPC metadata

         if (properties_style != Properties.ReadStyle.None)
         {
            Seek (0);
            properties = new Properties (ReadBlock ((int) Properties.HeaderSize),
               ape_location + Ape.Footer.Size - ape_tag.Footer.CompleteTagSize);
         }
      }
      
      private long FindApe (bool has_id3v1)
      {
         if (!IsValid)
            return -1;

         if(has_id3v1)
            Seek (-160, System.IO.SeekOrigin.End);
         else
            Seek (-32, System.IO.SeekOrigin.End);

         long p = Tell;
         
         if(ReadBlock (8) == Ape.Tag.FileIdentifier)
            return p;

         return -1;
      }
      
      private long FindId3v1 ()
      {
         if (!IsValid)
            return -1;

         Seek (-128, System.IO.SeekOrigin.End);
         long p = Tell;

         if(ReadBlock (3) == Id3v1.Tag.FileIdentifier)
            return p;

         return -1;
      }
   }
}
