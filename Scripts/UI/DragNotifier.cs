using UnityGameLib.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityGameLib.UI {
	/// <summary>
	/// Triggers events when the pointer is pressed an dragged on a canvas element.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class DragNotifier : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
		/// <summary> Triggers when the pointer is pressed down on this object. </summary>
		public PointerEvent onDragStart;
		/// <summary> Triggers each frame as the pointer is dragged after pressing down. </summary>
		public PointerEvent onDrag;
		/// <summary> Triggers when the pointer is released after pressing on this object. </summary>
		public PointerEvent onDragEnd;

		/// <summary>
		/// The RectTransform attached to this object.
		/// </summary>
		public RectTransform rectTransform {
			get { return GetComponent<RectTransform>(); }
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
			onDragStart.Invoke(eventData);
		}

		void IDragHandler.OnDrag(PointerEventData eventData) {
			onDrag.Invoke(eventData);
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
			onDragEnd.Invoke(eventData);
		}
	}
}
