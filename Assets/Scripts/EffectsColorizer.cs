using UnityEngine;

public class EffectsColorizer : MonoBehaviour
{
	public struct HSBColor
	{
		public float h;

		public float s;

		public float b;

		public float a;

		public HSBColor(float h, float s, float b, float a)
		{
			this.h = h;
			this.s = s;
			this.b = b;
			this.a = a;
		}
	}

	public Color TintColor;

	public bool UseInstanceWhenNotEditorMode = true;

	private Color oldColor;

	private void Start()
	{
	}

	private void Update()
	{
		if (oldColor != TintColor)
		{
			ChangeColor(base.gameObject, TintColor);
		}
		oldColor = TintColor;
	}

	private void ChangeColor(GameObject effect, Color color)
	{
		Renderer[] componentsInChildren = effect.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			Material material = (!UseInstanceWhenNotEditorMode) ? renderer.sharedMaterial : renderer.material;
			HSBColor hSBColor = ColorToHSV(TintColor);
			if (material == null)
			{
				continue;
			}
			if (material.HasProperty("_TintColor"))
			{
				Color color2 = material.GetColor("_TintColor");
				HSBColor hsbColor = ColorToHSV(color2);
				hsbColor.h = hSBColor.h / 360f;
				color = HSVToColor(hsbColor);
				material.SetColor("_TintColor", color);
			}
			if (material.HasProperty("_CoreColor"))
			{
				Color color3 = material.GetColor("_CoreColor");
				HSBColor hsbColor2 = ColorToHSV(color3);
				hsbColor2.h = hSBColor.h / 360f;
				color = HSVToColor(hsbColor2);
				material.SetColor("_CoreColor", color);
			}
			Projector[] componentsInChildren2 = effect.GetComponentsInChildren<Projector>();
			foreach (Projector projector in componentsInChildren2)
			{
				material = projector.material;
				if (!(material == null) && material.HasProperty("_TintColor"))
				{
					Color color4 = material.GetColor("_TintColor");
					HSBColor hsbColor3 = ColorToHSV(color4);
					hsbColor3.h = hSBColor.h / 360f;
					color = HSVToColor(hsbColor3);
					projector.material.SetColor("_TintColor", color);
				}
			}
		}
		Light componentInChildren = effect.GetComponentInChildren<Light>();
		if (componentInChildren != null)
		{
			componentInChildren.color = color;
		}
	}

	public HSBColor ColorToHSV(Color color)
	{
		HSBColor hSBColor = new HSBColor(0f, 0f, 0f, color.a);
		float r = color.r;
		float g = color.g;
		float b = color.b;
		float num = Mathf.Max(r, Mathf.Max(g, b));
		if (num <= 0f)
		{
			return hSBColor;
		}
		float num2 = Mathf.Min(r, Mathf.Min(g, b));
		float num3 = num - num2;
		if (num > num2)
		{
			if (g == num)
			{
				hSBColor.h = (b - r) / num3 * 60f + 120f;
			}
			else if (b == num)
			{
				hSBColor.h = (r - g) / num3 * 60f + 240f;
			}
			else if (b > g)
			{
				hSBColor.h = (g - b) / num3 * 60f + 360f;
			}
			else
			{
				hSBColor.h = (g - b) / num3 * 60f;
			}
			if (hSBColor.h < 0f)
			{
				hSBColor.h += 360f;
			}
		}
		else
		{
			hSBColor.h = 0f;
		}
		hSBColor.h *= 0.00277777785f;
		hSBColor.s = num3 / num * 1f;
		hSBColor.b = num;
		return hSBColor;
	}

	public Color HSVToColor(HSBColor hsbColor)
	{
		float value = hsbColor.b;
		float value2 = hsbColor.b;
		float value3 = hsbColor.b;
		if (hsbColor.s != 0f)
		{
			float b = hsbColor.b;
			float num = hsbColor.b * hsbColor.s;
			float num2 = hsbColor.b - num;
			float num3 = hsbColor.h * 360f;
			if (num3 < 60f)
			{
				value = b;
				value2 = num3 * num / 60f + num2;
				value3 = num2;
			}
			else if (num3 < 120f)
			{
				value = (0f - (num3 - 120f)) * num / 60f + num2;
				value2 = b;
				value3 = num2;
			}
			else if (num3 < 180f)
			{
				value = num2;
				value2 = b;
				value3 = (num3 - 120f) * num / 60f + num2;
			}
			else if (num3 < 240f)
			{
				value = num2;
				value2 = (0f - (num3 - 240f)) * num / 60f + num2;
				value3 = b;
			}
			else if (num3 < 300f)
			{
				value = (num3 - 240f) * num / 60f + num2;
				value2 = num2;
				value3 = b;
			}
			else if (num3 <= 360f)
			{
				value = b;
				value2 = num2;
				value3 = (0f - (num3 - 360f)) * num / 60f + num2;
			}
			else
			{
				value = 0f;
				value2 = 0f;
				value3 = 0f;
			}
		}
		return new Color(Mathf.Clamp01(value), Mathf.Clamp01(value2), Mathf.Clamp01(value3), hsbColor.a);
	}
}
