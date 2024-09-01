using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityGameLib.Utilities;

namespace UnityGameLib.Editor.Commands {
	/// <summary>
	/// Provides menu items and commands for building the application and assets.
	/// </summary>
	public static class BuildCommands {
		public const string BUNDLE_OUTPUT_PATH = "AssetBundles";
		public const string STREAMING_ASSETS_PATH = "Assets/StreamingAssets";
		public const string FOLDER_PATH_FORMAT = "dist/{0}/{1}";
		public const string OUTPUT_PATH_FORMAT = "dist/{0}/{1}/{2}";
		public const string FILENAME = "Game";
		public const string DEBUG_DEFINE = "DEBUG_ENABLED";
		public const string LOCAL_BUNDLES_DEFINE = "USE_LOCAL_BUNDLES";
		public const string INTERNAL_DEFINE = "INTERNAL_BUILD";
		public const string ANDROID_KEYSTORE_PASS = "";

		public const BuildOptions DEV_BUILD_OPTIONS = BuildOptions.Development | BuildOptions.AllowDebugging;

		private const string MENU_BUILD = "Build/";
		private const string MENU_BUILD_DEV = MENU_BUILD + "Development Test App/";
		private const string MENU_BUILD_DEBUG = MENU_BUILD + "Debug Application/";
		private const string MENU_BUILD_RELEASE = MENU_BUILD + "Release Application/";

		private const string MENU_ITEM_DEV_LOCAL = MENU_BUILD_DEV + "Build Internal Dev with Local Bundles";
		private const string MENU_ITEM_DEV_REMOTE = MENU_BUILD_DEV + "Build Internal Dev";

		private const string MENU_ITEM_DEBUG_PUBLIC = MENU_BUILD_DEBUG + "Build Public Debug";
		private const string MENU_ITEM_DEBUG_INTERNAL = MENU_BUILD_DEBUG + "Build Internal Debug";

		private const string MENU_ITEM_RELEASE_PUBLIC = MENU_BUILD_RELEASE + "Build Public Release";

		private const string MENU_ITEM_BUILD_BUNDLES = MENU_BUILD + "Build Asset Bundles";

		public enum DistributionType {
			Release = 0,
			Debug = 1,
			Internal_Release = 2,
			Internal_Debug = Internal_Release | Debug
		}

		private static bool canBuild {
			get { return !EditorApplication.isPlayingOrWillChangePlaymode; }
		}

		[MenuItem(MENU_ITEM_BUILD_BUNDLES, false, 120)]
		public static void BuildAssetBundles() {
			string outputPath = Path.Combine(BUNDLE_OUTPUT_PATH, PlatformUtilities.GetPlatformName());
			BuildAssetBundles(outputPath);
		}
		[MenuItem(MENU_ITEM_BUILD_BUNDLES, true, 120)]
		public static bool BuildAssetBundlesValidator() { return canBuild; }


		[MenuItem(MENU_ITEM_DEBUG_PUBLIC, false, 20)]
		public static void BuildDebug() {
			ExecuteBuild(EditorUserBuildSettings.activeBuildTarget, DistributionType.Debug, FILENAME);
		}
		[MenuItem(MENU_ITEM_DEBUG_PUBLIC, true, 20)]
		public static bool BuildDebugValidator() { return canBuild; }

		[MenuItem(MENU_ITEM_RELEASE_PUBLIC, false, 20)]
		public static void BuildRelease() {
			ExecuteBuild(EditorUserBuildSettings.activeBuildTarget, DistributionType.Release, FILENAME);
		}
		[MenuItem(MENU_ITEM_RELEASE_PUBLIC, true, 20)]
		public static bool BuildReleaseValidator() { return canBuild; }

		[MenuItem(MENU_ITEM_DEBUG_INTERNAL, false, 22)]
		public static void BuildDebugInternal() {
			ExecuteBuild(EditorUserBuildSettings.activeBuildTarget, DistributionType.Internal_Debug, FILENAME);
		}
		[MenuItem(MENU_ITEM_DEBUG_INTERNAL, true, 22)]
		public static bool BuildDebugInternalValidator() { return canBuild; }
		
		[MenuItem(MENU_ITEM_DEV_REMOTE, false, 40)]
		public static void BuildDevWithRemoteBundles() {
			ExecuteBuild(EditorUserBuildSettings.activeBuildTarget, DistributionType.Internal_Debug, FILENAME, DEV_BUILD_OPTIONS, false);
		}
		[MenuItem(MENU_ITEM_DEV_REMOTE, true, 40)]
		public static bool BuildDevWithRemoteBundlesValidator() { return canBuild; }

		[MenuItem(MENU_ITEM_DEV_LOCAL, false, 41)]
		public static void BuildDevWithLocalBundles() {
			string outputPath = Path.Combine(BUNDLE_OUTPUT_PATH, PlatformUtilities.GetPlatformName());
			BuildAssetBundles(outputPath);
			
			//Generate StreamingAssets folder if needed
			if (!Directory.Exists(STREAMING_ASSETS_PATH))
				Directory.CreateDirectory(STREAMING_ASSETS_PATH);

			//Copy bundles to StreamingAssets
			DirectoryInfo outputDir = new DirectoryInfo(outputPath);
			FileInfo[] outputFiles = outputDir.GetFiles();
			List<FileInfo> copyFiles = new List<FileInfo>(outputFiles.Length);
			foreach (FileInfo file in outputFiles) {
				if (file.Extension == ".manifest")
					continue;

                string copyPath = Path.Combine(STREAMING_ASSETS_PATH, file.Name);
				FileInfo copyFile = file.CopyTo(copyPath, true);
				copyFiles.Add(copyFile);
			}
			
			ExecuteBuild(EditorUserBuildSettings.activeBuildTarget, DistributionType.Internal_Debug, FILENAME, DEV_BUILD_OPTIONS, true);

			//Remove copied bundles from StreamingAssets
			foreach (FileInfo file in copyFiles) {
				file.Delete();
			}
		}
		[MenuItem(MENU_ITEM_DEV_LOCAL, true, 41)]
		public static bool BuildDevWithLocalBundlesValidator() { return canBuild; }

		public static void BuildAssetBundles(string outputPath) {
			if (!Directory.Exists(outputPath))
				Directory.CreateDirectory(outputPath);

			BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
		}

		public static void ExecuteBuild(BuildTarget platform, DistributionType distType, string filename = "", BuildOptions options = BuildOptions.None, bool useLocalBundles = false) {
			string folderPath = string.Format(FOLDER_PATH_FORMAT, distType.ToString(), platform.ToString());
			string path = string.Format(OUTPUT_PATH_FORMAT, distType.ToString(), platform.ToString(), filename + PlatformUtilities.GetBuildFileExtension(platform));
			string originalCompilerDefines = "";

			BuildTargetGroup platformGroup = PlatformUtilities.GetPlatformGroup(platform);
			if (platformGroup == BuildTargetGroup.Unknown) {
				Debug.LogError("Unknown build target group.  Unable to set compiler flags.");
			} else {
				originalCompilerDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(platformGroup);

				if (platformGroup == BuildTargetGroup.Android) {
					PlayerSettings.Android.keystorePass = ANDROID_KEYSTORE_PASS;
					PlayerSettings.Android.keyaliasPass = ANDROID_KEYSTORE_PASS;
				}

				if (((int) distType & (int) DistributionType.Debug) != 0) {
					AddScriptingDefines(platformGroup, DEBUG_DEFINE);
				} else {
					RemoveScriptingDefines(platformGroup, DEBUG_DEFINE);
				}

				if (((int) distType & (int) DistributionType.Internal_Release) != 0) {
					AddScriptingDefines(platformGroup, INTERNAL_DEFINE);
				} else {
					RemoveScriptingDefines(platformGroup, INTERNAL_DEFINE);
				}

				if (useLocalBundles) {
					AddScriptingDefines(platformGroup, LOCAL_BUNDLES_DEFINE);
				} else {
					RemoveScriptingDefines(platformGroup, LOCAL_BUNDLES_DEFINE);
				}
			}

			Directory.CreateDirectory(folderPath);
			BuildPipeline.BuildPlayer(GetLevels(), path, platform, options);

			if (platformGroup != BuildTargetGroup.Unknown) {
				PlayerSettings.SetScriptingDefineSymbolsForGroup(platformGroup, originalCompilerDefines);
			}
		}

		public static void AddScriptingDefines(BuildTargetGroup platform, params string[] defines) {
			string originalStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
			List<string> definesList = new List<string>(originalStr.Split(';'));

			for (int i = 0, count = defines.Length; i < count; ++i) {
				if (!definesList.Contains(defines[i])) {
					definesList.Add(defines[i]);
				}
			}

			string resultStr = string.Join(";", definesList.ToArray());
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, resultStr);
		}

		public static void RemoveScriptingDefines(BuildTargetGroup platform, params string[] defines) {
			string originalStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
			List<string> definesList = new List<string>(originalStr.Split(';'));

			for (int i = 0, count = defines.Length; i < count; ++i) {
				int index = definesList.IndexOf(defines[i]);
				if (index != -1) {
					definesList.RemoveAt(index);
				}
			}

			string resultStr = string.Join(";", definesList.ToArray());
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, resultStr);
		}

		private static string[] GetLevels() {
			EditorBuildSettingsScene[] settingsScenes = EditorBuildSettings.scenes;
			List<string> result = new List<string>(settingsScenes.Length);

			for (int i = 0, count = settingsScenes.Length; i < count; ++i) {
				if (settingsScenes[i].enabled) {
					result.Add(settingsScenes[i].path);
				}
			}

			return result.ToArray();
		}
	}
}
