using UnityGameLib.Attributes;
using UnityGameLib.Serialization;
using UnityEngine;

namespace UnityGameLib.Utilities.Components {
	/// <summary>
	/// Provides access to a <see cref="SaveCache"/> key for use in animation events or callbacks.
	/// </summary>
	/// <remarks>
	/// Attach this component to a GameObject to enable the included functions.  If you need to
	/// access multiple keys, create additional GameObjects with this component.
	/// </remarks>
	public class SaveCacheAccess : MonoBehaviour {
		[SerializeField, Required, Tooltip("The SaveCache key to access.")]
		protected string _saveKey = "";
		[SerializeField, Tooltip("Whether the key is shared across all users on a device.")]
		protected bool _shared = false;

		/// <summary>
		/// The <see cref="SaveCache"/> key to access/>.
		/// </summary>
		public virtual string saveKey {
			get { return _saveKey; }
			set { _saveKey = value; }
		}

		/// <summary>The value of the field as a float.</summary>
		public virtual float floatValue {
			get { return SaveCache.GetFloat(_saveKey, 0f, _shared); }
			set { SaveCache.SetFloat(_saveKey, value, _shared); }
		}

		/// <summary>The value of the field as an integer.</summary>
		public virtual int intValue {
			get { return SaveCache.GetInt(_saveKey, 0, _shared); }
			set { SaveCache.SetInt(_saveKey, value, _shared); }
		}

		/// <summary>The value of the field as a boolean.</summary>
		public virtual bool boolValue {
			get { return SaveCache.GetBool(_saveKey, false, _shared); }
			set { SaveCache.SetBool(_saveKey, value, _shared); }
		}

		/// <summary>The value of the field as a string.</summary>
		public virtual string stringValue {
			get { return SaveCache.GetString(_saveKey, "", _shared); }
			set { SaveCache.SetString(_saveKey, value, _shared); }
		}
	}
}
