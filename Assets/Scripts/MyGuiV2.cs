using System;
using UnityEngine;

public class MyGuiV2 : MonoBehaviour
{
	public enum GuiStat
	{
		Ball,
		Bottom,
		Middle,
		Top
	}

	public int CurrentPrefabNomber;

	public float UpdateInterval = 0.5f;

	public Light DirLight;

	public GameObject Target;

	public GameObject TopPosition;

	public GameObject MiddlePosition;

	public GameObject BottomPosition;

	public GameObject Plane1;

	public GameObject Plane2;

	public Material[] PlaneMaterials;

	public GuiStat[] GuiStats;

	public GameObject[] Prefabs;

	private float oldLightIntensity;

	private Color oldAmbientColor;

	private GameObject currentGo;

	private GameObject defaultBall;

	private bool isDay;

	private bool isHomingMove;

	private bool isDefaultPlaneTexture;

	private int current;

	private Animator anim;

	private float prefabSpeed = 4f;

	private EffectSettings effectSettings;

	private EffectSettings defaultBallEffectSettings;

	private bool isReadyEffect;

	private bool isReadyDefaulBall;

	private float accum;

	private int frames;

	private float timeleft;

	private float fps;

	private GUIStyle guiStyleHeader = new GUIStyle();

	private void Start()
	{
		oldAmbientColor = RenderSettings.ambientLight;
		oldLightIntensity = DirLight.intensity;
		anim = Target.GetComponent<Animator>();
		guiStyleHeader.fontSize = 14;
		guiStyleHeader.normal.textColor = new Color(1f, 1f, 1f);
		EffectSettings component = Prefabs[current].GetComponent<EffectSettings>();
		if (component != null)
		{
			prefabSpeed = component.MoveSpeed;
		}
		current = CurrentPrefabNomber;
		InstanceCurrent(GuiStats[CurrentPrefabNomber]);
	}

	private void InstanceEffect(Vector3 pos)
	{
		currentGo = UnityEngine.Object.Instantiate(Prefabs[current], pos, Prefabs[current].transform.rotation);
		effectSettings = currentGo.GetComponent<EffectSettings>();
		effectSettings.Target = GetTargetObject(GuiStats[current]);
		if (isHomingMove)
		{
			effectSettings.IsHomingMove = isHomingMove;
		}
		prefabSpeed = effectSettings.MoveSpeed;
		effectSettings.EffectDeactivated += effectSettings_EffectDeactivated;
		currentGo.transform.parent = base.transform;
	}

	private void InstanceEffectWithoutObjectPool()
	{
		currentGo = UnityEngine.Object.Instantiate(Prefabs[current], GetInstancePosition(GuiStats[current]), Prefabs[current].transform.rotation);
		effectSettings = currentGo.GetComponent<EffectSettings>();
		effectSettings.Target = GetTargetObject(GuiStats[current]);
		if (isHomingMove)
		{
			effectSettings.IsHomingMove = isHomingMove;
		}
		prefabSpeed = effectSettings.MoveSpeed;
		effectSettings.EffectDeactivated += effectSettings_EffectDeactivated;
		currentGo.transform.parent = base.transform;
	}

	private GameObject GetTargetObject(GuiStat stat)
	{
		switch (stat)
		{
		case GuiStat.Ball:
			return Target;
		case GuiStat.Bottom:
			return BottomPosition;
		case GuiStat.Top:
			return TopPosition;
		case GuiStat.Middle:
			return MiddlePosition;
		default:
			return base.gameObject;
		}
	}

	private void InstanceDefaulBall()
	{
		defaultBall = UnityEngine.Object.Instantiate(Prefabs[1], base.transform.position, Prefabs[1].transform.rotation);
		defaultBallEffectSettings = defaultBall.GetComponent<EffectSettings>();
		defaultBallEffectSettings.Target = Target;
		defaultBallEffectSettings.EffectDeactivated += defaultBall_EffectDeactivated;
		defaultBall.transform.parent = base.transform;
	}

	private void defaultBall_EffectDeactivated(object sender, EventArgs e)
	{
		defaultBall.transform.position = base.transform.position;
		isReadyDefaulBall = true;
	}

	private void effectSettings_EffectDeactivated(object sender, EventArgs e)
	{
		if (current == 15 || current == 16)
		{
			UnityEngine.Object.Destroy(effectSettings.gameObject);
			InstanceEffect(GetInstancePosition(GuiStats[current]));
		}
		else
		{
			currentGo.transform.position = GetInstancePosition(GuiStats[current]);
			isReadyEffect = true;
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f, 15f, 105f, 30f), "Previous Effect"))
		{
			ChangeCurrent(-1);
		}
		if (GUI.Button(new Rect(130f, 15f, 105f, 30f), "Next Effect"))
		{
			ChangeCurrent(1);
		}
		if (Prefabs[current] != null)
		{
			GUI.Label(new Rect(300f, 15f, 100f, 20f), "Prefab name is \"" + Prefabs[current].name + "\"  \r\nHold any mouse button that would move the camera", guiStyleHeader);
		}
		if (GUI.Button(new Rect(10f, 60f, 225f, 30f), "Day/Night"))
		{
			DirLight.intensity = ((!isDay) ? 0f : oldLightIntensity);
			RenderSettings.ambientLight = ((!isDay) ? new Color(0.1f, 0.1f, 0.1f) : oldAmbientColor);
			isDay = !isDay;
		}
		if (GUI.Button(new Rect(10f, 105f, 225f, 30f), "Change environment"))
		{
			if (isDefaultPlaneTexture)
			{
				Plane1.GetComponent<Renderer>().material = PlaneMaterials[0];
				Plane2.GetComponent<Renderer>().material = PlaneMaterials[0];
			}
			else
			{
				Plane1.GetComponent<Renderer>().material = PlaneMaterials[1];
				Plane2.GetComponent<Renderer>().material = PlaneMaterials[2];
			}
			isDefaultPlaneTexture = !isDefaultPlaneTexture;
		}
		if (current <= 40)
		{
			GUI.Label(new Rect(10f, 152f, 225f, 30f), "Ball Speed " + (int)prefabSpeed + "m", guiStyleHeader);
			prefabSpeed = GUI.HorizontalSlider(new Rect(115f, 155f, 120f, 30f), prefabSpeed, 1f, 30f);
			isHomingMove = GUI.Toggle(new Rect(10f, 190f, 150f, 30f), isHomingMove, " Is Homing Move");
			effectSettings.MoveSpeed = prefabSpeed;
		}
		if (current == 15 || current == 16)
		{
			GUIStyle gUIStyle = new GUIStyle();
			gUIStyle.fontSize = 18;
			gUIStyle.normal.textColor = new Color(0.8f, 0.1f, 0.05f);
			GUI.Label(new Rect(600f, 15f, 30f, 30f), "Effect worked only on Unity4!", gUIStyle);
		}
	}

	private void Update()
	{
		anim.enabled = isHomingMove;
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			fps = accum / (float)frames;
			timeleft = UpdateInterval;
			accum = 0f;
			frames = 0;
		}
		if (isReadyEffect)
		{
			isReadyEffect = false;
			currentGo.SetActive(value: true);
		}
		if (isReadyDefaulBall)
		{
			isReadyDefaulBall = false;
			defaultBall.SetActive(value: true);
		}
	}

	private void InstanceCurrent(GuiStat stat)
	{
		switch (stat)
		{
		case GuiStat.Ball:
			InstanceEffect(base.transform.position);
			break;
		case GuiStat.Bottom:
			InstanceEffect(BottomPosition.transform.position);
			break;
		case GuiStat.Top:
			InstanceEffect(TopPosition.transform.position);
			break;
		case GuiStat.Middle:
			MiddlePosition.SetActive(value: true);
			InstanceEffect(MiddlePosition.transform.position);
			break;
		}
	}

	private Vector3 GetInstancePosition(GuiStat stat)
	{
		switch (stat)
		{
		case GuiStat.Ball:
			return base.transform.position;
		case GuiStat.Bottom:
			return BottomPosition.transform.position;
		case GuiStat.Top:
			return TopPosition.transform.position;
		case GuiStat.Middle:
			return MiddlePosition.transform.position;
		default:
			return base.transform.position;
		}
	}

	private void ChangeCurrent(int delta)
	{
		UnityEngine.Object.Destroy(currentGo);
		UnityEngine.Object.Destroy(defaultBall);
		CancelInvoke("InstanceDefaulBall");
		current += delta;
		if (current > Prefabs.Length - 1)
		{
			current = 0;
		}
		else if (current < 0)
		{
			current = Prefabs.Length - 1;
		}
		if (effectSettings != null)
		{
			effectSettings.EffectDeactivated -= effectSettings_EffectDeactivated;
		}
		if (defaultBallEffectSettings != null)
		{
			defaultBallEffectSettings.EffectDeactivated -= effectSettings_EffectDeactivated;
		}
		MiddlePosition.SetActive(GuiStats[current] == GuiStat.Middle);
		InstanceEffect(GetInstancePosition(GuiStats[current]));
	}
}
