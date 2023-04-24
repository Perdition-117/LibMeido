using NeiLib;

namespace LibMeido;

public class YotogiSkillRequirements {
	public int SkillId { get; set; }
	public string DisplayName { get; set; }
	public bool Virgin { get; set; }
	public bool Vaginal { get; set; }
	public bool Anal { get; set; }
	public bool BothHoles { get; set; }
	public int Lust { get; set; }
	public int Masochism { get; set; }
	public int Hentai { get; set; }
	public int Service { get; set; }
	public int Lovely { get; set; }
	public int Elegance { get; set; }
	public int Charm { get; set; }
	public string Personality { get; set; }
	public string MaidClass { get; set; }
	public int MaidClassLevel { get; set; }
	public string YotogiClass { get; set; }
	public int YotogiClassLevel { get; set; }
	public bool RequestMarried { get; set; }
	public bool RequestSlave { get; set; }
	public string RejectPersonalities { get; set; }

	public static List<YotogiSkillRequirements> ParseList(byte[] buffer) {
		var csvList = NeiConverter.ToCSVList(buffer);
		var yotogiReq = new List<YotogiSkillRequirements>();
		for (var i = 1; i < csvList.Count; i++) {
			var row = csvList[i];
			yotogiReq.Add(new() {
				SkillId = int.Parse(row[0]),
				DisplayName = row[1],
				Virgin = row[2] == "〇",
				Vaginal = row[3] == "〇",
				Anal = row[4] == "〇",
				BothHoles = row[5] == "〇",
				Lust = int.TryParse(row[6], out var row6) ? row6 : 0,
				Masochism = int.TryParse(row[7], out var row7) ? row7 : 0,
				Hentai = int.TryParse(row[8], out var row8) ? row8 : 0,
				Service = int.TryParse(row[9], out var row9) ? row9 : 0,
				Lovely = int.TryParse(row[10], out var row10) ? row10 : 0,
				Elegance = int.TryParse(row[11], out var row11) ? row11 : 0,
				Charm = int.TryParse(row[12], out var row12) ? row12 : 0,
				Personality = row[13],
				MaidClass = row[14],
				MaidClassLevel = int.TryParse(row[15], out var row15) ? row15 : 0,
				YotogiClass = row[16],
				YotogiClassLevel = int.TryParse(row[17], out var row17) ? row17 : 0,
				RequestMarried = row[18] == "〇",
				RequestSlave = row[19] == "〇",
				RejectPersonalities = row[20],
			});
		}
		return yotogiReq;
	}
}
