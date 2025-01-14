using App.Player;
using UnityEngine;

namespace App.Camera
{
	public class ClimbCameraFix : MonoBehaviour
	{
		public float maxAngle = 85f;

		public float angleFixSpeed = 100f;

		public float angleFixDelay = 0.5f;

		private PlayerModel player;

		private DurationTimer badAngleTimer = new DurationTimer(useFixedTime: true);

		private bool touched;

		private bool blocked;

		private bool firstFrame = true;

		private void Start()
		{
			player = ServiceLocator.GetPlayerModel();
			if (player.ClimbController == null)
			{
				base.enabled = false;
				return;
			}
			ETCTouchPad controlTouchPad = ETCInput.GetControlTouchPad("CameraPad");
			if (controlTouchPad != null)
			{
				controlTouchPad.onTouchStart.AddListener(delegate
				{
					touched = true;
					blocked = true;
				});
				controlTouchPad.onTouchUp.AddListener(delegate
				{
					touched = false;
				});
			}
		}

		private void Update()
		{
			if (!firstFrame && player.ClimbController.Running())
			{
				if (!touched && blocked && (ETCInput.GetAxis("HorizontalJoystick") != 0f || ETCInput.GetAxis("VerticalJoystick") != 0f))
				{
					blocked = false;
				}
				if (!blocked)
				{
					Quaternion quaternion = Quaternion.Euler(0f, player.Transform.eulerAngles.y, 0f);
					Quaternion quaternion2 = Quaternion.Euler(0f, base.transform.parent.eulerAngles.y, 0f);
					if (Quaternion.Angle(quaternion, quaternion2) > maxAngle)
					{
						if (!badAngleTimer.Running())
						{
							badAngleTimer.Run(angleFixDelay);
						}
						else if (badAngleTimer.Done())
						{
							quaternion2 = Quaternion.RotateTowards(quaternion2, quaternion, Time.deltaTime * angleFixSpeed);
							base.transform.parent.rotation = Quaternion.Euler(base.transform.parent.eulerAngles.x, quaternion2.eulerAngles.y, base.transform.parent.eulerAngles.z);
						}
					}
					else
					{
						badAngleTimer.Stop();
					}
				}
				else
				{
					badAngleTimer.Stop();
				}
			}
			firstFrame = false;
		}
	}
}
