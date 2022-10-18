using System.Collections;
using System.Numerics;
using LibMeido.MeidoStubs;
using LibMeido.UnityStubs;

namespace LibMeido;

public class Mesh {
	private const string FileHeader = "CM3D2_MESH";

	public int Version { get; set; }

	public string FileName { get; set; }
	public string Name { get; set; }
	public List<ExObjIn> Objects { get; } = new();
	public Vector3[] Vertices { get; set; }
	public Vector3[] Normals { get; set; }
	public Vector2[] Uv { get; set; }
	public Vector4[] Tangents { get; set; }
	public Material[] Materials { get; set; }
	public List<BlendData> BlendData { get; } = new();

	public Mesh(string path) {
		using var binaryReader = new BinaryReader(File.Open(path, FileMode.Open));

		Deserialize(binaryReader);
	}

	void Deserialize(BinaryReader reader) {
		var header = reader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(Mesh)}. Expected \"{FileHeader}\".");
		}

		Version = reader.ReadInt32();
		FileName = reader.ReadString();
		Name = reader.ReadString();

		var gameObject = new GameObject {
			Name = "_SM_" + FileName
		};

		var hashtable = new Hashtable();

		var numObjects = reader.ReadInt32();

		for (var i = 0; i < numObjects; i++) {
			var gameObject3 = new ExObjIn {
				Name = reader.ReadString(),
				ScaleCount = reader.ReadByte(),
			};
			Objects.Add(gameObject3);
		}

		for (var i = 0; i < numObjects; i++) {
			var parentIndex = reader.ReadInt32();
			//if (parentIndex >= 0) {
			//	list[i].transform.parent = list[parentIndex].transform;
			//} else {
			//	list[i].transform.parent = gameObject.transform;
			//}
		}

		for (var i = 0; i < numObjects; i++) {
			//Transform transform = list[i].transform;
			var transform = new Transform();
			transform.LocalPosition = reader.ReadVector3();
			transform.LocalRotation = reader.ReadQuaternion();
			if (Version >= 2001) {
				var flag = reader.ReadBoolean();
				if (flag) {
					transform.LocalScale = reader.ReadVector3();
				}
			}
		}

		var numVertices = reader.ReadInt32();
		var numSubMeshes = reader.ReadInt32();
		var num6 = reader.ReadInt32();

		//Transform[] bones = new Transform[num6];
		for (var i = 0; i < num6; i++) {
			var text2 = reader.ReadString();
		}

		var bindPoses = new Matrix4x4[num6];
		for (var i = 0; i < num6; i++) {
			var m = new float[16];
			for (var j = 0; j < 16; j++) {
				m[j] = reader.ReadSingle();
			}
			bindPoses[i] = new(
				m[ 0], m[ 1], m[ 2], m[ 3],
				m[ 4], m[ 5], m[ 6], m[ 7],
				m[ 8], m[ 9], m[10], m[11],
				m[12], m[13], m[14], m[15]
			);
		}

		Vertices = new Vector3[numVertices];
		Normals = new Vector3[numVertices];
		Uv = new Vector2[numVertices];
		for (var i = 0; i < numVertices; i++) {
			Vertices[i] = reader.ReadVector3();
			Normals[i] = reader.ReadVector3();
			Uv[i] = reader.ReadVector2();
		}

		var numTangents = reader.ReadInt32();
		Tangents = new Vector4[numTangents];
		if (numTangents > 0) {
			for (int i = 0; i < numTangents; i++) {
				Tangents[i] = reader.ReadVector4();
			}
		}

		var array = new BoneUse[num6];
		var boneWeights = new BoneWeight[numVertices];
		for (int i = 0; i < numVertices; i++) {
			var boneWeight = boneWeights[i];
			boneWeight.BoneIndex0 = reader.ReadUInt16();
			boneWeight.BoneIndex1 = reader.ReadUInt16();
			boneWeight.BoneIndex2 = reader.ReadUInt16();
			boneWeight.BoneIndex3 = reader.ReadUInt16();

			boneWeight.Weight0 = reader.ReadSingle();
			boneWeight.Weight1 = reader.ReadSingle();
			boneWeight.Weight2 = reader.ReadSingle();
			boneWeight.Weight3 = reader.ReadSingle();
		}

		var _nSubMeshOriTri = new int[numSubMeshes][];

		for (var i = 0; i < numSubMeshes; i++) {
			var numTriangles = reader.ReadInt32();
			var nSubMeshOriTri = new int[numTriangles];
			for (var j = 0; j < numTriangles; j++) {
				nSubMeshOriTri[j] = reader.ReadUInt16();
			}
			_nSubMeshOriTri[i] = nSubMeshOriTri;
			//mesh2.SetTriangles(nSubMeshOriTri, i);
		}

		var numMaterials = reader.ReadInt32();
		Materials = new Material[numMaterials];
		for (var i = 0; i < numMaterials; i++) {
			var material = new Material();
			material.ReadMaterial(reader);
			Materials[i] = material;
		}

		while (true) {
			var a = reader.ReadString();
			if (a == "end") {
				break;
			}
			if (a == "morph") {
				var name = reader.ReadString();
				var numBlendData = reader.ReadInt32();
				var blendData = new BlendData {
					Name = name,
					Index = new int[numBlendData],
					Vertices = new Vector3[numBlendData],
					Normals = new Vector3[numBlendData],
				};
				for (int i = 0; i < numBlendData; i++) {
					blendData.Index[i] = reader.ReadUInt16();
					blendData.Vertices[i] = reader.ReadVector3();
					blendData.Normals[i] = reader.ReadVector3();
				}
				BlendData.Add(blendData);
			}
		}

	}
}
