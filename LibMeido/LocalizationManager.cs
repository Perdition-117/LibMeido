using AssetsTools.NET.Extra;
using CM3D2.Toolkit.Guest4168Branch.Arc;

namespace LibMeido;

static public class LocalizationManager {
	private const int MonoBehavior = 0x72;
	private const string LanguageCodeEn = "en";

	private static readonly AssetsManager AssetsManager = new();
	private static readonly Dictionary<string, string> Terms = new();

	public static void LoadTranslations(string filePath) {
		var afs = new ArcFileSystem();
		afs.LoadArc(filePath);

		foreach (var file in afs.Files) {
			var fileName = file.Key;
			var filePointer = file.Value;
			if (Path.GetExtension(fileName) == ".asset_language") {
				LoadTranslations(filePointer.Pointer.Data, fileName);
			}
		}
	}

	public static void LoadTranslations(byte[] data, string fileName) {
		var bundleInstance = AssetsManager.LoadBundleFile(new MemoryStream(data), fileName);
		// load the first file in bundle as an assets file
		// note the first file may not be an assets file, so check
		// bunInst.file.bundleInf6.dirInf[i].name if you're not sure
		var fileInstance = AssetsManager.LoadAssetsFileFromBundle(bundleInstance, 0);

		foreach (var info in fileInstance.table.GetAssetsOfType(MonoBehavior)) {
			// load the monobehaviour data and get the first field
			var baseField = AssetsManager.GetTypeInstance(fileInstance.file, info).GetBaseField();

			var languageIndex = -1;

			var languages = baseField["mLanguages"];
			for (var i = 0; i < languages[0].childrenCount; i++) {
				if (languages[0][i]["Code"].value.AsString() == LanguageCodeEn) {
					languageIndex = i;
					break;
				}
			}

			foreach (var termData in baseField["mTerms"][0].children) {
				var term = termData["Term"].value.AsString();
				if (Terms.ContainsKey(term)) {
					Terms.Remove(term);
				}
				Terms.Add(term, termData["Languages"][0][languageIndex].value.AsString());
			}
		}
	}

	public static bool TryGetTranslation(string term, out string translation) => Terms.TryGetValue(term, out translation) && translation != string.Empty;
}
