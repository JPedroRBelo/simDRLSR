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
	/// The subclass of RobotPerception in which the sensing is done by an haptic technology. 
	/// The unique ID of known object or the position of an unknown object is supplied with the tactil information. Note that, the unique ID is only known if the object is in one of the hands of the robot.  In this case, the knowledge is obtained because, for the robot to catch the object, it must have seen this object first .
	public class RobotTouch : RobotPerceptionEvent
	{
        private string sTouch;                   // Sparql command for generate the touch information
        private string sPosition = null;        // Sparql command for generate the position information

		/// Constructor of the RobotTouch class. The objective is to create a instance for a touch perception. 
		/// The instant parameter represens the exact moment of the touch capture. 
		/// The idObject parameter represents an unique identifier associated with the object responsible to produce  the touch perception. 
		/// The hardness, moisture, pressure, roughness and temperature represent specific  caractheristics of the touch perception.
		/// 
///  
		public RobotTouch(DateTime instant, long idObject, double hard, double mois, double press, double rough, double temp)
		{
            long countEv = getEventCount();          // get a unique identifier for position and color

            // to create a Sparql command for generate the touch information
            sTouch = string.Format(SparqlAccess.INSERT_TOUCH, countEv, instant.ToString(SparqlAccess.XSD_DATETIME), idObject, temp, hard, mois, rough, press);


        }



		/// Constructor of the RobotTouch class. The objective is to create a instance for a touch perception.
		/// The instant parameter represens the exact moment of the touch capture. 
		/// The pos parameter represents the position of an unknow object responsible to produce the touch perception.
		/// The hardness, moisture, pressure, roughness and temperature represent specific  caractheristics of the touch perception.  
		public RobotTouch(DateTime instant, CartesianPos pos, double hard, double mois, double press, double rough, double temp)
		{
            long countEv = getEventCount();          // get a unique identifier for position and color

            // to create a Sparql command for generate the position information
            //
            sPosition = string.Format(SparqlAccess.INSERT_POSITION_LOCAL, countEv, pos.cartesianX, pos.cartesianY, pos.cartesianZ);



            // to create a Sparql command for generate the touch information
            sTouch = string.Format(SparqlAccess.INSERT_TOUCH_POS, countEv, instant.ToString(SparqlAccess.XSD_DATETIME), temp, hard, mois, rough, press);


        }

        /// insert the touch information captured by the touch sensor. 
        public override void insert()
        {
            SparqlEndPoint instanceSparql = SparqlEndPoint.getInstance();       // gets the instance for the  singleton object

            // updates all information associated with the event
            if (!String.IsNullOrEmpty(sPosition))                               // if a position was defined then updated it
            {                         
                instanceSparql.executeSparqlUpdate(sPosition);
                sPosition = null;                                               // just in case...
            }
            instanceSparql.executeSparqlUpdate(sTouch);
        }
	}

}

