//
// MovieIdTag.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System;

namespace TagLib.Riff {
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

#region TagLib.Tag
		
		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    Always <see cref="TagTypes.MovieId" />.
		/// </value>
		public override TagTypes TagTypes {
			get {return TagTypes.MovieId;}
		}
		
		/// <summary>
		///    Gets and sets the title for the media described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the title for
		///    the media described by the current instance or <see
		///    langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TITL" item.
		/// </remarks>
		public override string Title {
			get {
				foreach (string s in GetValuesAsStrings ("TITL"))
					if (s != null)
						return s;
				
				return null;
			}
			set {SetValue ("TITL", value);}
		}
		
		/// <summary>
		///    Gets and sets the performers or artists who performed in
		///    the media described by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the performers or
		///    artists who performed in the media described by the
		///    current instance or an empty array if no value is
		///    present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "IART" item.
		/// </remarks>
		public override string [] Performers {
			get {return GetValuesAsStrings ("IART");}
			set {SetValue ("IART", value);}
		}
		
		public override string Comment {
			get {
				foreach (string s in GetValuesAsStrings ("COMM"))
					if (s != null)
						return s;
				return null;
			}
			set {SetValue ("COMM", value);}
		}
		
		public override string [] Genres {
			get {return GetValuesAsStrings ("GENR");}
			set {SetValue ("GENR", value);}
		}
		
		public override uint Track {
			get {return GetValueAsUInt ("PRT1");}
			set {SetValue ("PRT1", value);}
		}
		
		public override uint TrackCount {
			get {return GetValueAsUInt ("PRT2");}
			set {SetValue ("PRT2", value);}
		}
#endregion
	}
}
