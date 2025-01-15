using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSPlayerControl : MonoBehaviour
{
	public AudioClip gunSound;

	public AudioClip reload;

	public AudioClip needReload;

	public ParticleSystem shellParticle;

	public GameObject muzzleEffect;

	public GameObject impactEffect;

	public Text armoText;

	private bool inFire;

	private bool inReload;

	private Animator anim;

	private int armoCount = 30;

	private AudioSource audioSource;

	private void Awake()
	{
		anim = GetComponentInChildren<Animator>();
		audioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (ETCInput.GetButton("Fire") && !inFire && armoCount > 0 && !inReload)
		{
			inFire = true;
			anim.SetBool("Shoot", value: true);
			InvokeRepeating("GunFire", 0.12f, 0.12f);
			GunFire();
		}
		if (ETCInput.GetButtonDown("Fire") && armoCount == 0 && !inReload)
		{
			audioSource.PlayOneShot(needReload, 1f);
		}
		if (ETCInput.GetButtonUp("Fire"))
		{
			anim.SetBool("Shoot", value: false);
			muzzleEffect.SetActive(value: false);
			inFire = false;
			CancelInvoke();
		}
		if (ETCInput.GetButtonDown("Reload"))
		{
			inReload = true;
			audioSource.PlayOneShot(reload, 1f);
			anim.SetBool("Reload", value: true);
			StartCoroutine(Reload());
		}
		if (ETCInput.GetButtonDown("Back"))
		{
			base.transform.Rotate(Vector3.up * 180f);
		}
		armoText.text = armoCount.ToString();
	}

	public void MoveStart()
	{
		anim.SetBool("Move", value: true);
	}

	public void MoveStop()
	{
		anim.SetBool("Move", value: false);
	}

	public void GunFire()
	{
		if (armoCount > 0)
		{
			muzzleEffect.transform.Rotate(Vector3.forward * UnityEngine.Random.Range(0f, 360f));
			muzzleEffect.transform.localScale = new Vector3(UnityEngine.Random.Range(0.1f, 0.2f), UnityEngine.Random.Range(0.1f, 0.2f), 1f);
			muzzleEffect.SetActive(value: true);
			StartCoroutine(Flash());
			audioSource.PlayOneShot(gunSound, 1f);
			shellParticle.Emit(1);
			Vector3 a = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
			a += new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), 0f);
			RaycastHit[] array = Physics.RaycastAll(Camera.main.ScreenPointToRay(a));
			if (array.Length != 0)
			{
				Object.Instantiate(impactEffect, array[0].point - array[0].normal * -0.2f, Quaternion.identity);
			}
		}
		else
		{
			anim.SetBool("Shoot", value: false);
			muzzleEffect.SetActive(value: false);
			inFire = false;
		}
		armoCount--;
		if (armoCount < 0)
		{
			armoCount = 0;
		}
	}

	public void TouchPadSwipe(bool value)
	{
		ETCInput.SetControlSwipeIn("FreeLookTouchPad", value);
	}

	private IEnumerator Flash()
	{
		yield return new WaitForSeconds(0.08f);
		muzzleEffect.SetActive(value: false);
	}

	private IEnumerator Reload()
	{
		yield return new WaitForSeconds(0.5f);
		armoCount = 30;
		inReload = false;
		anim.SetBool("Reload", value: false);
	}
}
