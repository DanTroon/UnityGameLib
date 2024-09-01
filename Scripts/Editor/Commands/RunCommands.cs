using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityGameLib.Editor.Commands {
	/// <summary>
	/// Provides menu items for running and testing in the Editor.
	/// </summary>
	[InitializeOnLoad]
	public class RunCommands {
		private const string MENU_ITEM_EXECUTE = "Run/Run Start Scene";
		
		private static SceneSetup[] _savedSceneSetup;
	
		static RunCommands() {
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		[MenuItem(MENU_ITEM_EXECUTE, false, 50)]
		public static void RunStartScene() {
			if (EditorBuildSettings.scenes.Length == 0) {
				Debug.LogError("Cannot run from start scene: No scenes have been added to build settings.");
				return;
			}

			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
				SaveSceneSetup();

				EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path, OpenSceneMode.Single);
				EditorApplication.isPlaying = true;
			}
		}
		[MenuItem(MENU_ITEM_EXECUTE, true, 50)]
		public static bool RunStartSceneValidator() {
			return !EditorApplication.isPlayingOrWillChangePlaymode;
		}
		
		public static void SaveSceneSetup() {
			_savedSceneSetup = EditorSceneManager.GetSceneManagerSetup();
			if (_savedSceneSetup != null) {
				EditorPrefs.SetInt("SceneSetup_Count", _savedSceneSetup.Length);

				for (int i = 0; i < _savedSceneSetup.Length; ++i) {
					//Debug.Log("Saving scene setup: " + _savedSceneSetup[i].path + " LOADED=" + _savedSceneSetup[i].isLoaded + ", ACTIVE=" + _savedSceneSetup[i].isActive);
					string prefix = "SceneSetup_" + i + "_";
					EditorPrefs.SetString(prefix + "Path", _savedSceneSetup[i].path);
					EditorPrefs.SetBool(prefix + "Loaded", _savedSceneSetup[i].isLoaded);
					EditorPrefs.SetBool(prefix + "Active", _savedSceneSetup[i].isActive);
				}
			}
		}

		public static void LoadSceneSetup(bool canShowError = true) {
			if (!EditorPrefs.HasKey("SceneSetup_Count") || EditorPrefs.GetInt("SceneSetup_Count") == 0) {
				if (canShowError) {
					Debug.LogError("Cannot load scene setup because none is saved.");
				}
			} else {
				int count = EditorPrefs.GetInt("SceneSetup_Count");
				_savedSceneSetup = new SceneSetup[count];
				for (int i = 0; i < count; ++i) {
					string prefix = "SceneSetup_" + i + "_";
					_savedSceneSetup[i] = new SceneSetup();
					_savedSceneSetup[i].path = EditorPrefs.GetString(prefix + "Path");
					_savedSceneSetup[i].isLoaded = EditorPrefs.GetBool(prefix + "Loaded");
					_savedSceneSetup[i].isActive = EditorPrefs.GetBool(prefix + "Active");
					//Debug.Log("Loading scene setup: " + _savedSceneSetup[i].path + " LOADED=" + _savedSceneSetup[i].isLoaded + ", ACTIVE=" + _savedSceneSetup[i].isActive);
				}

				EditorSceneManager.RestoreSceneManagerSetup(_savedSceneSetup);
				ClearSavedSceneSetup();
			}
		}

		//[MenuItem("Run/Clear Saved Scene Setup")]
		public static void ClearSavedSceneSetup() {
			EditorPrefs.DeleteKey("SceneSetup_Count");
			_savedSceneSetup = null;

			//Debug.Log("Scene setup cleared.");
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange state) {
			if (state == PlayModeStateChange.EnteredEditMode) {
				OnStopPlaying();
			}
		}

		private static void OnStopPlaying() {
			LoadSceneSetup(false);
		}
	}
}
