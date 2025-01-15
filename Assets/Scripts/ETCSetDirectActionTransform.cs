using UnityEngine;

[AddComponentMenu("EasyTouch Controls/Set Direct Action Transform ")]
public class ETCSetDirectActionTransform : MonoBehaviour
{
	public string axisName1;

	public string axisName2;

	private void Start()
	{
		if (!string.IsNullOrEmpty(axisName1))
		{
			ETCInput.SetAxisDirecTransform(axisName1, base.transform);
		}
		if (!string.IsNullOrEmpty(axisName2))
		{
			ETCInput.SetAxisDirecTransform(axisName2, base.transform);
		}
	}
}
