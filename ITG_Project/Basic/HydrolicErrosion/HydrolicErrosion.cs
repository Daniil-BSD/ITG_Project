namespace ITG_Core {
	using ITG_Core.Brushes;
	using System;

	/// <summary>
	/// Defines the <see cref="HydrolicErrosion" />
	/// </summary>
	public class HydrolicErrosion : Layer<float, float> {
		public static readonly int BRUSHGROUP_SIZE = 4;

		public readonly int extraHeightmapRadius = 5;

		public readonly int extraDropletRadius = 4;

		public readonly float brushRadius;

		public readonly float depositSpeed;

		public readonly float erodeSpeed;

		public readonly float gravity;

		public readonly float inertia;

		public readonly float initialSpeed;

		public readonly float initialVolume;

		public readonly int maxIterations;

		public readonly float maxSedimentForTermination;

		public readonly float minSedimentCapacity;

		public readonly float minVolume;

		public readonly float outputFactor;

		public readonly float sedimentCapacityFactor;

		public readonly float speedStepFactor;

		public readonly float stepLength;

		public readonly float volumeStepFactor;

		private readonly CircularFloatBrushGroup depositBrushes;

		private readonly CircularFloatBrushGroup errosionBrushes;

		private LayeringEnumeratorBuilder layeringEnumeratorBuilder;

		public HydrolicErrosion(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> source, float brushRadius, float depositSpeed, float erodeSpeed, float evaporationSpeed, float gravity, float inertia, float initialSpeed, float initialVolume, int layeringPower, int[] layeringIndexes, int maxIterations, float maxSedimentForTermination, float minSedimentCapacity, float minVolume, float outputFactor, float sedimentCapacityFactor, float stepLength, float friction) : base(offset, threadPool, source)
		{
			this.brushRadius = brushRadius;
			this.depositSpeed = depositSpeed;
			this.erodeSpeed = erodeSpeed;
			this.volumeStepFactor = 1f - evaporationSpeed;
			this.speedStepFactor = 1f - friction;
			this.gravity = gravity;
			this.inertia = inertia;
			this.initialSpeed = initialSpeed;
			this.initialVolume = initialVolume;
			this.maxIterations = maxIterations;
			this.maxSedimentForTermination = maxSedimentForTermination;
			this.minSedimentCapacity = minSedimentCapacity;
			this.minVolume = minVolume;
			this.outputFactor = outputFactor;
			this.sedimentCapacityFactor = sedimentCapacityFactor;
			this.stepLength = stepLength;

			errosionBrushes = new CircularFloatBrushGroup(brushRadius, CircularBrushMode.Linear, Constants.AROUND_0_POSITIVE, BRUSHGROUP_SIZE);
			depositBrushes = new CircularFloatBrushGroup(brushRadius, CircularBrushMode.Linear, Constants.AROUND_0_POSITIVE, BRUSHGROUP_SIZE);
			layeringEnumeratorBuilder = new LayeringEnumeratorBuilder(layeringPower, layeringIndexes);

			extraDropletRadius = (int) ((brushRadius + stepLength * maxIterations) / (Constants.CHUNK_SIZE));
			extraHeightmapRadius = 1 + (int) (extraDropletRadius * 1.125f);
		}

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

			float gradXtop = (val11 - val01);
			float gradXbottom = (val10 - val00);

			float gradYright = (val11 - val10);
			float gradYleft = (val01 - val00);


			return new Vec2(gradXbottom + positionWithinCell.x * (gradXtop - gradXbottom),
				gradYleft + positionWithinCell.y * (gradYright - gradYleft)
				);
		}

		public static float GetHeight(in Sector<float> heightmap, in Vec2 positionWithinCell, in CoordinateBasic positionInt)
		{
			int x = positionInt.x;
			int y = positionInt.y;
			int xpo = x + 1;
			int ypo = y + 1;

			return Interpolator.ComputeStatic(heightmap[x, y], heightmap[x, ypo], heightmap[xpo, y], heightmap[xpo, ypo], positionWithinCell);
		}

		protected override Sector<float> SectorPopulation(in RequstSector requstSector)
		{
			var outgoingRequestSector = requstSector.GetExpandedCopy(extraHeightmapRadius);
			var heightmap = source.GetSector(outgoingRequestSector).GetDeepCopy();

			var enumeratorRequestSector = requstSector.GetExpandedCopy(extraDropletRadius);
			var enumeratorOffset = new Vec2((extraHeightmapRadius - extraDropletRadius) * Constants.CHUNK_SIZE);
			var enumerator = layeringEnumeratorBuilder.GetEnumerator(enumeratorRequestSector.Width_units, enumeratorRequestSector.Height_units);

			int margin = (int) stepLength + (int) brushRadius + 2;
			int margin_right = heightmap.Width_units - margin;
			int margin_top = heightmap.Height_units - margin;

			int margin_big = margin + (int) (stepLength * maxIterations);

			while ( enumerator.MoveNext() ) {
				float sediment = 0;
				float speed = initialSpeed;
				float volume = initialVolume;
				float sedimentCapacity = 1;
				Vec2 position = enumerator.Current + enumeratorOffset;

				bool borderline = (position.x < margin_big ||
						position.y < margin_big ||
						position.x > heightmap.Width_units - margin_big ||
						position.y > heightmap.Height_units - margin_big
						);// true if droplet will never fall off the edge
				Vec2 dir = new Vec2();
				CoordinateBasic positionInt = (CoordinateBasic) position;
				Vec2 positionWithinCell = new Vec2(position.x - positionInt.x, position.y - positionInt.y);
				float height = GetHeight(heightmap, positionWithinCell, positionInt);

				float heightNext;
				Vec2 positionNext;
				CoordinateBasic positionIntNext;
				Vec2 positionWithinCellNext;
				Vec2 gradients;
				int lifetime;
				for ( lifetime = 0 ; lifetime < maxIterations && sedimentCapacity > -1 ; lifetime++ ) {
					gradients = GetGradients(heightmap, positionWithinCell, positionInt);
					gradients.Magnitude = stepLength;
					//gradients.Magnitude = stepLength;
					//Console.WriteLine((-gradients + inertia * (dir + gradients)) + ", " + dir);
					dir = -gradients + inertia * (dir + gradients);
					//dir = (-gradients * (1 - inertia)) + dir * inertia;
					//Console.WriteLine(dir);
					if ( dir.MagnitudeSquared <= Constants.AROUND_0_POSITIVE ) {
						dir = gradients;
					}
					dir.Magnitude = stepLength;
					positionNext = position + dir;
					positionIntNext = (CoordinateBasic) positionNext;

					if ( borderline &&
						(positionNext.x < margin ||
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


					float amountToErode, errosionHeight, amountToDeposit, depositHeight;
					if ( sediment > sedimentCapacity || deltaHeight > 0 ) {

						float minDeposit = 0.001f;
						amountToDeposit = (deltaHeight > 0) ? MathExt.Min(deltaHeight, sediment) : MathExt.Max((sediment - sedimentCapacity) * depositSpeed, minDeposit);
						sediment -= amountToDeposit;
						var depositBrush = depositBrushes.GetBrush(position);
						var depositBrushSamples = depositBrush.Touples;
						depositHeight = (amountToDeposit / depositBrush.sum) * outputFactor;

						for ( int i = 0 ; i < depositBrushSamples.Length ; i++ ) {
							heightmap[depositBrushSamples[i].offset + positionInt] += depositBrushSamples[i].value * depositHeight;
						}

						if ( sediment == 0 && deltaHeight > 0 && lifetime > 7 ) {
							//Console.WriteLine(lifetime);
							break;
						}

					} else {
						amountToErode = (sedimentCapacity - sediment) * erodeSpeed;
						var errosionBrush = errosionBrushes.GetBrush(position);
						var errosionBrushSamples = errosionBrush.Touples;
						errosionHeight = MathExt.Min((amountToErode / errosionBrush.sum), -deltaHeight) * outputFactor;


						for ( int i = 0 ; i < errosionBrushSamples.Length ; i++ ) {
							heightmap[errosionBrushSamples[i].offset + positionInt] -= errosionBrushSamples[i].value * errosionHeight;
						}
						sediment += errosionHeight * errosionBrush.sum;
					}

					position = positionNext;
					positionInt = positionIntNext;
					positionWithinCell = positionWithinCellNext;
					height = heightNext;
					var speedSquared = speed * speed * speedStepFactor + deltaHeight * gravity;
					if ( speedSquared < 0 ) {
						speedSquared = -speedSquared;
						dir = -dir;
					}
					speed = (float) Math.Sqrt(speedSquared);
					volume *= volumeStepFactor;
				}
			}
			return heightmap.GetCopy(extraHeightmapRadius);
		}
	}
}
