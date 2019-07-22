using System;

namespace TagLib.Id3v2
{
	/// <summary>
	/// 
	/// </summary>
	public class EventTimeCode : ICloneable
	{
		#region Public Properties

		/// <summary>
		/// 
		/// </summary>
		public EventType TypeOfEvent { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int Time { get; set; }

		#endregion

		#region Public Constructors

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeOfEvent"></param>
		/// <param name="time"></param>
		public EventTimeCode (EventType typeOfEvent, int time)
		{
			TypeOfEvent = typeOfEvent;
			Time = time;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static EventTimeCode CreateEmpty ()
		{
			return new EventTimeCode (EventType.Padding, 0);
		}

		#endregion

		#region ICloneable

		/// <summary>
		/// 
		/// </summary>
		/// <returns><see cref="EventTimeCode" /></returns>
		public object Clone ()
		{
			return new EventTimeCode (TypeOfEvent, Time);
		}

		#endregion
	}
}