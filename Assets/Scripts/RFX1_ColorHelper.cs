using System;
using UnityEngine;

public static class RFX1_ColorHelper
{
	public struct HSBColor
	{
		public float H;

		public float S;

		public float B;

		public float A;

		public HSBColor(float h, float s, float b, float a)
		{
			H = h;
			S = s;
			B = b;
			A = a;
		}
	}

	private const float TOLERANCE = 0.0001f;

	private static string[] colorProperties = new string[10]
	{
		"_TintColor",
		"_Color",
		"_EmissionColor",
		"_BorderColor",
		"_ReflectColor",
		"_RimColor",
		"_MainColor",
		"_CoreColor",
		"_FresnelColor",
		"_CutoutColor"
	};

	public static HSBColor ColorToHSV(Color color)
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
			if (Math.Abs(g - num) < 0.0001f)
			{
				hSBColor.H = (b - r) / num3 * 60f + 120f;
			}
			else if (Math.Abs(b - num) < 0.0001f)
			{
				hSBColor.H = (r - g) / num3 * 60f + 240f;
			}
			else if (b > g)
			{
				hSBColor.H = (g - b) / num3 * 60f + 360f;
			}
			else
			{
				hSBColor.H = (g - b) / num3 * 60f;
			}
			if (hSBColor.H < 0f)
			{
				hSBColor.H += 360f;
			}
		}
		else
		{
			hSBColor.H = 0f;
		}
		hSBColor.H *= 0.00277777785f;
		hSBColor.S = num3 / num * 1f;
		hSBColor.B = num;
		return hSBColor;
	}

	public static Color HSVToColor(HSBColor hsbColor)
	{
		float value = hsbColor.B;
		float value2 = hsbColor.B;
		float value3 = hsbColor.B;
		if (Math.Abs(hsbColor.S) > 0.0001f)
		{
			float b = hsbColor.B;
			float num = hsbColor.B * hsbColor.S;
			float num2 = hsbColor.B - num;
			float num3 = hsbColor.H * 360f;
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
		return new Color(Mathf.Clamp01(value), Mathf.Clamp01(value2), Mathf.Clamp01(value3), hsbColor.A);
	}

	public static Color ConvertRGBColorByHUE(Color rgbColor, float hue)
	{
		float num = ColorToHSV(rgbColor).B;
		if (num < 0.0001f)
		{
			num = 0.0001f;
		}
		HSBColor hsbColor = ColorToHSV(rgbColor / num);
		hsbColor.H = hue;
		Color result = HSVToColor(hsbColor) * num;
		result.a = rgbColor.a;
		return result;
	}

	public static void ChangeObjectColorByHUE(GameObject go, float hue)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material material = componentsInChildren[i].material;
			if (material == null)
			{
				continue;
			}
			string[] array = colorProperties;
			foreach (string name in array)
			{
				if (material.HasProperty(name))
				{
					setMatHUEColor(material, name, hue);
				}
			}
		}
		ParticleSystemRenderer[] componentsInChildren2 = go.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true);
		foreach (ParticleSystemRenderer particleSystemRenderer in componentsInChildren2)
		{
			Material trailMaterial = particleSystemRenderer.trailMaterial;
			if (trailMaterial == null)
			{
				continue;
			}
			trailMaterial = (particleSystemRenderer.trailMaterial = new Material(trailMaterial)
			{
				name = trailMaterial.name + " (Instance)"
			});
			string[] array = colorProperties;
			foreach (string name2 in array)
			{
				if (trailMaterial.HasProperty(name2))
				{
					setMatHUEColor(trailMaterial, name2, hue);
				}
			}
		}
		SkinnedMeshRenderer[] componentsInChildren3 = go.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		for (int i = 0; i < componentsInChildren3.Length; i++)
		{
			Material material3 = componentsInChildren3[i].material;
			if (material3 == null)
			{
				continue;
			}
			string[] array = colorProperties;
			foreach (string name3 in array)
			{
				if (material3.HasProperty(name3))
				{
					setMatHUEColor(material3, name3, hue);
				}
			}
		}
		Projector[] componentsInChildren4 = go.GetComponentsInChildren<Projector>(includeInactive: true);
		foreach (Projector projector in componentsInChildren4)
		{
			if (!projector.material.name.EndsWith("(Instance)"))
			{
				projector.material = new Material(projector.material)
				{
					name = projector.material.name + " (Instance)"
				};
			}
			Material material4 = projector.material;
			if (material4 == null)
			{
				continue;
			}
			string[] array = colorProperties;
			foreach (string name4 in array)
			{
				if (material4.HasProperty(name4))
				{
					projector.material = setMatHUEColor(material4, name4, hue);
				}
			}
		}
		Light[] componentsInChildren5 = go.GetComponentsInChildren<Light>(includeInactive: true);
		foreach (Light obj in componentsInChildren5)
		{
			HSBColor hsbColor = ColorToHSV(obj.color);
			hsbColor.H = hue;
			obj.color = HSVToColor(hsbColor);
		}
		ParticleSystem[] componentsInChildren6 = go.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		foreach (ParticleSystem obj2 in componentsInChildren6)
		{
			ParticleSystem.MainModule main = obj2.main;
			HSBColor hSBColor = ColorToHSV(obj2.main.startColor.color);
			hSBColor.H = hue;
			main.startColor = HSVToColor(hSBColor);
			ParticleSystem.ColorOverLifetimeModule colorOverLifetime = obj2.colorOverLifetime;
			ParticleSystem.MinMaxGradient color = colorOverLifetime.color;
			Gradient gradient = colorOverLifetime.color.gradient;
			GradientColorKey[] colorKeys = colorOverLifetime.color.gradient.colorKeys;
			float num = 0f;
			hSBColor = ColorToHSV(colorKeys[0].color);
			num = Math.Abs(ColorToHSV(colorKeys[1].color).H - hSBColor.H);
			hSBColor.H = hue;
			colorKeys[0].color = HSVToColor(hSBColor);
			for (int k = 1; k < colorKeys.Length; k++)
			{
				hSBColor = ColorToHSV(colorKeys[k].color);
				hSBColor.H = Mathf.Repeat(hSBColor.H + num, 1f);
				colorKeys[k].color = HSVToColor(hSBColor);
			}
			gradient.colorKeys = colorKeys;
			color.gradient = gradient;
			colorOverLifetime.color = color;
		}
		RFX1_ShaderColorGradient[] componentsInChildren7 = go.GetComponentsInChildren<RFX1_ShaderColorGradient>(includeInactive: true);
		for (int i = 0; i < componentsInChildren7.Length; i++)
		{
			componentsInChildren7[i].HUE = hue;
		}
	}

	private static Material setMatHUEColor(Material mat, string name, float hueColor)
	{
		Color value = ConvertRGBColorByHUE(mat.GetColor(name), hueColor);
		mat.SetColor(name, value);
		return mat;
	}

	private static Material setMatAlphaColor(Material mat, string name, float alpha)
	{
		Color color = mat.GetColor(name);
		color.a = alpha;
		mat.SetColor(name, color);
		return mat;
	}
}
