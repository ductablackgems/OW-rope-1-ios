using UnityEngine;

namespace MagicalFX
{
	public class FX_ElectroLine : MonoBehaviour
	{
		public LineRenderer LineRender;

		public bool RayCast;

		public float Length = 300f;

		public Transform StartObject;

		public Transform EndObject;

		public Vector3 EndPosition;

		public Vector3 StartPosition;

		public float DistancePerSegment = 0.5f;

		public float Noise = 0.5f;

		public float NoiseInterval = 0.05f;

		public bool Blending = true;

		private Vector3[] vertexTemps;

		private Vector3[] vertexTempsTarget;

		private Vector3[] vertexTempsCurrent;

		private int vertexCount;

		private float noiseIntervalTemp;

		public bool FixRotation;

		public bool Normal;

		public bool ParentFXstart = true;

		public bool ParentFXend = true;

		public GameObject FXStart;

		public GameObject FXEnd;

		private GameObject fxStart;

		private GameObject fxEnd;

		public bool KeepConnect;

		private void Start()
		{
			if ((bool)StartObject)
			{
				StartPosition = StartObject.transform.position;
			}
			if ((bool)EndObject)
			{
				EndPosition = EndObject.transform.position;
			}
			if (RayCast)
			{
				StartPosition = base.transform.position;
				if (Physics.Raycast(new Ray(base.transform.position, base.transform.forward), out RaycastHit hitInfo, Length))
				{
					EndPosition = hitInfo.point;
					vertexCount = (int)(hitInfo.distance / DistancePerSegment);
				}
			}
			else
			{
				vertexCount = (int)(Vector3.Distance(StartPosition, EndPosition) / DistancePerSegment);
			}
			if (LineRender == null)
			{
				LineRender = GetComponent<LineRenderer>();
				LineRender.SetVertexCount(vertexCount);
				vertexTemps = new Vector3[vertexCount];
				vertexTempsTarget = new Vector3[vertexCount];
				vertexTempsCurrent = new Vector3[vertexCount];
				for (int i = 0; i < vertexCount; i++)
				{
					vertexTemps[i] = StartPosition + base.transform.forward * DistancePerSegment * i;
					if (i == 0 && (bool)StartObject)
					{
						vertexTemps[i] = StartPosition;
					}
					if (i == vertexCount - 1 && (bool)EndObject)
					{
						vertexTemps[i] = EndPosition;
					}
					vertexTempsTarget[i] = vertexTemps[i];
					vertexTempsCurrent[i] = vertexTemps[i];
					LineRender.SetPosition(i, vertexTemps[i]);
					if (!EndObject && i == vertexCount - 1)
					{
						EndPosition = vertexTemps[i];
					}
				}
			}
			if (FXStart != null)
			{
				Quaternion rotation = base.transform.rotation;
				if (!FixRotation)
				{
					rotation = FXStart.transform.rotation;
				}
				fxStart = UnityEngine.Object.Instantiate(FXStart, StartPosition, rotation);
				if (Normal)
				{
					fxStart.transform.forward = base.transform.forward;
				}
				if (ParentFXstart)
				{
					fxStart.transform.parent = base.transform;
				}
			}
			if (FXEnd != null)
			{
				Quaternion rotation2 = base.transform.rotation;
				if (!FixRotation)
				{
					rotation2 = FXEnd.transform.rotation;
				}
				fxEnd = UnityEngine.Object.Instantiate(FXEnd, EndPosition, rotation2);
				if (Normal)
				{
					fxEnd.transform.forward = base.transform.forward;
				}
				if (ParentFXend)
				{
					fxEnd.transform.parent = base.transform;
				}
			}
		}

		private void UpdatePosition()
		{
			base.transform.forward = (EndPosition - StartPosition).normalized;
			for (int i = 0; i < vertexCount; i++)
			{
				vertexTemps[i] = StartPosition + base.transform.forward * DistancePerSegment * i;
			}
			if ((bool)fxStart)
			{
				fxStart.transform.position = StartPosition;
			}
			if ((bool)fxEnd)
			{
				fxEnd.transform.position = EndPosition;
			}
		}

		private void Update()
		{
			if ((bool)StartObject)
			{
				StartPosition = StartObject.transform.position;
			}
			if ((bool)EndObject)
			{
				EndPosition = EndObject.transform.position;
			}
			if (KeepConnect)
			{
				UpdatePosition();
			}
			if (LineRender == null)
			{
				return;
			}
			if (Time.time > noiseIntervalTemp + NoiseInterval)
			{
				noiseIntervalTemp = Time.time;
				if (Noise > 0f)
				{
					for (int i = 0; i < vertexCount; i++)
					{
						Vector3 b = new Vector3((float)UnityEngine.Random.Range(-100, 100) * Noise * base.transform.up.x, (float)UnityEngine.Random.Range(-100, 100) * Noise * base.transform.up.y, (float)UnityEngine.Random.Range(-100, 100) * Noise * base.transform.up.z);
						Vector3 b2 = new Vector3((float)UnityEngine.Random.Range(-100, 100) * Noise * base.transform.right.x, (float)UnityEngine.Random.Range(-100, 100) * Noise * base.transform.right.y, (float)UnityEngine.Random.Range(-100, 100) * Noise * base.transform.right.z);
						vertexTempsTarget[i] = vertexTemps[i] + b2 + b;
						if (!Blending)
						{
							LineRender.SetPosition(i, vertexTemps[i] + b2 + b);
						}
					}
				}
			}
			if (!Blending)
			{
				return;
			}
			for (int j = 0; j < vertexCount; j++)
			{
				if (j != 0 && j != vertexCount - 1)
				{
					vertexTempsCurrent[j] = Vector3.Lerp(vertexTempsCurrent[j], vertexTempsTarget[j], 0.5f);
					LineRender.SetPosition(j, vertexTempsCurrent[j]);
				}
			}
		}
	}
}
