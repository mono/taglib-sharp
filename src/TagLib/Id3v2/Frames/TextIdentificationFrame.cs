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
      public TextIdentificationFrame (ByteVector type, StringType encoding) : base (type, 4)
      {
         field_list = new StringList ();
         text_encoding = encoding;
      }

      public TextIdentificationFrame (ByteVector data, uint version) : base (data, version)
      {
         field_list = new StringList ();
         text_encoding = StringType.UTF8;
         SetData (data, 0, version);
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
      protected override void ParseFields (ByteVector data, uint version)
      {
         // read the string data type (the first byte of the field data)
         text_encoding = (StringType) data [0];
         field_list.Clear ();
         
         if (version > 3)
            field_list.Add (data.ToString (text_encoding, 1).Split (new char []{'\0'}));
         else
         {
            string value = data.ToString (text_encoding, 1);

            if (value.Length == 0 || value [0] == '\0')
               return;
            
            // Do a fast removal of end bytes.
            if (value.Length > 1 && value [value.Length - 1] == '\0')
               for (int i = value.Length - 1; i >= 0; i --)
                  if (value [i] != '\0')
                  {
                     value = value.Substring (0, i + 1);
                     break;
                  }
            
            if (Header.FrameId == "TCOM" ||
                Header.FrameId == "TEXT" ||
                Header.FrameId == "TOLY" ||
                Header.FrameId == "TOPE" ||
                Header.FrameId == "TPE1" ||
                Header.FrameId == "TPE2" ||
                Header.FrameId == "TPE3" ||
                Header.FrameId == "TPE4")
               field_list.Add (value.Split (new char []{'/'}));
            else
               field_list.Add (value);
         }
         
      }

      protected override ByteVector RenderFields (uint version)
      {
         StringType encoding = CorrectEncoding (text_encoding, version);
         ByteVector v = new ByteVector ((byte) encoding);
         
         if (version > 3)
         {
            for (int i = 0; i < field_list.Count; i++)
            {
               // Since the field list is null delimited, if this is not the
               // first element in the list, append the appropriate delimiter
               // for this encoding.
               if (i !=0)
                  v.Add (TextDelimiter (encoding));
            
               v.Add (ByteVector.FromString (field_list [i], encoding));
            }
         }
         else
         {
            if (this.Header.FrameId == "TCON")
            {
               byte id;
               string data = "";
               foreach (string s in field_list)
                  data += byte.TryParse (s, out id) ? ("(" + id + ")") : s;
               v.Add (ByteVector.FromString (data, encoding));
            }
            else
               v.Add (ByteVector.FromString (field_list.ToString ("/"), encoding));
         }
         
         return v;
      }

      protected internal TextIdentificationFrame (ByteVector data, int offset, FrameHeader h, uint version) : base (h)
      {
         field_list = new StringList ();
         text_encoding = StringType.UTF8;
         ParseFields (FieldData (data, offset, version), version);
         
         // Bad tags may have one or more nul characters at the end of a string,
         // resulting in empty strings at the end of the FieldList. Strip them
         // off.
         while (field_list.Count != 0 && field_list [field_list.Count - 1] == String.Empty)
            field_list.RemoveAt (field_list.Count - 1);
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

      public UserTextIdentificationFrame (ByteVector data, uint version) : base (data, version)
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
            Description = String.Empty;
         
         if(fields <= 1)
            SetText (String.Empty);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected internal UserTextIdentificationFrame (ByteVector data, int offset, FrameHeader h, uint version) : base (data, offset, h, version)
      {
         CheckFields ();
      }
   }
}
