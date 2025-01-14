using System;
using UnityEngine;

public class EffectSettings : MonoBehaviour
{
	public enum EffectTypeEnum
	{
		Projectile,
		AOE,
		Other
	}

	public enum DeactivationEnum
	{
		Deactivate,
		DestroyAfterCollision,
		DestroyAfterTime,
		Nothing
	}

	[Tooltip("Type of the effect")]
	public EffectTypeEnum EffectType;

	[Tooltip("The radius of the collider is required to correctly calculate the collision point. For example, if the radius 0.5m, then the position of the collision is shifted on 0.5m relative motion vector.")]
	public float ColliderRadius = 0.2f;

	[Tooltip("The radius of the \"Area Of Damage (AOE)\"")]
	public float EffectRadius;

	[Tooltip("Get the position of the movement of the motion vector, and not to follow to the target.")]
	public bool UseMoveVector;

	[Tooltip("A projectile will be moved to the target (any object)")]
	public GameObject Target;

	[Tooltip("Motion vector for the projectile (eg Vector3.Forward)")]
	public Vector3 MoveVector = Vector3.forward;

	[Tooltip("The speed of the projectile")]
	public float MoveSpeed = 1f;

	[Tooltip("Should the projectile have move to the target, until the target not reaches?")]
	public bool IsHomingMove;

	[Tooltip("Distance flight of the projectile, after which the projectile is deactivated and call a collision event with a null value \"RaycastHit\"")]
	public float MoveDistance = 20f;

	[Tooltip("Allows you to smoothly activate / deactivate effects which have an indefinite lifetime")]
	public bool IsVisible = true;

	[Tooltip("Whether to deactivate or destroy the effect after a collision. Deactivation allows you to reuse the effect without instantiating, using \"effect.SetActive (true)\"")]
	public DeactivationEnum InstanceBehaviour = DeactivationEnum.Nothing;

	[Tooltip("Delay before deactivating effect. (For example, after effect, some particles must have time to disappear).")]
	public float DeactivateTimeDelay = 4f;

	[Tooltip("Delay before deleting effect. (For example, after effect, some particles must have time to disappear).")]
	public float DestroyTimeDelay = 10f;

	[Tooltip("Allows you to adjust the layers, which can interact with the projectile.")]
	public LayerMask LayerMask = -1;

	private GameObject[] active_key = new GameObject[100];

	private float[] active_value = new float[100];

	private GameObject[] inactive_Key = new GameObject[100];

	private float[] inactive_value = new float[100];

	private int lastActiveIndex;

	private int lastInactiveIndex;

	private int currentActiveGo;

	private int currentInactiveGo;

	private bool deactivatedIsWait;

	public event EventHandler<CollisionInfo> CollisionEnter;

	public event EventHandler<EventArgs> EffectDeactivated;

	private void Start()
	{
		if (InstanceBehaviour == DeactivationEnum.DestroyAfterTime)
		{
			UnityEngine.Object.Destroy(base.gameObject, DestroyTimeDelay);
		}
	}

	public void OnCollisionHandler(CollisionInfo e)
	{
		for (int i = 0; i < lastActiveIndex; i++)
		{
			Invoke("SetGoActive", active_value[i]);
		}
		for (int j = 0; j < lastInactiveIndex; j++)
		{
			Invoke("SetGoInactive", inactive_value[j]);
		}
		this.CollisionEnter?.Invoke(this, e);
		if (InstanceBehaviour == DeactivationEnum.Deactivate && !deactivatedIsWait)
		{
			deactivatedIsWait = true;
			Invoke("Deactivate", DeactivateTimeDelay);
		}
		if (InstanceBehaviour == DeactivationEnum.DestroyAfterCollision)
		{
			UnityEngine.Object.Destroy(base.gameObject, DestroyTimeDelay);
		}
	}

	public void OnEffectDeactivatedHandler()
	{
		this.EffectDeactivated?.Invoke(this, EventArgs.Empty);
	}

	public void Deactivate()
	{
		OnEffectDeactivatedHandler();
		base.gameObject.SetActive(value: false);
	}

	private void SetGoActive()
	{
		active_key[currentActiveGo].SetActive(value: false);
		currentActiveGo++;
		if (currentActiveGo >= lastActiveIndex)
		{
			currentActiveGo = 0;
		}
	}

	private void SetGoInactive()
	{
		inactive_Key[currentInactiveGo].SetActive(value: true);
		currentInactiveGo++;
		if (currentInactiveGo >= lastInactiveIndex)
		{
			currentInactiveGo = 0;
		}
	}

	public void OnEnable()
	{
		for (int i = 0; i < lastActiveIndex; i++)
		{
			active_key[i].SetActive(value: true);
		}
		for (int j = 0; j < lastInactiveIndex; j++)
		{
			inactive_Key[j].SetActive(value: false);
		}
		deactivatedIsWait = false;
	}

	public void OnDisable()
	{
		CancelInvoke("SetGoActive");
		CancelInvoke("SetGoInactive");
		CancelInvoke("Deactivate");
		currentActiveGo = 0;
		currentInactiveGo = 0;
	}

	public void RegistreActiveElement(GameObject go, float time)
	{
		active_key[lastActiveIndex] = go;
		active_value[lastActiveIndex] = time;
		lastActiveIndex++;
	}

	public void RegistreInactiveElement(GameObject go, float time)
	{
		inactive_Key[lastInactiveIndex] = go;
		inactive_value[lastInactiveIndex] = time;
		lastInactiveIndex++;
	}
}
