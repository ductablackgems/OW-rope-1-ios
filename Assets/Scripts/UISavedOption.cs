using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Saved Option")]
public class UISavedOption : MonoBehaviour
{
	public string keyName;

	private UIPopupList mList;

	private UIToggle mCheck;

	private UIProgressBar mSlider;

	private string key
	{
		get
		{
			if (!string.IsNullOrEmpty(keyName))
			{
				return keyName;
			}
			return "NGUI State: " + base.name;
		}
	}

	private void Awake()
	{
		mList = GetComponent<UIPopupList>();
		mCheck = GetComponent<UIToggle>();
		mSlider = GetComponent<UIProgressBar>();
	}

	private void OnEnable()
	{
		if (mList != null)
		{
			EventDelegate.Add(mList.onChange, SaveSelection);
			string @string = PlayerPrefs.GetString(key);
			if (!string.IsNullOrEmpty(@string))
			{
				mList.value = @string;
			}
			return;
		}
		if (mCheck != null)
		{
			EventDelegate.Add(mCheck.onChange, SaveState);
			mCheck.value = (PlayerPrefs.GetInt(key, mCheck.startsActive ? 1 : 0) != 0);
			return;
		}
		if (mSlider != null)
		{
			EventDelegate.Add(mSlider.onChange, SaveProgress);
			mSlider.value = PlayerPrefs.GetFloat(key, mSlider.value);
			return;
		}
		string string2 = PlayerPrefs.GetString(key);
		UIToggle[] componentsInChildren = GetComponentsInChildren<UIToggle>(includeInactive: true);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			UIToggle obj = componentsInChildren[i];
			obj.value = (obj.name == string2);
		}
	}

	private void OnDisable()
	{
		if (mCheck != null)
		{
			EventDelegate.Remove(mCheck.onChange, SaveState);
			return;
		}
		if (mList != null)
		{
			EventDelegate.Remove(mList.onChange, SaveSelection);
			return;
		}
		if (mSlider != null)
		{
			EventDelegate.Remove(mSlider.onChange, SaveProgress);
			return;
		}
		UIToggle[] componentsInChildren = GetComponentsInChildren<UIToggle>(includeInactive: true);
		int num = 0;
		int num2 = componentsInChildren.Length;
		UIToggle uIToggle;
		while (true)
		{
			if (num < num2)
			{
				uIToggle = componentsInChildren[num];
				if (uIToggle.value)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		PlayerPrefs.SetString(key, uIToggle.name);
	}

	public void SaveSelection()
	{
		PlayerPrefs.SetString(key, UIPopupList.current.value);
	}

	public void SaveState()
	{
		PlayerPrefs.SetInt(key, UIToggle.current.value ? 1 : 0);
	}

	public void SaveProgress()
	{
		PlayerPrefs.SetFloat(key, UIProgressBar.current.value);
	}
}
