using System.Numerics;

namespace LibMeido.MeidoStubs;

public class BlendData {
	public string Name { get; set; }
	public int[] Index { get; set; }
	public Vector3[] Vertices { get; set; }
	public Vector3[] Normals { get; set; }
}
