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
   public class Properties : AudioProperties
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
      public Properties (File file, ReadStyle style) : base (style)
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
         
         Read ();
      }
      
      public Properties (File file) : this (file, ReadStyle.Average) {}
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      
      public override TimeSpan    Duration      {get {return duration;}}
      public override int         Bitrate       {get {return bitrate;}}
      public override int         SampleRate    {get {return sample_rate;}}
      public override int         Channels      {get {return channels;}}
      public          Version     Version       {get {return version;}}
      public          int         Layer         {get {return layer;}}
      public          ChannelMode ChannelMode   {get {return channel_mode;}}
      public          bool        IsCopyrighted {get {return is_copyrighted;}}
      public          bool        IsOriginal    {get {return is_original;}}


      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////

      private void Read ()
      {
         // Since we've likely just looked for the ID3v1 tag, start at the end of the
         // file where we're least likely to have to have to move the disk head.

         long last = file.LastFrameOffset;

         if (last < 0)
         {
            Debugger.Debug ("Mpeg.Properties.Read() -- Could not find a valid last MPEG frame in the stream.");
            return;
         }

         file.Seek (last);
         Header last_header = new Header (file.ReadBlock (4));

         long first = file.FirstFrameOffset;

         if (first < 0)
         {
            Debugger.Debug ("Mpeg.Properties.Read() -- Could not find a valid first MPEG frame in the stream.");
            return;
         }

         if(!last_header.IsValid)
         {
            long pos = last;

            while (pos > first)
            {
               pos = file.PreviousFrameOffset (pos);

               if(pos < 0)
                  break;

               file.Seek (pos);
               Header header = new Header (file.ReadBlock (4));

               if (header.IsValid)
               {
                  last_header = header;
                  last = pos;
                  break;
               }
            }
         }

         // Now jump back to the front of the file and read what we need from there.

         file.Seek (first);
         Header first_header = new Header (file.ReadBlock (4));

         if (!first_header.IsValid || !last_header.IsValid)
         {
            Debugger.Debug ("Mpeg.Properties.Read() -- Page headers were invalid.");
            return;
         }

         // Check for a Xing header that will help us in gathering information about a
         // VBR stream.

         int xing_header_offset = XingHeader.XingHeaderOffset (first_header.Version,
                                      first_header.ChannelMode);

         file.Seek (first + xing_header_offset);
         XingHeader xing_header = new XingHeader (file.ReadBlock (16));

         // Read the length and the bitrate from the Xing header.

         if(xing_header.IsValid && first_header.SampleRate > 0 && xing_header.TotalFrames > 0)
         {
            int [] block_size = {0, 384, 1152, 1152};
            
            double time_per_frame = block_size [first_header.Layer];
            time_per_frame = first_header.SampleRate > 0 ? time_per_frame / first_header.SampleRate : 0;
            duration = new TimeSpan((int)(time_per_frame * xing_header.TotalFrames) * TimeSpan.TicksPerSecond);
            bitrate = (int) (duration > TimeSpan.Zero ? ((xing_header.TotalSize * 8L) / duration.TotalSeconds) / 1000 : 0);
         }

         // Since there was no valid Xing header found, we hope that we're in a constant
         // bitrate file.

         // TODO: Make this more robust with audio property detection for VBR without a
         // Xing header.

         else if (first_header.FrameLength > 0 && first_header.Bitrate > 0)
         {
            int frames = (int) ((last - first) / first_header.FrameLength + 1);

            duration = TimeSpan.FromSeconds ((double) (first_header.FrameLength * frames) / (double) (first_header.Bitrate * 125) + 0.5);
            bitrate = first_header.Bitrate;
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
