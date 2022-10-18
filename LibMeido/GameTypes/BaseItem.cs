using System.Text.RegularExpressions;

namespace LibMeido;

public class BaseItem {
	// parsed properties
	public int Version { get; set; }
	public string Name { get; set; }
	public string CategoryName { get; set; }
	public string Description { get; set; }
	public List<KeyValuePair<string, List<string>>> Properties { get; set; } = new();

	// computed properties
	public Mpn Mpn { get; set; }
	public Mpn ColorSetMpn { get; set; }
	public string MenuNameInColorSet { get; set; }

	public string FileName { get; set; }
	public string IconFileName { get; set; }
	public float Priority { get; set; }

	public bool IsDefaultItem() {
		return MaidProp.DefaultItems.TryGetValue(Mpn, out var item) && FileName.ToLower() == item;
	}

	public bool TryGetParentItemFileName(out string parentItemFileName) {
		parentItemFileName = null;
		if ((MpnTypeRange)Mpn is (< MpnTypeRange.WearStart or > MpnTypeRange.WearEnd) and (< MpnTypeRange.SetStart or > MpnTypeRange.SetEnd)) {
			return false;
		}

		var fileName = FileName.ToLower();

		var match = Regex.Match(fileName, @"(.+?)_z.*?([_\.].*)");
		if (match.Success) {
			parentItemFileName = match.Groups[1].Value + match.Groups[2].Value;
		}
		return match.Success;
	}
}
