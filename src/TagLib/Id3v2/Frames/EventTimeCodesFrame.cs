using System.Collections.Generic;

namespace TagLib.Id3v2
{
	public class EventTimeCodesFrame : Frame
	{
		#region Private Properties

		private TimestampFormat timestampFormat;

		private List<EventTimeCode> events;

		#endregion

		#region Constructors

		public EventTimeCodesFrame() : base (FrameType.ETCO, 4)
		{
			Flags = FrameFlags.FileAlterPreservation;
		}

		public EventTimeCodesFrame(TimestampFormat timestampFormat) : base(FrameType.ETCO, 4)
		{
			this.timestampFormat = timestampFormat;
			Flags = FrameFlags.FileAlterPreservation;
		}

		public EventTimeCodesFrame(ByteVector data, 
			byte version) : base (data, version)
		{
			SetData(data, 0, version, true);
		}

		public EventTimeCodesFrame(FrameHeader frameHeader) : base (frameHeader)
		{

		}

		public EventTimeCodesFrame(ByteVector data,
								int offset,
								FrameHeader header,
								byte version) : base (header)
		{
			SetData(data, offset, version, false);
		}

		#endregion

		#region Public Properties

		public TimestampFormat TimestampFormat
		{
			get { return timestampFormat; }
			set { timestampFormat = value; }
		}

		public List<EventTimeCode> Events
		{
			get { return events; }
			set { events = value; }
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		///    Gets a play count frame from a specified tag, optionally
		///    creating it if it does not exist.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="Tag" /> object to search in.
		/// </param>
		/// <param name="create">
		///    A <see cref="bool" /> specifying whether or not to create
		///    and add a new frame to the tag if a match is not found.
		/// </param>
		/// <returns>
		///    A <see cref="EventTimeCodesFrame" /> object containing the
		///    matching frame, or <see langword="null" /> if a match
		///    wasn't found and <paramref name="create" /> is <see
		///    langword="false" />.
		/// </returns>
		public static EventTimeCodesFrame Get(Tag tag, bool create)
		{
			EventTimeCodesFrame etco;
			foreach (Frame frame in tag)
			{
				etco = frame as EventTimeCodesFrame;

				if (etco != null)
					return etco;
			}

			if (!create)
				return null;

			etco = new EventTimeCodesFrame();
			tag.AddFrame(etco);
			return etco;
		}

		#endregion

		#region Protected Methods

		protected override void ParseFields(ByteVector data, byte version)
		{
			events = new List<EventTimeCode>();
			timestampFormat = (TimestampFormat)data.Data[0];

			var incomingEventsData = data.Mid(1);
			for (var i = 0; i < incomingEventsData.Count - 1; i++)
			{
				var eventType = (EventType)incomingEventsData.Data[i];
				i++;

				var timestampData = new ByteVector(incomingEventsData.Data[i], 
					incomingEventsData.Data[i+1],
					incomingEventsData.Data[i+2],
					incomingEventsData.Data[i+3]);

				i += 3;

				var timestamp = timestampData.ToInt();

				events.Add(new EventTimeCode(eventType, timestamp));
			}
		}

		protected override ByteVector RenderFields(byte version)
		{
			var data = new List<byte>();
			data.Add((byte)timestampFormat);

			foreach (var @event in events)
			{
				data.Add((byte)@event.TypeOfEvent);

				var timeData = ByteVector.FromInt(@event.Time);
				data.AddRange(timeData.Data);
			}

			return new ByteVector(data.ToArray());
		}

		#endregion

		#region ICloneable

		/// <summary>
		///    Creates a deep copy of the current instance.
		/// </summary>
		/// <returns>
		///    A new <see cref="Frame" /> object identical to the
		///    current instance.
		/// </returns>
		public override Frame Clone()
		{
			var frame = new EventTimeCodesFrame(header);
			frame.timestampFormat = timestampFormat;
			frame.events = events.ConvertAll(item => (EventTimeCode)item.Clone());
			return frame;
		}

		#endregion
	}
}
