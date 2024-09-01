using UnityGameLib.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameLib.Audio {
	/// <summary>
	/// A component that filters frequent Play calls for <see cref="AudioRandomizer"/>s and maps them to string identifiers.
	/// Works with AnimationEvents and UnityEvent callbacks.
	/// </summary>
	public class AudioMap : MonoBehaviour {
		[SerializeField, Tooltip("The list of channel details.")]
		protected List<Channel> _channels;

		protected Dictionary<string, Channel> _channelLookup;

		protected virtual void Awake() {
			_channelLookup = new Dictionary<string, Channel>(_channels.Count);

			foreach (Channel channel in _channels) {
				#if UNITY_EDITOR
				if (_channelLookup.ContainsKey(channel.id)) {
					Debug.LogError(string.Format("Duplicate channel ID '{0}' found in AudioMap. Only the last one will be used."));
				}
				#endif
				_channelLookup[channel.id] = channel;
			}
		}

		/// <summary>
		/// Plays the audio channel with the specified <paramref name="id"/> if it exists.
		/// </summary>
		/// <param name="id">the channel ID</param>
		public virtual void PlayChannel(string id) {
			Channel channel;
			if (_channelLookup.TryGetValue(id, out channel) && channel.isReady) {
				channel.Play();
			}
		}

		/// <summary>
		/// Returns the channel with the specified <paramref name="id"/>, or <c>null</c> if none exists.
		/// </summary>
		/// <param name="id">the channel ID</param>
		/// <returns>the channel associated with <paramref name="id"/></returns>
		public Channel GetChannel(string id) {
			if (_channelLookup.ContainsKey(id)) {
				return _channelLookup[id];
			}
			return null;
		}

		[Serializable]
		public class Channel {
			[SerializeField, Required, RuntimeLocked, Tooltip("The unique string identifier for this channel.")]
			private string _id;
			[SerializeField, Required, RuntimeLocked, Tooltip("The AudioRandomizer used to play audio for this channel.")]
			private AudioRandomizer _playback;

			[SerializeField, Tooltip("An optional cooldown time between plays. Use zero for no cooldown.")]
			private float _cooldownSec = 0f;
			[SerializeField, Tooltip("If enabled, allows playing the channel again before it finishes a previous sound.  Cooldown time still applies.")]
			private bool _allowInterrupt = false;

			private float _volumeMultiplier = 1f;
			private float _pitchMultiplier = 1f;
			private float _panMultiplier = 1f;
			private float _lastPlayedTime = 0f;

			/// <summary>The unique string idenifier for this channel.</summary>
			public string id {
				get { return _id; }
			}

			/// <summary>The <see cref="AudioRandomizer"/> used to play audio for this channel.</summary>
			public AudioRandomizer playback {
				get { return _playback; }
			}

			/// <summary>The cooldown time, in seconds, before this channel is ready to play again.</summary>
			public float cooldownSec {
				get { return _cooldownSec; }
				set { _cooldownSec = value; }
			}

			/// <summary>Whether this channel can start new audio while its previous audio is still playing.</summary>
			public bool allowInterrupt {
				get { return _allowInterrupt; }
				set { _allowInterrupt = value; }
			}

			/// <summary>The volume multiplier applied when this channel plays.</summary>
			public float volumeMultiplier {
				get { return _volumeMultiplier; }
				set { _volumeMultiplier = value; }
			}

			/// <summary>The pitch multiplier applied when this channel plays.</summary>
			public float pitchMultiplier {
				get { return _pitchMultiplier; }
				set { _pitchMultiplier = value; }
			}

			/// <summary>The stereo pan multiplier applied when this channel plays.</summary>
			public float panMultiplier {
				get { return _panMultiplier; }
				set { _panMultiplier = value; }
			}

			/// <summary>
			/// Whether this channel should be allowed to play now, according
			/// to <see cref="cooldownSec"/> and <see cref="allowInterrupt"/>.
			/// </summary>
			public bool isReady {
				get {
					return Time.time - _lastPlayedTime > _cooldownSec && (_allowInterrupt || !_playback.source.isPlaying);
				}
			}

			/// <summary>
			/// Plays the channel, even if <see cref="isReady"/> is <c>false</c>.
			/// </summary>
			public void Play() {
				_lastPlayedTime = Time.time;
				playback.Play(_volumeMultiplier, _pitchMultiplier, _panMultiplier);
			}
		}
	}
}
