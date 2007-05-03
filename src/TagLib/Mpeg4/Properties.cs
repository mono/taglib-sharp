/***************************************************************************
    copyright            : (C) 2006 by Brian Nickel
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

using System.Collections;
using System;

namespace TagLib.Mpeg4
{
   public class Properties : TagLib.Properties
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private IsoMovieHeaderBox   mvhd_box;
      private IsoAudioSampleEntry audio_sample_entry;
      private IsoVisualSampleEntry visual_sample_entry;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Properties (IsoMovieHeaderBox mvhd_box, IsoAudioSampleEntry audio_sample_entry, IsoVisualSampleEntry visual_sample_entry, ReadStyle style) : base (style)
      {
         this.mvhd_box = mvhd_box;
         this.audio_sample_entry = audio_sample_entry;
         this.visual_sample_entry = visual_sample_entry;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      

      public override MediaTypes MediaTypes
      {
         get
         {
            MediaTypes types = MediaTypes.Unknown;
            if (audio_sample_entry != null)
               types |= MediaTypes.Audio;
            if (visual_sample_entry != null)
               types |= MediaTypes.Video;
            return types;
         }
      }
      
      // All additional special info from the Movie Header.
      public double   Rate             {get {return mvhd_box == null ? 1.0 : mvhd_box.Rate;}}
      public double   Volume           {get {return mvhd_box == null ? 1.0 : mvhd_box.Volume;}}
   }
}
