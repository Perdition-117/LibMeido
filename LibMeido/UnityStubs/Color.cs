namespace LibMeido.UnityStubs;

public struct ColorEx {
	public float R { get; set; }
	public float G { get; set; }
	public float B { get; set; }
	public float A { get; set; }

	public static ColorEx red => new ColorEx(1f, 0f, 0f, 1f);
	public static ColorEx green => new ColorEx(0f, 1f, 0f, 1f);
	public static ColorEx blue => new ColorEx(0f, 0f, 1f, 1f);
	public static ColorEx white => new ColorEx(1f, 1f, 1f, 1f);
	public static ColorEx black => new ColorEx(0f, 0f, 0f, 1f);
	public static ColorEx yellow => new ColorEx(1f, 47f / 51f, 4f / 255f, 1f);
	public static ColorEx cyan => new ColorEx(0f, 1f, 1f, 1f);
	public static ColorEx magenta => new ColorEx(1f, 0f, 1f, 1f);
	public static ColorEx gray => new ColorEx(0.5f, 0.5f, 0.5f, 1f);
	public static ColorEx grey => new ColorEx(0.5f, 0.5f, 0.5f, 1f);
	public static ColorEx clear => new ColorEx(0f, 0f, 0f, 0f);

	public float grayscale => 0.299f * R + 0.587f * G + 0.114f * B;

	//public Color linear => new Color(Mathf.GammaToLinearSpace(r), Mathf.GammaToLinearSpace(g), Mathf.GammaToLinearSpace(b), a);

	//public Color gamma => new Color(Mathf.LinearToGammaSpace(r), Mathf.LinearToGammaSpace(g), Mathf.LinearToGammaSpace(b), a);

	public float maxColorComponent => Math.Max(Math.Max(R, G), B);

	public float this[int index] {
		get {
			return index switch {
				0 => R,
				1 => G,
				2 => B,
				3 => A,
				_ => throw new IndexOutOfRangeException("Invalid Vector3 index!"),
			};
		}
		set {
			switch (index) {
				case 0:
					R = value;
					break;
				case 1:
					G = value;
					break;
				case 2:
					B = value;
					break;
				case 3:
					A = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Vector3 index!");
			}
		}
	}

	public ColorEx(float r, float g, float b, float a) {
		R = r;
		G = g;
		B = b;
		A = a;
	}

	public ColorEx(float r, float g, float b) {
		R = r;
		G = g;
		B = b;
		A = 1f;
	}

	//public override string ToString() {
	//	return UnityString.Format("RGBA({0:F3}, {1:F3}, {2:F3}, {3:F3})", r, g, b, a);
	//}

	//public string ToString(string format) {
	//	return UnityString.Format("RGBA({0}, {1}, {2}, {3})", r.ToString(format), g.ToString(format), b.ToString(format), a.ToString(format));
	//}

	public static ColorEx operator +(ColorEx a, ColorEx b) {
		return new(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
	}

	public static ColorEx operator -(ColorEx a, ColorEx b) {
		return new(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
	}

	public static ColorEx operator *(ColorEx a, ColorEx b) {
		return new(a.R * b.R, a.G * b.G, a.B * b.B, a.A * b.A);
	}

	public static ColorEx operator *(ColorEx a, float b) {
		return new(a.R * b, a.G * b, a.B * b, a.A * b);
	}

	public static ColorEx operator *(float b, ColorEx a) {
		return new(a.R * b, a.G * b, a.B * b, a.A * b);
	}

	public static ColorEx operator /(ColorEx a, float b) {
		return new(a.R / b, a.G / b, a.B / b, a.A / b);
	}

	//public static bool operator ==(Color lhs, Color rhs) {
	//	return (Vector4)lhs == (Vector4)rhs;
	//}

	//public static bool operator !=(Color lhs, Color rhs) {
	//	return !(lhs == rhs);
	//}

	//public static Color Lerp(Color a, Color b, float t) {
	//	t = Mathf.Clamp01(t);
	//	return new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
	//}

	public static ColorEx LerpUnclamped(ColorEx a, ColorEx b, float t) {
		return new ColorEx(a.R + (b.R - a.R) * t, a.G + (b.G - a.G) * t, a.B + (b.B - a.B) * t, a.A + (b.A - a.A) * t);
	}

	internal ColorEx RGBMultiplied(float multiplier) {
		return new ColorEx(R * multiplier, G * multiplier, B * multiplier, A);
	}

	internal ColorEx AlphaMultiplied(float multiplier) {
		return new ColorEx(R, G, B, A * multiplier);
	}

	internal ColorEx RGBMultiplied(ColorEx multiplier) {
		return new ColorEx(R * multiplier.R, G * multiplier.G, B * multiplier.B, A);
	}

	//public static implicit operator Vector4(Color c) {
	//	return new Vector4(c.r, c.g, c.b, c.a);
	//}

	//public static implicit operator Color(Vector4 v) {
	//	return new Color(v.x, v.y, v.z, v.w);
	//}
}
