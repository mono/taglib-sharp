/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2commentsframe.cpp from TagLib
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
using System;

namespace TagLib.Id3v2
{
   public class CommentsFrame : Frame
   {
      #region Private Properties
      private StringType encoding    = StringType.UTF8;
      private string     language    = null;
      private string     description = null;
      private string     text        = null;
      #endregion
      
      
      
      #region Constructors
      public CommentsFrame (string description, string language, StringType encoding) : base (FrameType.COMM, 4)
      {
         this.encoding    = encoding;
         this.language    = language;
         this.description = description;
      }
      
      public CommentsFrame (string description, string language) : this (description, language, TagLib.Id3v2.Tag.DefaultEncoding)
      {}

      public CommentsFrame (string description) : this (description, null)
      {}

      public CommentsFrame (ByteVector data, byte version) : base (data, version)
      {
         SetData (data, 0, version, true);
      }

      protected internal CommentsFrame (ByteVector data, int offset, FrameHeader header, byte version) : base(header)
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
         get {return description != null ? description : string.Empty;}
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
      public static CommentsFrame Get (Tag tag, string description, string language, bool create)
      {
         foreach (Frame f in tag.GetFrames (FrameType.COMM))
         {
            CommentsFrame cf = f as CommentsFrame;
            
            if (cf != null && cf.Description == description && (language == null || language == cf.Language))
               return cf;
         }
         
         if (!create)
            return null;
         
         CommentsFrame frame = new CommentsFrame (description, language);
         tag.AddFrame (frame);
         return frame;
      }
      
      public static CommentsFrame GetPreferred (Tag tag, string description, string language)
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
         CommentsFrame best_frame = null;
         
         foreach (Frame f in tag.GetFrames (FrameType.COMM))
         {
            CommentsFrame cf = f as CommentsFrame;
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
         language = data.Mid (1, 3).ToString (StringType.Latin1);
      	
         // Instead of splitting into two string, in the format
         // [{desc}\0{value}], try splitting into three strings in case of a
         // misformatted [{desc}\0{value}\0].
         string [] split = data.ToStrings (encoding, 4, 3);
         
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

      protected override ByteVector RenderFields (byte version)
      {
         StringType encoding = CorrectEncoding (TextEncoding, version);
         ByteVector v = new ByteVector ();

         v.Add ((byte) encoding);
         v.Add (ByteVector.FromString (Language, StringType.Latin1));
         v.Add (ByteVector.FromString (description, encoding));
         v.Add (ByteVector.TextDelimiter (encoding));
         v.Add (ByteVector.FromString (text, encoding));

         return v;
      }
      #endregion
   }
}
