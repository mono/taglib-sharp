using System.Collections;
using System;

namespace TagLib.Id3v2
{
   public class MusicCdIdentifier : Frame
   {
      #region Private Properties
      private ByteVector field_data = null;
      #endregion
      
      
      
      #region Constructors
      public MusicCdIdentifier (ByteVector data, byte version) : base (data, version)
      {
         SetData (data, 0, version, true);
      }
      
      public MusicCdIdentifier () : base ("MCDI", 4)
      {
      }
      
      protected internal MusicCdIdentifier (ByteVector data, int offset, FrameHeader header, byte version) : base(header)
      {
         SetData (data, offset, version, false);
      }
      #endregion
      
      
      
      #region Public Properties
      public ByteVector Data
      {
         get {return field_data;}
         set {field_data = value;}
      }
      #endregion
      
      
      
      #region Public Methods
      public override string ToString ()
      {
         return null;
      }
      #endregion
      
      
      
      #region Public Static Methods
      public static MusicCdIdentifier Get (Tag tag, bool create)
      {
         foreach (Frame f in tag)
            if (f is MusicCdIdentifier)
               return f as MusicCdIdentifier;
         
         if (!create)
            return null;
         
         MusicCdIdentifier frame = new MusicCdIdentifier ();
         tag.AddFrame (frame);
         return frame;
      }
      #endregion
      
      
      
      #region Protected Methods
      protected override void ParseFields (ByteVector data, byte version)
      {
         field_data = data;
      }
      
      protected override ByteVector RenderFields (byte version)
      {
         return field_data != null ? field_data : new ByteVector ();
      }
      #endregion
   }
}