using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
	public delegate void OnVisibilityChange(bool isVisible);

	public OnVisibilityChange onChange;

	public Transform target;

	public Camera gameCamera;

	public Camera uiCamera;

	public bool disableIfInvisible = true;

	public bool destroyWithTarget = true;

	private Transform mTrans;

	private int mIsVisible = -1;

	public bool isVisible => mIsVisible == 1;

	private void Awake()
	{
		mTrans = base.transform;
	}

	private void Start()
	{
		if ((bool)target)
		{
			if (gameCamera == null)
			{
				gameCamera = NGUITools.FindCameraForLayer(target.gameObject.layer);
			}
			if (uiCamera == null)
			{
				uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
			}
			Update();
		}
		else if (destroyWithTarget)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if ((bool)target && uiCamera != null)
		{
			Vector3 vector = gameCamera.WorldToViewportPoint(target.position);
			int num = ((gameCamera.orthographic || vector.z > 0f) && vector.x > 0f && vector.x < 1f && vector.y > 0f && vector.y < 1f) ? 1 : 0;
			bool flag = num == 1;
			if (flag)
			{
				vector = uiCamera.ViewportToWorldPoint(vector);
				vector = mTrans.parent.InverseTransformPoint(vector);
				vector.z = 0f;
				mTrans.localPosition = vector;
			}
			if (mIsVisible == num)
			{
				return;
			}
			mIsVisible = num;
			if (disableIfInvisible)
			{
				int i = 0;
				for (int childCount = mTrans.childCount; i < childCount; i++)
				{
					NGUITools.SetActive(mTrans.GetChild(i).gameObject, flag);
				}
			}
			if (onChange != null)
			{
				onChange(flag);
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
