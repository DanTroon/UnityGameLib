using UnityEngine;
using UnityEngine.Networking;
using UnityGameLib.Events;
using System;
using System.Collections.Generic;

namespace UnityGameLib.Net {
	/// <summary>
	/// A wrapper that represents a web request throughout its lifespan, including any retry attempts and the final outcome.
	/// </summary>
	public class RequestWrapper {
		protected List<long> _expectedFailureCodes;

		/// <summary> The unique identifier for this wrapper. </summary>
		public string id { get; private set; }
		/// <summary> The most recent request sent. </summary>
		public UnityWebRequest request { get; private set; }
		/// <summary> A function that creates each request. </summary>
		public Func<UnityWebRequest> requestFactory { get; set; }
		/// <summary> The error message from the most recent request. </summary>
		public string errorMessage { get; set; }
		/// <summary> The number of requests attempted so far. </summary>
		public int attemptCount { get; set; }
		/// <summary> The maximum number of requests to attempt before reporting failure. </summary>
		public int maxAttempts { get; set; }
		/// <summary> The remaining delay before the next request attempt, as set by <see cref="RequestManager"/>. </summary>
		public float retryCooldown { get; set; }
		/// <summary> If true, this wrapper is ignoring <see cref="RequestManager"/>'s request queue. </summary>
		public bool bypassQueue { get; set; }
		/// <summary> Whether this wrapper is done sending requests and has reported success or failure. </summary>
		public bool isDone { get; private set; }

		private SimpleEvent _onSuccess = new SimpleEvent();
		/// <summary> Triggers when the request is completed with an acceptable response. </summary>
		public SimpleEvent onSuccess {
			get { return _onSuccess; }
		}

		private SimpleEvent _onFailure = new SimpleEvent();
		/// <summary> Triggers when the request fails all attempts or returns an expected failure code. </summary>
		public SimpleEvent onFailure {
			get { return _onFailure; }
		}

		/// <summary> Whether an error has been stored in <see cref="errorMessage"/>. </summary>
		public bool isError {
			get { return !string.IsNullOrEmpty(errorMessage); }
		}

		/// <summary> The AssetBundle downloaded by this wrapper, if applicable. </summary>
		/// <remarks> An error may occur if this wrapper did not download an AssetBundle. </remarks>
		public AssetBundle bundle {
			get {
				return ((DownloadHandlerAssetBundle) request.downloadHandler).assetBundle;
			}
		}

		/// <summary>
		/// Creates a new RequestWrapper with the specified ID and factory.
		/// </summary>
		/// <remarks>
		/// A RequestWrapper can be constructed without setting <paramref name="requestFactory"/>, but an
		/// exception will occur if you attempt to send the request without first defining a factory.
		/// </remarks>
		/// <param name="id">the unique identifier for this wrapper</param>
		/// <param name="requestFactory">the function that generates a UnityWebRequest for each attempt</param>
		public RequestWrapper(string id, Func<UnityWebRequest> requestFactory = null) {
			this.id = id;
			this.requestFactory = requestFactory;

			_expectedFailureCodes = new List<long>();

			errorMessage = "";
			attemptCount = 0;
			maxAttempts = 1;
			retryCooldown = 0f;
			isDone = false;
		}

		/// <summary>
		/// Adds an anticipated HTTP response code.
		/// If received, this request will stop retrying and immediately report failure.
		/// </summary>
		/// <param name="responseCode">the HTTP response code to add</param>
		/// <returns><c>true</c> if the code was added, or <c>false</c> if it already had been</returns>
		/// <seealso cref="RemoveFailureCode(long)"/>
		public bool AddFailureCode(long responseCode) {
			if (_expectedFailureCodes.Contains(responseCode)) {
				return false;
			}
			_expectedFailureCodes.Add(responseCode);
			return true;
		}

		/// <summary>
		/// Removes an anticipated HTTP response code.
		/// </summary>
		/// <param name="responseCode">the HTTP response code to remove</param>
		/// <returns><c>true</c> if the code was removed, or <c>false</c> if it was unused</returns>
		/// <seealso cref="AddFailureCode(long)"/>
		public bool RemoveFailureCode(long responseCode) {
			return _expectedFailureCodes.Remove(responseCode);
		}

		/// <summary>
		/// Returns whether a given HTTP response code is an expected failure response for this wrapper.
		/// </summary>
		/// <param name="responseCode">the HTTP response code</param>
		/// <returns><c>true</c> if the response code is registered, or <c>false</c> if not</returns>
		/// <seealso cref="AddFailureCode(long)"/>
		/// <seealso cref="RemoveFailureCode(long)"/>
		public bool ExpectsFailureCode(long responseCode) {
			return _expectedFailureCodes.Contains(responseCode);
		}

		/// <summary>
		/// Executes <see cref="requestFactory"/> and populates <see cref="request"/>.
		/// </summary>
		public void CreateRequest() {
			request = requestFactory();
		}

		/// <summary>
		/// Ends the request process and reports success or failure as appropriate.
		/// </summary>
		public void Complete() {
			isDone = true;

			if (isError) {
				_onFailure.Invoke();
			} else {
				_onSuccess.Invoke();
			}
			_onSuccess.RemoveAllListeners();
			_onFailure.RemoveAllListeners();
		}

		/// <summary>
		/// References the downloaded AssetBundle to ensure that scenes can be loaded from it.
		/// </summary>
		public void ReferenceAssetBundle() {
			#pragma warning disable 0219
			AssetBundle reference = bundle;
			#pragma warning restore 0219
		}
	}
}
