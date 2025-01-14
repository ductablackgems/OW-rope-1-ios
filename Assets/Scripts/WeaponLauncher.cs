using App;
using App.Spawn.Pooling;
using App.Weapons;
using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponLauncher : WeaponBase, IWeapon
{
	public string gunName;

	public GunType gunType;

	public bool isAutomat;

	public Transform leftHandPosition;

	public int impactParticlesDensity = 1;

	public bool OnActive;

	public bool OnScreenAiming;

	public Transform[] MissileOuter;

	public GameObject Missile;

	public float FireRate = 0.1f;

	public float Spread = 1f;

	public float ForceShoot = 8000f;

	public int NumBullet = 1;

	public int AmmoCommonReserve = 20;

	public int AmmoReserve = 20;

	public int Ammo = 10;

	public int AmmoMax = 10;

	public bool InfinityAmmo;

	public float ReloadTime = 1f;

	public bool ShowHUD = true;

	public int MaxAimRange = 10000;

	public bool ShowCrosshair;

	public Texture2D CrosshairTexture;

	public Texture2D TargetLockOnTexture;

	public Texture2D TargetLockedTexture;

	public float DistanceLock = 200f;

	public float TimeToLock = 2f;

	public float AimDirection = 0.8f;

	public bool Seeker;

	public GameObject Shell;

	public float ShellLifeTime = 4f;

	public Transform[] ShellOuter;

	public int ShellOutForce = 300;

	public GameObject Muzzle;

	public float MuzzleLifeTime = 2f;

	public AudioClip[] SoundGun;

	public AudioClip SoundReloading;

	public AudioClip SoundReloaded;

	public Animator weaponAnimator;

	public float range;

	private float timetolockcount;

	private float nextFireTime;

	private GameObject target;

	private Vector3 torqueTemp;

	private float reloadTimeTemp;

	private AudioSource audioSource;

	[HideInInspector]
	public bool Reloading;

	[HideInInspector]
	public float ReloadingProcess;

	private SmartPooler smartPooler;

	private readonly int shootAnimHash = Animator.StringToHash("shoot");

	[HideInInspector]
	public Vector3 AimPoint;

	[HideInInspector]
	public GameObject AimObject;

	public Camera CurrentCamera;

	private Vector3 crosshairPos;

	private int currentOuter;

	public float Range => range;

	public event Action OnShot;

	public event Action OnReload;

	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public GunType GetGunType()
	{
		return gunType;
	}

	public bool IsAutomat()
	{
		return isAutomat;
	}

	public Transform GetLeftHandPosition()
	{
		return leftHandPosition;
	}

	public void RegisterOnShot(Action OnShot)
	{

	}

	public void UnregisterOnShot(Action OnShot)
	{

	}

	public void RegisterOnReload(Action OnReload)
	{

	}

	public void UnregisterOnReload(Action OnReload)
	{

	}

	public void SetAmmoReserve(int ammoReserve)
	{
		AmmoReserve = ammoReserve;
	}

	public int GetAmmo()
	{
		return Ammo;
	}

	public int GetAmmoReserve()
	{
		return AmmoReserve;
	}

	public string GetGunName()
	{
		return gunName;
	}

	public void SetAmmo(int ammo)
	{
		Ammo = ammo;
	}

	public int GetAmmoCommonReserve()
	{
		return AmmoCommonReserve;
	}

	public bool Shooting()
	{
		return false;
	}

	public void SetOwner(GameObject owner)
	{
		Owner = owner;
	}

	private void Awake()
	{
		smartPooler = ServiceLocator.Get<SmartPooler>();
	}

	private void Start()
	{
		if (!Owner)
		{
			Owner = base.transform.root.gameObject;
		}
		if (!audioSource)
		{
			audioSource = GetComponent<AudioSource>();
			if (!audioSource)
			{
				base.gameObject.AddComponent<AudioSource>();
			}
		}
	}

	private void rayAiming()
	{
		RaycastHit hitInfo;
		if (OnScreenAiming)
		{
			if (!CurrentCamera)
			{
				return;
			}
			Ray ray = CurrentCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
			if (Physics.Raycast(ray, out hitInfo, MaxAimRange))
			{
				if (Missile != null && !hitInfo.collider.CompareTag(Missile.tag))
				{
					AimPoint = hitInfo.point;
					AimObject = hitInfo.collider.gameObject;
				}
			}
			else
			{
				AimPoint = ray.origin + ray.direction * MaxAimRange;
				AimObject = null;
			}
		}
		else if (Physics.Raycast(base.transform.position, base.transform.forward, out hitInfo, MaxAimRange))
		{
			if (Missile != null && !hitInfo.collider.CompareTag(Missile.tag))
			{
				AimPoint = hitInfo.point;
				AimObject = hitInfo.collider.gameObject;
			}
		}
		else
		{
			AimPoint = base.transform.position + base.transform.forward * MaxAimRange;
			AimObject = null;
		}
	}

	private void Update()
	{
		if (CurrentCamera == null)
		{
			CurrentCamera = Camera.main;
			if (CurrentCamera == null)
			{
				CurrentCamera = Camera.current;
			}
		}
		if (OnActive)
		{
			if ((bool)TorqueObject)
			{
				TorqueObject.transform.Rotate(torqueTemp * Time.deltaTime);
				torqueTemp = Vector3.Lerp(torqueTemp, Vector3.zero, Time.deltaTime);
			}
			if (Seeker)
			{
				for (int i = 0; i < TargetTag.Length; i++)
				{
					if (GameObject.FindGameObjectsWithTag(TargetTag[i]).Length == 0)
					{
						continue;
					}
					GameObject[] array = GameObject.FindGameObjectsWithTag(TargetTag[i]);
					float num = 2.14748365E+09f;
					if (AimObject != null && AimObject.tag == TargetTag[i])
					{
						float num2 = Vector3.Distance(AimObject.transform.position, base.transform.position);
						if (DistanceLock > num2 && num > num2 && timetolockcount + TimeToLock < Time.time)
						{
							num = num2;
							target = AimObject;
						}
						continue;
					}
					for (int j = 0; j < array.Length; j++)
					{
						if ((bool)array[j])
						{
							float num3 = Vector3.Dot((array[j].transform.position - base.transform.position).normalized, base.transform.forward);
							float num4 = Vector3.Distance(array[j].transform.position, base.transform.position);
							if (num3 >= AimDirection && DistanceLock > num4 && num > num4 && timetolockcount + TimeToLock < Time.time)
							{
								num = num4;
								target = array[j];
							}
						}
					}
				}
			}
			if ((bool)target)
			{
				float num5 = Vector3.Distance(base.transform.position, target.transform.position);
				float num6 = Vector3.Dot((target.transform.position - base.transform.position).normalized, base.transform.forward);
				if (num5 > DistanceLock || num6 <= AimDirection)
				{
					Unlock();
				}
			}
			if (Reloading)
			{
				ReloadingProcess = 1f / ReloadTime * (reloadTimeTemp + ReloadTime - Time.time);
				if (Time.time >= reloadTimeTemp + ReloadTime)
				{
					Reloading = false;
					if ((bool)SoundReloaded && (bool)audioSource)
					{
						audioSource.PlayOneShot(SoundReloaded);
					}
					if (AmmoMax > AmmoReserve)
					{
						Ammo = AmmoReserve;
					}
					else
					{
						Ammo = AmmoMax;
					}
					AmmoReserve -= Ammo;
				}
			}
			else if (Ammo <= 0)
			{
				Unlock();
				if (AmmoReserve > 0)
				{
					Reloading = true;
					reloadTimeTemp = Time.time;
					if ((bool)SoundReloading && (bool)audioSource)
					{
						audioSource.PlayOneShot(SoundReloading);
					}
					if (this.OnReload != null)
					{
						this.OnReload();
					}
				}
			}
		}
		SetShootAnimation(isShooting: false);
	}

	private void DrawTargetLockon(Transform aimtarget, bool locked)
	{
		if (!ShowHUD || !CurrentCamera || !(Vector3.Dot((aimtarget.position - CurrentCamera.transform.position).normalized, CurrentCamera.transform.forward) > 0.5f))
		{
			return;
		}
		Vector3 vector = CurrentCamera.WorldToScreenPoint(aimtarget.transform.position);
		float f = Vector3.Distance(base.transform.position, aimtarget.transform.position);
		if (locked)
		{
			if ((bool)TargetLockedTexture)
			{
				GUI.DrawTexture(new Rect(vector.x - (float)(TargetLockedTexture.width / 2), (float)Screen.height - vector.y - (float)(TargetLockedTexture.height / 2), TargetLockedTexture.width, TargetLockedTexture.height), TargetLockedTexture);
			}
			GUI.Label(new Rect(vector.x + 40f, (float)Screen.height - vector.y, 200f, 30f), aimtarget.name + " " + Mathf.Floor(f) + "m.");
		}
		else if ((bool)TargetLockOnTexture)
		{
			GUI.DrawTexture(new Rect(vector.x - (float)(TargetLockOnTexture.width / 2), (float)Screen.height - vector.y - (float)(TargetLockOnTexture.height / 2), TargetLockOnTexture.width, TargetLockOnTexture.height), TargetLockOnTexture);
		}
	}

	private void DrawCrosshair()
	{
		if (ShowCrosshair && (bool)CurrentCamera)
		{
			Vector3 a = CurrentCamera.WorldToScreenPoint(AimPoint);
			crosshairPos += (a - crosshairPos) / 5f;
			if ((bool)CrosshairTexture)
			{
				GUI.DrawTexture(new Rect(crosshairPos.x - (float)(CrosshairTexture.width / 2), (float)Screen.height - crosshairPos.y - (float)(CrosshairTexture.height / 2), CrosshairTexture.width, CrosshairTexture.height), CrosshairTexture);
			}
		}
	}

	private void OnGUI()
	{
		if (!OnActive)
		{
			return;
		}
		if (Seeker)
		{
			if ((bool)target)
			{
				DrawTargetLockon(target.transform, locked: true);
			}
			for (int i = 0; i < TargetTag.Length; i++)
			{
				if (GameObject.FindGameObjectsWithTag(TargetTag[i]).Length == 0)
				{
					continue;
				}
				GameObject[] array = GameObject.FindGameObjectsWithTag(TargetTag[i]);
				for (int j = 0; j < array.Length; j++)
				{
					if ((bool)array[j] && Vector3.Dot((array[j].transform.position - base.transform.position).normalized, base.transform.forward) >= AimDirection)
					{
						float num = Vector3.Distance(array[j].transform.position, base.transform.position);
						if (DistanceLock > num)
						{
							DrawTargetLockon(array[j].transform, locked: false);
						}
					}
				}
			}
		}
		DrawCrosshair();
	}

	private void Unlock()
	{
		timetolockcount = Time.time;
		target = null;
	}

	public void FocusMissileOuterTo(Vector3 targetPosition)
	{
		if (MissileOuter.Length != 0)
		{
			MissileOuter[currentOuter].transform.LookAt(targetPosition);
		}
	}

	public void Shoot()
	{
		if (InfinityAmmo)
		{
			Ammo = 1;
		}
		if (Ammo > 0 && Time.time > nextFireTime + FireRate)
		{
			rayAiming();
			nextFireTime = Time.time;
			torqueTemp = TorqueSpeedAxis;
			Ammo--;
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			if (MissileOuter.Length != 0)
			{
				rotation = MissileOuter[currentOuter].rotation;
				position = MissileOuter[currentOuter].position;
			}
			if (MissileOuter.Length != 0)
			{
				currentOuter++;
				if (currentOuter >= MissileOuter.Length)
				{
					currentOuter = 0;
				}
			}
			if ((bool)Muzzle)
			{
				Poolable poolable = default(Poolable);
				GameObject gameObject = smartPooler.Pop(Muzzle, out poolable);
				gameObject.transform.position = position;
				gameObject.transform.rotation = rotation;
				gameObject.transform.parent = base.transform;
				if (poolable.IsNew)
				{
					poolable.UpdatePushTime(MuzzleLifeTime);
				}
				if (MissileOuter.Length != 0)
				{
					gameObject.transform.parent = MissileOuter[currentOuter].transform;
				}
			}
			for (int i = 0; i < NumBullet; i++)
			{
				if (!Missile)
				{
					continue;
				}
				Vector3 b = new Vector3(UnityEngine.Random.Range(0f - Spread, Spread), UnityEngine.Random.Range(0f - Spread, Spread), UnityEngine.Random.Range(0f - Spread, Spread)) / 100f;
				Vector3 vector = base.transform.forward + b;
				if (MissileOuter.Length != 0)
				{
					vector = MissileOuter[currentOuter].forward + b;
				}
				GameObject gameObject2 = smartPooler.Pop(Missile, position, rotation);
				DamageBase component = gameObject2.GetComponent<DamageBase>();
				if ((bool)component)
				{
					component.smartPooler = smartPooler;
					component.Owner = Owner;
					component.TargetTag = TargetTag;
					component.IgnoreTag = IgnoreTag;
					component.emitImpactParticles = (i % impactParticlesDensity == 0);
					if (component.GetType() == typeof(RayShoot))
					{
						((RayShoot)component).ForceUpdate();
					}
				}
				WeaponBase component2 = gameObject2.GetComponent<WeaponBase>();
				if ((bool)component2)
				{
					component2.Owner = Owner;
					component2.Target = target;
					component2.TargetTag = TargetTag;
					component2.IgnoreTag = IgnoreTag;
				}
				gameObject2.transform.forward = vector;
				if (RigidbodyProjectile && (bool)gameObject2.GetComponent<Rigidbody>())
				{
					if (Owner != null && (bool)Owner.GetComponent<Rigidbody>())
					{
						gameObject2.GetComponent<Rigidbody>().velocity = Owner.GetComponent<Rigidbody>().velocity;
					}
					gameObject2.GetComponent<Rigidbody>().AddForce(vector * ForceShoot);
				}
			}
			if ((bool)Shell)
			{
				Transform transform = base.transform;
				if (ShellOuter.Length != 0)
				{
					transform = ShellOuter[currentOuter];
				}
				GameObject gameObject3 = UnityEngine.Object.Instantiate(Shell, transform.position, UnityEngine.Random.rotation);
				UnityEngine.Object.Destroy(gameObject3.gameObject, ShellLifeTime);
				if ((bool)gameObject3.GetComponent<Rigidbody>())
				{
					gameObject3.GetComponent<Rigidbody>().AddForce(transform.forward * ShellOutForce);
				}
			}
			if (SoundGun.Length != 0 && (bool)audioSource)
			{
				audioSource.PlayOneShot(SoundGun[UnityEngine.Random.Range(0, SoundGun.Length)]);
			}
			nextFireTime += FireRate;
			if (this.OnShot != null)
			{
				this.OnShot();
			}
		}
		SetShootAnimation(isShooting: true);
	}

	private void SetShootAnimation(bool isShooting)
	{
		if (!(weaponAnimator == null))
		{
			weaponAnimator.SetBool(shootAnimHash, isShooting);
		}
	}
}
