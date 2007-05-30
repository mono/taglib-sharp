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
      private DataType   _type   = DataType.Unicode;
      private string     _name   = null;
      private string     _sValue = null;
      private ByteVector _bValue = null;
      private ulong      _lValue = 0;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public ContentDescriptor (string name, string value)
      {
         _name   = name;
         _type   = DataType.Unicode;
         _sValue = value;
      }

      public ContentDescriptor (string name, ByteVector value)
      {
         _name   = name;
         _type   = DataType.Bytes;
         _bValue = new ByteVector (value);
      }

      public ContentDescriptor (string name, uint value)
      {
         _name   = name;
         _type   = DataType.DWord;
         _lValue = value;
      }

      public ContentDescriptor (string name, ulong value)
      {
         _name   = name;
         _type   = DataType.QWord;
         _lValue = value;
      }

      public ContentDescriptor (string name, ushort value)
      {
         _name   = name;
         _type   = DataType.Word;
         _lValue = value;
      }

      public ContentDescriptor (string name, bool value)
      {
         _name   = name;
         _type   = DataType.Bool;
         _lValue = (ulong)(value ? 1 : 0);
      }

      public string Name {get {return _name;}}

      public DataType Type {get {return _type;}}

      public override string ToString ()
      {
         return _sValue;
      }

      public ByteVector ToByteVector ()
      {
         return _bValue;
      }

      public bool ToBool ()
      {
         return _lValue != 0;
      }

      public uint ToDWord ()
      {
         uint value;
         if (_type == DataType.Unicode && _sValue != null && uint.TryParse (_sValue, out value))
            return value;
         
         return (uint) _lValue;
      }

      public ulong ToQWord ()
      {
         ulong value;
         if (_type == DataType.Unicode && _sValue != null && ulong.TryParse (_sValue, out value))
            return value;
         
         return _lValue;
      }

      public ushort ToWord ()
      {
         ushort value;
         if (_type == DataType.Unicode && _sValue != null && ushort.TryParse (_sValue, out value))
            return value;
         
         return (ushort) _lValue;
      }

      public ByteVector Render ()
      {

         ByteVector value = Object.RenderUnicode (_name);
         
         ByteVector data = Object.RenderWord ((ushort) value.Count);
         data.Add (value);
         data.Add (Object.RenderWord ((ushort) _type));
         
         switch (_type)
         {
            case DataType.Unicode:
               value = Object.RenderUnicode (_sValue);
               data.Add (Object.RenderWord ((ushort) value.Count));
               data.Add (value);
            break;
            
            case DataType.Bytes:
               data.Add (Object.RenderWord ((ushort) _bValue.Count));
               data.Add (_bValue);
            break;
            
            case DataType.Bool:
               data.Add (Object.RenderWord (4));
               data.Add (Object.RenderDWord ((uint) _lValue));
            break;
            
            case DataType.DWord:
               data.Add (Object.RenderWord (4));
               data.Add (Object.RenderDWord ((uint) _lValue));
            break;
            
            case DataType.QWord:
               data.Add (Object.RenderWord (8));
               data.Add (Object.RenderQWord (_lValue));
            break;
            
            case DataType.Word:
               data.Add (Object.RenderWord (2));
               data.Add (Object.RenderWord ((ushort) _lValue));
            break;
            
            default:  
               return null; 
         }
         
         return data;
      }
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected internal ContentDescriptor (Asf.File file)
      {
         Parse (file);
      }

      protected bool Parse (Asf.File file)
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         int size = file.ReadWord ();
         _name = file.ReadUnicode (size);
         
         _type = (DataType) file.ReadWord ();
         size = file.ReadWord ();

         switch (_type)
         {
            case DataType.Word:
               _lValue = file.ReadWord ();
            break;
            
            case DataType.Bool:
               _lValue = file.ReadDWord ();
            break;
            
            case DataType.DWord:
               _lValue = file.ReadDWord ();
            break;
            
            case DataType.QWord:  
               _lValue = file.ReadQWord ();
            break;
            
            case DataType.Unicode:  
               _sValue = file.ReadUnicode (size);
            break;
            
            case DataType.Bytes:  
               _bValue = file.ReadBlock (size);
            break;
            
            default:
               return false; 
         }
         
         return true;
      }
   }
}
