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

      public void Save (TagTypes types, bool strip_others)
      {
         if (types == TagTypes.NoTags && strip_others) {
            if(!Strip (TagTypes.AllTags)) {
                throw new ApplicationException("Could not strip tags");
            }
            
            return;
         }

         if (id3v2_tag == null && id3v1_tag == null && ape_tag == null) {
            if (strip_others) {
               if(!Strip (TagTypes.AllTags)) {
                throw new ApplicationException("Could not strip tags");
               }
            }
            
            return;
         }
         
         if(IsReadOnly) {
            throw new ReadOnlyException();
         }
         
         Mode = AccessMode.Write;

         // Create the tags if we've been asked to.  Copy the values from the tag that
         // does exist into the new tag.

         if ((types & TagTypes.Id3v2) != 0 && id3v1_tag != null)
            TagLib.Tag.Duplicate (id3v1_tag, GetTag (TagTypes.Id3v2, true), false);

         if ((types & TagTypes.Id3v1) != 0 && id3v2_tag != null)
            TagLib.Tag.Duplicate (id3v2_tag, GetTag (TagTypes.Id3v1, true), false);

         bool success = true;

         if ((TagTypes.Id3v2 & types) != 0 && id3v2_tag != null && !id3v2_tag.IsEmpty)
         {
            long id3v2_location = FindId3v2 ();
            int  id3v2_size     = 0;
            
            if (id3v2_location < 0)
               id3v2_location = 0;
            else
            {
               Seek (id3v2_location);
               Id3v2.Header header = new Id3v2.Header (ReadBlock ((int) Id3v2.Header.Size));
            
               if (header.TagSize == 0)
                  Debugger.Debug ("Mpc.File.Save() -- Id3v2 header is broken. Ignoring.");
               else
                  id3v2_size = (int) header.CompleteTagSize;
            }

            Insert (id3v2_tag.Render (), id3v2_location, id3v2_size);
         }
         else if (strip_others)
            success = Strip (TagTypes.Id3v2) && success;

         if((TagTypes.Id3v1 & types) != 0 && id3v1_tag != null && !id3v1_tag.IsEmpty)
         {
            long id3v1_location = FindId3v1 ();
            if (id3v1_location < 0)
               Seek (0, System.IO.SeekOrigin.End);
            else
               Seek (id3v1_location);
            
            WriteBlock (id3v1_tag.Render ());
         }
         else if (strip_others)
            success = Strip (TagTypes.Id3v1, false) && success;

         // Dont save an APE-tag unless one has been created
         if((TagTypes.Ape & types) != 0 && ape_tag != null && !ape_tag.IsEmpty)
         {
            long ape_location = FindApe (FindId3v1 () >= 0);
            long ape_size = 0;
            
            if (ape_location < 0)
               ape_location = Length;
            else
            {
               Seek (ape_location);
               ape_size = (new Ape.Footer (ReadBlock ((int) Ape.Footer.Size))).CompleteTagSize;
               ape_location = ape_location + Ape.Footer.Size - ape_size;
            }
            
            Insert (ape_tag.Render (), ape_location, ape_size);
         }
         else if(strip_others)
            success = Strip (TagTypes.Ape, false) && success;

         Mode = AccessMode.Closed;
         
         if(!success) {
            throw new ApplicationException("Cannot write tags");
         }
      }

      public void Save (TagTypes tags)
      {
         Save (tags, true);
      }

      public override void Save ()
      {
         Save (TagTypes.AllTags);
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

      public bool Strip (TagTypes types, bool free_memory)
      {
         AccessMode original_mode = Mode;
         
         if (IsReadOnly)
         {
            Debugger.Debug ("Mpeg.File.Strip() - Cannot strip tags from a read only file.");
            return false;
         }
         
         try {Mode = AccessMode.Write;}
         catch
         {
            Debugger.Debug ("Mpeg.File.Strip() - Cannot strip tags from a read only file.");
            return false;
         }

         if ((types & TagTypes.Id3v2) != 0)
         {
            long id3v2_location = FindId3v2 ();
            if (id3v2_location >= 0)
            {
               Seek (id3v2_location);
               Id3v2.Header header = new Id3v2.Header (ReadBlock ((int) Id3v2.Header.Size));
               
               if (header.TagSize == 0)
                  Debugger.Debug ("Mpc.File.Save() -- Id3v2 header is broken. Ignoring.");
               else
                  RemoveBlock (id3v2_location, (int) header.CompleteTagSize);
            }
            
            if (free_memory)
               id3v2_tag = null;
         }

         long id3v1_location = FindId3v1 ();
         
         if ((types & TagTypes.Id3v1) != 0)
         {
            if (id3v1_location >= 0)
            {
               Truncate (id3v1_location);
               id3v1_location = -1;
            }
            
            if (free_memory)
               id3v1_tag = null;
         }

         if ((types & TagTypes.Ape) != 0)
         {
            long ape_location = FindApe (id3v1_location >= 0);
            if (ape_location != -1)
            {
               Seek (ape_location);
               int ape_size = (int) (new Ape.Footer (ReadBlock ((int) Ape.Footer.Size))).CompleteTagSize;
               ape_location = ape_location + Ape.Footer.Size - ape_size;
               RemoveBlock (ape_location, ape_size);
            }
            
            if (free_memory)
               ape_tag = null;
         }
         
         tag.SetTags (id3v2_tag, ape_tag, id3v1_tag);
         
         Mode = original_mode;
         return true;
      }

      public bool Strip (TagTypes types)
      {
         return Strip (types, true);
      }

      public bool Strip ()
      {
         return Strip (TagTypes.AllTags);
      }
      
      public long NextFrameOffset (long position)
      {
         ByteVector buffer = (byte) 0;
         bool previous_partial_synch_match = false;
         
         while (buffer.Count > 0)
         {
            Seek (position);
            buffer = ReadBlock ((int) BufferSize);

            if (previous_partial_synch_match && SecondSynchByte (buffer [0]))
               return position - 1;
            
            for (int i = 0; i < buffer.Count - 1; i++)
               if (buffer [i] == 0xFF && SecondSynchByte (buffer [i + 1]))
                  return position + i;
            
            if (buffer [buffer.Count - 1] == 0xFF)
               previous_partial_synch_match = true;
            
            position += BufferSize;
         }
         
         return -1;
      }

      public long PreviousFrameOffset (long position)
      {
         bool previous_partial_synch_match = false;
         
         while (position - BufferSize > BufferSize)
         {
            position -= BufferSize;
            Seek (position);
            ByteVector buffer = ReadBlock ((int) BufferSize);

            if (previous_partial_synch_match && SecondSynchByte (buffer [(int) BufferSize - 1]))
               return position + BufferSize - 1;
            
            // If the amount of data is smaller than an MPEG header (4 bytes) there's no
            // chance of this being valid.

            if (buffer.Count < 4)
               return -1;

            for (int i = buffer.Count - 2; i >= 0; i--)
               if (buffer [i] == 0xFF && SecondSynchByte (buffer [i + 1]))
                  return position + i;
            
            if (buffer [0] == 0xFF)
               previous_partial_synch_match = true;
         }

         return -1;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public long FirstFrameOffset
      {
         get
         {
            // This is kind of bad, because the ID3 tag could be anywhere (and
            // under the old method, if it were an end tag there wouldn't be any
            // room to find a frame. So this is just a "good guess".
            return NextFrameOffset (id3v2_tag != null ? id3v2_tag.Header.CompleteTagSize : 0);
         }
      }

      public long LastFrameOffset
      {
         get
         {
            long id3v1_location = FindId3v1 ();
            return PreviousFrameOffset ((id3v1_location >= 0) ? id3v1_location - 1 : Length);
         }
      }

      public override TagLib.Tag Tag {get {return tag;}}
      
      public override AudioProperties AudioProperties {get {return properties;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Read (Properties.ReadStyle properties_style)
      {
         // Look for an ID3v2 tag
         long id3v2_location = FindId3v2 ();

         if (id3v2_location >= 0)
            id3v2_tag = new Id3v2.Tag (this, id3v2_location);


         // Look for an ID3v1 tag
         long id3v1_location = FindId3v1 ();

         if (id3v1_location >= 0)
            id3v1_tag = new Id3v1.Tag (this, id3v1_location);


         // Look for an APE tag
         long ape_location = FindApe (id3v1_location >= 0);

         if (ape_location >= 0)
            ape_tag = new Ape.Tag (this, ape_location);
         
         
         tag.SetTags (id3v2_tag, ape_tag, id3v1_tag);

         GetTag (TagTypes.Id3v2, true);
         
         if (properties_style != Properties.ReadStyle.None)
            properties = new Properties (this, properties_style);
      }

      private long FindId3v2 ()
      {
         // This method is based on the contents of TagLib::File::find(), but because
         // of some subtlteies -- specifically the need to look for the bit pattern of
         // an MPEG sync, it has been modified for use here.
         
         if (IsValid && Id3v2.Header.FileIdentifier.Count <= BufferSize)
         {
            // The position in the file that the current buffer starts at.

            long buffer_offset = 0;
            ByteVector buffer;

            // These variables are used to keep track of a partial match that happens at
            // the end of a buffer.

            int previous_partial_match = -1;
            bool previous_partial_synch_match = false;

            // Save the location of the current read pointer.  We will restore the
            // position using seek() before all returns.

            long original_position = Tell;

            // Start the search at the beginning of the file.

            Seek (0);

            // This loop is the crux of the find method.  There are three cases that we
            // want to account for:
            // (1) The previously searched buffer contained a partial match of the search
            // pattern and we want to see if the next one starts with the remainder of
            // that pattern.
            //
            // (2) The search pattern is wholly contained within the current buffer.
            //
            // (3) The current buffer ends with a partial match of the pattern.  We will
            // note this for use in the next itteration, where we will check for the rest
            // of the pattern.

            for (buffer = ReadBlock ((int) BufferSize); buffer.Count > 0; buffer = ReadBlock ((int) BufferSize))
            {

               // (1) previous partial match

               if(previous_partial_synch_match && SecondSynchByte (buffer [0]))
                  return -1;

               if (previous_partial_match >= 0 && (int) BufferSize > previous_partial_match)
               {
                  int pattern_offset = (int) (BufferSize - previous_partial_match);
                  if (buffer.ContainsAt (Id3v2.Header.FileIdentifier, 0, pattern_offset))
                  {
                     Seek (original_position);
                     return buffer_offset - BufferSize + previous_partial_match;
                  }
               }

               // (2) pattern contained in current buffer

               long location = buffer.Find (Id3v2.Header.FileIdentifier);
               if (location >= 0)
               {
                  Seek (original_position);
                  return buffer_offset + location;
               }

               int first_synch_byte = buffer.Find ((byte) 0xFF);

               // Here we have to loop because there could be several of the first
               // (11111111) byte, and we want to check all such instances until we find
               // a full match (11111111 111) or hit the end of the buffer.

               while (first_synch_byte >= 0)
               {
                  // if this *is not* at the end of the buffer
                  
                  if (first_synch_byte < (int) buffer.Count - 1)
                  {
                     if (SecondSynchByte (buffer [first_synch_byte + 1]))
                     {
                        // We've found the frame synch pattern.
                        Seek (original_position);
                        return -1;
                     }
                     else
                     {
                        // We found 11111111 at the end of the current buffer indicating a
                        // partial match of the synch pattern.  The find() below should
                        // return -1 and break out of the loop.

                        previous_partial_synch_match = true;
                     }
                  }

                  // Check in the rest of the buffer.

                  first_synch_byte = buffer.Find ((byte) 0xFF, first_synch_byte + 1);
               }

               // (3) partial match

               previous_partial_match = buffer.EndsWithPartialMatch (Id3v2.Header.FileIdentifier);

               buffer_offset += BufferSize;
            }

            Seek (original_position);
         }
         
         return -1;
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

      private long FindApe (bool has_id3v1)
      {
         if (IsValid)
         {
            if (has_id3v1)
               Seek (-160, System.IO.SeekOrigin.End);
            else
               Seek (-32, System.IO.SeekOrigin.End);

            long p = Tell;

            if (ReadBlock (8) == Ape.Tag.FileIdentifier)
               return p;
         }
         return -1;
      }
      
      private bool SecondSynchByte (byte b)
      {
         return b >= 0xE0;
      }
   }
}
