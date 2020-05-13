using ITG_Core.Base;
using ITG_Core.Bulders;

namespace ITG_Core.Basic.Builders {

	public class MemoryStrictSectoringBuilder<T> : LayerBuilder<T, T> where T : struct {

		public int SectorSize { get; set; } = Constants.DEFAULT_SECTOR_SIZE;

		public override Algorithm<T> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new MemoryStrictSectoring<T>(Offset, intermidiate.ThreadPool, intermidiate.Get<T>(SourceID), SectorSize);
		}
	}
}