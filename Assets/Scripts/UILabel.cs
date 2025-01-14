using System;
using System.Collections.Generic;
using UnityEngine;


public class UILabel : UIWidget
{
	public enum Effect
	{
		None,
		Shadow,
		Outline,
		Outline8
	}

	public enum Overflow
	{
		ShrinkContent,
		ClampContent,
		ResizeFreely,
		ResizeHeight
	}

	public enum Crispness
	{
		Never,
		OnDesktop,
		Always
	}

	public enum Modifier
	{
		None = 0,
		ToUppercase = 1,
		ToLowercase = 2,
		Custom = 0xFF
	}

	public delegate string ModifierFunc(string s);

	public Crispness keepCrispWhenShrunk = Crispness.OnDesktop;

	[SerializeField]
	private Font mTrueTypeFont;

	[SerializeField]
	private UIFont mFont;

	[Multiline(6)]
	[HideInInspector]
	[SerializeField]
	private string mText = "";

	[HideInInspector]
	[SerializeField]
	private int mFontSize = 16;

	[HideInInspector]
	[SerializeField]
	private FontStyle mFontStyle;

	[HideInInspector]
	[SerializeField]
	private NGUIText.Alignment mAlignment;

	[HideInInspector]
	[SerializeField]
	private bool mEncoding = true;

	[HideInInspector]
	[SerializeField]
	private int mMaxLineCount;

	[HideInInspector]
	[SerializeField]
	private Effect mEffectStyle;

	[HideInInspector]
	[SerializeField]
	private Color mEffectColor = Color.black;

	[HideInInspector]
	[SerializeField]
	private NGUIText.SymbolStyle mSymbols = NGUIText.SymbolStyle.Normal;

	[HideInInspector]
	[SerializeField]
	private Vector2 mEffectDistance = Vector2.one;

	[HideInInspector]
	[SerializeField]
	private Overflow mOverflow;

	[HideInInspector]
	[SerializeField]
	private bool mApplyGradient;

	[HideInInspector]
	[SerializeField]
	private Color mGradientTop = Color.white;

	[HideInInspector]
	[SerializeField]
	private Color mGradientBottom = new Color(0.7f, 0.7f, 0.7f);

	[HideInInspector]
	[SerializeField]
	private int mSpacingX;

	[HideInInspector]
	[SerializeField]
	private int mSpacingY;

	[HideInInspector]
	[SerializeField]
	private bool mUseFloatSpacing;

	[HideInInspector]
	[SerializeField]
	private float mFloatSpacingX;

	[HideInInspector]
	[SerializeField]
	private float mFloatSpacingY;

	[HideInInspector]
	[SerializeField]
	private bool mOverflowEllipsis;

	[HideInInspector]
	[SerializeField]
	private int mOverflowWidth;

	[HideInInspector]
	[SerializeField]
	private Modifier mModifier;

	[HideInInspector]
	[SerializeField]
	private bool mShrinkToFit;

	[HideInInspector]
	[SerializeField]
	private int mMaxLineWidth;

	[HideInInspector]
	[SerializeField]
	private int mMaxLineHeight;

	[HideInInspector]
	[SerializeField]
	private float mLineWidth;

	[HideInInspector]
	[SerializeField]
	private bool mMultiline = true;

	[NonSerialized]
	private Font mActiveTTF;

	[NonSerialized]
	private float mDensity = 1f;

	[NonSerialized]
	private bool mShouldBeProcessed = true;

	[NonSerialized]
	private string mProcessedText;

	[NonSerialized]
	private bool mPremultiply;

	[NonSerialized]
	private Vector2 mCalculatedSize = Vector2.zero;

	[NonSerialized]
	private float mScale = 1f;

	[NonSerialized]
	private int mFinalFontSize;

	[NonSerialized]
	private int mLastWidth;

	[NonSerialized]
	private int mLastHeight;

	public ModifierFunc customModifier;

	private static BetterList<UILabel> mList = new BetterList<UILabel>();

	private static Dictionary<Font, int> mFontUsage = new Dictionary<Font, int>();

	[NonSerialized]
	private static BetterList<UIDrawCall> mTempDrawcalls;

	private static bool mTexRebuildAdded = false;

	private static List<Vector3> mTempVerts = new List<Vector3>();

	private static List<int> mTempIndices = new List<int>();

	public int finalFontSize
	{
		get
		{
			if ((bool)trueTypeFont)
			{
				return Mathf.RoundToInt(mScale * (float)mFinalFontSize);
			}
			return Mathf.RoundToInt((float)mFinalFontSize * mScale);
		}
	}

	private bool shouldBeProcessed
	{
		get
		{
			return mShouldBeProcessed;
		}
		set
		{
			if (value)
			{
				mChanged = true;
				mShouldBeProcessed = true;
			}
			else
			{
				mShouldBeProcessed = false;
			}
		}
	}

	public override bool isAnchoredHorizontally
	{
		get
		{
			if (!base.isAnchoredHorizontally)
			{
				return mOverflow == Overflow.ResizeFreely;
			}
			return true;
		}
	}

	public override bool isAnchoredVertically
	{
		get
		{
			if (!base.isAnchoredVertically && mOverflow != Overflow.ResizeFreely)
			{
				return mOverflow == Overflow.ResizeHeight;
			}
			return true;
		}
	}

	public override Material material
	{
		get
		{
			if (mMat != null)
			{
				return mMat;
			}
			if (mFont != null)
			{
				return mFont.material;
			}
			if (mTrueTypeFont != null)
			{
				return mTrueTypeFont.material;
			}
			return null;
		}
		set
		{
			base.material = value;
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (mFont != null)
			{
				return mFont.texture;
			}
			if (mTrueTypeFont != null)
			{
				Material material = mTrueTypeFont.material;
				if (material != null)
				{
					return material.mainTexture;
				}
			}
			return null;
		}
		set
		{
			base.mainTexture = value;
		}
	}

	[Obsolete("Use UILabel.bitmapFont instead")]
	public UIFont font
	{
		get
		{
			return bitmapFont;
		}
		set
		{
			bitmapFont = value;
		}
	}

	public UIFont bitmapFont
	{
		get
		{
			return mFont;
		}
		set
		{
			if (mFont != value)
			{
				RemoveFromPanel();
				mFont = value;
				mTrueTypeFont = null;
				MarkAsChanged();
			}
		}
	}

	public Font trueTypeFont
	{
		get
		{
			if (mTrueTypeFont != null)
			{
				return mTrueTypeFont;
			}
			if (!(mFont != null))
			{
				return null;
			}
			return mFont.dynamicFont;
		}
		set
		{
			if (mTrueTypeFont != value)
			{
				SetActiveFont(null);
				RemoveFromPanel();
				mTrueTypeFont = value;
				shouldBeProcessed = true;
				mFont = null;
				SetActiveFont(value);
				ProcessAndRequest();
				if (mActiveTTF != null)
				{
					base.MarkAsChanged();
				}
			}
		}
	}

	public UnityEngine.Object ambigiousFont
	{
		get
		{
			return (UnityEngine.Object)(((object)mFont) ?? ((object)mTrueTypeFont));
		}
		set
		{
			UIFont uIFont = value as UIFont;
			if (uIFont != null)
			{
				bitmapFont = uIFont;
			}
			else
			{
				trueTypeFont = (value as Font);
			}
		}
	}

	public string text
	{
		get
		{
			return mText;
		}
		set
		{
			if (mText == value)
			{
				return;
			}
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mText))
				{
					mText = "";
					MarkAsChanged();
					ProcessAndRequest();
				}
			}
			else if (mText != value)
			{
				mText = value;
				MarkAsChanged();
				ProcessAndRequest();
			}
			if (autoResizeBoxCollider)
			{
				ResizeCollider();
			}
		}
	}

	public int defaultFontSize
	{
		get
		{
			if (!(trueTypeFont != null))
			{
				if (!(mFont != null))
				{
					return 16;
				}
				return mFont.defaultSize;
			}
			return mFontSize;
		}
	}

	public int fontSize
	{
		get
		{
			return mFontSize;
		}
		set
		{
			value = Mathf.Clamp(value, 0, 256);
			if (mFontSize != value)
			{
				mFontSize = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	public FontStyle fontStyle
	{
		get
		{
			return mFontStyle;
		}
		set
		{
			if (mFontStyle != value)
			{
				mFontStyle = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	public NGUIText.Alignment alignment
	{
		get
		{
			return mAlignment;
		}
		set
		{
			if (mAlignment != value)
			{
				mAlignment = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	public bool applyGradient
	{
		get
		{
			return mApplyGradient;
		}
		set
		{
			if (mApplyGradient != value)
			{
				mApplyGradient = value;
				MarkAsChanged();
			}
		}
	}

	public Color gradientTop
	{
		get
		{
			return mGradientTop;
		}
		set
		{
			if (mGradientTop != value)
			{
				mGradientTop = value;
				if (mApplyGradient)
				{
					MarkAsChanged();
				}
			}
		}
	}

	public Color gradientBottom
	{
		get
		{
			return mGradientBottom;
		}
		set
		{
			if (mGradientBottom != value)
			{
				mGradientBottom = value;
				if (mApplyGradient)
				{
					MarkAsChanged();
				}
			}
		}
	}

	public int spacingX
	{
		get
		{
			return mSpacingX;
		}
		set
		{
			if (mSpacingX != value)
			{
				mSpacingX = value;
				MarkAsChanged();
			}
		}
	}

	public int spacingY
	{
		get
		{
			return mSpacingY;
		}
		set
		{
			if (mSpacingY != value)
			{
				mSpacingY = value;
				MarkAsChanged();
			}
		}
	}

	public bool useFloatSpacing
	{
		get
		{
			return mUseFloatSpacing;
		}
		set
		{
			if (mUseFloatSpacing != value)
			{
				mUseFloatSpacing = value;
				shouldBeProcessed = true;
			}
		}
	}

	public float floatSpacingX
	{
		get
		{
			return mFloatSpacingX;
		}
		set
		{
			if (!Mathf.Approximately(mFloatSpacingX, value))
			{
				mFloatSpacingX = value;
				MarkAsChanged();
			}
		}
	}

	public float floatSpacingY
	{
		get
		{
			return mFloatSpacingY;
		}
		set
		{
			if (!Mathf.Approximately(mFloatSpacingY, value))
			{
				mFloatSpacingY = value;
				MarkAsChanged();
			}
		}
	}

	public float effectiveSpacingY
	{
		get
		{
			if (!mUseFloatSpacing)
			{
				return mSpacingY;
			}
			return mFloatSpacingY;
		}
	}

	public float effectiveSpacingX
	{
		get
		{
			if (!mUseFloatSpacing)
			{
				return mSpacingX;
			}
			return mFloatSpacingX;
		}
	}

	public bool overflowEllipsis
	{
		get
		{
			return mOverflowEllipsis;
		}
		set
		{
			if (mOverflowEllipsis != value)
			{
				mOverflowEllipsis = value;
				MarkAsChanged();
			}
		}
	}

	public int overflowWidth
	{
		get
		{
			return mOverflowWidth;
		}
		set
		{
			if (mOverflowWidth != value)
			{
				mOverflowWidth = value;
				MarkAsChanged();
			}
		}
	}

	private bool keepCrisp
	{
		get
		{
			if (trueTypeFont != null && keepCrispWhenShrunk != 0)
			{
				return keepCrispWhenShrunk == Crispness.Always;
			}
			return false;
		}
	}

	public bool supportEncoding
	{
		get
		{
			return mEncoding;
		}
		set
		{
			if (mEncoding != value)
			{
				mEncoding = value;
				shouldBeProcessed = true;
			}
		}
	}

	public NGUIText.SymbolStyle symbolStyle
	{
		get
		{
			return mSymbols;
		}
		set
		{
			if (mSymbols != value)
			{
				mSymbols = value;
				shouldBeProcessed = true;
			}
		}
	}

	public Overflow overflowMethod
	{
		get
		{
			return mOverflow;
		}
		set
		{
			if (mOverflow != value)
			{
				mOverflow = value;
				shouldBeProcessed = true;
			}
		}
	}

	[Obsolete("Use 'width' instead")]
	public int lineWidth
	{
		get
		{
			return base.width;
		}
		set
		{
			base.width = value;
		}
	}

	[Obsolete("Use 'height' instead")]
	public int lineHeight
	{
		get
		{
			return base.height;
		}
		set
		{
			base.height = value;
		}
	}

	public bool multiLine
	{
		get
		{
			return mMaxLineCount != 1;
		}
		set
		{
			if (mMaxLineCount != 1 != value)
			{
				mMaxLineCount = ((!value) ? 1 : 0);
				shouldBeProcessed = true;
			}
		}
	}

	public override Vector3[] localCorners
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return base.localCorners;
		}
	}

	public override Vector3[] worldCorners
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return base.worldCorners;
		}
	}

	public override Vector4 drawingDimensions
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return base.drawingDimensions;
		}
	}

	public int maxLineCount
	{
		get
		{
			return mMaxLineCount;
		}
		set
		{
			if (mMaxLineCount != value)
			{
				mMaxLineCount = Mathf.Max(value, 0);
				shouldBeProcessed = true;
				if (overflowMethod == Overflow.ShrinkContent)
				{
					MakePixelPerfect();
				}
			}
		}
	}

	public Effect effectStyle
	{
		get
		{
			return mEffectStyle;
		}
		set
		{
			if (mEffectStyle != value)
			{
				mEffectStyle = value;
				shouldBeProcessed = true;
			}
		}
	}

	public Color effectColor
	{
		get
		{
			return mEffectColor;
		}
		set
		{
			if (mEffectColor != value)
			{
				mEffectColor = value;
				if (mEffectStyle != 0)
				{
					shouldBeProcessed = true;
				}
			}
		}
	}

	public Vector2 effectDistance
	{
		get
		{
			return mEffectDistance;
		}
		set
		{
			if (mEffectDistance != value)
			{
				mEffectDistance = value;
				shouldBeProcessed = true;
			}
		}
	}

	public int quadsPerCharacter
	{
		get
		{
			if (mEffectStyle == Effect.Shadow)
			{
				return 2;
			}
			if (mEffectStyle == Effect.Outline)
			{
				return 5;
			}
			if (mEffectStyle == Effect.Outline8)
			{
				return 9;
			}
			return 1;
		}
	}

	[Obsolete("Use 'overflowMethod == UILabel.Overflow.ShrinkContent' instead")]
	public bool shrinkToFit
	{
		get
		{
			return mOverflow == Overflow.ShrinkContent;
		}
		set
		{
			if (value)
			{
				overflowMethod = Overflow.ShrinkContent;
			}
		}
	}

	public string processedText
	{
		get
		{
			if (mLastWidth != mWidth || mLastHeight != mHeight)
			{
				mLastWidth = mWidth;
				mLastHeight = mHeight;
				mShouldBeProcessed = true;
			}
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return mProcessedText;
		}
	}

	public Vector2 printedSize
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return mCalculatedSize;
		}
	}

	public override Vector2 localSize
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return base.localSize;
		}
	}

	private bool isValid
	{
		get
		{
			if (!(mFont != null))
			{
				return mTrueTypeFont != null;
			}
			return true;
		}
	}

	public Modifier modifier
	{
		get
		{
			return mModifier;
		}
		set
		{
			if (mModifier != value)
			{
				mModifier = value;
				MarkAsChanged();
				ProcessAndRequest();
			}
		}
	}

	public string printedText
	{
		get
		{
			if (!string.IsNullOrEmpty(mText))
			{
				if (mModifier == Modifier.None)
				{
					return mText;
				}
				if (mModifier == Modifier.ToLowercase)
				{
					return mText.ToLower();
				}
				if (mModifier == Modifier.ToUppercase)
				{
					return mText.ToUpper();
				}
				if (mModifier == Modifier.Custom && customModifier != null)
				{
					return customModifier(mText);
				}
			}
			return mText;
		}
	}

	protected override void OnInit()
	{
		base.OnInit();
		mList.Add(this);
		SetActiveFont(trueTypeFont);
	}

	protected override void OnDisable()
	{
		SetActiveFont(null);
		mList.Remove(this);
		base.OnDisable();
	}

	protected void SetActiveFont(Font fnt)
	{
		if (!(mActiveTTF != fnt))
		{
			return;
		}
		Font font = mActiveTTF;
		if (font != null && mFontUsage.TryGetValue(font, out int value))
		{
			value = Mathf.Max(0, --value);
			if (value == 0)
			{
				mFontUsage.Remove(font);
			}
			else
			{
				mFontUsage[font] = value;
			}
		}
		mActiveTTF = fnt;
		if (fnt != null)
		{
			int num = 0;
			num = (mFontUsage[fnt] = num + 1);
		}
	}

	private static void OnFontChanged(Font font)
	{
		for (int i = 0; i < mList.size; i++)
		{
			UILabel uILabel = mList[i];
			if (!(uILabel != null))
			{
				continue;
			}
			Font trueTypeFont = uILabel.trueTypeFont;
			if (trueTypeFont == font)
			{
				trueTypeFont.RequestCharactersInTexture(uILabel.mText, uILabel.mFinalFontSize, uILabel.mFontStyle);
				uILabel.MarkAsChanged();
				if (uILabel.panel == null)
				{
					uILabel.CreatePanel();
				}
				if (mTempDrawcalls == null)
				{
					mTempDrawcalls = new BetterList<UIDrawCall>();
				}
				if (uILabel.drawCall != null && !mTempDrawcalls.Contains(uILabel.drawCall))
				{
					mTempDrawcalls.Add(uILabel.drawCall);
				}
			}
		}
		if (mTempDrawcalls == null)
		{
			return;
		}
		int j = 0;
		for (int size = mTempDrawcalls.size; j < size; j++)
		{
			UIDrawCall uIDrawCall = mTempDrawcalls[j];
			if (uIDrawCall.panel != null)
			{
				uIDrawCall.panel.FillDrawCall(uIDrawCall);
			}
		}
		mTempDrawcalls.Clear();
	}

	public override Vector3[] GetSides(Transform relativeTo)
	{
		if (shouldBeProcessed)
		{
			ProcessText();
		}
		return base.GetSides(relativeTo);
	}

	protected override void UpgradeFrom265()
	{
		ProcessText(legacyMode: true);
		if (mShrinkToFit)
		{
			overflowMethod = Overflow.ShrinkContent;
			mMaxLineCount = 0;
		}
		if (mMaxLineWidth != 0)
		{
			base.width = mMaxLineWidth;
			overflowMethod = ((mMaxLineCount > 0) ? Overflow.ResizeHeight : Overflow.ShrinkContent);
		}
		else
		{
			overflowMethod = Overflow.ResizeFreely;
		}
		if (mMaxLineHeight != 0)
		{
			base.height = mMaxLineHeight;
		}
		if (mFont != null)
		{
			int defaultSize = mFont.defaultSize;
			if (base.height < defaultSize)
			{
				base.height = defaultSize;
			}
			fontSize = defaultSize;
		}
		mMaxLineWidth = 0;
		mMaxLineHeight = 0;
		mShrinkToFit = false;
		NGUITools.UpdateWidgetCollider(base.gameObject, considerInactive: true);
	}

	protected override void OnAnchor()
	{
		if (mOverflow == Overflow.ResizeFreely)
		{
			if (base.isFullyAnchored)
			{
				mOverflow = Overflow.ShrinkContent;
			}
		}
		else if (mOverflow == Overflow.ResizeHeight && topAnchor.target != null && bottomAnchor.target != null)
		{
			mOverflow = Overflow.ShrinkContent;
		}
		base.OnAnchor();
	}

	private void ProcessAndRequest()
	{
		if (ambigiousFont != null)
		{
			ProcessText();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (!mTexRebuildAdded)
		{
			mTexRebuildAdded = true;
			Font.textureRebuilt += OnFontChanged;
		}
	}

	protected override void OnStart()
	{
		base.OnStart();
		if (mLineWidth > 0f)
		{
			mMaxLineWidth = Mathf.RoundToInt(mLineWidth);
			mLineWidth = 0f;
		}
		if (!mMultiline)
		{
			mMaxLineCount = 1;
			mMultiline = true;
		}
		mPremultiply = (material != null && material.shader != null && material.shader.name.Contains("Premultiplied"));
		ProcessAndRequest();
	}

	public override void MarkAsChanged()
	{
		shouldBeProcessed = true;
		base.MarkAsChanged();
	}

	public void ProcessText(bool legacyMode = false, bool full = true)
	{
		if (!isValid)
		{
			return;
		}
		mChanged = true;
		shouldBeProcessed = false;
		float num = mDrawRegion.z - mDrawRegion.x;
		float num2 = mDrawRegion.w - mDrawRegion.y;
		NGUIText.rectWidth = ((!legacyMode) ? base.width : ((mMaxLineWidth != 0) ? mMaxLineWidth : 1000000));
		NGUIText.rectHeight = ((!legacyMode) ? base.height : ((mMaxLineHeight != 0) ? mMaxLineHeight : 1000000));
		NGUIText.regionWidth = ((num != 1f) ? Mathf.RoundToInt((float)NGUIText.rectWidth * num) : NGUIText.rectWidth);
		NGUIText.regionHeight = ((num2 != 1f) ? Mathf.RoundToInt((float)NGUIText.rectHeight * num2) : NGUIText.rectHeight);
		mFinalFontSize = Mathf.Abs(legacyMode ? Mathf.RoundToInt(base.cachedTransform.localScale.x) : defaultFontSize);
		mScale = 1f;
		if (NGUIText.regionWidth < 1 || NGUIText.regionHeight < 0)
		{
			mProcessedText = "";
			return;
		}
		bool flag = trueTypeFont != null;
		if (flag && this.keepCrisp)
		{
			UIRoot root = base.root;
			if (root != null)
			{
				mDensity = ((root != null) ? root.pixelSizeAdjustment : 1f);
			}
		}
		else
		{
			mDensity = 1f;
		}
		if (full)
		{
			UpdateNGUIText();
		}
		if (mOverflow == Overflow.ResizeFreely)
		{
			NGUIText.rectWidth = 1000000;
			NGUIText.regionWidth = 1000000;
			if (mOverflowWidth > 0)
			{
				NGUIText.rectWidth = Mathf.Min(NGUIText.rectWidth, mOverflowWidth);
				NGUIText.regionWidth = Mathf.Min(NGUIText.regionWidth, mOverflowWidth);
			}
		}
		if (mOverflow == Overflow.ResizeFreely || mOverflow == Overflow.ResizeHeight)
		{
			NGUIText.rectHeight = 1000000;
			NGUIText.regionHeight = 1000000;
		}
		if (mFinalFontSize > 0)
		{
			bool keepCrisp = this.keepCrisp;
			int num3 = mFinalFontSize;
			while (num3 > 0)
			{
				if (keepCrisp)
				{
					mFinalFontSize = num3;
					NGUIText.fontSize = mFinalFontSize;
				}
				else
				{
					mScale = (float)num3 / (float)mFinalFontSize;
					NGUIText.fontScale = (flag ? mScale : ((float)mFontSize / (float)mFont.defaultSize * mScale));
				}
				NGUIText.Update(request: false);
				bool flag2 = NGUIText.WrapText(printedText, out mProcessedText, keepCharCount: false, wrapLineColors: false, mOverflow == Overflow.ClampContent && mOverflowEllipsis);
				if (mOverflow == Overflow.ShrinkContent && !flag2)
				{
					if (--num3 <= 1)
					{
						break;
					}
					num3--;
					continue;
				}
				if (mOverflow == Overflow.ResizeFreely)
				{
					mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);
					int num4 = Mathf.Max(minWidth, Mathf.RoundToInt(mCalculatedSize.x));
					if (num != 1f)
					{
						num4 = Mathf.RoundToInt((float)num4 / num);
					}
					int num5 = Mathf.Max(minHeight, Mathf.RoundToInt(mCalculatedSize.y));
					if (num2 != 1f)
					{
						num5 = Mathf.RoundToInt((float)num5 / num2);
					}
					if ((num4 & 1) == 1)
					{
						num4++;
					}
					if ((num5 & 1) == 1)
					{
						num5++;
					}
					if (mWidth != num4 || mHeight != num5)
					{
						mWidth = num4;
						mHeight = num5;
						if (onChange != null)
						{
							onChange();
						}
					}
				}
				else if (mOverflow == Overflow.ResizeHeight)
				{
					mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);
					int num6 = Mathf.Max(minHeight, Mathf.RoundToInt(mCalculatedSize.y));
					if (num2 != 1f)
					{
						num6 = Mathf.RoundToInt((float)num6 / num2);
					}
					if ((num6 & 1) == 1)
					{
						num6++;
					}
					if (mHeight != num6)
					{
						mHeight = num6;
						if (onChange != null)
						{
							onChange();
						}
					}
				}
				else
				{
					mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);
				}
				if (legacyMode)
				{
					base.width = Mathf.RoundToInt(mCalculatedSize.x);
					base.height = Mathf.RoundToInt(mCalculatedSize.y);
					base.cachedTransform.localScale = Vector3.one;
				}
				break;
			}
		}
		else
		{
			base.cachedTransform.localScale = Vector3.one;
			mProcessedText = "";
			mScale = 1f;
		}
		if (full)
		{
			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;
		}
	}

	public override void MakePixelPerfect()
	{
		if (ambigiousFont != null)
		{
			Vector3 localPosition = base.cachedTransform.localPosition;
			localPosition.x = Mathf.RoundToInt(localPosition.x);
			localPosition.y = Mathf.RoundToInt(localPosition.y);
			localPosition.z = Mathf.RoundToInt(localPosition.z);
			base.cachedTransform.localPosition = localPosition;
			base.cachedTransform.localScale = Vector3.one;
			if (mOverflow == Overflow.ResizeFreely)
			{
				AssumeNaturalSize();
				return;
			}
			int width = base.width;
			int height = base.height;
			Overflow overflow = mOverflow;
			if (overflow != Overflow.ResizeHeight)
			{
				mWidth = 100000;
			}
			mHeight = 100000;
			mOverflow = Overflow.ShrinkContent;
			ProcessText();
			mOverflow = overflow;
			int a = Mathf.RoundToInt(mCalculatedSize.x);
			int a2 = Mathf.RoundToInt(mCalculatedSize.y);
			a = Mathf.Max(a, base.minWidth);
			a2 = Mathf.Max(a2, base.minHeight);
			if ((a & 1) == 1)
			{
				a++;
			}
			if ((a2 & 1) == 1)
			{
				a2++;
			}
			mWidth = Mathf.Max(width, a);
			mHeight = Mathf.Max(height, a2);
			MarkAsChanged();
		}
		else
		{
			base.MakePixelPerfect();
		}
	}

	public void AssumeNaturalSize()
	{
		if (ambigiousFont != null)
		{
			mWidth = 100000;
			mHeight = 100000;
			ProcessText();
			mWidth = Mathf.RoundToInt(mCalculatedSize.x);
			mHeight = Mathf.RoundToInt(mCalculatedSize.y);
			if ((mWidth & 1) == 1)
			{
				mWidth++;
			}
			if ((mHeight & 1) == 1)
			{
				mHeight++;
			}
			MarkAsChanged();
		}
	}

	[Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex(Vector3 worldPos)
	{
		return GetCharacterIndexAtPosition(worldPos, precise: false);
	}

	[Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex(Vector2 localPos)
	{
		return GetCharacterIndexAtPosition(localPos, precise: false);
	}

	public int GetCharacterIndexAtPosition(Vector3 worldPos, bool precise)
	{
		Vector2 localPos = base.cachedTransform.InverseTransformPoint(worldPos);
		return GetCharacterIndexAtPosition(localPos, precise);
	}

	public int GetCharacterIndexAtPosition(Vector2 localPos, bool precise)
	{
		if (isValid)
		{
			string processedText = this.processedText;
			if (string.IsNullOrEmpty(processedText))
			{
				return 0;
			}
			UpdateNGUIText();
			if (precise)
			{
				NGUIText.PrintExactCharacterPositions(processedText, mTempVerts, mTempIndices);
			}
			else
			{
				NGUIText.PrintApproximateCharacterPositions(processedText, mTempVerts, mTempIndices);
			}
			if (mTempVerts.Count > 0)
			{
				ApplyOffset(mTempVerts, 0);
				int result = precise ? NGUIText.GetExactCharacterIndex(mTempVerts, mTempIndices, localPos) : NGUIText.GetApproximateCharacterIndex(mTempVerts, mTempIndices, localPos);
				mTempVerts.Clear();
				mTempIndices.Clear();
				NGUIText.bitmapFont = null;
				NGUIText.dynamicFont = null;
				return result;
			}
			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;
		}
		return 0;
	}

	public string GetWordAtPosition(Vector3 worldPos)
	{
		int characterIndexAtPosition = GetCharacterIndexAtPosition(worldPos, precise: true);
		return GetWordAtCharacterIndex(characterIndexAtPosition);
	}

	public string GetWordAtPosition(Vector2 localPos)
	{
		int characterIndexAtPosition = GetCharacterIndexAtPosition(localPos, precise: true);
		return GetWordAtCharacterIndex(characterIndexAtPosition);
	}

	public string GetWordAtCharacterIndex(int characterIndex)
	{
		string printedText = this.printedText;
		if (characterIndex != -1 && characterIndex < printedText.Length)
		{
			int num = printedText.LastIndexOfAny(new char[2]
			{
				' ',
				'\n'
			}, characterIndex) + 1;
			int num2 = printedText.IndexOfAny(new char[4]
			{
				' ',
				'\n',
				',',
				'.'
			}, characterIndex);
			if (num2 == -1)
			{
				num2 = printedText.Length;
			}
			if (num != num2)
			{
				int num3 = num2 - num;
				if (num3 > 0)
				{
					return NGUIText.StripSymbols(printedText.Substring(num, num3));
				}
			}
		}
		return null;
	}

	public string GetUrlAtPosition(Vector3 worldPos)
	{
		return GetUrlAtCharacterIndex(GetCharacterIndexAtPosition(worldPos, precise: true));
	}

	public string GetUrlAtPosition(Vector2 localPos)
	{
		return GetUrlAtCharacterIndex(GetCharacterIndexAtPosition(localPos, precise: true));
	}

	public string GetUrlAtCharacterIndex(int characterIndex)
	{
		string printedText = this.printedText;
		if (characterIndex != -1 && characterIndex < printedText.Length - 6)
		{
			int num = (printedText[characterIndex] != '[' || printedText[characterIndex + 1] != 'u' || printedText[characterIndex + 2] != 'r' || printedText[characterIndex + 3] != 'l' || printedText[characterIndex + 4] != '=') ? printedText.LastIndexOf("[url=", characterIndex) : characterIndex;
			if (num == -1)
			{
				return null;
			}
			num += 5;
			int num2 = printedText.IndexOf("]", num);
			if (num2 == -1)
			{
				return null;
			}
			int num3 = printedText.IndexOf("[/url]", num2);
			if (num3 == -1 || characterIndex <= num3)
			{
				return printedText.Substring(num, num2 - num);
			}
		}
		return null;
	}

	public int GetCharacterIndex(int currentIndex, KeyCode key)
	{
		if (isValid)
		{
			string processedText = this.processedText;
			if (string.IsNullOrEmpty(processedText))
			{
				return 0;
			}
			int defaultFontSize = this.defaultFontSize;
			UpdateNGUIText();
			NGUIText.PrintApproximateCharacterPositions(processedText, mTempVerts, mTempIndices);
			if (mTempVerts.Count > 0)
			{
				ApplyOffset(mTempVerts, 0);
				int i = 0;
				for (int count = mTempIndices.Count; i < count; i++)
				{
					if (mTempIndices[i] == currentIndex)
					{
						Vector2 pos = mTempVerts[i];
						switch (key)
						{
						case KeyCode.UpArrow:
							pos.y += (float)defaultFontSize + effectiveSpacingY;
							break;
						case KeyCode.DownArrow:
							pos.y -= (float)defaultFontSize + effectiveSpacingY;
							break;
						case KeyCode.Home:
							pos.x -= 1000f;
							break;
						case KeyCode.End:
							pos.x += 1000f;
							break;
						}
						int approximateCharacterIndex = NGUIText.GetApproximateCharacterIndex(mTempVerts, mTempIndices, pos);
						if (approximateCharacterIndex == currentIndex)
						{
							break;
						}
						mTempVerts.Clear();
						mTempIndices.Clear();
						return approximateCharacterIndex;
					}
				}
				mTempVerts.Clear();
				mTempIndices.Clear();
			}
			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;
			switch (key)
			{
			case KeyCode.UpArrow:
			case KeyCode.Home:
				return 0;
			case KeyCode.DownArrow:
			case KeyCode.End:
				return processedText.Length;
			}
		}
		return currentIndex;
	}

	public void PrintOverlay(int start, int end, UIGeometry caret, UIGeometry highlight, Color caretColor, Color highlightColor)
	{
		caret?.Clear();
		highlight?.Clear();
		if (!isValid)
		{
			return;
		}
		string processedText = this.processedText;
		UpdateNGUIText();
		int count = caret.verts.Count;
		Vector2 item = new Vector2(0.5f, 0.5f);
		float finalAlpha = base.finalAlpha;
		if (highlight != null && start != end)
		{
			int count2 = highlight.verts.Count;
			NGUIText.PrintCaretAndSelection(processedText, start, end, caret.verts, highlight.verts);
			if (highlight.verts.Count > count2)
			{
				ApplyOffset(highlight.verts, count2);
				Color item2 = new Color(highlightColor.r, highlightColor.g, highlightColor.b, highlightColor.a * finalAlpha);
				int i = count2;
				for (int count3 = highlight.verts.Count; i < count3; i++)
				{
					highlight.uvs.Add(item);
					highlight.cols.Add(item2);
				}
			}
		}
		else
		{
			NGUIText.PrintCaretAndSelection(processedText, start, end, caret.verts, null);
		}
		ApplyOffset(caret.verts, count);
		Color item3 = new Color(caretColor.r, caretColor.g, caretColor.b, caretColor.a * finalAlpha);
		int j = count;
		for (int count4 = caret.verts.Count; j < count4; j++)
		{
			caret.uvs.Add(item);
			caret.cols.Add(item3);
		}
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
	}

	public override void OnFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (!isValid)
		{
			return;
		}
		int num = verts.Count;
		Color color = base.color;
		color.a = finalAlpha;
		if (mFont != null && mFont.premultipliedAlphaShader)
		{
			color = NGUITools.ApplyPMA(color);
		}
		string processedText = this.processedText;
		int count = verts.Count;
		UpdateNGUIText();
		NGUIText.tint = color;
		NGUIText.Print(processedText, verts, uvs, cols);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
		Vector2 vector = ApplyOffset(verts, count);
		if (mFont != null && mFont.packedFontShader)
		{
			return;
		}
		if (effectStyle != 0)
		{
			int count2 = verts.Count;
			vector.x = mEffectDistance.x;
			vector.y = mEffectDistance.y;
			ApplyShadow(verts, uvs, cols, num, count2, vector.x, 0f - vector.y);
			if (effectStyle == Effect.Outline || effectStyle == Effect.Outline8)
			{
				num = count2;
				count2 = verts.Count;
				ApplyShadow(verts, uvs, cols, num, count2, 0f - vector.x, vector.y);
				num = count2;
				count2 = verts.Count;
				ApplyShadow(verts, uvs, cols, num, count2, vector.x, vector.y);
				num = count2;
				count2 = verts.Count;
				ApplyShadow(verts, uvs, cols, num, count2, 0f - vector.x, 0f - vector.y);
				if (effectStyle == Effect.Outline8)
				{
					num = count2;
					count2 = verts.Count;
					ApplyShadow(verts, uvs, cols, num, count2, 0f - vector.x, 0f);
					num = count2;
					count2 = verts.Count;
					ApplyShadow(verts, uvs, cols, num, count2, vector.x, 0f);
					num = count2;
					count2 = verts.Count;
					ApplyShadow(verts, uvs, cols, num, count2, 0f, vector.y);
					num = count2;
					count2 = verts.Count;
					ApplyShadow(verts, uvs, cols, num, count2, 0f, 0f - vector.y);
				}
			}
		}
		if (onPostFill != null)
		{
			onPostFill(this, num, verts, uvs, cols);
		}
	}

	public Vector2 ApplyOffset(List<Vector3> verts, int start)
	{
		Vector2 pivotOffset = base.pivotOffset;
		float f = Mathf.Lerp(0f, -mWidth, pivotOffset.x);
		float f2 = Mathf.Lerp(mHeight, 0f, pivotOffset.y) + Mathf.Lerp(mCalculatedSize.y - (float)mHeight, 0f, pivotOffset.y);
		f = Mathf.Round(f);
		f2 = Mathf.Round(f2);
		int i = start;
		for (int count = verts.Count; i < count; i++)
		{
			Vector3 value = verts[i];
			value.x += f;
			value.y += f2;
			verts[i] = value;
		}
		return new Vector2(f, f2);
	}

	public void ApplyShadow(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, int start, int end, float x, float y)
	{
		Color color = mEffectColor;
		color.a *= finalAlpha;
		if (bitmapFont != null && bitmapFont.premultipliedAlphaShader)
		{
			color = NGUITools.ApplyPMA(color);
		}
		Color value = color;
		for (int i = start; i < end; i++)
		{
			verts.Add(verts[i]);
			uvs.Add(uvs[i]);
			cols.Add(cols[i]);
			Vector3 value2 = verts[i];
			value2.x += x;
			value2.y += y;
			verts[i] = value2;
			Color color2 = cols[i];
			if (color2.a == 1f)
			{
				cols[i] = value;
				continue;
			}
			Color value3 = color;
			value3.a = color2.a * color.a;
			cols[i] = value3;
		}
	}

	public int CalculateOffsetToFit(string text)
	{
		UpdateNGUIText();
		NGUIText.encoding = false;
		NGUIText.symbolStyle = NGUIText.SymbolStyle.None;
		int result = NGUIText.CalculateOffsetToFit(text);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
		return result;
	}

	public void SetCurrentProgress()
	{
		if (UIProgressBar.current != null)
		{
			text = UIProgressBar.current.value.ToString("F");
		}
	}

	public void SetCurrentPercent()
	{
		if (UIProgressBar.current != null)
		{
			text = Mathf.RoundToInt(UIProgressBar.current.value * 100f) + "%";
		}
	}

	public void SetCurrentSelection()
	{
		if (UIPopupList.current != null)
		{
			text = (UIPopupList.current.isLocalized ? Localization.Get(UIPopupList.current.value) : UIPopupList.current.value);
		}
	}

	public bool Wrap(string text, out string final)
	{
		return Wrap(text, out final, 1000000);
	}

	public bool Wrap(string text, out string final, int height)
	{
		UpdateNGUIText();
		NGUIText.rectHeight = height;
		NGUIText.regionHeight = height;
		bool result = NGUIText.WrapText(text, out final);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
		return result;
	}

	public void UpdateNGUIText()
	{
		Font trueTypeFont = this.trueTypeFont;
		bool flag = trueTypeFont != null;
		NGUIText.fontSize = mFinalFontSize;
		NGUIText.fontStyle = mFontStyle;
		NGUIText.rectWidth = mWidth;
		NGUIText.rectHeight = mHeight;
		NGUIText.regionWidth = Mathf.RoundToInt((float)mWidth * (mDrawRegion.z - mDrawRegion.x));
		NGUIText.regionHeight = Mathf.RoundToInt((float)mHeight * (mDrawRegion.w - mDrawRegion.y));
		NGUIText.gradient = (mApplyGradient && (mFont == null || !mFont.packedFontShader));
		NGUIText.gradientTop = mGradientTop;
		NGUIText.gradientBottom = mGradientBottom;
		NGUIText.encoding = mEncoding;
		NGUIText.premultiply = mPremultiply;
		NGUIText.symbolStyle = mSymbols;
		NGUIText.maxLines = mMaxLineCount;
		NGUIText.spacingX = effectiveSpacingX;
		NGUIText.spacingY = effectiveSpacingY;
		NGUIText.fontScale = (flag ? mScale : ((float)mFontSize / (float)mFont.defaultSize * mScale));
		if (mFont != null)
		{
			NGUIText.bitmapFont = mFont;
			while (true)
			{
				UIFont replacement = NGUIText.bitmapFont.replacement;
				if (replacement == null)
				{
					break;
				}
				NGUIText.bitmapFont = replacement;
			}
			if (NGUIText.bitmapFont.isDynamic)
			{
				NGUIText.dynamicFont = NGUIText.bitmapFont.dynamicFont;
				NGUIText.bitmapFont = null;
			}
			else
			{
				NGUIText.dynamicFont = null;
			}
		}
		else
		{
			NGUIText.dynamicFont = trueTypeFont;
			NGUIText.bitmapFont = null;
		}
		if (flag && keepCrisp)
		{
			UIRoot root = base.root;
			if (root != null)
			{
				NGUIText.pixelDensity = ((root != null) ? root.pixelSizeAdjustment : 1f);
			}
		}
		else
		{
			NGUIText.pixelDensity = 1f;
		}
		if (mDensity != NGUIText.pixelDensity)
		{
			ProcessText(legacyMode: false, full: false);
			NGUIText.rectWidth = mWidth;
			NGUIText.rectHeight = mHeight;
			NGUIText.regionWidth = Mathf.RoundToInt((float)mWidth * (mDrawRegion.z - mDrawRegion.x));
			NGUIText.regionHeight = Mathf.RoundToInt((float)mHeight * (mDrawRegion.w - mDrawRegion.y));
		}
		if (alignment == NGUIText.Alignment.Automatic)
		{
			switch (base.pivot)
			{
			case Pivot.TopLeft:
			case Pivot.Left:
			case Pivot.BottomLeft:
				NGUIText.alignment = NGUIText.Alignment.Left;
				break;
			case Pivot.TopRight:
			case Pivot.Right:
			case Pivot.BottomRight:
				NGUIText.alignment = NGUIText.Alignment.Right;
				break;
			default:
				NGUIText.alignment = NGUIText.Alignment.Center;
				break;
			}
		}
		else
		{
			NGUIText.alignment = alignment;
		}
		NGUIText.Update();
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused && mTrueTypeFont != null)
		{
			Invalidate(includeChildren: false);
		}
	}
}
