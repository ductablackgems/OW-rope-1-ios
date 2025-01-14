using UnityEngine;

public class RedSkeletonEffect : MonoBehaviour
{
	public PSMeshRendererUpdater meshUpdater;

	public GameObject EffectMesh;

	public ParticleSystem[] Fire;

	private Renderer RendererMesh;

	private ParticleSystem.MainModule main;

	private float Cas10 = 10f;

	private bool End;

	private AudioSource Audio;

	public float multip = 1f;

	public bool IsAcid;

	private void Awake()
	{
		Audio = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		if (meshUpdater != null && !IsAcid)
		{
			meshUpdater.enabled = true;
			meshUpdater.UpdateMeshEffect(EffectMesh);
		}
		if (EffectMesh != null)
		{
			RendererMesh = EffectMesh.GetComponent<Renderer>();
		}
		if (IsAcid)
		{
			End = true;
			return;
		}
		Fire[0].gravityModifier = -0.2f;
		main = Fire[0].main;
		Fire[0].startSize = 2f;
		Fire[1].startSize = 0.15f;
		Cas10 = 10f;
		End = false;
		Audio.volume = 1f;
	}

	private void destroyEffects()
	{
		RendererMesh.materials = new Material[1]
		{
			RendererMesh.materials[0]
		};
	}

	private void Update()
	{
		if (IsAcid)
		{
			return;
		}
		if (main.startSize.constant > 0.5f)
		{
			Fire[0].startSize -= 0.25f * Time.deltaTime * multip;
			Fire[1].startSize -= 0.01f * Time.deltaTime * multip;
			if (Audio.volume > 0f)
			{
				Audio.volume -= 0.16f * Time.deltaTime * multip;
			}
			if (Fire[0].gravityModifier < 0f)
			{
				Fire[0].gravityModifier += 0.015f * Time.deltaTime * multip;
			}
		}
		if (RendererMesh != null)
		{
			Cas10 -= Time.deltaTime;
			if (Cas10 <= 0f && !End)
			{
				RendererMesh.materials = new Material[1]
				{
					RendererMesh.materials[0]
				};
				meshUpdater.enabled = false;
				End = true;
			}
		}
	}
}
