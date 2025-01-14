using UnityEngine;

[ExecuteInEditMode]
public class ME_CustomPostEffectIgnore : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.layer = LayerMask.NameToLayer("CustomPostEffectIgnore");
	}
}
