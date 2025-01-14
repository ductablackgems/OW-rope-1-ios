using UnityEngine;

public class RFX1_ShaderColorGradient : MonoBehaviour
{
	public RFX1_ShaderProperties ShaderColorProperty;

	public Gradient Color = new Gradient();

	public float TimeMultiplier = 1f;

	public bool IsLoop;

	public bool UseSharedMaterial;

	[HideInInspector]
	public float HUE = -1f;

	[HideInInspector]
	public bool canUpdate;

	private Material mat;

	private int propertyID;

	private float startTime;

	private Color startColor;

	private bool isInitialized;

	private string shaderProperty;

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		shaderProperty = ShaderColorProperty.ToString();
		startTime = Time.time;
		canUpdate = true;
		Renderer component = GetComponent<Renderer>();
		if (component == null)
		{
			Projector component2 = GetComponent<Projector>();
			if (component2 != null)
			{
				if (!component2.material.name.EndsWith("(Instance)"))
				{
					component2.material = new Material(component2.material)
					{
						name = component2.material.name + " (Instance)"
					};
				}
				mat = component2.material;
			}
		}
		else if (!UseSharedMaterial)
		{
			mat = component.material;
		}
		else
		{
			mat = component.sharedMaterial;
		}
		if (mat == null)
		{
			canUpdate = false;
			return;
		}
		if (!mat.HasProperty(shaderProperty))
		{
			canUpdate = false;
			return;
		}
		if (mat.HasProperty(shaderProperty))
		{
			propertyID = Shader.PropertyToID(shaderProperty);
		}
		startColor = mat.GetColor(propertyID);
		Color a = Color.Evaluate(0f);
		mat.SetColor(propertyID, a * startColor);
		isInitialized = true;
	}

	private void OnEnable()
	{
		if (isInitialized)
		{
			startTime = Time.time;
			canUpdate = true;
		}
	}

	private void Update()
	{
		if (mat == null)
		{
			return;
		}
		float num = Time.time - startTime;
		if (canUpdate)
		{
			Color a = Color.Evaluate(num / TimeMultiplier);
			mat.SetColor(propertyID, a * startColor);
		}
		if (num >= TimeMultiplier)
		{
			if (IsLoop)
			{
				startTime = Time.time;
			}
			else
			{
				canUpdate = false;
			}
		}
	}

	private void OnDisable()
	{
		if (!(mat == null))
		{
			if (UseSharedMaterial)
			{
				mat.SetColor(propertyID, startColor);
			}
			mat.SetColor(propertyID, startColor);
		}
	}
}
