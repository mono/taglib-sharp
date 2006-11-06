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
   public abstract class Object
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Guid id;
      private long size;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Object (Asf.File file, long position)
      {
         file.Seek (position);
         id = file.ReadGuid ();
         size = file.ReadQWord ();
      }
      
      public Object (Guid guid)
      {
         id = guid;
         size = 0;
      }
      
      public abstract ByteVector Render ();
      
      public static ByteVector RenderUnicode (string str)
      {
         ByteVector v = ByteVector.FromString (str, StringType.UTF16LE);
         v.Add (ByteVector.FromShort (0));
         return v;
      }
      
      public static ByteVector RenderDWord (uint value)
      {
         return ByteVector.FromUInt (value, false);
      }
      
      public static ByteVector RenderQWord (long value)
      {
         return ByteVector.FromLong (value, false);
      }
      
      public static ByteVector RenderWord (short value)
      {
         return ByteVector.FromShort (value, false);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public Guid Guid {get {return id;}}
      
      public long OriginalSize {get {return size;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected ByteVector Render (ByteVector data)
      {
         long length = (data != null ? data.Count : 0) + 24;
         ByteVector v = Guid.Render ();
         v.Add (RenderQWord (length));
         v.Add (data);
         return v;
      }
   }
}
