using UnityEngine;

public class SetRandomStartPoint : MonoBehaviour
{
	public Vector3 RandomRange;

	public GameObject StartPointGo;

	public float Height = 10f;

	private EffectSettings effectSettings;

	private bool isInitialized;

	private Transform tRoot;

	private void GetEffectSettingsComponent(Transform tr)
	{
		Transform parent = tr.parent;
		if (parent != null)
		{
			effectSettings = parent.GetComponentInChildren<EffectSettings>();
			if (effectSettings == null)
			{
				GetEffectSettingsComponent(parent.transform);
			}
		}
	}

	private void Start()
	{
		GetEffectSettingsComponent(base.transform);
		if (effectSettings == null)
		{
			UnityEngine.Debug.Log("Prefab root or children have not script \"PrefabSettings\"");
		}
		tRoot = effectSettings.transform;
		InitDefaultVariables();
		isInitialized = true;
	}

	private void OnEnable()
	{
		if (isInitialized)
		{
			InitDefaultVariables();
		}
	}

	private void InitDefaultVariables()
	{
		if (GetComponent<ParticleSystem>() != null)
		{
			GetComponent<ParticleSystem>().Stop();
		}
		Vector3 position = effectSettings.Target.transform.position;
		Vector3 vector = new Vector3(position.x, Height, position.z);
		float num = UnityEngine.Random.Range(0f, RandomRange.x * 200f) / 100f - RandomRange.x;
		float num2 = UnityEngine.Random.Range(0f, RandomRange.y * 200f) / 100f - RandomRange.y;
		float num3 = UnityEngine.Random.Range(0f, RandomRange.z * 200f) / 100f - RandomRange.z;
		Vector3 position2 = new Vector3(vector.x + num, vector.y + num2, vector.z + num3);
		if (StartPointGo == null)
		{
			tRoot.position = position2;
		}
		else
		{
			StartPointGo.transform.position = position2;
		}
		if (GetComponent<ParticleSystem>() != null)
		{
			GetComponent<ParticleSystem>().Play();
		}
	}

	private void Update()
	{
	}
}
