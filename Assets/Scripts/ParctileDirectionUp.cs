using UnityEngine;

[ExecuteInEditMode]
public class ParctileDirectionUp : MonoBehaviour
{
	private void Update()
	{
		if (base.transform.eulerAngles != Vector3.zero)
		{
			base.transform.eulerAngles = Vector3.zero;
		}
	}
}
