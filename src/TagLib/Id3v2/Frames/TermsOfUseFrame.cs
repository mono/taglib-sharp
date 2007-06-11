using System.Collections;
using System;

namespace TagLib.Id3v2
{
   public class TermsOfUseFrame : Frame
   {
      #region Private Properties
      private StringType encoding    = TagLib.Id3v2.Tag.DefaultEncoding;
      private string     language    = null;
      private string     text        = null;
      #endregion
      
      
      
      #region Constructors
      public TermsOfUseFrame (string language, StringType encoding) : base (FrameType.USER, 4)
      {
         this.encoding    = encoding;
         this.language    = language;
      }
      
      public TermsOfUseFrame (string language) : base (FrameType.USER, 4)
      {
         this.language    = language;
      }

      public TermsOfUseFrame (ByteVector data, byte version) : base (data, version)
      {
         SetData (data, 0, version, true);
      }

      protected internal TermsOfUseFrame (ByteVector data, int offset, FrameHeader header, byte version) : base(header)
      {
         SetData (data, offset, version, false);
      }
      #endregion
      
      
      
      #region Public Properties
      public StringType TextEncoding
      {
         get {return encoding;}
         set {encoding = value;}
      }

      public string Language
      {
         get {return (language != null && language.Length > 2) ? language.Substring (0, 3) : "XXX";}
         set {language = value;}
      }
      
      public string Text
      {
         get {return text;}
         set {text = value;}
      }
      #endregion
      
      
      
      #region Public Methods
      public override string ToString ()
      {
         return text;
      }
      #endregion
      
      
      
      #region Public Static Methods
      public static TermsOfUseFrame Get (Tag tag, string language, bool create)
      {
         foreach (Frame f in tag.GetFrames (FrameType.USER))
         {
            TermsOfUseFrame cf = f as TermsOfUseFrame;
            
            if (cf != null && (language == null || language == cf.Language))
               return cf;
         }
         
         if (!create)
            return null;
         
         TermsOfUseFrame frame = new TermsOfUseFrame (language);
         tag.AddFrame (frame);
         return frame;
      }
      
      public static TermsOfUseFrame GetPreferred (Tag tag, string language)
      {
         TermsOfUseFrame best = null;
         foreach (Frame f in tag.GetFrames (FrameType.USER))
         {
            TermsOfUseFrame cf = f as TermsOfUseFrame;
            if (cf == null) continue;
            
            if (cf.Language == language)
               return cf;
            
            if (best == null)
               best = cf;
         }
         
         return best;
      }
      #endregion
      
      
      
      #region Protected Methods
      protected override void ParseFields (ByteVector data, byte version)
      {
         if (data.Count < 4)
            throw new CorruptFileException ("Not enough bytes in field.");
         
         encoding = (StringType) data [0];
         language = data.Mid (1, 3).ToString (StringType.Latin1);
         text = data.ToString (encoding, 4);
      }

      protected override ByteVector RenderFields (byte version)
      {
         StringType encoding = CorrectEncoding (TextEncoding, version);
         ByteVector v = new ByteVector ();

         v.Add ((byte) encoding);
         v.Add (ByteVector.FromString (Language, StringType.Latin1));
         v.Add (ByteVector.FromString (text, encoding));

         return v;
      }
      #endregion
   }
}
