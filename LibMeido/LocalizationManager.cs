using AssetsTools.NET.Extra;
using CM3D2.Toolkit.Guest4168Branch.Arc;

namespace LibMeido;

static public class LocalizationManager {
	private const int MonoBehavior = 0x72;
	private const string LanguageCodeEn = "en";

	private static readonly AssetsManager AssetsManager = new();
	private static readonly Dictionary<string, Dictionary<string, string>> Terms = new();

	public static Dictionary<string, Dictionary<string, string>> GetTerms() => Terms;

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

			var languageIndices = new Dictionary<int, string>();

			var languages = baseField["mLanguages"];
			for (var i = 0; i < languages[0].childrenCount; i++) {
				languageIndices.Add(i, languages[0][i]["Code"].value.AsString());
			}

			foreach (var termData in baseField["mTerms"][0].children) {
				var term = termData["Term"].value.AsString();
				if (Terms.ContainsKey(term)) {
					Terms.Remove(term);
				}
				var translations = new Dictionary<string, string>();
				foreach (var language in languageIndices) {
					translations.Add(language.Value, termData["Languages"][0][language.Key].value.AsString());
				}
				Terms.Add(term, translations);
			}
		}
	}

	public static bool TryGetTranslation(string term, out string translation) {
		translation = string.Empty;
		return Terms.TryGetValue(term, out var translations) && translations.TryGetValue(LanguageCodeEn, out translation) && translation != string.Empty;
	}

	public static bool TryGetTranslation(string term, string languageCode, out string translation) {
		translation = string.Empty;
		return Terms.TryGetValue(term, out var translations) && translations.TryGetValue(languageCode, out translation) && translation != string.Empty;
	}
}
