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

using System.Collections;
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

      private ArrayList      frame_list;
      
      private static ByteVector language = "eng";
      private static uint render_version = 4;
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Tag () : base ()
      {
         tag_offset      = -1;
         header          = new Header ();
         extended_header = null;
         footer          = null;
         frame_list      = new ArrayList ();
      }
      
      public Tag (File file, long tag_offset) : this ()
      {
         this.tag_offset = tag_offset;
         
         Read (file);
      }

      public Frame [] GetFrames ()
      {
         return (Frame []) frame_list.ToArray (typeof (Frame));
      }

      public Frame [] GetFrames (ByteVector id)
      {
         ArrayList l = new ArrayList ();
         foreach (Frame f in frame_list)
            if (f.FrameId == id)
               l.Add (f);
         
         return (Frame []) l.ToArray (typeof (Frame));
      }

      public void AddFrame (Frame frame)
      {
        frame_list.Add (frame);
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

      public ByteVector Render (uint version)
      {
         // We need to render the "tag data" first so that we have to correct size to
         // render in the tag's header.  The "tag data" -- everything that is included
         // in ID3v2::Header::tagSize() -- includes the extended header, frames and
         // padding, but does not include the tag's header or footer.
         
         header.MajorVersion = version;
         
         ByteVector tag_data = new ByteVector ();

         // TODO: Render the extended header.
         header.ExtendedHeader = false;

         // Loop through the frames rendering them and adding them to the tagData.
         foreach (Frame frame in frame_list)
            if (!frame.Header.TagAlterPreservation)
               tag_data.Add (frame.Render (version));
         
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
      
      public ByteVector Render (bool end_tag)
      {
         // End tags need a header.
         if (end_tag)
            header.FooterPresent = true;
         
         // Make sure footers are rendered properly.
         return Render ((end_tag && RenderVersion < 4) ? 4 : RenderVersion);
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
            // This is weird, so bear with me. The best thing we can have is 
            // something straightforward and in our own language. If it has a 
            // description, then it is probably used for something other than
            // an actual comment. If that doesn't work, we'd still rather have 
            // something in our language than something in another. After that
            // all we have left are things in other languages, so we'd rather 
            // have one with actual content, so we try to get one with no 
            // description first.
            Frame [] frames = GetFrames ("COMM");
            
            foreach (CommentsFrame f in frames)
               if (f.Description == String.Empty && f.Language == Language)
                  return f.ToString ();
            
            foreach (CommentsFrame f in frames)
               if (f.Language == Language)
                  return f.ToString ();
            
            foreach (CommentsFrame f in frames)
               if (f.Description == String.Empty)
                  return f.ToString ();
            
            foreach (CommentsFrame f in frames)
               return f.ToString ();
            
            return null;
         }
         set
         {
            if (value == null || value == string.Empty)
            {
               RemoveFrames ("COMM");
               return;
            }
            
            // See above.
            Frame [] frames = GetFrames ("COMM");
            
            foreach (CommentsFrame f in frames)
               if (f.Description == String.Empty && f.Language == Language)
               {
                  f.SetText (value);
                  return;
               }
            
            foreach (CommentsFrame f in frames)
               if (f.Language == Language)
               {
                  f.SetText (value);
                  return;
               }
            
            foreach (CommentsFrame f in frames)
               if (f.Description == String.Empty)
               {
                  f.SetText (value);
                  return;
               }
            
            foreach (CommentsFrame f in frames)
            {
               f.SetText (value);
               return;
            }
            
            // There were absolutely no comment frames. Let's add one in our
            // language.
            CommentsFrame frame = new CommentsFrame (FrameFactory.DefaultTextEncoding);
            frame.Language = Language;
            frame.SetText (value);
            AddFrame (frame);
         }
      }
      
      public override string [] Genres
      {
         get
         {
            ArrayList l = new ArrayList ();
            
            Frame [] frames = GetFrames ("TCON");
            TextIdentificationFrame frame;
            if (frames.Length != 0 && (frame = (TextIdentificationFrame) frames [0]) != null)
            {
               StringList fields = frame.FieldList;

               foreach (string s in fields)
               {
                  byte value;
                  
                  if (s == null)
                     continue;
                  
                  // The string may just be a genre number.
                  if (byte.TryParse (s, out value))
                  {
                     l.Add (Id3v1.GenreList.Genre (value));
                     continue;
                  }
                  
                  // Check for ID3v2.3 style (#)
                  int closing = s.IndexOf (')');
                  if (closing > 0 && s[0] == '(' && byte.TryParse (s.Substring (1, closing - 1), out value))
                  {
                     l.Add (Id3v1.GenreList.Genre (value));
                     continue;
                  }
                  
                  // We don't have a number, add the string.
                  l.Add (s);
               }
            }
            
            return (string []) l.ToArray (typeof (string));
         }
         set
         {
            for (int i = 0; i < value.Length; i ++)
            {
               int index = Id3v1.GenreList.GenreIndex (value [i]);
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
            Frame [] raw_frames = GetFrames("APIC");
            if(raw_frames == null || raw_frames.Length == 0) {
               return base.Pictures;
            }
            
            AttachedPictureFrame [] frames = new AttachedPictureFrame[raw_frames.Length];
            for(int i = 0; i < frames.Length; i++) {
                frames[i] = (AttachedPictureFrame)raw_frames[i];
            }
            
            return frames;
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
            // SEE Comment property for explanation.
            Frame [] frames = GetFrames ("USLT");
            
            foreach (UnsynchronisedLyricsFrame f in frames)
               if (f.Description == String.Empty && f.Language == Language)
                  return f.ToString ();
            
            foreach (UnsynchronisedLyricsFrame f in frames)
               if (f.Language == Language)
                  return f.ToString ();
            
            foreach (UnsynchronisedLyricsFrame f in frames)
               if (f.Description == String.Empty)
                  return f.ToString ();
            
            foreach (UnsynchronisedLyricsFrame f in frames)
               return f.ToString ();
            
            return null;
         }
         set
         {
            if (value == null || value == string.Empty)
            {
               RemoveFrames ("USLT");
               return;
            }
            
            // See above.
            Frame [] frames = GetFrames ("USLT");
            
            foreach (UnsynchronisedLyricsFrame f in frames)
               if (f.Description == String.Empty && f.Language == Language)
               {
                  f.SetText (value);
                  return;
               }
            
            foreach (UnsynchronisedLyricsFrame f in frames)
               if (f.Language == Language)
               {
                  f.SetText (value);
                  return;
               }
            
            foreach (UnsynchronisedLyricsFrame f in frames)
               if (f.Description == String.Empty)
               {
                  f.SetText (value);
                  return;
               }
            
            foreach (UnsynchronisedLyricsFrame f in frames)
            {
               f.SetText (value);
               return;
            }
            
            // There were absolutely no lyrics frames. Let's add one in our
            // language.
            UnsynchronisedLyricsFrame frame = new UnsynchronisedLyricsFrame (FrameFactory.DefaultTextEncoding);
            frame.Language = Language;
            frame.SetText (value);
            AddFrame (frame);
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
      
      public static uint RenderVersion
      {
         get {return render_version;}
         set
         {
            if (value < 2 || value > 4)
               throw new ArgumentException ("Unsupported version");
            
            render_version = value;
         }
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

         // check for footer -- we don't actually need to parse it, as it *must*
         // contain the same data as the header, but we do need to account for its
         // size.

         if(header.FooterPresent && Footer.Size <= frame_data_length)
            frame_data_length -= (int) Footer.Size;

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
         
         Frame [] frames = GetFrames (id);
         if (frames.Length != 0)
         {
            bool first = true;
            foreach (TextIdentificationFrame frame in frames)
            {
               // There should only be one of each text frame, per the specification.
               if (first)
                  frame.SetText (value);
               else
                  RemoveFrame (frame);
               
               first = false;
            }
         }
         else
         {
            TextIdentificationFrame f = new TextIdentificationFrame (id, FrameFactory.DefaultTextEncoding);
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
