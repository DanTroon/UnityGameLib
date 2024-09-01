using UnityEngine;
using UnityGameLib.Net;
using UnityGameLib.Serialization;

namespace UnityGameLib.Utilities {
	/// <summary>
	/// Provides static utility methods for handling assets.
	/// </summary>
	public static class AssetUtilities {
		#if UNITY_EDITOR
		private static bool simulateBundlesInEditor {
			get { return UnityEditor.EditorPrefs.GetBool("SimulateAssetBundles"); }
		}

		private static T LoadSimulatedAssetFromBundle<T>(string assetName, string bundleName) where T : Object {
			string[] paths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
			if (paths.Length == 0) {
				Debug.LogErrorFormat("Asset bundle '{0}' does not contain an asset named '{1}'", bundleName, assetName);
				return null;
			} else if (paths.Length > 1) {
				Debug.LogWarningFormat("Asset name '{1}' in bundle '{0}' leads to multiple assets. Only the first will be used.\nEditor paths:\n{2}", bundleName, assetName, string.Join("\n", paths));
			}

			T simResult = UnityEditor.AssetDatabase.LoadMainAssetAtPath(paths[0]) as T;
			if (!simResult) {
				Debug.LogErrorFormat("Asset bundle '{0}' asset '{1}' was not found or was not a {2}.\nEditor path: {3}", bundleName, assetName, typeof(T).Name, paths[0]);
			}
			return simResult;
		}
		#endif

		#region LoadAsset
		/// <summary>
		/// Synchronously loads the asset at <paramref name="resourcePath"/>.
		/// </summary>
		/// <remarks>
		/// Specify <paramref name="bundleName"/> to retrieve the asset from an asset bundle instead of from Resources.
		/// </remarks>
		/// <typeparam name="T">The asset type</typeparam>
		/// <param name="resourcePath">The path to the prefab in Resources or in an AssetBundle</param>
		/// <param name="bundleName">The name of the AssetBundle containing the asset, or null to retrieve from Resources</param>
		/// <returns>the requested asset if it exists, or null otherwise</returns>
		public static T LoadAsset<T>(string resourcePath, string bundleName = null) where T : Object {
			if (string.IsNullOrEmpty(bundleName)) {
				return LoadAssetFromResources<T>(resourcePath);
			} else {
				return LoadAssetFromBundle<T>(resourcePath, bundleName);
			}
		}

		private static T LoadAssetFromResources<T>(string resourcePath) where T : Object {
			T result = Resources.Load<T>(resourcePath);

			if (!result) {
				Debug.LogErrorFormat("Asset does not exist at path: {0}", resourcePath);
				return null;
			}

			return result;
		}

		private static T LoadAssetFromBundle<T>(string assetName, string bundleName) where T : Object {
			#if UNITY_EDITOR
			if (simulateBundlesInEditor) {
				return LoadSimulatedAssetFromBundle<T>(assetName, bundleName);
			}
			#endif

			AssetBundle bundle = RequestManager.instance.GetAssetBundle(bundleName);
			if (!bundle) {
				Debug.LogErrorFormat("AssetBundle does not exist or is not loaded: {0}", bundleName);
				return null;
			}

			T result = bundle.LoadAsset<T>(assetName);
			if (!result) {
				Debug.LogErrorFormat("AssetBundle '{0}' does not contain asset: {1}", bundleName, assetName);
				return null;
			}

			return result;
		}
		#endregion


		#region LoadAssetAsync
		/// <summary>
		/// Asynchronously loads the asset at <paramref name="resourcePath"/>.
		/// </summary>
		/// <remarks>
		/// Note that since the asset is not loaded immediately, this method cannot identify whether a requested asset
		/// exists or not. The result is always an AssetRequest, unless <paramref name="bundleName"/> is specified and
		/// refers to an AssetBundle that does not exist, in which case the result is null.
		/// </remarks>
		/// <typeparam name="T">The asset type</typeparam>
		/// <param name="resourcePath">The path to the prefab in Resources or in an AssetBundle</param>
		/// <param name="bundleName">The name of the AssetBundle containing the asset, or null to retrieve from Resources</param>
		/// <returns>an AssetRequest that handles the asset load operation, or null if the AssetBundle does not exist</returns>
		public static AssetRequest<T> LoadAssetAsync<T>(string resourcePath, string bundleName = null) where T : Object {
			if (string.IsNullOrEmpty(bundleName)) {
				return LoadAssetAsyncFromResources<T>(resourcePath);
			} else {
				return LoadAssetAsyncFromBundle<T>(resourcePath, bundleName);
			}
		}

		private static AssetRequest<T> LoadAssetAsyncFromResources<T>(string resourcePath) where T : Object {
			ResourceRequest op = Resources.LoadAsync(resourcePath);
			return new AssetRequest<T>(op);
		}

		private static AssetRequest<T> LoadAssetAsyncFromBundle<T>(string assetName, string bundleName) where T : Object {
			#if UNITY_EDITOR
			if (simulateBundlesInEditor) {
				T simResult = LoadSimulatedAssetFromBundle<T>(assetName, bundleName);
				return new AssetRequest<T>(simResult);
			}
			#endif

			AssetBundle bundle = RequestManager.instance.GetAssetBundle(bundleName);
			if (!bundle) {
				Debug.LogErrorFormat("AssetBundle does not exist or is not loaded: {0}", bundleName);
				return null;
			}

			AssetBundleRequest op = bundle.LoadAssetAsync<T>(assetName);
			return new AssetRequest<T>(op);
		}
		#endregion


		#region InstantiatePrefab
		/// <summary>
		/// Instantiates the prefab at <paramref name="resourcePath"/>.
		/// </summary>
		/// <param name="resourcePath">The path to the prefab in Resources or in an AssetBundle</param>
		/// <param name="bundleName">The name of the AssetBundle containing the prefab, or null to retrieve from Resources</param>
		/// <returns>An instance of the prefab</returns>
		public static GameObject InstantiatePrefab(string resourcePath, string bundleName = null) {
			GameObject prefab = LoadAsset<GameObject>(resourcePath, bundleName);
			return InstantiatePrefab(prefab);
		}

		/// <summary>
		/// Instantiates a prefab.
		/// </summary>
		/// <param name="prefab">The prefab to instantiate</param>
		/// <returns>An instance of the prefab</returns>
		public static GameObject InstantiatePrefab(GameObject prefab) {
			return Object.Instantiate(prefab);
		}

		/// <summary>
		/// Instantiates the prefab at <paramref name="resourcePath"/> and returns a component of the instance.
		/// </summary>
		/// <typeparam name="T">The component type to return</typeparam>
		/// <param name="resourcePath">The path to the prefab in Resources or in an AssetBundle</param>
		/// <param name="bundleName">The name of the AssetBundle containing the prefab, or null to retrieve from Resources</param>
		/// <returns>The component of type T attached to the instance or its children</returns>
		public static T InstantiatePrefab<T>(string resourcePath, string bundleName = null) where T : Component {
			GameObject instance = InstantiatePrefab(resourcePath, bundleName);
			if (!instance)
				return null;

			T component = instance.GetComponentInChildren<T>(true);

			#if UNITY_EDITOR
			if (!component) {
				Debug.LogError("Prefab at \"" + resourcePath + "\" has no component of type " + typeof(T).Name);
				return null;
			}
			#endif

			return component;
		}

		/// <summary>
		/// Instantiates a prefab and returns a component of the instance.
		/// </summary>
		/// <typeparam name="T">The component type to return</typeparam>
		/// <param name="prefab">The prefab to instantiate</param>
		/// <returns>The component of type T attached to the instance or its children</returns>
		public static T InstantiatePrefab<T>(GameObject prefab) where T : Component {
			GameObject instance = InstantiatePrefab(prefab);
			if (!instance)
				return null;

			T component = instance.GetComponentInChildren<T>(true);

			#if UNITY_EDITOR
			if (!component) {
				Debug.LogError("Prefab has no component of type " + typeof(T).Name);
				return null;
			}
			#endif

			return component;
		}
		#endregion
	}
}
