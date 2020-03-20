namespace ITG_Core.Basic {
	using System;
	using System.Runtime.CompilerServices;
	using ITG_Core.Base;
	using ITG_Core.Brushes;

	/// <summary>
	/// Defines the <see cref="HydrolicErrosion" />
	/// </summary>
	public class HydrolicErosion : Layer<float, float> {

		private readonly CircularFloatBrushGroup depositBrushes;

		private readonly CircularFloatBrushGroup errosionBrushes;

		private readonly LayerungEnumeratorBuilder layeringEnumeratorBuilder;

		private readonly int maxSectorSize;

		public static readonly int BRUSHGROUP_SIZE = 4;

		public readonly float brushRadius;

		public readonly float depositSpeed;

		public readonly float erodeSpeed;

		public readonly int extraDropletRadius = 4;

		public readonly int extraHeightmapRadius = 5;

		public readonly float gravity;

		public readonly float initialSpeed;

		public readonly float initialVolume;

		public readonly int maxIterations;

		public readonly float minModification;

		public readonly float minSedimentCapacity;

		public readonly float outputFactor;

		public readonly float sedimentCapacityFactor;

		public readonly float speedStepFactor;

		public readonly float stepLength;

		public readonly float volumeStepFactor;

		public override int StdSectorSize => maxSectorSize;

		public HydrolicErosion(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> source, float brushRadius, float depositSpeed, float erodeSpeed, float evaporationSpeed, float gravity, float initialSpeed, float initialVolume, int layeringPower, int maxIterations, float minSedimentCapacity, float outputFactor, float sedimentCapacityFactor, float stepLength, float friction, float minModification, int maxSectorSize, float coverageFactor) : base(offset, threadPool, source)
		{
			this.brushRadius = brushRadius;
			this.depositSpeed = depositSpeed;
			this.erodeSpeed = -erodeSpeed;
			volumeStepFactor = 1f - evaporationSpeed;
			speedStepFactor = 1f - friction;
			this.gravity = gravity;
			this.initialSpeed = initialSpeed;
			this.initialVolume = initialVolume;
			this.maxIterations = maxIterations;
			this.minSedimentCapacity = minSedimentCapacity;
			this.outputFactor = outputFactor;
			this.sedimentCapacityFactor = sedimentCapacityFactor;
			this.stepLength = stepLength;
			this.minModification = minModification;
			this.maxSectorSize = maxSectorSize;

			errosionBrushes = new CircularFloatBrushGroup(brushRadius, CircularBrushMode.Quadratic_EaseOut, Constants.AROUND_0_POSITIVE, BRUSHGROUP_SIZE);
			depositBrushes = new CircularFloatBrushGroup(brushRadius, CircularBrushMode.Quadratic_Smooth, Constants.AROUND_0_POSITIVE, BRUSHGROUP_SIZE);
			layeringEnumeratorBuilder = new LayerungEnumeratorBuilder(layeringPower, coverageFactor);

			extraDropletRadius = (int)( ( brushRadius + stepLength * maxIterations ) / ( Constants.CHUNK_SIZE ) );
			extraHeightmapRadius = 1 + (int)( extraDropletRadius * 1.125f );
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Sector<float> SectorPopulation(in RequstSector requstSector)
		{
			RequstSector outgoingRequestSector = requstSector.GetExpandedCopy(extraHeightmapRadius);
			Sector<float> heightmap = source.GetSector(outgoingRequestSector).GetDeepCopy();

			RequstSector enumeratorRequestSector = requstSector.GetExpandedCopy(extraDropletRadius);
			Vec2 enumeratorOffset = new Vec2(( extraHeightmapRadius - extraDropletRadius ) * Constants.CHUNK_SIZE);
			LayeringEnumerator enumerator = layeringEnumeratorBuilder.BuildEnumerator(enumeratorRequestSector.Width_units, enumeratorRequestSector.Height_units);

			int margin = (int)stepLength + (int)brushRadius + 2;
			int margin_right = heightmap.Width_units - margin;
			int margin_top = heightmap.Height_units - margin;

			int margin_big = margin + (int)( stepLength * maxIterations );
			float minSedimentCapacityPlusMinModificatioon = minSedimentCapacity + minModification;
			while ( enumerator.MoveNext() ) {
				float sediment = 0;
				float speed = initialSpeed;
				float volume = initialVolume;
				float sedimentCapacity = 1;
				Vec2 position = enumerator.Current + enumeratorOffset;
				bool borderline = ( position.x < margin_big ||
						position.y < margin_big ||
						position.x > heightmap.Width_units - margin_big ||
						position.y > heightmap.Height_units - margin_big
						);// true if droplet could fall off the edge
				CoordinateBasic positionInt = (CoordinateBasic)position;
				Vec2 positionWithinCell = new Vec2(position.x - positionInt.x, position.y - positionInt.y);
				float height = GetHeight(heightmap, positionWithinCell, positionInt);
				Vec2 dir = GetGradients(heightmap, positionWithinCell, positionInt);

				dir.Magnitude = speed;
				float heightNext;
				Vec2 positionNext;
				CoordinateBasic positionIntNext;
				Vec2 positionWithinCellNext;
				Vec2 gradients;
				int lifetime;
				for ( lifetime = 0 ; lifetime < maxIterations && sedimentCapacity > -1 ; lifetime++ ) {
					gradients = GetGradients(heightmap, positionWithinCell, positionInt);
					gradients.Magnitude = stepLength;
					if ( gradients.x == 0 && gradients.y == 0 )
						dir = dir * speedStepFactor;
					dir = dir * speedStepFactor + gradients.NormalizedCopy * -gravity;
					speed = dir.Magnitude;
					if ( float.IsInfinity(speed) )
						speed = 0;

					if ( dir.MagnitudeSquared <= Constants.AROUND_0_POSITIVE ) {
						dir = gradients * Constants.AROUND_0_POSITIVE;
					}
					Vec2 dirNormalized = dir;
					dirNormalized.Magnitude = stepLength;

					positionNext = position + dirNormalized;

					positionIntNext = (CoordinateBasic)positionNext;

					if ( borderline &&
						( positionNext.x < margin ||
						positionNext.y < margin ||
						positionNext.x > margin_right ||
						positionNext.y > margin_top
						)
						) {
						break;
					}
					positionWithinCellNext = new Vec2(positionNext.x - positionIntNext.x, positionNext.y - positionIntNext.y);
					heightNext = GetHeight(heightmap, positionWithinCellNext, positionIntNext);

					float deltaHeight = heightNext - height;

					sedimentCapacity = MathExt.Max(-deltaHeight * speed * volume * sedimentCapacityFactor, minSedimentCapacity);
					float sedimentMinusSedimentCapacity = sediment - sedimentCapacity;
					if ( sedimentMinusSedimentCapacity > 0 || deltaHeight > 0 ) {
						//float amountToDeposit = (deltaHeight > 0) ? MathExt.Min(deltaHeight, sediment) : MathExt.Max((sediment - sedimentCapacity) * depositSpeed, (sedimentCapacity < minDeposit * 10) ? minDeposit : 0);
						float amountToDeposit = ( deltaHeight > 0 ) ? MathExt.Min(deltaHeight, sediment) : MathExt.Max(sedimentMinusSedimentCapacity * depositSpeed, ( sedimentCapacity <= minSedimentCapacityPlusMinModificatioon ) ? minModification : 0);
						if ( amountToDeposit >= minModification ) {
							sediment -= MathExt.Min(amountToDeposit, sediment);
							CircularFloatBrush depositBrush = depositBrushes.GetBrush(position);
							BrushTouple<float>[] depositBrushSamples = depositBrush.Touples;
							float depositHeight = ( amountToDeposit / depositBrush.sum ) * outputFactor;

							for ( int i = 0 ; i < depositBrushSamples.Length ; i++ ) {
								heightmap[depositBrushSamples[i].offset + positionInt] += depositBrushSamples[i].value * depositHeight;
							}

							if ( sediment == 0 && deltaHeight < 0 ) {
								break;
							}
						}
					} else {
						float amountToErode = sedimentMinusSedimentCapacity * erodeSpeed;

						if ( amountToErode >= minModification ) {
							CircularFloatBrush errosionBrush = errosionBrushes.GetBrush(position);
							BrushTouple<float>[] errosionBrushSamples = errosionBrush.Touples;
							float errosionHeight = MathExt.Min(( amountToErode / errosionBrush.sum ), -deltaHeight) * outputFactor;

							for ( int i = 0 ; i < errosionBrushSamples.Length ; i++ ) {
								heightmap[errosionBrushSamples[i].offset + positionInt] -= errosionBrushSamples[i].value * errosionHeight;
							}
							sediment += errosionHeight * errosionBrush.sum;
						}
					}

					position = positionNext;
					positionInt = positionIntNext;
					positionWithinCell = positionWithinCellNext;
					height = heightNext;
					volume *= volumeStepFactor;
				}
			}
			return heightmap.GetCopy(extraHeightmapRadius);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vec2 GetGradients(in Sector<float> heightmap, in Vec2 positionWithinCell, in CoordinateBasic positionInt)
		{
			int x = positionInt.x;
			int y = positionInt.y;
			int xpo = x + 1;
			int ypo = y + 1;

			float val00 = heightmap[x, y];
			float val01 = heightmap[x, ypo];
			float val10 = heightmap[xpo, y];
			float val11 = heightmap[xpo, ypo];

			float gradXtop = ( val11 - val01 );
			float gradXbottom = ( val10 - val00 );

			float gradYright = ( val11 - val10 );
			float gradYleft = ( val01 - val00 );

			return new Vec2(gradXbottom + positionWithinCell.x * ( gradXtop - gradXbottom ),
				gradYleft + positionWithinCell.y * ( gradYright - gradYleft )
				);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetHeight(in Sector<float> heightmap, in Vec2 positionWithinCell, in CoordinateBasic positionInt)
		{
			int x = positionInt.x;
			int y = positionInt.y;
			int xpo = x + 1;
			int ypo = y + 1;

			return Interpolator.ComputeStatic(heightmap[x, y], heightmap[x, ypo], heightmap[xpo, y], heightmap[xpo, ypo], positionWithinCell);
		}
	}
}