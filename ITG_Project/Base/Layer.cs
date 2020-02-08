using System;
using System.Collections.Generic;
using System.Text;

namespace ITG_Core {

	public abstract class Layer<T, S> : Algorithm<T> where T : struct where S : struct {
		protected readonly Algorithm<S> source;

		public Layer(Algorithm<S> algorithm) : base()
		{
			this.source = algorithm;
		}
	}
}
