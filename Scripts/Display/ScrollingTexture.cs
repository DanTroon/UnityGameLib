using UnityGameLib.Attributes;
using UnityEngine;

namespace UnityGameLib.Display {
	/// <summary>
	/// Animates a material by applying motion to its texture's UV coordinates.
	/// </summary>
	public class ScrollingTexture : MonoBehaviour {
		[SerializeField, RuntimeLocked, Tooltip("The renderer to modify.")]
		protected Renderer _targetRenderer;
		[SerializeField, RuntimeLocked, Tooltip("The index of the material in the renderer. Use zero if it's the only one.")]
		protected int _materialIndex = 0;
		[SerializeField, RuntimeLocked, Tooltip("The name of the affected texture property on the material.")]
		protected string _textureName = "_MainTex";

		/// <summary>
		/// The scrolling rate along the X and Y axes of the texture.
		/// </summary>
		public Vector2 velocity = new Vector2(1.0f, 0.0f);
		
		protected Material _targetMaterial;

		protected virtual void OnEnable() {
			if (!_targetRenderer)
				_targetRenderer = GetComponent<Renderer>();

			_targetMaterial = _targetRenderer.materials[_materialIndex];
		}

		protected virtual void LateUpdate() {
			Vector2 delta = velocity * Time.deltaTime;
			_targetMaterial.SetTextureOffset(_textureName, _targetMaterial.GetTextureOffset(_textureName) + delta);
		}
	}
}