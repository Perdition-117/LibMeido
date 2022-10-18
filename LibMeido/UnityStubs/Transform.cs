using System.Numerics;

namespace LibMeido.UnityStubs;

public class Transform {
	public Transform Parent { get; set; }
	public Vector3 LocalPosition { get; set; }
	public Quaternion LocalRotation { get; set; }
	public Vector3 LocalScale { get; set; }
}
