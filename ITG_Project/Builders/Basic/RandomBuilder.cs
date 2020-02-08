using System;
using System.Collections.Generic;
using System.Text;

namespace ITG_Core {

	public class RandomBuilder : AlgorithmBuilder<uint> {

		public int Seed { get; set; }

		public override Algorithm<uint> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			base.Build(itermidiate);
			return new Random(Seed);
		}
	}

}
