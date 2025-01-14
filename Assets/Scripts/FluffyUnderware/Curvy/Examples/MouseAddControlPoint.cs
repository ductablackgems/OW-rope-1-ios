using System.Collections;
using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class MouseAddControlPoint : MonoBehaviour
	{
		public bool RemoveUnusedSegments = true;

		private CurvySpline mSpline;

		private FollowSpline Walker;

		private IEnumerator Start()
		{
			mSpline = GetComponent<CurvySpline>();
			Walker = (UnityEngine.Object.FindObjectOfType(typeof(FollowSpline)) as FollowSpline);
			while (!mSpline.IsInitialized)
			{
				yield return null;
			}
		}

		private void Update()
		{
			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}
			Vector3 vector = UnityEngine.Input.mousePosition;
			vector.z = 10f;
			vector = Camera.main.ScreenToWorldPoint(vector);
			mSpline.Add(vector);
			if (!RemoveUnusedSegments)
			{
				return;
			}
			CurvySplineSegment currentSegment = Walker.CurrentSegment;
			if (!currentSegment)
			{
				return;
			}
			int num = currentSegment.ControlPointIndex - 2;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					mSpline.Delete(mSpline.ControlPoints[0], refreshSpline: false);
				}
				mSpline.RefreshImmediately();
			}
		}
	}
}
