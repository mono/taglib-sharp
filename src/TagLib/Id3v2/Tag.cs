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
using System.Collections.Generic;
using System;
using System.Globalization;

namespace TagLib.Id3v2
{
   public class Tag : TagLib.Tag, IEnumerable<Frame>
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Header         header = new Header ();
      private ExtendedHeader extended_header = null;

      private List<Frame>    frame_list = new List<Frame> ();
      
      private static string language = "XXX";
      private static byte default_version = 4;
      private static bool force_default_version = false;
      private static StringType default_string_type = StringType.UTF8;
      private static bool force_default_string_type = false;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Tag () : base ()
      {}
      
      public Tag (File file, long position) : base ()
      {
         Read (file, position);
      }

      public IEnumerator<Frame> GetEnumerator ()
      {
         return frame_list.GetEnumerator ();
      }
      
      IEnumerator IEnumerable.GetEnumerator ()
      {
         return frame_list.GetEnumerator ();
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
      
      public void ReplaceFrame (Frame oldFrame, Frame newFrame)
      {
         if (oldFrame == newFrame)
            return;
         
         int i = frame_list.IndexOf (oldFrame);
         if (i >= 0)
            frame_list [i] = newFrame;
         else
            frame_list.Add (newFrame);
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
         
         header.MajorVersion = (header.Flags & HeaderFlags.FooterPresent) != 0 ? (byte)4 : Version;
         
         ByteVector tag_data = new ByteVector ();

         // TODO: Render the extended header.
         header.Flags &= ~HeaderFlags.ExtendedHeader;

         // Loop through the frames rendering them and adding them to the tagData.
         foreach (Frame frame in frame_list)
            if ((frame.Flags & FrameFlags.TagAlterPreservation) == 0)
               try
               {
                  tag_data.Add (frame.Render (header.MajorVersion));
               }
               catch (NotImplementedException) {};
         
         // Add unsyncronization bytes if necessary.
         if ((header.Flags & HeaderFlags.Unsynchronisation) != 0)
            SynchData.UnsynchByteVector (tag_data);
         
         // Compute the amount of padding, and append that to tagData.

         uint padding_size = 0;
         uint original_size = header.TagSize;

         if ((header.Flags & HeaderFlags.FooterPresent) == 0)
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
         if ((header.Flags & HeaderFlags.FooterPresent) != 0)
            tag_data.Add (new Footer (header).Render ());
         
         return tag_data;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TagTypes TagTypes {get {return TagTypes.Id3v2;}}
      
      public override string Title
      {
         get
         {
            TextInformationFrame f = TextInformationFrame.Get (this, FrameType.TIT2);
            return f == null ? null : f.ToString ();
         }
         set
         {
            SetTextFrame (FrameType.TIT2, value);
         }
      }
      
      public override string [] AlbumArtists
      {
         get
         {
            TextInformationFrame f = TextInformationFrame.Get (this, FrameType.TPE2);
            return f == null ? new string [] {} : f.FieldList.ToArray ();
         }
         set
         {
            SetTextFrame (FrameType.TPE2, new StringCollection (value));
         }
      }
      
      public override string [] Performers
      {
         get
         {
            TextInformationFrame f = TextInformationFrame.Get (this, FrameType.TPE1);
            return f == null ? new string [] {} : f.FieldList.ToArray ();
         }
         set
         {
            SetTextFrame (FrameType.TPE1, new StringCollection (value));
         }
      }
      
      public override string [] Composers
      {
         get
         {
            TextInformationFrame f = TextInformationFrame.Get (this, FrameType.TCOM);
            return f == null ? new string [] {} : f.FieldList.ToArray ();
         }
         set
         {
            SetTextFrame (FrameType.TCOM, new StringCollection (value));
         }
      }
      
      public override string Album
      {
         get
         {
            TextInformationFrame f = TextInformationFrame.Get (this, FrameType.TALB);
            return f == null ? null : f.ToString ();
         }
         set
         {
            SetTextFrame (FrameType.TALB, value);
         }
      }
      
      public override string Comment
      {
         get
         {
            CommentsFrame f = CommentsFrame.GetPreferred (this, String.Empty, Language);
            return f != null ? f.ToString () : null;
         }
         set
         {
            CommentsFrame.Get (this, String.Empty, Language, true).Text = value;
         }
      }
      
      public override string [] Genres
      {
         get
         {
            TextInformationFrame f = TextInformationFrame.Get (this, FrameType.TCON);
            if (f != null)
            {
               StringCollection l = new StringCollection ();

               foreach (string genre in f.FieldList)
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
            if (value != null)
               for (int i = 0; i < value.Length; i ++)
               {
                  int index = TagLib.Genres.AudioToIndex (value [i]);
                  if (index != 255)
                     value [i] = index.ToString (CultureInfo.InvariantCulture);
               }
            
            SetTextFrame (FrameType.TCON, new StringCollection (value));
         }
      }
      
      public override uint Year
      {
         get
         {
            TextInformationFrame f = TextInformationFrame.Get (this, FrameType.TDRC);
            if (f == null)
               return 0;
            
            uint value;
            string text = f.ToString ();
            if (text.Length > 3 && uint.TryParse (text.Substring (0, 4), out value))
               return value;
            
            return 0;
         }
         set
         {
            SetNumberFrame (FrameType.TDRC, value, 0);
         }
      }
      
      public override uint Track
      {
         get
         {
            TextInformationFrame f = TextInformationFrame.Get (this, FrameType.TRCK);
            if (f == null)
               return 0;
            
            string [] values;
            uint value;
            
            if ((values = f.ToString ().Split ('/')).Length > 0 && uint.TryParse (values [0], out value))
               return value;
            
            return 0;
         }
         set
         {
            SetNumberFrame (FrameType.TRCK, value, TrackCount);
         }
      }
      
      public override uint TrackCount
      {
         get
         {
            TextInformationFrame f = TextInformationFrame.Get (this, FrameType.TRCK);
            if (f == null)
               return 0;
            
            string [] values;
            uint value;
            
            if ((values = f.ToString ().Split ('/')).Length > 1 && uint.TryParse (values [1], out value))
               return value;
            
            return 0;
         }
         set
         {
            SetNumberFrame (FrameType.TRCK, Track, value);
         }
      }
      
      public override uint Disc
      {
         get
         {
            TextInformationFrame f = TextInformationFrame.Get (this, FrameType.TPOS);
            if (f == null)
               return 0;
            
            string [] values;
            uint value;
            
            if ((values = f.ToString ().Split ('/')).Length > 0 && uint.TryParse (values [0], out value))
               return value;
            
            return 0;
         }
         set
         {
            SetNumberFrame (FrameType.TPOS, value, DiscCount);
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            TextInformationFrame f = TextInformationFrame.Get (this, FrameType.TPOS);
            if (f == null)
               return 0;
            
            string [] values;
            uint value;
            
            if ((values = f.ToString ().Split ('/')).Length > 1 && uint.TryParse (values [1], out value))
               return value;
            
            return 0;
         }
         set
         {
            SetNumberFrame (FrameType.TPOS, Disc, value);
         }
      }
      
      public override IPicture [] Pictures {
         get { 
            List<AttachedPictureFrame> frames = new List<AttachedPictureFrame> ();
            foreach (Frame f in GetFrames(FrameType.APIC))
               if (f is AttachedPictureFrame)
                  frames.Add (f as AttachedPictureFrame);
            
            return frames.Count > 0 ? frames.ToArray () : base.Pictures;
         }
         
         set {
            RemoveFrames(FrameType.APIC);
            
            if(value == null || value.Length < 1) {
               return;
            }
            
            foreach(IPicture picture in value) {
               if (picture is AttachedPictureFrame)
                  AddFrame (picture as AttachedPictureFrame);
               else
                  AddFrame(new AttachedPictureFrame(picture));
            }
         }
      }
      
      public override string Lyrics
      {
         get
         {
            UnsynchronisedLyricsFrame f = UnsynchronisedLyricsFrame.GetPreferred (this, String.Empty, Language);
            return f != null ? f.ToString () : null;
         }
         set
         {
            UnsynchronisedLyricsFrame.Get (this, String.Empty, Language, true).Text = value;
         }
      }
      
      public override uint BeatsPerMinute
      {
         get
         {
            uint value;
            foreach (TextInformationFrame f in GetFrames(FrameType.TBPM))
                if(uint.TryParse(f.ToString(), out value))
                    return value;
           
            return 0;
         }
         set
         {
            SetNumberFrame (FrameType.TBPM, value, 0);
         }
      }
      
      public override string Grouping
      {
         get
         {
            foreach (TextInformationFrame f in GetFrames (FrameType.TIT1))
               return f.ToString ();
            return null;
         }
         set
         {
            SetTextFrame (FrameType.TIT1, value);
         }
      }
      
      public override string Conductor
      {
         get
         {
            foreach (TextInformationFrame f in GetFrames (FrameType.TPE3))
               return f.ToString ();
            return null;
         }
         set
         {
             SetTextFrame(FrameType.TPE3, value);
         }
      }
      
      public override string Copyright
      {
         get
         {
            foreach (TextInformationFrame f in GetFrames (FrameType.TCOP))
               return f.ToString ();
            return null;
         }
         set
         {
             SetTextFrame(FrameType.TCOP, value);
         }
      }
      
      public override bool IsEmpty {get {return frame_list.Count == 0;}}
      
      //////////////////////////////////////////////////////////////////////////
      // public static properties
      //////////////////////////////////////////////////////////////////////////
      public static string Language
      {
         get {return language;}
         set {language = (value == null || value.Length < 3) ? "XXX" : value.Substring (0,3);}
      }
      
      public byte Version
      {
         get {return ForceDefaultVersion ? DefaultVersion : header.MajorVersion;}
         set
         {
            header.MajorVersion = value;
         }
      }
      
      public static byte DefaultVersion
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
      
      public HeaderFlags Flags {get {return header.Flags;} set {header.Flags = value;}}
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected void Read (TagLib.File file, long position)
      {
         if (file == null)
            return;
         
         file.Mode = File.AccessMode.Read;
         
         file.Seek (position);
         header = new Header (file.ReadBlock ((int) Header.Size));
         
         // if the tag size is 0, then this is an invalid tag (tags must contain
         // at least one frame)
         
         if(header.TagSize == 0)
            return;
         
         Parse (file.ReadBlock ((int) header.TagSize));
      }
      
      protected void Parse (ByteVector data)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         // If the entire tag is marked as unsynchronized, resynchronize it.
         if ((header.Flags & HeaderFlags.Unsynchronisation) != 0)
            SynchData.ResynchByteVector (data);
         
         int frame_data_position = 0;
         int frame_data_length = data.Count;

         // Check for the extended header.
         if ((header.Flags & HeaderFlags.ExtendedHeader) != 0)
         {
            extended_header = new ExtendedHeader (data, header.MajorVersion);
            
            if (extended_header.Size <= data.Count)
            {
               frame_data_position += (int) extended_header.Size;
               frame_data_length -= (int) extended_header.Size;
            }
         }

         // Parse the frames. TDRC, TDAT, and TIME will be needed for post-
         // processing, so check for them as they are loaded.
         TextInformationFrame tdrc = null;
         TextInformationFrame tdat = null;
         TextInformationFrame time = null;
         
         while (frame_data_position < frame_data_length - FrameHeader.Size (header.MajorVersion))
         {
            // If the next data is position is 0, assume that we've hit the padding
            // portion of the frame data.
            if(data [frame_data_position] == 0)
               break;
            
            try
            {
               Frame frame = FrameFactory.CreateFrame (data,
                  ref frame_data_position, header.MajorVersion);
               
               if(frame == null)
                  break;
               
               // Only add frames that contain data.
               if (frame.Size == 0)
                  continue;
               
               AddFrame (frame);
               
               // If the tag is version 4, no post-processing is needed.
               if (header.MajorVersion == 4)
                  continue;
               
               // Load up the first instance of each, for post-processing.
               if (tdrc == null && frame.FrameId == FrameType.TDRC)
                  tdrc = frame as TextInformationFrame;
               else if (tdat == null && frame.FrameId == FrameType.TDAT)
                  tdat = frame as TextInformationFrame;
               else if (time == null && frame.FrameId == FrameType.TIME)
                  time = frame as TextInformationFrame;
            }
            catch (NotImplementedException) {}
         }
         
         // Post-processing: Combine the three frames into one TDRC frame.
         if (tdrc != null && tdat != null) {
            string tdrc_text = tdrc.ToString ();
            string tdat_text = tdat.ToString ();
            if (tdat_text.Length == 4 && tdat_text.Length == 4) {
               tdrc_text += "-" + tdat_text.Substring (0, 2) + "-" + 
                  tdat_text.Substring (2, 2);
               
               if (time != null) {
                  string time_text = time.ToString ();
                  if (time_text.Length == 4) {
                     tdrc_text += "T" + time_text.Substring (0, 2) + ":" + 
                        time_text.Substring (2, 2);
                  }
               }
               tdrc.SetText (tdrc_text);
            }
         }
         
         if (tdat != null)
            RemoveFrames (FrameType.TDAT);
         
         if (time != null)
            RemoveFrames (FrameType.TIME);
      }
      
      public void SetTextFrame (ByteVector id, params string [] value)
      {
         SetTextFrame (id, new StringCollection (value));
      }
      
      public void SetTextFrame (ByteVector id, StringCollection value)
      {
         if (value == null)
         {
            RemoveFrames (id);
            return;
         }
         
         for (int i = value.Count - 1; i >= 0; i --)
            if (value [i] == null || value [i].Trim ().Length == 0)
               value.RemoveAt (i);
         
         if (value.Count == 0)
            RemoveFrames (id);
         else
            TextInformationFrame.Get (this, id, true).SetText (value);
      }
      
      public void SetNumberFrame (ByteVector id, uint number, uint count)
      {
         if (number == 0 && count == 0)
            RemoveFrames (id);
         else if (count != 0)
         {
            SetTextFrame (id, string.Format (CultureInfo.InvariantCulture, "{0}/{1}", number, count));
         }
         else
            SetTextFrame (id, number.ToString (CultureInfo.InvariantCulture));
      }
      
      public override void Clear ()
      {
         frame_list.Clear ();
      }
   }
}
