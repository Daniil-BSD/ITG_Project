using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ITG_Core {

	public struct LayeringProperties {

		public readonly bool checkered;

		public readonly int layeringPower;

		public readonly int stepX;

		public readonly int stepY;

		public LayeringProperties(in int stepY, in int stepX, in bool checkered, in int layeringPower)
		{
			this.stepX = stepX;
			this.stepY = stepY;
			this.checkered = checkered;
			this.layeringPower = layeringPower;
		}
	}

	public class LayerEnumerator : IEnumerator<CoordinateBasic> {

		private CoordinateBasic current;

		private int rowIndex;

		public readonly int height;

		public readonly bool include00;

		public readonly int offsetX;

		public readonly int offsetY;

		public readonly LayeringProperties properties;

		public readonly int width;

		public CoordinateBasic Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => current;
		}

		object IEnumerator.Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Current;
		}

		public LayerEnumerator(in int layerIndex, in LayeringProperties properties, in int width, in int height)
		{
			this.width = width;
			this.height = height;
			this.properties = properties;

			include00 = layerIndex % 2 == 1;
			rowIndex = ( include00 ) ? -1 : 0;

			offsetX = ( ( properties.checkered ) ? layerIndex / 2 : layerIndex ) % properties.stepY;
			offsetY = ( ( properties.checkered ) ? layerIndex / 2 : layerIndex ) / properties.stepY;
			current = new CoordinateBasic(width, offsetY - properties.stepY);
		}

		public void Dispose()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if ( ( current.x += properties.stepX ) >= width ) {
				current.x = offsetX;
				if ( properties.checkered )
					current.x += ( ++rowIndex % 2 == 0 ) ? properties.stepY : 0;
				if ( ( current.y += properties.stepY ) >= height ) {
					return false;
				}
			}
			return true;
		}

		public void Reset()
		{
			rowIndex = -1;
			current.x = width;
			current.y = offsetY - properties.stepY;
		}
	}

	public class LayeringEnumerator : IEnumerator<CoordinateBasic> {

		private LayerEnumerator layerEnumerator;

		public readonly int height;

		public readonly int layeringPower;

		public readonly int layers;

		public readonly LayeringProperties properties;

		public readonly int width;

		public int currentIndex;

		public CoordinateBasic Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => layerEnumerator.Current;
		}

		object IEnumerator.Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Current;
		}

		public LayeringEnumerator(in LayeringProperties properties, in int width, in int height, in int layers)
		{
			this.width = width;
			this.height = height;
			this.properties = properties;
			this.layers = layers;
			currentIndex = 0;
			layerEnumerator = new LayerEnumerator(currentIndex, properties, width, height);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetShuffledIndex(in int indexIN, in LayeringProperties layeringProperties)
		{
			if ( layeringProperties.layeringPower % 2 == 0 )
				return GetShuffledIndex(indexIN, layeringProperties.layeringPower / 2);
			return ( GetShuffledIndex(indexIN >> 1, layeringProperties.layeringPower / 2) << 1 ) + ( ( indexIN % 2 == 0 ) ? 1 : 0 );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetShuffledIndex(in int indexIN, in int widthAsTwoToThePowerOfThis)
		{
			int power = widthAsTwoToThePowerOfThis;
			int powerMone = widthAsTwoToThePowerOfThis - 1;
			int sideLength = 1 << power;
			int x = 0;
			int y = 0;
			for ( int p = 0 ; p < power ; p++ ) {
				int temp = ( indexIN >> ( p * 2 ) ) & 3;
				int a = temp & 1;
				int b = temp >> 1;
				int pInverse = powerMone - p;

				x = ( x << 1 ) | a;
				y = ( y << 1 ) | ( a ^ b );
				//x += a << pInverse;
				//y += ( a ^ b ) << pInverse;
			}
			int indexOut = y * sideLength + x;
			return indexOut;
		}

		public void Dispose()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if ( layerEnumerator.MoveNext() )
				return true;
			currentIndex++;
			if ( currentIndex < layers ) {
				int shuffledIndex = GetShuffledIndex(currentIndex, properties);
				layerEnumerator = new LayerEnumerator(shuffledIndex, properties, width, height);
				layerEnumerator.MoveNext();
				return true;
			}
			return false;
		}

		public void Reset()
		{
			currentIndex = 0;
			layerEnumerator = new LayerEnumerator(currentIndex, properties, width, height);
		}
	}

	public class LayerungEnumeratorBuilder {

		public readonly int layeringPower;

		public readonly int layers;

		public readonly LayeringProperties properties;

		public LayerungEnumeratorBuilder(int layeringPower, float coverageFactor)
		{
			this.layeringPower = layeringPower;
			int totalLayers = 1 << ( layeringPower );
			layers = (int)( totalLayers * coverageFactor );
			bool checkered = layeringPower % 2 == 1;
			int stepY = 1 << ( layeringPower / 2 );
			int stepX = ( checkered ) ? stepY + stepY : stepY;

			properties = new LayeringProperties(stepY, stepX, checkered, layeringPower);
		}

		public LayeringEnumerator BuildEnumerator(int width, int height)
		{
			return new LayeringEnumerator(properties, width, height, layers);
		}
	}
}