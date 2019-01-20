using System;

namespace TagLib.Id3v2
{
	public class EventTimeCode : ICloneable
	{
		#region Public Properties

		public EventType TypeOfEvent { get; set; }

		public int Time { get; set; }

		#endregion

		#region Public Constructors

		public EventTimeCode (EventType typeOfEvent, int time)
		{
			TypeOfEvent = typeOfEvent;
			Time = time;
		}

		#endregion

		#region Static Methods

		public static EventTimeCode CreateEmpty ()
		{
			return new EventTimeCode (EventType.Padding, 0);
		}

		#endregion

		#region ICloneable

		public object Clone ()
		{
			return new EventTimeCode (TypeOfEvent, Time);
		}

		#endregion
	}
}