using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Examples/HUD Text")]
public class HUDText : MonoBehaviour
{
	protected class Entry
	{
		public float time;

		public float stay;

		public float offset;

		public float val;

		public UILabel label;

		public float movementStart => time + stay;
	}

	[HideInInspector]
	[SerializeField]
	private UIFont font;

	public UIFont bitmapFont;

	public Font trueTypeFont;

	public int fontSize = 20;

	public FontStyle fontStyle;

	public bool applyGradient;

	public Color gradientTop = Color.white;

	public Color gradienBottom = new Color(0.7f, 0.7f, 0.7f);

	public UILabel.Effect effect;

	public Color effectColor = Color.black;

	public AnimationCurve offsetCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(3f, 40f));

	public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe(1f, 1f), new Keyframe(3f, 0f));

	public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 1f));

	private List<Entry> mList = new List<Entry>();

	private List<Entry> mUnused = new List<Entry>();

	private int counter;

	private Keyframe[] mOffsets;

	private Keyframe[] mAlphas;

	private Keyframe[] mScales;

	private bool mUseDynamicFont;

	public bool isVisible => mList.Count != 0;

	public UnityEngine.Object ambigiousFont
	{
		get
		{
			if (trueTypeFont != null)
			{
				return trueTypeFont;
			}
			if (bitmapFont != null)
			{
				return bitmapFont;
			}
			return font;
		}
		set
		{
			if (value is Font)
			{
				trueTypeFont = (value as Font);
				bitmapFont = null;
				font = null;
			}
			else if (value is UIFont)
			{
				bitmapFont = (value as UIFont);
				trueTypeFont = null;
				font = null;
			}
		}
	}

	private static int Comparison(Entry a, Entry b)
	{
		if (a.movementStart < b.movementStart)
		{
			return -1;
		}
		if (a.movementStart > b.movementStart)
		{
			return 1;
		}
		return 0;
	}

	private Entry Create()
	{
		if (mUnused.Count > 0)
		{
			Entry entry = mUnused[mUnused.Count - 1];
			mUnused.RemoveAt(mUnused.Count - 1);
			entry.time = Time.realtimeSinceStartup;
			entry.label.depth = NGUITools.CalculateNextDepth(base.gameObject);
			NGUITools.SetActive(entry.label.gameObject, state: true);
			entry.offset = 0f;
			mList.Add(entry);
			return entry;
		}
		Entry entry2 = new Entry();
		entry2.time = Time.realtimeSinceStartup;
		entry2.label = base.gameObject.AddWidget<UILabel>();
		entry2.label.name = counter.ToString();
		entry2.label.ambigiousFont = ambigiousFont;
		entry2.label.fontSize = fontSize;
		entry2.label.fontStyle = fontStyle;
		entry2.label.applyGradient = applyGradient;
		entry2.label.gradientTop = gradientTop;
		entry2.label.gradientBottom = gradienBottom;
		entry2.label.effectStyle = effect;
		entry2.label.effectColor = effectColor;
		entry2.label.overflowMethod = UILabel.Overflow.ResizeFreely;
		entry2.label.cachedTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		mList.Add(entry2);
		counter++;
		return entry2;
	}

	private void Delete(Entry ent)
	{
		mList.Remove(ent);
		mUnused.Add(ent);
		NGUITools.SetActive(ent.label.gameObject, state: false);
	}

	public void AddLocalized(string text, Color c, float stayDuration)
	{
		Add(Localization.Get(text), c, stayDuration);
	}

	private IEnumerator Add2(object obj, Color c, float stayDuration)
	{
		if (!base.enabled)
		{
			yield break;
		}
		yield return new WaitForSeconds(1E-05f);
		while (Time.timeScale < 0.1f)
		{
			yield return null;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		bool flag = false;
		float num = 0f;
		if (obj is float)
		{
			flag = true;
			num = (float)obj;
		}
		else if (obj is int)
		{
			flag = true;
			num = (int)obj;
		}
		if (flag)
		{
			if (num == 0f)
			{
				yield break;
			}
			int num2 = mList.Count;
			while (num2 > 0)
			{
				Entry entry = mList[--num2];
				if (!(entry.time + 1f < realtimeSinceStartup) && entry.val != 0f)
				{
					if (entry.val < 0f && num < 0f)
					{
						entry.val += num;
						entry.label.text = Mathf.RoundToInt(entry.val).ToString();
						yield break;
					}
					if (entry.val > 0f && num > 0f)
					{
						entry.val += num;
						entry.label.text = "+" + Mathf.RoundToInt(entry.val);
						yield break;
					}
				}
			}
		}
		Entry entry2 = Create();
		entry2.stay = stayDuration;
		entry2.label.color = c;
		entry2.label.alpha = 0f;
		entry2.val = num;
		if (flag)
		{
			entry2.label.text = ((num < 0f) ? Mathf.RoundToInt(entry2.val).ToString() : ("+" + Mathf.RoundToInt(entry2.val)));
		}
		else
		{
			entry2.label.text = obj.ToString();
		}
		mList.Sort(Comparison);
	}

	public void Add(object obj, Color c, float stayDuration)
	{
		StartCoroutine(Add2(obj, c, stayDuration));
	}

	private void OnEnable()
	{
		if (font != null)
		{
			if (font.isDynamic)
			{
				trueTypeFont = font.dynamicFont;
				fontStyle = font.dynamicFontStyle;
				mUseDynamicFont = true;
			}
			else if (bitmapFont == null)
			{
				bitmapFont = font;
				mUseDynamicFont = false;
			}
			font = null;
		}
	}

	private void OnValidate()
	{
		Font x = trueTypeFont;
		UIFont uIFont = bitmapFont;
		bitmapFont = null;
		trueTypeFont = null;
		if (x != null && (uIFont == null || !mUseDynamicFont))
		{
			bitmapFont = null;
			trueTypeFont = x;
			mUseDynamicFont = true;
		}
		else if (uIFont != null)
		{
			if (uIFont.isDynamic)
			{
				trueTypeFont = uIFont.dynamicFont;
				fontStyle = uIFont.dynamicFontStyle;
				fontSize = uIFont.defaultSize;
				mUseDynamicFont = true;
			}
			else
			{
				bitmapFont = uIFont;
				mUseDynamicFont = false;
			}
		}
		else
		{
			trueTypeFont = x;
			mUseDynamicFont = true;
		}
	}

	private void OnDisable()
	{
		int num = mList.Count;
		while (num > 0)
		{
			Entry entry = mList[--num];
			if (entry.label != null)
			{
				entry.label.enabled = false;
			}
			else
			{
				mList.RemoveAt(num);
			}
		}
	}

	private void Update()
	{
		float time = RealTime.time;
		if (mOffsets == null)
		{
			mOffsets = offsetCurve.keys;
			mAlphas = alphaCurve.keys;
			mScales = scaleCurve.keys;
		}
		float time2 = mOffsets[mOffsets.Length - 1].time;
		float time3 = mAlphas[mAlphas.Length - 1].time;
		float num = Mathf.Max(mScales[mScales.Length - 1].time, Mathf.Max(time2, time3));
		int num2 = mList.Count;
		while (num2 > 0)
		{
			Entry entry = mList[--num2];
			float num3 = time - entry.movementStart;
			entry.offset = offsetCurve.Evaluate(num3);
			entry.label.alpha = alphaCurve.Evaluate(num3);
			float num4 = scaleCurve.Evaluate(time - entry.time);
			if (num4 < 0.001f)
			{
				num4 = 0.001f;
			}
			entry.label.cachedTransform.localScale = new Vector3(num4, num4, num4);
			if (num3 > num)
			{
				Delete(entry);
			}
			else
			{
				entry.label.enabled = true;
			}
		}
		float a = 0f;
		int num5 = mList.Count;
		while (num5 > 0)
		{
			Entry entry2 = mList[--num5];
			a = Mathf.Max(a, entry2.offset);
			entry2.label.cachedTransform.localPosition = new Vector3(0f, a, 0f);
			a += Mathf.Round(entry2.label.cachedTransform.localScale.y * (float)entry2.label.fontSize);
		}
	}
}
