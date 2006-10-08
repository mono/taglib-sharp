/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : mpcfile.cpp from TagLib
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

namespace TagLib.Mpc
{
   [SupportedMimeType("taglib/mpc", "mpc")]
   [SupportedMimeType("taglib/mp+", "mp+")]
   [SupportedMimeType("audio/x-musepack")]
   public class File : TagLib.File
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Ape.Tag      ape_tag;
      private Id3v1.Tag    id3v1_tag;
      private Id3v2.Tag    id3v2_tag;
      private CombinedTag  tag;
      private Properties   properties;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public File (string file, Properties.ReadStyle properties_style) : base (file)
      {
         ape_tag    = null;
         id3v1_tag  = null;
         id3v2_tag  = null;
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
         if(IsReadOnly) {
            throw new ReadOnlyException();
         }
         
         Mode = AccessMode.Write;
         
         // Update ID3v2 tag
         long id3v2_location = FindId3v2 ();
         int  id3v2_size     = 0;
         
         if (id3v2_location != -1)
         {
            Seek (id3v2_location);
            Id3v2.Header header = new Id3v2.Header (ReadBlock ((int) Id3v2.Header.Size));
            if (header.TagSize == 0)
            {
               Debugger.Debug ("Mpc.File.Save() -- Id3v2 header is broken. Ignoring.");
               id3v2_location = - 1;
            }
            else
               id3v2_size = (int) header.CompleteTagSize;
         }
         
         if(id3v2_tag != null)
         {
            if (id3v2_location >= 0)
               Insert (id3v2_tag.Render (), id3v2_location, id3v2_size);
            else
               Insert (id3v2_tag.Render (), 0, 0);
         }
         else if (id3v2_location >= 0)
            RemoveBlock (id3v2_location, id3v2_size);


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
         
         if (ape_location >= 0)
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
            case TagTypes.Id3v2:
            {
               if (create && id3v2_tag == null)
               {
                  id3v2_tag = new Id3v2.Tag ();
                  
                  if (tag != null)
                     TagLib.Tag.Duplicate (tag, id3v2_tag, true);
                  
                  tag.SetTags (ape_tag, id3v2_tag, id3v1_tag);
               }
               return id3v2_tag;
            }
            
            case TagTypes.Id3v1:
            {
               if (create && id3v1_tag == null)
               {
                  id3v1_tag = new Id3v1.Tag ();
                  
                  if (tag != null)
                     TagLib.Tag.Duplicate (tag, id3v1_tag, true);
                  
                  tag.SetTags (ape_tag, id3v2_tag, id3v1_tag);
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
                  
                  tag.SetTags (ape_tag, id3v2_tag, id3v1_tag);
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

         tag.SetTags (ape_tag, id3v2_tag, id3v1_tag);
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
         {
            id3v1_tag = new Id3v1.Tag (this, id3v1_location);
         }
         

         // Look for an APE tag
         long ape_location = FindApe (id3v1_location != 0);

         if (ape_location >= 0)
            ape_tag = new Ape.Tag (this, ape_location);


         // Look for an ID3v2 tag
         long id3v2_location = FindId3v2 ();

         if(id3v2_location >= 0)
            id3v2_tag = new Id3v2.Tag (this, id3v2_location);


         tag.SetTags (ape_tag, id3v2_tag, id3v1_tag);
         GetTag (TagTypes.Ape, true);

         
         // Look for MPC metadata
         Seek ((id3v2_location >= 0) ? (id3v2_location + id3v2_tag.Header.CompleteTagSize) : 0);
         
         if(properties_style != Properties.ReadStyle.None)
            properties = new Properties (ReadBlock ((int) Properties.HeaderSize),
               Length - Tell - ape_tag.Footer.CompleteTagSize);
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
      
      private long FindId3v2 ()
      {
         if (!IsValid)
            return -1;

         Seek (0);

         if(ReadBlock (3) == Id3v2.Header.FileIdentifier)
            return 0;

         return -1;
      }
   }
}
