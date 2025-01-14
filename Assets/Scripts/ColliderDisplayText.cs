using UnityEngine;

[AddComponentMenu("NGUI/Examples/Collider - Display Text")]
public class ColliderDisplayText : MonoBehaviour
{
	public GameObject prefab;

	public Transform target;

	private HUDText mText;

	private bool mHover;

	private void Start()
	{
		if (HUDRoot.go == null)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		GameObject gameObject = HUDRoot.go.AddChild(prefab);
		mText = gameObject.GetComponentInChildren<HUDText>();
		gameObject.AddComponent<UIFollowTarget>().target = target;
	}

	private void OnHover(bool isOver)
	{
		if (mText != null && isOver && !mHover)
		{
			mHover = true;
			mText.Add("Left-click, right-click", Color.cyan, 2f);
		}
		else if (!isOver)
		{
			mHover = false;
		}
	}

	private void OnClick()
	{
		if (mText != null)
		{
			if (UICamera.currentTouchID == -1)
			{
				mText.Add(-10f + UnityEngine.Random.value * -10f, Color.red, 0f);
			}
			else if (UICamera.currentTouchID == -2)
			{
				mText.Add(10f + UnityEngine.Random.value * 10f, Color.green, 0f);
			}
		}
	}
}
