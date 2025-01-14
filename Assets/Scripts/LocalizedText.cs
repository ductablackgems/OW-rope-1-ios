using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LocalizedText : MonoBehaviour
{
	[LocalizeID]
	[SerializeField]
	private int m_TextID;

	private Text textUpdate;

	public string m_DisplayText;

	public string m_DisplayTextValue;

	private void OnEnable()
	{
		textUpdate = GetComponent<Text>();
		GetDisplayTextUpdate();
	}

	private void SetTextDirty()
	{
		m_DisplayText = null;
	}

	public void GetDisplayTextUpdate()
	{
		m_DisplayText = ((m_TextID != 0) ? LocalizationManager.Instance.GetText(m_TextID) : (textUpdate.text ?? string.Empty));
		if (m_DisplayTextValue != string.Empty)
		{
			m_DisplayText = string.Format(m_DisplayText, m_DisplayTextValue);
		}
		if (m_DisplayText != null)
		{
			m_DisplayText = m_DisplayText.Replace("\\n", "\n\r");
		}
		if (m_TextID != 0 || !(textUpdate.text == m_DisplayText))
		{
			textUpdate.text = (m_DisplayText ?? string.Empty);
		}
	}
}
