using System;


namespace OntSenseCSharpAPI
{
	/// The subclass of RobotPerception in which the sensing is done by an auditory sensor. 
	/// The unique ID of known object or the position of an unknown object is supplied with the sound information. Note that, the unique ID is only known if the object is in the robot view.
	public class RobotHear : RobotPerceptionEvent
	{

        private string sHear;                   // Sparql command for generate the hear information
        private string sPosition = null;        // Sparql command for generate the position information


        /// Constructor of the RobotHear class. The objective is to create a instance for a sound perception. 
        /// he instant parameter represens the exact moment of the sound capture.
        /// The idObject parameter represents an unique identifier associated the object responsible to produce the sound. 
        /// The kind defines the kind of the sound. 
        /// The volume parameter defines the sound volume.
        /// The detail parameter defines additional information associated with the sound, for example, if kind is MARIANA_VOICE then the detail represents the sentence said.
        /// 
        ///  
        ///  
        public RobotHear( DateTime instant, long idObject, HearingAttribute kind, double volume, string detail)
		{

            long countEv = getEventCount();          // get a unique identifier for position and color

            // to create a Sparql command for generate the hear information
            sHear = string.Format(SparqlAccess.INSERT_HEAR, countEv, instant.ToString(SparqlAccess.XSD_DATETIME), idObject, volume, kind, detail);


        }






		/// Constructor of the RobotHear class. The objective is to create a instance for a sound perception.
		/// The instant parameter represens the exact moment of the sound capture. 
		/// The pos parameter represents the position of an unknow object responsible to produce the sound. 
		/// The kind defines the kind of the sound. 
		/// The volume parameter defines the sound volume. 
		/// The detail parameter defines additional information associated with the sound, for example, if kind is MARIANA_VOICE then the detail represents the sentence said.
		public RobotHear(DateTime instant, CartesianPos pos, HearingAttribute kind, double volume, string detail)
		{
            long countEv = getEventCount();          // get a unique identifier for position and color

            // to create a Sparql command for generate the position information
            //
            sPosition = string.Format(SparqlAccess.INSERT_POSITION_LOCAL, countEv, pos.cartesianX, pos.cartesianY, pos.cartesianZ);



            // to create a Sparql command for generate the hear information
            sHear = string.Format(SparqlAccess.INSERT_HEAR_POS, countEv, instant.ToString(SparqlAccess.XSD_DATETIME), volume, kind, detail);



        }


 


        /// insert the sound captured by the hear sensor. 
		public override void insert()
        {

            SparqlEndPoint instanceSparql = SparqlEndPoint.getInstance();       // gets the instance for the  singleton object

            // updates all information associated with the event
            if (!String.IsNullOrEmpty(sPosition))
            {                         
                instanceSparql.executeSparqlUpdate(sPosition);                  // if a position was defined then updated it
                sPosition = null;                                               // just in case...
            }
            instanceSparql.executeSparqlUpdate(sHear);
        }

    }

}

