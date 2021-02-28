
using System;
using UnityEngine;
using System.IO;

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
namespace OntSenseCSharpAPI
{
	/// The subclass of RobotPerception in which the sensing is done by a vision  sensor. 
	/// A Thing or CognitiveThing object are only inserted in the triple store by this class.
	public class RobotVision : RobotPerceptionEvent
	{
        private string sPosition;       // Sparql command for generate the position information
        private string sColor;          // Sparql command for generate the color information
        private string sAnything;       // Sparql command for generate the thing information      
        private string sVision;         // Sparql command for generate the vision information



        /// Constructor of the RobotVision class. The objective is to create a instance of an inanimate object view.
        /// 
        /// 
        public RobotVision(DateTime instant, Thing obj)
		{
            long countEv = getEventCount();          // get a unique identifier for position and color

            // to create a Sparql command for generate the position information
            //
            sPosition = string.Format(SparqlAccess.INSERT_POSITION_OBJ, countEv, obj.pos.cartesianX, obj.pos.cartesianY, obj.pos.cartesianZ);

            // to create a Sparql command for generate the color information
            sColor = string.Format(SparqlAccess.INSERT_COLOR, countEv, obj.color.red, obj.color.blue, obj.color.green);

            // to create a Sparql command for generate the thing information
            sAnything = string.Format(SparqlAccess.INSERT_THING, countEv, obj.objectId, obj.material, obj.state, obj.tagInfo, obj.uriId, obj.name);

            // to create a Sparql command for generate the vision information
            sVision = string.Format(SparqlAccess.INSERT_VISION, countEv, instant.ToString(SparqlAccess.XSD_DATETIME), obj.objectId);



        }

        /// Constructor of the RobotVision class. The objective is to create a instance of an animate agents with emotion.
        public RobotVision(DateTime instant, Human agent)
        {
            long countEv = getEventCount();          // get a unique identifier for position and color

            // to create a Sparql command for generate the position information
            //
            sPosition = string.Format(SparqlAccess.INSERT_POSITION_OBJ, countEv, agent.pos.cartesianX, agent.pos.cartesianY, agent.pos.cartesianZ);

            // to create a Sparql command for generate the color information
            sColor = string.Format(SparqlAccess.INSERT_COLOR, countEv, agent.color.red, agent.color.blue, agent.color.green);

            // to create a Sparql command for generate the Human information
            sAnything = string.Format(SparqlAccess.INSERT_HUMAN, countEv, agent.objectId, agent.material, agent.state, agent.tagInfo, agent.uriId, agent.emotion, agent.name);

            // to create a Sparql command for generate the vision information
            //
            sVision = string.Format(SparqlAccess.INSERT_VISION, countEv, instant.ToString(SparqlAccess.XSD_DATETIME), agent.objectId);


        }


        /// Constructor of the RobotVision class. The information about the robot agent is supplied as a vision perception. 
        /// This is a trick used to upgrade the triple store with robot status information.  In a philosophical view,
        /// the robot is expressing the knowledge of itself.
        public RobotVision(DateTime instant, Robot rob)
        {
            long countEv = getEventCount();          // get a unique identifier for position and color

            // to create a Sparql command for generate the position information
            //
            sPosition = string.Format(SparqlAccess.INSERT_POSITION_OBJ, countEv, rob.pos.cartesianX, rob.pos.cartesianY, rob.pos.cartesianZ);

            // to create a Sparql command for generate the color information
            sColor = string.Format(SparqlAccess.INSERT_COLOR, countEv, rob.color.red, rob.color.blue, rob.color.green);

            // to create a Sparql command for generate the Robot information
            sAnything = string.Format(SparqlAccess.INSERT_ROBOT, countEv, rob.objectId, rob.material, rob.state, rob.tagInfo, rob.uriId, rob.name);

            // to create a Sparql command for generate the vision information
            sVision = string.Format(SparqlAccess.INSERT_VISION, countEv, instant.ToString(SparqlAccess.XSD_DATETIME), rob.objectId);


        }

        /// insert the objects captured by the vision sensor. 
        public override void insert()
		{

            SparqlEndPoint instanceSparql = SparqlEndPoint.getInstance();       // gets the instance for the  singleton object

            // updates all information associated with a vison event
            instanceSparql.executeSparqlUpdate(sPosition);
            instanceSparql.executeSparqlUpdate(sColor);
            instanceSparql.executeSparqlUpdate(sAnything);
            instanceSparql.executeSparqlUpdate(sVision);

        }

	}

}

