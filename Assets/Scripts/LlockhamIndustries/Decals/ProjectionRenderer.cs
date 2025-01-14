using UnityEngine;
using UnityEngine.Rendering;

namespace LlockhamIndustries.Decals
{
	[ExecuteInEditMode]
	public class ProjectionRenderer : MonoBehaviour
	{
		private MaterialPropertyBlock block;

		private bool marked = true;

		[SerializeField]
		private Projection projection;

		private Projection active;

		[SerializeField]
		private ProjectionProperty[] properties;

		[SerializeField]
		private Vector2 tiling;

		[SerializeField]
		private Vector2 offset;

		[SerializeField]
		protected MaskMethod maskMethod;

		[SerializeField]
		protected bool[] masks = new bool[4];

		private MeshRenderer meshRenderer;

		private ProjectionData data;

		private PoolItem poolItem;

		public int _TilingOffset;

		public int _MaskBase;

		public int _MaskLayers;

		public ProjectionProperty[] Properties
		{
			get
			{
				return properties;
			}
			set
			{
				if (value != null)
				{
					properties = (ProjectionProperty[])value.Clone();
				}
				else
				{
					properties = null;
				}
				MarkProperties();
			}
		}

		public Vector2 Tiling
		{
			get
			{
				return tiling;
			}
			set
			{
				if (tiling != value)
				{
					tiling = value;
					MarkProperties();
				}
			}
		}

		public Vector2 Offset
		{
			get
			{
				return offset;
			}
			set
			{
				if (offset != value)
				{
					offset = value;
					MarkProperties();
				}
			}
		}

		public MaskMethod MaskMethod
		{
			get
			{
				return maskMethod;
			}
			set
			{
				if (maskMethod != value)
				{
					maskMethod = value;
					MarkProperties();
				}
			}
		}

		public bool MaskLayer1
		{
			get
			{
				return masks[0];
			}
			set
			{
				if (masks[0] != value)
				{
					masks[0] = value;
					MarkProperties();
				}
			}
		}

		public bool MaskLayer2
		{
			get
			{
				return masks[1];
			}
			set
			{
				if (masks[1] != value)
				{
					masks[1] = value;
					MarkProperties();
				}
			}
		}

		public bool MaskLayer3
		{
			get
			{
				return masks[2];
			}
			set
			{
				if (masks[2] != value)
				{
					masks[2] = value;
					MarkProperties();
				}
			}
		}

		public bool MaskLayer4
		{
			get
			{
				return masks[3];
			}
			set
			{
				if (masks[3] != value)
				{
					masks[3] = value;
					MarkProperties();
				}
			}
		}

		public Projection Projection
		{
			get
			{
				if (base.gameObject.activeInHierarchy && active != null)
				{
					return active;
				}
				return projection;
			}
			set
			{
				if (projection != value)
				{
					projection = value;
					ChangeProjection();
				}
			}
		}

		public MeshRenderer Renderer => meshRenderer;

		public ProjectionData Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
			}
		}

		public ProjectionPool Pool
		{
			get
			{
				if (poolItem != null)
				{
					return poolItem.Pool;
				}
				return null;
			}
		}

		public PoolItem PoolItem
		{
			get
			{
				return poolItem;
			}
			set
			{
				poolItem = value;
			}
		}

		public void ResetProperties(bool UpdateImmediately = false)
		{
			tiling = Vector2.one;
			offset = Vector2.zero;
			maskMethod = MaskMethod.DrawOnEverythingExcept;
			masks = new bool[4];
			if (projection != null)
			{
				properties = (ProjectionProperty[])projection.Properties.Clone();
			}
			else
			{
				properties = null;
			}
			MarkProperties(UpdateImmediately);
		}

		public void UpdateProperties()
		{
			if (meshRenderer != null && marked && Projection != null)
			{
				UpdateRendererBlock(Properties, Projection.Properties);
				meshRenderer.SetPropertyBlock(block);
				marked = false;
			}
		}

		public void MarkProperties(bool UpdateImmediately = false)
		{
			marked = true;
			if (UpdateImmediately && meshRenderer != null)
			{
				UpdateProperties();
			}
		}

		public void SetFloat(int PropertyIndex, float Float)
		{
			if ((properties[PropertyIndex].type == PropertyType.Float || properties[PropertyIndex].type == PropertyType.Combo) && properties[PropertyIndex].value != Float)
			{
				properties[PropertyIndex].value = Float;
				properties[PropertyIndex].enabled = true;
				MarkProperties();
			}
		}

		public void SetColor(int PropertyIndex, Color Color)
		{
			if ((properties[PropertyIndex].type == PropertyType.Color || properties[PropertyIndex].type == PropertyType.Combo) && properties[PropertyIndex].color != Color)
			{
				properties[PropertyIndex].color = Color;
				properties[PropertyIndex].enabled = true;
				MarkProperties();
			}
		}

		public void ChangeProjection()
		{
			if (active != projection)
			{
				if (base.gameObject.activeInHierarchy && base.enabled && active != null)
				{
					Deregister();
				}
				active = projection;
				if (base.gameObject.activeInHierarchy && base.enabled && active != null)
				{
					Register();
				}
				tiling = Vector2.one;
				offset = Vector2.zero;
				if (active != null)
				{
					Properties = active.Properties;
				}
				else
				{
					Properties = null;
				}
			}
			UpdateProjection();
		}

		public void UpdateProjection()
		{
			if (!(meshRenderer != null))
			{
				return;
			}
			if (Projection != null && Projection.Valid)
			{
				meshRenderer.gameObject.SetActive(value: true);
				meshRenderer.sharedMaterial = Projection.Mat;
				UpdateRendererBlock(Properties, Projection.Properties);
				meshRenderer.SetPropertyBlock(block);
				marked = false;
				return;
			}
			meshRenderer.gameObject.SetActive(value: false);
			meshRenderer.sharedMaterial = null;
			if (block != null)
			{
				block.Clear();
			}
			meshRenderer.SetPropertyBlock(block);
		}

		private bool Register()
		{
			if (this != null)
			{
				return DynamicDecals.System.Register(this);
			}
			return false;
		}

		private void Deregister()
		{
			if (this != null)
			{
				DynamicDecals.System.Deregister(this);
			}
		}

		public void InitializeRenderer(bool Active)
		{
			if (meshRenderer == null)
			{
				foreach (Transform item in base.transform)
				{
					if (item.name == "Renderer")
					{
						meshRenderer = item.GetComponent<MeshRenderer>();
						break;
					}
				}
			}
			if (meshRenderer == null)
			{
				GameObject gameObject = new GameObject("Renderer");
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				gameObject.layer = base.gameObject.layer;
				gameObject.hideFlags = (HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild);
				gameObject.AddComponent<MeshFilter>().sharedMesh = DynamicDecals.System.Cube;
				meshRenderer = gameObject.AddComponent<MeshRenderer>();
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
				meshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
			}
			meshRenderer.gameObject.SetActive(Active);
		}

		public void TerminateRenderer()
		{
			meshRenderer.gameObject.SetActive(value: false);
		}

		private void UpdateRendererBlock(ProjectionProperty[] Local, ProjectionProperty[] Global)
		{
			if (block == null)
			{
				block = new MaterialPropertyBlock();
			}
			else
			{
				block.Clear();
			}
			block.SetVector(_TilingOffset, new Vector4(Tiling.x, Tiling.y, Offset.x, Offset.y));
			switch (maskMethod)
			{
			case MaskMethod.DrawOnEverythingExcept:
			{
				block.SetFloat(_MaskBase, 1f);
				Color clear2 = Color.clear;
				clear2.r = (masks[0] ? 0f : 0.5f);
				clear2.g = (masks[1] ? 0f : 0.5f);
				clear2.b = (masks[2] ? 0f : 0.5f);
				clear2.a = (masks[3] ? 0f : 0.5f);
				block.SetVector(_MaskLayers, clear2);
				break;
			}
			case MaskMethod.OnlyDrawOn:
			{
				block.SetFloat(_MaskBase, 0f);
				Color clear = Color.clear;
				clear.r = (masks[0] ? 1f : 0.5f);
				clear.g = (masks[1] ? 1f : 0.5f);
				clear.b = (masks[2] ? 1f : 0.5f);
				clear.a = (masks[3] ? 1f : 0.5f);
				block.SetVector(_MaskLayers, clear);
				break;
			}
			}
			for (int i = 0; i < Local.Length; i++)
			{
				if (Local[i].type == PropertyType.Float)
				{
					float value = Local[i].enabled ? Local[i].value : Global[i].value;
					block.SetFloat(Global[i].nameID, value);
				}
				if (Local[i].type == PropertyType.Color)
				{
					Color value2 = Local[i].enabled ? Local[i].color : Global[i].color;
					block.SetColor(Global[i].nameID, value2);
				}
				if (Local[i].type == PropertyType.Combo)
				{
					Color value3 = Local[i].enabled ? (Local[i].color * Local[i].value) : (Global[i].color * Global[i].value);
					block.SetColor(Global[i].nameID, value3);
				}
			}
		}

		private void OnEnable()
		{
			_TilingOffset = Shader.PropertyToID("_TilingOffset");
			_MaskBase = Shader.PropertyToID("_MaskBase");
			_MaskLayers = Shader.PropertyToID("_MaskLayers");
			Initialize();
		}

		private void OnDisable()
		{
			Terminate();
		}

		private void Initialize()
		{
			if (projection != null)
			{
				active = projection;
				bool flag = Register();
				InitializeRenderer(flag);
			}
			else
			{
				InitializeRenderer(Active: false);
			}
			UpdateProjection();
		}

		private void Terminate()
		{
			if (projection != null)
			{
				Deregister();
			}
			TerminateRenderer();
		}

		public void MoveToTop()
		{
			if (data != null)
			{
				data.MoveToTop(this);
			}
		}

		public void MoveToBottom()
		{
			if (data != null)
			{
				data.MoveToBottom(this);
			}
		}

		public float CheckIntersecting(Vector3 Point)
		{
			Vector3 vector = base.transform.InverseTransformPoint(Point);
			return Mathf.Clamp01(2f * (0.5f - Mathf.Max(Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y)), Mathf.Abs(vector.z))));
		}

		public void Destroy()
		{
			if (poolItem != null)
			{
				poolItem.Return();
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
