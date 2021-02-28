
using System;

namespace OntSenseCSharpAPI
{
	/// A CartesianPos denotes the quantitative position of an object in a cartesian coordinate system.
	public class CartesianPos
	{
		public double cartesianX { get; set; }

        public double cartesianY { get; set; }

        public double cartesianZ { get; set; }

        public CartesianPos(double pX, double pY, double pZ)
		{
            cartesianX = pX;
            cartesianY = pY;
            cartesianZ = pZ;
        }


        
    }

}

