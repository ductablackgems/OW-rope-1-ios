using UnityEngine;

[ExecuteInEditMode]
public class LocalizedTextNGUI : MonoBehaviour
{
	[LocalizeID]
	[SerializeField]
	private int m_TextID;

	public string m_DisplayText;

	private UILabel labelText;

	private void OnEnable()
	{
		labelText = GetComponent<UILabel>();
		GetDisplayText(m_TextID, -1);
	}

	private void Start()
	{
		labelText = GetComponent<UILabel>();
		GetDisplayText(m_TextID, -1);
	}

	public void GetDisplayText(int TextID, int missionParamater)
	{
		m_TextID = TextID;
		if (m_DisplayText != null)
		{
			m_DisplayText = ((m_TextID != 0) ? LocalizationManager.Instance.GetText(m_TextID) : (labelText.text ?? string.Empty));
		}
		if (missionParamater != -1)
		{
			m_DisplayText = string.Format(m_DisplayText, missionParamater);
		}
		if (m_DisplayText != null)
		{
			m_DisplayText = m_DisplayText.Replace("\\n", "\n\r");
		}
		SetText(m_DisplayText);
	}

	public void SetText(string text)
	{
		if (m_TextID != 0 || !(labelText.text == text))
		{
			labelText.text = (text ?? string.Empty);
		}
	}
}
