namespace ITG_Core {
	using System;

	public enum AngleFormat {
		Unit,
		Radians,
		Degrees
	}

	/// <summary>
	/// Defines the <see cref="Angle" />
	/// </summary>
	public struct Angle {
		public readonly static Angle HalfRightAngle = new Angle((uint.MaxValue >> 3) + 1);

		public readonly static Angle RightAngle = new Angle((uint.MaxValue >> 2) + 1);

		public readonly static Angle StraightAngle = new Angle((uint.MaxValue >> 1) + 1);

		private uint angle;

		public uint AngleRaw => angle;

		public AngleFormat DefaultFormat { get; set; }

		public bool isZero => angle == 0;

		public Vec2 Vec2 => new Vec2((float) Math.Cos(GetAngle(AngleFormat.Radians)), (float) Math.Sin(GetAngle(AngleFormat.Radians)));

		public Angle(double input, AngleFormat format = AngleFormat.Unit) : this()
		{
			SetAngle(input, format);
		}

		public Angle(uint input, AngleFormat format = AngleFormat.Unit) : this()
		{
			SetAngle(input, format);
		}

		public override bool Equals(object obj)
		{
			if ( !(obj is Angle) ) {
				return false;
			}
			var a = (Angle) obj;
			return angle == a.angle;
		}

		public double GetAngle()
		{
			return GetAngle(DefaultFormat);
		}

		public double GetAngle(AngleFormat format)
		{
			double temp = angle / (((double) uint.MaxValue) + 1);
			switch ( format ) {
				case AngleFormat.Unit:
					return temp;
				case AngleFormat.Radians:
					return temp * (Math.PI * 2);
				case AngleFormat.Degrees:
					return temp * 360;
				default:
					throw new NotImplementedException(format + " format is not supportd by this method.(unreachable code)");
			}
		}

		public override int GetHashCode()
		{
			return (int) angle;
		}

		public void SetAngle(double input, AngleFormat format = AngleFormat.Unit)
		{
			bool inverted = input < 0;
			double absolute = (inverted) ? -input : input;
			switch ( format ) {
				case AngleFormat.Unit:
					angle = (uint) ((absolute % 1) * uint.MaxValue);
					break;
				case AngleFormat.Radians:
					angle = (uint) (((absolute / (Math.PI * 2)) % 1) * uint.MaxValue);
					break;
				case AngleFormat.Degrees:
					angle = (uint) (((absolute / 360) % 1) * uint.MaxValue);
					break;
				default:
					throw new NotImplementedException(format + " format is not supportd by this method.(unreachable code)");
			}
			if ( inverted )
				angle = unchecked(uint.MaxValue - angle + 1);
		}

		public void SetAngle(uint input, AngleFormat format = AngleFormat.Unit)
		{
			switch ( format ) {
				case AngleFormat.Unit:
					angle = input;
					break;
				case AngleFormat.Radians:
				case AngleFormat.Degrees:
					SetAngle((double) input, format);
					break;
				default:
					throw new NotImplementedException(format + " format is not supportd by this method.(unreachable code)");
			}
		}

		public override string ToString()
		{
			return GetAngle().ToString() + " " + DefaultFormat.ToString();
		}








		public static Angle operator -(Angle a)
		{
			Angle ret = new Angle(unchecked(uint.MaxValue - a.angle + 1));
			ret.DefaultFormat = a.DefaultFormat;
			return ret;
		}
		public static Angle operator +(Angle a, Angle b)
		{
			Angle ret = new Angle(a.angle + b.angle);
			ret.DefaultFormat = a.DefaultFormat;
			return ret;
		}
		public static Angle operator -(Angle a, Angle b)
		{
			Angle ret = new Angle(a.angle + -b.angle);
			ret.DefaultFormat = a.DefaultFormat;
			return ret;
		}

		public static Angle operator *(Angle a, double b)
		{
			Angle ret = new Angle(a.angle * b, AngleFormat.Unit);
			ret.DefaultFormat = a.DefaultFormat;
			return ret;
		}

		public static Angle operator *(double a, Angle b)
		{
			return b * a;
		}

		public static Angle operator /(Angle a, double b)
		{
			Angle ret = new Angle(a.angle / b, AngleFormat.Unit);
			ret.DefaultFormat = a.DefaultFormat;
			return ret;
		}
	}
}
