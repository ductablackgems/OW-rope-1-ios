using UnityEngine;

public class ElectrifiedSkeletonEffect : MonoBehaviour
{
	[SerializeField]
	private GameObject effectMesh;

	[SerializeField]
	private PSMeshRendererUpdater meshUpdater;

	[SerializeField]
	private float effectDuration = 5f;

	private float duration;

	private void OnEnable()
	{
		duration = effectDuration;
		meshUpdater.gameObject.SetActive(value: true);
		meshUpdater.enabled = true;
		meshUpdater.UpdateMeshEffect(effectMesh);
	}

	private void Update()
	{
		if (!(duration <= 0f))
		{
			duration -= Time.deltaTime;
			if (!(duration > 0f))
			{
				meshUpdater.Clear();
				meshUpdater.enabled = false;
				meshUpdater.gameObject.SetActive(value: false);
			}
		}
	}
}
