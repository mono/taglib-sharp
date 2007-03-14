using System;
using System.Collections.Generic;
using System.Text;
using TagLib.Id3v2;

namespace TagLib.Id3v2
{
    public class UnsynchronisedLyricsFrame : Frame
    {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      StringType text_encoding;
      ByteVector language;
      string description;
      string text;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public UnsynchronisedLyricsFrame(StringType encoding)
            : base("USLT", 4)
      {
         text_encoding = encoding;
         language = null;
         description = null;
         text = null;
      }

      public UnsynchronisedLyricsFrame(ByteVector data, uint version)
            : base(data, version)
      {
         text_encoding = StringType.UTF8;
         language = null;
         description = null;
         text = null;
         SetData (data, 0, version);
      }


      public override string ToString ()
      {
         return text;
      }
      
      public override void SetText (string text)
      {
         this.text = text;
      }
      
      public static UnsynchronisedLyricsFrame Find (Tag tag, string description)
      {
         foreach (UnsynchronisedLyricsFrame f in tag.GetFrames ("USLT"))
            if (f != null && f.Description == description)
               return f;
         return null;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public StringType TextEncoding
      {
         get {return text_encoding;}
         set {text_encoding = value;}
      }

      public ByteVector Language
      {
         get {return language != null ? language : "XXX";}
         set {language = value != null ? value.Mid (0, 3) : "XXX";}
      }
      
      public string Description
      {
         get {return description;}
         set {description = value;}
      }
      
      public string Text
      {
         get {return text;}
         set {text = value;}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected override void ParseFields (ByteVector data, uint version)
      {
         if (data.Count < 4)
            throw new CorruptFileException ("Not enough bytes in field.");
         
         text_encoding = (StringType) data [0];
         language = data.Mid (1, 3);

         string [] split = data.ToStrings (text_encoding, 4, 2);
         
         if (split.Length == 1)
         {
            // Bad comment frame. Assume that it lacks a description.
            description = String.Empty;
            text        = split [0];
         }
         else
         {
            description = split [0];
            text        = split [1];
         }
      }

        protected override ByteVector RenderFields(uint version)
        {
            StringType encoding = CorrectEncoding(TextEncoding, version);
            ByteVector v = new ByteVector();

            v.Add((byte)encoding);
            v.Add(Language);
            v.Add(ByteVector.FromString (description, encoding));
            v.Add(TextDelimiter(encoding));
            v.Add(ByteVector.FromString (text, encoding));

            return v;
        }

        protected internal UnsynchronisedLyricsFrame(ByteVector data, int offset, FrameHeader h, uint version)
            : base(h)
      {
         text_encoding = StringType.UTF8;
         language = null;
         description = null;
         text = null;
         ParseFields (FieldData (data, offset, version), version);
      }
    }
}