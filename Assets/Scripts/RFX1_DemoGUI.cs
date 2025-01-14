using System;
using UnityEngine;

public class RFX1_DemoGUI : MonoBehaviour
{
	public int Current;

	public GameObject[] Prefabs;

	public bool[] IsShield;

	public GameObject ShieldProjectile;

	public GameObject ShieldProjectile2;

	public float ShieldProjectileReactiovationTime = 7f;

	public Light Sun;

	public ReflectionProbe ReflectionProbe;

	public Light[] NightLights = new Light[0];

	public Texture HUETexture;

	public bool UseMobileVersion;

	public GameObject MobileCharacter;

	public GameObject Target;

	public Color guiColor = Color.red;

	public RFX1_DistortionAndBloom RFX1_DistortionAndBloom;

	private int currentNomber;

	private GameObject currentInstance;

	private GUIStyle guiStyleHeader = new GUIStyle();

	private GUIStyle guiStyleHeaderMobile = new GUIStyle();

	private float dpiScale;

	private bool isDay;

	private float colorHUE;

	private float startSunIntensity;

	private Quaternion startSunRotation;

	private Color startAmbientLight;

	private float startAmbientIntencity;

	private float startReflectionIntencity;

	private LightShadows startLightShadows;

	private float currentSpeed = 1f;

	private GameObject mobileCharacterInstance;

	private bool isButtonPressed;

	private GameObject instanceShieldProjectile;

	private void Start()
	{
		if (Screen.dpi < 1f)
		{
			dpiScale = 1f;
		}
		if (Screen.dpi < 200f)
		{
			dpiScale = 1f;
		}
		else
		{
			dpiScale = Screen.dpi / 200f;
		}
		guiStyleHeader.fontSize = (int)(15f * dpiScale);
		guiStyleHeader.normal.textColor = guiColor;
		guiStyleHeaderMobile.fontSize = (int)(17f * dpiScale);
		ChangeCurrent(Current);
		startSunIntensity = Sun.intensity;
		startSunRotation = Sun.transform.rotation;
		startAmbientLight = RenderSettings.ambientLight;
		startAmbientIntencity = RenderSettings.ambientIntensity;
		startReflectionIntencity = RenderSettings.reflectionIntensity;
		startLightShadows = Sun.shadows;
		RFX1_DistortionAndBloom = Camera.main.GetComponent<RFX1_DistortionAndBloom>();
	}

	private void OnGUI()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyUp(KeyCode.RightArrow) || UnityEngine.Input.GetKeyUp(KeyCode.DownArrow))
		{
			isButtonPressed = false;
		}
		if (GUI.Button(new Rect(10f * dpiScale, 15f * dpiScale, 135f * dpiScale, 37f * dpiScale), "PREVIOUS EFFECT") || (!isButtonPressed && UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow)))
		{
			isButtonPressed = true;
			ChangeCurrent(-1);
		}
		if (GUI.Button(new Rect(160f * dpiScale, 15f * dpiScale, 135f * dpiScale, 37f * dpiScale), "NEXT EFFECT") || (!isButtonPressed && UnityEngine.Input.GetKeyDown(KeyCode.RightArrow)))
		{
			isButtonPressed = true;
			ChangeCurrent(1);
		}
		float num = 0f;
		if (GUI.Button(new Rect(10f * dpiScale, 63f * dpiScale + num, 285f * dpiScale, 37f * dpiScale), "Day / Night") || (!isButtonPressed && UnityEngine.Input.GetKeyDown(KeyCode.DownArrow)))
		{
			isButtonPressed = true;
			if (ReflectionProbe != null)
			{
				ReflectionProbe.RenderProbe();
			}
			Sun.intensity = ((!isDay) ? 0.05f : startSunIntensity);
			Sun.shadows = (isDay ? startLightShadows : LightShadows.None);
			Light[] nightLights = NightLights;
			for (int i = 0; i < nightLights.Length; i++)
			{
				nightLights[i].shadows = ((!isDay) ? startLightShadows : LightShadows.None);
			}
			Sun.transform.rotation = (isDay ? startSunRotation : Quaternion.Euler(350f, 30f, 90f));
			RenderSettings.ambientLight = ((!isDay) ? new Color(0.1f, 0.1f, 0.1f) : startAmbientLight);
			float num2 = (!UseMobileVersion) ? 1f : 0.2f;
			RenderSettings.ambientIntensity = (isDay ? startAmbientIntencity : num2);
			RenderSettings.reflectionIntensity = (isDay ? startReflectionIntencity : 0.2f);
			isDay = !isDay;
		}
		GUI.Label(new Rect(400f * dpiScale, 15f * dpiScale + num / 2f, 100f * dpiScale, 20f * dpiScale), "Prefab name is \"" + Prefabs[currentNomber].name + "\"  \r\nHold any mouse button that would move the camera", guiStyleHeader);
		if (!IsShield[currentNomber] && !UseMobileVersion)
		{
			GUI.Label(new Rect(12f * dpiScale, 110f * dpiScale + num, 50f * dpiScale, 20f * dpiScale), "Projectile Speed: " + Mathf.Round(currentSpeed * 10f) / 10f, guiStyleHeader);
			float num3 = currentSpeed;
			if (!UseMobileVersion)
			{
				currentSpeed = GUI.HorizontalSlider(new Rect(154f * dpiScale, 114f * dpiScale + num, 135f * dpiScale, 15f * dpiScale), currentSpeed, 0.1f, 10f);
			}
			if ((double)Math.Abs(num3 - currentSpeed) > 0.001)
			{
				RFX1_AnimatorEvents component = currentInstance.GetComponent<RFX1_AnimatorEvents>();
				if (component != null)
				{
					component.Speed = currentSpeed;
				}
			}
		}
		GUI.DrawTexture(new Rect(12f * dpiScale, 140f * dpiScale + num, 285f * dpiScale, 15f * dpiScale), HUETexture, ScaleMode.StretchToFill, alphaBlend: false, 0f);
		float num4 = colorHUE;
		colorHUE = GUI.HorizontalSlider(new Rect(12f * dpiScale, 147f * dpiScale + num, 285f * dpiScale, 15f * dpiScale), colorHUE, 0f, 360f);
		if (!((double)Mathf.Abs(num4 - colorHUE) > 0.001))
		{
			return;
		}
		RFX1_AnimatorEvents component2 = currentInstance.GetComponent<RFX1_AnimatorEvents>();
		if (component2 != null)
		{
			component2.HUE = colorHUE / 360f;
		}
		if (UseMobileVersion)
		{
			RFX1_EffectSettingColor rFX1_EffectSettingColor = currentInstance.GetComponent<RFX1_EffectSettingColor>();
			if (rFX1_EffectSettingColor == null)
			{
				rFX1_EffectSettingColor = currentInstance.AddComponent<RFX1_EffectSettingColor>();
			}
			RFX1_ColorHelper.HSBColor hsbColor = RFX1_ColorHelper.ColorToHSV(rFX1_EffectSettingColor.Color);
			hsbColor.H = colorHUE / 360f;
			rFX1_EffectSettingColor.Color = RFX1_ColorHelper.HSVToColor(hsbColor);
		}
	}

	private void ChangeCurrent(int delta)
	{
		currentSpeed = 1f;
		currentNomber += delta;
		if (currentNomber > Prefabs.Length - 1)
		{
			currentNomber = 0;
		}
		else if (currentNomber < 0)
		{
			currentNomber = Prefabs.Length - 1;
		}
		if (currentInstance != null)
		{
			UnityEngine.Object.Destroy(currentInstance);
			RemoveClones();
		}
		currentInstance = UnityEngine.Object.Instantiate(Prefabs[currentNomber]);
		RFX1_AnimatorEvents component = currentInstance.GetComponent<RFX1_AnimatorEvents>();
		if (component != null)
		{
			component.Target = Target;
		}
		RFX1_Target component2 = currentInstance.GetComponent<RFX1_Target>();
		if (component2 != null)
		{
			component2.Target = Target;
		}
		CancelInvoke("ReactivateShieldProjectile");
		if (IsShield[currentNomber])
		{
			if (currentNomber != 23)
			{
				InvokeRepeating("ReactivateShieldProjectile", 5f, ShieldProjectileReactiovationTime);
			}
			else
			{
				InvokeRepeating("ReactivateShieldProjectile", 3f, 3f);
			}
		}
		RFX1_TransformMotion componentInChildren = currentInstance.GetComponentInChildren<RFX1_TransformMotion>();
		if (componentInChildren != null)
		{
			currentSpeed = componentInChildren.Speed;
		}
		if (UseMobileVersion)
		{
			CancelInvoke("ReactivateEffect");
			componentInChildren = currentInstance.GetComponentInChildren<RFX1_TransformMotion>();
			if (componentInChildren != null)
			{
				componentInChildren.CollisionEnter += delegate
				{
					Invoke("ReactivateEffect", 3f);
				};
			}
		}
		if (mobileCharacterInstance != null)
		{
			UnityEngine.Object.Destroy(mobileCharacterInstance);
		}
		if (IsShield[currentNomber] && UseMobileVersion)
		{
			mobileCharacterInstance = UnityEngine.Object.Instantiate(MobileCharacter);
		}
	}

	private void RemoveClones()
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
		foreach (GameObject gameObject in array)
		{
			if (gameObject.name.Contains("(Clone)"))
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	private void ReactivateShieldProjectile()
	{
		if (instanceShieldProjectile != null)
		{
			UnityEngine.Object.Destroy(instanceShieldProjectile);
		}
		instanceShieldProjectile = ((currentNomber != 23) ? UnityEngine.Object.Instantiate(ShieldProjectile) : UnityEngine.Object.Instantiate(ShieldProjectile2));
		instanceShieldProjectile.SetActive(value: false);
		instanceShieldProjectile.SetActive(value: true);
	}

	private void ReactivateEffect()
	{
		currentInstance.SetActive(value: false);
		currentInstance.SetActive(value: true);
	}
}
