/***************************************************************************
    copyright            : (C) 2006 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : audioproperties.cpp from TagLib
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

namespace TagLib
{
   [Flags]
   public enum MediaTypes
   {
      None  = 0,
      Audio = 1,
      Video = 2
   }
   
   public interface ICodec
   {
      TimeSpan   Duration    {get;}
      MediaTypes MediaTypes  {get;}
      string     Description {get;}
   }
   
   public interface IAudioCodec : ICodec
   {
      int AudioBitrate    {get;}
      int AudioSampleRate {get;}
      int AudioChannels   {get;}
   }
   
   public interface IVideoCodec : ICodec
   {
      int VideoWidth  {get;}
      int VideoHeight {get;}
   }
}