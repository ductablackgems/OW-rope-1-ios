using UnityEngine;

public class WingsController : MonoBehaviour
{
	public WingsAnimationSettings upWingsAnimationSettings;

	public WingsAnimationSettings idleWingsAnimationSettings;

	public WingsAnimationSettings leftWingsAnimationSettings;

	public WingsAnimationSettings rightWingsAnimationSettings;

	public WingsAnimationSettings frontWingsAnimationSettings;

	public WingsAnimationSettings backWingsAnimationSettings;

	public void DeactivateOther(WingsAnimationSettings actualWingsAnimatorSetting)
	{
		if (actualWingsAnimatorSetting != upWingsAnimationSettings)
		{
			upWingsAnimationSettings.End();
		}
		if (actualWingsAnimatorSetting != idleWingsAnimationSettings)
		{
			idleWingsAnimationSettings.End();
		}
		if (actualWingsAnimatorSetting != leftWingsAnimationSettings)
		{
			leftWingsAnimationSettings.End();
		}
		if (actualWingsAnimatorSetting != rightWingsAnimationSettings)
		{
			rightWingsAnimationSettings.End();
		}
		if (actualWingsAnimatorSetting != frontWingsAnimationSettings)
		{
			frontWingsAnimationSettings.End();
		}
		if (actualWingsAnimatorSetting != backWingsAnimationSettings)
		{
			backWingsAnimationSettings.End();
		}
	}

	public void DeactivateAll()
	{
		upWingsAnimationSettings.End();
		idleWingsAnimationSettings.End();
		leftWingsAnimationSettings.End();
		rightWingsAnimationSettings.End();
		frontWingsAnimationSettings.End();
		backWingsAnimationSettings.End();
	}
}
