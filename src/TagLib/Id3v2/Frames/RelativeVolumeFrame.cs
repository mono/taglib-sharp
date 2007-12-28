//
// RelativeVolumeFrame.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   textidentificationframe.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
// Copyright (C) 2004 Scott Wheeler (Original Implementation)
// 
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

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
		private ChannelData [] channels = new ChannelData [9];
		
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
				List<ChannelType> types = new List<ChannelType> ();
				for (int i = 0; i < 9; i ++)
					if (channels [i].IsSet)
						types.Add ((ChannelType) i);
				return types.ToArray ();
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
			return channels [(int) type].VolumeAdjustmentIndex;
		}
		
		public void SetVolumeAdjustmentIndex (ChannelType type,
		                                      short index)
		{
			channels [(int) type].VolumeAdjustmentIndex = index;
		}
		
		public float GetVolumeAdjustment (ChannelType type)
		{
			return channels [(int) type].VolumeAdjustment;
		}
		
		public void SetVolumeAdjustment (ChannelType type,
		                                 float adjustment)
		{
			channels [(int) type].VolumeAdjustment = adjustment;
		}
		
		public ulong GetPeakVolumeIndex (ChannelType type)
		{
			return channels [(int) type].PeakVolumeIndex;
		}
		
		public void SetPeakVolumeIndex (ChannelType type, ulong index)
		{
			channels [(int) type].PeakVolumeIndex = index;
		}
		
		public double GetPeakVolume (ChannelType type)
		{
			return channels [(int) type].PeakVolume;
		}
		
		public void SetPeakVolume (ChannelType type, double peak)
		{
			channels [(int) type].PeakVolume = peak;
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
			
			identification = data.ToString (StringType.Latin1, 0,
				pos++);
			
			// Each channel is at least 4 bytes.
			
			while (pos <= data.Count - 4) {
				int type = data [pos++];
				
				unchecked {
					channels [type].VolumeAdjustmentIndex =
						(short) data.Mid (pos,
							2).ToUShort ();
				}
				pos += 2;
				
				int bytes = BitsToBytes (data [pos++]);
				
				if (data.Count < pos + bytes)
					throw new CorruptFileException (
						"Insufficient peak data.");
				
				channels [type].PeakVolumeIndex = data.Mid (pos,
					bytes).ToULong ();
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
			
			for (byte i = 0; i < 9; i ++) {
				if (!channels [i].IsSet)
					continue;
				
				data.Add (i);
				unchecked {
					data.Add (ByteVector.FromUShort (
						(ushort) channels [i]
							.VolumeAdjustmentIndex));
				}
				
				byte bits = 0;
				
				for (byte j = 0; j < 64; j ++)
					if ((channels [i].PeakVolumeIndex &
						(1UL << j)) != 0)
						bits = (byte)(j + 1);
				
				data.Add (bits);
				
				if (bits > 0)
					data.Add (ByteVector.FromULong (
						channels [i].PeakVolumeIndex)
							.Mid (8 - BitsToBytes (bits)));
			}
			
			return data;
		}
		
#endregion
		
		
		
#region IClonable
		
		public override Frame Clone ()
		{
			RelativeVolumeFrame frame =
				new RelativeVolumeFrame (identification);
			for (int i = 0; i < 9; i ++)
				frame.channels [i] = channels [i];
			return frame;
		}
		
#endregion
		
		
		
#region Private Static Methods
		
		private static int BitsToBytes (int i)
		{
			return i % 8 == 0 ? i / 8 : (i - i % 8) / 8 + 1;
		}
		
		#endregion
		
		
		
		#region Classes
		
		private struct ChannelData
		{
			public short VolumeAdjustmentIndex;
			public ulong PeakVolumeIndex;
			
			public bool IsSet {
				get {
					return VolumeAdjustmentIndex != 0 ||
						PeakVolumeIndex != 0;
				}
			}
			
			public float VolumeAdjustment {
				get {return VolumeAdjustmentIndex / 512f;}
				set {
					VolumeAdjustmentIndex =
						(short) (value * 512f);
				}
			}
			
			public double PeakVolume {
				get {return PeakVolumeIndex / 512.0;}
				set {PeakVolumeIndex = (ulong) (value * 512.0);}
			}
		}
		
		#endregion
	}
}
