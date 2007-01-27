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

namespace TagLib.Asf
{
   public class PaddingObject : Object
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private long size;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public PaddingObject (Asf.File file, long position) : base (file, position)
      {
         if (!Guid.Equals (Asf.Guid.AsfPaddingObject))
            throw new CorruptFileException ("Object GUID incorrect.");
         
         if (OriginalSize < 24)
            throw new CorruptFileException ("Object size too small.");
         
         size = OriginalSize;
      }
      
      public PaddingObject (uint size) : base (Asf.Guid.AsfPaddingObject)
      {
         this.size = size;
      }
      
      public override ByteVector Render ()
      {
         return Render (new ByteVector ((int) (size - 24)));
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public long Size {get {return size;} set {size = value;}}
   }
}
