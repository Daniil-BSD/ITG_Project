using System;
using System.Drawing;
using ITG_Core;
using ITG_Core.Base;
using ITG_Core.Builders.Base;

namespace ConsoleApp1 {

	public static class BitmapExt {

		public static void SetPixelOverride(this Bitmap bmp, in int x, in int y, int R, int G, int B, bool borders)
		{
			bmp.SetPixel(x, y, Color.FromArgb(( ( ( x % Constants.CHUNK_SIZE == 0 ) || ( y % Constants.CHUNK_SIZE == 0 ) ) && borders ) ? HeightMapImageOutputter.MAX_SATURAION / 2 : R, ( x % Constants.CHUNK_SIZE == 0 && borders ) ? HeightMapImageOutputter.MAX_SATURAION : G, ( y % Constants.CHUNK_SIZE == 0 && borders ) ? HeightMapImageOutputter.MAX_SATURAION : B));
		}

		public static void SetPixelOverride(this Bitmap bmp, in int x, in int y, int saturation, bool borders)
		{
			bmp.SetPixel(x, y, Color.FromArgb(
				( saturation == 0 || saturation == HeightMapImageOutputter.MAX_SATURAION ) ? HeightMapImageOutputter.MAX_SATURAION / 2 : saturation,
				( x % Constants.CHUNK_SIZE == 0 && borders ) ? HeightMapImageOutputter.MAX_SATURAION : saturation,
				( y % Constants.CHUNK_SIZE == 0 && borders ) ? HeightMapImageOutputter.MAX_SATURAION : saturation));
		}
	}

	public class HeightMapImageOutputter : Outputter<Bitmap, float> {

		public const int MAX_SATURAION = ( 1 << 8 ) - 1;

		public const int MAX_SATURAION_p1 = MAX_SATURAION + 1;

		public readonly bool borders;

		public readonly bool colored;

		public readonly float factor;

		public readonly int layers;

		public readonly float min;

		public readonly int size;

		public HeightMapImageOutputter(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> source, int size, float min, float max, int layers, bool borders, bool colored) : base(offset, threadPool, source)
		{
			this.size = size;
			this.layers = layers;
			this.min = min;
			factor = 1f / ( max - min );
			this.borders = borders;
			this.colored = colored;
		}

		protected override Bitmap GenerarteObject(in Coordinate coordinate)
		{
			RequstSector request = new RequstSector(coordinate * size, size, size);
			Sector<float> sector = source.GetSector(request);
			Bitmap ret = new Bitmap(sector.Width_units, sector.Height_units);
			for ( int i = 0 ; i < sector.Width_units ; i++ ) {
				for ( int j = 0 ; j < sector.Height_units ; j++ ) {
					float normalized = ( ( sector[i, j] - min ) * factor );
					if ( !colored ) {
						int saturation = ( (int)( normalized * MAX_SATURAION * layers ) ).Modulo(256);
						ret.SetPixelOverride(i, j, saturation, borders);
					} else {
						int position = ( (int)( normalized * ( MAX_SATURAION_p1 * 6 ) * layers ) ).Modulo(MAX_SATURAION_p1);
						int section = ( (int)( normalized * 6 * layers ) ).Modulo(6);
						//position = MAX_SATURAION - position;
						//section = 5 - section;
						int R = 0;
						int G = 0;
						int B = 0;
						switch ( section ) {
							case 0:
								R = position;
								break;
							case 1:
								R = MAX_SATURAION;
								G = position;
								break;
							case 2:
								R = MAX_SATURAION - position;
								G = MAX_SATURAION - position / 16;
								break;
							case 3:
								G = MAX_SATURAION - ( ( MAX_SATURAION - position ) / 16 );
								B = position;
								break;
							case 4:
								G = MAX_SATURAION - position;
								B = MAX_SATURAION;
								break;
							case 5:
								B = MAX_SATURAION - position;
								break;
							default:
								R = MAX_SATURAION;
								G = MAX_SATURAION;
								B = MAX_SATURAION;
								break;
						}
						ret.SetPixelOverride(i, j, R, G, B, borders);
					}
				}
			}
			return ret;
		}
	}

	public class HeightMapImageOutputterBuilder : OutputterBuilder<Bitmap, float> {

		public bool Borders { get; set; } = false;

		public bool Colored { get; set; } = true;

		public int Layeers { get; set; } = 1;

		public float Max { get; set; } = 1;

		public float Min { get; set; } = -1;

		public int Size { get; set; } = 16;

		public override Outputter<Bitmap> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new HeightMapImageOutputter(
				offset: Offset,
				threadPool: intermidiate.ThreadPool,
				source: intermidiate.Get<float>(SourceID),
				size: Size,
				min: Min,
				max: Max,
				layers: Layeers,
				borders: Borders,
				colored: Colored);
		}
	}
}