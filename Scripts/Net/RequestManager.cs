using UnityGameLib.Attributes;
using UnityGameLib.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityGameLib.Net {
	/// <summary>
	/// A singleton that manages all web requests, with some additional support for retrieving asset bundles.
	/// </summary>
	public class RequestManager : MonoBehaviour {
		private static RequestManager _instance;
		/// <summary>
		/// The singleton instance.
		/// </summary>
		public static RequestManager instance {
			get { return _instance; }
		}
		
		[SerializeField, RuntimeLocked, Tooltip("The URL prefix used for methods that handle AssetBundles, expected to end with '/'")]
		private string _urlPrefix = "http://www.test.test/test/";
		[SerializeField, RuntimeLocked, Tooltip("If enabled, the name of the active platform is appended (with '/') to the URL prefix.")]
		private bool _appendPlatformURL = true;
		[SerializeField, Tooltip("The maximum number of retry attempts for AssetBundle requests")]
		private int _maxAttemptCount = 100;
		[SerializeField, Tooltip("Time in seconds to wait before retrying any failed request")]
		private float _attemptCooldown = 1f;
		[SerializeField, Tooltip("The maximum number of simultaneous requests to send through the queue. Non-queued requests ignore this limit.")]
		private int _maxParallelRequests = 1;

		private RequestWrapper _manifestRequest;
		private AssetBundleManifest _manifest;
		private string _platformName = "";

		private Dictionary<string, RequestWrapper> _requestLookup = new Dictionary<string, RequestWrapper>();
		private List<RequestWrapper> _waitingRequests = new List<RequestWrapper>();
		private List<RequestWrapper> _activeRequests = new List<RequestWrapper>();
		private List<RequestWrapper> _completedRequests = new List<RequestWrapper>();
		private List<RequestWrapper> _failedRequests = new List<RequestWrapper>();

		private int _activeBypassCount = 0;

		/// <summary>
		/// The URL prefix used for methods that handle AssetBundles.
		/// </summary>
		/// <seealso cref="RetrieveManifest"/>
		/// <seealso cref="QueueBundleDownload(string)"/>
		public string urlPrefix {
			get { return _urlPrefix; }
			set { _urlPrefix = value; }
		}

		/// <summary>
		/// If true, AssetBundle requests will append the active platform name (with '/') to <see cref="urlPrefix"/>.
		/// </summary>
		/// <seealso cref="urlPrefix"/>
		public bool appendPlatformURL {
			get { return _appendPlatformURL; }
			set {
				_appendPlatformURL = value;
			}
		}

		private string platformURLFragment {
			get { return _appendPlatformURL ? _platformName + "/" : ""; }
		}

		/// <summary>
		/// The downloaded AssetBundleManifest.
		/// </summary>
		public AssetBundleManifest manifest {
			get { return _manifest; }
		}

		/// <summary>
		/// The first active download request, or null if no request is active.
		/// </summary>
		/// <remarks>
		/// This is a convenience getter for cases where requests are queued one at a time.
		/// To track the state of multiple simultaneous requests, keep a reference to each
		/// wrapper when it is created.
		/// </remarks>
		public RequestWrapper activeRequest {
			get {
				if (_activeRequests.Count > 0)
					return _activeRequests[0];
				return null;
			}
		}

		/// <summary>
		/// Looks up a managed request in any state by ID.
		/// </summary>
		/// <param name="id">the request ID</param>
		/// <returns>the managed request with <paramref name="id"/>, or null if it is not found</returns>
		public RequestWrapper GetRequestWrapper(string id) {
			if (_requestLookup.ContainsKey(id)) {
				return _requestLookup[id];
			}

			return null;
		}

		/// <summary>
		/// Retrieves a downloaded AssetBundle by name.
		/// </summary>
		/// <param name="bundleName">the name of the bundle</param>
		/// <returns>the bundle, or null if it is not found</returns>
		public AssetBundle GetAssetBundle(string bundleName) {
			if (_requestLookup.ContainsKey(bundleName)) {
				return _requestLookup[bundleName].bundle;
			}

			return null;
		}

		protected void Awake() {
			_instance = this;
			DontDestroyOnLoad(gameObject);

			_platformName = PlatformUtilities.GetPlatformName();
		}

		protected void Update() {
			for (int i = _activeRequests.Count - 1; i >= 0; --i) {
				if (_activeRequests[i].request.isDone) {
					EndRequest(_activeRequests[i]);
				}
			}

			if (_activeRequests.Count < _maxParallelRequests && _waitingRequests.Count > 0) {
				RequestWrapper wrapper = _waitingRequests[0];
				if (wrapper.retryCooldown > 0f) {
					wrapper.retryCooldown -= Time.deltaTime;
				} else {
					StartRequest(wrapper);
				}
			}
		}

		private void AddRequest(RequestWrapper wrapper) {
			_requestLookup.Add(wrapper.id, wrapper);
			_waitingRequests.Add(wrapper);

			wrapper.CreateRequest();
		}

		private bool StartRequest(RequestWrapper wrapper) {
			if (!_waitingRequests.Remove(wrapper))
				return false;

			++wrapper.attemptCount;

			_activeRequests.Add(wrapper);
			wrapper.request.SendWebRequest();

			if (wrapper.bypassQueue) {
				++_activeBypassCount;
			}

			return true;
		}

		private bool EndRequest(RequestWrapper wrapper) {
			if (!_activeRequests.Remove(wrapper))
				return false;

			if (wrapper.request.isNetworkError) {
				FailRequest(wrapper, wrapper.request.error);
			} else if (wrapper.request.responseCode >= 400 || wrapper.request.responseCode < 0) {
				FailRequest(wrapper, "Response Code " + wrapper.request.responseCode);
			} else {
				SucceedRequest(wrapper);
			}

			return true;
		}

		private void FailRequest(RequestWrapper wrapper, string errorMessage) {
			if (wrapper.attemptCount < wrapper.maxAttempts && !wrapper.ExpectsFailureCode(wrapper.request.responseCode)) {
				Debug.LogWarningFormat("Request '{0}' failed attempt {1} of {2}:\n{3}", wrapper.id, wrapper.attemptCount, wrapper.maxAttempts, errorMessage);

				wrapper.retryCooldown = _attemptCooldown;
				wrapper.CreateRequest();
				_waitingRequests.Insert(0, wrapper);
				
				if (wrapper.bypassQueue) {
					StartRequest(wrapper);
				}
			} else {
				Debug.LogWarningFormat("Request '{0}' failed all attempts:\n{1}", wrapper.id, errorMessage);

				wrapper.errorMessage = errorMessage;
				_failedRequests.Add(wrapper);
				
				if (wrapper.bypassQueue) {
					--_activeBypassCount;
				}

				wrapper.Complete();
			}
		}

		private void SucceedRequest(RequestWrapper wrapper) {
			Debug.LogFormat("Request '{0}' succeeded.", wrapper.id);

			wrapper.errorMessage = "";
			_completedRequests.Add(wrapper);
			
			if (wrapper.bypassQueue) {
				--_activeBypassCount;
			}

			wrapper.Complete();
		}

		private bool RetryRequest(RequestWrapper wrapper) {
			if (_activeRequests.Contains(wrapper))
				return false;

			_failedRequests.Remove(wrapper);

			_activeRequests.Add(wrapper);
			wrapper.request.SendWebRequest();

			return true;
		}

		/// <summary>
		/// Queues a request to retrieve an AssetBundleManifest, based on <see cref="urlPrefix"/> and the active platform.
		/// </summary>
		/// <returns>a <see cref="RequestWrapper"/> that downloads the manifest</returns>
		public RequestWrapper RetrieveManifest() {
			_manifestRequest = new RequestWrapper(_platformName);
			_manifestRequest.requestFactory = () => {
				return UnityWebRequestAssetBundle.GetAssetBundle(_urlPrefix + platformURLFragment + _platformName);
			};
			_manifestRequest.maxAttempts = _maxAttemptCount;
			_manifestRequest.onSuccess.AddListener(RetrieveManifest_OnSuccess);
			_manifestRequest.onFailure.AddListener(RetrieveManifest_OnFailure);
			AddRequest(_manifestRequest);

			return _manifestRequest;
		}

		private void RetrieveManifest_OnFailure() {
			Debug.LogError("Failed to download AssetBundleManifest: " + _manifestRequest.id);
			_requestLookup.Remove(_manifestRequest.id);
			_failedRequests.Remove(_manifestRequest);
		}

		private void RetrieveManifest_OnSuccess() {
			_requestLookup.Remove(_manifestRequest.id);
			_completedRequests.Remove(_manifestRequest);

			AssetBundle bundle = _manifestRequest.bundle;
			_manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		}

		/// <summary>
		/// Queues an AssetBundle download by name and returns the request wrapper.
		/// </summary>
		/// <param name="bundleName">the name of the bundle to download</param>
		/// <returns>the <see cref="RequestWrapper"/> that downloads the bundle</returns>
		public RequestWrapper QueueBundleDownload(string bundleName) {
			if (!_manifest) {
				Debug.LogError("The AssetBundleManifest must be retrieved before downloading bundles.");
				return null;
			}

			RequestWrapper wrapper = GetRequestWrapper(bundleName);
			if (wrapper == null) {
				wrapper = new RequestWrapper(bundleName);
				wrapper.maxAttempts = _maxAttemptCount;
				wrapper.requestFactory = () => {
					return UnityWebRequestAssetBundle.GetAssetBundle(_urlPrefix + platformURLFragment + bundleName, _manifest.GetAssetBundleHash(bundleName), 0);
				};

				QueueRequest(wrapper);
			} else if (!wrapper.isDone || wrapper.isError) {
				QueueRequest(wrapper);
			}

			return wrapper;
		}

		/// <summary>
		/// Queues a request, sending it when bandwidth is available, according to the maximum parallel requests setting.
		/// </summary>
		/// <param name="wrapper">the request to queue</param>
		/// <returns><c>true</c> if the request was successfully queued, or <c>false</c> if an error occurred</returns>
		public bool QueueRequest(RequestWrapper wrapper) {
			if (wrapper.requestFactory == null) {
				Debug.LogError("Cannot queue a request wrapper without setting requestFactory.");
				return false;
			}
			
			if (CancelRequest(wrapper.id)) {
				Debug.Log("A request with ID '" + wrapper.id + "' already exists and will be replaced.");
			}

			wrapper.bypassQueue = false;
			AddRequest(wrapper);

			return true;
		}

		/// <summary>
		/// Immediately sends a request, ignoring the queue.
		/// </summary>
		/// <param name="wrapper">the request to send</param>
		/// <returns><c>true</c> if the request successfully sent, or <c>false</c> if it could not be sent for any reason</returns>
		public bool SendRequest(RequestWrapper wrapper) {
			if (wrapper.requestFactory == null) {
				Debug.LogError("Cannot send a request wrapper without setting requestFactory.");
				return false;
			}

			if (CancelRequest(wrapper.id)) {
				Debug.Log("A request with ID '" + wrapper.id + "' already exists and will be replaced.");
			}

			wrapper.bypassQueue = true;
			AddRequest(wrapper);

			if (!StartRequest(wrapper))
				return false;

			return true;
		}

		/// <summary>
		/// Aborts a request if applicable and completely removes it from this manager.
		/// </summary>
		/// <param name="wrapper">the request to cancel</param>
		/// <returns><c>true</c> if the request was previously managed, or <c>false</c> if it was not found</returns>
		public bool CancelRequest(RequestWrapper wrapper) {
			if (wrapper == null || !_requestLookup.ContainsKey(wrapper.id))
				return false;

			_waitingRequests.Remove(wrapper);
			_activeRequests.Remove(wrapper);
			_failedRequests.Remove(wrapper);
			_completedRequests.Remove(wrapper);
			
			if (wrapper.request != null) {
				wrapper.request.Abort();
			}

			_requestLookup.Remove(wrapper.id);
			return true;
		}

		/// <summary>
		/// Aborts a request if applicable and completely removes it from this manager.
		/// </summary>
		/// <param name="id">the ID of the request to cancel</param>
		/// <returns><c>true</c> if the request was previously managed, or <c>false</c> if it was not found</returns>
		public bool CancelRequest(string id) {
			RequestWrapper wrapper;
			if (_requestLookup.TryGetValue(id, out wrapper)) {
				return CancelRequest(wrapper);
			}
			return false;
		}
	}
}
