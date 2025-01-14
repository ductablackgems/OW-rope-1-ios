using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Car.Navigation.Modes.Curve
{
	public class AICarBoxSensor : MonoBehaviour
	{
		public delegate void AnalysisDoneEventHandler(BoxAnalysisResult result);

		public Rigidbody carRigidbody;

		public float maxZScaleMultiplier = 2.5f;

		public float maxXScaleMultiplier = 1.5f;

		public float maxCarSpeed = 18f;

		private bool analyzeContent;

		private bool contentAnalysisStarted;

		private HashSet<Transform> usedTransforms = new HashSet<Transform>();

		private BoxAnalysisResult boxAnalysisResult;

		private bool hit;

		private Collider[] result = new Collider[10];

		private TriggerMonitor trigger = new TriggerMonitor();

		private int layerMask = 15360;

		private Vector3 originalScale;

		private Vector3 maxScale;

		public event AnalysisDoneEventHandler OnAnalysisDone;

		public bool Hit()
		{
			return hit;
		}

		public void AnalyzeContent()
		{
			analyzeContent = true;
			usedTransforms.Clear();
		}

		private void Start()
		{
			originalScale = base.transform.parent.localScale;
			maxScale = originalScale;
			maxScale.z *= maxZScaleMultiplier;
			maxScale.x *= maxXScaleMultiplier;
		}

		private void FixedUpdate()
		{
			trigger.FixedUpdate();
			hit = trigger.IsTriggered();
			if (carRigidbody != null)
			{
				float magnitude = carRigidbody.velocity.magnitude;
				float t = (magnitude < 0.5f) ? 0.05f : (magnitude / maxCarSpeed);
				base.transform.parent.localScale = Vector3.Lerp(originalScale, maxScale, t);
			}
			if (!analyzeContent)
			{
				return;
			}
			if (contentAnalysisStarted)
			{
				analyzeContent = false;
				contentAnalysisStarted = false;
				if (this.OnAnalysisDone != null)
				{
					this.OnAnalysisDone(boxAnalysisResult);
				}
			}
			else
			{
				contentAnalysisStarted = true;
				boxAnalysisResult = default(BoxAnalysisResult);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (((1 << other.gameObject.layer) & layerMask) == 0)
			{
				return;
			}
			trigger.MarkTrigger(other);
			if (contentAnalysisStarted)
			{
				WhoIsResult whoIsResult = WhoIs.Resolve(other, WhoIs.Masks.AICarBoxSensor, usedTransforms);
				if (whoIsResult.IsEmpty)
				{
					boxAnalysisResult.MarkUnknown();
				}
				else if (whoIsResult.Compare(WhoIs.Masks.AllVehicles))
				{
					boxAnalysisResult.AddVehicle(whoIsResult.transform);
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = (hit ? Color.blue : Color.red);
			Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix *= matrix4x;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			Gizmos.matrix = matrix;
		}
	}
}
