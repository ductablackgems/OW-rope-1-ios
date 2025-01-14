using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("MiniMap/Map marker")]
public class MapMarker : MonoBehaviour
{
	public Image MarkerImage
	{
		get
		{
			return this.markerImage;
		}
	}

	private void Start()
	{
		if (!this.markerSprite)
		{
			UnityEngine.Debug.LogError(" Please, specify the marker sprite.");
		}
		GameObject gameObject = new GameObject("Marker");
		gameObject.AddComponent<Image>();
		this.markerImage = gameObject.GetComponent<Image>();
		this.markerImage.sprite = this.markerSprite;
		this.markerImage.rectTransform.localPosition = Vector3.zero;
		this.markerImage.rectTransform.localScale = Vector3.one;
		this.markerImage.rectTransform.sizeDelta = new Vector2(this.markerSize, this.markerSize);
		this.markerImage.gameObject.SetActive(false);
	}

	private void Update()
	{
		this.markerImage.rectTransform.rotation = Quaternion.identity;
	}

	private void OnDestroy()
	{
		if (this.markerImage)
		{
			Destroy(this.markerImage.gameObject);
		}
	}

	public void show()
	{
		this.markerImage.gameObject.SetActive(true);
	}

	public void hide()
	{
		this.markerImage.gameObject.SetActive(false);
	}

	public bool isVisible()
	{
		return this.markerImage.gameObject.activeSelf;
	}

	public Vector3 getPosition()
	{
		return base.gameObject.transform.position;
	}

	public void setLocalPos(Vector3 pos)
	{
		this.markerImage.rectTransform.localPosition = pos;
	}

	public void setOpacity(float opacity)
	{
		this.markerImage.color = new Color(1f, 1f, 1f, opacity);
	}

	public Sprite markerSprite;

	public float markerSize = 6.5f;

	public bool isActive = true;

	private Image markerImage;
}
