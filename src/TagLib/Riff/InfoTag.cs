/***************************************************************************
    copyright            : (C) 2007 by Brian Nickel
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
namespace TagLib.Riff
{
   public class InfoTag : ListTag
   {
      public InfoTag () : base ()
      {}
      
      public InfoTag (ByteVector data) : base (data)
      {}
      
      public InfoTag (TagLib.File file, long position, int length) : base (file, position, length)
      {}
      
      public override ByteVector RenderEnclosed ()
      {
         return RenderEnclosed ("INFO");
      }

      public override string Title
      {
         get {return GetValueAsString ("INAM");}
         set {SetValue ("INAM", value);}
      }
      
      public override string [] AlbumArtists
      {
         get {return GetValueAsStringList ("IART").ToArray ();}
         set {SetValue ("IART", value);}
      }
      
      public override string [] Performers
      {
         get {return GetValueAsStringList ("ISTR").ToArray ();}
         set {SetValue ("ISTR", value);}
      }
      
      public override string [] Composers
      {
         get {return GetValueAsStringList ("IWRI").ToArray ();}
         set {SetValue ("IWRI", value);}
      }
      
      public override string Comment
      {
         get {return GetValueAsString ("ICMT");}
         set {SetValue ("ICMT", value);}
      }
      
      public override string [] Genres
      {
         get {return GetValueAsStringList ("IGNR").ToArray ();}
         set {SetValue ("IGNR", value);}
      }
      
      public override uint Year
      {
         get {return GetValueAsUInt ("ICRD");}
         set {SetValue ("ICRD", value);}
      }
      
      public override uint Track
      {
         get {return GetValueAsUInt ("IPRT");}
         set {SetValue ("IPRT", value);}
      }
      
      public override uint TrackCount
      {
         get {return GetValueAsUInt ("IFRM");}
         set {SetValue ("IFRM", value);}
      }
   }
}