///
/// This file is part of OntCog project.
/// 
/// OntCog is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
/// 
/// OntCog is distributed in the hope that it will be useful,  but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
/// 
/// 
/// You should have received a copy of the GNU General Public License  along with Foobar. 
/// If not, see<http://www.gnu.org/licenses/>.
/// 
///
/// 

using System;


namespace OntSenseCSharpAPI
{
	/// Represents the robot's perception about the environment.
	public abstract class RobotPerceptionEvent
	{
		/// Defines the  instant of event ocurrence. This instant is assumed to be the time of object  instantiation.  
		protected DateTime occurs;

		/// this attribute counts the occurence of events. 000000000 has a special meaning for the API, so its not used as a identifier.
		private static long eventCount = 0;

		/// Defines the object  responsable by the event generation. Note that, this knowledge is not always present. As an example, when an odor is present but the source is unknown.
		protected Thing generateBy;

		/// abstract function that is responsibile in the subclasses for the information update.  
		public abstract void insert();

		/// return a unique identification for the event. 
		public long getEventCount()
		{
            		if (++ eventCount == 9999999999)		// reach max positive value with 10 digits
				eventCount = 1;				// roll back to beginning
			return eventCount;
		}

	}

}

