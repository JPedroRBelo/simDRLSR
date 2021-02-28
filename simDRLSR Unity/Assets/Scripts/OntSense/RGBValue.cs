namespace OntSenseCSharpAPI
{
	/// Represents the main color associated with a given object.
	public class RGBValue
	{
		public double blue { get; set; }

        public double green { get; set; }

        public double red { get; set; }

		public RGBValue(double redLevel, double greenLevel, double blueLevel)
		{
            red = redLevel;
            green = greenLevel;
            blue = blueLevel;
        }

        

    }

}

