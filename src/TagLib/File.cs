/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
                         : (C) 2006 Novell, Inc.
    email                : brian.nickel@gmail.com
                         : Aaron Bockover <abockover@novell.com>
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
using System.Collections.Generic;
using System.Globalization;

namespace TagLib {
	public enum ReadStyle {
		None = 0,
		// Fast = 1,
		Average = 2,
		// Accurate = 3
	}
	
	public abstract class File
	{
		#region Enums
		
		public enum AccessMode {
			Read,
			Write,
			Closed
		}
		
		#endregion
		
		
		
		#region Delegates
		
		public delegate File FileTypeResolver (IFileAbstraction abstraction,
		                                       string mimetype,
		                                       ReadStyle style);
		
		#endregion
		
		
		
		#region Private Properties
		
		private System.IO.Stream file_stream;
		private IFileAbstraction file_abstraction;
		private string mime_type;
		private TagTypes tags_on_disk = TagTypes.None;
		private static int buffer_size = 1024;
		private static List<FileTypeResolver> file_type_resolvers
			= new List<FileTypeResolver> ();
		
		#endregion
		
		
		
		#region Public Static Properties
		
		public static uint BufferSize {
			get {return (uint) buffer_size;}
		}
		
		#endregion
		
		
		#region Constructors
		
		protected File (string path)
			: this (new LocalFileAbstraction (path))
		{
		}
		
		protected File (IFileAbstraction abstraction)
		{
			file_abstraction = abstraction;
		}
		
		#endregion
		
		
		
		#region Public Properties
		
		public abstract Tag Tag {get;}
		public abstract Properties Properties {get;}
		
		public TagTypes TagTypesOnDisk {
			get {return tags_on_disk;}
			protected set {tags_on_disk = value;}
		}
		
		public TagTypes TagTypes {
			get {return Tag != null ? Tag.TagTypes : TagTypes.None;}
		}
		
		public string Name {
			get {return file_abstraction.Name;}
		}
		
		public string MimeType {
			get {return mime_type;}
			internal set {mime_type = value;}
		}
		
		public long Tell {
			get {return (Mode == AccessMode.Closed) ?
				0 : file_stream.Position;}
		}
		
		public long Length {
			get {return (Mode == AccessMode.Closed) ?
				0 : file_stream.Length;}
		}
		
		public AccessMode Mode {
			get {return (file_stream == null) ?
				AccessMode.Closed : (file_stream.CanWrite) ?
					AccessMode.Write : AccessMode.Read;}
			set {
				if (Mode == value || (Mode == AccessMode.Write
					&& value == AccessMode.Read))
					return;
				
				if (file_stream != null)
					file_abstraction.CloseStream (file_stream);
				
				file_stream = null;
				
				if (value == AccessMode.Read)
					file_stream = file_abstraction.ReadStream;
				else if (value == AccessMode.Write)
					file_stream = file_abstraction.WriteStream;
				
				Mode = value;
			}
		}
		
		#endregion
		
		
		
		#region Public Methods
		
		public abstract void Save ();
		public abstract void RemoveTags (TagTypes types);
		public abstract Tag GetTag (TagTypes type, bool create);
		
		public Tag GetTag (TagTypes type)
		{
			return GetTag (type, false);
		}
		
		/// <summary>
		///    Reads a specified number of bytes at the current seek
		///    position from the current instance.
		/// </summary>
		/// <param name="length">
		///    A <see cref="int" /> specifying the number of bytes to
		///    read.
		/// </param>
		/// <returns>
		///    A <see cref="ByteVector" /> containing the data.
		/// </returns>
		/// <remarks>
		///    <para>This method reads the block of data at the current
		///    seek position. To change the seek position, use <see
		///    cref="Seek(long,System.IO.SeekOrigin)" />.</para>
		/// </remarks>
		/// <exception cref="ArgumentException">
		///    <paramref name="length" /> is less than zero.
		/// </exception>
		public ByteVector ReadBlock (int length)
		{
			if (length < 0)
				throw new ArgumentException (
					"Length must be non-negative",
					"length");
			
			if (length == 0)
				return new ByteVector ();
			
			Mode = AccessMode.Read;
			
			if (Tell + length > Length)
				length = (int) (Length - Tell);
			
			if (length <= 0)
				return new ByteVector ();
			
			byte [] buffer = new byte [length];
			int count = file_stream.Read (buffer, 0, length);
			return new ByteVector (buffer, count);
		}
		
		public void WriteBlock (ByteVector data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
					Mode = AccessMode.Write;
			
			file_stream.Write (data.Data, 0, data.Count);
		}
		
		public long Find (ByteVector pattern, long startPosition,
		                  ByteVector before)
		{
			if (pattern == null)
				throw new ArgumentNullException ("pattern");
			
			Mode = AccessMode.Read;
			
			if (pattern.Count > buffer_size)
				return -1;
			
			// The position in the file that the current buffer
			// starts at.
			
			long buffer_offset = startPosition;
			ByteVector buffer;
			
			// These variables are used to keep track of a partial
			// match that happens at the end of a buffer.
			
			int previous_partial_match = -1;
			int before_previous_partial_match = -1;
			
			// Save the location of the current read pointer.  We
			// will restore the position using Seek() before all
			// returns.
			
			long original_position = file_stream.Position;
			
			// Start the search at the offset.
			
			file_stream.Position = startPosition;
			
			// This loop is the crux of the find method.  There are
			// three cases that we want to account for:
			//
			// (1) The previously searched buffer contained a
			//     partial match of the search pattern and we want
			//     to see if the next one starts with the remainder
			//     of that pattern.
			//
			// (2) The search pattern is wholly contained within the
			//     current buffer.
			//
			// (3) The current buffer ends with a partial match of
			//     the pattern.  We will note this for use in the 
			//     next iteration, where we will check for the rest 
			//     of the pattern.
			//
			// All three of these are done in two steps.  First we
			// check for the pattern and do things appropriately if
			// a match (or partial match) is found.  We then check 
			// for "before".  The order is important because it 
			// gives priority to "real" matches.
			
			for (buffer = ReadBlock (buffer_size); 
				buffer.Count > 0;
				buffer = ReadBlock(buffer_size)) {
				
				// (1) previous partial match
				
				if (previous_partial_match >= 0 &&
					buffer_size > previous_partial_match) {
					int pattern_offset = buffer_size -
						previous_partial_match;
					
					if (buffer.ContainsAt (pattern, 0,
						pattern_offset)) {
						
						file_stream.Position =
							original_position;
						
						return buffer_offset -
							buffer_size +
							previous_partial_match;
					}
				}
				
				if (before != null &&
					before_previous_partial_match >= 0 &&
					buffer_size >
					before_previous_partial_match) {
					
					int before_offset = buffer_size -
						before_previous_partial_match;
					
					if (buffer.ContainsAt (before, 0,
						before_offset)) {
						
						file_stream.Position =
							original_position;
						
						return -1;
					}
				}
				
				// (2) pattern contained in current buffer
				
				long location = buffer.Find (pattern);
				
				if (location >= 0) {
					file_stream.Position = original_position;
					return buffer_offset + location;
				}
				
				if (before != null && buffer.Find (before) >= 0) {
					file_stream.Position = original_position;
					return -1;
				}
				
				// (3) partial match
				
				previous_partial_match =
					buffer.EndsWithPartialMatch (pattern);
				
				if (before != null)
					before_previous_partial_match =
						buffer.EndsWithPartialMatch (
							before);
				
				buffer_offset += buffer_size;
			}
			
			// Since we hit the end of the file, reset the status
			// before continuing.
			
			file_stream.Position = original_position;
			return -1;
		}
		
		public long Find (ByteVector pattern, long startPosition)
		{
			return Find (pattern, startPosition, null);
		}
		
		public long Find (ByteVector pattern)
		{
			return Find (pattern, 0);
		}
		
		long RFind (ByteVector pattern, long startPosition,
			ByteVector before)
		{
			Mode = AccessMode.Read;
			
			if (pattern.Count > buffer_size)
				return -1;
			
			// The position in the file that the current buffer
			// starts at.
			
			ByteVector buffer;
			
			// These variables are used to keep track of a partial
			// match that happens at the end of a buffer.
			
			/*
			int previous_partial_match = -1;
			int before_previous_partial_match = -1;
			*/
			
			// Save the location of the current read pointer.  We
			// will restore the position using Seek() before all 
			// returns.
			
			long original_position = file_stream.Position;
			
			// Start the search at the offset.
			
			long buffer_offset;
			
			if (startPosition == 0)
				Seek (-1 * buffer_size,
					System.IO.SeekOrigin.End);
			else
				Seek (startPosition + -1 * buffer_size,
					System.IO.SeekOrigin.Begin);
			
			buffer_offset = file_stream.Position;
			
			// See the notes in find() for an explanation of this
			// algorithm.
			
			for (buffer = ReadBlock(buffer_size); buffer.Count > 0;
				buffer = ReadBlock (buffer_size)) {
				
				// TODO: (1) previous partial match
				
				// (2) pattern contained in current buffer
				
				long location = buffer.RFind (pattern);
				if (location >= 0) {
					file_stream.Position = original_position;
					return buffer_offset + location;
				}
				
				if(before != null && buffer.Find (before) >= 0) {
					file_stream.Position = original_position;
					return -1;
				}
				
				// TODO: (3) partial match
				
				buffer_offset -= buffer_size;
				file_stream.Position = buffer_offset;
			}
			
			// Since we hit the end of the file, reset the status
			// before continuing.
			
			file_stream.Position = original_position;
			return -1;
		}
		
		public long RFind (ByteVector pattern, long startPosition)
		{
			return RFind (pattern, startPosition, null);
		}
		
		public long RFind (ByteVector pattern)
		{
			return RFind (pattern, 0);
		}
		
		public void Insert (ByteVector data, long start, long replace)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			Mode = AccessMode.Write;
			
			if (data.Count == replace) {
				file_stream.Position = start;
				WriteBlock (data);
				return;
			} else if(data.Count < replace) {
				file_stream.Position = start;
				WriteBlock (data);
				RemoveBlock (start + data.Count,
					replace - data.Count);
				return;
			}
			
			// Woohoo!  Faster (about 20%) than id3lib at last. I
			// had to get hardcore and avoid TagLib's high level API
			// for rendering just copying parts of the file that
			// don't contain tag data.
			//
			// Now I'll explain the steps in this ugliness:
			
			// First, make sure that we're working with a buffer
			// that is longer than the *differnce* in the tag sizes.
			// We want to avoid overwriting parts that aren't yet in
			// memory, so this is necessary.
			
			int buffer_length = buffer_size;
			
			while (data.Count - replace > buffer_length)
				buffer_length += (int) BufferSize;
			
			// Set where to start the reading and writing.
			
			long read_position = start + replace;
			long write_position = start;
			
			byte [] buffer;
			byte [] about_to_overwrite;
			
			// This is basically a special case of the loop below.  
			// Here we're just doing the same steps as below, but 
			// since we aren't using the same buffer size -- instead
			// we're using the tag size -- this has to be handled as
			// a special case.  We're also using File::writeBlock()
			// just for the tag. That's a bit slower than using char
			// *'s so, we're only doing it here.
			
			file_stream.Position = read_position;
			about_to_overwrite = ReadBlock (buffer_length).Data;
			read_position += buffer_length;
			
			file_stream.Position = write_position;
			WriteBlock (data);
			write_position += data.Count;
			
			buffer = new byte [about_to_overwrite.Length];
			System.Array.Copy (about_to_overwrite, 0, buffer, 0,
				about_to_overwrite.Length);
			
			// Ok, here's the main loop.  We want to loop until the
			// read fails, which means that we hit the end of the 
			// file.
			
			while (buffer_length != 0) {
				// Seek to the current read position and read
				// the data that we're about to overwrite. 
				// Appropriately increment the readPosition.
				
				file_stream.Position = read_position;
				int bytes_read = file_stream.Read (
					about_to_overwrite, 0, buffer_length <
					about_to_overwrite.Length ?
						buffer_length :
						about_to_overwrite.Length);
				read_position += buffer_length;
				
				// Seek to the write position and write our
				// buffer. Increment the writePosition.
				
				file_stream.Position = write_position;
				file_stream.Write (buffer, 0,
					buffer_length < buffer.Length ?
						buffer_length : buffer.Length);
				write_position += buffer_length;
				
				// Make the current buffer the data that we read
				// in the beginning.
				
				System.Array.Copy (about_to_overwrite, 0,
					buffer, 0, bytes_read);
				
				// Again, we need this for the last write.  We
				// don't want to write garbage at the end of our
				// file, so we need to set the buffer size to
				// the amount that we actually read.
				
				buffer_length = bytes_read;
			}
		}
		
		public void Insert (ByteVector data, long start)
		{
			Insert (data, start, 0);
		}
		
		public void RemoveBlock (long start, long length)
		{
			if (length == 0)
				return;
			
			Mode = AccessMode.Write;
			
			int buffer_length = buffer_size;
			
			long read_position = start + length;
			long write_position = start;
			
			ByteVector buffer = (byte) 1;
			
			while(buffer.Count != 0) {
				file_stream.Position = read_position;
				buffer = ReadBlock (buffer_length);
				read_position += buffer.Count;
				
				file_stream.Position = write_position;
				WriteBlock (buffer);
				write_position += buffer.Count;
			}
			
			Truncate (write_position);
		}
		
		public void Seek (long offset, System.IO.SeekOrigin origin)
		{
			if (Mode != AccessMode.Closed)
				file_stream.Seek (offset, origin);
		}
		
		public void Seek (long offset)
		{
			Seek (offset, System.IO.SeekOrigin.Begin);
		}
		
		#endregion
		
		
		
		#region Public Static Methods
		
		public static File Create (string path)
		{
			return Create(path, null, ReadStyle.Average);
		}
		
		public static File Create (IFileAbstraction abstraction)
		{
			return Create(abstraction, null, ReadStyle.Average);
		}
		
		public static File Create (string path,
		                           ReadStyle propertiesStyle)
		{
			return Create(path, null, propertiesStyle);
		}
		
		public static File Create (IFileAbstraction abstraction,
		                           ReadStyle propertiesStyle)
		{
			return Create(abstraction, null, propertiesStyle);
		}
		
		public static File Create (string path, string mimetype,
		                           ReadStyle propertiesStyle)
		{
			return Create (new LocalFileAbstraction (path),
				mimetype, propertiesStyle);
		}
		
		public static File Create(IFileAbstraction abstraction,
		                          string mimetype,
		                          ReadStyle propertiesStyle)
		{
			if(mimetype == null) {
				string ext = String.Empty;
				
				int index = abstraction.Name.LastIndexOf (".") + 1;
				
				if(index >= 1 && index < abstraction.Name.Length)
					ext = abstraction.Name.Substring (index,
						abstraction.Name.Length - index);
				
				mimetype = "taglib/" + ext.ToLower(
					CultureInfo.InvariantCulture);
			}
			
			foreach (FileTypeResolver resolver in file_type_resolvers) {
				File file = resolver(abstraction, mimetype,
					propertiesStyle);
				
				if(file != null)
					return file;
			}
			
			if (!FileTypes.AvailableTypes.ContainsKey(mimetype))
				throw new UnsupportedFormatException (
					String.Format (
						CultureInfo.InvariantCulture,
						"{0} ({1})",
						abstraction.Name,
						mimetype));
			
			Type file_type = FileTypes.AvailableTypes[mimetype];
			
			try {
				File file = (File) Activator.CreateInstance(
					file_type,
					new object [] {abstraction, propertiesStyle});
				
				file.MimeType = mimetype;
				return file;
			} catch (System.Reflection.TargetInvocationException e) {
				throw e.InnerException;
			}
		}
		
		/// <summary>
		///    Adds a <see cref="FileTypeResolver" /> to the <see
		///    cref="File" /> class. The one added last gets run first.
		/// </summary>
		/// <param name="FileTypeResolver">
		///    A <see cref="FileTypeResolver" /> delegate to add to the
		///    file type recognition stack.
		/// </param>
		/// <remarks>
		///    A <see cref="FileTypeResolver" /> adds support for 
		///    recognizing a file type outside of the standard mime-type
		///    methods.
		/// </remarks>
		public static void AddFileTypeResolver (FileTypeResolver resolver)
		{
			if (resolver != null)
				file_type_resolvers.Insert (0, resolver);
		}
		
		#endregion
		
		
		
		#region Protected Methods
		
		/// <summary>
		///    Resized the current instance to a specified number of
		///    bytes.
		/// </summary>
		/// <param name="length">
		///    A <see cref="long" /> value specifying the number of
		///    bytes to resize the file to.
		/// </param>
		protected void Truncate (long length)
		{
			AccessMode old_mode = Mode;
			Mode = AccessMode.Write;
			file_stream.SetLength (length);
			Mode = old_mode;
		}
		
		#endregion
		
		
		
		#region Classes
		
		/// <summary>
		///    This class implements <see cref="IFileAbstraction" />
		///    to provide support for accessing the local/standard file
		///    system.
		/// </summary>
		/// <remarks>
		///    This class is used as the standard file abstraction
		///    throughout the library.
		/// </remarks>
		public class LocalFileAbstraction : IFileAbstraction
		{
			/// <summary>
			///    Contains the name used to open the file.
			/// </summary>
			private string name;
			
			/// <summary>
			///    Constructs and initializes a new instance of
			///    <see cref="LocalFileAbstraction" /> for a
			///    specified path in the local file system.
			/// </summary>
			/// <param name="path">
			///    A <see cref="string" /> object containing the
			///    path of the file to use in the new instance.
			/// </param>
			/// <exception cref="ArgumentNullException">
			///    <paramref name="path" /> is <see langref="null"
			///    />.
			/// </exception>
			public LocalFileAbstraction (string path)
			{
				if (path == null)
					throw new ArgumentNullException ("path");
				
				name = path;
			}
			
			/// <summary>
			///    Gets the path of the file represented by the
			///    current instance.
			/// </summary>
			/// <value>
			///    A <see cref="string" /> object containing the
			///    path of the file represented by the current
			///    instance.
			/// </value>
			public string Name {
				get {return name;}
			}
			
			/// <summary>
			///    Gets a new readable, seekable stream from the
			///    file represented by the current instance.
			/// </summary>
			/// <value>
			///    A new <see cref="System.IO.Stream" /> to be used
			///    when reading the file represented by the current
			///    instance.
			/// </value>
			public System.IO.Stream ReadStream {
				get {return System.IO.File.Open (Name,
					System.IO.FileMode.Open,
					System.IO.FileAccess.Read,
					System.IO.FileShare.Read);}
			}
			
			/// <summary>
			///    Gets a new writable, seekable stream from the
			///    file represented by the current instance.
			/// </summary>
			/// <value>
			///    A new <see cref="System.IO.Stream" /> to be used
			///    when writing to the file represented by the
			///    current instance.
			/// </value>
			public System.IO.Stream WriteStream {
				get {return System.IO.File.Open (Name,
					System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite);}
			}
			
			/// <summary>
			///    Closes a stream created by the current instance.
			/// </summary>
			/// <param name="stream">
			///    A <see cref="System.IO.Stream" /> object
			///    created by the current instance.
			/// </param>
			public void CloseStream (System.IO.Stream stream)
			{
				if (stream == null)
					throw new ArgumentNullException ("stream");
				
				stream.Close ();
			}
		}
		
		#endregion
		
		
		
		#region Interfaces
		
		/// <summary>
		///    This interface provides abstracted access to a file. It
		//     premits access to non-standard file systems and data
		///    retrieval methods.
		/// </summary>
		/// <remarks>
		///    <para>To use a custom abstraction, use <see
		///    cref="Create(IFileAbstraction)" /> instead of <see
		///    cref="Create(string)" /> when creating files.</para>
		/// </remarks>
		/// <example>
		///    <para>The following example uses Gnome VFS to open a file
		///    and read its title.</para>
		/// <code lang="C#">using TagLib;
		///using Gnome.Vfs;
		///
		///public class ReadTitle
		///{
		///   public static void Main (string [] args)
		///   {
		///      if (args.Length != 1)
		///         return;
		///
		///      Gnome.Vfs.Vfs.Initialize ();
		///      
		///      try {
		///          TagLib.File file = TagLib.File.Create (
		///             new VfsFileAbstraction (args [0]));
		///          System.Console.WriteLine (file.Tag.Title);
		///      } finally {
		///         Vfs.Shutdown()
		///      }
		///   }
		///}
		///
		///public class VfsFileAbstraction : TagLib.File.IFileAbstraction
		///{
		///    private string name;
		///
		///    public VfsFileAbstraction (string file)
		///    {
		///        name = file;
		///    }
		///
		///    public string Name {
		///        get { return name; }
		///    }
		///
		///    public System.IO.Stream ReadStream {
		///        get { return new VfsStream(Name, System.IO.FileMode.Open); }
		///    }
		///
		///    public System.IO.Stream WriteStream {
		///        get { return new VfsStream(Name, System.IO.FileMode.Open); }
		///    }
		///
		///    public void CloseStream (System.IO.Stream stream)
		///    {
		///        stream.Close ();
		///    }
		///}</code>
		///    <code lang="Boo">import TagLib from "taglib-sharp.dll"
		///import Gnome.Vfs from "gnome-vfs-sharp"
		///
		///class VfsFileAbstraction (TagLib.File.IFileAbstraction):
		///        
		///        _name as string
		///        
		///        def constructor(file as string):
		///                _name = file
		///        
		///        Name:
		///                get:
		///                        return _name
		///                
		///        ReadStream:
		///                get:
		///                        return VfsStream(_name, FileMode.Open)
		///                
		///        WriteStream:
		///                get:
		///                        return VfsStream(_name, FileMode.Open)
		///        
		///if len(argv) == 1:
		///        Vfs.Initialize()
		///
		///        try:
		///                file as TagLib.File = TagLib.File.Create (VfsFileAbstraction (argv[0]))
		///                print file.Tag.Title
		///        ensure:
		///                Vfs.Shutdown()</code>
		/// </example>
		public interface IFileAbstraction
		{
			/// <summary>
			///    Gets the name or identifier used by the
			///    implementation.
			/// </summary>
			/// <value>
			///    A <see cref="string" /> object containing the 
			///    name or identifier used by the implementation.
			/// </value>
			/// <remarks>
			///    This value would typically represent a path or
			///    URL to be used when identifying the file in the
			///    file system, but it could be any value
			///    as appropriate for the implementation.
			/// </remarks>
			string Name {get;}
			
			/// <summary>
			///    Gets a readable, seekable stream for the file
			///    referenced by the current instance.
			/// </summary>
			/// <value>
			///    A <see cref="System.IO.Stream" /> object to be
			///    used when reading a file.
			/// </value>
			/// <remarks>
			///    This property is typically used when creating
			///    constructing an instance of <see cref="File" />.
			///    Upon completion of the constructor, <see
			///    cref="CloseStream" /> will be called to close
			///    the stream. If the stream is to be reused after
			///    this point, <see cref="CloseStream" /> should be
			///    implemented in a way to keep it open.
			/// </remarks>
			System.IO.Stream ReadStream  {get;}
			
			/// <summary>
			///    Gets a writable, seekable stream for the file
			///    referenced by the current instance.
			/// </summary>
			/// <value>
			///    A <see cref="System.IO.Stream" /> object to be
			///    used when writing to a file.
			/// </value>
			/// <remarks>
			///    This property is typically used when saving a
			///    file with <see cref="Save" />. Upon completion of
			///    the method, <see cref="CloseStream" /> will be
			///    called to close the stream. If the stream is to
			///    be reused after this point, <see
			///    cref="CloseStream" /> should be implemented in a
			///    way to keep it open.
			/// </remarks>
			System.IO.Stream WriteStream {get;}
			
			/// <summary>
			///    Closes a stream originating from the current
			///    instance.
			/// </summary>
			/// <param name="stream">
			///    A <see cref="System.IO.Stream" /> object
			///    originating from the current instance.
			/// </param>
			/// <remarks>
			///    If the stream is to be used outside of the scope,
			///    of TagLib#, this method should perform no action.
			///    For example, a stream that was created outside of
			///    the current instance, or a stream that will
			///    subsequently be used to play the file.
			/// </remarks>
			void CloseStream (System.IO.Stream stream);
		}
		
		#endregion
	}
}
