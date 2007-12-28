//
// UnsynchronisedLyricsFrame.cs:
//
// Author:
//   Patrick Laplante
//
// Original Source:
//   TagLib.Id3v2.CommentsFrame
//
// Copyright (C) 2007 Brian Nickel (Original Implementation)
// Copyright (C) 2002,2003 Scott Wheeler (Original Implementation)
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

using System;
using System.Collections.Generic;
using System.Text;
using TagLib.Id3v2;

namespace TagLib.Id3v2
{
    public class UnsynchronisedLyricsFrame : Frame
    {
      #region Private Properties
      private StringType encoding      = StringType.UTF8;
      private string     language      = null;
      private string     description   = null;
      private string     text          = null;
      #endregion
      
      
      
      #region Constructors
      public UnsynchronisedLyricsFrame (string description, string language, StringType encoding) : base (FrameType.USLT, 4)
      {
         this.encoding    = encoding;
         this.language    = language;
         this.description = description;
      }
      
      public UnsynchronisedLyricsFrame (string description, string language) : this (description, language, TagLib.Id3v2.Tag.DefaultEncoding)
      {}

      public UnsynchronisedLyricsFrame (string description) : this (description, null)
      {}
      
      public UnsynchronisedLyricsFrame(ByteVector data, byte version) : base(data, version)
      {
         SetData (data, 0, version, true);
      }
      
      protected internal UnsynchronisedLyricsFrame(ByteVector data, int offset, FrameHeader header, byte version) : base(header)
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
      #endregion
      
      
      
      #region Public Methods
      public override string ToString ()
      {
         return text;
      }
      #endregion
      
      
      
      #region Public Static Methods
      public static UnsynchronisedLyricsFrame Get (Tag tag, string description, string language, bool create)
      {
         foreach (Frame f in tag.GetFrames (FrameType.USLT))
         {
            UnsynchronisedLyricsFrame cf = f as UnsynchronisedLyricsFrame;
            
            if (cf != null && cf.Description == description && (language == null || language == cf.Language))
               return cf;
         }
         
         if (!create)
            return null;
         
         UnsynchronisedLyricsFrame frame = new UnsynchronisedLyricsFrame (description, language);
         tag.AddFrame (frame);
         return frame;
      }
      
      public static UnsynchronisedLyricsFrame GetPreferred (Tag tag, string description, string language)
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
         UnsynchronisedLyricsFrame best_frame = null;
         
         foreach (Frame f in tag.GetFrames (FrameType.USLT))
         {
            UnsynchronisedLyricsFrame cf = f as UnsynchronisedLyricsFrame;
            if (cf == null) continue;
            
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
      #endregion
      
      
      
      
      #region Protected Methods
      protected override void ParseFields (ByteVector data, byte version)
      {
         if (data.Count < 4)
            throw new CorruptFileException ("Not enough bytes in field.");
         
         encoding = (StringType) data [0];
         language = data.ToString (StringType.Latin1, 1, 3);

         string [] split = data.ToStrings (encoding, 4, 2);
         
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
      
      protected override ByteVector RenderFields(byte version)
      {
         StringType encoding = CorrectEncoding(TextEncoding, version);
         ByteVector v = new ByteVector();

         v.Add((byte)encoding);
         v.Add(ByteVector.FromString (Language, StringType.Latin1));
         v.Add(ByteVector.FromString (description, encoding));
         v.Add(ByteVector.TextDelimiter(encoding));
         v.Add(ByteVector.FromString (text, encoding));

         return v;
      }
		
#endregion
		
		
		
#region IClonable
		
		public override Frame Clone ()
		{
			UnsynchronisedLyricsFrame frame =
				new UnsynchronisedLyricsFrame (description,
					language, encoding);
			frame.text = text;
			return frame;
		}
		
#endregion
	}
}
