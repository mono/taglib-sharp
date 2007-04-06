/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : mpegproperties.cpp from TagLib
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
   public class Properties : TagLib.Properties
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private File        file;
      private TimeSpan    duration;
      private int         bitrate;
      private int         sample_rate;
      private int         channels;
      private Version     version;
      private int         layer;
      private ChannelMode channel_mode;
      private bool        is_copyrighted;
      private bool        is_original;
  
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Properties (File file, Header first_header, ReadStyle style) : base (style)
      {
         this.file      = file;
         duration       = TimeSpan.Zero;
         bitrate        = 0;
         sample_rate    = 0;
         channels       = 0;
         version        = Version.Version1;
         layer          = 0;
         channel_mode   = ChannelMode.Stereo;
         is_copyrighted = false;
         is_original    = false;
         
         Read (first_header);
      }
      
      public Properties (File file, Header first_header) : this (file, first_header, ReadStyle.Average) {}
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      
      public override TimeSpan    Duration      {get {return duration;}}
      public override int         AudioBitrate       {get {return bitrate;}}
      public override int         AudioSampleRate    {get {return sample_rate;}}
      public override int         AudioChannels      {get {return channels;}}
      public override MediaTypes MediaTypes  {get {return MediaTypes.Audio;}}
      public          Version     Version       {get {return version;}}
      public          int         Layer         {get {return layer;}}
      public          ChannelMode ChannelMode   {get {return channel_mode;}}
      public          bool        IsCopyrighted {get {return is_copyrighted;}}
      public          bool        IsOriginal    {get {return is_original;}}


      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////

      private void Read (Header first_header)
      {
         // If the header doesn't exist, the file is bad.
         if (first_header == null)
            throw new CorruptFileException ("First MPEG header could not be found.");
         
         try
         {
            // Check for a Xing header that will help us in gathering
            // information about a VBR stream.
            long xing_header_position = first_header.Position +
               XingHeader.XingHeaderOffset (first_header.Version, first_header.ChannelMode);
         
            file.Seek (xing_header_position);
            XingHeader xing_header = new XingHeader (file.ReadBlock (16));

            // Read the length and the bitrate from the Xing header.

            int [,] block_size = new int [3,4] {
               { 0, 384, 1152, 1152 }, // Version 1
               { 0, 384, 1152, 576 },  // Version 2
               { 0, 384, 1152, 576 }   // Version 2.5
            };
            
            double time_per_frame = (double) block_size [(int) first_header.Version, first_header.Layer] / (double) first_header.SampleRate;
            duration = new TimeSpan((int)(time_per_frame * xing_header.TotalFrames) * TimeSpan.TicksPerSecond);
            bitrate = (int) (duration > TimeSpan.Zero ? ((xing_header.TotalSize * 8L) / duration.TotalSeconds) / 1000 : 0);
         }
         catch
         {
            // Since there was no valid Xing header found, we hope that we're
            // in a constant bitrate file.

            // TODO: Make this more robust with audio property detection for VBR
            // without a Xing header.

            if (first_header.FrameLength > 0 && first_header.Bitrate > 0)
            {
               int frames = (int) ((file.Length - first_header.Position) / first_header.FrameLength + 1);
               duration = TimeSpan.FromSeconds ((double) (first_header.FrameLength * frames) / (double) (first_header.Bitrate * 125) + 0.5);
               bitrate = first_header.Bitrate;
            }
         }
         
         sample_rate    = first_header.SampleRate;
         channels       = first_header.ChannelMode == ChannelMode.SingleChannel ? 1 : 2;
         version        = first_header.Version;
         layer          = first_header.Layer;
         channel_mode   = first_header.ChannelMode;
         is_copyrighted = first_header.IsCopyrighted;
         is_original    = first_header.IsOriginal;
      }
   }
}
