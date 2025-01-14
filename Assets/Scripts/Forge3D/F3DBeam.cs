using UnityEngine;

namespace Forge3D
{
	[RequireComponent(typeof(LineRenderer))]
	public class F3DBeam : MonoBehaviour
	{
		public LayerMask layerMask;

		public F3DFXType fxType;

		public bool OneShot;

		public Texture[] BeamFrames;

		public float FrameStep;

		public float beamScale;

		public float MaxBeamLength;

		public bool AnimateUV;

		public float UVTime;

		public Transform rayImpact;

		public Transform rayMuzzle;

		private LineRenderer lineRenderer;

		private RaycastHit hitPoint;

		private RaycastHit2D hitPoint2D;

		private int frameNo;

		private int FrameTimerID;

		private float beamLength;

		private float initialBeamOffset;

		public float fxOffset;

		private float animateUVTime;

		private void Awake()
		{
			lineRenderer = GetComponent<LineRenderer>();
			if (!AnimateUV && BeamFrames.Length != 0)
			{
				lineRenderer.material.mainTexture = BeamFrames[0];
			}
			initialBeamOffset = UnityEngine.Random.Range(0f, 5f);
		}

		private void OnSpawned()
		{
			if (OneShot)
			{
				Raycast();
			}
			if (BeamFrames.Length > 1)
			{
				Animate();
			}
		}

		private void OnDespawned()
		{
			frameNo = 0;
			if (FrameTimerID != -1)
			{
				F3DTime.time.RemoveTimer(FrameTimerID);
				FrameTimerID = -1;
			}
		}

		private void Raycast()
		{
			hitPoint = default(RaycastHit);
			Ray ray = new Ray(base.transform.position, base.transform.forward);
			float x = MaxBeamLength * (beamScale / 10f);
			if (Physics.Raycast(ray, out hitPoint, MaxBeamLength, layerMask))
			{
				beamLength = Vector3.Distance(base.transform.position, hitPoint.point);
				lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));
				x = beamLength * (beamScale / 10f);
				switch (fxType)
				{
				case F3DFXType.Sniper:
					F3DFXController.instance.SniperImpact(hitPoint.point + hitPoint.normal * fxOffset);
					ApplyForce(4f);
					break;
				case F3DFXType.RailGun:
					F3DFXController.instance.RailgunImpact(hitPoint.point + hitPoint.normal * fxOffset);
					ApplyForce(7f);
					break;
				case F3DFXType.PlasmaBeam:
					ApplyForce(0.5f);
					break;
				case F3DFXType.PlasmaBeamHeavy:
					ApplyForce(2f);
					break;
				}
				if ((bool)rayImpact)
				{
					rayImpact.position = hitPoint.point - base.transform.forward * 0.5f;
				}
			}
			else
			{
				RaycastHit2D hit = Physics2D.Raycast(new Vector2(base.transform.position.x, base.transform.position.y), new Vector2(base.transform.forward.x, base.transform.forward.y), beamLength, layerMask);
				if ((bool)hit)
				{
					beamLength = Vector3.Distance(base.transform.position, hit.point);
					lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));
					x = beamLength * (beamScale / 10f);
					switch (fxType)
					{
					case F3DFXType.Sniper:
						F3DFXController.instance.SniperImpact(hit.point + hit.normal * fxOffset);
						ApplyForce(4f);
						break;
					case F3DFXType.RailGun:
						F3DFXController.instance.RailgunImpact(hit.point + hit.normal * fxOffset);
						ApplyForce(7f);
						break;
					case F3DFXType.PlasmaBeam:
						ApplyForce(0.5f);
						break;
					case F3DFXType.PlasmaBeamHeavy:
						ApplyForce(2f);
						break;
					}
					if ((bool)rayImpact)
					{
						rayImpact.position = new Vector3(hit.point.x, hit.point.y, base.gameObject.transform.position.z) - base.transform.forward * 0.5f;
					}
				}
				else
				{
					beamLength = MaxBeamLength;
					lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));
					if ((bool)rayImpact)
					{
						rayImpact.position = base.transform.position + base.transform.forward * beamLength;
					}
				}
			}
			if ((bool)rayMuzzle)
			{
				rayMuzzle.position = base.transform.position + base.transform.forward * 0.1f;
			}
			lineRenderer.material.SetTextureScale("_MainTex", new Vector2(x, 1f));
		}

		private void OnFrameStep()
		{
			lineRenderer.material.mainTexture = BeamFrames[frameNo];
			frameNo++;
			if (frameNo == BeamFrames.Length)
			{
				frameNo = 0;
			}
		}

		private void Animate()
		{
			if (BeamFrames.Length > 1)
			{
				frameNo = 0;
				lineRenderer.material.mainTexture = BeamFrames[frameNo];
				FrameTimerID = F3DTime.time.AddTimer(FrameStep, BeamFrames.Length - 1, OnFrameStep);
				frameNo = 1;
			}
		}

		private void ApplyForce(float force)
		{
			if (hitPoint.rigidbody != null)
			{
				hitPoint.rigidbody.AddForceAtPosition(base.transform.forward * force, hitPoint.point, ForceMode.VelocityChange);
			}
		}

		public void SetOffset(float offset)
		{
			fxOffset = offset;
		}

		private void Update()
		{
			if (AnimateUV)
			{
				animateUVTime += Time.deltaTime;
				if (animateUVTime > 1f)
				{
					animateUVTime = 0f;
				}
				lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(animateUVTime * UVTime + initialBeamOffset, 0f));
			}
			if (!OneShot)
			{
				Raycast();
			}
		}
	}
}
