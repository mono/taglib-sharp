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

using System.Collections.Generic;
using System;

namespace TagLib.Asf {
	public class HeaderExtensionObject : Object
	{
		#region Private Fields
		
		private List<Object> children;
		
		#endregion
		
		
		
		#region Constructors
		
		public HeaderExtensionObject (Asf.File file, long position) : base (file, position)
		{
			if (!Guid.Equals (Asf.Guid.AsfHeaderExtensionObject))
				throw new CorruptFileException ("Object GUID incorrect.");
			
			children = new List<Object> ();
			
			if (file.ReadGuid () != Asf.Guid.AsfReserved1)
				throw new CorruptFileException ("Reserved1 GUID expected.");
			
			if (file.ReadWord () != 6)
				throw new CorruptFileException ("Invalid reserved WORD. Expected '6'.");
			
			uint size_remaining = file.ReadDWord ();
			position += 0x170 / 8;
			
			while (size_remaining > 0) {
				Object obj = file.ReadObject (position);
				position += (long) obj.OriginalSize;
				size_remaining -= (uint) obj.OriginalSize;
				children.Add (obj);
			}
		}
		
		#endregion
		
		
		
		#region Public Properties
		
		public IEnumerable<Object> Children {get {return children;}}
		
		#endregion
		
		
		
		#region Public Methods
		
		public override ByteVector Render ()
		{
			ByteVector output = new ByteVector ();
			
			foreach (Object child in children)
				output.Add (child.Render ());
			
			output.Insert (0, RenderDWord ((uint) output.Count));
			output.Insert (0, RenderWord (6));
			output.Insert (0, Asf.Guid.AsfReserved1.ToByteArray ());
			
			return Render (output);
		}
		
		public void AddObject (Object obj)
		{
			children.Add (obj);
		}
		
		public void AddUniqueObject (Object obj)
		{
			for (int i = 0; i < children.Count; i ++)
				if (((Object) children [i]).Guid == obj.Guid) {
					children [i] = obj;
					return;
				}
			
			children.Add (obj);
		}
		
		#endregion
	}
}
