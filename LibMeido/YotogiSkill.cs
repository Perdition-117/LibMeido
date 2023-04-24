using NeiLib;

namespace LibMeido;

public class YotogiSkill {
	public int Id { get; set; }
	public string Name { get; set; }
	public string NameLocalized => LocalizationManager.TryGetTranslation($"YotogiSkillName/{Name}", out var translation) ? translation : Name;

	public static List<YotogiSkill> ParseList(byte[] buffer) {
		var csv = NeiConverter.ToCSVList(buffer);
		var yotogiSkills = new List<YotogiSkill>();
		for (var i = 1; i < csv.Count; i++) {
			var row = csv[i];
			yotogiSkills.Add(new() {
				Id = int.Parse(row[0]),
				Name = row[1],
			});
		}
		return yotogiSkills;
	}
}
