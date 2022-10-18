using Microsoft.Win32;

namespace LibMeido;

internal class LibMeido {
	private const string RegistryKeyPathEn = @"HKEY_CURRENT_USER\Software\KISS\CUSTOM ORDER MAID3D 2";
	private const string RegistryKeyPathJp = @"HKEY_CURRENT_USER\SOFTWARE\KISS\カスタムオーダーメイド3D2";
	private const string RegistryValueName = "InstallPath";

	public string GetGamePath() {
		var ExecutableName = "COM3D2.exe";
		var ExecutablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ExecutableName);

		string gamePath = null;
		if (File.Exists(ExecutablePath)) {
			gamePath = Path.GetDirectoryName(ExecutablePath);
		}
		gamePath ??= GetGamePath2(RegistryKeyPathEn);
		gamePath ??= GetGamePath2(RegistryKeyPathJp);

		if (gamePath == null) {
			Console.WriteLine("Unable to find game directory.");
		}

		return gamePath;
	}

	private string GetGamePath2(string key) {
		return (string)Registry.GetValue(key, RegistryValueName, null);
	}
}
