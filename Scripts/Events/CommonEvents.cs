using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityGameLib.Events {
	/// <summary>A serializable UnityEvent type with no parameters.</summary>
	[Serializable]
	public class SimpleEvent : UnityEvent { }

	/// <summary>A serializable UnityEvent type with a boolean parameter.</summary>
	[Serializable]
	public class BooleanEvent : UnityEvent<bool> { }

	/// <summary>A serializable UnityEvent type with an integer parameter.</summary>
	[Serializable]
	public class IntegerEvent : UnityEvent<int> { }

	/// <summary>A serializable UnityEvent type with a float parameter.</summary>
	[Serializable]
	public class FloatEvent : UnityEvent<float> { }

	/// <summary>A serializable UnityEvent type with a string parameter.</summary>
	[Serializable]
	public class StringEvent : UnityEvent<string> { }

	/// <summary>A serializable UnityEvent type with a PointerEventData parameter for input handling.</summary>
	[Serializable]
	public class PointerEvent : UnityEvent<PointerEventData> { }
}
