using UnityEngine;
using System;
using System.Collections.Generic;

namespace UnityGameLib.Collections {
	/// <summary>
	/// A dictionary extension that works within the Unity property inspector.
	/// </summary>
	/// <remarks>
	/// Keys can be any "value type" except Char. Valid key types include:
	/// - All numeric types
	/// - Strings
	/// - Enums
	/// - Booleans
	/// - Serializable structs (Vector2, Rect, etc.)
	/// 
	/// Values can be any serializable reference or value type.
	/// 
	/// Note: This class was modified for Unity 5.5 to extend SortedDictionary instead of Dictionary.
	/// SortedDictionary causes erroneous behavior in Unity 5.6+, so the change was reverted.
	/// </remarks>
	/// <example>
	/// Example Usage:
	/// <code>
	/// [Serializable] public class StringMap : SerializableDictionary<string, string> { }
	///	[DictionaryType] public StringMap test;
	///	</code>
	///	</example>
	/// <typeparam name="TKey">The dictionary key type</typeparam>
	/// <typeparam name="TValue">The dictionary value type</typeparam>
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
		private const int MAX_KEYGEN_ATTEMPTS = 500;

		[SerializeField, HideInInspector] protected List<TKey> _serialKeys = new List<TKey>();
		[SerializeField, HideInInspector] protected List<TValue> _serialValues = new List<TValue>();
		
		public virtual void OnBeforeSerialize() {
			_serialKeys.Clear();
			_serialValues.Clear();

			if (!ValidateTypes())
				return;

			foreach (KeyValuePair<TKey, TValue> keyVal in this) {
				_serialKeys.Add(keyVal.Key);
				_serialValues.Add(keyVal.Value);
			}
		}
		
		public virtual void OnAfterDeserialize() {
			Clear();

			if (!ValidateTypes())
				return;

			//Ensure the key and value counts match, and take measures to correct them if not
			if (_serialKeys.Count < _serialValues.Count) {
				Debug.LogError(string.Format("Key-value count mismatch ({0} keys, {1} values). Orphaned values will be removed to correct this.", _serialKeys.Count, _serialValues.Count));
				_serialValues.RemoveRange(_serialKeys.Count, _serialValues.Count - _serialKeys.Count);
			} else if (_serialKeys.Count > _serialValues.Count) {
				Debug.LogError(string.Format("Key-value count mismatch ({0} keys, {1} values). Default values will be added to correct this.", _serialKeys.Count, _serialValues.Count));
				for (int i = _serialValues.Count; i < _serialKeys.Count; ++i) {
					_serialValues[i] = default(TValue);
				}
			}

			//Write the keys and values into the dictionary
			for (int i = 0; i < _serialKeys.Count; i++) {
				if (ContainsKey(_serialKeys[i])) {
					//If the key already exists during deserialization, assume it's a new entry in the inspector and generate a new key
					try {
						TKey generatedKey = GenerateUniqueKey(_serialKeys[i]);
						Add(generatedKey, _serialValues[i]);
					} catch {
						throw new UnityException(string.Format("Unable to generate a unique key of type '{0}'.", typeof(TKey).Name));
					}
				} else {
					//If the key is already unique, add it as normal
					Add(_serialKeys[i], _serialValues[i]);
				}
			}
		}

		/// <summary>
		/// Tests this dictionary for key or value types that may prevent it from functioning correctly and logs an error if not.
		/// </summary>
		/// <returns><c>true</c> if this dictionary uses valid key and value types, or <c>false</c> if not</returns>
		protected virtual bool ValidateTypes() {
			if (typeof(TKey) == typeof(char)) {
				Debug.LogError("Type 'Char' cannot be used as a SerializableDictionary key. Use an integer, string, or enum instead.");
				return false;
			}

			return true;
		}

		#region KeyGeneration
		private TKey GenerateUniqueKey(TKey startingValue) {
			//Attempt to create a unique key of the correct type. This is type-specific and may fail.
			Type keyType = typeof(TKey);

			if (keyType == typeof(string)) {
				return (TKey) Convert.ChangeType(GenerateUniqueString(Convert.ToString(startingValue)), keyType);
			}

			if (keyType == typeof(bool)) {
				return (TKey) Convert.ChangeType(GenerateUniqueBool(Convert.ToBoolean(startingValue)), keyType);
			}

			if (keyType.IsEnum) {
				return (TKey) Enum.ToObject(keyType, GenerateUniqueEnum(Convert.ToInt32(startingValue)));
			}

			if (IsNumericType(keyType)) {
				return (TKey) Convert.ChangeType(GenerateUniqueInt(Convert.ToInt32(startingValue)), keyType);
			}

			//Use the default value for arbitrary structs. This will almost certainly fail on reference types.
			return default(TKey);
		}

		private string GenerateUniqueString(string startingValue = "key") {
			//Make string copies of all existing keys for comparison
			KeyCollection rawKeys = Keys;
			List<string> stringKeys = new List<string>(Keys.Count);
			foreach (TKey rawKey in rawKeys) {
				stringKeys.Add(Convert.ToString(rawKey));
			}

			//Identify any integer suffix on the string
			int numChars = startingValue.Length;
			int numIntegerChars = 0;
			for (int i = 1; i < numChars; ++i) {
				if (char.IsDigit(startingValue, numChars - i)) {
					numIntegerChars = i;
				} else {
					break;
				}
			}

			//Increment the integer suffix until a unique string is found, but give up eventually
			string baseStr = string.IsNullOrEmpty(startingValue) ? "key" : startingValue.Substring(0, numChars - numIntegerChars);
			int suffix = numIntegerChars == 0 ? 0 : int.Parse(startingValue.Substring(numChars - numIntegerChars)) + 1;
			int attemptsRemaining = MAX_KEYGEN_ATTEMPTS;
			string candidate;

			while (attemptsRemaining-- > 0) {
				candidate = baseStr + suffix;
				if (!stringKeys.Contains(candidate)) {
					return candidate;
				}
				++suffix;
			}

			//If failed, just return an empty string
			return "";
		}

		private bool GenerateUniqueBool(bool startingValue = false) {
			KeyCollection rawKeys = Keys;

			//If exactly one key exists, identify its boolean value and choose the other one
			if (rawKeys.Count == 1) {
				foreach (TKey rawKey in rawKeys) {
					return !Convert.ToBoolean(rawKey);
				}
			}

			//Otherwise just return the startingValue, which is expected to work only if there are no keys
			return startingValue;
		}

		private int GenerateUniqueEnum(int startingValue) {
			//Get the possible enum values, if any exist
			Array rawCandidates = Enum.GetValues(typeof(TKey));
			int numCandidates = rawCandidates.Length;
			if (numCandidates == 0) {
				return 0;
			}

			//List the integer representations of all possible enum values
			List<int> candidates = new List<int>(numCandidates);
			for (int i = 0; i < numCandidates; ++i) {
				candidates.Add((int) rawCandidates.GetValue(i));
			}

			//Make integer copies of all existing keys for comparison
			KeyCollection rawKeys = Keys;
			List<int> intKeys = new List<int>(Keys.Count);
			foreach (TKey rawKey in rawKeys) {
				intKeys.Add(Convert.ToInt32(rawKey));
			}

			//Identify a starting point based on startingValue
			int startingIndex = candidates.IndexOf(startingValue);
			if (startingIndex == -1)
				startingIndex = 0;

			//Search for an unused enum value
			int index = startingIndex;
			do {
				if (!intKeys.Contains(candidates[index])) {
					return candidates[index];
				}

				index = index < numCandidates - 1 ? index + 1 : 0;
			} while (index != startingIndex);

			//If no value is valid, just return the first enum option
			return candidates[0];
		}

		private int GenerateUniqueInt(int startingValue = 0) {
			//Make integer copies of all existing keys for comparison
			KeyCollection rawKeys = Keys;
			List<int> intKeys = new List<int>(Keys.Count);
			foreach (TKey rawKey in rawKeys) {
				intKeys.Add(Convert.ToInt32(rawKey));
			}

			//Increment from startingValue until an unused value is found, but give up eventually
			int attemptsRemaining = MAX_KEYGEN_ATTEMPTS;
			int candidate = startingValue;
			while (attemptsRemaining-- > 0) {
				if (!intKeys.Contains(candidate)) {
					return candidate;
				}
				++candidate;
			}

			//If failed, just return a random value and hope it works
			return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		}

		private static bool IsNumericType(Type type) {
			switch (Type.GetTypeCode(type)) {
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				case TypeCode.Object:
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
						return IsNumericType(Nullable.GetUnderlyingType(type));
					}
					return false;
				default:
					return false;
			}
		}
		#endregion
	}
}
