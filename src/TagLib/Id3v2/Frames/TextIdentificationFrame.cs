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
      #region Private Properties
      private StringType encoding   = StringType.UTF8;
      private StringList field_list = new StringList ();
      #endregion
      
      
      
      #region Constructors
      public TextIdentificationFrame (ByteVector type, StringType encoding) : base (type, 4)
      {
         this.encoding = encoding;
      }

      public TextIdentificationFrame (ByteVector data, byte version) : base (data, version)
      {
         SetData (data, 0, version, true);
      }

      protected internal TextIdentificationFrame (ByteVector data, int offset, FrameHeader h, byte version) : base (h)
      {
         SetData (data, offset, version, false);
         
         // Bad tags may have one or more nul characters at the end of a string,
         // resulting in empty strings at the end of the FieldList. Strip them
         // off.
         while (field_list.Count != 0 && field_list [field_list.Count - 1] == String.Empty)
            field_list.RemoveAt (field_list.Count - 1);
      }
      #endregion
      
      
      
      #region Public Properties
      public StringList FieldList
      {
        get {return new StringList (field_list);}
      }
      
      public StringType TextEncoding
      {
         get {return encoding;}
         set {encoding = value;}
      }
      #endregion
      
      
      
      #region Public Methods
      public void SetText (StringList fields)
      {
         field_list.Clear ();
         field_list.Add (fields);
      }

      public void SetText (params string [] text)
      {
         field_list.Clear ();
         field_list.Add (text);
      }

      public override string ToString ()
      {
         return field_list.ToString ();
      }
      #endregion
      
      
      
      #region Public Static Methods
      public static TextIdentificationFrame Get (Tag tag, ByteVector type, StringType encoding, bool create)
      {
         foreach (Frame f in tag.GetFrames (type))
            if (f is TextIdentificationFrame)
               return f as TextIdentificationFrame;
         
         if (!create)
            return null;
         
         TextIdentificationFrame frame = new TextIdentificationFrame (type, encoding);
         tag.AddFrame (frame);
         return frame;
      }
      
      public static TextIdentificationFrame Get (Tag tag, ByteVector type, bool create)
      {
         return Get (tag, type, Tag.DefaultEncoding, create);
      }
      
      public static TextIdentificationFrame Get (Tag tag, ByteVector type)
      {
         return Get (tag, type, false);
      }
      #endregion
      
      
      
      #region Protected Methods
      protected override void ParseFields (ByteVector data, byte version)
      {
         // read the string data type (the first byte of the field data)
         encoding = (StringType) data [0];
         field_list.Clear ();
         
         if (version > 3 || FrameId == "TXXX")
            field_list.Add (data.ToStrings (encoding, 1));
         else
         {
            string value = data.ToString (encoding, 1);

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
            
            if (FrameId == "TCOM" ||
                FrameId == "TEXT" ||
                FrameId == "TOLY" ||
                FrameId == "TOPE" ||
                FrameId == "TPE1" ||
                FrameId == "TPE2" ||
                FrameId == "TPE3" ||
                FrameId == "TPE4")
                field_list.Add (value.Split (new char []{'/'}));
            else if (FrameId == "TCON")
            {
               while (value.Length > 1 && value [0] == '(')
               {
                  int closing = value.IndexOf (')');
                     if (closing < 0)
                        break;
            
                  field_list.Add (value.Substring (1, closing - 1));
                  
                  value = value.Substring (closing + 1);
               }
               
               if (value != string.Empty)
                  field_list.Add (value);
            }
            else
               field_list.Add (value);
         }
      }

      protected override ByteVector RenderFields (byte version)
      {
         StringType encoding = CorrectEncoding (TextEncoding, version);
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
            if (FrameId == "TCON")
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
      #endregion
   }
   
   
   
   public class UserTextIdentificationFrame : TextIdentificationFrame
   {
      #region Constructors
      public UserTextIdentificationFrame (string description, StringType encoding) : base ("TXXX", encoding)
      {
         StringList l = new StringList ();
         l.Add (description);
         l.Add ((string)null);
         
         base.SetText (l);
      }
      
      public UserTextIdentificationFrame (ByteVector data, byte version) : base (data, version)
      {
         CheckFields ();
      }
      
      protected internal UserTextIdentificationFrame (ByteVector data, int offset, FrameHeader h, byte version) : base (data, offset, h, version)
      {
         CheckFields ();
      }
      #endregion
      
      
      
      #region Public Properties
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
      #endregion
      
      
      
      #region Public Methods
      public override string ToString ()
      {
         return "[" + Description + "] " + FieldList.ToString ();
      }

      new public void SetText (string [] text)
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
      #endregion
      
      
      
      #region Public Static Methods
      public static UserTextIdentificationFrame Get (Tag tag, string description, StringType type, bool create)
      {
         foreach (Frame f in tag.GetFrames ("TXXX"))
            if (f is UserTextIdentificationFrame && (f as UserTextIdentificationFrame).Description == description)
               return f as UserTextIdentificationFrame;
         
         if (!create)
            return null;
         
         UserTextIdentificationFrame frame = new UserTextIdentificationFrame (description, type);
         tag.AddFrame (frame);
         return frame;
      }

      public static UserTextIdentificationFrame Get (Tag tag, string description, bool create)
      {
         return Get (tag, description, TagLib.Id3v2.Tag.DefaultEncoding, create);
      }
      
      public static UserTextIdentificationFrame Get (Tag tag, string description)
      {
         return Get (tag, description, false);
      }
      #endregion
      
      
      
      #region Private Properties
      private void CheckFields ()
      {
         int fields = base.FieldList.Count;
         
         if (fields == 0)
            Description = String.Empty;
         
         if(fields <= 1)
            SetText (String.Empty);
      }
      #endregion
   }
}
