using System;
using System.Collections.Generic;

namespace TagLib.Mpeg4 {
	public class FileParser
	{
		/// <summary>
		///    Contains the file to read from.
		/// </summary>
		private TagLib.File file;
		
		/// <summary>
		///    Contains the first header found in the file.
		/// </summary>
		private BoxHeader first_header;
		
		/// <summary>
		///    Contains the ISO movie header box.
		/// </summary>
		private IsoMovieHeaderBox mvhd_box;
		
		/// <summary>
		///    Contains the ISO user data box.
		/// </summary>
		private IsoUserDataBox udta_box;
		
		/// <summary>
		///    Contains the box headers from the top of the file to the
		///    "moov" box.
		/// </summary>
		private BoxHeader [] moov_tree;
		
		/// <summary>
		///    Contains the box headers from the top of the file to the
		///    "udta" box.
		/// </summary>
		private BoxHeader [] udta_tree;
		
		/// <summary>
		///    Contains the "stco" boxes found in the file.
		/// </summary>
		private List<Box> stco_boxes;
		
		/// <summary>
		///    Contains the "stsd" boxes found in the file.
		/// </summary>
		private List<Box> stsd_boxes;
		
		/// <summary>
		///    Contains the position at which the "mdat" box starts.
		/// </summary>
		private long mdat_start = -1;
		
		/// <summary>
		///    Contains the position at which the "mdat" box ends.
		/// </summary>
		private long mdat_end = -1;
		
		public FileParser (TagLib.File file)
		{
			this.file = file;
			first_header = new BoxHeader (file, 0);
			
			if (first_header.BoxType != "ftyp")
				throw new CorruptFileException (
					"File does not start with 'ftyp' box.");
			
			stco_boxes = new List<Box> ();
			stsd_boxes = new List<Box> ();
		}
		
		public IsoMovieHeaderBox MovieHeaderBox {
			get {return mvhd_box;}
		}
		
		public IsoUserDataBox UserDataBox {
			get {return udta_box;}
		}
		
		public IsoAudioSampleEntry AudioSampleEntry {
			get {
				foreach (IsoSampleDescriptionBox box in stsd_boxes)
					foreach (Box sub in box.Children) {
						IsoAudioSampleEntry entry = sub
							as IsoAudioSampleEntry;
						
						if (entry != null)
							return entry;
					}
				return null;
			}
		}
		
		public IsoVisualSampleEntry VisualSampleEntry {
			get {
				foreach (IsoSampleDescriptionBox box in stsd_boxes)
					foreach (Box sub in box.Children) {
						IsoVisualSampleEntry entry = sub
							as IsoVisualSampleEntry;
						
						if (entry != null)
							return entry;
					}
				return null;
			}
		}
		
		public BoxHeader [] MoovTree {
			get {return moov_tree;}
		}
		
		public BoxHeader [] UdtaTree {
			get {return udta_tree;}
		}
		
		public Box [] ChunkOffsetBoxes {
			get {return stco_boxes.ToArray ();}
		}
		
		public long MdatStartPosition {
			get {return mdat_start;}
		}
		
		public long MdatEndPosition {
			get {return mdat_end;}
		}
		
		private static List<BoxHeader> AddParent (List<BoxHeader> parents,
		                                          BoxHeader current)
		{
			List<BoxHeader> boxes = new List<BoxHeader> ();
			if (parents != null)
				boxes.AddRange (parents);
			boxes.Add (current);
			return boxes;
		}
		
		public void ParseBoxHeaders ()
		{
			ParseBoxHeaders (first_header.TotalBoxSize,
				file.Length, null);
		}
		
		private void ParseBoxHeaders (long start, long end,
		                              List<BoxHeader> parents)
		{
			long position = start;
			for (BoxHeader header = new BoxHeader (file, position);
				header.TotalBoxSize != 0 && position < end;
				position += header.TotalBoxSize) {
				header = new BoxHeader (file, position);
				
				if (moov_tree == null &&
					header.BoxType == BoxType.Moov) {
					List<BoxHeader> new_parents = AddParent (
						parents, header);
					moov_tree = new_parents.ToArray ();
					ParseBoxHeaders (
						header.HeaderSize + position,
						header.TotalBoxSize + position,
						new_parents);
				} else if (header.BoxType == BoxType.Mdia ||
					header.BoxType == BoxType.Minf ||
					header.BoxType == BoxType.Stbl ||
					header.BoxType == BoxType.Trak) {
					ParseBoxHeaders (
						header.HeaderSize + position,
						header.TotalBoxSize + position,
						AddParent (parents, header));
				} else if (udta_box == null &&
					header.BoxType == BoxType.Udta) {
					udta_tree = AddParent (parents,
						header).ToArray ();
				} else if (header.BoxType == BoxType.Mdat) {
					mdat_start = position;
					mdat_end = position + header.TotalBoxSize;
				}
			}
		}
		
		public void ParseTag ()
		{
			ParseTag (first_header.TotalBoxSize, file.Length);
		}
		
		private void ParseTag (long start, long end)
		{
			long position = start;
			BoxHeader header;
			
			do {
				header = new BoxHeader (file, position);
				
				if (header.BoxType == BoxType.Moov ||
					header.BoxType == BoxType.Mdia ||
					header.BoxType == BoxType.Minf ||
					header.BoxType == BoxType.Stbl ||
					header.BoxType == BoxType.Trak) {
					ParseTag (header.HeaderSize + position,
						header.TotalBoxSize + position);
				} else if (udta_box == null &&
					header.BoxType == BoxType.Udta) {
					udta_box = BoxFactory.CreateBox (file,
					header) as IsoUserDataBox;
				} else if (header.BoxType == BoxType.Mdat) {
					mdat_start = position;
					mdat_end = position + header.TotalBoxSize;
				}
				
				position += header.TotalBoxSize;
			} while (header.TotalBoxSize != 0 && position < end);
		}
		
		public void ParseTagAndProperties ()
		{
			ParseTagAndProperties (first_header.TotalBoxSize,
				file.Length, null);
		}
		
		private void ParseTagAndProperties (long start, long end,
		                                    IsoHandlerBox handler)
		{
			long position = start;
			BoxHeader header;
			
			do {
				header = new BoxHeader (file, position);
				ByteVector type = header.BoxType;
				
				if (type == BoxType.Moov ||
					type == BoxType.Mdia ||
					type == BoxType.Minf ||
					type == BoxType.Stbl ||
					type == BoxType.Trak) {
					ParseTagAndProperties (
						header.HeaderSize + position,
						header.TotalBoxSize + position,
						handler);
				} else if (type == BoxType.Stsd) {
					stsd_boxes.Add (BoxFactory.CreateBox (
						file, header, handler));
				} else if (type == BoxType.Hdlr) {
					handler = BoxFactory.CreateBox (file,
						header, handler) as
							IsoHandlerBox;
				} else if (mvhd_box == null &&
					type == BoxType.Mvhd) {
					mvhd_box = BoxFactory.CreateBox (file,
						header, handler) as
							IsoMovieHeaderBox;
				} else if (udta_box == null &&
					type == BoxType.Udta) {
					udta_box = BoxFactory.CreateBox (file,
						header, handler) as
							IsoUserDataBox;
				} else if (type == BoxType.Mdat) {
					mdat_start = position;
					mdat_end = position + header.TotalBoxSize;
				}
				
				position += header.TotalBoxSize;
			} while (header.TotalBoxSize != 0 && position < end);
		}
		
		public void ParseChunkOffsets ()
		{
			ParseChunkOffsets (first_header.TotalBoxSize,
				file.Length);
		}
		
		private void ParseChunkOffsets (long start, long end)
		{
			long position = start;
			BoxHeader header;
			
			do {
				header = new BoxHeader (file, position);
				
				if (header.BoxType == BoxType.Moov) {
					ParseChunkOffsets (
						header.HeaderSize + position,
						header.TotalBoxSize + position);
				} else if (header.BoxType == BoxType.Moov ||
					header.BoxType == BoxType.Mdia ||
					header.BoxType == BoxType.Minf ||
					header.BoxType == BoxType.Stbl ||
					header.BoxType == BoxType.Trak) {
					ParseChunkOffsets (
						header.HeaderSize + position,
						header.TotalBoxSize + position);
				} else if (header.BoxType == BoxType.Stco ||
					header.BoxType == BoxType.Co64) {
					stco_boxes.Add (BoxFactory.CreateBox (
						file, header));
				} else if (header.BoxType == BoxType.Mdat) {
					mdat_start = position;
					mdat_end = position + header.TotalBoxSize;
				}
				
				position += header.TotalBoxSize;
			} while (header.TotalBoxSize != 0 && position < end);
		}
	}
}
