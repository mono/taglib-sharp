/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2tag.cpp from TagLib
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

namespace TagLib.Id3v2
{
   public class Tag : TagLib.Tag
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private long           tag_offset;
      
      private Header         header;
      private ExtendedHeader extended_header;
      private Footer         footer;

      private List<Frame>    frame_list;
      
      private static ByteVector language = "eng";
      private static uint default_version = 4;
      private static bool force_default_version = false;
      private static StringType default_string_type = StringType.UTF8;
      private static bool force_default_string_type = false;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Tag () : base ()
      {
         tag_offset      = -1;
         header          = new Header ();
         extended_header = null;
         footer          = null;
         frame_list      = new List<Frame> ();
      }
      
      public Tag (File file, long tag_offset) : this ()
      {
         this.tag_offset = tag_offset;
         
         Read (file);
      }

      public IEnumerable<Frame> GetFrames ()
      {
         return frame_list;
      }

      public IEnumerable<Frame> GetFrames (ByteVector id)
      {
         foreach (Frame f in frame_list)
            if (f.FrameId == id)
               yield return f;
      }
      
      public void AddFrame (Frame frame)
      {
        frame_list.Add (frame);
      }
      
      public void ReplaceFrame (Frame old_frame, Frame new_frame)
      {
         if (old_frame == new_frame)
            return;
         
         int i = frame_list.IndexOf (old_frame);
         if (i >= 0)
            frame_list [i] = new_frame;
         else
            frame_list.Add (new_frame);
      }
      
      public void RemoveFrame (Frame frame)
      {
         if (frame_list.Contains (frame))
            frame_list.Remove (frame);
      }
      
      public void RemoveFrames (ByteVector id)
      {
         for (int i = frame_list.Count - 1; i >= 0; i --)
         {
            Frame f = (Frame) frame_list [i];
            if (f.FrameId == id)
               RemoveFrame (f);
         }
      }
      
      public ByteVector Render ()
      {
         // We need to render the "tag data" first so that we have to correct size to
         // render in the tag's header.  The "tag data" -- everything that is included
         // in ID3v2::Header::tagSize() -- includes the extended header, frames and
         // padding, but does not include the tag's header or footer.
         
         header.MajorVersion = header.FooterPresent ? 4 : Version;
         
         ByteVector tag_data = new ByteVector ();

         // TODO: Render the extended header.
         header.ExtendedHeader = false;

         // Loop through the frames rendering them and adding them to the tagData.
         foreach (Frame frame in frame_list)
            if (!frame.Header.TagAlterPreservation)
               tag_data.Add (frame.Render (header.MajorVersion));
         
         // Add unsyncronization bytes if necessary.
         if (header.Unsynchronisation)
            tag_data = SynchData.UnsynchByteVector (tag_data);
         
         // Compute the amount of padding, and append that to tagData.

         uint padding_size = 0;
         uint original_size = header.TagSize;

         if (!header.FooterPresent)
         {
            if (tag_data.Count < original_size)
               padding_size = (uint) (original_size - tag_data.Count);
            else
               padding_size = 1024;
            
            tag_data.Add (new ByteVector ((int) padding_size));
         }
         
         // Set the tag size.
         header.TagSize = (uint) tag_data.Count;
         
         tag_data.Insert (0, header.Render ());
         if (header.FooterPresent)
         {
            footer = new Footer (header);
            tag_data.Add (footer.Render ());
         }
         
         return tag_data;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override string Title
      {
         get
         {
            foreach (TextIdentificationFrame f in GetFrames ("TIT2"))
               return f.ToString ();
            return null;
         }
         set
         {
            SetTextFrame ("TIT2", value);
         }
      }
      
      public override string [] AlbumArtists
      {
         get
         {
            foreach (TextIdentificationFrame f in GetFrames ("TPE1"))
               return f.FieldList.ToArray ();
            return new string [] {};
         }
         set
         {
            SetTextFrame ("TPE1", new StringList (value));
         }
      }
      
      public override string [] Performers
      {
         get
         {
            foreach (TextIdentificationFrame f in GetFrames ("TPE2"))
               return f.FieldList.ToArray ();
            return new string [] {};
         }
         set
         {
            SetTextFrame ("TPE2", new StringList (value));
         }
      }
      
      public override string [] Composers
      {
         get
         {
            foreach (TextIdentificationFrame f in GetFrames ("TCOM"))
               return f.FieldList.ToArray ();
            return new string [] {};
         }
         set
         {
            SetTextFrame ("TCOM", new StringList (value));
         }
      }
      
      public override string Album
      {
         get
         {
            foreach (TextIdentificationFrame f in GetFrames ("TALB"))
               return f.ToString ();
            return null;
         }
         set
         {
            SetTextFrame ("TALB", value);
         }
      }
      
      public override string Comment
      {
         get
         {
            CommentsFrame f = CommentsFrame.GetPreferred (this, String.Empty, Language, false);
            return f != null ? f.ToString () : null;
         }
         set
         {
            CommentsFrame f = CommentsFrame.GetPreferred (this, String.Empty, Language, true);
            f.SetText (value);
         }
      }
      
      public override string [] Genres
      {
         get
         {
            foreach (TextIdentificationFrame frame in GetFrames ("TCON"))
            {
               StringList l = new StringList ();

               foreach (string genre in frame.FieldList)
               {
                  if (genre == null)
                     continue;
                  
                  // The string may just be a genre number.
                  string genre_from_index = TagLib.Genres.IndexToAudio (genre);
                  
                  if (genre_from_index != null)
                     l.Add (genre_from_index);
                  else
                     l.Add (genre);
               }
               
               return l.ToArray ();
            }
            
            return new string [0];
         }
         set
         {
            for (int i = 0; i < value.Length; i ++)
            {
               int index = TagLib.Genres.AudioToIndex (value [i]);
               if (index != 255)
                  value [i] = index.ToString ();
            }
            
            SetTextFrame ("TCON", new StringList (value));
         }
      }
      public override uint Year
      {
         get
         {
            uint value;
            foreach (TextIdentificationFrame f in GetFrames ("TDRC"))
            {
               string text = f.ToString ();
               if (text.Length > 3 && uint.TryParse (text.Substring (0, 4), out value))
                  return value;
            }
            
            return 0;
         }
         set
         {
            SetNumberFrame ("TDRC", value, 0);
         }
      }
      
      public override uint Track
      {
         get
         {
            string [] values;
            uint value;
            foreach (TextIdentificationFrame f in GetFrames ("TRCK"))
               if ((values = f.ToString ().Split ('/')).Length > 0 && uint.TryParse (values [0], out value))
                  return value;
            
            return 0;
         }
         set
         {
            SetNumberFrame ("TRCK", value, TrackCount);
         }
      }
      
      public override uint TrackCount
      {
         get
         {
            string [] values;
            uint value;
            foreach (TextIdentificationFrame f in GetFrames ("TRCK"))
               if ((values = f.ToString ().Split ('/')).Length > 1 && uint.TryParse (values [1], out value))
                  return value;
            
            return 0;
         }
         set
         {
            SetNumberFrame ("TRCK", Track, value);
         }
      }
      
      public override uint Disc
      {
         get
         {
            string [] values;
            uint value;
            foreach (TextIdentificationFrame f in GetFrames ("TPOS"))
               if ((values = f.ToString ().Split ('/')).Length > 0 && uint.TryParse (values [0], out value))
                  return value;
            
            return 0;
         }
         set
         {
            SetNumberFrame ("TPOS", value, DiscCount);
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            string [] values;
            uint value;
            foreach (TextIdentificationFrame f in GetFrames ("TPOS"))
               if ((values = f.ToString ().Split ('/')).Length > 1 && uint.TryParse (values [1], out value))
                  return value;
            
            return 0;
         }
         set
         {
            SetNumberFrame ("TPOS", Disc, value);
         }
      }
      
      public override IPicture [] Pictures {
         get { 
            List<AttachedPictureFrame> frames = new List<AttachedPictureFrame> ();
            foreach (Frame f in GetFrames("APIC"))
               frames.Add (f as AttachedPictureFrame);
            
            return frames.Count > 0 ? frames.ToArray () : base.Pictures;
         }
         
         set {
            if(value == null || value.Length < 1) {
               return;
            }
            
            RemoveFrames("APIC");
            
            foreach(IPicture picture in value) {
               AddFrame(new AttachedPictureFrame(picture));
            }
         }
      }
      
      
      public override string Lyrics
      {
         get
         {
            UnsynchronisedLyricsFrame f = UnsynchronisedLyricsFrame.GetPreferred (this, String.Empty, Language, false);
            return f != null ? f.ToString () : null;
         }
         set
         {
            UnsynchronisedLyricsFrame f = UnsynchronisedLyricsFrame.GetPreferred (this, String.Empty, Language, true);
            f.SetText (value);
         }
      }
      
      public override bool IsEmpty {get {return frame_list.Count == 0;}}

      public Header         Header         {get {return header;}}
      public ExtendedHeader ExtendedHeader {get {return extended_header;}}
      public Footer         Footer         {get {return footer;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // public static properties
      //////////////////////////////////////////////////////////////////////////
      public static ByteVector Language
      {
         get {return language;}
         set {language = (value == null || value.Count < 3) ? "XXX" : value.Mid (0,3);}
      }
      
      public uint Version
      {
         get {return ForceDefaultVersion ? DefaultVersion : header.MajorVersion;}
         set
         {
            header.MajorVersion = value;
         }
      }
      
      public static uint DefaultVersion
      {
         get {return default_version;}
         set
         {
            if (value < 2 || value > 4)
               throw new ArgumentException ("Unsupported version");
            
            default_version = value;
         }
      }
      
      public static bool ForceDefaultVersion
      {
         get {return force_default_version;}
         set {force_default_version = value;}
      }

      public static StringType DefaultEncoding
      {
         get {return default_string_type;}
         set {default_string_type = value;}
      }
      
      public static bool ForceDefaultEncoding
      {
         get {return force_default_string_type;}
         set {force_default_string_type = value;}
      }
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected void Read (TagLib.File file)
      {
         if (file == null)
            return;
         
         file.Mode = File.AccessMode.Read;
         
         file.Seek (tag_offset);
         header.SetData (file.ReadBlock ((int) Header.Size));

         // if the tag size is 0, then this is an invalid tag (tags must contain
         // at least one frame)

         if(header.TagSize == 0)
            return;

         Parse (file.ReadBlock ((int) header.TagSize));
      }
      
      protected void Parse (ByteVector data)
      {
         if (header.Unsynchronisation)
            data = SynchData.ResynchByteVector (data);
         
         int frame_data_position = 0;
         int frame_data_length = data.Count;

         // check for extended header
         if (header.ExtendedHeader)
         {
            if (ExtendedHeader == null)
               extended_header = new ExtendedHeader ();
            
            ExtendedHeader.SetData (data);
            
            if(ExtendedHeader.Size <= data.Count)
            {
               frame_data_position += (int) ExtendedHeader.Size;
               frame_data_length -= (int) ExtendedHeader.Size;
            }
         }

         // parse frames

         // Make sure that there is at least enough room in the remaining frame data for
         // a frame header.
         
         while (frame_data_position < frame_data_length - FrameHeader.Size (header.MajorVersion))
         {
            // If the next data is position is 0, assume that we've hit the padding
            // portion of the frame data.
            if(data [frame_data_position] == 0)
               return;

            Frame frame = FrameFactory.CreateFrame (data, frame_data_position, header.MajorVersion);

            if(frame == null)
               return;

            // Checks to make sure that frame parsed correctly.
            if(frame.Size < 0)
               return;

            frame_data_position += (int) (frame.Size + FrameHeader.Size (header.MajorVersion));            
            // Only add frames with content so we don't send out just we got in.
            if (frame.Size > 0)
               AddFrame (frame);
         }
      }
      
      public void SetTextFrame (ByteVector id, string value)
      {
         if (value == null || value == string.Empty)
         {
            RemoveFrames (id);
            return;
         }
         
         SetTextFrame (id, new StringList (value));
      }
      
      public void SetTextFrame (ByteVector id, StringList value)
      {
         if (value == null || value.Count == 0)
         {
            RemoveFrames (id);
            return;
         }
         
         bool no_frames = true;
         foreach (TextIdentificationFrame frame in GetFrames (id))
         {
            // There should only be one of each text frame, per the specification.
            if (no_frames)
               frame.SetText (value);
            else
               RemoveFrame (frame);
            
            no_frames = false;
         }
         
         if (no_frames)
         {
            TextIdentificationFrame f = new TextIdentificationFrame (id, Tag.DefaultEncoding);
            AddFrame (f);
            f.SetText (value);
         }
      }
      
      public void SetNumberFrame (ByteVector id, uint number, uint count)
      {
         if (number == 0 && count == 0)
            this.RemoveFrames (id);
         else if (count != 0)
            SetTextFrame (id, number.ToString () + "/" + count.ToString ());
         else
            SetTextFrame (id, number.ToString ());
      }
   }
}
