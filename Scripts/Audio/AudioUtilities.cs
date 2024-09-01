using UnityEngine;

namespace UnityGameLib.Audio {
	/// <summary>
	/// Provides utility and extension methods for handling audio.
	/// </summary>
	public static class AudioUtilities {
		#region Extension Methods
		/// <summary>
		/// Play the current AudioClip with modifiers.
		/// </summary>
		/// <param name="source">the AudioSource to play from</param>
		/// <param name="volume">the volume multiplier (expects 0 to 1), or NaN to leave unchanged</param>
		/// <param name="pitch">the pitch multiplier (expects -3 to 3), or NaN to leave unchanged</param>
		/// <param name="pan">the stereo pan value (-1 is left, 1 is right), or NaN to leave unchanged</param>
		public static void Play(this AudioSource source, float volume, float pitch = float.NaN, float pan = float.NaN) {
			Play(source, null, volume, pitch, pan);
		}

		/// <summary>
		/// Play a specified AudioClip with modifiers.
		/// </summary>
		/// <param name="source">the AudioSource to play from</param>
		/// <param name="clip">the AudioClip to play, or null to leave unchanged</param>
		/// <param name="volume">the volume multiplier (expects 0 to 1), or NaN to leave unchanged</param>
		/// <param name="pitch">the pitch multiplier (expects -3 to 3), or NaN to leave unchanged</param>
		/// <param name="pan">the stereo pan value (-1 is left, 1 is right), or NaN to leave unchanged</param>
		public static void Play(this AudioSource source, AudioClip clip, float volume = float.NaN, float pitch = float.NaN, float pan = float.NaN) {
			SetAudioSourceProperties(source, clip, volume, pitch, pan);
			source.Play();
		}

		/// <summary>
		/// Play the current AudioClip with modifiers and a delay.
		/// </summary>
		/// <param name="source">the AudioSource to play from</param>
		/// <param name="delay">the delay in seconds</param>
		/// <param name="volume">the volume multiplier (expects 0 to 1), or NaN to leave unchanged</param>
		/// <param name="pitch">the pitch multiplier (expects -3 to 3), or NaN to leave unchanged</param>
		/// <param name="pan">the stereo pan value (-1 is left, 1 is right), or NaN to leave unchanged</param>
		public static void PlayDelayed(this AudioSource source, float delay, float volume, float pitch = float.NaN, float pan = float.NaN) {
			PlayDelayed(source, delay, null, volume, pitch, pan);
		}

		/// <summary>
		/// Play a specified AudioClip with modifiers and a delay.
		/// </summary>
		/// <param name="source">the AudioSource to play from</param>
		/// <param name="delay">the delay in seconds</param>
		/// <param name="clip">the AudioClip to play, or null to leave unchanged</param>
		/// <param name="volume">the volume multiplier (expects 0 to 1), or NaN to leave unchanged</param>
		/// <param name="pitch">the pitch multiplier (expects -3 to 3), or NaN to leave unchanged</param>
		/// <param name="pan">the stereo pan value (-1 is left, 1 is right), or NaN to leave unchanged</param>
		public static void PlayDelayed(this AudioSource source, float delay, AudioClip clip, float volume = float.NaN, float pitch = float.NaN, float pan = float.NaN) {
			SetAudioSourceProperties(source, clip, volume, pitch, pan);
			source.PlayDelayed(delay);
		}

		private static void SetAudioSourceProperties(AudioSource source, AudioClip clip, float volume, float pitch, float pan) {
			if (clip != null)
				source.clip = clip;
			if (!float.IsNaN(volume))
				source.volume = volume;
			if (!float.IsNaN(pitch))
				source.pitch = pitch;
			if (!float.IsNaN(pan))
				source.panStereo = pan;
		}
		#endregion
	}
}
