/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2frame.cpp from TagLib
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
      public CommentsFrame (StringType encoding) : base ("COMM", 4)
      {
         text_encoding = encoding;
         language = null;
         description = null;
         text = null;
      }

      public CommentsFrame (ByteVector data, uint version) : base (data, version)
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
      
      public static CommentsFrame Find (Tag tag, string description)
      {
         foreach (CommentsFrame f in tag.GetFrames ("COMM"))
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
         if (data.Count < 5)
         {
            Debugger.Debug ("A comment frame must contain at least 5 bytes.");
            return;
         }
         
         text_encoding = (StringType) data [0];
         language = data.Mid (1, 3);

         string [] split = data.ToString (text_encoding, 4).Split (new char [] {'\0'}, 2);
         
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

      protected override ByteVector RenderFields (uint version)
      {
         StringType encoding = CorrectEncoding (TextEncoding, version);
         ByteVector v = new ByteVector ();

         v.Add ((byte) encoding);
         v.Add (Language);
         v.Add (ByteVector.FromString (description, encoding));
         v.Add (TextDelimiter (encoding));
         v.Add (ByteVector.FromString (text, encoding));

         return v;
      }

      protected internal CommentsFrame (ByteVector data, int offset, FrameHeader h, uint version) : base (h)
      {
         text_encoding = StringType.UTF8;
         language = null;
         description = null;
         text = null;
         ParseFields (FieldData (data, offset, version), version);
      }
   }
}
