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
   public class MovieIdTag : ListTag
   {
      public MovieIdTag () : base ()
      {}
      
      public MovieIdTag (ByteVector data) : base (data)
      {}
      
      public MovieIdTag (TagLib.File file, long position, int length) : base (file, position, length)
      {}
      
      public override ByteVector RenderEnclosed ()
      {
         return RenderEnclosed ("MID ");
      }

      public override TagTypes TagTypes {get {return TagTypes.MovieId;}}
      
      public override string Title
      {
         get
         {
            foreach (string s in GetValuesAsStringCollection ("TITL"))
               if (s != null)
                  return s;
            
            return null;
         }
         set {SetValue ("TITL", value);}
      }
      
      public override string [] Performers
      {
         get {return GetValuesAsStringCollection ("IART").ToArray ();}
         set {SetValue ("IART", value);}
      }
      
      public override string Comment
      {
         get
         {
            foreach (string s in GetValuesAsStringCollection ("COMM"))
               if (s != null)
                  return s;
            
            return null;
         }
         set {SetValue ("COMM", value);}
      }
      
      public override string [] Genres
      {
         get {return GetValuesAsStringCollection ("GENT").ToArray ();}
         set {SetValue ("GENR", value);}
      }
      
      public override uint Track
      {
         get {return GetValueAsUInt ("PRT1");}
         set {SetValue ("PRT1", value);}
      }
      
      public override uint TrackCount
      {
         get {return GetValueAsUInt ("PRT2");}
         set {SetValue ("PRT2", value);}
      }
   }
}