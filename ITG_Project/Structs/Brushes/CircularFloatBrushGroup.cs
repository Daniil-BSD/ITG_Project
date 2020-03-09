namespace ITG_Core.Brushes {
	/// <summary>
	/// Defines the <see cref="CircularFloatBrushGroup" />
	/// </summary>
	public class CircularFloatBrushGroup : BrushGroup<CircularFloatBrush, float> {
		public readonly CircularBrushMode mode;

		public readonly float radius;

		public readonly float threashhold;

		public CircularFloatBrushGroup(float radius, CircularBrushMode mode, float threashhold, int size) : base(size)
		{
			this.mode = mode;
			this.threashhold = threashhold;
			this.radius = radius;
		}

		protected override CircularFloatBrush NewBrush(in float offsetX, in float offsetY)
		{
			return new CircularFloatBrush(radius, offsetX, offsetY, mode, threashhold);
		}
	}
}
