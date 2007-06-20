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
   public class TextInformationFrame : Frame
   {
      #region Private Properties
      private StringType encoding   = StringType.UTF8;
      private StringCollection field_list = new StringCollection ();
      
      // Performance savings for simple processes.
      private ByteVector  raw_data      = null;
      private byte        raw_version   = 0;
      #endregion
      
      
      
      #region Constructors
      public TextInformationFrame (ByteVector type, StringType encoding) :
         base (type, 4)
      {
         this.encoding = encoding;
      }

      public TextInformationFrame (ByteVector type) :
         this (type, Id3v2.Tag.DefaultEncoding)
      {
      }

      public TextInformationFrame (ByteVector data, byte version) :
         base (data, version)
      {
         SetData (data, 0, version, true);
      }

      protected internal TextInformationFrame (ByteVector data, int offset,
                                                  FrameHeader header,
                                                  byte version) : base(header)
      {
         SetData (data, offset, version, false);
      }
      #endregion
      
      
      
      #region Public Properties
      public StringCollection FieldList
      {
        get {ParseRawData (); return new StringCollection (field_list);}
      }
      
      public StringType TextEncoding
      {
         get {ParseRawData (); return encoding;}
         set {encoding = value;}
      }
      #endregion
      
      
      
      #region Public Methods
      public void SetText (StringCollection fields)
      {
         raw_data = null;
         field_list.Clear ();
         field_list.Add (fields);
      }

      public void SetText (params string [] text)
      {
         raw_data = null;
         field_list.Clear ();
         field_list.Add (text);
      }

      public override string ToString ()
      {
         ParseRawData ();
         return field_list.ToString ();
      }
      
      public override ByteVector Render (byte version)
      {
         if (version != 3 || FrameId != FrameType.TDRC)
            return base.Render (version);
      	
         string text = ToString ();
         if (text.Length < 10 || text [4] != '-' || text [7] != '-')
            return base.Render (version);
         
         ByteVector output = new ByteVector ();
         TextInformationFrame f;
         
         f = new TextInformationFrame (FrameType.TYER, encoding);
         f.SetText (text.Substring (0, 4));
         output.Add (f.Render (version));
         
         f = new TextInformationFrame (FrameType.TDAT, encoding);
         f.SetText (text.Substring (5, 2) + text.Substring (8, 2));
         output.Add (f.Render (version));
         
         if (text.Length < 16 || text [10] != 'T' || text [13] != ':')
            return output;
         
         f = new TextInformationFrame (FrameType.TIME, encoding);
         f.SetText (text.Substring (11, 2) + text.Substring (14, 2));
         output.Add (f.Render (version));
         
         return output;
      }
      #endregion
      
      
      
      #region Public Static Methods
      public static TextInformationFrame Get (Tag tag, ByteVector type, StringType encoding, bool create)
      {
         foreach (Frame f in tag.GetFrames (type))
            if (f is TextInformationFrame)
               return f as TextInformationFrame;
         
         if (!create)
            return null;
         
         TextInformationFrame frame = new TextInformationFrame (type, encoding);
         tag.AddFrame (frame);
         return frame;
      }
      
      public static TextInformationFrame Get (Tag tag, ByteVector type, bool create)
      {
         return Get (tag, type, Tag.DefaultEncoding, create);
      }
      
      public static TextInformationFrame Get (Tag tag, ByteVector type)
      {
         return Get (tag, type, false);
      }
      #endregion
      
      
      
      #region Protected Methods
      protected override void ParseFields (ByteVector data, byte version)
      {
         raw_data = data;
         raw_version = version;
      }
      
      protected void ParseRawData ()
      {
         if (raw_data == null)
            return;
         
         ByteVector data = raw_data;
         raw_data = null;
         
         // read the string data type (the first byte of the field data)
         encoding = (StringType) data [0];
         field_list.Clear ();
         
         if (raw_version > 3 || FrameId == FrameType.TXXX)
            field_list.Add (data.ToStrings (encoding, 1));
         else
         {
            string value = data.ToString (encoding, 1);

            if (value.Length == 0 || value [0] == 0)
               goto done;
            
            // Do a fast removal of end bytes.
            if (value.Length > 1 && value [value.Length - 1] == 0)
               for (int i = value.Length - 1; i >= 0; i --)
                  if (value [i] != 0)
                  {
                     value = value.Substring (0, i + 1);
                     break;
                  }
            
            if (FrameId == FrameType.TCOM ||
                FrameId == FrameType.TEXT ||
                FrameId == FrameType.TOLY ||
                FrameId == FrameType.TOPE ||
                FrameId == FrameType.TPE1 ||
                FrameId == FrameType.TPE2 ||
                FrameId == FrameType.TPE3 ||
                FrameId == FrameType.TPE4)
                field_list.Add (value.Split (new char []{'/'}));
            else if (FrameId == FrameType.TCON)
            {
               while (value.Length > 1 && value [0] == '(')
               {
                  int closing = value.IndexOf (')');
                     if (closing < 0)
                        break;
            
                  field_list.Add (value.Substring (1, closing - 1));
                  
                  value = value.Substring (closing + 1);
               }
               
               if (value.Length > 0)
                  field_list.Add (value);
            }
            else
               field_list.Add (value);
         }
         
         done:
         // Bad tags may have one or more nul characters at the end of a string,
         // resulting in empty strings at the end of the FieldList. Strip them
         // off.
         while (field_list.Count != 0 && string.IsNullOrEmpty (field_list [field_list.Count - 1]))
            field_list.RemoveAt (field_list.Count - 1);
      }

      protected override ByteVector RenderFields (byte version)
      {
         if (raw_data != null && raw_version == version)
            return raw_data;
         
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
                  v.Add (ByteVector.TextDelimiter (encoding));
            
               v.Add (ByteVector.FromString (field_list [i], encoding));
            }
         }
         else
         {
            if (FrameId == FrameType.TCON)
            {
               byte id;
               string data = string.Empty;
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
   
   
   
   public class UserTextInformationFrame : TextInformationFrame
   {
      #region Constructors
      public UserTextInformationFrame (string description, StringType encoding) : base (FrameType.TXXX, encoding)
      {
         StringCollection l = new StringCollection ();
         l.Add (description);
         l.Add ((string)null);
         
         base.SetText (l);
      }
      
      public UserTextInformationFrame (ByteVector data, byte version) : base (data, version)
      {
         CheckFields ();
      }
      
      protected internal UserTextInformationFrame (ByteVector data, int offset, FrameHeader header, byte version) : base (data, offset, header, version)
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
            StringCollection l = new StringCollection (base.FieldList);

            if (l.IsEmpty)
               l.Add (value);
            else
               l [0] = value;

            base.SetText (l);
         }
      }

      new public StringCollection FieldList
      {
         get
         {
            StringCollection l = new StringCollection (base.FieldList);
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
         StringCollection l = new StringCollection (Description);
         l.Add (text);
         
         base.SetText (l);
      }

      new public void SetText (StringCollection fields)
      {
         StringCollection l = new StringCollection (Description);
         l.Add (fields);
         
         base.SetText (l);
      }
      #endregion
      
      
      
      #region Public Static Methods
      public static UserTextInformationFrame Get (Tag tag, string description, StringType type, bool create)
      {
         foreach (Frame f in tag.GetFrames (FrameType.TXXX))
            if (f is UserTextInformationFrame && (f as UserTextInformationFrame).Description == description)
               return f as UserTextInformationFrame;
         
         if (!create)
            return null;
         
         UserTextInformationFrame frame = new UserTextInformationFrame (description, type);
         tag.AddFrame (frame);
         return frame;
      }

      public static UserTextInformationFrame Get (Tag tag, string description, bool create)
      {
         return Get (tag, description, TagLib.Id3v2.Tag.DefaultEncoding, create);
      }
      
      public static UserTextInformationFrame Get (Tag tag, string description)
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
