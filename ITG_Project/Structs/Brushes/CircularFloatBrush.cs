namespace ITG_Core.Brushes {
	using System;
	using System.Collections.Generic;

	public enum CircularBrushMode {
		Linear, Quadratic_Smooth, Quadratic_EaseOut, Fill
	}

	public class CircularFloatBrush : Brush<float> {
		public const float THREASHHOLD_DEFAULT = 0.01f;

		private readonly BrushTouple<float>[] touples;
		public readonly float sum;

		public CircularFloatBrush(float radius, float xOffset = 0, float yOffset = 0, CircularBrushMode mode = CircularBrushMode.Linear, float threashhold = THREASHHOLD_DEFAULT, bool offsetMod1 = true)
		{
			List<BrushTouple<float>> touplesList = new List<BrushTouple<float>>();
			float radiusSquared = radius * radius;
			radius = Math.Abs(radius);
			if ( offsetMod1 ) {
				xOffset = xOffset.Modulo(1);
				yOffset = yOffset.Modulo(1);
			}
			if ( threashhold < 0 || threashhold >= 1 )
				throw new ArgumentOutOfRangeException(nameof(threashhold));
			for ( int i = -((int) radius) - 1 ; i < (int) radius + 1 ; i++ ) {
				for ( int j = -((int) radius) - 1 ; j < (int) radius + 2 ; j++ ) {
					float value = 0;
					float x = i - xOffset;
					float y = j - yOffset;
					float dSquared = x * x + y * y;
					float d = (float) Math.Sqrt(dSquared);
					switch ( mode ) {
						case CircularBrushMode.Fill:
							if ( d <= radius )
								value = 1;
							break;
						case CircularBrushMode.Quadratic_Smooth:
							value = (radiusSquared - dSquared) / radiusSquared;
							break;
						case CircularBrushMode.Quadratic_EaseOut:
							float temp1 = radius - d;
							float temp2 = temp1 * temp1;
							if ( d < radius )
								value = 1 - ((radiusSquared - temp2) / radiusSquared);
							break;
						case CircularBrushMode.Linear:
						default:
							value = (radius - d) / radius;
							break;
					}
					if ( value > threashhold )
						touplesList.Add(new BrushTouple<float>(new CoordinateBasic(i, j), (value > 1) ? 1f : value));
				}
			}
			touples = new BrushTouple<float>[touplesList.Count];
			sum = 0;
			for ( int i = 0 ; i < touples.Length ; i++ )
				sum += (touples[i] = touplesList[i]).value;
		}

		public BrushTouple<float> this[in int index] => touples[index];

		public BrushTouple<float>[] Touples => touples;
	}
}
