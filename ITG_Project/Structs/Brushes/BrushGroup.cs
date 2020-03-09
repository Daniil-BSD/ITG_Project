namespace ITG_Core.Brushes {
	using System;

	/// <summary>
	/// Defines the <see cref="BrushGroup{B, T}" />
	/// </summary>
	/// <typeparam name="B"></typeparam>
	/// <typeparam name="T"></typeparam>
	public abstract class BrushGroup<B, T> where B : Brush<T> where T : struct {
		public readonly int size;

		public readonly float step;

		private B[,] brushes;

		public BrushGroup(int size)
		{
			this.size = size;
			this.step = (1f / size);
			brushes = new B[size, size];
		}

		public void Drop()
		{
			brushes = new B[size, size];
		}

		public B GetBrush(in float x, in float y)
		{
			float lx = x.Modulo(1);
			float ly = y.Modulo(1);

			int indexX = (int) (lx * size);
			int indexY = (int) (ly * size);
			if ( brushes[indexX, indexY] == null )
				brushes[indexX, indexY] = NewBrush(lx, ly);
			return brushes[indexX, indexY];
		}

		public B GetBrush(Vec2 vec2)
		{
			return GetBrush(vec2.x, vec2.y);
		}

		protected abstract B NewBrush(in float offsextX, in float offsetY);
	}
}
