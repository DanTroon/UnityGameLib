using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityGameLib.Utilities {
	/// <summary>
	/// Provides static utility methods for handling objects and structs.
	/// </summary>
	public static class ObjectUtilities {
		#region CreateObject
		/// <summary>
		/// Creates a GameObject with a component and returns the component instance.
		/// </summary>
		/// <typeparam name="T">The component type</typeparam>
		/// <param name="name">An optional name for the GameObject</param>
		/// <returns>The resulting component instance</returns>
		public static T CreateObject<T>(string name = null) where T : Component {
			if (string.IsNullOrEmpty(name))
				name = typeof(T).Name;

			return new GameObject(name).AddComponent<T>();
		}
		#endregion

		#region GetTaggedObject
		/// <summary>
		/// Returns the first object found with the matching tag and name.
		/// </summary>
		/// <param name="tag">The tag to search for</param>
		/// <param name="name">The object name to search for</param>
		/// <returns>The first object matching tag and name, or null if none exists</returns>
		public static GameObject GetTaggedObject(string tag, string name) {
			GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);
			for (int i = 0, count = candidates.Length; i < count; ++i) {
				if (candidates[i].name == name) {
					return candidates[i];
				}
			}
			return null;
		}
		/// <summary>
		/// Returns a component of the first object found with the matching tag and name.
		/// </summary>
		/// <typeparam name="T">The component type</typeparam>
		/// <param name="tag">The tag to search for</param>
		/// <param name="name">The object name to search for</param>
		/// <returns>The component of the first object matching tag and name, or null if none exists</returns>
		public static T GetTaggedObject<T>(string tag, string name) where T : Component {
			GameObject obj = GetTaggedObject(tag, name);
			return obj ? obj.GetComponent<T>() : null;
		}
		#endregion

		#region MountObject
		/// <summary>
		/// Places an object within a parent and removes its local transformations.
		/// </summary>
		/// <param name="mount">The parent transform</param>
		/// <param name="child">The child object or transform</param>
		public static void MountObject(Transform mount, Transform child) {
			child.SetParent(mount, false);
			child.localPosition = Vector3.zero;
			child.localRotation = Quaternion.identity;
		}
		/// <summary>
		/// Places an object within a parent and removes its local transformations.
		/// </summary>
		/// <param name="mount">The parent transform</param>
		/// <param name="child">The child object or transform</param>
		public static void MountObject(Transform mount, GameObject child) {
			MountObject(mount, child.transform);
		}
		/// <summary>
		/// Places an object within a parent and removes its local transformations.
		/// </summary>
		/// <param name="mount">The parent transform</param>
		/// <param name="child">A component of the child object</param>
		public static void MountObject(Transform mount, Component child) {
			MountObject(mount, child.transform);
		}
		#endregion
		
		/// <summary>
		/// Whether an input pointer is over a canvas element.  Use this to determine when a raycast should pass through to world space.
		/// </summary>
		/// <returns>True if any input pointer is touching a canvas element, otherwise false.</returns>
		public static bool IsPointerOverCanvas() {
			if (!EventSystem.current)
				return false;

			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; ++i) {
				if (EventSystem.current.IsPointerOverGameObject(i)) {
					return true;
				}
			}

			if (EventSystem.current.IsPointerOverGameObject()) {
				return true;
			}

			return false;
		}
	}
}
