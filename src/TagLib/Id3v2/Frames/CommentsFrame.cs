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
      public CommentsFrame (StringType encoding) : base ("COMM")
      {
         text_encoding = encoding;
         language = null;
         description = null;
         text = null;
      }

      public CommentsFrame (ByteVector data) : base (data)
      {
         text_encoding = StringType.UTF8;
         language = null;
         description = null;
         text = null;
         SetData (data);
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
      protected override void ParseFields (ByteVector data)
      {
         if (data.Count < 5)
         {
            Debugger.Debug ("A comment frame must contain at least 5 bytes.");
            return;
         }
         
         text_encoding = (StringType) data [0];
         language = data.Mid (1, 3);

         int byte_align = text_encoding == StringType.Latin1 || text_encoding == StringType.UTF8 ? 1 : 2;

         ByteVectorList l = ByteVectorList.Split (data.Mid (4), TextDelimiter (text_encoding), byte_align, 2);

         if (l.Count == 2)
         {
            description = l [0].ToString (text_encoding);
            text        = l [1].ToString (text_encoding);
         }
      }

      protected override ByteVector RenderFields ()
      {
         ByteVector v = new ByteVector ();

         v.Add ((byte) TextEncoding);
         v.Add (Language);
         v.Add (ByteVector.FromString (description, TextEncoding));
         v.Add (TextDelimiter (TextEncoding));
         v.Add (ByteVector.FromString (text, TextEncoding));

         return v;
      }

      protected internal CommentsFrame (ByteVector data, FrameHeader h) : base (h)
      {
         text_encoding = StringType.UTF8;
         language = null;
         description = null;
         text = null;
         ParseFields (FieldData (data));
      }
   }
}
