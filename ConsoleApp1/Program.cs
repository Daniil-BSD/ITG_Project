namespace ConsoleApp1 {
	using ITG_Core;
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;

	public class Program {
		private static readonly bool BORDERS = false;

		private static readonly int RADIUS = 12;//128;

		private static readonly int SCALE = 512;//512;

		private static readonly int LAYERS = 12;

		private static void Main(string[] args)
		{

			Vec3 v1 = new Vec3(1);
			Console.WriteLine("v1 is: " + v1);
			Vec3 v2 = new Vec3(1, 2, 4);
			Console.WriteLine("v2 is: " + v2);
			Console.WriteLine("Dot(v1, v2): " + Vec3.Dot(v1, v2));
			Console.WriteLine("Cross(v1, v2): " + Vec3.Cross(v1, v2));
			Console.WriteLine("Cross(v2, v1): " + Vec3.Cross(v2, v1));
			Console.WriteLine("v1 + v2: " + (v1 + v2));
			Console.WriteLine("v2 + v1: " + (v2 + v1));
			Console.WriteLine("v1 - v2: " + (v1 - v2));
			Console.WriteLine("v2 - v1: " + (v2 - v1));
			Console.WriteLine("v1 * v2: " + (v1 * v2));
			Console.WriteLine("v1 * 5: " + (v1 * 5));
			Console.WriteLine("v1 / 5: " + (v1 / 5));
			Console.WriteLine("v1 is: " + v1);
			Console.WriteLine("v2 is: " + v2);

			Console.WriteLine("90 degrees: " + Angle.RightAngle.Vec2);

			Console.WriteLine("180 degrees: " + Angle.StraightAngle.AngleRaw);
			Console.WriteLine("180 degrees: " + Angle.StraightAngle.GetAngle(AngleFormat.Unit));
			Console.WriteLine("180 degrees: " + Angle.StraightAngle.GetAngle(AngleFormat.Degrees));
			Console.WriteLine("180 degrees: " + Angle.StraightAngle.GetAngle(AngleFormat.Radians));
			Console.WriteLine("180 degrees: " + Angle.StraightAngle.Vec2);


			Console.WriteLine("Cross(v1, X): " + Vec3.Cross(v1, new Vec3(1, 0, 0)));
			Console.WriteLine("Cross(v1, Y): " + Vec3.Cross(v1, new Vec3(0, 1, 0)));
			Console.WriteLine("Cross(v1, Z): " + Vec3.Cross(v1, new Vec3(0, 0, 1)));
			Console.WriteLine("v1.CrossX: " + v1.CrossX);
			Console.WriteLine("v1.CrossY: " + v1.CrossY);
			Console.WriteLine("v1.CrossZ: " + v1.CrossZ);

			Sector<float> sectorFloat = new Sector<float>(new Coordinate(0, 0), 1, 1);
			sectorFloat[0, 0] = 0;
			sectorFloat[1, 0] = 50;

			Console.WriteLine("should be 12.5: " + sectorFloat.ValueAt(0.5f, 0.5f));


			Stopwatch sw = new Stopwatch();
			Console.WriteLine("Stopwatch Started");
			Console.WriteLine("Building...");
			sw.Start();

			LandscapeBuilder landscapeBuilder = new LandscapeBuilder();

			landscapeBuilder["random"] = new RandomBuilder() { Seed = 6 };
			landscapeBuilder["vec2"] = new Vec2FieldBuilder() { SourceID = "random", Magnitude = Constants.SQRT_2_OVER_2_FLOAT };
			landscapeBuilder["mem1"] = new MemoryBuilder<Vec2>() { SourceID = "vec2" };
			landscapeBuilder["perlin"] = new ParlinGroupBuiler() { Vec2FieldID = "mem1", UpperTargetScale = SCALE * 2, MaxPerlinScale = SCALE / 4, DeltaFactor = 0.625f, ScaleStep = 1.875f, RetFactor = 1.375f, BottomUp = true, OffsetGlobal = new Coordinate(5, 5), LowerTargetScale = 8 };
			landscapeBuilder["mem2"] = new MemoryBuilder<float>() { SourceID = "perlin" };
			landscapeBuilder["HE"] = new HydrolicErrosionBuilder() {
				SourceID = "mem2",
				OutputFactor = 0.75f,
				LayeringPower = 5,
				LayeringIndexes = new int[] { 0, 1, 4, 5, 10, 11, 14, 15, 16, 17, 20, 21, 26, 27, 30, 31 },
				SedimentCapacityFactor = 6,
				Gravity = 4f,
				StepLength = 1f,
				InitialSpeed = 0f,
				BrushRadius = 3f,
				InitialVolume = 20f,
				EvaporationSpeed = 0.01f,
				MaxIterations = 64,
				Inertia = 0.000005f,
				ErodeSpeed = 0.125f,
				DepositSpeed = 0.0675f,
			};
			landscapeBuilder["HEmem"] = new MemoryBuilder<float>() { SourceID = "HE" };
			landscapeBuilder["HEinv"] = new FloatAdderBuilder() { Sources = new string[] { "HEmem" }, RetFactor = -1 };
			landscapeBuilder["HEdiff"] = new FloatAdderBuilder() { Sources = new string[] { "HEinv", "mem2" }, RetFactor = 2 };

			//landscapeBuilder["blur1"] = new BlurBuilder() { SourceID = "mem2" };
			//landscapeBuilder["brush"] = new BrushTestBuilder();
			Landscape landscape = landscapeBuilder.Build();

			Console.WriteLine("Elapsed={0}", sw.Elapsed);
			Console.WriteLine("Computing...");



			sw.Restart();
			var outputPerlin = landscape.GetAlgorithm<float>("mem2");
			var outputHE = landscape.GetAlgorithm<float>("HEmem");
			var outputdiff = landscape.GetAlgorithm<float>("HEdiff");

			var request = new RequstSector(new Coordinate(-RADIUS, -RADIUS), RADIUS * 2, RADIUS * 2);
			Sector<float> sectorHE = outputHE.GetSector(request);
			Sector<float> sectorPerlin = outputPerlin.GetSector(request);
			Sector<float> sectordiff = outputdiff.GetSector(request);
			//Sector<float> area = output.GetSector(new RequstSector(new Coordinate(0, 0), 4, 4));




			sw.Stop();
			Console.WriteLine("Elapsed={0}", sw.Elapsed);
			Console.WriteLine("Press \"Enter\" to save", sw.Elapsed);
			//Console.ReadKey();
			Console.WriteLine("Saving...");

			int width = request.Width_units * 3;
			int height = request.Height_units;
			Bitmap bmp = new Bitmap(width, height);
			int errors = 0;

			Draw(sectorPerlin, bmp, 0, ref errors);
			Draw(sectorHE, bmp, request.Width_units, ref errors);
			Draw(sectordiff, bmp, request.Width_units * 2, ref errors);


			bmp.Save("D:\\output.png", ImageFormat.Png);

			//Console.WriteLine("min: " + min + " max: " + max + " avg: " + (double) total / (width * height) + " errors: " + errors);
			Console.WriteLine("Saved");

			Console.ReadKey();
		}

		public static void Draw(Sector<float> area, in Bitmap bmp, int offset, ref int errors)
		{
			for ( int i = 0 ; i < area.Width_units ; i++ ) {
				for ( int j = 0 ; j < area.Height_units ; j++ ) {
					int saturation = ((int) ((area[i, j]) * 255) * LAYERS).Modulo(256);
					if ( saturation < 0 || saturation > 255 ) {
						errors++;
						bmp.SetPixel(offset + i, j, Color.FromArgb(255, 0, 0));
					} else {
						bmp.SetPixel(offset + i, j, Color.FromArgb((saturation == 0 || saturation == 255) ? 127 : saturation, (i % Constants.CHUNK_SIZE == 0 && BORDERS) ? 255 : saturation, (j % Constants.CHUNK_SIZE == 0 && BORDERS) ? 255 : saturation));
					}
				}
			}
		}
	}
}
