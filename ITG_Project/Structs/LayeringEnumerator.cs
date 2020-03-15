namespace ITG_Core.old {
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Defines the <see cref="LayeringEnumerator" />
	/// </summary>
	public class LayeringEnumerator : IEnumerator<CoordinateBasic> {
		private readonly IEnumerator<CoordinateBasic>[] enumerators;

		private int currentIndex;

		private bool reset;

		public CoordinateBasic Current {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return enumerators[currentIndex].Current;
			}
		}

		object IEnumerator.Current {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return Current;
			}
		}

		public LayeringEnumerator(IEnumerator<CoordinateBasic>[] enumerators)
		{
			this.enumerators = enumerators;
			currentIndex = -1;
			reset = true;
		}

		public void Dispose()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if ( reset ) {
				reset = false;
				currentIndex++;
			}
			if ( enumerators[currentIndex].MoveNext() )
				return true;
			currentIndex++;
			if ( currentIndex < enumerators.Length )
				return enumerators[currentIndex].MoveNext();
			return false;
		}

		public void Reset()
		{
			for ( int i = 0 ; i < enumerators.Length ; i++ ) {
				enumerators[i].Reset();
			}
			currentIndex = -1;
			reset = true;
		}
	}

	/// <summary>
	/// Defines the <see cref="LayeringEnumeratorBasic" />
	/// </summary>
	public class LayeringEnumeratorBasic : IEnumerator<CoordinateBasic> {
		public readonly bool checkered;

		public readonly int height;

		public readonly bool include00;

		public readonly int offsetX;

		public readonly int stepX;

		public readonly int stepY;

		public readonly int width;

		private CoordinateBasic current;

		private LayeringEnumeratorBasicBuilder parent;

		private int rowIndex;

		public CoordinateBasic Current {

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return current;
			}
		}

		object IEnumerator.Current {

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				return Current;
			}
		}

		public LayeringEnumeratorBasic(LayeringEnumeratorBasicBuilder parent, int width, int height)
		{
			this.parent = parent;
			this.width = width;
			this.height = height;
			rowIndex = (include00) ? -1 : 0;
			current = new CoordinateBasic(width, parent.offsetY - parent.stepY);

			offsetX = parent.offsetX;
			stepX = parent.stepX;
			stepY = parent.stepY;
			checkered = parent.checkered;
			include00 = parent.include00;
		}

		public void Dispose()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if ( (current.x += stepX) >= width ) {
				current.x = offsetX;
				if ( checkered )
					current.x += (++rowIndex % 2 == 0) ? stepY : 0;
				if ( (current.y += stepY) >= height ) {
					return false;
				}
			}
			return true;
		}

		public void Reset()
		{
			rowIndex = -1;
			current.x = width;
			current.y = parent.offsetY - parent.stepY;
		}
	}

	/// <summary>
	/// Defines the <see cref="LayeringEnumeratorBasicBuilder" />
	/// </summary>
	public class LayeringEnumeratorBasicBuilder {
		public readonly bool checkered;

		public readonly bool include00;

		public readonly int layeringIndex;

		public readonly int layeringPower;

		public readonly int offsetX;

		public readonly int offsetY;

		public readonly int stepX;

		public readonly int stepY;

		public LayeringEnumeratorBasicBuilder(in int layeringPower, in int layeringIndex)
		{
			this.layeringPower = layeringPower;
			this.layeringIndex = layeringIndex % layeringPower;
			int layers = 1 << (layeringPower);
			checkered = layeringPower % 2 == 1;
			include00 = layeringIndex % 2 == 0;
			stepY = 1 << (layeringPower / 2);
			stepX = (checkered) ? stepY + stepY : stepY;
			offsetX = ((checkered) ? layeringIndex / 2 : layeringIndex) % stepY;
			offsetY = ((checkered) ? layeringIndex / 2 : layeringIndex) / stepY;
		}

		public virtual IEnumerator<CoordinateBasic> GetEnumerator(in int width, in int height)
		{
			return new LayeringEnumeratorBasic(this, width, height);
		}
	}

	/// <summary>
	/// Defines the <see cref="LayeringEnumeratorBuilder" />
	/// </summary>
	public class LayeringEnumeratorBuilder {
		private readonly LayeringEnumeratorBasicBuilder[] basicBuilders;

		public LayeringEnumeratorBuilder(in int layeringPower, in int layeringIndex)
		{
			this.basicBuilders = new LayeringEnumeratorBasicBuilder[1] { new LayeringEnumeratorBasicBuilder(layeringPower, layeringIndex) };
		}

		public LayeringEnumeratorBuilder(in int layeringPower, in int[] layeringIndexes)
		{
			basicBuilders = new LayeringEnumeratorBasicBuilder[layeringIndexes.Length];
			for ( int i = 0 ; i < layeringIndexes.Length ; i++ ) {
				basicBuilders[i] = new LayeringEnumeratorBasicBuilder(layeringPower, layeringIndexes[i]);
			}
		}

		public IEnumerator<CoordinateBasic> GetEnumerator(in int width, in int height)
		{
			if ( basicBuilders.Length == 1 )
				return basicBuilders[0].GetEnumerator(width, height);

			var enumerators = new IEnumerator<CoordinateBasic>[basicBuilders.Length];
			for ( int i = 0 ; i < basicBuilders.Length ; i++ ) {
				enumerators[i] = basicBuilders[i].GetEnumerator(width, height);
			}
			return new LayeringEnumerator(enumerators);
		}
	}
}
