namespace ConsoleApp1 {
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using ITG_Core;
	using ITG_Core.Basic.Builders;

	public class Program {

		private static readonly bool BORDERS = false;

		private static readonly float FACTOR = 2f;

		private static readonly bool LAYERED = false;

		private static readonly int LAYERS = 2;

		private static readonly int SIZE = 32;//128;

		private static readonly int SCALE = 3072;//512;

		private static void Main()
		{
			/*Vec3 v1 = new Vec3(1);
			Console.WriteLine("v1 is: " + v1);
			Vec3 v2 = new Vec3(1, 2, 4);
			Console.WriteLine("v2 is: " + v2);
			Console.WriteLine("Dot(v1, v2): " + Vec3.Dot(v1, v2));
			Console.WriteLine("Cross(v1, v2): " + Vec3.Cross(v1, v2));
			Console.WriteLine("Cross(v2, v1): " + Vec3.Cross(v2, v1));
			Console.WriteLine("v1 + v2: " + ( v1 + v2 ));1
			Console.WriteLine("v2 + v1: " + ( v2 + v1 ));
			Console.WriteLine("v1 - v2: " + ( v1 - v2 ));
			Console.WriteLine("v2 - v1: " + ( v2 - v1 ));
			Console.WriteLine("v1 * v2: " + ( v1 * v2 ));
			Console.WriteLine("v1 * 5: " + ( v1 * 5 ));
			Console.WriteLine("v1 / 5: " + ( v1 / 5 ));
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
			*/
			Stopwatch sw = new Stopwatch();
			Console.WriteLine("Stopwatch Started");
			Console.WriteLine("Building...");
			sw.Start();

			LandscapeBuilder landscapeBuilder = new LandscapeBuilder();

			landscapeBuilder["random"] = new RandomBuilder() { Seed = 6 };
			landscapeBuilder["vec2"] = new Vec2FieldBuilder() { SourceID = "random", Magnitude = Constants.SQRT_2_OVER_2_FLOAT };
			landscapeBuilder["mem1"] = new MemoryBuilder<Vec2>() { SourceID = "vec2" };
			landscapeBuilder["perlin"] = new ParlinGroupBuiler() { Vec2FieldID = "mem1", UpperTargetScale = SCALE, MaxPerlinScale = SCALE / 4, DeltaFactor = 0.5f, ScaleStep = 2f, RetFactor = 1.375f, BottomUp = false, OffsetGlobal = new Coordinate(64, 64), LowerTargetScale = 6 };
			//landscapeBuilder["mem2"] = new MemoryBuilder<float>() { SourceID = "perlin" };
			//0.5325f
			landscapeBuilder["mem2"] = new MemoryBuilder<float>() { SourceID = "perlin" };
			landscapeBuilder["HE"] = new HydraulicErosionBuilder() {
				SourceID = "mem2",
				LayeringPower = 8,
				CoverageFactor = 1 / 16f,
				BrushRadius = 8,
				StepLength = 4,
				Gravity = 2,
			};

			landscapeBuilder["HEmem"] = new MemoryStrictSectoringBuilder<float>() { SourceID = "HE", SectorSize = 32 };
			landscapeBuilder["HEmblur"] = new BlurBuilder { SourceID = "HEmem" };
			landscapeBuilder["HEmblurMeM"] = new MemoryBuilder<float>() { SourceID = "HEmblur" };
			landscapeBuilder["HEinv"] = new FloatAdderBuilder() { Sources = new string[] { "HEmblurMeM" }, RetFactor = -1 };
			landscapeBuilder["HEdiff"] = new FloatAdderBuilder() { Sources = new string[] { "HEinv", "mem2" }, RetFactor = 2 };

			//landscapeBuilder["blur1"] = new BlurBuilder() { SourceID = "mem2" };
			//landscapeBuilder["brush"] = new BrushTestBuilder();
			Landscape landscape = landscapeBuilder.Build();

			Console.WriteLine("Elapsed={0}", sw.Elapsed);
			Console.WriteLine("Computing...");

			sw.Restart();
			ITG_Core.Base.Algorithm<float> outputPerlin = landscape.GetAlgorithm<float>("mem2");
			ITG_Core.Base.Algorithm<float> outputHE = landscape.GetAlgorithm<float>("HEmblurMeM");
			ITG_Core.Base.Algorithm<float> outputdiff = landscape.GetAlgorithm<float>("HEdiff");

			RequstSector request = new RequstSector(new Coordinate(0, 0), SIZE, SIZE);
			Sector<float> sectordiff = outputdiff.GetSector(request);
			Sector<float> sectorHE = outputHE.GetSector(request);
			Sector<float> sectorPerlin = outputPerlin.GetSector(request);
			//Sector<float> area = output.GetSector(new RequstSector(new Coordinate(0, 0), 4, 4));

			sw.Stop();
			Console.WriteLine("Elapsed={0}", sw.Elapsed);
			Console.WriteLine("Press \"Enter\" to save", sw.Elapsed);
			//Console.ReadKey();
			Console.WriteLine("Saving...");

			int width = request.Width_units * 3;
			int height = request.Height_units;
			Bitmap bmp = new Bitmap(width, height);

			Draw(sectorPerlin, bmp, 0, LAYERED);
			Draw(sectorHE, bmp, request.Width_units, LAYERED);
			Draw(sectordiff, bmp, request.Width_units * 2, true);

			bmp.Save("D:\\output.png", ImageFormat.Png);

			//Console.WriteLine("min: " + min + " max: " + max + " avg: " + (double) total / (width * height) + " errors: " + errors);
			Console.WriteLine("Saved");
			GC.Collect();
			Console.ReadKey();
		}

		public static void Draw(Sector<float> area, in Bitmap bmp, int offset, bool layered)
		{
			for ( int i = 0 ; i < area.Width_units ; i++ ) {
				for ( int j = 0 ; j < area.Height_units ; j++ ) {
					if ( layered ) {
						int saturation = ( (int)( ( ( area[i, j] * FACTOR ) ) * 256 ) * LAYERS ).Modulo(256);
						bmp.SetPixel(offset + i, j, Color.FromArgb(( saturation == 0 || saturation == 255 ) ? 127 : saturation, ( i % Constants.CHUNK_SIZE == 0 && BORDERS ) ? 255 : saturation, ( j % Constants.CHUNK_SIZE == 0 && BORDERS ) ? 255 : saturation));
					} else {
						int position = ( (int)( ( ( area[i, j] * -FACTOR ) / 2 + 0.5f ) * 256 * 6 ) ).Modulo(256);
						int section = (int)( ( area[i, j] * -FACTOR / 2 + 0.5f ) * 6 );
						int R = 0;
						int G = 0;
						int B = 0;
						switch ( section ) {
							case 0:
								R = position;
								break;

							case 1:
								R = 255;
								G = position;
								break;

							case 2:
								R = 255 - position;
								G = 255 - position / 8;
								break;

							case 3:
								G = 255 - ( ( 255 - position ) / 8 );
								B = position;
								break;

							case 4:
								G = 255 - position;
								B = 255;
								break;

							case 5:
								R = position;
								G = position;
								B = 255;
								break;

							default:
								R = 255;
								G = 255;
								B = 255;
								break;
						}
						bmp.SetPixel(offset + i, j, Color.FromArgb(R, G, B));
					}
				}
			}
		}
	}
}