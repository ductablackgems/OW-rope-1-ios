using FluffyUnderware.Curvy.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class SplinePathMeshBuilder : MonoBehaviour
{
	public enum MeshCapShape
	{
		Line,
		NGon,
		Rectangle,
		Custom
	}

	public enum MeshExtrusion
	{
		FixedF,
		FixedDistance,
		Adaptive
	}

	public enum MeshUV
	{
		StretchV,
		StretchVSegment,
		Absolute
	}

	public enum MeshScaleModifier
	{
		None,
		ControlPoint,
		UserValue,
		Delegate,
		AnimationCurve
	}

	public delegate Vector3 OnGetScaleEvent(SplinePathMeshBuilder sender, float tf);

	public CurvySplineBase Spline;

	public float FromTF;

	public float ToTF = 1f;

	public bool FastInterpolation;

	public bool UseWorldPosition;

	public MeshExtrusion Extrusion = MeshExtrusion.Adaptive;

	public float ExtrusionParameter = 1f;

	public MeshCapShape CapShape = MeshCapShape.NGon;

	public float CapWidth = 1f;

	public float CapHeight = 0.5f;

	public float CapHollow;

	public int CapSegments = 9;

	public bool StartCap = true;

	public Mesh StartMesh;

	public bool EndCap = true;

	public Mesh EndMesh;

	public MeshUV UV;

	public float UVParameter = 1f;

	public bool CalculateTangents;

	public MeshScaleModifier ScaleModifier;

	public int ScaleModifierUserValueSlot;

	public AnimationCurve ScaleModifierCurve;

	public bool AutoRefresh = true;

	public float AutoRefreshSpeed;

	public bool RefreshMeshCollider;

	private Mesh mMesh;

	private MeshFilter mMeshFilter;

	private MeshCollider mMeshCollider;

	private Transform mTransform;

	private Vector3[] mVerts;

	private Vector2[] mUV;

	private int[] mTris;

	private float mLastRefresh;

	private List<CurvyMeshSegmentInfo> mSegmentInfo = new List<CurvyMeshSegmentInfo>();

	private MeshInfo StartMeshInfo;

	private MeshInfo EndMeshInfo;

	public CurvySpline TargetSpline => Spline as CurvySpline;

	public CurvySplineGroup TargetSplineGroup => Spline as CurvySplineGroup;

	public int VertexCount
	{
		get
		{
			if (!mMesh)
			{
				return 0;
			}
			return mMesh.vertexCount;
		}
	}

	public int TriangleCount
	{
		get
		{
			if (mTris == null)
			{
				return 0;
			}
			return mTris.Length / 3;
		}
	}

	public Mesh Mesh => mMesh;

	public Transform Transform
	{
		get
		{
			if (!mTransform)
			{
				mTransform = base.transform;
			}
			return mTransform;
		}
	}

	private int SegmentSteps => mSegmentInfo.Count;

	public event OnGetScaleEvent OnGetScale;

	private void OnEnable()
	{
		mMeshFilter = GetComponent<MeshFilter>();
		mMesh = mMeshFilter.sharedMesh;
		if (!mMesh)
		{
			mMesh = new Mesh();
			mMesh.hideFlags = HideFlags.HideAndDontSave;
			mMesh.name = "CurvyMesh";
			mMeshFilter.sharedMesh = mMesh;
		}
		mMeshCollider = GetComponent<MeshCollider>();
		if (!Application.isPlaying)
		{
			if ((bool)Spline && !Spline.IsInitialized)
			{
				Spline.RefreshImmediately(refreshLength: true, refreshOrientation: true, skipIfInitialized: false);
			}
			Refresh();
		}
	}

	private void OnDisable()
	{
		if ((bool)Spline)
		{
			Spline.OnRefresh -= OnSplineRefresh;
		}
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(mMeshFilter.sharedMesh);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(mMeshFilter.sharedMesh, allowDestroyingAssets: true);
		}
		Resources.UnloadUnusedAssets();
	}

	private IEnumerator Start()
	{
		if ((bool)Spline)
		{
			while (!Spline.IsInitialized)
			{
				yield return 0;
			}
			Refresh();
		}
	}

	private void Update()
	{
		if ((bool)Spline && Spline.IsInitialized)
		{
			if (AutoRefresh)
			{
				Spline.OnRefresh -= OnSplineRefresh;
				Spline.OnRefresh += OnSplineRefresh;
			}
			else
			{
				Spline.OnRefresh -= OnSplineRefresh;
			}
		}
	}

	public static SplinePathMeshBuilder Create()
	{
		return new GameObject("CurvyMeshPath", typeof(SplinePathMeshBuilder)).GetComponent<SplinePathMeshBuilder>();
	}

	public Transform Detach()
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.position = base.transform.position;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		gameObject.name = "CurvyMeshPath_detached";
		meshFilter.sharedMesh = UnityEngine.Object.Instantiate(Mesh);
		meshFilter.sharedMesh.name = "CurvyMesh";
		meshFilter.sharedMesh.RecalculateBounds();
		meshRenderer.sharedMaterials = GetComponent<MeshRenderer>().sharedMaterials;
		return gameObject.transform;
	}

	public void Refresh()
	{
		StartMeshInfo = null;
		EndMeshInfo = null;
		if ((bool)mMesh)
		{
			mMesh.Clear();
		}
		if (!mMesh || !Spline || !Spline.IsInitialized || Spline.Length == 0f)
		{
			return;
		}
		BuildCaps();
		Prepare();
		if ((bool)StartMesh && StartMeshInfo != null && ToTF - FromTF != 0f)
		{
			mVerts = new Vector3[getTotalVertexCount()];
			mUV = new Vector2[mVerts.Length];
			mTris = new int[getTotalTriIndexCount()];
			Extrude();
			mMesh.vertices = mVerts;
			mMesh.uv = mUV;
			mMesh.triangles = mTris;
			mMesh.RecalculateNormals();
			if (CalculateTangents)
			{
				MeshHelper.CalculateMeshTangents(mMesh);
			}
			if (RefreshMeshCollider && (bool)mMeshCollider)
			{
				mMeshCollider.sharedMesh = null;
				mMeshCollider.sharedMesh = mMesh;
			}
		}
	}

	private void BuildCaps()
	{
		switch (CapShape)
		{
		case MeshCapShape.Line:
			StartMesh = MeshHelper.CreateLineMesh(CapWidth);
			break;
		case MeshCapShape.Rectangle:
			StartMesh = MeshHelper.CreateRectangleMesh(CapWidth, CapHeight, CapHollow);
			break;
		case MeshCapShape.NGon:
			StartMesh = MeshHelper.CreateNgonMesh(CapSegments, CapWidth, CapHollow);
			break;
		}
	}

	private void Prepare()
	{
		if (!Spline || !StartMesh || !(ExtrusionParameter > 0f))
		{
			return;
		}
		StartMeshInfo = new MeshInfo(StartMesh, calculateLoops: true, mirror: false);
		if ((bool)EndMesh)
		{
			EndMeshInfo = new MeshInfo(EndMesh, calculateLoops: false, mirror: true);
		}
		else
		{
			EndMeshInfo = new MeshInfo(StartMesh, calculateLoops: false, mirror: true);
		}
		float tf = FromTF;
		mSegmentInfo.Clear();
		FromTF = Mathf.Clamp01(FromTF);
		ToTF = Mathf.Max(FromTF, Mathf.Clamp01(ToTF));
		if (FromTF == ToTF)
		{
			return;
		}
		Vector3 scale;
		switch (Extrusion)
		{
		case MeshExtrusion.FixedF:
			for (; tf < ToTF; tf += ExtrusionParameter)
			{
				scale = getScale(tf);
				mSegmentInfo.Add(new CurvyMeshSegmentInfo(this, tf, scale));
			}
			break;
		case MeshExtrusion.FixedDistance:
		{
			float num = Spline.TFToDistance(FromTF);
			for (tf = Spline.DistanceToTF(num); tf < ToTF; tf = Spline.DistanceToTF(num))
			{
				scale = getScale(tf);
				mSegmentInfo.Add(new CurvyMeshSegmentInfo(this, tf, num, scale));
				num += ExtrusionParameter;
			}
			break;
		}
		case MeshExtrusion.Adaptive:
			while (tf < ToTF)
			{
				scale = getScale(tf);
				mSegmentInfo.Add(new CurvyMeshSegmentInfo(this, tf, scale));
				int direction = 1;
				Spline.MoveByAngle(ref tf, ref direction, ExtrusionParameter, CurvyClamping.Clamp, 0.005f);
			}
			break;
		}
		if (!Mathf.Approximately(tf, ToTF))
		{
			tf = ToTF;
		}
		scale = getScale(tf);
		mSegmentInfo.Add(new CurvyMeshSegmentInfo(this, tf, scale));
	}

	private void OnSplineRefresh(CurvySplineBase sender)
	{
		if (Time.realtimeSinceStartup - mLastRefresh > AutoRefreshSpeed)
		{
			mLastRefresh = Time.realtimeSinceStartup;
			Refresh();
		}
	}

	private int getTotalVertexCount()
	{
		int num = 0;
		if ((bool)StartMesh)
		{
			if (StartCap)
			{
				num += StartMesh.vertexCount;
			}
			if (EndCap)
			{
				num += (EndMesh ? EndMesh.vertexCount : StartMesh.vertexCount);
			}
			num = ((StartMeshInfo.LoopVertexCount <= 0) ? (num + SegmentSteps * StartMeshInfo.VertexCount) : (num + SegmentSteps * StartMeshInfo.LoopVertexCount));
		}
		return num;
	}

	private int getTotalTriIndexCount()
	{
		int num = 0;
		if ((bool)StartMesh)
		{
			if (StartCap)
			{
				num += StartMesh.triangles.Length;
			}
			if (EndCap)
			{
				num += (EndMesh ? EndMesh.triangles.Length : StartMesh.triangles.Length);
			}
			num = ((StartMeshInfo.LoopVertexCount != 0) ? (num + (SegmentSteps - 1) * StartMeshInfo.LoopTriIndexCount) : (num + (SegmentSteps - 1) * Mathf.Max(2, StartMeshInfo.VertexCount - 1) * 2 * 3));
		}
		return num;
	}

	private float getV(CurvyMeshSegmentInfo info, int step, int stepsTotal)
	{
		switch (UV)
		{
		case MeshUV.StretchVSegment:
			return (float)step * UVParameter;
		case MeshUV.Absolute:
			return info.Distance / UVParameter;
		default:
			return (float)step / (float)stepsTotal * UVParameter;
		}
	}

	private Vector3 getScale(float tf)
	{
		switch (ScaleModifier)
		{
		case MeshScaleModifier.ControlPoint:
			return Spline.InterpolateScale(tf);
		case MeshScaleModifier.UserValue:
			return Spline.InterpolateUserValue(tf, ScaleModifierUserValueSlot);
		case MeshScaleModifier.Delegate:
			if (this.OnGetScale == null)
			{
				return Vector3.one;
			}
			return this.OnGetScale(this, tf);
		case MeshScaleModifier.AnimationCurve:
		{
			Vector3 one = Vector3.one;
			if (ScaleModifierCurve != null)
			{
				return one * ScaleModifierCurve.Evaluate(tf);
			}
			return one;
		}
		default:
			return Vector3.one;
		}
	}

	private void Extrude()
	{
		int segmentSteps = SegmentSteps;
		int num = SegmentSteps - 1;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		if (StartMeshInfo.LoopVertexCount == 0)
		{
			for (int i = 0; i < segmentSteps; i++)
			{
				float v = getV(mSegmentInfo[i], i, segmentSteps);
				for (int j = 0; j < StartMeshInfo.VertexCount; j++)
				{
					Vector3[] array = mVerts;
					int num6 = num2 + j;
					CurvyMeshSegmentInfo curvyMeshSegmentInfo = mSegmentInfo[i];
					array[num6] = curvyMeshSegmentInfo.Matrix.MultiplyPoint3x4(StartMeshInfo.Vertices[j]);
					mUV[num2 + j] = new Vector2(StartMeshInfo.UVs[j].x, v);
				}
				num2 += StartMeshInfo.VertexCount;
			}
		}
		else
		{
			for (int k = 0; k < segmentSteps; k++)
			{
				float v2 = getV(mSegmentInfo[k], k, segmentSteps);
				for (int l = 0; l < StartMeshInfo.EdgeLoops.Length; l++)
				{
					EdgeLoop edgeLoop = StartMeshInfo.EdgeLoops[l];
					for (int m = 0; m < edgeLoop.vertexCount; m++)
					{
						Vector3[] array2 = mVerts;
						int num7 = num2 + m;
						CurvyMeshSegmentInfo curvyMeshSegmentInfo = mSegmentInfo[k];
						array2[num7] = curvyMeshSegmentInfo.Matrix.MultiplyPoint3x4(StartMeshInfo.EdgeVertices[edgeLoop.vertexIndex[m]]);
						mUV[num2 + m] = new Vector2(StartMeshInfo.EdgeUV[edgeLoop.vertexIndex[m]].x, v2);
					}
					num2 += edgeLoop.vertexCount;
				}
			}
		}
		if (StartCap)
		{
			StartMeshInfo.TRSVertices(mSegmentInfo[0].Matrix).CopyTo(mVerts, num2);
			StartMeshInfo.UVs.CopyTo(mUV, num2);
			num4 = num2;
			num2 += StartMeshInfo.VertexCount;
		}
		if (EndCap)
		{
			EndMeshInfo.TRSVertices(mSegmentInfo[segmentSteps - 1].Matrix).CopyTo(mVerts, num2);
			EndMeshInfo.UVs.CopyTo(mUV, num2);
			num5 = num2;
			num2 += EndMeshInfo.VertexCount;
		}
		if (StartMeshInfo.LoopVertexCount == 0)
		{
			int vertexCount = StartMeshInfo.VertexCount;
			for (int n = 0; n < num; n++)
			{
				int num8 = vertexCount * n;
				int num9 = vertexCount * (n + 1);
				for (int num10 = 0; num10 < StartMeshInfo.VertexCount - 1; num10++)
				{
					mTris[num3] = num8 + num10;
					mTris[num3 + 1] = num9 + num10;
					mTris[num3 + 2] = num9 + 1 + num10;
					mTris[num3 + 3] = num9 + 1 + num10;
					mTris[num3 + 4] = num8 + 1 + num10;
					mTris[num3 + 5] = num8 + num10;
					num3 += 6;
				}
			}
		}
		else
		{
			int loopVertexCount = StartMeshInfo.LoopVertexCount;
			for (int num11 = 0; num11 < num; num11++)
			{
				int num12 = loopVertexCount * num11;
				int num13 = loopVertexCount * (num11 + 1);
				for (int num14 = 0; num14 < StartMeshInfo.EdgeLoops.Length; num14++)
				{
					EdgeLoop edgeLoop2 = StartMeshInfo.EdgeLoops[num14];
					for (int num15 = 0; num15 < edgeLoop2.vertexCount; num15++)
					{
						mTris[num3] = num12 + edgeLoop2.vertexIndex[num15];
						mTris[num3 + 1] = num13 + edgeLoop2.vertexIndex[num15];
						mTris[num3 + 2] = num12 + edgeLoop2.vertexIndex[num15 + 1];
						mTris[num3 + 3] = num13 + edgeLoop2.vertexIndex[num15];
						mTris[num3 + 4] = num13 + edgeLoop2.vertexIndex[num15 + 1];
						mTris[num3 + 5] = num12 + edgeLoop2.vertexIndex[num15 + 1];
						num3 += 6;
					}
				}
			}
		}
		if (StartCap)
		{
			for (int num16 = 0; num16 < StartMeshInfo.Triangles.Length; num16++)
			{
				mTris[num3 + num16] = StartMeshInfo.Triangles[num16] + num4;
			}
			num3 += StartMeshInfo.Triangles.Length;
		}
		if (EndCap)
		{
			for (int num17 = 0; num17 < EndMeshInfo.Triangles.Length; num17++)
			{
				mTris[num3 + num17] = EndMeshInfo.Triangles[num17] + num5;
			}
		}
	}
}
