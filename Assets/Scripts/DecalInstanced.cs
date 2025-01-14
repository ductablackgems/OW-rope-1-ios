using ch.sycoforge.Decal;
using UnityEngine;

public class DecalInstanced : MonoBehaviour
{
	public EasyDecal DecalPrefab;

	public ParticleSystem Fire;

	public Transform Exlosion;

	private ParticleSystem.MainModule main;

	private bool End;

	private AudioSource Audio;

	private void Start()
	{
		Audio = GetComponent<AudioSource>();
	}

	public void Ignition()
	{
		Fire.gravityModifier = -0.1f;
		main = Fire.main;
		main.startSize = 1.5f;
		End = false;
		if (Physics.Raycast(base.transform.position, -Vector3.up, out RaycastHit hitInfo, 1f))
		{
			CreateMark(hitInfo);
		}
		else if (Physics.Raycast(base.transform.position, Vector3.forward, out hitInfo, 1f))
		{
			CreateMark(hitInfo);
		}
		else if (Physics.Raycast(base.transform.position, Vector3.right, out hitInfo, 1f))
		{
			CreateMark(hitInfo);
		}
		else if (Physics.Raycast(base.transform.position, Vector3.left, out hitInfo, 1f))
		{
			CreateMark(hitInfo);
		}
		else if (Physics.Raycast(base.transform.position, -Vector3.forward, out hitInfo, 1f))
		{
			CreateMark(hitInfo);
		}
	}

	private void CreateMark(RaycastHit hit)
	{
		EasyDecal.Project(DecalPrefab.gameObject, hit.point, hit.normal);
		Quaternion rotation = Quaternion.LookRotation(hit.normal);
		Exlosion.rotation = rotation;
	}

	private void Update()
	{
		if (main.startSize.constant > 0.5f)
		{
			Fire.startSize -= 0.05f * Time.deltaTime * 2f;
			if (Audio.volume > 0f)
			{
				Audio.volume -= 0.1f * Time.deltaTime;
			}
			if ((double)Fire.gravityModifier < -0.01)
			{
				Fire.gravityModifier += 0.02f * Time.deltaTime;
			}
		}
	}
}
