using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using ITG_Core;
using ITG_Core.Base;
using ITG_Core.Builders.Base;
using Windows.Devices.WiFi;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

namespace Extention {

	public class HeightMapImageOutputterBuilder : OutputterBuilder<SoftwareBitmap, float> {

		public float Min { get; set; } = -1;
		public float Max { get; set; } = 1;
		public int Layers { get; set; } = 1;
		public bool Borders { get; set; } = false;
		public bool Colored { get; set; } = true;
		public int Size { get; set; } = 16;
		public override Outputter<SoftwareBitmap> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			VerifyVallidity(intermidiate);
			return new HeightMapImageOutputter(
				offset: Offset,
				threadPool: intermidiate.ThreadPool,
				source: intermidiate.Get<float>(SourceID),
				size: Size,
				min: Min,
				max: Max,
				layers: Layers,
				borders: Borders,
				colored: Colored);
		}

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			if ( !base.IsValid(landscapeBuilder) )
				return false;
			if ( Min > Max )
				return false;
			if ( Layers < 1 )
				return false;
			if ( Size < 1 )
				return false;
			return true;
		}

		public override List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			List<string> ret = base.ValidityMessages(landscapeBuilder);
			if ( Min > Max )
				ret.Add("Min should be less then Max.");
			if ( Layers < 1 )
				ret.Add("Number of layers has to be positive (above zero).");
			if ( Size < 1 )
				ret.Add("Minimum size is 1.");
			return ret;
		}
	}

	public class HeightMapImageOutputter : Outputter<SoftwareBitmap, float> {

		public readonly int size;

		public const int MAX_SATURAION = ( 1 << 8 ) - 1;
		public const int MAX_SATURAION_p1 = MAX_SATURAION + 1;

		public readonly int layers;
		public readonly bool borders;
		public readonly float factor;
		public readonly float min;
		public readonly bool colored;
		public HeightMapImageOutputter(Coordinate offset, ITGThreadPool threadPool, Algorithm<float> source, int size, float min, float max, int layers, bool borders, bool colored) : base(offset, threadPool, source)
		{
			this.size = size;
			this.layers = layers;
			this.min = min;
			factor = 1f / ( max - min );
			this.borders = borders;
			this.colored = colored;

		}

		protected override SoftwareBitmap GenerarteObject(in Coordinate coordinate)
		{
			RequstSector request = new RequstSector(coordinate * size, size, size);
			Sector<float> sector = source.GetSector(request);
			byte[] imageArray = new byte[sector.Width_units * sector.Height_units * 4];

			for ( int i = 0 ; i < sector.Width_units ; i++ ) {
				for ( int j = 0 ; j < sector.Height_units ; j++ ) {
					float normalized = ( ( sector[i, j] - min ) * factor ) * layers;
					if ( !colored ) {
						int saturation = ( normalized * MAX_SATURAION ).ToIntegerConsistent().Modulo(MAX_SATURAION_p1);
						SetPixelOverride(imageArray, i, j, sector.Width_units, saturation, borders);
					} else {
						int globalPosition = ( normalized * ( MAX_SATURAION_p1 * 6 ) ).ToIntegerConsistent().Modulo(MAX_SATURAION_p1 * 6);
						int position = globalPosition.Modulo(MAX_SATURAION_p1);
						int section = ( globalPosition / MAX_SATURAION_p1 ).Modulo(6);
						position = MAX_SATURAION - position;
						section = 5 - section;
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
						SetPixelOverride(imageArray, i, j, sector.Width_units, R, G, B, borders);
					}
				}
			}

			WriteableBitmap wb = new WriteableBitmap(sector.Width_units, sector.Height_units);

			using ( Stream stream = wb.PixelBuffer.AsStream() ) {
				stream.Write(imageArray, 0, imageArray.Length);
			}

			SoftwareBitmap ret = SoftwareBitmap.CreateCopyFromBuffer(
				wb.PixelBuffer,
				BitmapPixelFormat.Bgra8,
				sector.Width_units,
				sector.Height_units
			);
			return ret;


		}
		public static void SetPixelOverride(byte[] img, in int x, in int y, in int width, int R, int G, int B, bool borders, bool markBordeeres = false)
		{
			int index = ( y * width + x ) * 4;
			img[index + 2] = (byte)( ( markBordeeres && ( R == 0 || R == MAX_SATURAION ) ) ? MAX_SATURAION_p1 / 2 : R );//Red
			img[index + 1] = (byte)( ( x % Constants.CHUNK_SIZE == 0 && borders ) ? MAX_SATURAION_p1 / 2 : G );//Green
			img[index] = (byte)( ( y % Constants.CHUNK_SIZE == 0 && borders ) ? MAX_SATURAION_p1 / 2 : B );//Blue
			img[index + 3] = 255;
		}
		public static void SetPixelOverride(byte[] img, in int x, in int y, in int width, int saturation, bool borders)
		{
			SetPixelOverride(img, x, y, width, saturation, saturation, saturation, borders, markBordeeres: true);
		}
	}
}
