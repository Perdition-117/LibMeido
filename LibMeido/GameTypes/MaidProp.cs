using LibMeido.MeidoStubs;
using LibMeido.UnityStubs;

namespace LibMeido;

public class MaidProp {
	private const string FileHeader = "CM3D2_MPROP";

	public int Version { get; set; }
	public int Index { get; set; }
	public string Name { get; set; }
	public MaidPropType Type { get; set; }
	public int Default { get; set; }
	public int Value { get; set; }
	public int TempValue { get; set; }
	public int ValueLinkMax { get; set; }
	public string FileName { get; set; }
	public int FileNameHash { get; set; }
	public bool Dut { get; set; }
	public int Min { get; set; }
	public int Max { get; set; }
	public List<SubProp> SubProps { get; } = new();
	public List<Bone> Bones { get; } = new();
	public List<Vertex> Vertices { get; } = new();
	public List<MaterialProp> MaterialProps { get; } = new();
	public List<BoneLength> BoneLengths { get; } = new();

	// computed properties
	public Mpn Mpn { get; set; }

	public MaidProp() { }

	public MaidProp(BinaryReader buffer) {
		Deserialize(buffer);

		//if (_Version <= 110 && _Index >= (int)MPN.kata) {
		//	Index = (MPN)_Index + 2;
		//}

		//Index = (MPN)Enum.Parse(typeof(MPN), Name, true);
		if (Enum.TryParse<Mpn>(Name, out var mpn)) {
			Index = (int)mpn;
			Mpn = mpn;
		}

		//if (!string.IsNullOrEmpty(FileName)) {
		//	FileNameRId = FileName.ToLower().GetHashCode();
		//}

		//if (_Version < 200 && Type == MaidPropType.Item && (Index is MPN.acctatoo or MPN.hokuro) && !string.IsNullOrEmpty(FileName) && !FileName.Contains("_del")) {
		//	var subProp = new SubProp {
		//		FileName = FileName,
		//		FileNameRId = FileNameRId,
		//	};
		//	_SubProps.Add(subProp);
		//	FileName = DefaultItems[Index];
		//	FileNameRId = FileName.ToLower().GetHashCode();
		//}

		//if (_Version < 2000) {
		//	if (string.IsNullOrEmpty(FileName)) {
		//		if (Index == MPN.eye_hi) {
		//			FileName = "_I_SkinHi.menu";
		//		} else if (Index == MPN.mayu) {
		//			FileName = "_I_mayu_001_mugen.menu";
		//		}
		//		FileNameRId = FileName.ToLower().GetHashCode();
		//	}
		//}

		//if (_Version <= 208 && Index == MPN.accnail && FileName.ToLower() == DefaultItems[MPN.acctatoo]) {
		//	FileName = DefaultItems[MPN.accnail];
		//	FileNameRId = FileName.ToLower().GetHashCode();
		//}

		//if (_Version < 200 && Index == MPN.hairr && Path.GetFileNameWithoutExtension(FileName.ToLower()) == "hair_r095_i_") {
		//	FileName = "hair_r110_i_.menu";
		//	FileNameRId = FileName.ToLower().GetHashCode();
		//}
	}

	public static MaidProp CreateBone(Mpn mpn, int minValue = 0, int maxValue = 100, int defaultValue = 50) {
		defaultValue = Math.Min(defaultValue, maxValue);
		defaultValue = Math.Max(defaultValue, minValue);
		var maidProp = new MaidProp {
			Name = mpn.ToString(),
			Type = MaidPropType.Bone,
			Min = minValue,
			Max = maxValue,
			Default = defaultValue,
			FileName = string.Empty,
			Index = (int)mpn,
			Dut = true,
			//TempDut = false,
		};
		var tempValue = maidProp.Value = defaultValue;
		return maidProp;
	}

	internal void Deserialize(BinaryReader reader) {
		var header = reader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(MaidProp)}. Expected \"{FileHeader}\".");
		}

		Version = reader.ReadInt32();
		Index = reader.ReadInt32();
		Name = reader.ReadString();
		Type = (MaidPropType)reader.ReadInt32();
		Default = reader.ReadInt32();
		Value = reader.ReadInt32();
		if (Version >= 101) {
			TempValue = reader.ReadInt32();
		}
		ValueLinkMax = reader.ReadInt32();
		FileName = reader.ReadString();
		FileNameHash = reader.ReadInt32();
		Dut = reader.ReadBoolean();
		Max = reader.ReadInt32();
		Min = reader.ReadInt32();

		if (Version >= 200) {
			var numSubProps = reader.ReadInt32();
			for (var i = 0; i < numSubProps; i++) {
				var flag = reader.ReadBoolean();
				if (flag) {
					var subProp = new SubProp {
						Dut = reader.ReadBoolean(),
						FileName = reader.ReadString(),
						FileNameHash = reader.ReadInt32(),
					};
					if (Version >= 211) {
						subProp.TextureOpacity = reader.ReadSingle();
					}
					SubProps.Add(subProp);
				} else {
					SubProps.Add(null);
				}
			}
		}

		if (Version >= 200) {
			var numBones = reader.ReadInt32();
			for (var i = 0; i < numBones; i++) {
				Bones.Add(new() {
					Slot = (Slot)reader.ReadInt32(),
					Key = reader.ReadInt32(),
					Enabled = reader.ReadBoolean(),
					Position = reader.ReadVector3(),
					Rotation = reader.ReadQuaternion(),
					Scale = reader.ReadVector3(),
				});
			}

			var numVertices = reader.ReadInt32();
			for (var i = 0; i < numVertices; i++) {
				var vertex = new Vertex {
					Slot = (Slot)reader.ReadInt32(),
				};
				var numSubVertices = reader.ReadInt32();
				vertex.Vertices = new();
				for (var l = 0; l < numSubVertices; l++) {
					vertex.Vertices.Add(new() {
						Slot = reader.ReadString(),
						Key = reader.ReadInt32(),
						Enabled = reader.ReadBoolean(),
						VertexCount = reader.ReadInt32(),
						Index = reader.ReadInt32(),
						Position = reader.ReadVector3(),
						Rotation = reader.ReadQuaternion(),
						Scale = reader.ReadVector3(),
					});
				}
				Vertices.Add(vertex);
			}

			var numMaterialProps = reader.ReadInt32();
			for (var i = 0; i < numMaterialProps; i++) {
				MaterialProps.Add(new() {
					Slot = (Slot)reader.ReadInt32(),
					Key = reader.ReadInt32(),
					MatNo = reader.ReadInt32(),
					PropName = reader.ReadString(),
					TypeName = reader.ReadString(),
					Value = reader.ReadString(),
				});
			}

			if (Version >= 213) {
				var numBoneLengths = reader.ReadInt32();
				for (var i = 0; i < numBoneLengths; i++) {
					var boneLength = new BoneLength {
						Slot = (Slot)reader.ReadInt32(),
						Key = reader.ReadInt32(),
						Num = reader.ReadInt32(),
						Lengths = new(),
					};
					for (var j = 0; j < boneLength.Num; j++) {
						var key = reader.ReadString();
						var value = reader.ReadSingle();
						boneLength.Lengths.Add(key, value);
					}
					BoneLengths.Add(boneLength);
				}
			}
		}
	}

	internal void Serialize(BinaryWriter writer) {
		writer.Write(FileHeader);
		writer.Write(Version);
		writer.Write(Index);
		writer.Write(Name);
		writer.Write((int)Type);
		writer.Write(Default);
		writer.Write(Value);
		if (Version >= 101) {
			writer.Write(TempValue);
		}
		writer.Write(ValueLinkMax);
		writer.Write(FileName);
		writer.Write(FileNameHash);
		writer.Write(Dut);
		writer.Write(Max);
		writer.Write(Min);

		if (Version >= 200) {
			writer.Write(SubProps.Count);
			foreach (var subProp in SubProps) {
				var hasSubProp = subProp != null;
				writer.Write(hasSubProp);
				if (hasSubProp) {
					writer.Write(subProp.Dut);
					writer.Write(subProp.FileName);
					writer.Write(subProp.FileNameHash);
					if (Version >= 211) {
						writer.Write(subProp.TextureOpacity);
					}
				}
			}
		}

		if (Version >= 200) {
			writer.Write(Bones.Count);
			foreach (var bone in Bones) {
				writer.Write((int)bone.Slot);
				writer.Write(bone.Key);
				writer.Write(bone.Enabled);

				writer.Write(bone.Position);
				writer.Write(bone.Rotation);
				writer.Write(bone.Scale);
			}

			writer.Write(Vertices.Count);
			foreach (var v in Vertices) {
				writer.Write((int)v.Slot);
				writer.Write(v.Vertices.Count);
				foreach (var v2 in v.Vertices) {
					writer.Write(v2.Slot);
					writer.Write(v2.Key);
					writer.Write(v2.Enabled);
					writer.Write(v2.VertexCount);
					writer.Write(v2.Index);

					writer.Write(v2.Position);
					writer.Write(v2.Rotation);
					writer.Write(v2.Scale);
				}
			}

			writer.Write(MaterialProps.Count);
			foreach (var materialProp in MaterialProps) {
				writer.Write((int)materialProp.Slot);
				writer.Write(materialProp.Key);
				writer.Write(materialProp.MatNo);
				writer.Write(materialProp.PropName);
				writer.Write(materialProp.TypeName);
				writer.Write(materialProp.Value);
			}

			if (Version >= 213) {
				writer.Write(BoneLengths.Count);
				foreach (var boneLength in BoneLengths) {
					writer.Write((int)boneLength.Slot);
					writer.Write(boneLength.Key);
					writer.Write(boneLength.Lengths.Count);
					foreach (var d in boneLength.Lengths) {
						writer.Write(d.Key);
						writer.Write(d.Value);
					}
				}
			}
		}
	}

	public bool IsDefaultItem() {
		return DefaultItems.TryGetValue((Mpn)Index, out var item) && FileName.ToLower() == item;
	}

	public static readonly Dictionary<Mpn, string> DefaultItems = new() {
		[Mpn.hairt] = "_i_hairt_del.menu",
		[Mpn.hairs] = "_i_hairs_del.menu",
		[Mpn.hairaho] = "_i_hairaho_del.menu",
		[Mpn.acctatoo] = "_i_acctatoo_del.menu",
		[Mpn.accnail] = "_i_accnail_del.menu",
		[Mpn.underhair] = "_i_underhair_del.menu",
		[Mpn.hokuro] = "_i_hokuro_del.menu",
		[Mpn.lip] = "_i_lip_del.menu",
		[Mpn.nose] = "nose_del_i_.menu",
		[Mpn.facegloss] = "facegloss_del_i_.menu",
		[Mpn.matsuge_low] = "_i_eyelashesunder_del.menu",
		[Mpn.futae] = "_i_eyelid_del.menu",
		[Mpn.wear] = "_i_wear_del.menu",
		[Mpn.skirt] = "_i_skirt_del.menu",
		[Mpn.mizugi] = "_i_mizugi_del.menu",
		[Mpn.bra] = "_i_bra_del.menu",
		[Mpn.panz] = "_i_panz_del.menu",
		[Mpn.stkg] = "_i_stkg_del.menu",
		[Mpn.shoes] = "_i_shoes_del.menu",
		[Mpn.headset] = "_i_headset_del.menu",
		[Mpn.glove] = "_i_glove_del.menu",
		[Mpn.acchead] = "_i_acchead_del.menu",
		[Mpn.accha] = "_i_accha_del.menu",
		[Mpn.acchana] = "_i_acchana_del.menu",
		[Mpn.acckamisub] = "_i_acckamisub_del.menu",
		[Mpn.acckami] = "_i_acckami_del.menu",
		[Mpn.accmimi] = "_i_accmimi_del.menu",
		[Mpn.accnip] = "_i_accnip_del.menu",
		[Mpn.acckubi] = "_i_acckubi_del.menu",
		[Mpn.acckubiwa] = "_i_acckubiwa_del.menu",
		[Mpn.accheso] = "_i_accheso_del.menu",
		[Mpn.accude] = "_i_accude_del.menu",
		[Mpn.accashi] = "_i_accashi_del.menu",
		[Mpn.accsenaka] = "_i_accsenaka_del.menu",
		[Mpn.accshippo] = "_i_accshippo_del.menu",
		[Mpn.accanl] = "_i_accanl_del.menu",
		[Mpn.accvag] = "_i_accvag_del.menu",
		[Mpn.megane] = "_i_megane_del.menu",
		[Mpn.accxxx] = "_i_accxxx_del.menu",
		[Mpn.handitem] = "_i_handitem_del.menu",
		[Mpn.acchat] = "_i_acchat_del.menu",
		[Mpn.onepiece] = "_i_onepiece_del.menu",
		[Mpn.folder_underhair] = "_i_underhair_folder_del.menu",
		[Mpn.folder_matsuge_low] = "_i_eyelashesunder_del_folder.menu",
		[Mpn.folder_futae] = "_i_eyelid_del_folder.menu",
		[Mpn.kousoku_upper] = "_i_kousokuu_del.menu",
		[Mpn.kousoku_lower] = "_i_kousokul_del.menu",
		[Mpn.seieki_naka] = string.Empty,
		[Mpn.seieki_hara] = string.Empty,
		[Mpn.seieki_face] = string.Empty,
		[Mpn.seieki_mune] = string.Empty,
		[Mpn.seieki_hip] = string.Empty,
		[Mpn.seieki_ude] = string.Empty,
		[Mpn.seieki_ashi] = string.Empty,
	};
}
