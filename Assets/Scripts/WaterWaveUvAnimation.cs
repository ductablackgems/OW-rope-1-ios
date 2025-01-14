using System.Collections;
using UnityEngine;

public class WaterWaveUvAnimation : MonoBehaviour
{
	public float speed = 1f;

	public int fps = 30;

	public Color color;

	private Material mat;

	private float offset;

	private float offsetHeight;

	private float delta;

	private void Start()
	{
		mat = GetComponent<Renderer>().material;
		delta = 1f / (float)fps * speed;
		StartCoroutine(updateTiling());
	}

	private IEnumerator updateTiling()
	{
		while (true)
		{
			offset += delta;
			offsetHeight += delta;
			if (offset >= 1f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			Vector2 value = new Vector2(0f, offset);
			mat.SetTextureOffset("_BumpMap", value);
			mat.SetFloat("_OffsetYHeightMap", offset);
			if (offset < 0.3f)
			{
				mat.SetColor("_Color", new Color(color.r, color.g, color.b, offset / 0.3f));
			}
			if (offset > 0.7f)
			{
				mat.SetColor("_Color", new Color(color.r, color.g, color.b, (1f - offset) / 0.3f));
			}
			yield return new WaitForSeconds(1f / (float)fps);
		}
	}
}
