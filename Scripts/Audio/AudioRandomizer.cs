using UnityGameLib.Geometry;
using UnityGameLib.Serialization;
using UnityGameLib.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameLib.Audio {
	/// <summary>
	/// Provides random variance for clip, volume, pitch, and pan when playing audio.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class AudioRandomizer : MonoBehaviour {
		[SerializeField, Tooltip("The available audio clips to randomly select. Leave empty to always use the Audio Source's clip.")]
		protected List<AssetRef> _audioRefs = new List<AssetRef>();

		[SerializeField, Tooltip("The range of volume multipliers.")]
		protected LinearRange _volumeRange = new LinearRange(1f);
		[SerializeField, Tooltip("The range of pitch multipliers.")]
		protected LinearRange _pitchRange = new LinearRange(1f);
		[SerializeField, Tooltip("The range of stereo pan values.")]
		protected LinearRange _panRange = new LinearRange(0f);
		
		/// <summary>The list of AudioClip references to use. If empty, the clip is not randomized.</summary>
		public List<AssetRef> audioRefs {
			get { return _audioRefs; }
		}

		/// <summary>The range of volume multipliers to use.</summary>
		/// <remarks>Note that AudioSource expects a value from 0 to 1.</remarks>
		public LinearRange volumeRange {
			get { return _volumeRange; }
			set { _volumeRange = value; }
		}

		/// <summary>The range of pitch/speed multipliers to use.</summary>
		/// <remarks>Note that AudioSource expects a value from -3 to 3.</remarks>
		public LinearRange pitchRange {
			get { return _pitchRange; }
			set { _pitchRange = value; }
		}

		/// <summary>The range of stereo pan values to use, from -1 (left) to 1 (right).</summary>
		public LinearRange panRange {
			get { return _panRange; }
			set { _panRange = value; }
		}

		protected AudioSource _source;
		/// <summary>The attached AudioSource.</summary>
		public AudioSource source {
			get { return _source; }
		}

		protected virtual void Awake() {
			_source = GetComponent<AudioSource>();
		}

		/// <summary>
		/// Play a sound using a random clip, volume, pitch, and/or pan as specified.
		/// </summary>
		public virtual void Play() {
			Play(1f, 1f, 1f);
		}

		/// <summary>
		/// Play a clip with an extra layer of multipliers for volume, pitch, and/or pan.
		/// </summary>
		/// <param name="volumeMultiplier">the multiplier for volume</param>
		/// <param name="pitchMultiplier">the multiplier for pitch</param>
		/// <param name="panMultiplier">the multiplier for volume</param>
		public virtual void Play(float volumeMultiplier, float pitchMultiplier = 1f, float panMultiplier = 1f) {
			RandomizeClip();
			_source.Play(volumeMultiplier * _volumeRange.GetRandom(), pitchMultiplier * _pitchRange.GetRandom(), panMultiplier * _panRange.GetRandom());
		}

		protected virtual void RandomizeClip() {
			if (_audioRefs.Count > 0) {
				_source.clip = _audioRefs.GetRandom().Load<AudioClip>();
			}
		}
	}
}
