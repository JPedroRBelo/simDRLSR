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
/// using ontsenseAPI;

namespace OntSenseCSharpAPI
{
	/// A CognitiveAgent is an Agent that has the ability to reason, deliberate, make plans, and experience emotions. Although Human is a subclass of CognitiveAgent, there may be instances of CognitiveAgent which are not also instances of Human. For example, Primates, dolphins, whales, and some extraterrestrials (if they exist) might be considered CognitiveAgents (SUMO definition).
	/// 
	public class Human : Thing
	{
        public EmotionalState emotion { get; set; }


        /// Constructor for the Thing class. The objective is to create a instance of an animate agent. 
        /// All the parameter defines this agent in a similar way of the super class Thing. 
        /// There are just an additional parameter, named emotion, that defines the emotion state of the agent.
        /// 

        public Human(long objId, string name, string tag, RGBValue color, CartesianPos pos, PhysicalState state, Material mat, string uri, EmotionalState emotion): base(objId, name, tag, color, pos, state, mat, uri)

        {
            this.emotion = emotion;

        }

	}

}

