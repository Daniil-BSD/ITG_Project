namespace ConsoleApp1 {
	using ITG_Core;
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Defines the <see cref="Program" />
	/// </summary>
	public class Program {
		private static readonly bool BORDERS = false;

		private static readonly int RADIUS = 32;//128;

		private static readonly int SCALE = 512;//512;

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


			Stopwatch sw = new Stopwatch();
			Console.WriteLine("Stopwatch Started");
			Console.WriteLine("Building...");
			sw.Start();

			LandscapeBuilder landscapeBuilder = new LandscapeBuilder();

			/*
			landscapeBuilder["random"] = new RandomBuilder() { Seed = 6 };
			landscapeBuilder["vec2"] = new Vec2FieldBuilder() { SourceID = "random", Magnitude = Constants.SQRT_2_OVER_2_FLOAT };
			landscapeBuilder["perlin"] = new PerlinNoiseBuilder() { SourceID = "vec2", Scale = SCALE / 4 };
			landscapeBuilder["mem1"] = new MemoryBuilder<float>() { SourceID = "perlin" };
			landscapeBuilder["inerpol"] = new InterpolatorBuilder() { SourceID = "mem1", Scale = 4 };
			*/

			landscapeBuilder["random"] = new RandomBuilder() { Seed = 6 };
			landscapeBuilder["vec2"] = new Vec2FieldBuilder() { SourceID = "random", Magnitude = Constants.SQRT_2_OVER_2_FLOAT };
			landscapeBuilder["mem1"] = new MemoryBuilder<Vec2>() { SourceID = "vec2" };
			landscapeBuilder["ret"] = new ParlinGroupBuiler() { Vec2FieldID = "mem1", TargetScale = SCALE * 2, MaxPerlinScale = SCALE / 4, DeltaFactor = 0.625f, ScaleStep = 1.875f, RetFactor = 1.575f, BottomUp = false };
			Landscape landscape = landscapeBuilder.Build();

			Console.WriteLine("Elapsed={0}", sw.Elapsed);
			Console.WriteLine("Computing...");

			sw.Restart();
			var output = landscape.GetAlgorithm<float>("ret");
			Sector<float> area = output.GetSector(new Sector<float>(new Coordinate(-RADIUS, -RADIUS), RADIUS * 2, RADIUS * 2));




			sw.Stop();
			Console.WriteLine("Elapsed={0}", sw.Elapsed);
			Console.WriteLine("Press \"Enter\" to save", sw.Elapsed);
			Console.ReadKey();
			Console.WriteLine("Saving...");

			int width = area.Width_units;
			int height = area.Height_units;
			Bitmap bmp = new Bitmap(width, height);

			float min = int.MaxValue;
			float max = 0;
			int errors = 0;
			long total = 0;
			for ( int i = 0 ; i < width ; i++ ) {
				for ( int j = 0 ; j < height ; j++ ) {
					int saturation = (int) (((area[i, j] + 1) / 2) * 255 * 5) % 256;

					total += saturation;
					if ( saturation < 0 || saturation > 255 ) {
						errors++;
						bmp.SetPixel(i, j, Color.FromArgb(255, 0, 0));
					} else {
						bmp.SetPixel(i, j, Color.FromArgb((saturation == 0 || saturation == 255) ? 127 : saturation, (i % Constants.CHUNK_SIZE == 0 && BORDERS) ? 255 : saturation, (j % Constants.CHUNK_SIZE == 0 && BORDERS) ? 255 : saturation));
						max = Math.Max(max, area[i, j]);
						min = Math.Min(min, area[i, j]);
					}
				}
			}
			bmp.Save("D:\\output.png", ImageFormat.Png);

			Console.WriteLine("min: " + min + " max: " + max + " avg: " + (double) total / (width * height) + " errors: " + errors);
			Console.WriteLine("Saved");

			Console.ReadKey();
		}
	}
}
