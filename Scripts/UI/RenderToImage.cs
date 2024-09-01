using UnityGameLib.Attributes;
using UnityGameLib.Geometry;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameLib.UI {
	/// <summary>
	/// While active, renders the attached camera onto a canvas Image instead of onto the screen.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public class RenderToImage : MonoBehaviour {
		[SerializeField, Required, Tooltip("The image on which to render.")]
		protected Image _image;
		[SerializeField, RuntimeLocked, Tooltip("The shader to use in rendering. If empty, the default Standard shader is used.")]
		protected Shader _shader;
		[SerializeField, RuntimeLocked, Tooltip("The name of the shader texture property to replace with the camera view.")]
		protected string _shaderTextureName = "_MainTex";
		[SerializeField, Tooltip("The output texture resolution. (Must be more than zero in each dimension).")]
		protected Coordinates2D _resolution = new Coordinates2D(32, 32);

		protected Camera _camera;
		protected RenderTexture _texture;
		protected Material _material;

		protected virtual void Awake() {
			if (_image == null)
				throw new UnityException("Target image cannot be null.");

			if (_resolution.x <= 0 || _resolution.y <= 0)
				throw new UnityException("Resolution must be non-zero.");

			if (_shader == null)
				_shader = Shader.Find("Unlit/Texture");

			_camera = GetComponent<Camera>();
			_texture = new RenderTexture(_resolution.x, _resolution.y, 0);
			_texture.depth = 24;
		}

		protected virtual void OnEnable() {
			_texture.Create();
			_texture.name = _camera.name;

			_material = new Material(_shader);
			_material.SetTexture(_shaderTextureName, _texture);
			_material.name = _camera.name;

			_image.material = _material;

			_camera.targetTexture = _texture;
		}

		protected virtual void OnDisable() {
			if (_image != null) {
				_image.material = _image.defaultMaterial;
			}

			if (_material != null) {
				_material.SetTexture(_shaderTextureName, null);
				_material = null;
			}

			if (_camera) {
				_camera.targetTexture = null;
			}

			_texture.DiscardContents();
			_texture.Release();
		}

		/// <summary>
		/// Reinitializes the texture and material used for rendering. 
		/// </summary>
		public virtual void Refresh() {
			if (isActiveAndEnabled) {
				OnDisable();

				#if UNITY_EDITOR
				//Fix asset bundle shader reference
				_shader = Shader.Find(_shader.name);
				#endif

				OnEnable();
			}
		}
	}
}
