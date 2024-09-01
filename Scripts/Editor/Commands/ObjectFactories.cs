using UnityGameLib.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameLib.Editor.Commands {
	/// <summary>
	/// Provides menu items to generate preset objects with UnityGameLib components.
	/// </summary>
	public static class ObjectFactories {
		[MenuItem("GameObject/Create Other/Troon/3D Object/TogglingElement Quad", false, 1)]
		public static void CommandCreateTogglingElementQuad(MenuCommand command) {
			GameObject context = command.context as GameObject;
			GameObject result;

			//Create the object
			result = GameObject.CreatePrimitive(PrimitiveType.Quad);
			result.name = "TogglingElement";

			//Mark the object creation for undo
			Undo.RegisterCreatedObjectUndo(result, "Create TogglingElement");

			//Parent the object to the selection
			if (context) {
				result.transform.SetParent(context.transform, false);
			}

			//Set the animator controller
			SetOverrideController(result.AddComponent<Animator>(), "TogglingMaterial");

			//Add the TogglingElement component
			result.AddComponent<TogglingElement>();

			//Select the resulting object
			Selection.activeGameObject = result;
		}

		[MenuItem("GameObject/Create Other/Troon/UI/TogglingElement UI", false, 5)]
		public static void CommandCreateTogglingElementUI(MenuCommand command) {
			GameObject context = command.context as GameObject;
			GameObject result;

			//Create the object and its components (include Canvas only if it's not being placed inside a Canvas)
			if (context && context.GetComponentInParent<Canvas>()) {
				result = new GameObject("TogglingElement", typeof(RectTransform), typeof(CanvasGroup), typeof(Animator), typeof(TogglingElement));
			} else {
				result = new GameObject("TogglingElement", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup), typeof(Animator), typeof(TogglingElement));
			}

			//Mark the object creation for undo
			Undo.RegisterCreatedObjectUndo(result, "Create TogglingElement");

			//Parent the object to the selection
			if (context) {
				result.transform.SetParent(context.transform, false);
			}

			//If the object is a Canvas, make it Overlay by default; otherwise stretch it to match its parent
			Canvas canvas = result.GetComponent<Canvas>();
			if (canvas) {
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			} else {
				RectTransform transform = result.GetComponent<RectTransform>();
				transform.anchorMin = new Vector2(0f, 0f);
				transform.anchorMax = new Vector2(1f, 1f);
				transform.sizeDelta = new Vector2(0f, 0f);
			}

			//Set the animator controller
			SetOverrideController(result.GetComponent<Animator>(), "TogglingElementSimple");

			//Select the resulting object
			Selection.activeGameObject = result;
		}

		private static void SetOverrideController(Animator animator, string controllerName) {
			string[] controllerCandidateGUIDs = AssetDatabase.FindAssets("t:AnimatorOverrideController " + controllerName);

			if (controllerCandidateGUIDs.Length > 0) {
				string path = AssetDatabase.GUIDToAssetPath(controllerCandidateGUIDs[0]);
				animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(path);
			}
		}
	}
}
