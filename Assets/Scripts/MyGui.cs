using System;
using UnityEngine;

public class MyGui : MonoBehaviour
{
	public enum GuiStat
	{
		Ball,
		Bottom,
		Middle,
		Top
	}

	public bool UseGui = true;

	public int CurrentPrefabNomber;

	private float UpdateInterval = 1f;

	public Light DirLight;

	public GameObject Target;

	public GameObject TopPosition;

	public GameObject MiddlePosition;

	public GameObject BottomPosition;

	public GameObject Plane1;

	public GameObject Plane2;

	public Material[] PlaneMaterials;

	public GuiStat[] GuiStats;

	public float[] Times;

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
		effectSettings.Target = Target;
		if (isHomingMove)
		{
			effectSettings.IsHomingMove = isHomingMove;
		}
		prefabSpeed = effectSettings.MoveSpeed;
		effectSettings.EffectDeactivated += effectSettings_EffectDeactivated;
		currentGo.transform.parent = base.transform;
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
		currentGo.transform.position = GetInstancePosition(GuiStats[current]);
		isReadyEffect = true;
	}

	private void OnGUI()
	{
		if (!UseGui)
		{
			return;
		}
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
		if (current <= 15)
		{
			GUI.Label(new Rect(10f, 152f, 225f, 30f), "Ball Speed " + (int)prefabSpeed + "m", guiStyleHeader);
			prefabSpeed = GUI.HorizontalSlider(new Rect(115f, 155f, 120f, 30f), prefabSpeed, 1f, 30f);
			isHomingMove = GUI.Toggle(new Rect(10f, 190f, 150f, 30f), isHomingMove, " Is Homing Move");
			effectSettings.MoveSpeed = prefabSpeed;
		}
	}

	private void Update()
	{
		if (anim != null)
		{
			anim.enabled = isHomingMove;
		}
		effectSettings.IsHomingMove = isHomingMove;
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			fps = frames;
			timeleft = UpdateInterval;
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
		if (GuiStats[current] == GuiStat.Middle)
		{
			Invoke("InstanceDefaulBall", 2f);
		}
		InstanceEffect(GetInstancePosition(GuiStats[current]));
	}
}
