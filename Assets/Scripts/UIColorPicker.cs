using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class UIColorPicker : MonoBehaviour
{
	public static UIColorPicker current;

	public Color value = Color.white;

	public UIWidget selectionWidget;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	[NonSerialized]
	private Transform mTrans;

	[NonSerialized]
	private UITexture mUITex;

	[NonSerialized]
	private Texture2D mTex;

	[NonSerialized]
	private UICamera mCam;

	[NonSerialized]
	private Vector2 mPos;

	[NonSerialized]
	private int mWidth;

	[NonSerialized]
	private int mHeight;

	private static AnimationCurve mRed;

	private static AnimationCurve mGreen;

	private static AnimationCurve mBlue;

	private void Start()
	{
		mTrans = base.transform;
		mUITex = GetComponent<UITexture>();
		mCam = UICamera.FindCameraForLayer(base.gameObject.layer);
		mWidth = mUITex.width;
		mHeight = mUITex.height;
		Color[] array = new Color[mWidth * mHeight];
		for (int i = 0; i < mHeight; i++)
		{
			float y = ((float)i - 1f) / (float)mHeight;
			for (int j = 0; j < mWidth; j++)
			{
				float x = ((float)j - 1f) / (float)mWidth;
				int num = j + i * mWidth;
				array[num] = Sample(x, y);
			}
		}
		mTex = new Texture2D(mWidth, mHeight, TextureFormat.RGB24, mipChain: false);
		mTex.SetPixels(array);
		mTex.filterMode = FilterMode.Trilinear;
		mTex.wrapMode = TextureWrapMode.Clamp;
		mTex.Apply();
		mUITex.mainTexture = mTex;
		Select(value);
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(mTex);
		mTex = null;
	}

	private void OnPress(bool pressed)
	{
		if (base.enabled && pressed && UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			Sample();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (base.enabled)
		{
			Sample();
		}
	}

	private void OnPan(Vector2 delta)
	{
		if (base.enabled)
		{
			mPos.x = Mathf.Clamp01(mPos.x + delta.x);
			mPos.y = Mathf.Clamp01(mPos.y + delta.y);
			Select(mPos);
		}
	}

	private void Sample()
	{
		Vector3 vector = UICamera.lastEventPosition;
		vector = mCam.cachedCamera.ScreenToWorldPoint(vector);
		vector = mTrans.InverseTransformPoint(vector);
		Vector3[] localCorners = mUITex.localCorners;
		mPos.x = Mathf.Clamp01((vector.x - localCorners[0].x) / (localCorners[2].x - localCorners[0].x));
		mPos.y = Mathf.Clamp01((vector.y - localCorners[0].y) / (localCorners[2].y - localCorners[0].y));
		if (selectionWidget != null)
		{
			vector.x = Mathf.Lerp(localCorners[0].x, localCorners[2].x, mPos.x);
			vector.y = Mathf.Lerp(localCorners[0].y, localCorners[2].y, mPos.y);
			vector = mTrans.TransformPoint(vector);
			selectionWidget.transform.OverlayPosition(vector, mCam.cachedCamera);
		}
		value = Sample(mPos.x, mPos.y);
		current = this;
		EventDelegate.Execute(onChange);
		current = null;
	}

	public void Select(Vector2 v)
	{
		v.x = Mathf.Clamp01(v.x);
		v.y = Mathf.Clamp01(v.y);
		mPos = v;
		if (selectionWidget != null)
		{
			Vector3[] localCorners = mUITex.localCorners;
			v.x = Mathf.Lerp(localCorners[0].x, localCorners[2].x, mPos.x);
			v.y = Mathf.Lerp(localCorners[0].y, localCorners[2].y, mPos.y);
			v = mTrans.TransformPoint(v);
			selectionWidget.transform.OverlayPosition(v, mCam.cachedCamera);
		}
		value = Sample(mPos.x, mPos.y);
		current = this;
		EventDelegate.Execute(onChange);
		current = null;
	}

	public Vector2 Select(Color c)
	{
		if (mUITex == null)
		{
			value = c;
			return mPos;
		}
		float num = float.MaxValue;
		for (int i = 0; i < mHeight; i++)
		{
			float y = ((float)i - 1f) / (float)mHeight;
			for (int j = 0; j < mWidth; j++)
			{
				float x = ((float)j - 1f) / (float)mWidth;
				Color color = Sample(x, y);
				color.r -= c.r;
				color.g -= c.g;
				color.b -= c.b;
				float num2 = color.r * color.r + color.g * color.g + color.b * color.b;
				if (num2 < num)
				{
					num = num2;
					mPos.x = x;
					mPos.y = y;
				}
			}
		}
		if (selectionWidget != null)
		{
			Vector3[] localCorners = mUITex.localCorners;
			Vector3 vector = default(Vector3);
			vector.x = Mathf.Lerp(localCorners[0].x, localCorners[2].x, mPos.x);
			vector.y = Mathf.Lerp(localCorners[0].y, localCorners[2].y, mPos.y);
			vector.z = 0f;
			vector = mTrans.TransformPoint(vector);
			selectionWidget.transform.OverlayPosition(vector, mCam.cachedCamera);
		}
		value = c;
		current = this;
		EventDelegate.Execute(onChange);
		current = null;
		return mPos;
	}

	public static Color Sample(float x, float y)
	{
		if (mRed == null)
		{
			mRed = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.142857149f, 1f), new Keyframe(0.2857143f, 0f), new Keyframe(0.428571433f, 0f), new Keyframe(0.5714286f, 0f), new Keyframe(0.714285731f, 1f), new Keyframe(0.857142866f, 1f), new Keyframe(1f, 0.5f));
			mGreen = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.142857149f, 1f), new Keyframe(0.2857143f, 1f), new Keyframe(0.428571433f, 1f), new Keyframe(0.5714286f, 0f), new Keyframe(0.714285731f, 0f), new Keyframe(0.857142866f, 0f), new Keyframe(1f, 0.5f));
			mBlue = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.142857149f, 0f), new Keyframe(0.2857143f, 0f), new Keyframe(0.428571433f, 1f), new Keyframe(0.5714286f, 1f), new Keyframe(0.714285731f, 1f), new Keyframe(0.857142866f, 0f), new Keyframe(1f, 0.5f));
		}
		Vector3 vector = new Vector3(mRed.Evaluate(x), mGreen.Evaluate(x), mBlue.Evaluate(x));
		if (y < 0.5f)
		{
			y *= 2f;
			vector.x *= y;
			vector.y *= y;
			vector.z *= y;
		}
		else
		{
			vector = Vector3.Lerp(vector, Vector3.one, y * 2f - 1f);
		}
		return new Color(vector.x, vector.y, vector.z, 1f);
	}
}
