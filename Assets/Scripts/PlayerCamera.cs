using App;
using App.Player;
using App.SaveSystem;
using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	private PlayerModel player;

	public LayerMask lineOfSightMask = 0;

	public float smoothTime = 0.15f;

	public float smoothRotate = 0.1f;

	public float xSpeed = 150f;

	public float ySpeed = 150f;

	public float yMinLimit = -40f;

	public float yMaxLimit = 60f;

	public float cameraDistance = 2.5f;

	public Vector3 targetOffset = Vector3.zero;

	public bool visibleMouseCursor = true;

	[NonSerialized]
	public bool controlled = true;

	[HideInInspector]
	public float x;

	[HideInInspector]
	public float y;

	[HideInInspector]
	public float z;

	[HideInInspector]
	public float xSmooth;

	[HideInInspector]
	public float ySmooth;

	[HideInInspector]
	public float zSmooth;

	private float xSmooth2;

	private float ySmooth2;

	private float distance = 10f;

	private float xVelocity;

	private float yVelocity;

	private float zVelocity;

	private float xSmooth2Velocity;

	private float ySmooth2Velocity;

	private Vector3 posVelocity = Vector3.zero;

	private float distanceVelocity;

	private Vector3 targetPos;

	private Quaternion rotation;

	public UISlider SensitivityAxisY;

	public UISlider SensitivityAxisX;

	public UISlider sightofviewSlider;

	private Camera Cam;

	private SettingsSaveEntity settingsSave;

	private void Awake()
	{
		player = ServiceLocator.GetPlayerModel();
		settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
	}

	private void Start()
	{
		if (visibleMouseCursor)
		{
			Cursor.visible = true;
		}
		else
		{
			Cursor.visible = false;
		}
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		if (SensitivityAxisY != null)
		{
			SensitivityAxisX.value = settingsSave.screenSensitivyX;
			SensitivityAxisY.value = settingsSave.screenSensitivyY;
		}
		xSpeed = settingsSave.screenSensitivyX * 400f;
		ySpeed = settingsSave.screenSensitivyY * 400f;
		Cam = ServiceLocator.GetGameObject("MainCamera").GetComponent<Camera>();
		if (sightofviewSlider != null)
		{
			sightofviewSlider.value = settingsSave.sightSetting;
		}
		Cam.farClipPlane = settingsSave.sightSetting * 500f;
		if (Cam.farClipPlane < 50f)
		{
			Cam.farClipPlane = 50f;
		}
	}

	public void SensitivityTouchpad()
	{
		if (SensitivityAxisX.value < 0.15f)
		{
			SensitivityAxisX.value = 0.15f;
			return;
		}
		if (SensitivityAxisY.value < 0.15f)
		{
			SensitivityAxisY.value = 0.15f;
			return;
		}
		ySpeed = SensitivityAxisY.value * 400f;
		xSpeed = SensitivityAxisX.value * 400f;
		settingsSave.screenSensitivyX = SensitivityAxisX.value;
		settingsSave.screenSensitivyY = SensitivityAxisY.value;
		settingsSave.Save();
	}

	public void SightofviewEvent()
	{
		Cam.farClipPlane = sightofviewSlider.value * 500f;
		if (sightofviewSlider.value < 0.1f)
		{
			sightofviewSlider.value = 0.1f;
		}
		settingsSave.sightSetting = sightofviewSlider.value;
		settingsSave.Save();
	}
}
