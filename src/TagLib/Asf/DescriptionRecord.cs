using System;

namespace TagLib.Asf {
	public class DescriptionRecord
	{
		private DataType type = DataType.Unicode;
		private ushort lang_list_index = 0;
		private ushort stream_number = 0;
		private string name = null;
		private string strValue = null;
		private ByteVector byteValue = null;
		private ulong longValue = 0;
		private System.Guid guidValue = System.Guid.Empty;
		
		public DescriptionRecord (ushort languageListIndex,
		                          ushort streamNumber, string name,
		                          string value)
		{
			this.lang_list_index = languageListIndex;
			this.stream_number = streamNumber;
			this.name = name;
			this.type = DataType.Unicode;
			this.strValue = value;
		}

		public DescriptionRecord (ushort languageListIndex,
		                          ushort streamNumber, string name,
		                          ByteVector value)
		{
			this.lang_list_index = languageListIndex;
			this.stream_number = streamNumber;
			this.name = name;
			this.type = DataType.Bytes;
			this.byteValue = new ByteVector (value);
		}
		
		public DescriptionRecord (ushort languageListIndex,
		                          ushort streamNumber, string name,
		                          uint value)
		{
			this.lang_list_index = languageListIndex;
			this.stream_number = streamNumber;
			this.name = name;
			this.type = DataType.DWord;
			this.longValue = value;
		}
		
		public DescriptionRecord (ushort languageListIndex,
		                          ushort streamNumber, string name,
		                          ulong value)
		{
			this.lang_list_index = languageListIndex;
			this.stream_number = streamNumber;
			this.name = name;
			this.type = DataType.QWord;
			this.longValue = value;
		}
		
		public DescriptionRecord (ushort languageListIndex,
		                          ushort streamNumber, string name,
		                          ushort value)
		{
			this.lang_list_index = languageListIndex;
			this.stream_number = streamNumber;
			this.name = name;
			this.type = DataType.Word;
			this.longValue = value;
		}
		
		public DescriptionRecord (ushort languageListIndex,
		                          ushort streamNumber, string name,
		                          bool value)
		{
			this.lang_list_index = languageListIndex;
			this.stream_number = streamNumber;
			this.name = name;
			this.type = DataType.Bool;
			this.longValue = value ? 1uL : 0;
		}
		
		public DescriptionRecord (ushort languageListIndex,
		                          ushort streamNumber, string name,
		                          System.Guid value)
		{
			this.lang_list_index = languageListIndex;
			this.stream_number = streamNumber;
			this.name = name;
			this.type = DataType.Guid;
			this.guidValue = value;
		}
		
		public ushort LanguageListIndex {
			get {return lang_list_index;}
		}
		
		public ushort StreamNumber {
			get {return stream_number;}
		}
		
		public string Name {
			get {return name;}
		}
		
		public DataType Type {
			get {return type;}
		}
		
		public override string ToString ()
		{
			if (type == DataType.Unicode)
				return strValue;
			
			if (type == DataType.Bytes)
				return byteValue.ToString ();
			
			return longValue.ToString ();
		}
		
		public ByteVector ToByteVector ()
		{
			return byteValue;
		}
		
		public bool ToBool ()
		{
			return longValue != 0;
		}
		
		public uint ToDWord ()
		{
			uint value;
			if (type == DataType.Unicode && strValue != null &&
				uint.TryParse (strValue, out value))
				return value;
			
			return (uint) longValue;
		}
		
		public ulong ToQWord ()
		{
			ulong value;
			if (type == DataType.Unicode && strValue != null &&
				ulong.TryParse (strValue, out value))
				return value;
			
			return longValue;
		}
		
		public ushort ToWord ()
		{
			ushort value;
			if (type == DataType.Unicode && strValue != null &&
				ushort.TryParse (strValue, out value))
				return value;
			
			return (ushort) longValue;
		}
		
		public System.Guid ToGuid ()
		{
			return guidValue;
		}
		
		public ByteVector Render ()
		{
			ByteVector name = Object.RenderUnicode (this.name);
			ByteVector value = null;
			
			switch (type)
			{
			case DataType.Unicode:
				value = Object.RenderUnicode (strValue);
				break;
			case DataType.Bytes:
				value = byteValue;
				break;
			case DataType.Bool:
			case DataType.DWord:
				value = Object.RenderDWord ((uint) longValue);
				break;
			case DataType.QWord:
				value = Object.RenderQWord (longValue);
				break;
			case DataType.Word:
				value = Object.RenderWord ((ushort) longValue);
				break;
			case DataType.Guid:
				value = guidValue.ToByteArray ();
				break;
			default:
				return null;
			}
			
			ByteVector output = new ByteVector ();
			output.Add (Object.RenderWord (lang_list_index));
			output.Add (Object.RenderWord (stream_number));
			output.Add (Object.RenderWord ((ushort) name.Count));
			output.Add (Object.RenderWord ((ushort) type));
			output.Add (Object.RenderDWord ((uint) value.Count));
			output.Add (name);
			output.Add (value);
			
			return output;
		}
		
		protected internal DescriptionRecord (Asf.File file)
		{
			if (!Parse (file))
				throw new CorruptFileException (
					"Failed to parse description record.");
		}
		
		protected bool Parse (Asf.File file)
		{
			// Field name          Field type Size (bits)
			// Language List Index WORD       16
			// Stream Number       WORD       16
			// Name Length         WORD       16
			// Data Type           WORD       16
			// Data Length         DWORD      32
			// Name                WCHAR      varies
			// Data                See below  varies
			
			if (file == null)
				throw new ArgumentNullException ("file");
			
			lang_list_index = file.ReadWord ();
			stream_number = file.ReadWord ();
			ushort name_length = file.ReadWord ();
			type = (DataType) file.ReadWord ();
			int data_length = (int) file.ReadDWord ();
			name = file.ReadUnicode (name_length);
			
			switch (type)
			{
			case DataType.Word:
				longValue = file.ReadWord ();
				break;
			case DataType.Bool:
			case DataType.DWord:
				longValue = file.ReadDWord ();
				break;
			case DataType.QWord:
				longValue = file.ReadQWord ();
				break;
			case DataType.Unicode:
				strValue = file.ReadUnicode (data_length);
				break;
			case DataType.Bytes:
				byteValue = file.ReadBlock (data_length);
				break;
			case DataType.Guid:
				guidValue = file.ReadGuid ();
				break;
			default:
				return false;
			}
			
			return true;
		}
	}
}
