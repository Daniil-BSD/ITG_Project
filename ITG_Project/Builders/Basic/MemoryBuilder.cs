using System;
using System.Collections.Generic;
using System.Text;

namespace ITG_Core {
	public class MemoryBuilder<T> : LayerBuilder<T, T> where T : struct {
		public override Algorithm<T> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			base.Build(itermidiate);
			return new Memory<T>(itermidiate.Get<T>(SourceID));
		}
	}
}
