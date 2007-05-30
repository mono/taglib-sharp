using System;
using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class IsoAudioSampleEntry : IsoSampleEntry, IAudioCodec
   {
      #region Private Properties
      private ushort channel_count;
      private ushort sample_size;
      private uint   sample_rate;
      private IEnumerable<Box> children;
      #endregion
      
      
      
      #region Constructors
      public IsoAudioSampleEntry (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, file, handler)
      {
         file.Seek (base.DataPosition + 8);
         channel_count = file.ReadBlock (2).ToUShort ();
         sample_size   = file.ReadBlock (2).ToUShort ();
         file.Seek (base.DataPosition + 16);
         sample_rate   = file.ReadBlock (4).ToUInt  ();
         children = LoadChildren (file);
      }
      #endregion
      
      
      
      #region Public Properties
      protected override long    DataPosition {get {return base.DataPosition + 20;}}
      public    override IEnumerable<Box> Children     {get {return children;}}
      #endregion
      
      
      
      #region IAudioCodec Properties
      public int AudioBitrate
      {
         get
         {
            AppleElementaryStreamDescriptor esds = GetChildRecursively ("esds") as AppleElementaryStreamDescriptor;
            
            // If we don't have an stream descriptor, we don't know what's what.
            if (esds == null)
               return 0;
            
            // Return from the elementary stream descriptor.
            return (int) esds.AverageBitrate;
         }
      }
      
      public int AudioChannels   {get {return channel_count;}}
      public int AudioSampleRate {get {return (int)(sample_rate >> 16);}}
      public int AudioSampleSize {get {return sample_size;}}
      public string Description {get {return "MPEG-4 Audio (" + BoxType.ToString () + ")";}}
      public MediaTypes MediaTypes {get {return MediaTypes.Audio;}}
      public TimeSpan Duration {get {return TimeSpan.Zero;}}
      #endregion
   }
}
