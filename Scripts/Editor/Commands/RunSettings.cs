using UnityEditor;

namespace UnityGameLib.Editor.Commands {
	/// <summary>
	/// Provides menu options that affect how the application runs within the Editor.
	/// </summary>
	public class RunSettings {
		private const string SIMULATE_BUNDLES_MENU_ITEM = "Run/Simulate AssetBundles";

		[MenuItem(SIMULATE_BUNDLES_MENU_ITEM, false, 61)]
		public static void ToggleSimulateBundles() {
			bool value = !EditorPrefs.GetBool("SimulateAssetBundles", false);
			EditorPrefs.SetBool("SimulateAssetBundles", value);
		}

		[MenuItem("Run/Simulate AssetBundles", true, 61)]
		public static bool ToggleSimulateBundlesValidation() {
			Menu.SetChecked(SIMULATE_BUNDLES_MENU_ITEM, EditorPrefs.GetBool("SimulateAssetBundles", false));
			return !EditorApplication.isPlayingOrWillChangePlaymode;
		}
	}
}