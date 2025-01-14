using System.Collections;
using UnityEngine;

namespace com.ootii.Geometry
{
	public class RaycastHitDistanceComparer : IComparer
	{
		int IComparer.Compare(object rCompare1, object rCompare2)
		{
			RaycastHit raycastHit = (RaycastHit)rCompare1;
			RaycastHit raycastHit2 = (RaycastHit)rCompare2;
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
