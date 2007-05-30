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
      private System.Guid id;
      private ulong size;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      protected Object (Asf.File file, long position)
      {
         if (file == null)
            throw new System.ArgumentNullException ("file");
         
         file.Seek (position);
         id = file.ReadGuid ();
         size = file.ReadQWord ();
      }
      
      protected Object (System.Guid guid)
      {
         id = guid;
      }
      
      public abstract ByteVector Render ();
      
      public static ByteVector RenderUnicode (string value)
      {
         ByteVector v = ByteVector.FromString (value, StringType.UTF16LE);
         v.Add (ByteVector.FromUShort (0));
         return v;
      }
      
      public static ByteVector RenderDWord (uint value)
      {
         return ByteVector.FromUInt (value, false);
      }
      
      public static ByteVector RenderQWord (ulong value)
      {
         return ByteVector.FromULong (value, false);
      }
      
      public static ByteVector RenderWord (ushort value)
      {
         return ByteVector.FromUShort (value, false);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public System.Guid Guid {get {return id;}}
      
      public ulong OriginalSize {get {return size;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected ByteVector Render (ByteVector data)
      {
         ulong length = (ulong)((data != null ? data.Count : 0) + 24);
         ByteVector v = id.ToByteArray ();
         v.Add (RenderQWord (length));
         v.Add (data);
         return v;
      }
   }
}
