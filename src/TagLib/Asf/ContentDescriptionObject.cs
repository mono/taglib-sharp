/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
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

using System;

namespace TagLib.Asf
{
   public class ContentDescriptionObject : Object
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private string _title       = string.Empty;
      private string _author      = string.Empty;
      private string _copyright   = string.Empty;
      private string _description = string.Empty;
      private string _rating      = string.Empty;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public ContentDescriptionObject (Asf.File file, long position) : base (file, position)
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         if (Guid != Asf.Guid.AsfContentDescriptionObject)
            throw new CorruptFileException ("Object GUID incorrect.");
         
         if (OriginalSize < 34)
            throw new CorruptFileException ("Object size too small.");
         
         ushort title_length       = file.ReadWord ();
         ushort author_length      = file.ReadWord ();
         ushort copyright_length   = file.ReadWord ();
         ushort description_length = file.ReadWord ();
         ushort rating_length      = file.ReadWord ();
         
         _title       = file.ReadUnicode (title_length);
         _author      = file.ReadUnicode (author_length);
         _copyright   = file.ReadUnicode (copyright_length);
         _description = file.ReadUnicode (description_length);
         _rating      = file.ReadUnicode (rating_length);
      }
      
      public ContentDescriptionObject () : base (Asf.Guid.AsfContentDescriptionObject)
      {
      }
      
      public override ByteVector Render ()
      {
         ByteVector title_bytes       = RenderUnicode (_title);
         ByteVector author_bytes      = RenderUnicode (_author);
         ByteVector copyright_bytes   = RenderUnicode (_copyright);
         ByteVector description_bytes = RenderUnicode (_description);
         ByteVector rating_bytes      = RenderUnicode (_rating);
         
         ByteVector output = RenderWord ((ushort) title_bytes.Count);
         output.Add (RenderWord ((ushort) author_bytes.Count));
         output.Add (RenderWord ((ushort) copyright_bytes.Count));
         output.Add (RenderWord ((ushort) description_bytes.Count));
         output.Add (RenderWord ((ushort) rating_bytes.Count));
         output.Add (title_bytes);
         output.Add (author_bytes);
         output.Add (copyright_bytes);
         output.Add (description_bytes);
         output.Add (rating_bytes);
         
         return Render (output);
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public string Title       {get {return _title;}       set {_title = value;}}
      public string Author      {get {return _author;}      set {_author = value;}}
      public string Copyright   {get {return _copyright;}   set {_copyright = value;}}
      public string Description {get {return _description;} set {_description = value;}}
      public string Rating      {get {return _rating;}      set {_rating = value;}}
      
      public bool IsEmpty
      {
         get
         {
            return string.IsNullOrEmpty (_title)
                && string.IsNullOrEmpty (_author)
                && string.IsNullOrEmpty (_copyright)
                && string.IsNullOrEmpty (_description)
                && string.IsNullOrEmpty (_rating);
         }
      }
   }
}
