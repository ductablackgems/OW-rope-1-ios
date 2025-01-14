using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImpactDeformable : MonoBehaviour
{
	public delegate bool GetColorEvent<T1, T2>(T1 impactDeformable, ref T2 deformationColor);

	private class ColliderXDeformable
	{
		public Collider Collider;

		public ImpactDeformable Deformable;
	}

	public float Hardness = 1f;

	public float MaxDeformationRadius;

	public float MaxVertexMov;

	public float RandomFactorDeformation;

	public Vector3 DeformationsScale = Vector3.one;

	public bool DeformMeshCollider;

	public bool LimitDeformationToMeshBounds = true;

	public bool RecalculateNormals = true;

	public Color DeformedColor = Color.black;

	public float ColorDeformationDistance;

	public Collision LastContact;

	[HideInInspector]
	public ImpactDeformable Master;

	[HideInInspector]
	public bool OverrideMaster;

	[HideInInspector]
	public MeshFilter MeshFilter;

	private List<ColliderXDeformable> ColliderXDeformablesCache = new List<ColliderXDeformable>();

	private MeshFilter deformedMeshFilter;

	private Mesh deformedMesh;

	private MeshCache meshCache;

	private Vector3[] deformedVertices;

	private Color[] deformedColors;

	private Color[] savedColors;

	private MeshCollider meshCollider;

	private float structuralDamage;

	private bool applyingChanges;

	public float StructuralDamage
	{
		get
		{
			if (structuralDamage == -1f)
			{
				if (deformedMesh == null)
				{
					return 0f;
				}
				Vector3 a = Vector3.zero;
				for (int i = 0; i < deformedVertices.Length; i++)
				{
					Vector3 vector = deformedVertices[i];
					Vector3 vector2 = meshCache.Vertices[i];
					a += new Vector3(Mathf.Abs(vector.x - vector2.x), Mathf.Abs(vector.y - vector2.y), Mathf.Abs(vector.z - vector2.z));
				}
				a /= deformedVertices.Length;
				a.Scale(meshCache.SizeFactor);
				structuralDamage = a.magnitude;
			}
			return structuralDamage;
		}
	}

	public float AverageStructuralDamage => (from i in base.transform.GetComponentsInChildren<ImpactDeformable>()
		where i.MeshFilter != null
		select i.StructuralDamage).DefaultIfEmpty(0f).Average();

	public event Action<ImpactDeformable, Vector3, Vector3> OnDeformForce;

	public event Action<ImpactDeformable> OnDeform;

	public event GetColorEvent<ImpactDeformable, Color> OnGetDeformationColor;

	private void Awake()
	{
		ImpactDeformable impactDeformable = FindMaster();
		if (impactDeformable != null && !OverrideMaster)
		{
			Hardness = impactDeformable.Hardness;
			MaxVertexMov = impactDeformable.MaxVertexMov;
			RandomFactorDeformation = impactDeformable.RandomFactorDeformation;
			DeformationsScale = impactDeformable.DeformationsScale;
			DeformMeshCollider = impactDeformable.DeformMeshCollider;
			DeformedColor = impactDeformable.DeformedColor;
			ColorDeformationDistance = impactDeformable.ColorDeformationDistance;
		}
		if (MeshFilter == null)
		{
			MeshFilter = GetComponent<MeshFilter>();
		}
		UpdateMeshFilter();
		meshCollider = GetComponent<MeshCollider>();
	}

	public ImpactDeformable FindMaster()
	{
		if (GetComponent<Rigidbody>() != null)
		{
			return null;
		}
		Transform parent = base.transform.parent;
		while (parent != null)
		{
			ImpactDeformable component = parent.GetComponent<ImpactDeformable>();
			Rigidbody component2 = parent.GetComponent<Rigidbody>();
			if ((bool)component && (bool)component2)
			{
				return component;
			}
			parent = parent.parent;
		}
		return null;
	}

	private bool UpdateMeshFilter()
	{
		if (deformedMeshFilter == MeshFilter)
		{
			return deformedMesh != null;
		}
		deformedMeshFilter = MeshFilter;
		if (deformedMeshFilter == null)
		{
			meshCache = null;
			deformedMesh = null;
			deformedVertices = null;
			deformedColors = null;
			return false;
		}
		meshCache = MeshCache.GetMeshCache(deformedMeshFilter.sharedMesh);
		deformedMesh = deformedMeshFilter.mesh;
		deformedVertices = deformedMesh.vertices;
		deformedColors = deformedMesh.colors;
		return true;
	}

	private void OnCollisionEnter(Collision col)
	{
		ProcessCollision(col);
	}

	private void OnCollisionStay(Collision col)
	{
		ProcessCollision(col);
	}

	private void ProcessCollision(Collision col)
	{
		int num = col.contacts.Length;
		for (int i = 0; i < num; i++)
		{
			ContactPoint contactPoint = col.contacts[i];
			ImpactDeformable impactDeformable = null;
			for (int j = 0; j < ColliderXDeformablesCache.Count; j++)
			{
				ColliderXDeformable colliderXDeformable = ColliderXDeformablesCache[j];
				if (colliderXDeformable.Collider == contactPoint.thisCollider)
				{
					impactDeformable = colliderXDeformable.Deformable;
					break;
				}
			}
			if (impactDeformable == null)
			{
				impactDeformable = (contactPoint.thisCollider.GetComponent<ImpactDeformable>() ?? this);
				ColliderXDeformablesCache.Add(new ColliderXDeformable
				{
					Collider = contactPoint.thisCollider,
					Deformable = impactDeformable
				});
			}
			impactDeformable.ProcessContactPoint(contactPoint.point, col.relativeVelocity, contactPoint.normal, col);
		}
	}

	private void ProcessContactPoint(Vector3 point, Vector3 relativeVelocity, Vector3 normal, Collision col)
	{
		if (UpdateMeshFilter() && !((double)(relativeVelocity.sqrMagnitude / Mathf.Max(Hardness, 0.01f)) < 0.25))
		{
			float num = Vector3.Dot(relativeVelocity.normalized, normal);
			if (!(num <= 0f))
			{
				Vector3 force = num * relativeVelocity.magnitude * normal * 0.02f;
				LastContact = col;
				Deform(point, force);
			}
		}
	}

	public void Deform(Vector3 point, Vector3 force)
	{
		if (!UpdateMeshFilter())
		{
			return;
		}
		point = MeshFilter.transform.InverseTransformPoint(point);
		if (Hardness < 0.01f)
		{
			Hardness = 0.01f;
		}
		RandomFactorDeformation = Mathf.Clamp01(RandomFactorDeformation);
		force = MeshFilter.transform.InverseTransformDirection(force) * (1f / Hardness);
		if (MeshFilter.transform.localScale != Vector3.one)
		{
			Vector3 localScale = MeshFilter.transform.localScale;
			force.Scale(new Vector3(1f / localScale.x, 1f / localScale.y, 1f / localScale.z));
		}
		float num = force.magnitude;
		if (MaxDeformationRadius > 0f)
		{
			num = Mathf.Min(num, MaxDeformationRadius);
		}
		if (num < 0.025f)
		{
			return;
		}
		num *= num;
		int num2 = deformedVertices.Length - 1;
		int num3 = 0;
		bool flag = false;
		Vector3 scale = MeshFilter.transform.InverseTransformVector(DeformationsScale);
		if (ColorDeformationDistance > 0f)
		{
			PrepareVertexColorArray();
		}
		while (num2 > 0)
		{
			num3 = num2;
			float sqrMagnitude = (deformedVertices[num3] - point).sqrMagnitude;
			if (sqrMagnitude <= num)
			{
				flag = true;
				Vector3 vector = force * (1f - sqrMagnitude / num);
				if (RandomFactorDeformation > 0f)
				{
					vector = vector * (1f - RandomFactorDeformation) + vector * UnityEngine.Random.value * RandomFactorDeformation;
				}
				if (DeformationsScale != Vector3.one)
				{
					vector.Scale(scale);
				}
				deformedVertices[num3] += vector;
				if (LimitDeformationToMeshBounds && !meshCache.Bounds.Contains(deformedVertices[num3]))
				{
					deformedVertices[num3] = meshCache.Bounds.ClosestPoint(deformedVertices[num3]);
				}
				if (MaxVertexMov > 0f || ColorDeformationDistance > 0f)
				{
					Vector3 a = deformedVertices[num3] - meshCache.Vertices[num3];
					float sqrMagnitude2 = a.sqrMagnitude;
					if (MaxVertexMov > 0f && sqrMagnitude2 > MaxVertexMov * MaxVertexMov)
					{
						deformedVertices[num3] = meshCache.Vertices[num3] + a * (MaxVertexMov / a.magnitude);
					}
					if (ColorDeformationDistance > 0f)
					{
						Color deformationColor = DeformedColor;
						bool flag2 = true;
						if (this.OnGetDeformationColor != null)
						{
							flag2 = this.OnGetDeformationColor(this, ref deformationColor);
						}
						if (flag2)
						{
							Color a2 = Color.white;
							if (savedColors != null)
							{
								a2 = savedColors[num3];
							}
							else if (meshCache.HasVertexColor)
							{
								a2 = meshCache.Colors[num3];
							}
							float t = Mathf.Clamp01(sqrMagnitude2 / (ColorDeformationDistance * ColorDeformationDistance)) * DeformedColor.a;
							deformedColors[num3] = Color.Lerp(a2, deformationColor, t);
						}
					}
				}
			}
			num2--;
		}
		if (flag)
		{
			if (Application.isEditor)
			{
				ImmediateApplyChanges();
			}
			else
			{
				ApplyChangesToMesh();
			}
			if (this.OnDeformForce != null)
			{
				this.OnDeformForce(this, point, force);
			}
		}
	}

	private void ApplyChangesToMesh()
	{
		if (!applyingChanges)
		{
			applyingChanges = true;
			StartCoroutine(DeferredApplyChanges());
		}
	}

	private IEnumerator DeferredApplyChanges()
	{
		yield return new WaitForEndOfFrame();
		applyingChanges = false;
		ImmediateApplyChanges();
	}

	private void ImmediateApplyChanges()
	{
		structuralDamage = -1f;
		deformedMesh.MarkDynamic();
		deformedMesh.vertices = deformedVertices;
		if (ColorDeformationDistance > 0f)
		{
			deformedMesh.colors = deformedColors;
		}
		deformedMesh.RecalculateBounds();
		if (DeformMeshCollider)
		{
			if (meshCollider != null)
			{
				meshCollider.sharedMesh = null;
				meshCollider.sharedMesh = deformedMesh;
				meshCollider.sharedMesh.UploadMeshData(markNoLongerReadable: false);
			}
		}
		else if (meshCollider != null && meshCollider.sharedMesh == deformedMesh)
		{
			meshCollider.sharedMesh = null;
			Mesh mesh = new Mesh();
			mesh.vertices = meshCache.Vertices;
			meshCollider.sharedMesh = mesh;
		}
		if (RecalculateNormals)
		{
			deformedMesh.RecalculateNormals();
		}
		deformedMesh.UploadMeshData(markNoLongerReadable: false);
		if (this.OnDeform != null)
		{
			this.OnDeform(this);
		}
	}

	public void Repair(float percentual, Vector3? point = default(Vector3?), float? radius = default(float?))
	{
		if (!UpdateMeshFilter())
		{
			return;
		}
		bool flag = false;
		Vector3 b = Vector3.zero;
		float num = 0f;
		if (point.HasValue && radius.HasValue)
		{
			num = radius.Value;
			num *= num;
			b = MeshFilter.transform.InverseTransformPoint(point.Value);
		}
		if (ColorDeformationDistance > 0f)
		{
			PrepareVertexColorArray();
		}
		for (int i = 0; i < deformedVertices.Length; i++)
		{
			if (!(deformedVertices[i] - meshCache.Vertices[i] != Vector3.zero) || (num > 0f && (deformedVertices[i] - b).sqrMagnitude >= num))
			{
				continue;
			}
			deformedVertices[i] = Vector3.Lerp(deformedVertices[i], meshCache.Vertices[i], percentual);
			if (ColorDeformationDistance > 0f)
			{
				Color b2 = Color.white;
				if (savedColors != null)
				{
					b2 = savedColors[i];
				}
				else if (meshCache.HasVertexColor)
				{
					b2 = meshCache.Colors[i];
				}
				deformedColors[i] = Color.Lerp(deformedColors[i], b2, percentual);
			}
			flag = true;
		}
		if (flag)
		{
			ImmediateApplyChanges();
		}
	}

	private void PrepareVertexColorArray()
	{
		if (deformedColors == null || deformedColors.Length == 0)
		{
			deformedColors = (from v in deformedVertices
				select Color.white).ToArray();
		}
	}

	public void SetCurrentVertexColorsAsOriginal()
	{
		savedColors = deformedMesh.colors;
		if (savedColors == null || savedColors.Length != deformedMesh.vertices.Length)
		{
			savedColors = (from v in deformedMesh.vertices
				select Color.white).ToArray();
		}
		deformedColors = savedColors.ToArray();
	}
}
