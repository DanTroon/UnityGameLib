using UnityEditor;
using UnityEngine;

namespace UnityGameLib.Editor.Windows {
	public class AutoSnap : EditorWindow {

		private const string KEY_ENABLE = "AutoSnap_enable";
		private const string KEY_COMPONENT_ENABLE = "AutoSnap_enable_{0}";//property
		private const string KEY_COMPONENT_VALUE = "AutoSnap_value_{0}_{1}";// property, axis

		private const string TAG_POSITION = "position";
		private const string TAG_ROTATION = "rotation";
		private const string TAG_SCALE = "scale";
		
		private const int VECTOR_GUI_INDENT = 2;

		private static readonly Vector3 POSITION_INTERVALS_DEFAULT = Vector3.zero;
		private static readonly Vector3 ROTATION_INTERVALS_DEFAULT = Vector3.zero;
		private static readonly Vector3 SCALE_INTERVALS_DEFAULT = Vector3.zero;

		private bool _enableSnap = false;
		private bool _enablePosition = false;
		private bool _enableRotation = false;
		private bool _enableScale = false;
		private Vector3 _positionIntervals = POSITION_INTERVALS_DEFAULT;
		private Vector3 _rotationIntervals = ROTATION_INTERVALS_DEFAULT;
		private Vector3 _scaleIntervals = SCALE_INTERVALS_DEFAULT;

		private Vector3 _previousPosition;
		private Vector3 _previousRotation;
		private Vector3 _previousScale;
		private Transform _currentTransform;

		[MenuItem("Window/Auto Snapping")]
		public static void Init() {
			AutoSnap window = GetWindow<AutoSnap>("Auto Snapping", true);
			window.minSize = new Vector2(400f, 120f);
			window.Show();
		}

		protected void OnEnable() {
			_enableSnap = EditorPrefs.GetBool(KEY_ENABLE, false);
			_enablePosition = EditorPrefs.GetBool(string.Format(KEY_COMPONENT_ENABLE, TAG_POSITION), false);
			_enableRotation = EditorPrefs.GetBool(string.Format(KEY_COMPONENT_ENABLE, TAG_ROTATION), false);
			_enableScale = EditorPrefs.GetBool(string.Format(KEY_COMPONENT_ENABLE, TAG_SCALE), false);

			_positionIntervals = LoadVector(TAG_POSITION, POSITION_INTERVALS_DEFAULT);
			_rotationIntervals = LoadVector(TAG_ROTATION, ROTATION_INTERVALS_DEFAULT);
			_scaleIntervals = LoadVector(TAG_POSITION, SCALE_INTERVALS_DEFAULT);

			OnSelectionChanged();
			
			EditorApplication.update += OnUpdate;
			Selection.selectionChanged += OnSelectionChanged;
		}

		protected void OnDisable() {
			EditorPrefs.SetBool(KEY_ENABLE, _enableSnap);
			EditorPrefs.SetBool(string.Format(KEY_COMPONENT_ENABLE, TAG_POSITION), _enablePosition);
			EditorPrefs.SetBool(string.Format(KEY_COMPONENT_ENABLE, TAG_ROTATION), _enableRotation);
			EditorPrefs.SetBool(string.Format(KEY_COMPONENT_ENABLE, TAG_SCALE), _enableScale);

			SaveVector(TAG_POSITION, _positionIntervals);
			SaveVector(TAG_ROTATION, _rotationIntervals);
			SaveVector(TAG_SCALE, _scaleIntervals);

			EditorApplication.update -= OnUpdate;
			Selection.selectionChanged -= OnSelectionChanged;
		}

		protected void OnGUI() {
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			_enableSnap = EditorGUILayout.Toggle("Snap Automatically", _enableSnap);
			if (GUI.Button(EditorGUILayout.GetControlRect(), "Snap Selection Now")) {
				Undo.RecordObjects(Selection.transforms, "Snap Selection");
				SnapAll();
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Intervals", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
			EditorGUILayout.LabelField("Use zero to disable snapping.", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic });
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			DrawVectorGUI("Position", _enablePosition, out _enablePosition, _positionIntervals, out _positionIntervals);
			DrawVectorGUI("Rotation", _enableRotation, out _enableRotation, _rotationIntervals, out _rotationIntervals);
			DrawVectorGUI("Scale", _enableScale, out _enableScale, _scaleIntervals, out _scaleIntervals);
		}

		private void OnUpdate() {
			if (_enableSnap && !EditorApplication.isPlaying && Selection.activeTransform && Selection.activeTransform == _currentTransform) {
				if (_enablePosition && _currentTransform.localPosition != _previousPosition)
					SnapPosition();
				if (_enableRotation && _currentTransform.localEulerAngles != _previousRotation)
					SnapRotation();
				if (_enableScale && _currentTransform.localScale != _previousScale)
					SnapScale();

				RefreshTransform();
			}
		}

		private void OnSelectionChanged() {
			_currentTransform = Selection.activeTransform;
			RefreshTransform();
		}

		private void RefreshTransform() {
			if (!_currentTransform)
				return;

			_previousPosition = _currentTransform.localPosition;
			_previousRotation = _currentTransform.localEulerAngles;
			_previousScale = _currentTransform.localScale;
		}

		private Vector3 LoadVector(string componentTag, Vector3 defaultValue) {
			return new Vector3(
				EditorPrefs.GetFloat(string.Format(KEY_COMPONENT_VALUE, componentTag, "x"), defaultValue.x),
				EditorPrefs.GetFloat(string.Format(KEY_COMPONENT_VALUE, componentTag, "y"), defaultValue.y),
				EditorPrefs.GetFloat(string.Format(KEY_COMPONENT_VALUE, componentTag, "z"), defaultValue.z)
			);
		}

		private void SaveVector(string componentTag, Vector3 value) {
			EditorPrefs.SetFloat(string.Format(KEY_COMPONENT_VALUE, componentTag, "x"), value.x);
			EditorPrefs.SetFloat(string.Format(KEY_COMPONENT_VALUE, componentTag, "y"), value.y);
			EditorPrefs.SetFloat(string.Format(KEY_COMPONENT_VALUE, componentTag, "z"), value.z);
		}

		private void DrawVectorGUI(string label, bool activeIn, out bool activeOut, Vector3 valuesIn, out Vector3 valuesOut) {
			EditorGUILayout.BeginHorizontal();

			EditorGUI.indentLevel += VECTOR_GUI_INDENT;
			EditorGUI.BeginChangeCheck();
			activeOut = EditorGUILayout.Toggle(label, activeIn);
			EditorGUI.indentLevel -= VECTOR_GUI_INDENT;

			EditorGUI.BeginDisabledGroup(!activeOut);
			valuesOut = EditorGUILayout.Vector3Field(GUIContent.none, valuesIn);
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.EndHorizontal();
		}

		private void SnapAll() {
			if (EditorApplication.isPlaying)
				return;

			if (_enablePosition)
				SnapPosition();
			if (_enableRotation)
				SnapRotation();
			if (_enableScale)
				SnapScale();
		}

		private void SnapPosition() {
			foreach (Transform transform in Selection.transforms) {
				transform.localPosition = RoundToMultiple(transform.localPosition, _positionIntervals);
			}
		}

		private void SnapRotation() {
			foreach (Transform transform in Selection.transforms) {
				transform.localEulerAngles = RoundToMultiple(transform.localEulerAngles, _rotationIntervals);
			}
		}

		private void SnapScale() {
			foreach (Transform transform in Selection.transforms) {
				transform.localScale = RoundToMultiple(transform.localScale, _scaleIntervals);
			}
		}

		private float RoundToMultiple(float value, float multiple) {
			if (multiple == 0f || float.IsNaN(multiple) || float.IsInfinity(multiple)) {
				return value;
			}
			return multiple * Mathf.Round(value / multiple);
		}

		private Vector3 RoundToMultiple(Vector3 value, Vector3 multiple) {
			return new Vector3(
				RoundToMultiple(value.x, multiple.x),
				RoundToMultiple(value.y, multiple.y),
				RoundToMultiple(value.z, multiple.z)
			);
		}
	}
}
