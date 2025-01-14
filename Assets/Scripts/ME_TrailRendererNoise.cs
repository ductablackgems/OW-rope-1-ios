using System.Collections.Generic;
using UnityEngine;

public class ME_TrailRendererNoise : MonoBehaviour
{
	[Range(0.01f, 10f)]
	public float MinVertexDistance = 0.1f;

	public float VertexTime = 1f;

	public float TotalLifeTime = 3f;

	public bool SmoothCurves;

	public bool IsRibbon;

	[Range(0.001f, 10f)]
	public float Frequency = 1f;

	[Range(0.001f, 10f)]
	public float TimeScale = 0.1f;

	[Range(0.001f, 10f)]
	public float Amplitude = 1f;

	public float Gravity = 1f;

	public float TurbulenceStrength = 1f;

	public bool Autodestruct;

	private LineRenderer lineRenderer;

	private Transform t;

	private Vector3 prevPos;

	private List<Vector3> points = new List<Vector3>(500);

	private List<float> lifeTimes = new List<float>(500);

	private List<Vector3> velocities = new List<Vector3>(500);

	private float randomOffset;

	private List<Vector3> controlPoints = new List<Vector3>();

	private int curveCount;

	private const float MinimumSqrDistance = 0.01f;

	private const float DivisionThreshold = -0.99f;

	private const float SmoothCurvesScale = 0.5f;

	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.useWorldSpace = true;
		t = base.transform;
		prevPos = t.position;
		points.Insert(0, t.position);
		lifeTimes.Insert(0, VertexTime);
		velocities.Insert(0, Vector3.zero);
		randomOffset = (float)UnityEngine.Random.Range(0, 10000000) / 1000000f;
	}

	private void OnEnable()
	{
		points.Clear();
		lifeTimes.Clear();
		velocities.Clear();
		if (Autodestruct)
		{
			UnityEngine.Object.Destroy(base.gameObject, TotalLifeTime);
		}
	}

	private void Update()
	{
		AddNewPoints();
		UpdatetPoints();
		if (SmoothCurves && points.Count > 2)
		{
			UpdateLineRendererBezier();
		}
		else
		{
			UpdateLineRenderer();
		}
	}

	private void AddNewPoints()
	{
		if ((t.position - prevPos).magnitude > MinVertexDistance || (IsRibbon && points.Count == 0) || (IsRibbon && points.Count > 0 && (t.position - points[0]).magnitude > MinVertexDistance))
		{
			prevPos = t.position;
			points.Insert(0, t.position);
			lifeTimes.Insert(0, VertexTime);
			velocities.Insert(0, Vector3.zero);
		}
	}

	private void UpdatetPoints()
	{
		int num = 0;
		while (true)
		{
			if (num < lifeTimes.Count)
			{
				List<float> list = lifeTimes;
				int index = num;
				list[index] -= Time.deltaTime;
				if (lifeTimes[num] <= 0f)
				{
					break;
				}
				CalculateTurbuelence(points[num], TimeScale, Frequency, Amplitude, Gravity, num);
				num++;
				continue;
			}
			return;
		}
		int count = lifeTimes.Count - num;
		lifeTimes.RemoveRange(num, count);
		points.RemoveRange(num, count);
		velocities.RemoveRange(num, count);
	}

	private void UpdateLineRendererBezier()
	{
		if (SmoothCurves && points.Count > 2)
		{
			InterpolateBezier(points, 0.5f);
			List<Vector3> drawingPoints = GetDrawingPoints();
			lineRenderer.positionCount = drawingPoints.Count - 1;
			lineRenderer.SetPositions(drawingPoints.ToArray());
		}
	}

	private void UpdateLineRenderer()
	{
		lineRenderer.positionCount = Mathf.Clamp(points.Count - 1, 0, int.MaxValue);
		lineRenderer.SetPositions(points.ToArray());
	}

	private void CalculateTurbuelence(Vector3 position, float speed, float scale, float height, float gravity, int index)
	{
		float num = Time.timeSinceLevelLoad * speed + randomOffset;
		float x = position.x * scale + num;
		float num2 = position.y * scale + num + 10f;
		float y = position.z * scale + num + 25f;
		position.x = (Mathf.PerlinNoise(num2, y) - 0.5f) * height * Time.deltaTime;
		position.y = (Mathf.PerlinNoise(x, y) - 0.5f) * height * Time.deltaTime - gravity * Time.deltaTime;
		position.z = (Mathf.PerlinNoise(x, num2) - 0.5f) * height * Time.deltaTime;
		List<Vector3> list = points;
		list[index] += position * TurbulenceStrength;
	}

	public void InterpolateBezier(List<Vector3> segmentPoints, float scale)
	{
		controlPoints.Clear();
		if (segmentPoints.Count < 2)
		{
			return;
		}
		for (int i = 0; i < segmentPoints.Count; i++)
		{
			if (i == 0)
			{
				Vector3 vector = segmentPoints[i];
				Vector3 a = segmentPoints[i + 1] - vector;
				Vector3 item = vector + scale * a;
				controlPoints.Add(vector);
				controlPoints.Add(item);
			}
			else if (i == segmentPoints.Count - 1)
			{
				Vector3 b = segmentPoints[i - 1];
				Vector3 vector2 = segmentPoints[i];
				Vector3 a2 = vector2 - b;
				Vector3 item2 = vector2 - scale * a2;
				controlPoints.Add(item2);
				controlPoints.Add(vector2);
			}
			else
			{
				Vector3 b2 = segmentPoints[i - 1];
				Vector3 vector3 = segmentPoints[i];
				Vector3 a3 = segmentPoints[i + 1];
				Vector3 normalized = (a3 - b2).normalized;
				Vector3 item3 = vector3 - scale * normalized * (vector3 - b2).magnitude;
				Vector3 item4 = vector3 + scale * normalized * (a3 - vector3).magnitude;
				controlPoints.Add(item3);
				controlPoints.Add(vector3);
				controlPoints.Add(item4);
			}
		}
		curveCount = (controlPoints.Count - 1) / 3;
	}

	public List<Vector3> GetDrawingPoints()
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < curveCount; i++)
		{
			List<Vector3> list2 = FindDrawingPoints(i);
			if (i != 0)
			{
				list2.RemoveAt(0);
			}
			list.AddRange(list2);
		}
		return list;
	}

	private List<Vector3> FindDrawingPoints(int curveIndex)
	{
		List<Vector3> list = new List<Vector3>();
		Vector3 item = CalculateBezierPoint(curveIndex, 0f);
		Vector3 item2 = CalculateBezierPoint(curveIndex, 1f);
		list.Add(item);
		list.Add(item2);
		FindDrawingPoints(curveIndex, 0f, 1f, list, 1);
		return list;
	}

	private int FindDrawingPoints(int curveIndex, float t0, float t1, List<Vector3> pointList, int insertionIndex)
	{
		Vector3 a = CalculateBezierPoint(curveIndex, t0);
		Vector3 vector = CalculateBezierPoint(curveIndex, t1);
		if ((a - vector).sqrMagnitude < 0.01f)
		{
			return 0;
		}
		float num = (t0 + t1) / 2f;
		Vector3 vector2 = CalculateBezierPoint(curveIndex, num);
		Vector3 normalized = (a - vector2).normalized;
		Vector3 normalized2 = (vector - vector2).normalized;
		if (Vector3.Dot(normalized, normalized2) > -0.99f || Mathf.Abs(num - 0.5f) < 0.0001f)
		{
			int num2 = 0;
			num2 += FindDrawingPoints(curveIndex, t0, num, pointList, insertionIndex);
			pointList.Insert(insertionIndex + num2, vector2);
			num2++;
			return num2 + FindDrawingPoints(curveIndex, num, t1, pointList, insertionIndex + num2);
		}
		return 0;
	}

	public Vector3 CalculateBezierPoint(int curveIndex, float t)
	{
		int num = curveIndex * 3;
		Vector3 p = controlPoints[num];
		Vector3 p2 = controlPoints[num + 1];
		Vector3 p3 = controlPoints[num + 2];
		Vector3 p4 = controlPoints[num + 3];
		return CalculateBezierPoint(t, p, p2, p3, p4);
	}

	private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float num = 1f - t;
		float num2 = t * t;
		float num3 = num * num;
		float d = num3 * num;
		float d2 = num2 * t;
		return d * p0 + 3f * num3 * t * p1 + 3f * num * num2 * p2 + d2 * p3;
	}
}
