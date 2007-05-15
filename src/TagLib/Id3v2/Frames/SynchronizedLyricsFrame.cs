using System;
using System.Collections.Generic;
using System.Text;
using TagLib.Id3v2;

namespace TagLib.Id3v2
{
   public enum TimestampFormat
   {
      Unknown              = 0x00,
      AbsoluteMpegFrames   = 0x01,
      AbsoluteMilliseconds = 0x02
   }
   
   public enum LyricsType
   {
      Other             = 0x00,
      Lyrics            = 0x01,
      TextTranscription = 0x02,
      Movement          = 0x03,
      Events            = 0x04,
      Chord             = 0x05,
      Trivia            = 0x06,
      WebpageUrls       = 0x07,
      ImageUrls         = 0x08
   }
                   
   public class SynchronisedLyricsFrame : Frame
   {
      #region Private Properties
      StringType      text_encoding    = StringType.UTF8;
      ByteVector      language         = null;
      string          description      = null;
      TimestampFormat timestamp_format = TimestampFormat.Unknown;
      LyricsType      lyrics_type      = LyricsType.Other;
      SynchedText []  text             = new SynchedText [0];
      #endregion
      
      #region Constructors
      public SynchronisedLyricsFrame (string description, ByteVector language, StringType encoding) : base ("SYLT", 4)
      {
         this.text_encoding = encoding;
         this.language      = language;
         this.description   = description;
      }
      
      public SynchronisedLyricsFrame (string description, ByteVector language) : this (description, language, TagLib.Id3v2.Tag.DefaultEncoding)
      {}

      public SynchronisedLyricsFrame (string description) : this (description, null)
      {}
      
      public SynchronisedLyricsFrame (ByteVector data, uint version) : base(data, version)
      {
         SetData (data, 0, version);
      }
      
      public static SynchronisedLyricsFrame Get (Tag tag, string description, ByteVector language, bool create)
      {
         foreach (Frame f in tag)
         {
            if (!(f is SynchronisedLyricsFrame))
               continue;
            
            SynchronisedLyricsFrame lyr = f as SynchronisedLyricsFrame;
            
            if (lyr.Description == description && (language == null || language == lyr.Language))
               return lyr;
         }
         
         if (!create)
            return null;
         
         SynchronisedLyricsFrame frame = new SynchronisedLyricsFrame (description, language);
         tag.AddFrame (frame);
         return frame;
      }
      #endregion
      
      #region Public Properties
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
      
      public TimestampFormat Format
      {
         get {return timestamp_format;}
         set {timestamp_format = value;}
      }
      
      public LyricsType Type
      {
         get {return lyrics_type;}
         set {lyrics_type = value;}
      }
      
      public SynchedText [] Text
      {
         get {return text;}
         set {text = value == null ? new SynchedText [0] : value;}
      }
      #endregion
      
      
      public static SynchronisedLyricsFrame GetPreferred (Tag tag, string description, ByteVector language)
      {
         // This is weird, so bear with me. The best thing we can have is 
         // something straightforward and in our own language. If it has a 
         // description, then it is probably used for something other than
         // an actual comment. If that doesn't work, we'd still rather have 
         // something in our language than something in another. After that
         // all we have left are things in other languages, so we'd rather 
         // have one with actual content, so we try to get one with no 
         // description first.
         
         int best_value = -1;
         SynchronisedLyricsFrame best_frame = null;
         
         foreach (Frame f in tag)
         {
            if (!(f is SynchronisedLyricsFrame))
               continue;
            
            SynchronisedLyricsFrame cf = f as SynchronisedLyricsFrame;
            
            bool same_name = cf.Description == description;
            bool same_lang = cf.Language == language;
            
            if (same_name && same_lang) return cf;
            
            int value = same_lang ? 2 : same_name ? 1 : 0;
            
            if (value <= best_value)
               continue;
            
            best_value = value;
            best_frame = cf;
         }
         
         return best_frame;
      }
      
      
      protected override void ParseFields (ByteVector data, uint version)
      {
         if (data.Count < 6)
            throw new CorruptFileException ("Not enough bytes in field.");
         
         text_encoding = (StringType) data [0];
         language = data.Mid (1, 3);
         timestamp_format = (TimestampFormat) data [4];
         lyrics_type = (LyricsType) data [5];
         
         ByteVector delim = TextDelimiter (text_encoding);
         int delim_index = data.Find (delim, 6);
         
         if (delim_index < 0)
            throw new CorruptFileException ("Text delimiter expected.");
         
         description = data.Mid (6, delim_index - 6).ToString (text_encoding);
         
         int offset = delim_index + delim.Count;
         List<SynchedText> l = new List<SynchedText> ();
         
         while (offset + delim.Count + 4 < data.Count)
         {
            delim_index = data.Find (delim, offset);
            if (delim_index < offset)
               throw new CorruptFileException ("Text delimiter expected.");
            
            string text = data.Mid (offset, delim_index - offset).ToString (text_encoding);
            offset = delim_index + delim.Count;
            
            if (offset + 4 > data.Count)
               break;
            
            l.Add (new SynchedText (data.Mid (offset, 4).ToUInt (), text));
            offset += 4;
         }
         
         this.text = l.ToArray ();
      }

      protected override ByteVector RenderFields (uint version)
      {
         StringType encoding = CorrectEncoding(TextEncoding, version);
         ByteVector delim = TextDelimiter (encoding);
         ByteVector v = new ByteVector ();
         
         v.Add ((byte)encoding);
         v.Add (Language);
         v.Add ((byte)timestamp_format);
         v.Add ((byte)lyrics_type);
         v.Add (ByteVector.FromString (description, encoding));
         v.Add (delim);
         
         foreach (SynchedText t in text)
         {
            v.Add (ByteVector.FromString (t.Text, encoding));
            v.Add (delim);
            v.Add (ByteVector.FromUInt (t.Time));
            System.Console.WriteLine (t.Text);
         }
         
         return v;
      }

      protected internal SynchronisedLyricsFrame(ByteVector data, int offset, FrameHeader h, uint version)
         : base(h)
      {
         ParseFields (FieldData (data, offset, version), version);
      }
   }
   
   public struct SynchedText
   {
      private uint time;
      private string text;
      
      public SynchedText (uint time, string text)
      {
         this.time = time;
         this.text = text;
      }
      
      public uint   Time {get {return time;} set {time = value;}}
      public string Text {get {return text;} set {text = value;}}
   }
}