using System;


namespace OntSenseCSharpAPI
{
	/// The subclass of RobotPerception in which the sensing is done by an olfactory sensor. 
	/// The unique ID of known object or the position of an unknown object is supplied with the odor information. Note that, the unique ID is only known if the object is in one of the hands of the robot. In this case,  the robot knows what is the object because in order for the robot to pick it up and take it up to the electronic nose, it must have seen the object first .
	public class RobotSmell : RobotPerceptionEvent
	{
        private string sSmell;                   // Sparql command for generate the smell information
        private string sPosition = null;        // Sparql command for generate the position information



		/// Constructor of the RobotSmell class. The objective is to create a instance for a odor perception. 
		/// The instant parameter represens the exact moment of the odor capture. 
		/// The idObject parameter represents an unique identifier associated the object responsible to produce the the smell. 
		/// The odor parameter identifies the odor.
		/// 
///  
		///  
		public RobotSmell(DateTime instant, long idObject, OlfactoryAttribute odor)
		{
            long countEv = getEventCount();          // get a unique identifier for position and color

            // to create a Sparql command for generate the olfatory information
            sSmell = string.Format(SparqlAccess.INSERT_SMELL, countEv, instant.ToString(SparqlAccess.XSD_DATETIME), idObject, odor);

        }

		/// Constructor of the RobotSmell class. The objective is to create a instance for a odor perception. 
		/// The instant parameter represens the exact moment of the odor capture.
		///  The pos parameter represents the position of an unknow object responsible to produce the the smell. 
		/// The odor parameter identifies the odor.
		public RobotSmell(DateTime instant, CartesianPos pos, OlfactoryAttribute odor)
		{
            long countEv = getEventCount();          // get a unique identifier for position and color

            // to create a Sparql command for generate the position information
            //
            sPosition = string.Format(SparqlAccess.INSERT_POSITION_LOCAL, countEv, pos.cartesianX, pos.cartesianY, pos.cartesianZ);



            // to create a Sparql command for generate the olfatory information
            sSmell = string.Format(SparqlAccess.INSERT_SMELL_POS, countEv, instant.ToString(SparqlAccess.XSD_DATETIME), odor);


        }

		/// insert the olfatory attribute captured by the Smell sensor. 
		public override void insert()
		{
            SparqlEndPoint instanceSparql = SparqlEndPoint.getInstance();       // gets the instance for the  singleton object

            // updates all information associated with the event
            if (!String.IsNullOrEmpty(sPosition))
            {                                                               
                instanceSparql.executeSparqlUpdate(sPosition);                  // if a position was defined then updated it
                sPosition = null;                                               // just in case...
            }
            instanceSparql.executeSparqlUpdate(sSmell);
        }

	}

}

