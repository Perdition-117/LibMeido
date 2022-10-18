using System.Numerics;

namespace LibMeido.MeidoStubs;

public class Bone {
	public Slot Slot { get; set; }
	public int Key { get; set; }
	public bool Enabled { get; set; }
	public Vector3 Position { get; set; }
	public Quaternion Rotation { get; set; }
	public Vector3 Scale { get; set; }
}
