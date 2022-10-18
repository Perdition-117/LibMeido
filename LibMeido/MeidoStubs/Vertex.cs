using System.Numerics;

namespace LibMeido.MeidoStubs;

public class Vertex {
	public Slot Slot { get; set; }
	public List<SubVertex> Vertices { get; set; }
}

public class SubVertex {
	public string Slot { get; set; }
	public int Key { get; set; }
	public bool Enabled { get; set; }
	public int VertexCount { get; set; }
	public int Index { get; set; }
	public Vector3 Position { get; set; }
	public Quaternion Rotation { get; set; }
	public Vector3 Scale { get; set; }
}
