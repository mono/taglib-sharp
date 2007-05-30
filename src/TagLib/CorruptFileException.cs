/***************************************************************************
    copyright            : (C) 2006 Novell, Inc.
    email                : abockover@novell.com
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
using System.Runtime.Serialization;

namespace TagLib
{
   [Serializable]
   public class CorruptFileException : Exception 
   {
      public CorruptFileException (string message) : base(message) 
      {
      }
      
      public CorruptFileException () : base()
      {
      }
      
      public CorruptFileException (string message, Exception innerException) : base (message, innerException)
      {
      }
      
      protected CorruptFileException (SerializationInfo info, StreamingContext context) : base(info, context)
      {
      }
   }
}
