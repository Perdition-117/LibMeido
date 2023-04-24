using NeiLib;

namespace LibMeido;

public class YotogiClass {
	public int Id { get; set; }
	public string Name { get; set; }
	public string NameLocalized => LocalizationManager.TryGetTranslation($"MaidStatus/夜伽クラス/{Name}", out var translation) ? translation : Name;

	public static List<YotogiClass> ParseList(byte[] buffer) {
		var csv = NeiConverter.ToCSVList(buffer);
		var yotogiClasses = new List<YotogiClass>();
		for (var i = 1; i < csv.Count; i++) {
			var row = csv[i];
			yotogiClasses.Add(new() {
				Id = int.Parse(row[0]),
				Name = row[1],
			});
		}
		return yotogiClasses;
	}
}
