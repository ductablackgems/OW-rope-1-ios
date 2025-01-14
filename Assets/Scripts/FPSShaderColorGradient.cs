using UnityEngine;

public class FPSShaderColorGradient : MonoBehaviour
{
	public string ShaderProperty = "_TintColor";

	public int MaterialID;

	public Gradient Color = new Gradient();

	public float TimeMultiplier = 1f;

	private bool canUpdate;

	private Material matInstance;

	private int propertyID;

	private float startTime;

	private Color oldColor;

	private void Start()
	{
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			Material[] materials = component.materials;
			if (MaterialID >= materials.Length)
			{
				UnityEngine.Debug.Log("ShaderColorGradient: Material ID more than shader materials count.");
			}
			matInstance = materials[MaterialID];
		}
		else
		{
			Projector component2 = GetComponent<Projector>();
			Material material = component2.material;
			if (!material.name.EndsWith("(Instance)"))
			{
				matInstance = new Material(material)
				{
					name = material.name + " (Instance)"
				};
			}
			else
			{
				matInstance = material;
			}
			component2.material = matInstance;
		}
		if (!matInstance.HasProperty(ShaderProperty))
		{
			UnityEngine.Debug.Log("ShaderColorGradient: Shader not have \"" + ShaderProperty + "\" property");
		}
		propertyID = Shader.PropertyToID(ShaderProperty);
		oldColor = matInstance.GetColor(propertyID);
	}

	private void OnEnable()
	{
		startTime = Time.time;
		canUpdate = true;
	}

	private void Update()
	{
		float num = Time.time - startTime;
		if (canUpdate)
		{
			Color a = Color.Evaluate(num / TimeMultiplier);
			matInstance.SetColor(propertyID, a * oldColor);
		}
		if (num >= TimeMultiplier)
		{
			canUpdate = false;
		}
	}
}
