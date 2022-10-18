using System.Numerics;

namespace LibMeido.UnityStubs;

internal static partial class BinaryStreamExtensions {
	public static Vector2 ReadVector2(this BinaryReader reader) {
		return new() {
			X = reader.ReadSingle(),
			Y = reader.ReadSingle(),
		};
	}

	public static void Write(this BinaryWriter writer, Vector2 vector) {
		writer.Write(vector.X);
		writer.Write(vector.Y);
	}

	public static Vector3 ReadVector3(this BinaryReader reader) {
		return new() {
			X = reader.ReadSingle(),
			Y = reader.ReadSingle(),
			Z = reader.ReadSingle(),
		};
	}

	public static void Write(this BinaryWriter writer, Vector3 vector) {
		writer.Write(vector.X);
		writer.Write(vector.Y);
		writer.Write(vector.Z);
	}

	public static Vector4 ReadVector4(this BinaryReader reader) {
		return new() {
			X = reader.ReadSingle(),
			Y = reader.ReadSingle(),
			Z = reader.ReadSingle(),
			W = reader.ReadSingle(),
		};
	}

	public static void Write(this BinaryWriter writer, Vector4 vector) {
		writer.Write(vector.X);
		writer.Write(vector.Y);
		writer.Write(vector.Z);
		writer.Write(vector.W);
	}

	public static Quaternion ReadQuaternion(this BinaryReader reader) {
		return new() {
			X = reader.ReadSingle(),
			Y = reader.ReadSingle(),
			Z = reader.ReadSingle(),
			W = reader.ReadSingle(),
		};
	}

	public static void Write(this BinaryWriter writer, Quaternion quaternion) {
		writer.Write(quaternion.X);
		writer.Write(quaternion.Y);
		writer.Write(quaternion.Z);
		writer.Write(quaternion.W);
	}
}
