namespace ITG_Core {
	/// <summary>
	/// Defines the <see cref="Transformation" />
	/// </summary>
	public class Transformation {
		public readonly Angle angle;

		public readonly float scaleX;

		public readonly float scaleY;

		public readonly float x;

		public readonly float y;

		public Transformation(float x, float y, float scale, Angle angle = new Angle()) : this(x, y, scale, scale, angle)
		{
		}

		public Transformation(float x, float y, float scaleX, float scaleY, Angle angle = new Angle())
		{
			this.x = x;
			this.y = y;
			this.scaleX = scaleX;
			this.scaleY = scaleY;
			this.angle = angle;
		}
	}
}
