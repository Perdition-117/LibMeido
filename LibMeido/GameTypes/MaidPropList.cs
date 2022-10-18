namespace LibMeido;

public class MaidPropList {
	private const string FileHeader = "CM3D2_MPROP_LIST";

	public int Version { get; set; }
	private OrderedDictionary<Mpn, MaidProp> _props;

	public List<MaidProp> Props { get; set; }

	public MaidPropList(BinaryReader buffer) {
		Deserialize(buffer);

		Props = new();

		foreach (var maidProp in _props) {
			Props.Add(maidProp.Value);
			if (Version <= 110 && maidProp.Key == Mpn.EyeScl) {
				Props.Add(new MaidProp {
					Type = maidProp.Value.Type,
					Index = (int)Mpn.EyeSclX,
					Name = Mpn.EyeSclX.ToString(),
					Value = maidProp.Value.Value,
					Min = maidProp.Value.Min,
					Max = maidProp.Value.Max,
					Dut = true,
				});
				Props.Add(new MaidProp {
					Type = maidProp.Value.Type,
					Index = (int)Mpn.EyeSclY,
					Name = Mpn.EyeSclY.ToString(),
					Value = maidProp.Value.Value,
					Min = maidProp.Value.Min,
					Max = maidProp.Value.Max,
					Dut = true,
				});
			}
		}

		if (Version < 200) {
			Props.Add(MaidProp.CreateBone(Mpn.EyeClose, 0, 100, 0));
			Props.Add(MaidProp.CreateBone(Mpn.FaceShape, 0, 100, 0));
			Props.Add(MaidProp.CreateBone(Mpn.MayuX));
			Props.Add(MaidProp.CreateBone(Mpn.MayuY));
		}

		if (Version < 210) {
			Props.Add(MaidProp.CreateBone(Mpn.EyeBallPosX));
			Props.Add(MaidProp.CreateBone(Mpn.EyeBallPosY));
			Props.Add(MaidProp.CreateBone(Mpn.EyeBallSclX));
			Props.Add(MaidProp.CreateBone(Mpn.EyeBallSclY));
		}

		if (!_props.ContainsKey(Mpn.FaceShapeSlim)) {
			Props.Add(MaidProp.CreateBone(Mpn.FaceShapeSlim, 0, 100, 0));
		}
		if (!_props.ContainsKey(Mpn.EarNone)) {
			Props.Add(MaidProp.CreateBone(Mpn.EarNone, 0, 1, 0));
		}
		if (!_props.ContainsKey(Mpn.EarElf)) {
			Props.Add(MaidProp.CreateBone(Mpn.EarElf, 0, 100, 0));
		}
		if (!_props.ContainsKey(Mpn.EarRot)) {
			Props.Add(MaidProp.CreateBone(Mpn.EarRot));
		}
		if (!_props.ContainsKey(Mpn.EarScl)) {
			Props.Add(MaidProp.CreateBone(Mpn.EarScl));
		}
		if (!_props.ContainsKey(Mpn.NosePos)) {
			Props.Add(MaidProp.CreateBone(Mpn.NosePos));
		}
		if (!_props.ContainsKey(Mpn.NoseScl)) {
			Props.Add(MaidProp.CreateBone(Mpn.NoseScl));
		}
		if (!_props.ContainsKey(Mpn.MayuShapeIn)) {
			Props.Add(MaidProp.CreateBone(Mpn.MayuShapeIn));
		}
		if (!_props.ContainsKey(Mpn.MayuShapeOut)) {
			Props.Add(MaidProp.CreateBone(Mpn.MayuShapeOut));
		}
		if (!_props.ContainsKey(Mpn.MayuRot)) {
			Props.Add(MaidProp.CreateBone(Mpn.MayuRot));
		}
	}

	internal void Deserialize(BinaryReader reader) {
		var header = reader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(MaidPropList)}. Expected \"{FileHeader}\".");
		}

		Version = reader.ReadInt32();
		var numProps = reader.ReadInt32();

		_props = new();
		for (var i = 0; i < numProps; i++) {
			if (Version >= 4) {
				var text = reader.ReadString();
			}
			var maidProp = new MaidProp(reader);
			if (!_props.ContainsKey((Mpn)maidProp.Index)) {
				_props.Add((Mpn)maidProp.Index, maidProp);
			}
		}
	}

	internal void Serialize(BinaryWriter writer) {
		writer.Write(FileHeader);
		writer.Write(Version);
		writer.Write(_props.Count);

		foreach (var prop in _props) {
			if (Version >= 4) {
				writer.Write(prop.Key.ToString());
			}
			prop.Value.Serialize(writer);
		}
	}
}
