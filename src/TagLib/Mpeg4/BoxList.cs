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

namespace TagLib.Mpeg4
{
   public class BoxList : ListBase<Box>
   {
      public BoxList() 
      {}
        
      public Box Get (ByteVector type)
      {
         foreach (Box box in this)
            if (box.BoxType == type)
               return box;
         
         return null;
      }
      
      public Box Get (System.Type type)
      {
         foreach (Box box in this)
            if (box.GetType () == type)
               return box;
         
         return null;
      }
      
      public Box GetRecursively (ByteVector type)
      {
         foreach (Box box in this)
            if (box.BoxType == type)
               return box;
         
         foreach (Box box in this)
         {
            if (box.Children == null)
               continue;
            
            Box child_box = box.Children.GetRecursively (type);
            if (child_box != null)
               return child_box;
         }
         
         return null;
      }
      
      public Box GetRecursively (System.Type type)
      {
         foreach (Box box in this)
            if (box.GetType () == type)
               return box;
         
         foreach (Box box in this)
         {
            if (box.Children == null)
               continue;
            
            Box child_box = box.Children.GetRecursively (type);
            if (child_box != null)
               return child_box;
         }
         
         return null;
      }
      
      public void RemoveByType (ByteVector type)
      {
         foreach (Box b in this)
            if (b.BoxType == type)
               Remove (b);
      }
      
      public void RemoveByType (System.Type type)
      {
         foreach (Box b in this)
            if (b.GetType () == type)
               Remove (b);
      }
   }
}
