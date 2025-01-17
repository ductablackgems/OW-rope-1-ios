using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Input Field")]
public class UIInput : MonoBehaviour
{
	public enum InputType
	{
		Standard,
		AutoCorrect,
		Password
	}

	public enum Validation
	{
		None,
		Integer,
		Float,
		Alphanumeric,
		Username,
		Name,
		Filename
	}

	public enum KeyboardType
	{
		Default,
		ASCIICapable,
		NumbersAndPunctuation,
		URL,
		NumberPad,
		PhonePad,
		NamePhonePad,
		EmailAddress
	}

	public enum OnReturnKey
	{
		Default,
		Submit,
		NewLine
	}

	public delegate char OnValidate(string text, int charIndex, char addedChar);

	public static UIInput current;

	public static UIInput selection;

	public UILabel label;

	public InputType inputType;

	public OnReturnKey onReturnKey;

	public KeyboardType keyboardType;

	public bool hideInput;

	[NonSerialized]
	public bool selectAllTextOnFocus = true;

	public bool submitOnUnselect;

	public Validation validation;

	public int characterLimit;

	public string savedAs;

	[HideInInspector]
	[SerializeField]
	private GameObject selectOnTab;

	public Color activeTextColor = Color.white;

	public Color caretColor = new Color(1f, 1f, 1f, 0.8f);

	public Color selectionColor = new Color(1f, 223f / 255f, 47f / 85f, 0.5f);

	public List<EventDelegate> onSubmit = new List<EventDelegate>();

	public List<EventDelegate> onChange = new List<EventDelegate>();

	public OnValidate onValidate;

	[SerializeField]
	[HideInInspector]
	protected string mValue;

	[NonSerialized]
	protected string mDefaultText = "";

	[NonSerialized]
	protected Color mDefaultColor = Color.white;

	[NonSerialized]
	protected float mPosition;

	[NonSerialized]
	protected bool mDoInit = true;

	[NonSerialized]
	protected NGUIText.Alignment mAlignment = NGUIText.Alignment.Left;

	[NonSerialized]
	protected bool mLoadSavedValue = true;

	protected static int mDrawStart = 0;

	protected static string mLastIME = "";

	protected static TouchScreenKeyboard mKeyboard;

	private static bool mWaitForKeyboard = false;

	[NonSerialized]
	protected int mSelectionStart;

	[NonSerialized]
	protected int mSelectionEnd;

	[NonSerialized]
	protected UITexture mHighlight;

	[NonSerialized]
	protected UITexture mCaret;

	[NonSerialized]
	protected Texture2D mBlankTex;

	[NonSerialized]
	protected float mNextBlink;

	[NonSerialized]
	protected float mLastAlpha;

	[NonSerialized]
	protected string mCached = "";

	[NonSerialized]
	protected int mSelectMe = -1;

	[NonSerialized]
	protected int mSelectTime = -1;

	[NonSerialized]
	protected bool mStarted;

	[NonSerialized]
	private UICamera mCam;

	[NonSerialized]
	private bool mEllipsis;

	private static int mIgnoreKey = 0;

	public string defaultText
	{
		get
		{
			if (mDoInit)
			{
				Init();
			}
			return mDefaultText;
		}
		set
		{
			if (mDoInit)
			{
				Init();
			}
			mDefaultText = value;
			UpdateLabel();
		}
	}

	public Color defaultColor
	{
		get
		{
			if (mDoInit)
			{
				Init();
			}
			return mDefaultColor;
		}
		set
		{
			mDefaultColor = value;
			if (!isSelected)
			{
				label.color = value;
			}
		}
	}

	public bool inputShouldBeHidden
	{
		get
		{
			if (hideInput && label != null && !label.multiLine)
			{
				return inputType != InputType.Password;
			}
			return false;
		}
	}

	[Obsolete("Use UIInput.value instead")]
	public string text
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public string value
	{
		get
		{
			if (mDoInit)
			{
				Init();
			}
			return mValue;
		}
		set
		{
			Set(value);
		}
	}

	[Obsolete("Use UIInput.isSelected instead")]
	public bool selected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
		}
	}

	public bool isSelected
	{
		get
		{
			return selection == this;
		}
		set
		{
			if (!value)
			{
				if (isSelected)
				{
					UICamera.selectedObject = null;
				}
			}
			else
			{
				UICamera.selectedObject = base.gameObject;
			}
		}
	}

	public int cursorPosition
	{
		get
		{
			if (mKeyboard != null && !inputShouldBeHidden)
			{
				return value.Length;
			}
			if (!isSelected)
			{
				return value.Length;
			}
			return mSelectionEnd;
		}
		set
		{
			if (isSelected && (mKeyboard == null || inputShouldBeHidden))
			{
				mSelectionEnd = value;
				UpdateLabel();
			}
		}
	}

	public int selectionStart
	{
		get
		{
			if (mKeyboard != null && !inputShouldBeHidden)
			{
				return 0;
			}
			if (!isSelected)
			{
				return value.Length;
			}
			return mSelectionStart;
		}
		set
		{
			if (isSelected && (mKeyboard == null || inputShouldBeHidden))
			{
				mSelectionStart = value;
				UpdateLabel();
			}
		}
	}

	public int selectionEnd
	{
		get
		{
			if (mKeyboard != null && !inputShouldBeHidden)
			{
				return value.Length;
			}
			if (!isSelected)
			{
				return value.Length;
			}
			return mSelectionEnd;
		}
		set
		{
			if (isSelected && (mKeyboard == null || inputShouldBeHidden))
			{
				mSelectionEnd = value;
				UpdateLabel();
			}
		}
	}

	public UITexture caret => mCaret;

	public void Set(string value, bool notify = true)
	{
		if (mDoInit)
		{
			Init();
		}
		if (value == this.value)
		{
			return;
		}
		mDrawStart = 0;
		value = Validate(value);
		if (isSelected && mKeyboard != null && mCached != value)
		{
			mKeyboard.text = value;
			mCached = value;
		}
		if (!(mValue != value))
		{
			return;
		}
		mValue = value;
		mLoadSavedValue = false;
		if (isSelected)
		{
			if (string.IsNullOrEmpty(value))
			{
				mSelectionStart = 0;
				mSelectionEnd = 0;
			}
			else
			{
				mSelectionStart = value.Length;
				mSelectionEnd = mSelectionStart;
			}
		}
		else if (mStarted)
		{
			SaveToPlayerPrefs(value);
		}
		UpdateLabel();
		if (notify)
		{
			ExecuteOnChange();
		}
	}

	public string Validate(string val)
	{
		if (string.IsNullOrEmpty(val))
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder(val.Length);
		for (int i = 0; i < val.Length; i++)
		{
			char c = val[i];
			if (onValidate != null)
			{
				c = onValidate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			else if (validation != 0)
			{
				c = Validate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			if (c != 0)
			{
				stringBuilder.Append(c);
			}
		}
		if (characterLimit > 0 && stringBuilder.Length > characterLimit)
		{
			return stringBuilder.ToString(0, characterLimit);
		}
		return stringBuilder.ToString();
	}

	public void Start()
	{
		if (mStarted)
		{
			return;
		}
		if (selectOnTab != null)
		{
			if (GetComponent<UIKeyNavigation>() == null)
			{
				base.gameObject.AddComponent<UIKeyNavigation>().onDown = selectOnTab;
			}
			selectOnTab = null;
			NGUITools.SetDirty(this);
		}
		if (mLoadSavedValue && !string.IsNullOrEmpty(savedAs))
		{
			LoadValue();
		}
		else
		{
			value = mValue.Replace("\\n", "\n");
		}
		mStarted = true;
	}

	protected void Init()
	{
		if (mDoInit && label != null)
		{
			mDoInit = false;
			mDefaultText = label.text;
			mDefaultColor = label.color;
			mEllipsis = label.overflowEllipsis;
			if (label.alignment == NGUIText.Alignment.Justified)
			{
				label.alignment = NGUIText.Alignment.Left;
				UnityEngine.Debug.LogWarning("Input fields using labels with justified alignment are not supported at this time", this);
			}
			mAlignment = label.alignment;
			mPosition = label.cachedTransform.localPosition.x;
			UpdateLabel();
		}
	}

	protected void SaveToPlayerPrefs(string val)
	{
		if (!string.IsNullOrEmpty(savedAs))
		{
			if (string.IsNullOrEmpty(val))
			{
				PlayerPrefs.DeleteKey(savedAs);
			}
			else
			{
				PlayerPrefs.SetString(savedAs, val);
			}
		}
	}

	protected virtual void OnSelect(bool isSelected)
	{
		if (isSelected)
		{
			if (label != null)
			{
				label.supportEncoding = false;
			}
			OnSelectEvent();
		}
		else
		{
			OnDeselectEvent();
		}
	}

	protected void OnSelectEvent()
	{
		mSelectTime = Time.frameCount;
		selection = this;
		if (mDoInit)
		{
			Init();
		}
		if (label != null)
		{
			mEllipsis = label.overflowEllipsis;
			label.overflowEllipsis = false;
		}
		if (label != null && NGUITools.GetActive(this))
		{
			mSelectMe = Time.frameCount;
		}
	}

	protected void OnDeselectEvent()
	{
		if (mDoInit)
		{
			Init();
		}
		if (label != null)
		{
			label.overflowEllipsis = mEllipsis;
		}
		if (label != null && NGUITools.GetActive(this))
		{
			mValue = value;
			if (mKeyboard != null)
			{
				mWaitForKeyboard = false;
				mKeyboard.active = false;
				mKeyboard = null;
			}
			if (string.IsNullOrEmpty(mValue))
			{
				label.text = mDefaultText;
				label.color = mDefaultColor;
			}
			else
			{
				label.text = mValue;
			}
			Input.imeCompositionMode = IMECompositionMode.Auto;
			label.alignment = mAlignment;
		}
		selection = null;
		UpdateLabel();
		if (submitOnUnselect)
		{
			Submit();
		}
	}

	protected virtual void Update()
	{
		if (!isSelected || mSelectTime == Time.frameCount)
		{
			return;
		}
		if (mDoInit)
		{
			Init();
		}
		if (mWaitForKeyboard)
		{
			if (mKeyboard != null && !mKeyboard.active)
			{
				return;
			}
			mWaitForKeyboard = false;
		}
		if (mSelectMe != -1 && mSelectMe != Time.frameCount)
		{
			mSelectMe = -1;
			mSelectionEnd = ((!string.IsNullOrEmpty(mValue)) ? mValue.Length : 0);
			mDrawStart = 0;
			mSelectionStart = ((!selectAllTextOnFocus) ? mSelectionEnd : 0);
			label.color = activeTextColor;
			RuntimePlatform platform = Application.platform;
			if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.WP8Player || platform == RuntimePlatform.BlackBerryPlayer || platform == RuntimePlatform.MetroPlayerARM || platform == RuntimePlatform.MetroPlayerX64 || platform == RuntimePlatform.MetroPlayerX86)
			{
				TouchScreenKeyboardType touchScreenKeyboardType;
				string text;
				if (inputShouldBeHidden)
				{
					TouchScreenKeyboard.hideInput = true;
					touchScreenKeyboardType = (TouchScreenKeyboardType)keyboardType;
					text = "|";
				}
				else if (inputType == InputType.Password)
				{
					TouchScreenKeyboard.hideInput = false;
					touchScreenKeyboardType = (TouchScreenKeyboardType)keyboardType;
					text = mValue;
					mSelectionStart = mSelectionEnd;
				}
				else
				{
					TouchScreenKeyboard.hideInput = false;
					touchScreenKeyboardType = (TouchScreenKeyboardType)keyboardType;
					text = mValue;
					mSelectionStart = mSelectionEnd;
				}
				mWaitForKeyboard = true;
				mKeyboard = ((inputType == InputType.Password) ? TouchScreenKeyboard.Open(text, touchScreenKeyboardType, autocorrection: false, multiline: false, secure: true) : TouchScreenKeyboard.Open(text, touchScreenKeyboardType, !inputShouldBeHidden && inputType == InputType.AutoCorrect, label.multiLine && !hideInput, secure: false, alert: false, defaultText));
			}
			else
			{
				Vector2 vector = (UICamera.current != null && UICamera.current.cachedCamera != null) ? UICamera.current.cachedCamera.WorldToScreenPoint(label.worldCorners[0]) : label.worldCorners[0];
				vector.y = (float)Screen.height - vector.y;
				Input.imeCompositionMode = IMECompositionMode.On;
				Input.compositionCursorPos = vector;
			}
			UpdateLabel();
			if (string.IsNullOrEmpty(Input.inputString))
			{
				return;
			}
		}
		if (mKeyboard != null)
		{
			string text2 = (mKeyboard.done || !mKeyboard.active) ? mCached : mKeyboard.text;
			if (inputShouldBeHidden)
			{
				if (text2 != "|")
				{
					if (!string.IsNullOrEmpty(text2))
					{
						Insert(text2.Substring(1));
					}
					else if (!mKeyboard.done && mKeyboard.active)
					{
						DoBackspace();
					}
					mKeyboard.text = "|";
				}
			}
			else if (mCached != text2)
			{
				mCached = text2;
				if (!mKeyboard.done && mKeyboard.active)
				{
					value = text2;
				}
			}
			if (mKeyboard.done || !mKeyboard.active)
			{
				if (!mKeyboard.wasCanceled)
				{
					Submit();
				}
				mKeyboard = null;
				isSelected = false;
				mCached = "";
			}
		}
		else
		{
			string compositionString = Input.compositionString;
			if (string.IsNullOrEmpty(compositionString) && !string.IsNullOrEmpty(Input.inputString))
			{
				string inputString = Input.inputString;
				for (int i = 0; i < inputString.Length; i++)
				{
					char c = inputString[i];
					if (c >= ' ' && c != '\uf700' && c != '\uf701' && c != '\uf702' && c != '\uf703' && c != '\uf728')
					{
						Insert(c.ToString());
					}
				}
			}
			if (mLastIME != compositionString)
			{
				mSelectionEnd = (string.IsNullOrEmpty(compositionString) ? mSelectionStart : (mValue.Length + compositionString.Length));
				mLastIME = compositionString;
				UpdateLabel();
				ExecuteOnChange();
			}
		}
		if (mCaret != null && mNextBlink < RealTime.time)
		{
			mNextBlink = RealTime.time + 0.5f;
			mCaret.enabled = !mCaret.enabled;
		}
		if (isSelected && mLastAlpha != label.finalAlpha)
		{
			UpdateLabel();
		}
		if (mCam == null)
		{
			mCam = UICamera.FindCameraForLayer(base.gameObject.layer);
		}
		if (!(mCam != null))
		{
			return;
		}
		bool flag = false;
		if (label.multiLine)
		{
			bool flag2 = UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
			flag = ((onReturnKey != OnReturnKey.Submit) ? (!flag2) : flag2);
		}
		if (UICamera.GetKeyDown(mCam.submitKey0) || (mCam.submitKey0 == KeyCode.Return && UICamera.GetKeyDown(KeyCode.KeypadEnter)))
		{
			if (flag)
			{
				Insert("\n");
			}
			else
			{
				if (UICamera.controller.current != null)
				{
					UICamera.controller.clickNotification = UICamera.ClickNotification.None;
				}
				UICamera.currentKey = mCam.submitKey0;
				Submit();
			}
		}
		if (UICamera.GetKeyDown(mCam.submitKey1) || (mCam.submitKey1 == KeyCode.Return && UICamera.GetKeyDown(KeyCode.KeypadEnter)))
		{
			if (flag)
			{
				Insert("\n");
			}
			else
			{
				if (UICamera.controller.current != null)
				{
					UICamera.controller.clickNotification = UICamera.ClickNotification.None;
				}
				UICamera.currentKey = mCam.submitKey1;
				Submit();
			}
		}
		if (!mCam.useKeyboard && UICamera.GetKeyUp(KeyCode.Tab))
		{
			OnKey(KeyCode.Tab);
		}
	}

	private void OnKey(KeyCode key)
	{
		int frameCount = Time.frameCount;
		if (mIgnoreKey == frameCount)
		{
			return;
		}
		if (mCam != null && (key == mCam.cancelKey0 || key == mCam.cancelKey1))
		{
			mIgnoreKey = frameCount;
			isSelected = false;
		}
		else if (key == KeyCode.Tab)
		{
			mIgnoreKey = frameCount;
			isSelected = false;
			UIKeyNavigation component = GetComponent<UIKeyNavigation>();
			if (component != null)
			{
				component.OnKey(KeyCode.Tab);
			}
		}
	}

	protected void DoBackspace()
	{
		if (string.IsNullOrEmpty(mValue))
		{
			return;
		}
		if (mSelectionStart == mSelectionEnd)
		{
			if (mSelectionStart < 1)
			{
				return;
			}
			mSelectionEnd--;
		}
		Insert("");
	}

	protected virtual void Insert(string text)
	{
		string leftText = GetLeftText();
		string rightText = GetRightText();
		int length = rightText.Length;
		StringBuilder stringBuilder = new StringBuilder(leftText.Length + rightText.Length + text.Length);
		stringBuilder.Append(leftText);
		int i = 0;
		for (int length2 = text.Length; i < length2; i++)
		{
			char c = text[i];
			if (c == '\b')
			{
				DoBackspace();
				continue;
			}
			if (characterLimit > 0 && stringBuilder.Length + length >= characterLimit)
			{
				break;
			}
			if (onValidate != null)
			{
				c = onValidate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			else if (validation != 0)
			{
				c = Validate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			if (c != 0)
			{
				stringBuilder.Append(c);
			}
		}
		mSelectionStart = stringBuilder.Length;
		mSelectionEnd = mSelectionStart;
		int j = 0;
		for (int length3 = rightText.Length; j < length3; j++)
		{
			char c2 = rightText[j];
			if (onValidate != null)
			{
				c2 = onValidate(stringBuilder.ToString(), stringBuilder.Length, c2);
			}
			else if (validation != 0)
			{
				c2 = Validate(stringBuilder.ToString(), stringBuilder.Length, c2);
			}
			if (c2 != 0)
			{
				stringBuilder.Append(c2);
			}
		}
		mValue = stringBuilder.ToString();
		UpdateLabel();
		ExecuteOnChange();
	}

	protected string GetLeftText()
	{
		int num = Mathf.Min(mSelectionStart, mSelectionEnd, mValue.Length);
		if (!string.IsNullOrEmpty(mValue) && num >= 0)
		{
			return mValue.Substring(0, num);
		}
		return "";
	}

	protected string GetRightText()
	{
		int num = Mathf.Max(mSelectionStart, mSelectionEnd);
		if (!string.IsNullOrEmpty(mValue) && num < mValue.Length)
		{
			return mValue.Substring(num);
		}
		return "";
	}

	protected string GetSelection()
	{
		if (string.IsNullOrEmpty(mValue) || mSelectionStart == mSelectionEnd)
		{
			return "";
		}
		int num = Mathf.Min(mSelectionStart, mSelectionEnd);
		int num2 = Mathf.Max(mSelectionStart, mSelectionEnd);
		return mValue.Substring(num, num2 - num);
	}

	protected int GetCharUnderMouse()
	{
		Vector3[] worldCorners = label.worldCorners;
		Ray currentRay = UICamera.currentRay;
		if (!new Plane(worldCorners[0], worldCorners[1], worldCorners[2]).Raycast(currentRay, out float enter))
		{
			return 0;
		}
		return mDrawStart + label.GetCharacterIndexAtPosition(currentRay.GetPoint(enter), precise: false);
	}

	protected virtual void OnPress(bool isPressed)
	{
		if (isPressed && isSelected && label != null && (UICamera.currentScheme == UICamera.ControlScheme.Mouse || UICamera.currentScheme == UICamera.ControlScheme.Touch))
		{
			selectionEnd = GetCharUnderMouse();
			if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
			{
				selectionStart = mSelectionEnd;
			}
		}
	}

	protected virtual void OnDrag(Vector2 delta)
	{
		if (label != null && (UICamera.currentScheme == UICamera.ControlScheme.Mouse || UICamera.currentScheme == UICamera.ControlScheme.Touch))
		{
			selectionEnd = GetCharUnderMouse();
		}
	}

	private void OnDisable()
	{
		Cleanup();
	}

	protected virtual void Cleanup()
	{
		if ((bool)mHighlight)
		{
			mHighlight.enabled = false;
		}
		if ((bool)mCaret)
		{
			mCaret.enabled = false;
		}
		if ((bool)mBlankTex)
		{
			NGUITools.Destroy(mBlankTex);
			mBlankTex = null;
		}
	}

	public void Submit()
	{
		if (NGUITools.GetActive(this))
		{
			mValue = value;
			if (current == null)
			{
				current = this;
				EventDelegate.Execute(onSubmit);
				current = null;
			}
			SaveToPlayerPrefs(mValue);
		}
	}

	public void UpdateLabel()
	{
		if (!(label != null))
		{
			return;
		}
		if (mDoInit)
		{
			Init();
		}
		bool isSelected = this.isSelected;
		string value = this.value;
		bool flag = string.IsNullOrEmpty(value) && string.IsNullOrEmpty(Input.compositionString);
		label.color = ((flag && !isSelected) ? mDefaultColor : activeTextColor);
		string text;
		if (flag)
		{
			text = (isSelected ? "" : mDefaultText);
			label.alignment = mAlignment;
		}
		else
		{
			if (inputType == InputType.Password)
			{
				text = "";
				string str = "*";
				if (label.bitmapFont != null && label.bitmapFont.bmFont != null && label.bitmapFont.bmFont.GetGlyph(42) == null)
				{
					str = "x";
				}
				int i = 0;
				for (int length = value.Length; i < length; i++)
				{
					text += str;
				}
			}
			else
			{
				text = value;
			}
			int num = isSelected ? Mathf.Min(text.Length, cursorPosition) : 0;
			string str2 = text.Substring(0, num);
			if (isSelected)
			{
				str2 += Input.compositionString;
			}
			text = str2 + text.Substring(num, text.Length - num);
			if (isSelected && label.overflowMethod == UILabel.Overflow.ClampContent && label.maxLineCount == 1)
			{
				int num2 = label.CalculateOffsetToFit(text);
				if (num2 == 0)
				{
					mDrawStart = 0;
					label.alignment = mAlignment;
				}
				else if (num < mDrawStart)
				{
					mDrawStart = num;
					label.alignment = NGUIText.Alignment.Left;
				}
				else if (num2 < mDrawStart)
				{
					mDrawStart = num2;
					label.alignment = NGUIText.Alignment.Left;
				}
				else
				{
					num2 = label.CalculateOffsetToFit(text.Substring(0, num));
					if (num2 > mDrawStart)
					{
						mDrawStart = num2;
						label.alignment = NGUIText.Alignment.Right;
					}
				}
				if (mDrawStart != 0)
				{
					text = text.Substring(mDrawStart, text.Length - mDrawStart);
				}
			}
			else
			{
				mDrawStart = 0;
				label.alignment = mAlignment;
			}
		}
		label.text = text;
		if (isSelected && (mKeyboard == null || inputShouldBeHidden))
		{
			int num3 = mSelectionStart - mDrawStart;
			int num4 = mSelectionEnd - mDrawStart;
			if (mBlankTex == null)
			{
				mBlankTex = new Texture2D(2, 2, TextureFormat.ARGB32, mipChain: false);
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						mBlankTex.SetPixel(k, j, Color.white);
					}
				}
				mBlankTex.Apply();
			}
			if (num3 != num4)
			{
				if (mHighlight == null)
				{
					mHighlight = label.cachedGameObject.AddWidget<UITexture>();
					mHighlight.name = "Input Highlight";
					mHighlight.mainTexture = mBlankTex;
					mHighlight.fillGeometry = false;
					mHighlight.pivot = label.pivot;
					mHighlight.SetAnchor(label.cachedTransform);
				}
				else
				{
					mHighlight.pivot = label.pivot;
					mHighlight.mainTexture = mBlankTex;
					mHighlight.MarkAsChanged();
					mHighlight.enabled = true;
				}
			}
			if (mCaret == null)
			{
				mCaret = label.cachedGameObject.AddWidget<UITexture>();
				mCaret.name = "Input Caret";
				mCaret.mainTexture = mBlankTex;
				mCaret.fillGeometry = false;
				mCaret.pivot = label.pivot;
				mCaret.SetAnchor(label.cachedTransform);
			}
			else
			{
				mCaret.pivot = label.pivot;
				mCaret.mainTexture = mBlankTex;
				mCaret.MarkAsChanged();
				mCaret.enabled = true;
			}
			if (num3 != num4)
			{
				label.PrintOverlay(num3, num4, mCaret.geometry, mHighlight.geometry, caretColor, selectionColor);
				mHighlight.enabled = mHighlight.geometry.hasVertices;
			}
			else
			{
				label.PrintOverlay(num3, num4, mCaret.geometry, null, caretColor, selectionColor);
				if (mHighlight != null)
				{
					mHighlight.enabled = false;
				}
			}
			mNextBlink = RealTime.time + 0.5f;
			mLastAlpha = label.finalAlpha;
		}
		else
		{
			Cleanup();
		}
	}

	protected char Validate(string text, int pos, char ch)
	{
		if (validation == Validation.None || !base.enabled)
		{
			return ch;
		}
		if (validation == Validation.Integer)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && pos == 0 && !text.Contains("-"))
			{
				return ch;
			}
		}
		else if (validation == Validation.Float)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && pos == 0 && !text.Contains("-"))
			{
				return ch;
			}
			if (ch == '.' && !text.Contains("."))
			{
				return ch;
			}
		}
		else if (validation == Validation.Alphanumeric)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return ch;
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (validation == Validation.Username)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return (char)(ch - 65 + 97);
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else
		{
			if (validation == Validation.Filename)
			{
				switch (ch)
				{
				case ':':
					return '\0';
				case '/':
					return '\0';
				case '\\':
					return '\0';
				case '<':
					return '\0';
				case '>':
					return '\0';
				case '|':
					return '\0';
				case '^':
					return '\0';
				case '*':
					return '\0';
				case ';':
					return '\0';
				case '"':
					return '\0';
				case '`':
					return '\0';
				case '\t':
					return '\0';
				case '\n':
					return '\0';
				default:
					return ch;
				}
			}
			if (validation == Validation.Name)
			{
				char c = (text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
				char c2 = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';
				if (ch >= 'a' && ch <= 'z')
				{
					if (c == ' ')
					{
						return (char)(ch - 97 + 65);
					}
					return ch;
				}
				if (ch >= 'A' && ch <= 'Z')
				{
					if (c != ' ' && c != '\'')
					{
						return (char)(ch - 65 + 97);
					}
					return ch;
				}
				switch (ch)
				{
				case '\'':
					if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
					{
						return ch;
					}
					break;
				case ' ':
					if (c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
					{
						return ch;
					}
					break;
				}
			}
		}
		return '\0';
	}

	protected void ExecuteOnChange()
	{
		if (current == null && EventDelegate.IsValid(onChange))
		{
			current = this;
			EventDelegate.Execute(onChange);
			current = null;
		}
	}

	public void RemoveFocus()
	{
		isSelected = false;
	}

	public void SaveValue()
	{
		SaveToPlayerPrefs(mValue);
	}

	public void LoadValue()
	{
		if (!string.IsNullOrEmpty(savedAs))
		{
			string text = mValue.Replace("\\n", "\n");
			mValue = "";
			value = (PlayerPrefs.HasKey(savedAs) ? PlayerPrefs.GetString(savedAs) : text);
		}
	}
}
