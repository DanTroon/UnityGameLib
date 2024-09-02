using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityGameLib.Utilities {
	/// <summary>
	/// Provides static utility methods for platform management, both in editor and at runtime.
	/// </summary>
	public class PlatformUtilities {
		public const string PLATFORM_NAME_ANDROID = "Android";
		public const string PLATFORM_NAME_IOS = "iOS";
		public const string PLATFORM_NAME_WEBGL = "WebGL";
		public const string PLATFORM_NAME_WINDOWS = "Windows";
		public const string PLATFORM_NAME_MAC_OSX = "OSX";
		public const string PLATFORM_NAME_LINUX = "Linux";

		/// <summary>
		/// Returns the name of the current platform as a readable string usable in filenames.
		/// In the Unity Editor, the result reflects the current build target.
		/// At runtime, the result reflects the platform of the running application.
		/// </summary>
		/// <returns>The name of the active build target or runtime platform</returns>
		public static string GetPlatformName() {
			#if UNITY_EDITOR
			return GetPlatformNameEditor(EditorUserBuildSettings.activeBuildTarget);
			#else
			return GetPlatformNameRuntime(Application.platform);
			#endif
		}

		private static string GetPlatformNameRuntime(RuntimePlatform platform) {
			switch (platform) {
				case RuntimePlatform.Android:
					return PLATFORM_NAME_ANDROID;
				case RuntimePlatform.IPhonePlayer:
					return PLATFORM_NAME_IOS;
				case RuntimePlatform.WebGLPlayer:
					return PLATFORM_NAME_WEBGL;
				case RuntimePlatform.WindowsPlayer:
					return PLATFORM_NAME_WINDOWS;
				case RuntimePlatform.OSXPlayer:
					return PLATFORM_NAME_MAC_OSX;
				case RuntimePlatform.LinuxPlayer:
					return PLATFORM_NAME_LINUX;
				default:
					return null;
			}
		}

		#if UNITY_EDITOR
		private static string GetPlatformNameEditor(BuildTarget target) {
			switch (target) {
				case BuildTarget.Android:
					return PLATFORM_NAME_ANDROID;
				case BuildTarget.iOS:
					return PLATFORM_NAME_IOS;
				case BuildTarget.WebGL:
					return PLATFORM_NAME_WEBGL;
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					return PLATFORM_NAME_WINDOWS;
				case BuildTarget.StandaloneOSXIntel:
				case BuildTarget.StandaloneOSXIntel64:
				case BuildTarget.StandaloneOSX:
					return PLATFORM_NAME_MAC_OSX;
				case BuildTarget.StandaloneLinuxUniversal:
				case BuildTarget.StandaloneLinux:
				case BuildTarget.StandaloneLinux64:
					return PLATFORM_NAME_LINUX;
				default:
					return null;
			}
		}

		/// <summary>
		/// EDITOR ONLY. Returns the BuildTargetGroup associated with the currently active BuildTarget.
		/// </summary>
		/// <returns>The active BuildTargetGroup in the Editor</returns>
		public static BuildTargetGroup GetPlatformGroup() {
			return GetPlatformGroup(EditorUserBuildSettings.activeBuildTarget);
		}

		/// <summary>
		/// EDITOR ONLY. Returns the BuildTargetGroup associated with a given BuildTarget.
		/// </summary>
		/// <param name="platform">The BuildTarget</param>
		/// <returns>The BuildTargetGroup associated with <paramref name="platform"/></returns>
		public static BuildTargetGroup GetPlatformGroup(BuildTarget platform) {
			if (platform == BuildTarget.Android)
				return BuildTargetGroup.Android;

			if (platform == BuildTarget.iOS)
				return BuildTargetGroup.iOS;

			if (platform == BuildTarget.StandaloneWindows || platform == BuildTarget.StandaloneWindows64
				|| platform == BuildTarget.StandaloneOSXIntel || platform == BuildTarget.StandaloneOSXIntel64 || platform == BuildTarget.StandaloneOSX
				|| platform == BuildTarget.StandaloneLinux || platform == BuildTarget.StandaloneLinux64 || platform == BuildTarget.StandaloneLinuxUniversal)
				return BuildTargetGroup.Standalone;

			if (platform == BuildTarget.WebGL)
				return BuildTargetGroup.WebGL;

			if (platform == BuildTarget.XboxOne)
				return BuildTargetGroup.XboxOne;
			if (platform == BuildTarget.PS4)
				return BuildTargetGroup.PS4;
			if (platform == BuildTarget.WiiU)
				return BuildTargetGroup.WiiU;

			if (platform == BuildTarget.PSP2)
				return BuildTargetGroup.PSP2;

			if (platform == BuildTarget.WSAPlayer)
				return BuildTargetGroup.WSA;

			if (platform == BuildTarget.tvOS)
				return BuildTargetGroup.tvOS;

			if (platform == BuildTarget.SamsungTV)
				return BuildTargetGroup.SamsungTV;

			if (platform == BuildTarget.Tizen)
				return BuildTargetGroup.Tizen;

			return BuildTargetGroup.Unknown;
		}

		/// <summary>
		/// EDITOR ONLY. Returns the expected file extension for builds on a given platform.
		/// For platforms that output a directory, this returns an empty string.
		/// </summary>
		/// <param name="platform">The BuildTarget</param>
		/// <returns>The output file extension for <paramref name="platform"/></returns>
		public static string GetBuildFileExtension(BuildTarget platform) {
			switch (platform) {
				case BuildTarget.Android:
					return ".apk";
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					return ".exe";
				case BuildTarget.StandaloneOSX:
				case BuildTarget.StandaloneOSXIntel:
				case BuildTarget.StandaloneOSXIntel64:
					return ".app";
				case BuildTarget.StandaloneLinuxUniversal:
				case BuildTarget.StandaloneLinux:
				case BuildTarget.StandaloneLinux64:
					return ".exe";
				default:
					return "";
			}
		}
		#endif
	}
}