/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2frame.cpp from TagLib
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
   public enum DataType
   {
      Unicode = 0,
      Bytes   = 1,
      Bool    = 2,
      DWord   = 3,
      QWord   = 4,
      Word    = 5
   }
   
   public class ContentDescriptor
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private DataType   type;
      private string     name;
      private string     sValue;
      private ByteVector bvValue;
      private long       lValue;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public ContentDescriptor (string name, string value) : this ()
      {
         this.name   = name;
         this.type   = DataType.Unicode;
         this.sValue = value;
      }

      public ContentDescriptor (string name, ByteVector value) : this ()
      {
         this.name    = name;
         this.type    = DataType.Bytes;
         this.bvValue = new ByteVector (value);
      }

      public ContentDescriptor (string name, uint value) : this ()
      {
         this.name   = name;
         this.type   = DataType.DWord;
         this.lValue = value;
      }

      public ContentDescriptor (string name, long value) : this ()
      {
         this.name   = name;
         this.type   = DataType.QWord;
         this.lValue = value;
      }

      public ContentDescriptor (string name, short value) : this ()
      {
         this.name   = name;
         this.type   = DataType.Word;
         this.lValue = value;
      }

      public ContentDescriptor (string name, bool value) : this ()
      {
         this.name   = name;
         this.type   = DataType.Bool;
         this.lValue = value ? 1 : 0;
      }

      public string Name {get {return name;}}

      public DataType Type {get {return type;}}

      public override string ToString ()
      {
         return sValue;
      }

      public ByteVector ToByteVector ()
      {
         return bvValue;
      }

      public bool ToBool ()
      {
         return lValue != 0;
      }

      public uint ToDWord ()
      {
         uint value;
         if (type == DataType.Unicode && sValue != null && uint.TryParse (sValue, out value))
            return value;
         
         return (uint) lValue;
      }

      public long ToQWord ()
      {
         long value;
         if (type == DataType.Unicode && sValue != null && long.TryParse (sValue, out value))
            return value;
         
         return (long) lValue;
      }

      public short ToWord ()
      {
         short value;
         if (type == DataType.Unicode && sValue != null && short.TryParse (sValue, out value))
            return value;
         
         return (short) lValue;
      }

      public ByteVector Render ()
      {

         ByteVector v = Object.RenderUnicode (name);
         
         ByteVector data = Object.RenderWord ((short) v.Count);
         data.Add (v);
         data.Add (Object.RenderWord ((short) type));
         
         switch (type)
         {
            case DataType.Unicode:
               v = Object.RenderUnicode (sValue);
               data.Add (Object.RenderWord ((short) v.Count));
               data.Add (v);
            break;
            
            case DataType.Bytes:
               data.Add (Object.RenderWord ((short) bvValue.Count));
               data.Add (bvValue);
            break;
            
            case DataType.Bool:
               data.Add (Object.RenderWord (4));
               data.Add (Object.RenderDWord ((uint) lValue));
            break;
            
            case DataType.DWord:
               data.Add (Object.RenderWord (4));
               data.Add (Object.RenderDWord ((uint) lValue));
            break;
            
            case DataType.QWord:
               data.Add (Object.RenderWord (8));
               data.Add (Object.RenderQWord (lValue));
            break;
            
            case DataType.Word:
               data.Add (Object.RenderWord (2));
               data.Add (Object.RenderWord ((short) lValue));
            break;
            
            default:  
               return null; 
         }
         
         return data;
      }      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected ContentDescriptor ()
      {
         type    = DataType.Unicode;
         name    = null;
         sValue  = null;
         bvValue = null;
         lValue  = 0;
      }

      protected internal ContentDescriptor (Asf.File file) : this ()
      {
         Parse (file);
      }

      protected bool Parse (Asf.File file)
      {
         int size = file.ReadWord ();
         name = file.ReadUnicode (size);
         
         type = (DataType) file.ReadWord ();
         size = file.ReadWord ();

         switch (type)
         {
            case DataType.Word:
               lValue = file.ReadWord ();
            break;
            
            case DataType.Bool:
               lValue = file.ReadDWord ();
            break;
            
            case DataType.DWord:
               lValue = file.ReadDWord ();
            break;
            
            case DataType.QWord:  
               lValue = file.ReadQWord ();
            break;
            
            case DataType.Unicode:  
               sValue = file.ReadUnicode (size);
            break;
            
            case DataType.Bytes:  
               bvValue = file.ReadBlock (size);
            break;
            
            default:
               return false; 
         }
         
         return true;
      }
   }
}
