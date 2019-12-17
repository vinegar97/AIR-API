using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace AIR_API
{
	public static class SteamROMHandler
	{
		private static string GetSteamInstallationPath()
		{
			RegistryKey hKey;
			string valueName = "SteamPath";
			string keyPath = @"HKEY_CURRENT_USER\Software\Valve\Steam";
			string keyName = @"Software\Valve\Steam";

			if (Registry.GetValue(keyPath, valueName, null) == null)
			{
				return "";
			}
			else
			{
				hKey = Registry.CurrentUser.OpenSubKey(keyName, false);
				return hKey.GetValue(valueName, "").ToString();
			}
		}

		private static string GetSteamBaseInstallFolder(string steamConfigFilename)
		{
			if (File.Exists(steamConfigFilename))
			{
				using (StreamReader sr = new StreamReader(steamConfigFilename, Encoding.Default))
				{
					while (sr.EndOfStream == false)
					{
						string line = sr.ReadLine();
						if (line.Contains("BaseInstallFolder"))
						{
							foreach (Match match in Regex.Matches(line, "\"([^\"]*)\""))
							{
								if (!match.ToString().Contains("BaseInstallFolder"))
								{
									return match.ToString();
								}
							}
						}
					}

				} 
			}
			return "";
		}

		public static string TryGetSteamRomPath()
		{
			Console.WriteLine("Trying to find S3&K Steam ROM");

			string steamPath = GetSteamInstallationPath();
			if (steamPath != "")
			{
				Console.WriteLine("Steam installation found: " + steamPath);

				List<string> searchPaths = new List<string>();
				searchPaths.Add(steamPath);

				string baseInstallFolder = GetSteamBaseInstallFolder(steamPath + "/config/config.vdf");
				if (baseInstallFolder != "") searchPaths.Add(baseInstallFolder);

				foreach (string searchPath in searchPaths)
				{
					string romFilename = searchPath + "\\steamapps\\common\\Sega Classics\\uncompressed ROMs\\Sonic_Knuckles_wSonic3.bin";
					Console.WriteLine("Searching ROM at location: " + romFilename);

					if (File.Exists(romFilename))
					{
						Console.WriteLine("Success!");
						return romFilename;
					}

					Console.WriteLine("Not found");
				}
			}
			return "";
		}
	}
}
