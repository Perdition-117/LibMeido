using System.Globalization;

namespace LibMeido;

public class MenuItem : BaseItem {
	private const string FileHeader = "CM3D2_MENU";

	private const string LineSeparatorJp = "《改行》";
	private const string Attach = "アタッチ";
	private const string AttachToBone = "ボーンにアタッチ";

	// parsed properties
	public string Path { get; set; }

	// computed properties
	public string DisplayName { get; set; }
	public string DisplayCategoryName { get; set; }
	public string DisplayDescription { get; set; }

	public List<string> Items { get; } = new();
	public List<AddItem> AddItems { get; } = new();
	public List<string> MaskItems { get; } = new();
	public List<string> RemoveItems { get; } = new();

	public ColorPart MultiColorId = ColorPart.NONE;

	public string NameLocalized => GetLocalizedString(DisplayName ?? Name, "name");
	public string DescriptionLocalized => GetLocalizedString(DisplayDescription ?? Description, "info");

	public MenuItem() {
		Version = 1553;
	}

	public MenuItem(Stream stream, string fileName) {
		using var reader = new BinaryReader(stream);

		Deserialize(reader);

		FileName = System.IO.Path.GetFileName(fileName);
		if (Enum.TryParse<Mpn>(CategoryName, out var mpn)) {
			Mpn = mpn;
		}

		foreach (var property in Properties) {
			var values = property.Value;
			if (values.Count > 0) {
				var value = values[0];
				switch (property.Key) {
					case PropertyNames.Name:
						DisplayName = value.TrimStart('\u3000', ' ');
						break;
					case PropertyNames.Description:
						DisplayDescription = value.Replace(LineSeparatorJp, "\n");
						break;
					case PropertyNames.Category:
						DisplayCategoryName = value.ToLower();
						if (Enum.TryParse<Mpn>(DisplayCategoryName, out var mpn2)) {
							Mpn = mpn2;
						} else {
							//Console.WriteLine("There is no category." + mi.m_strCateName);
							Mpn = Mpn.null_mpn;
						}
						break;
					case PropertyNames.ColorSet:
						if (Enum.TryParse<Mpn>(value.ToLower(), out var mpn3)) {
							ColorSetMpn = mpn3;
						} else {
							//Console.WriteLine("There is no category." + mi.m_strCateName);
						}
						if (values.Count >= 2) {
							MenuNameInColorSet = values[1].ToLower();
						}
						break;
					case PropertyNames.Texture1:
					case PropertyNames.Texture2:
						if (values.Count == 5) {
							var pcMultiColorID = ColorPart.NONE;
							var colorName = values[4];
							if (!Enum.TryParse(colorName.ToUpper(), out pcMultiColorID)) {
								throw new Exception("There is no infinite color ID." + colorName);
							}
							MultiColorId = pcMultiColorID;
						}
						break;
					case PropertyNames.Icon1:
					case PropertyNames.Icon2:
						IconFileName = value;
						break;
					case PropertyNames.Item:
						Items.Add(value);
						break;
					case PropertyNames.AddItem:
						var item = new AddItem {
							FileName = values[0],
							Slot = values[1],
						};
						if (values.Count == 4 && values[2] == AttachToBone) {
							item.AttachSlot = AttachToBone;
							item.AttachName = values[3];
						} else if (values.Count == 5 && values[2] == Attach) {
							item.AttachSlot = values[3];
							item.AttachName = values[4];
						}
						AddItems.Add(item);
						break;
					case PropertyNames.RemoveItem:
						RemoveItems.Add(value);
						break;
					case PropertyNames.MaskItem:
						MaskItems.Add(value);
						break;
					case PropertyNames.Priority:
						Priority = float.Parse(value, CultureInfo.InvariantCulture);
						break;
				}
			}
		}
	}

	public MenuItem(string filename) : this(File.Open(filename, FileMode.Open), filename) { }

	private string GetLocalizedString(string source, string category) {
		var fileName = System.IO.Path.GetFileNameWithoutExtension(FileName).ToLower();
		var text = LocalizationManager.TryGetTranslation($"{DisplayCategoryName}/{fileName}|{category}", out var translation) ? translation.Replace(LineSeparatorJp, "\n") : source;
		return CountryReplace(text);
	}

	private static readonly Dictionary<string, string> array = new() {
		["ゴスロリ"] = "ピュアゴス",
		["ロリゴス"] = "プリティゴス",
		["ロリータ"] = "プリティ",
	};

	private static string CountryReplace(string text) {
		foreach (var array3 in array) {
			text = text.Replace(array3.Key, array3.Value);
		}
		return text;
	}

	//public void SortColorItem() {
	//	ColorSet.Sort((x, y) => {
	//		if (x.Priority == y.Priority) {
	//			return 0;
	//		}
	//		if (x.Priority < y.Priority) {
	//			return -1;
	//		}
	//		if (x.Priority > y.Priority) {
	//			return 1;
	//		}
	//		return 0;
	//	});
	//}

	internal void Deserialize(BinaryReader reader) {
		var header = reader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(MenuItem)}. Expected \"{FileHeader}\".");
		}

		Version = reader.ReadInt32();
		Path = reader.ReadString();
		Name = reader.ReadString();
		CategoryName = reader.ReadString();
		Description = reader.ReadString();
		var propertiesLength = reader.ReadInt32();

		while (true) {
			var numStrings = reader.ReadByte();
			if (numStrings == 0) {
				break;
			}
			var property = new List<string>();
			for (var i = 0; i < numStrings; i++) {
				property.Add(reader.ReadString());
			}
			var key = property[0];
			if (key == "end") {
				break;
			}
			property.RemoveAt(0);
			Properties.Add(new(key, property));
		}
	}

	internal void Serialize(BinaryWriter writer) {
		writer.Write(FileHeader);
		writer.Write(Version);

		writer.Write(Path);
		writer.Write(Name);
		writer.Write(CategoryName);
		writer.Write(Description);

		using var memoryStream = new MemoryStream();
		using var binaryWriter = new BinaryWriter(memoryStream);

		foreach (var property in Properties) {
			binaryWriter.Write((byte)(property.Value.Count + 1));
			binaryWriter.Write(property.Key);
			foreach (var property2 in property.Value) {
				binaryWriter.Write(property2);
			}
		}
		if (!Properties.Exists(e => e.Key == "end")) {
			binaryWriter.Write((byte)0);
		}

		writer.Write((int)memoryStream.Length);
		memoryStream.WriteTo(writer.BaseStream);
	}

	public void Write(string path) {
		using var fileStream = new FileStream(path, FileMode.Create);
		using var binaryWriter = new BinaryWriter(fileStream);
		Serialize(binaryWriter);
	}

	public class AddItem {
		public string FileName { get; set; }
		public string Slot { get; set; }
		public string AttachSlot { get; set; }
		public string AttachName { get; set; }
	}

	private static class PropertyNames {
		public const string Name = "name";
		public const string Description = "setumei";
		public const string Category = "category";
		public const string Icon1 = "icon";
		public const string Icon2 = "icons";
		public const string Item = "アイテム";
		public const string ColorSet = "color_set";
		public const string Texture1 = "tex";
		public const string Texture2 = "テクスチャ変更";
		public const string AddItem = "additem";
		public const string RemoveItem = "delitem";
		public const string MaskItem = "maskitem";
		public const string Priority = "priority";
	}
}
