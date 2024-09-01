using UnityGameLib.Serialization.SimpleJSON;
using UnityGameLib.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameLib.Serialization {
	/// <summary>
	/// A static interface for optimizing reads and writes of local save data.
	/// </summary>
	/// <remarks>
	/// All Get and Set methods include an optional "shared" parameter that specifies whether the
	/// saved data should be user-specific or shared across all users on the device.
	/// 
	/// Call <see cref="InitializeUser(string)"/> before attempting to read or write user-specific
	/// data. This step is unnecessary if only shared data is used.
	/// 
	/// All values are cached in memory the first time they are accessed. Use <see cref="ClearCache"/>
	/// to flush that memory. The cache will repopulate naturally whenever values are accessed again.
	/// </remarks>
	public class SaveCache : MonoBehaviour {
		private static Dictionary<string, JSONNode> _jsonCache = new Dictionary<string, JSONNode>();
		private static Dictionary<string, string> _stringCache = new Dictionary<string, string>();
		private static Dictionary<string, float> _floatCache = new Dictionary<string, float>();
		private static Dictionary<string, int> _intCache = new Dictionary<string, int>();

		private const string SAVE_KEY_GAME_VERSION = "game_saved_version";

		private static string userID = "UNKNOWN";

		/// <summary>
		/// If enabled, all device read and write operations are logged to the console.
		/// </summary>
		public static bool logDebugMessages = false;

		private static string GetSaveKey(string userID, string key) {
			return userID + "_" + key;
		}

		private static JSONNode CoalesceSavedJSON(string saveKey, JSONNode defaultValue) {
			if (!_jsonCache.ContainsKey(saveKey)) {
				string str = PlayerPrefs.GetString(saveKey, "");
				JSONNode value;

				if (string.IsNullOrEmpty(str)) {
					str = defaultValue.ToString();
					value = JSONNode.Parse(str);
				} else {
					try {
						value = JSONNode.Parse(str);
					} catch {
						Debug.LogError("Saved JSON for '" + saveKey + "' failed to parse. Replaced with default value.");
						value = JSONNode.Parse(defaultValue.ToString());
					}
				}
				if (logDebugMessages) Debug.Log(string.Format("Retrieved saved JSON: {0} = {1}", saveKey, value.ToString("")));

				_jsonCache.Add(saveKey, value);
			}

			return _jsonCache[saveKey];
		}

		private static string CoalesceSavedString(string saveKey, string defaultValue) {
			if (!_stringCache.ContainsKey(saveKey)) {
				_stringCache.Add(saveKey, PlayerPrefs.GetString(saveKey, defaultValue));
				if (logDebugMessages) Debug.Log(string.Format("Retrieved saved string: {0} = {1}", saveKey, _stringCache[saveKey]));
			}
			return _stringCache[saveKey];
		}

		private static float CoalesceSavedFloat(string saveKey, float defaultValue) {
			if (!_floatCache.ContainsKey(saveKey)) {
				_floatCache.Add(saveKey, BitwiseUtilities.FloatFromInt32(PlayerPrefs.GetInt(saveKey, BitwiseUtilities.FloatToInt32(defaultValue))));
				if (logDebugMessages) Debug.Log(string.Format("Retrieved saved float: {0} = {1}", saveKey, _floatCache[saveKey]));
			}
			return _floatCache[saveKey];
		}

		private static int CoalesceSavedInt(string saveKey, int defaultValue) {
			if (!_intCache.ContainsKey(saveKey)) {
				_intCache.Add(saveKey, PlayerPrefs.GetInt(saveKey, defaultValue));
				if (logDebugMessages) Debug.Log(string.Format("Retrieved saved int: {0} = {1}", saveKey, _intCache[saveKey]));
			}
			return _intCache[saveKey];
		}

		/// <summary>
		/// Sets the active userID for interacting with user-specific save data.
		/// This must be called before saving or loading any user-specific entries.
		/// </summary>
		/// <param name="userID">The unique identifier representing the current user</param>
		public static void InitializeUser(string userID) {
			ClearCache();
			SaveCache.userID = userID;
		}

		/// <summary>
		/// Sets a new saved game version and outputs the game version for the existing save data.
		/// </summary>
		/// <param name="newVersion">The current game version</param>
		/// <param name="oldVersion">The game version in which the existing data was saved</param>
		public static void UpdateGameVersion(string newVersion, out string oldVersion) {
			oldVersion = GetString(SAVE_KEY_GAME_VERSION);
			SetString(SAVE_KEY_GAME_VERSION, newVersion);
		}

		/// <summary>
		/// Clears all cached values retrieved from the device.
		/// </summary>
		public static void ClearCache() {
			_jsonCache.Clear();
			_stringCache.Clear();
			_floatCache.Clear();
			_intCache.Clear();
		}

		/// <summary>
		/// Delete a saved entry from both the device and the cache.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		public static void DeleteKey(string key, bool shared = false) {
			string saveKey = shared ? key : GetSaveKey(userID, key);
			PlayerPrefs.DeleteKey(saveKey);

			_jsonCache.Remove(key);
			_stringCache.Remove(key);
			_floatCache.Remove(key);
			_intCache.Remove(key);
		}

		/// <summary>
		/// Deletes all saved data from both the device and the cache, including any PlayerPrefs not written through SaveCache.
		/// </summary>
		public static void DeleteAll() {
			PlayerPrefs.DeleteAll();
			ClearCache();
		}

		/// <summary>
		/// Retrieve a saved JSON value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="defaultValue">If no value has been previously saved, this value will be cached and returned.</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		/// <returns>the value stored with <paramref name="key"/>, or <paramref name="defaultValue"/> if none was stored</returns>
		public static JSONNode GetJSON(string key, JSONNode defaultValue = null, bool shared = false) {
			string saveKey = shared ? key : GetSaveKey(userID, key);
			return CoalesceSavedJSON(saveKey, defaultValue ?? new JSONClass());
		}

		/// <summary>
		/// Save a JSON value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="value">The value to store</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		public static void SetJSON(string key, JSONNode value, bool shared = false) {
			string saveKey = shared ? key : GetSaveKey(userID, key);
			PlayerPrefs.SetString(saveKey, value.ToString());
			_jsonCache[saveKey] = value;

			if (logDebugMessages) Debug.Log(string.Format("Saved JSON value: {0} = {1}", saveKey, value.ToString("")));
		}

		/// <summary>
		/// Retrieve a saved string value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="defaultValue">If no value has been previously saved, this value will be cached and returned.</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		/// <returns>the value stored with <paramref name="key"/>, or <paramref name="defaultValue"/> if none was stored</returns>
		public static string GetString(string key, string defaultValue = "", bool shared = false) {
			string saveKey = shared ? key : GetSaveKey(userID, key);
			return CoalesceSavedString(saveKey, defaultValue);
		}

		/// <summary>
		/// Save a string value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="value">The value to store</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		public static void SetString(string key, string value, bool shared = false) {
			string saveKey = shared ? key : GetSaveKey(userID, key);
			PlayerPrefs.SetString(saveKey, value);
			_stringCache[saveKey] = value;

			if (logDebugMessages) Debug.Log(string.Format("Saved string value: {0} = {1}", saveKey, value));
		}

		/// <summary>
		/// Retrieve a saved floating point value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="defaultValue">If no value has been previously saved, this value will be cached and returned.</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		/// <returns>the value stored with <paramref name="key"/>, or <paramref name="defaultValue"/> if none was stored</returns>
		public static float GetFloat(string key, float defaultValue = 0f, bool shared = false) {
			string saveKey = shared ? key : GetSaveKey(userID, key);
			return CoalesceSavedFloat(saveKey, defaultValue);
		}

		/// <summary>
		/// Save a floating point value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="value">The value to store</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		public static void SetFloat(string key, float value, bool shared = false) {
			string saveKey = shared ? key : GetSaveKey(userID, key);
			PlayerPrefs.SetInt(saveKey, BitwiseUtilities.FloatToInt32(value));
			_floatCache[saveKey] = value;

			if (logDebugMessages) Debug.Log(string.Format("Saved float value: {0} = {1}", saveKey, value));
		}

		/// <summary>
		/// Retrieve a saved integer value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="defaultValue">If no value has been previously saved, this value will be cached and returned.</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		/// <returns>the value stored with <paramref name="key"/>, or <paramref name="defaultValue"/> if none was stored</returns>
		public static int GetInt(string key, int defaultValue = 0, bool shared = false) {
			string saveKey = shared ? key : GetSaveKey(userID, key);
			return CoalesceSavedInt(saveKey, defaultValue);
		}

		/// <summary>
		/// Save an integer value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="value">The value to store</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		public static void SetInt(string key, int value, bool shared = false) {
			string saveKey = shared ? key : GetSaveKey(userID, key);
			PlayerPrefs.SetInt(saveKey, value);
			_intCache[saveKey] = value;

			if (logDebugMessages) Debug.Log(string.Format("Saved int value: {0} = {1}", saveKey, value));
		}

		/// <summary>
		/// Retrieve a saved boolean value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="defaultValue">If no value has been previously saved, this value will be cached and returned.</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		/// <returns>the value stored with <paramref name="key"/>, or <paramref name="defaultValue"/> if none was stored</returns>
		public static bool GetBool(string key, bool defaultValue = false, bool shared = false) {
			return GetInt(key, defaultValue ? 1 : 0, shared) != 0;
		}

		/// <summary>
		/// Save a boolean value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="value">The value to store</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		public static void SetBool(string key, bool value, bool shared = false) {
			SetInt(key, value ? 1 : 0, shared);
		}

		/// <summary>
		/// Retrieve a saved color value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="defaultValue">If no value has been previously saved, this value will be cached and returned.</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		/// <returns>the value stored with <paramref name="key"/>, or <paramref name="defaultValue"/> if none was stored</returns>
		public static Color32 GetColor(string key, Color32 defaultValue = default(Color32), bool shared = false) {
			return GetInt(key, defaultValue.ToInt(), shared).ToColor();
		}

		/// <summary>
		/// Save a color value.
		/// </summary>
		/// <param name="key">The unique identifier for the saved entry</param>
		/// <param name="value">The value to store</param>
		/// <param name="shared">If true, the saved entry is shared among all users instead of only the active userID.</param>
		public static void SetColor(string key, Color32 value, bool shared = false) {
			SetInt(key, value.ToInt(), shared);
		}
	}
}
