/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
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

using System;
using System.Text;

namespace TagLib.Asf
{
   public class StreamPropertiesObject : Object
   {
#region Private Properties
      private Guid stream_type;
      private Guid error_correction_type;
      private ulong time_offset;
      private ushort flags;
      private uint reserved;
      private ByteVector type_specific_data;
      private ByteVector error_correction_data;
#endregion
      
#region Constructors
      public StreamPropertiesObject (Asf.File file, long position) : base (file, position)
      {
         if (!Guid.Equals (Asf.Guid.AsfStreamPropertiesObject))
            throw new System.Exception ("Object GUID incorrect.");
         
         if (OriginalSize < 78)
            throw new System.Exception ("Object size too small.");
         
         stream_type                       = file.ReadGuid  ();
         error_correction_type             = file.ReadGuid  ();
         time_offset                       = file.ReadQWord ();
         int type_specific_data_length     = (int) file.ReadDWord ();
         int error_correction_data_length  = (int) file.ReadDWord ();
         flags										 = file.ReadWord  ();
         reserved                          = file.ReadDWord ();
         type_specific_data                = file.ReadBlock (type_specific_data_length);
         error_correction_data             = file.ReadBlock (error_correction_data_length);
      }
#endregion
      
#region Public Methods
      public override ByteVector Render ()
      {
         ByteVector output = stream_type.Render ();
         output.Add (error_correction_type.Render ());
         output.Add (RenderQWord (time_offset));
         output.Add (RenderDWord ((uint) type_specific_data.Count));
         output.Add (RenderDWord ((uint) error_correction_data.Count));
         output.Add (RenderWord  (flags));
         output.Add (RenderDWord (reserved));
         output.Add (type_specific_data);
         output.Add (error_correction_data);
         
         return Render (output);
      }
      
      public ICodec GetCodec ()
      {
         if (stream_type.Equals (Guid.AsfAudioMedia))
            return new AudioStreamProperties (TypeSpecificData);
         
         if (stream_type.Equals (Guid.AsfVideoMedia))
            return new VideoStreamProperties (TypeSpecificData);
         
         return null;
      }
#endregion
      
#region Public Properties
      public Guid       StreamType {get {return stream_type;}}
      public Guid       ErrorCorrectionType {get {return error_correction_type;}}
      public TimeSpan   TimeOffset {get {return new TimeSpan ((long)time_offset);}}
      public ushort     Flags {get {return flags;}}
      public ByteVector TypeSpecificData {get {return type_specific_data;}}
      public ByteVector ErrorCorrectionData {get {return error_correction_data;}}
#endregion
   }
   
   public class AudioStreamProperties : IAudioCodec
   {
      private Riff.WaveFormatEx wave_format_ex;
      
      public AudioStreamProperties (ByteVector type_specific_data)
      {
         wave_format_ex = new Riff.WaveFormatEx (type_specific_data, 0);
      }
      
      public int AudioSampleRate {get {return (int) wave_format_ex.SamplesPerSecond;}}
      public int AudioChannels {get {return wave_format_ex.Channels;}}
      public int AudioBitrate {get {return (int) Math.Round (wave_format_ex.AverageBytesPerSecond * 8d / 1000d);}}
      public string Description {get {return wave_format_ex.Description;}}
      public MediaTypes MediaTypes {get {return MediaTypes.Audio;}}
      public TimeSpan Duration {get {return TimeSpan.Zero;}}
   }
   
   public class VideoStreamProperties : IVideoCodec
   {
      TagLib.Riff.BitmapInfoHeader header;
      
      public VideoStreamProperties (ByteVector data)
      {
         header = new TagLib.Riff.BitmapInfoHeader (data, 11);
      }
      
      public int VideoWidth  {get {return (int) header.Width;}}
      public int VideoHeight {get {return (int) header.Height;}}
      public string Description {get {return header.Description;}}
      public MediaTypes MediaTypes {get {return MediaTypes.Video;}}
      public TimeSpan Duration {get {return TimeSpan.Zero;}}
   }
}
