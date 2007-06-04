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
   public enum Marker
   {
      Corrupt          = -1,
      Zero             = 0,
      SystemSyncPacket = 0xBA,
      VideoSyncPacket  = 0xB3,
      
      SystemPacket     = 0xBB,
      PaddingPacket    = 0xBE,
      AudioPacket      = 0xC0,
      VideoPacket      = 0xE0,
      
      EndOfStream      = 0xB9
   }
   
   [SupportedMimeType("taglib/mpg",  "mpg")]
   [SupportedMimeType("taglib/mpeg", "mpeg")]
   [SupportedMimeType("taglib/mpe",  "mpe")]
   [SupportedMimeType("taglib/mpv2", "mpv2")]
   [SupportedMimeType("video/x-mpg")]
   [SupportedMimeType("video/mpeg")]
   public class File : TagLib.NonContainer.File
   {
      private Version version;
      private AudioHeader audio_header;
      private VideoHeader video_header;
      private bool video_found = false;
      private bool audio_found = false;
      private double? start_time = null;
      private double end_time;
      
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
      
      public override TagLib.Tag GetTag (TagTypes type, bool create)
      {
         Tag t = (Tag as TagLib.NonContainer.Tag).GetTag (type);
         
         if (t != null || !create)
            return t;
         
         switch (type)
         {
         case TagTypes.Id3v1:
            return EndTag.AddTag (type, Tag);
         
         case TagTypes.Id3v2:
            return EndTag.AddTag (type, Tag);
         
         case TagTypes.Ape:
            return EndTag.AddTag (type, Tag);
         
         default:
            return null;
         }
      }
      
      private static readonly ByteVector MarkerStart = new byte [] {0, 0, 1};
      protected Marker GetMarker (long position)
      {
         Seek (position);
         ByteVector identifier = ReadBlock (4);
         
         if (identifier.Count == 4 && identifier.StartsWith (MarkerStart))
            return (Marker) identifier [3];
         
         throw new CorruptFileException ("Invalid marker at position " + position);
      }
      
      protected Marker FindMarker (ref long position)
      {
         position = Find (MarkerStart, position);
         if (position < 0)
            throw new CorruptFileException ("Marker not found");
         
         return GetMarker (position);
      }
      
      protected Marker FindMarker (ref long position, Marker marker)
      {
         ByteVector packet = new ByteVector (MarkerStart);
         packet.Add ((byte) marker);
         position = Find (packet, position);
         if (position < 0)
            throw new CorruptFileException ("Marker not found");
         
         return GetMarker (position);
      }
      
      protected Marker RFindMarker (ref long position, Marker marker)
      {
         ByteVector packet = new ByteVector (MarkerStart);
         packet.Add ((byte) marker);
         position = RFind (packet, position);
         if (position < 0)
            throw new CorruptFileException ("Marker not found");
         
         return GetMarker (position);
      }
      
      protected override void ReadStart (long start, ReadStyle propertiesStyle)
      {
         if (propertiesStyle == ReadStyle.None)
            return;
         
         FindMarker (ref start, Marker.SystemSyncPacket);
         ReadSystemFile (start);
      }
      
      protected override void ReadEnd (long end, ReadStyle propertiesStyle)
      {
         // Make sure we have ID3v1 and ID3v2 tags.
         GetTag (TagTypes.Id3v1, true);
         GetTag (TagTypes.Id3v2, true);
         
         if (propertiesStyle == ReadStyle.None || start_time == null)
            return;
         
         RFindMarker (ref end, Marker.SystemSyncPacket);
         
         end_time = ReadTimestamp (end + 4);
      }
      
      protected override TagLib.Properties ReadProperties (long start, long end, ReadStyle propertiesStyle)
      {
         TimeSpan duration = TimeSpan.FromSeconds (start_time == null ? 0d : (end_time - (double) start_time));
         return new Properties (duration, video_header, audio_header);
      }
      
      protected void ReadSystemFile (long position)
      {
         int sanity_limit = 100;
         
         for (int i = 0; i < sanity_limit && (start_time == null || !audio_found || !video_found); i ++)
         {
            Marker marker = FindMarker (ref position);
            
            switch (marker)
            {
            case Marker.SystemSyncPacket:
               ReadSystemSyncPacket (ref position);
               break;
            case Marker.SystemPacket:
            case Marker.PaddingPacket:
               Seek (position + 4);
               position += ReadBlock (2).ToUShort () + 6;
               break;
            case Marker.VideoPacket:
               ReadVideoPacket (ref position);
               break;
            case Marker.AudioPacket:
               ReadAudioPacket (ref position);
               break;
            case Marker.EndOfStream:
               return;
            default:
               position += 4;
               break;
            }
         }
      }
      
      void ReadAudioPacket (ref long position)
      {
         Seek (position + 4);
         int length = ReadBlock (2).ToUShort ();
         if (!audio_found)
            audio_found = AudioHeader.Find (out audio_header, this, position + 15, length - 9);
         position += length;
      }
      
      void ReadVideoPacket (ref long position)
      {
         Seek (position + 4);
         int length = ReadBlock (2).ToUShort ();
         
         long offset = position + 6;
         while (!video_found && offset < position + length)
            if (FindMarker (ref offset) == Marker.VideoSyncPacket)
            {
               video_header = new VideoHeader (this, offset + 4);
               video_found = true;
            }
            
         position += length;
      }
      
      void ReadSystemSyncPacket (ref long position)
      {
         int packet_size = 0;
         Seek (position + 4);
         byte version_info = ReadBlock (1) [0];
         if ((version_info & 0xF0) == 0x20)
         {
            version = Version.Version1;
            packet_size = 12;
         }
         else if ((version_info & 0xC0) == 0x40)
         {
            version = Version.Version2;
            Seek (position + 13);
            packet_size = 14 + (ReadBlock (1) [0] & 0x07);
         }
         else
            throw new UnsupportedFormatException ("Unknown MPEG version.");
         
         if (start_time == null)
            start_time = ReadTimestamp (position + 4);
         
         position += packet_size;
      }
      
      private double ReadTimestamp (long position)
      {
         double     high;
         uint       low;
         ByteVector data;
         
         Seek (position);
         
         if (version == Version.Version1)
         {
            data = ReadBlock (5);
            high = (double) ((data [0] >> 3) & 0x01);
            
            low = ((uint)((data [0] >> 1) & 0x03) << 30)
                |  (uint) (data [1] << 22)
                |  (uint)((data [2] >> 1) << 15)
                |  (uint) (data [3] << 7)
                |  (uint) (data [4] << 1);
         }
         else
         {
            data = ReadBlock (6);
            
            high = (double) ((data [0] & 0x20) >> 5);

            low = ((uint) ((data [0] & 0x18) >> 3) << 30)
                |  (uint) ((data [0] & 0x03) << 28)
                |  (uint)  (data [1] << 20)
                |  (uint) ((data [2] & 0xF8) << 12)
                |  (uint) ((data [2] & 0x03) << 13)
                |  (uint)  (data [3] << 5)
                |  (uint)  (data [4] >> 3);
         }
         
         return (((high * 0x10000) * 0x10000) + low) / 90000.0;
      }
   }
}
