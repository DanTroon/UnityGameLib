using UnityEngine;
using System.Collections.Generic;
using UnityGameLib.Utilities;
using UnityGameLib.Collections;

namespace UnityGameLib.Audio {
	/// <summary>
	/// A singleton manager for audio.
	/// </summary>
	/// <remarks>
	/// This manager supports multiple layers of looping audio for ambience and music.
	/// 
	/// Oneshot sounds play on reusable AudioSource instances that are organized as children
	/// and named after the clips they're currently playing, so you can visualize which
	/// sounds are playing at any given time within the scene structure.
	/// </remarks>
	public class SoundManager : MonoBehaviour {
		private static SoundManager _instance;
		/// <summary>The singleton instance.</summary>
		public static SoundManager instance {
			get {
				if (!_instance)
					_instance = new GameObject("SoundManager").AddComponent<SoundManager>();
				return _instance;
			}
		}

		[SerializeField] private AudioSource _musicLayer;
		[SerializeField] private AudioSource _ambienceLayer;
		[SerializeField] private Transform _oneshotMount;

		private Stack<AudioSource> _unusedSources2D;
		private List<AudioSource> _activeSources;
		private List<float> _activeTimesRemaining;

		private float _targetMusicVolume;
		private float _targetAmbienceVolume;
		private float _musicFadeDuration = 0f;
		private float _ambienceFadeDuration = 0f;

		[SerializeField, Dictionary]
		private SerializableDictionary<string, AudioClip> _idMap;

		/// <summary>A persistent AudioSource for playing music loops.</summary>
		public AudioSource musicLayer {
			get { return _musicLayer; }
		}

		/// <summary>A persistent AudioSource for playing ambient loops.</summary>
		public AudioSource ambienceLayer {
			get { return _ambienceLayer; }
		}

		protected void Awake() {
			_instance = this;
			DontDestroyOnLoad(gameObject);

			_unusedSources2D = new Stack<AudioSource>();
			_activeSources = new List<AudioSource>();
			_activeTimesRemaining = new List<float>();
			_idMap = new SerializableDictionary<string, AudioClip>();

			if (!_musicLayer) {
				GameObject child = new GameObject("Music");
				child.transform.SetParent(transform, false);
				_musicLayer = child.AddComponent<AudioSource>();
				_musicLayer.loop = true;
			}
			if (!_ambienceLayer) {
				GameObject child = new GameObject("Ambience");
				child.transform.SetParent(transform, false);
				_ambienceLayer = child.AddComponent<AudioSource>();
				_ambienceLayer.loop = true;
			}
			if (!_oneshotMount) {
				GameObject child = new GameObject("Oneshots");
				child.transform.SetParent(transform, false);
				_oneshotMount = child.transform;
			}
		}

		protected void Start() {
			_targetMusicVolume = musicLayer.volume;
			_targetAmbienceVolume = ambienceLayer.volume;
		}

		protected void Update() {
			float remaining;
			AudioSource source;

			for (int i = _activeSources.Count - 1; i >= 0; --i) {
				remaining = _activeTimesRemaining[i] - Time.deltaTime;
				if (remaining <= 0f) {
					_activeTimesRemaining.RemoveAt(i);
					source = _activeSources[i];
					_activeSources.RemoveAt(i);

					AddUnusedSource(source);
				} else {
					_activeTimesRemaining[i] = remaining;
				}
			}

			if (_musicLayer.volume != _targetMusicVolume) {
				if (Time.deltaTime < _musicFadeDuration) {
					_musicLayer.volume = Mathf.Lerp(_musicLayer.volume, _targetMusicVolume, Time.deltaTime / _musicFadeDuration);
					_musicFadeDuration -= Time.deltaTime;
				} else {
					_musicFadeDuration = 0f;
					_musicLayer.volume = _targetMusicVolume;
				}
			}
			if (ambienceLayer.volume != _targetAmbienceVolume) {
				if (Time.deltaTime < _ambienceFadeDuration) {
					_ambienceLayer.volume = Mathf.Lerp(_ambienceLayer.volume, _targetAmbienceVolume, Time.deltaTime / _ambienceFadeDuration);
					_ambienceFadeDuration -= Time.deltaTime;
				} else {
					_ambienceFadeDuration = 0f;
					_ambienceLayer.volume = _targetAmbienceVolume;
				}
			}
		}

		/// <summary>
		/// Assigns an ID to refer to an AudioClip.
		/// </summary>
		/// <remarks>
		/// Assigning an ID that has already been used will transfer it to the new clip.
		/// </remarks>
		/// <param name="id">the alias</param>
		/// <param name="resourcePath">the resource path to the AudioClip</param>
		/// <param name="bundleName">the bundle name containing the asset, if applicable</param>
		public void RegisterClip(string id, string resourcePath, string bundleName = null) {
			RegisterClip(id, AssetUtilities.LoadAsset<AudioClip>(resourcePath, bundleName));
		}

		/// <summary>
		/// Assigns an ID to refer to an AudioClip.
		/// </summary>
		/// <remarks>
		/// Assigning an ID that has already been used will transfer it to the new clip.
		/// </remarks>
		/// <param name="id">the alias</param>
		/// <param name="clip">the clip to alias</param>
		public void RegisterClip(string id, AudioClip clip) {
			_idMap.Add(id, clip);
		}

		/// <summary>
		/// Spawns a 2D AudioSource to play a oneshot AudioClip.
		/// </summary>
		/// <param name="id">the ID or resource path for the AudioClip</param>
		/// <param name="bundleName">the bundle name containing the asset, if applicable</param>
		public AudioSource PlayOneshot(string id, string bundleName = null) {
			AudioClip clip = GetAudioClip(id, bundleName);
			if (!clip)
				return null;

			AudioSource source = GetOneshotSource();
			source.gameObject.name = clip.name;
			source.clip = clip;
			source.Play();

			AddActiveSource(source, clip.length);
			return source;
		}

		/// <summary>
		/// Spawns a 2D AudioSource to play a oneshot AudioClip with a delay.
		/// </summary>
		/// <param name="id">the ID or resource path for the AudioClip</param>
		/// <param name="delay">the delay in seconds</param>
		public AudioSource PlayOneshot(string id, float delay) {
			return PlayOneshot(id, null, delay);
		}

		/// <summary>
		/// Spawns a 2D AudioSource to play a oneshot AudioClip with a delay.
		/// </summary>
		/// <param name="id">the ID or resource path for the AudioClip</param>
		/// <param name="bundleName">the bundle name containing the asset, if applicable</param>
		/// <param name="delay">the delay in seconds</param>
		public AudioSource PlayOneshot(string id, string bundleName, float delay) {
			AudioClip clip = GetAudioClip(id, bundleName);
			if (!clip)
				return null;

			AudioSource source = GetOneshotSource();
			source.gameObject.name = clip.name;
			source.clip = clip;
			source.PlayDelayed(delay);

			AddActiveSource(source, clip.length + delay);
			return source;
		}

		/// <summary>
		/// Plays a looping AudioClip on the ambience layer.
		/// </summary>
		/// <remarks>
		/// If an ambience is already playing, it is stopped immediately and replaced by
		/// the new one.
		/// </remarks>
		/// <param name="id">the ID or resource path for the AudioClip</param>
		/// <param name="bundleName">the bundle name containing the asset, if applicable</param>
		public void PlayAmbience(string id, string bundleName = null) {
			AudioClip clip = GetAudioClip(id, bundleName);
			if (!clip)
				return;

			_ambienceLayer.clip = clip;
			_ambienceLayer.Play();
		}

		/// <summary>
		/// Immediately stops the ambience layer.
		/// </summary>
		public void StopAmbience() {
			_ambienceLayer.Stop();
		}

		/// <summary>
		/// Fades the ambience layer to <paramref name="targetVolume"/> over <paramref name="fadeDuration"/> seconds.
		/// </summary>
		/// <param name="targetVolume">the final volume</param>
		/// <param name="fadeDuration">the time in seconds to finish fading</param>
		public void FadeAmbience(float targetVolume, float fadeDuration) {
			_targetAmbienceVolume = targetVolume;
			_ambienceFadeDuration = fadeDuration;

			if (fadeDuration == 0f) {
				_ambienceLayer.volume = targetVolume;
			}
		}

		/// <summary>
		/// Plays a looping AudioClip on the music layer.
		/// </summary>
		/// <remarks>
		/// If a music clip is already playing, it is stopped immediately and replaced by
		/// the new one.
		/// </remarks>
		/// <param name="id">the ID or resource path for the AudioClip</param>
		/// <param name="bundleName">the bundle name containing the asset, if applicable</param>
		public void PlayMusic(string id, string bundleName = null) {
			AudioClip clip = GetAudioClip(id, bundleName);
			if (!clip)
				return;

			_musicLayer.clip = clip;
			_musicLayer.Play();
		}

		/// <summary>
		/// Immediately stops the music layer.
		/// </summary>
		public void StopMusic() {
			_musicLayer.Stop();
		}
		
		/// <summary>
		/// Fades the music layer to <paramref name="targetVolume"/> over <paramref name="fadeDuration"/> seconds.
		/// </summary>
		/// <param name="targetVolume">the final volume</param>
		/// <param name="fadeDuration">the time in seconds to finish fading</param>
		public void FadeMusic(float targetVolume, float fadeDuration) {
			_targetMusicVolume = targetVolume;
			_musicFadeDuration = fadeDuration;

			if (fadeDuration == 0f) {
				_musicLayer.volume = targetVolume;
			}
		}

		/// <summary>
		/// Retrieves an AudioClip, either by assigned ID or by resource path.
		/// </summary>
		/// <param name="id">the ID or resource path for the AudioClip</param>
		/// <param name="bundleName">the bundle name containing the asset, if applicable</param>
		public AudioClip GetAudioClip(string id, string bundleName = null) {
			AudioClip result;
			if (string.IsNullOrEmpty(bundleName) && _idMap.TryGetValue(id, out result)) {
				return result;
			}

			result = AssetUtilities.LoadAsset<AudioClip>(id, bundleName);

			if (!result)
				Debug.LogWarningFormat("Audio ID or path does not exist: {0}", id);
			return result;
		}

		private AudioSource GetOneshotSource() {
			AudioSource result;
			if (_unusedSources2D.Count > 0) {
				result = _unusedSources2D.Pop();
			} else {
				result = new GameObject().AddComponent<AudioSource>();
				result.transform.SetParent(_oneshotMount, false);
			}

			return result;
		}

		private void AddActiveSource(AudioSource source, float duration) {
			_activeSources.Add(source);
			_activeTimesRemaining.Add(duration);
		}

		private void AddUnusedSource(AudioSource source) {
			source.gameObject.name = "(Unused)";
			source.priority = 128;
			source.volume = 1f;
			source.pitch = 1f;
			source.panStereo = 0f;
			source.spatialBlend = 0f;
			_unusedSources2D.Push(source);
		}
	}
}
