using UnityGameLib.Attributes;
using UnityGameLib.Utilities;
using UnityEngine;

namespace UnityGameLib.Serialization {
	/// <summary>
	/// A serializable entity that refers to an asset from Resources or AssetBundles.
	/// </summary>
	/// <remarks>Utility methods are included to retrieve the asset as a known type.</remarks>
	[System.Serializable]
	public struct AssetRef {
		/// <summary>The name or path for the asset.</summary>
		[Required, Tooltip("The name or path for the asset.")]
		public string resourcePath;

		/// <summary>The name of the AssetBundle containing the asset. Leave empty for assets in Resources.</summary>
		[Optional, Tooltip("The name of the AssetBundle containing the asset. Leave empty for assets in Resources.")]
		public string bundleName;

		/// <summary>
		/// Returns <c>true</c> if <see cref="resourcePath"/> is empty, meaning this AssetRef does not point to anything.
		/// </summary>
		public bool isEmpty {
			get { return string.IsNullOrEmpty(resourcePath); }
		}

		/// <summary>
		/// Creates a new AssetRef.
		/// </summary>
		/// <param name="resourcePath">The name or path for the asset</param>
		/// <param name="bundleName">The name of the AssetBundle containing the asset, or null to use Resources.</param>
		public AssetRef(string resourcePath, string bundleName = null) {
			this.resourcePath = resourcePath;
			this.bundleName = bundleName;
		}

		/// <summary>
		/// Synchronously loads the referenced asset.
		/// </summary>
		/// <typeparam name="T">The asset type</typeparam>
		/// <returns>The requested asset if it exists, or null otherwise</returns>
		public T Load<T>() where T : Object {
			return AssetUtilities.LoadAsset<T>(resourcePath, bundleName);
		}

		/// <summary>
		/// Asynchronously loads the referenced asset.
		/// </summary>
		/// <typeparam name="T">The asset type</typeparam>
		/// <returns>An AssetRequest containing the load operation if the asset exists, or null otherwise</returns>
		public AssetRequest<T> LoadAsync<T>() where T : Object {
			return AssetUtilities.LoadAssetAsync<T>(resourcePath, bundleName);
		}

		/// <summary>
		/// Instantiates the referenced prefab.
		/// </summary>
		/// <returns>An instance of the prefab</returns>
		public GameObject Instantiate() {
			return AssetUtilities.InstantiatePrefab(resourcePath, bundleName);
		}

		/// <summary>
		/// Instantiates the referenced prefab and returns a component of the instance.
		/// </summary>
		/// <typeparam name="T">The component type to return</typeparam>
		/// <returns>The component of type T attached to the instance or its children</returns>
		public T Instantiate<T>() where T : Component {
			return AssetUtilities.InstantiatePrefab<T>(resourcePath, bundleName);
		}
	}
}
