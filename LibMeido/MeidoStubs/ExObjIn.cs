using System.Numerics;
using LibMeido.UnityStubs;

namespace LibMeido.MeidoStubs;

public class ExObjIn {
	public string Name { get; set; }
	public ExObjIn Parent { get; set; }
	public List<ExObjIn> Children { get;set;} = new();
	public Vector3 PositionLocal { get; set; }
	public Quaternion RotationLocal { get; set; }
	public Vector3[] Vertices { get; set; }
	public int FaceCount { get; set; }
	public int Uid { get; set; }
	public int Depth { get; set; }
	//public Dictionary<int, List<ExFace>> Faces { get; set; } = new();
	public int ScaleCount { get; set; }
	public Vector3[] SharedNormals { get; set; }
	public Vector2[] SharedUvs { get; set; }
	public BoneWeight[] SharedBoneWeight { get; set; }
}
