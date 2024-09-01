using UnityGameLib.Attributes;
using UnityGameLib.Audio;
using UnityEngine;

namespace UnityGameLib.Utilities.Components {
	/// <summary>
	/// Provides access to common audio functions for use in animation events or callbacks.
	/// </summary>
	/// <remarks>
	/// Attach this component to a GameObject to enable the included functions for
	/// other components on the same object.
	/// </remarks>
	public class AudioAccess : MonoBehaviour {
		[SerializeField, Optional, Tooltip("The asset bundle name for referenced audio assets, if applicable.")]
		protected string _bundleName = "";

		/// <summary>The asset bundle name for referenced audio assets, if applicable.</summary>
		public string bundleName {
			get { return _bundleName; }
			set { _bundleName = value; }
		}

		/// <summary>
		/// Play a sound effect.
		/// </summary>
		/// <param name="id">the ID or resource path of the audio asset</param>
		public void PlayOneshot(string id) {
			SoundManager.instance.PlayOneshot(id, bundleName);
		}

		/// <summary>
		/// Start playing an ambience loop.
		/// </summary>
		/// <param name="id">the ID or resource path of the audio asset</param>
		public void PlayAmbience(string id) {
			SoundManager.instance.PlayAmbience(id, bundleName);
		}

		/// <summary>
		/// Start playing a music loop.
		/// </summary>
		/// <param name="id">the ID or resource path of the audio asset</param>
		public void PlayMusic(string id) {
			SoundManager.instance.PlayMusic(id, bundleName);
		}

		/// <summary>
		/// Stop playing an ambience loop.
		/// </summary>
		public void StopAmbience() {
			SoundManager.instance.StopAmbience();
		}

		/// <summary>
		/// Stop playing a music loop.
		/// </summary>
		public void StopMusic() {
			SoundManager.instance.StopMusic();
		}
	}
}
