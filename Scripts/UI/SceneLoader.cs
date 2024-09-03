using UnityGameLib.Animation;
using UnityGameLib.Attributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityGameLib.UI {
	/// <summary>
	/// A TogglingElement that uses its animation as a curtain to mask scene loading.
	/// </summary>
	/// <remarks>
	/// SceneLoader is a <see cref="TogglingElement"/>. Call <see cref="LoadScene(string, Action, Action)"/>
	/// to begin the sequence. The SceneLoader will perform this sequence:
	/// - Animate on
	/// - Unload the previous scene (triggers an event when done)
	/// - Load the temporary "loading scene"
	/// - Asynchronously load the new scene (triggers an event when done)
	/// - Animate off
	/// </remarks>
	public class SceneLoader : TogglingElement {
		[Header("SceneLoader Data")]
		[SerializeField, Required, Tooltip("The name of the scene to keep active while loading.")]
		private string _loadingSceneName = "Loading";

		[Header("SceneLoader UI")]
		[SerializeField, Optional, Tooltip("Displays the loading progress as a progress bar.")]
		private FillBar _progressFill;
		[SerializeField, Optional, Tooltip("Displays the loading progress as a percentage in text.")]
		private Text _progressPercentText;
		
		private string _sceneName;
		private Action _onUnloadComplete;
		private Action _onLoadComplete;

		protected override void Awake() {
			base.Awake();
			DontDestroyOnLoad(gameObject);

			SetProgressDisplay(0f, false);
		}

		/// <summary>
		/// Begins a scene loading sequence.
		/// </summary>
		/// <param name="sceneName">The name of the scene to load</param>
		/// <param name="onUnloadComplete">A callback for when the previous scene finishes unloading</param>
		/// <param name="onLoadComplete">A callback for when the target scene finishes loading</param>
		public void LoadScene(string sceneName, Action onUnloadComplete = null, Action onLoadComplete = null) {
			_sceneName = sceneName;
			_onUnloadComplete = onUnloadComplete;
			_onLoadComplete = onLoadComplete;

			SetProgressDisplay(0f, false);
			TransitionOn();
		}

		protected override void TransitionOn_Done() {
			base.TransitionOn_Done();
			StartCoroutine("LoadSceneRoutine");
		}

		protected virtual IEnumerator LoadSceneRoutine() {
			Debug.Log("Loading scene: " + _sceneName);

			SetProgressDisplay(0f);

			AsyncOperation loadOp = SceneManager.LoadSceneAsync(_loadingSceneName);
			while (!loadOp.isDone) {
				SetProgressDisplay(.08f * loadOp.progress);
				yield return null;
			}

			SetProgressDisplay(.08f);
			loadOp = null;
			yield return null;

			loadOp = Resources.UnloadUnusedAssets();
			while (!loadOp.isDone) {
				SetProgressDisplay(.08f + .02f * loadOp.progress);
				yield return null;
			}
			
			SetProgressDisplay(.1f);
			loadOp = null;
			yield return null;

			UnloadComplete();
			yield return null;

			loadOp = GetSceneLoadOp(_sceneName);
			while (!loadOp.isDone) {
				SetProgressDisplay(.1f + .9f * loadOp.progress);
				yield return null;
			}

			SetProgressDisplay(1f);
			loadOp = null;
			yield return null;

			LoadComplete();
		}

		protected virtual void UnloadComplete() {
			Debug.Log("Previous scene unloaded.");

			if (_onUnloadComplete != null) {
				Action func = _onUnloadComplete;
				_onUnloadComplete = null;
				func();
			}
		}

		protected virtual void LoadComplete() {
			Debug.Log("Scene loaded.");

			#if UNITY_EDITOR
			if (!UnityEditor.EditorPrefs.GetBool("SimulateAssetBundles")) {
				Debug.Log("EDITOR -- Bundles are not being simulated. Replacing shaders for mobile asset bundle compatibility.");

				//Reassign all shaders by name in case they're in asset bundles
				foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
					foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>(true)) {
						foreach (Material material in renderer.materials) {
							if (material) {
								material.shader = Shader.Find(material.shader.name);
							}
						}

						renderer.SendMessage("OnAssetBundleClean", SendMessageOptions.DontRequireReceiver);
					}
				
					foreach (CanvasRenderer renderer in obj.GetComponentsInChildren<CanvasRenderer>(true)) {
						for (int i = 0, count = renderer.materialCount; i < count; ++i) {
							Material material = renderer.GetMaterial(i);
							if (material) {
								material.shader = Shader.Find(material.shader.name);
							}
						}

						renderer.SendMessage("OnAssetBundleClean", SendMessageOptions.DontRequireReceiver);
					}
				}
				if (RenderSettings.skybox) {
					RenderSettings.skybox.shader = Shader.Find(RenderSettings.skybox.shader.name);
				}
			}
			#endif

			if (_onLoadComplete != null) {
				Action func = _onLoadComplete;
				_onLoadComplete = null;
				func();
			}
		}

		protected virtual void SetProgressDisplay(float progress, bool smooth = true) {
			if (_progressFill)
				_progressFill.SetFill(progress, smooth);
			if (_progressPercentText)
				_progressPercentText.text = Mathf.RoundToInt(progress * 100f) + "%";
		}

		private AsyncOperation GetSceneLoadOp(string sceneName) {
			#if UNITY_EDITOR
			if (string.IsNullOrEmpty(SceneManager.GetSceneByName(sceneName).path) && UnityEditor.EditorPrefs.GetBool("SimulateAssetBundles")) {
				Debug.Log("EDITOR -- Scene is not included in the main build, but asset bundles are being simulated.\nSearching the project to find the scene instead...");

				string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Scene " + sceneName);
				if (guids.Length == 0) {
					throw new UnityException(string.Format("Scene '{0}' not found in assets.", sceneName));
				} else if (guids.Length > 1) {
					Debug.LogWarning(string.Format("Multiple scenes containing '{0}' exist. This may cause the wrong scene to be loaded in the Editor.", sceneName));
				}

				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
				return UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path, new LoadSceneParameters(LoadSceneMode.Single));
			}
			#endif

			return SceneManager.LoadSceneAsync(sceneName);
		}
	}
}
