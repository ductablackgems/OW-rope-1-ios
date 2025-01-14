using System.Collections;
using UnityEngine;

namespace com.ootii.Geometry
{
	public class RaycastHitInvDistanceComparer : IComparer
	{
		int IComparer.Compare(object rCompare1, object rCompare2)
		{
			RaycastHit raycastHit = (RaycastHit)rCompare2;
			RaycastHit raycastHit2 = (RaycastHit)rCompare1;
			if (raycastHit.distance > raycastHit2.distance)
			{
				return 1;
			}
			if (raycastHit.distance < raycastHit2.distance)
			{
				return -1;
			}
			return 0;
		}
	}
}
