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
      private string title;
      private string author;
      private string copyright;
      private string description;
      private string rating;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public ContentDescriptionObject (Asf.File file, long position) : base (file, position)
      {
         if (!Guid.Equals (Asf.Guid.AsfContentDescriptionObject))
            throw new CorruptFileException ("Object GUID incorrect.");
         
         if (OriginalSize < 34)
            throw new CorruptFileException ("Object size too small.");
         
         short title_length       = file.ReadWord ();
         short author_length      = file.ReadWord ();
         short copyright_length   = file.ReadWord ();
         short description_length = file.ReadWord ();
         short rating_length      = file.ReadWord ();
         
         title       = file.ReadUnicode (title_length);
         author      = file.ReadUnicode (author_length);
         copyright   = file.ReadUnicode (copyright_length);
         description = file.ReadUnicode (description_length);
         rating      = file.ReadUnicode (rating_length);
      }
      
      public ContentDescriptionObject () : base (Asf.Guid.AsfContentDescriptionObject)
      {
         title       = String.Empty;
         author      = String.Empty;
         copyright   = String.Empty;
         description = String.Empty;
         rating      = String.Empty;
      }
      
      public override ByteVector Render ()
      {
         ByteVector title_bytes       = RenderUnicode (title);
         ByteVector author_bytes      = RenderUnicode (author);
         ByteVector copyright_bytes   = RenderUnicode (copyright);
         ByteVector description_bytes = RenderUnicode (description);
         ByteVector rating_bytes      = RenderUnicode (rating);
         
         ByteVector output = RenderWord ((short) title_bytes.Count);
         output.Add (RenderWord ((short) author_bytes.Count));
         output.Add (RenderWord ((short) copyright_bytes.Count));
         output.Add (RenderWord ((short) description_bytes.Count));
         output.Add (RenderWord ((short) rating_bytes.Count));
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
      public string Title       {get {return title;}       set {title = value;}}
      public string Author      {get {return author;}      set {author = value;}}
      public string Copyright   {get {return copyright;}   set {copyright = value;}}
      public string Description {get {return description;} set {description = value;}}
      public string Rating      {get {return rating;}      set {rating = value;}}
   }
}
