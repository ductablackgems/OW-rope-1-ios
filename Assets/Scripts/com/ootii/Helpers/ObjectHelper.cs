using UnityEngine;

namespace com.ootii.Helpers
{
	public class ObjectHelper
	{
		public static float IsObjectVisible(Vector3 rPosition, Vector3 rForward, float rFOV, float rDistance, Transform rTarget)
		{
			Vector3 position = rTarget.transform.position;
			if (Mathf.Abs(NumberHelper.GetHorizontalAngle(rForward, position - rPosition)) < rFOV * 0.5f)
			{
				float num = Vector3.Distance(rPosition, position);
				if (num <= rDistance)
				{
					return num;
				}
			}
			return 0f;
		}

		public static GameObject IsObjectVisible(Vector3 rPosition, Vector3 rForward, float rFOV, float rDistance, LayerMask rTargetLayerMask, bool rClosest)
		{
			GameObject result = null;
			Collider[] array = Physics.OverlapSphere(rPosition, rDistance, rTargetLayerMask);
			if (array != null)
			{
				if (!rClosest)
				{
					return array[0].gameObject;
				}
				float num = float.MaxValue;
				for (int i = 0; i < array.Length; i++)
				{
					GameObject gameObject = array[i].gameObject;
					if (gameObject != null)
					{
						float num2 = IsObjectVisible(rPosition, rForward, rFOV, rDistance, gameObject.transform);
						if (num2 > 0f && num2 < num)
						{
							num = num2;
							result = gameObject;
						}
					}
				}
			}
			return result;
		}
	}
}
