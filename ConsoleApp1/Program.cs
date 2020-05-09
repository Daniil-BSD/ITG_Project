namespace ConsoleApp1 {
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Xml.Serialization;
	using ITG_Core;
	using ITG_Core.Base;
	using ITG_Core.Basic.Builders;

	public class Program {

		private static readonly bool BORDERS = false;

		private static readonly float FACTOR = 2f;

		private static readonly bool LAYERED = false;

		private static readonly int LAYERS = 2;

		private static readonly int SIZE = 9;//128;

		private static readonly int SCALE = 1024;//512;

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
			landscapeBuilder["perlin"] = new ParlinGroupBuiler() { Vec2FieldID = "mem1", UpperTargetScale = SCALE, MaxPerlinScale = SCALE / 4, DeltaFactor = 0.625f, ScaleStep = 1.5f, RetFactor = 1f, BottomUp = false, OffsetGlobal = new Coordinate(64, 64), LowerTargetScale = 2 };
			//landscapeBuilder["mem2"] = new MemoryBuilder<float>() { SourceID = "perlin" };
			//0.5325f
			landscapeBuilder["mem2"] = new MemoryBuilder<float>() { SourceID = "perlin" };
			landscapeBuilder["HE"] = new HydraulicErosionBuilder() {
				SourceID = "mem2",
				LayeringPower = 7,
				CoverageFactor = 1 / 4f,
				BrushRadius = 4,
				StepLength = 2f,
				Gravity = 4,
				OutputFactor = 0.2f,
				Friction = 0.5f,
				SedimentCapacityFactor = 1.125f,
				MinModification = 0.01f,
				ErodeSpeed = 0.5f,
				DepositSpeed = 0.125f,
				SedimentFactor = 2,
			};
			landscapeBuilder["HEmem"] = new MemoryStrictSectoringBuilder<float>() { SourceID = "HE", SectorSize = 32 };
			landscapeBuilder["HEmblur"] = new BlurBuilder { SourceID = "HEmem" };
			landscapeBuilder["finalInterpol"] = new InterpolatorBuilder { SourceID = "HEmblur", Scale = 2 };
			landscapeBuilder["finalblur"] = new BlurBuilder { SourceID = "finalInterpol" };
			landscapeBuilder["fianlMem"] = new MemoryBuilder<float>() { SourceID = "HEmblur" };
			landscapeBuilder["HEinv"] = new FloatAdderBuilder() { Sources = new string[] { "fianlMem" }, RetFactor = -1 };
			landscapeBuilder["HEdiff"] = new FloatAdderBuilder() { Sources = new string[] { "HEinv", "mem2" }, RetFactor = 2 };
			landscapeBuilder["output"] = new HeightMapImageOutputterBuilder() { SourceID = "mem2", Layeers = 3, Size = 100, Min = -1, Max = 1 };

			//landscapeBuilder["blur1"] = new BlurBuilder() { SourceID = "mem2" };
			//landscapeBuilder["brush"] = new BrushTestBuilder();





			Landscape landscape = landscapeBuilder.Build();

			//string xml = landscapeBuilder.XML;
			//Console.WriteLine(xml);
			//landscapeBuilder.XML = xml;
			//Console.WriteLine(landscapeBuilder.XML);
			Console.WriteLine("Elapsed={0}", sw.Elapsed);
			Console.WriteLine("Computing...");

			sw.Restart();
			Algorithm<float> outputPerlin = landscape.GetAlgorithm<float>("mem2");
			Algorithm<float> outputHE = landscape.GetAlgorithm<float>("fianlMem");
			Algorithm<float> outputdiff = landscape.GetAlgorithm<float>("HEdiff");
			Outputter<Bitmap> bitmapOutputter = landscape.GetOutputter<Bitmap>("output");

			RequstSector request = new RequstSector(new Coordinate(0, 0), SIZE, SIZE);
			//Sector<float> sectordiff = outputdiff.GetSector(request);
			//Sector<float> sectorHE = outputHE.GetSector(request);
			//Sector<float> sectorPerlin = outputPerlin.GetSector(request);

			sw.Stop();
			Console.WriteLine("Elapsed={0}", sw.Elapsed);
			//Console.WriteLine("Press \"Enter\" to save", sw.Elapsed);
			//Console.ReadKey();

			int width = request.Width_units * 3;
			int height = request.Height_units;

			Bitmap bmp = bitmapOutputter.GetObject(new Coordinate());
			Console.WriteLine("Saving...");
			bmp.Save("D:\\output.png", ImageFormat.Png);

			//Console.WriteLine("min: " + min + " max: " + max + " avg: " + (double) total / (width * height) + " errors: " + errors);
			Console.WriteLine("Saved");
			GC.Collect();
			Console.ReadKey();
		}

	}
}