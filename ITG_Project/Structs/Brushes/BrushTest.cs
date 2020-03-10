namespace ITG_Core {
	using ITG_Core.Brushes;

	/// <summary>
	/// Defines the <see cref="BrushTest" />
	/// </summary>
	public class BrushTest : Algorithm<float> {
		public BrushTest(Coordinate offset, ITGThreadPool threadPool) : base(offset, threadPool)
		{
		}

		protected override Chunk<float> ChunkPopulation(in Coordinate coordinate)
		{
			Chunk<float> chunk = new Chunk<float>();
			for ( int i = 0 ; i < Constants.CHUNK_SIZE ; i++ )
				for ( int j = 0 ; j < Constants.CHUNK_SIZE ; j++ )
					chunk[i, j] = -1;
			CircularFloatBrushGroup brushGroup = new CircularFloatBrushGroup(3.495f, CircularBrushMode.Quadratic_EaseOut, 0.01f, 4);
			int x0 = Constants.CHUNK_SIZE / 2;
			int y0 = Constants.CHUNK_SIZE / 2;
			CircularFloatBrush brush = brushGroup.GetBrush(0.21f * coordinate.x, 0.21f * coordinate.y);
			var touples = brush.Touples;
			for ( int i = touples.Length - 1 ; i >= 0 ; i-- ) {
				int x = x0 + touples[i].offset.x;
				int y = y0 + touples[i].offset.y;
				chunk[x, y] += touples[i].value * 2;
			}
			return chunk;
		}
	}

	/// <summary>
	/// Defines the <see cref="BrushTestBuilder" />
	/// </summary>
	public class BrushTestBuilder : AlgorithmBuilder<float> {
		public override Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new BrushTest(Offset, intermidiate.ThreadPool);
		}
	}
}
