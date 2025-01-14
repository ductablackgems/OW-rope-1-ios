using UnityEngine;

public class CarHull : MonoBehaviour
{
	public AudioSource Audio;

	public ImpactDeformable[] Bumpers;

	private void Awake()
	{
		GetComponent<ImpactDeformable>().OnDeformForce += CarHull_OnDeformForce;
		ImpactDeformable[] bumpers = Bumpers;
		for (int i = 0; i < bumpers.Length; i++)
		{
			bumpers[i].OnDeformForce += CarHull_OnDeformForce;
		}
	}

	private void OnDisable()
	{
		GetComponent<ImpactDeformable>().OnDeformForce -= CarHull_OnDeformForce;
		ImpactDeformable[] bumpers = Bumpers;
		for (int i = 0; i < bumpers.Length; i++)
		{
			bumpers[i].OnDeformForce += CarHull_OnDeformForce;
		}
	}

	private void CarHull_OnDeformForce(ImpactDeformable impactDeformable, Vector3 point, Vector3 force)
	{
		if (!Audio.isPlaying)
		{
			Audio.pitch = UnityEngine.Random.Range(0.2f, 1f);
			Audio.Play();
		}
		Audio.volume = Mathf.Max(force.magnitude * 5f, Audio.volume);
	}
}
