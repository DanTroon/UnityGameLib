using UnityEngine;

namespace UnityGameLib.Serialization {
	/// <summary>
	/// A wrapper that provides a common interface for asynchronous asset load requests from bundles or resources.
	/// </summary>
	/// <remarks>
	/// This wrapper can be used as a yield instruction for Coroutines to wait until the asset is loaded.
	/// </remarks>
	public class AssetRequest<TAsset> : CustomYieldInstruction where TAsset : Object {
		protected AsyncOperation _operation = null;
		protected TAsset _result = null;

		/// <summary>The current loading progress percentage.</summary>
		public virtual float progress {
			get {
				if (_operation != null)
					return _operation.progress;
				return _result ? 1f : 0f;
			}
		}

		/// <summary>Whether the asset is done loading.</summary>
		public virtual bool isDone {
			get {
				return _operation == null || _operation.isDone;
			}
		}

		/// <summary>The loaded asset, available after loading is done.</summary>
		public virtual TAsset asset {
			get {
				if (!_result && _operation != null)
					_result = (TAsset) ((_operation is AssetBundleRequest) ? ((AssetBundleRequest) _operation).asset : ((ResourceRequest) _operation).asset);
				return _result;
			}
		}

		/// <summary>The inverse of <see cref="isDone"/> (for use as a yield instruction).</summary>
		public override bool keepWaiting {
			get { return !isDone; }
		}

		/// <summary>Creates an AssetRequest from a loading operation.</summary>
		/// <param name="operation">The operation loading the asset</param>
		public AssetRequest(AsyncOperation operation) {
			_operation = operation;

			#if UNITY_EDITOR
			if (_operation == null) {
				Debug.LogError("AsyncOperation passed to AssetRequest is null.");
			} else if (!((_operation is AssetBundleRequest) || (_operation is ResourceRequest))) {
				Debug.LogError("AsyncOperation passed to AssetRequest is not an asset load operation.");
			}
			#endif
		}

		/// <summary>Creates an AssetRequest that provides a preloaded asset.</summary>
		/// <param name="asset">The loaded asset</param>
		public AssetRequest(TAsset asset) {
			_result = asset;

			#if UNITY_EDITOR
			if (asset == null) {
				Debug.LogError("Asset passed to AssetRequest is null.");
			}
			#endif
		}
	}
}
