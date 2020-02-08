﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ITG_Core {
	public class Vec2FieldBuilder : LayerBuilder<Vec2, uint> {

		public float Magnitude { get; set; }

		public override Algorithm<Vec2> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			base.Build(itermidiate); // validity check and exception throwing
			return new Vec2Field(itermidiate.Get<uint>(SourceID), Magnitude);
		}
	}

}
