using LibMeido.UnityStubs;

namespace LibMeido.MeidoStubs;

public class BoneUse {
	public BoneUse(string name, int index, Transform bone) {
		Name = name;
		Index = index;
		Bone = bone;
	}

	public bool Use { get; set; }
	public string Name { get; set; }
	public int Index { get; set; }
	public Transform Bone { get; set; }
	public bool Delete { get; set; }
}
