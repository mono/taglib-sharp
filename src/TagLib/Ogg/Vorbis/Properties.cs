/***************************************************************************
    copyright            : (C) 2006 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : audioproperties.cpp from TagLib
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

namespace TagLib.Ogg.Vorbis
{
   public class Properties : AudioProperties
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private TimeSpan  duration;
      private int       bitrate;
      private int       sample_rate;
      private int       channels;
      private int       vorbis_version;
      private int       bitrate_maximum;
      private int       bitrate_nominal;
      private int       bitrate_minimum;
      
      private static byte [] vorbis_comment_header_id = {0x01, (byte)'v', (byte)'o', (byte)'r', (byte)'b', (byte)'i', (byte)'s'};
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Properties (File file, ReadStyle style) : base (style)
      {
         duration        = TimeSpan.Zero;
         bitrate         = 0;
         sample_rate     = 0;
         channels        = 0;
         vorbis_version  = 0;
         bitrate_maximum = 0;
         bitrate_nominal = 0;
         bitrate_minimum = 0;
         
         Read (file, style);
      }

      public Properties (File file) : this (file, ReadStyle.Average)
      {
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TimeSpan Duration       {get {return duration;}}
      public override int      Bitrate        {get {return (int) ((float)bitrate / 1000f + 0.5);}}
      public override int      SampleRate     {get {return sample_rate;}}
      public override int      Channels       {get {return channels;}}
      public          int      VorbisVersion  {get {return vorbis_version;}}
      public          int      BitrateMaximum {get {return bitrate_maximum;}}
      public          int      BitrateNominal {get {return bitrate_nominal;}}
      public          int      BitrateMinimum {get {return bitrate_minimum;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Read (File file, ReadStyle style)
      {
         // Get the identification header from the Ogg implementation.

         ByteVector data = file.GetPacket (0);

         int pos = 0;

         if (data.Mid (pos, 7) != vorbis_comment_header_id)
            throw new CorruptFileException ("Invalid Vorbis identification header.");

         pos += 7;

         vorbis_version = (int) data.Mid(pos, 4).ToUInt (false);
         pos += 4;

         channels = data [pos];
         pos += 1;

         sample_rate = (int) data.Mid(pos, 4).ToUInt (false);
         pos += 4;

         bitrate_maximum = (int) data.Mid(pos, 4).ToUInt (false);
         pos += 4;

         bitrate_nominal = (int) data.Mid(pos, 4).ToUInt (false);
         pos += 4;

         bitrate_minimum = (int) data.Mid(pos, 4).ToUInt (false);

         // TODO: Later this should be only the "fast" mode.
         bitrate = bitrate_nominal;

         // Find the length of the file.  See http://wiki.xiph.org/VorbisStreamLength/
         // for my notes on the topic.

         Ogg.PageHeader first = file.FirstPageHeader;
         Ogg.PageHeader last = file.LastPageHeader;

         if (first != null && last != null)
         {
            long start = first.AbsoluteGranularPosition;
            long end = last.AbsoluteGranularPosition;

            if (start >= 0 && end >= 0 && sample_rate > 0)
               duration = TimeSpan.FromSeconds (((double)(end - start) / (double) sample_rate));
            else
               Debugger.Debug ("Vorbis.Properties.Read() -- Either the PCM " +
                               "values for the start or end of this file was " +
                               "incorrect or the sample rate is zero.");
         }
         else
            Debugger.Debug("Vorbis.Properties.Read() -- Could not find valid first and last Ogg pages.");
      }
   }
}
