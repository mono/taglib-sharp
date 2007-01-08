/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : textidentificationframe.cpp from TagLib
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
   public class TextIdentificationFrame : Frame
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      StringType text_encoding;
      StringList field_list;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public TextIdentificationFrame (ByteVector type, StringType encoding) : base (type)
      {
         field_list = new StringList ();
         text_encoding = encoding;
      }

      public TextIdentificationFrame (ByteVector data) : base (data)
      {
         field_list = new StringList ();
         text_encoding = StringType.UTF8;
         SetData (data);
      }

      public void SetText (StringList l)
      {
         field_list.Clear ();
         field_list.Add (l);
      }

      public override void SetText (String s)
      {
         field_list.Clear ();
         field_list.Add (s);
      }

      public override string ToString ()
      {
         return field_list.ToString ();
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public StringList FieldList
      {
        get {return new StringList (field_list);}
      }
      
      public StringType TextEncoding
      {
         get {return text_encoding;}
         set {text_encoding = value;}
      }


      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected override void ParseFields (ByteVector data)
      {
         // read the string data type (the first byte of the field data)
         
         text_encoding = (StringType) data [0];
         
         // split the byte array into chunks based on the string type (two byte delimiter
         // for unicode encodings)
         
         int byte_align = text_encoding == StringType.Latin1 || text_encoding == StringType.UTF8 ? 1 : 2;
         
         ByteVectorList l = ByteVectorList.Split (data.Mid (1), TextDelimiter (text_encoding), byte_align);

         field_list.Clear ();

         // append those split values to the list and make sure that the new string's
         // type is the same specified for this frame
         
         foreach (ByteVector v in l)
            field_list.Add (v.ToString (text_encoding));
      }

      protected override ByteVector RenderFields ()
      {
         ByteVector v = new ByteVector ((byte) text_encoding);

         for (int i = 0; i < field_list.Count; i++)
         {
            // Since the field list is null delimited, if this is not the
            // first element in the list, append the appropriate delimiter
            // for this encoding.
            if (i !=0)
               v.Add (TextDelimiter (text_encoding));
            
            v.Add (ByteVector.FromString (field_list [i], text_encoding));
         }
         
         return v;
      }

      protected internal TextIdentificationFrame (ByteVector data, FrameHeader h) : base (h)
      {
         field_list = new StringList ();
         text_encoding = StringType.UTF8;
         
         ParseFields (FieldData (data));
      }
   }


   public class UserTextIdentificationFrame : TextIdentificationFrame
   {
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public UserTextIdentificationFrame (StringType encoding) : base ("TXXX", encoding)
      {
         StringList l = new StringList ();
         l.Add ((string)null);
         l.Add ((string)null);
         
         base.SetText (l);
      }

      public UserTextIdentificationFrame (ByteVector data) : base (data)
      {
         CheckFields ();
      }

      public override string ToString ()
      {
         return "[" + Description + "] " + FieldList.ToString ();
      }

      public override void SetText (string text)
      {
         StringList l = new StringList (Description);
         l.Add (text);
         
         base.SetText (l);
      }

      new public void SetText (StringList fields)
      {
         StringList l = new StringList (Description);
         l.Add (fields);
         
         base.SetText (l);
      }
      
      public static UserTextIdentificationFrame Find (Tag tag, string description)
      {
         foreach (UserTextIdentificationFrame f in tag.GetFrames ("TXXX"))
            if (f != null && f.Description == description)
               return f;
         return null;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public string Description
      {
         get {return !base.FieldList.IsEmpty ? base.FieldList [0] : null;}
         set
         {
            StringList l = new StringList (base.FieldList);

            if (l.IsEmpty)
               l.Add (value);
            else
               l [0] = value;

            base.SetText (l);
         }
      }

      new public StringList FieldList
      {
         get
         {
            StringList l = new StringList (base.FieldList);
            if (!l.IsEmpty)
               l.RemoveAt (0);
            return l;
         }
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void CheckFields ()
      {
         int fields = base.FieldList.Count;
         
         if (fields == 0)
            Description = "";
         
         if(fields <= 1)
            SetText ("");
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected internal UserTextIdentificationFrame (ByteVector data, FrameHeader h) : base (data, h)
      {
         CheckFields ();
      }
   }
}
