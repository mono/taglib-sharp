using System.Collections;
using System;

namespace TagLib.Id3v2
{
   public class MusicCdIdentifierFrame : Frame
   {
      #region Private Properties
      private ByteVector field_data = null;
      #endregion
      
      
      
      #region Constructors
      public MusicCdIdentifierFrame (ByteVector data, byte version) : base (data, version)
      {
         SetData (data, 0, version, true);
      }
      
      public MusicCdIdentifierFrame () : base (FrameType.MCDI, 4)
      {
      }
      
      protected internal MusicCdIdentifierFrame (ByteVector data, int offset, FrameHeader header, byte version) : base(header)
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
      public static MusicCdIdentifierFrame Get (Tag tag, bool create)
      {
         foreach (Frame f in tag)
            if (f is MusicCdIdentifierFrame)
               return f as MusicCdIdentifierFrame;
         
         if (!create)
            return null;
         
         MusicCdIdentifierFrame frame = new MusicCdIdentifierFrame ();
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