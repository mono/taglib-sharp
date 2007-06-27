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

namespace TagLib.Id3v2 {
	public enum ChannelType {
		
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
		#region Private Properties
		
		private string identification = null;
		
		private Dictionary<ChannelType,ChannelData> channels =
			new Dictionary<ChannelType,ChannelData> ();
		
		#endregion
		
		
		
		#region Constructors
		
		public RelativeVolumeFrame (ByteVector data, byte version)
			: base (data, version)
		{
			SetData (data, 0, version, true);
		}
		
		public RelativeVolumeFrame (string identification)
			: base (FrameType.RVA2, 4)
		{
			this.identification = identification;
		}
		
		protected internal RelativeVolumeFrame (ByteVector data,
		                                        int offset,
		                                        FrameHeader header,
		                                        byte version)
			: base(header)
		{
			SetData (data, offset, version, false);
		}
		
		#endregion
		
		
		
		#region Public Properties
		public string Identification {
			get {return identification;}
		}
		
		public ChannelType [] Channels {
			get {
				ChannelType [] output =
					new ChannelType [channels.Count];
				channels.Keys.CopyTo (output, channels.Count);
				return output;
			}
		}
		
		#endregion
		
		
		
		#region Public Methods
		
		public override string ToString ()
		{
			return identification;
		}
		
		public short GetVolumeAdjustmentIndex (ChannelType type)
		{
			if (channels.ContainsKey (type))
				return channels [type].VolumeAdjustment;
			
			return 0;
		}
		
		public void SetVolumeAdjustmentIndex (ChannelType type,
		                                      short index)
		{
			if (!channels.ContainsKey (type))
				channels.Add (type, new ChannelData (type));
			
			channels [type].VolumeAdjustment = index;
		}
		
		public float GetVolumeAdjustment (ChannelType type)
		{
			return ((float) GetVolumeAdjustmentIndex (type)) / 512f;
		}
		
		public void SetVolumeAdjustment (ChannelType type,
		                                 float adjustment)
		{
			SetVolumeAdjustmentIndex (type,
				(short) (adjustment * 512f));
		}
		
		public ulong GetPeakVolumeIndex (ChannelType type)
		{
			if (channels.ContainsKey (type))
				return channels [type].PeakVolume;
			
			return 0;
		}
		
		public void SetPeakVolumeIndex (ChannelType type, ulong index)
		{
			if (!channels.ContainsKey (type))
				channels.Add (type, new ChannelData (type));
			
			channels [type].PeakVolume = index;
		}
		
		public double GetPeakVolume (ChannelType type)
		{
			return ((double) GetPeakVolumeIndex (type)) / 512.0;
		}
		
		public void SetPeakVolume (ChannelType type, double adjustment)
		{
			SetPeakVolumeIndex (type, (ulong) (adjustment * 512.0));
		}
		
		#endregion
		
		
		
		#region Public Static Methods
		public static RelativeVolumeFrame Get (Tag tag,
		                                       string identification,
		                                       bool create)
		{
			RelativeVolumeFrame rva2;
			foreach (Frame frame in tag.GetFrames (FrameType.RVA2)) {
				rva2 = frame as RelativeVolumeFrame;
				
				if (rva2 == null)
					continue;
				
				if (rva2.Identification != identification)
					continue;
				
				return rva2;
			}
			
			if (!create)
				return null;
			
			rva2 = new RelativeVolumeFrame (identification);
			tag.AddFrame (rva2);
			return rva2;
		}
		
		#endregion
		
		
		
		#region Protected Properties
		
		protected override void ParseFields (ByteVector data,
		                                     byte version)
		{
			int pos = data.Find (ByteVector.TextDelimiter (
				StringType.Latin1));
			if (pos < 0)
				return;
			
			identification = data.Mid (0, pos).ToString (
				StringType.Latin1);
			pos += 1;
			
			// Each channel is at least 4 bytes.
			
			while (pos <= data.Count - 4) {
				ChannelType type = (ChannelType) data [pos];
				pos += 1;
				
				unchecked {
					SetVolumeAdjustmentIndex (type,
						(short) data.Mid (pos,
							2).ToUShort ());
				}
				pos += 2;
				
				int bytes = BitsToBytes (data [pos]);
				pos += 1;
				
				SetPeakVolumeIndex (type, data.Mid (pos,
					bytes).ToULong ());
				pos += bytes;
			}
		}
		
		protected override ByteVector RenderFields (byte version)
		{
			if (version < 4)
				throw new NotImplementedException ();
			
			ByteVector data = new ByteVector ();
			data.Add (ByteVector.FromString (identification,
				StringType.Latin1));
			data.Add (ByteVector.TextDelimiter(StringType.Latin1));
			
			foreach (ChannelData channel in channels.Values) {
				data.Add ((byte) channel.ChannelType);
				unchecked {
					data.Add (ByteVector.FromUShort (
						(ushort)channel.VolumeAdjustment));
				}
				
				byte bits = 0;
				
				for (byte i = 0; i < 64; i ++)
					if ((channel.PeakVolume & (1UL << i)) != 0)
						bits = (byte)(i + 1);
				
				data.Add (bits);
				
				if (bits > 0)
					data.Add (ByteVector.FromULong (
						channel.PeakVolume).Mid (
							8 - BitsToBytes (bits)));
			}
			
			return data;
		}
		
		#endregion
		
		
		
		#region Private Static Methods
		
		private static int BitsToBytes (int i)
		{
			return i % 8 == 0 ? i / 8 : (i - i % 8) / 8 + 1;
		}
		
		#endregion
		
		
		
		#region Classes
		
		private class ChannelData
		{
			public ChannelType ChannelType;
			public short VolumeAdjustment;
			public ulong PeakVolume;
			
			public ChannelData (ChannelType type)
			{
				ChannelType = type;
			}
		}
		
		#endregion
	}
}
