/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2frame.cpp from TagLib
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

namespace TagLib.Id3v2
{
   public enum ChannelType
   {
      Other        = 0x00,
      MasterVolume = 0x01,
      FrontRight   = 0x02,
      FrontLeft    = 0x03,
      BackRight    = 0x04,
      BackLeft     = 0x05,
      FrontCentre  = 0x06,
      BackCentre   = 0x07,
      Subwoofer    = 0x08
   }
   
   public class RelativeVolumeFrame : Frame
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private string identification;
      private Dictionary<ChannelType,ChannelData> channels;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public RelativeVolumeFrame (ByteVector data, uint version) : base (data, version)
      {
         identification = null;
         channels = new Dictionary<ChannelType,ChannelData> ();
         
         SetData (data, 0, version);
      }

      public RelativeVolumeFrame (string identification) : base ("RVA2", 4)
      {
         this.identification = identification;
         channels = new Dictionary<ChannelType,ChannelData> ();
      }

      public override string ToString ()
      {
         return identification;
      }
      
      public ChannelType [] Channels
      {
         get
         {
            ChannelType [] output = new ChannelType [channels.Count];
            channels.Keys.CopyTo (output, channels.Count);
            return output;
         }
      }
      
      public short VolumeAdjustmentIndex (ChannelType type)
      {
         return channels.ContainsKey (type) ? channels [type].VolumeAdjustment : (short) 0;
      }

      public void SetVolumeAdjustmentIndex (short index, ChannelType type)
      {
         if (!channels.ContainsKey (type))
            channels.Add (type, new ChannelData (type));
         
         channels [type].VolumeAdjustment = index;
      }

      public float VolumeAdjustment (ChannelType type)
      {
         return ((float) VolumeAdjustmentIndex (type)) / 512f;
      }

      public void SetVolumeAdjustment (float adjustment, ChannelType type)
      {
         SetVolumeAdjustmentIndex ((short) (adjustment * 512f), type);
      }
      
      public ulong PeakVolumeIndex (ChannelType type)
      {
         return channels.ContainsKey (type) ? channels [type].PeakVolume : 0;
      }

      public void SetPeakVolumeIndex (ulong index, ChannelType type)
      {
         if (!channels.ContainsKey (type))
            channels.Add (type, new ChannelData (type));
         
         channels [type].PeakVolume = index;
      }

      public double PeakVolume (ChannelType type)
      {
         return ((double) PeakVolumeIndex (type)) / 512.0;
      }

      public void SetPeakVolume (double adjustment, ChannelType type)
      {
         SetPeakVolumeIndex ((ulong) (adjustment * 512.0), type);
      }
      
      public static RelativeVolumeFrame Find (Tag tag, string identification)
      {
         foreach (RelativeVolumeFrame f in tag.GetFrames ("RVA2"))
            if (f != null && f.Identification == identification)
               return f;
         return null;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public string Identification
      {
         get {return identification;}
      }

      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected override void ParseFields (ByteVector data, uint version)
      {
         int pos = data.Find (TextDelimiter (StringType.Latin1));
         if (pos < 0)
            return;
         
         identification = data.Mid (0, pos).ToString (StringType.Latin1);
         pos += 1;
         
         // Each channel is at least 4 bytes.

         while (pos <= data.Count - 4)
         {
            ChannelType type = (ChannelType) data [pos];
            pos += 1;

            SetVolumeAdjustmentIndex (data.Mid (pos, 2).ToShort (), type);
            pos += 2;

            int bytes = BitsToBytes (data [pos]);
            pos += 1;
            
            SetPeakVolumeIndex (ParsePeakVolume (data.Mid (pos, bytes)), type);
            pos += bytes;
         }
      }
      
      protected override ByteVector RenderFields (uint version)
      {
         ByteVector data = new ByteVector ();
         if (version < 4)
            return data;

         data.Add (ByteVector.FromString (identification, StringType.Latin1));
         data.Add (TextDelimiter(StringType.Latin1));

         foreach (ChannelData channel in channels.Values)
         {
            data.Add ((byte) channel.ChannelType);
            data.Add (ByteVector.FromShort (channel.VolumeAdjustment));
            data.Add (RenderPeakVolume (channel.PeakVolume));
         }
         
         return data;
      }
      
      protected internal RelativeVolumeFrame (ByteVector data, int offset, FrameHeader h, uint version) : base (h)
      {
         identification = null;
         channels = new Dictionary<ChannelType,ChannelData> ();
         
         ParseFields (FieldData (data, offset, version), version);
      }
      
      protected ulong ParsePeakVolume (ByteVector data)
      {
         if (data.Count == 0)
            return 0;
         
         // If common, pound it out.
         if (data.Count == 2)
            return (ulong)(data [0] + data [1] * 0xFF);
         
         ulong peak = 0;
         for (int i = data.Count - 1; i >= 0; i --)
            peak = peak * 256 + data [i];
         
         return peak;
      }
      
      protected ByteVector RenderPeakVolume (ulong peak)
      {
         ByteVector v = new ByteVector (1);
         
         if (peak == 0)
            return v;
         
         ulong bigger = 1;
         byte bits = 1;
         while (bigger < peak)
         {
            bigger *= 2;
            bits += 1;
         }
         
         v [0] = bits;
         
         for (uint j = 0; j < BitsToBytes (bits); j ++)
         {
            byte o = (byte)(peak % 0xFF);
            peak -= o;
            peak /= 0xFF;
            v.Add (o);
         }
         return v;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private static int BitsToBytes (int i)
      {
         return i % 8 == 0 ? i / 8 : (i - i % 8) / 8 + 1;
      }
      
      //////////////////////////////////////////////////////////////////////////
      // ChannelData class
      //////////////////////////////////////////////////////////////////////////
      private class ChannelData
      {
         public ChannelType ChannelType;
         public short VolumeAdjustment;
         public ulong PeakVolume;
         public ChannelData (ChannelType type)
         {
            ChannelType = type;
            VolumeAdjustment = 0;
            PeakVolume = 0;
         }
      };
   }
}
