using ITG_Core.Base;

namespace ITG_Core.Basic {

	public class MemoryStrictSectoring<T> : Layer<T, T> where T : struct {

		public MemoryStrictSectoring(Coordinate offset, ITGThreadPool threadPool, Algorithm<T> algorithm) : base(offset, threadPool, algorithm)
		{
		}
	}
}