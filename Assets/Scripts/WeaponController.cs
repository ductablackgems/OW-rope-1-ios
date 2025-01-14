using App.Weapons;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
	public string[] TargetTag = new string[1]
	{
		"Enemy"
	};

	public IWeapon[] WeaponLists;

	public int CurrentWeapon;

	public bool ShowCrosshair;

	[Tooltip("If set to True, controller will Activate the Current Weapon and will Deactivate the others")]
	public bool HandleWeaponActivation;

	private void Awake()
	{
		if (base.transform.GetComponentsInChildren(typeof(IWeapon)).Length == 0)
		{
			return;
		}
		Component[] componentsInChildren = base.transform.GetComponentsInChildren(typeof(IWeapon));
		WeaponLists = new IWeapon[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			WeaponLists[i] = componentsInChildren[i].GetComponent<IWeapon>();
			if (WeaponLists[i].GetType() == typeof(WeaponLauncher))
			{
				((WeaponLauncher)WeaponLists[i]).TargetTag = TargetTag;
			}
		}
	}

	public IWeapon GetCurrentWeapon()
	{
		if (CurrentWeapon < WeaponLists.Length && WeaponLists[CurrentWeapon] != null)
		{
			return WeaponLists[CurrentWeapon];
		}
		return null;
	}

	private void Start()
	{
		for (int i = 0; i < WeaponLists.Length; i++)
		{
			if (WeaponLists[i] != null && WeaponLists[i].GetType() == typeof(WeaponLauncher))
			{
				WeaponLauncher obj = (WeaponLauncher)WeaponLists[i];
				obj.TargetTag = TargetTag;
				obj.ShowCrosshair = ShowCrosshair;
			}
		}
		ActivateCurrentWeapon();
	}

	private void Update()
	{
		for (int i = 0; i < WeaponLists.Length; i++)
		{
			if (WeaponLists[i] != null && WeaponLists[i].GetType() == typeof(WeaponLauncher))
			{
				((WeaponLauncher)WeaponLists[i]).OnActive = false;
			}
		}
		if (CurrentWeapon < WeaponLists.Length && WeaponLists[CurrentWeapon] != null && WeaponLists[CurrentWeapon].GetType() == typeof(WeaponLauncher))
		{
			((WeaponLauncher)WeaponLists[CurrentWeapon]).OnActive = true;
		}
	}

	public void LaunchWeapon(int index)
	{
		CurrentWeapon = index;
		if (CurrentWeapon < WeaponLists.Length && WeaponLists[index] != null)
		{
			WeaponLists[index].Shoot();
		}
	}

	public void SwitchWeapon()
	{
		CurrentWeapon++;
		if (CurrentWeapon >= WeaponLists.Length)
		{
			CurrentWeapon = 0;
		}
		ActivateCurrentWeapon();
	}

	public void SwitchToPreviousWeapon()
	{
		int num = CurrentWeapon - 1;
		CurrentWeapon = ((num < 0) ? (WeaponLists.Length - 1) : num);
		ActivateCurrentWeapon();
	}

	public void LaunchWeapon()
	{
		if (CurrentWeapon < WeaponLists.Length && WeaponLists[CurrentWeapon] != null)
		{
			WeaponLists[CurrentWeapon].Shoot();
		}
	}

	private void ActivateCurrentWeapon()
	{
		if (!HandleWeaponActivation)
		{
			return;
		}
		int num = WeaponLists.Length;
		for (int i = 0; i < num; i++)
		{
			IWeapon weapon = WeaponLists[i];
			if (weapon != null)
			{
				GameObject gameObject = weapon.GetGameObject();
				if (!(gameObject == null))
				{
					gameObject.SetActive(i == CurrentWeapon);
				}
			}
		}
	}
}
