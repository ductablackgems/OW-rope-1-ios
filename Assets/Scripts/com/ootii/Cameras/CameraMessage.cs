using com.ootii.Collections;
using com.ootii.Messages;

namespace com.ootii.Cameras
{
	public class CameraMessage : Message
	{
		public static int MSG_CAMERA_MOTOR_UNKNOWN = 200;

		public static int MSG_CAMERA_MOTOR_ACTIVATE = 201;

		public static int MSG_CAMERA_MOTOR_DEACTIVATE = 202;

		public static int MSG_CAMERA_MOTOR_TEST = 203;

		public static int MSG_CAMERA_SPLINE_END = 204;

		public static string[] Names = new string[4]
		{
			"Unknown",
			"Motor Activate",
			"Motor Deactivate",
			"Motor Test"
		};

		public CameraMotor Motor;

		public bool Continue;

		private static ObjectPool<CameraMessage> sPool = new ObjectPool<CameraMessage>(10, 10);

		public override void Clear()
		{
			Motor = null;
			Continue = false;
			base.Clear();
		}

		public override void Release()
		{
			Clear();
			base.IsSent = true;
			base.IsHandled = true;
			if (this != null)
			{
				sPool.Release(this);
			}
		}

		public new static CameraMessage Allocate()
		{
			CameraMessage cameraMessage = sPool.Allocate();
			if (cameraMessage == null)
			{
				cameraMessage = new CameraMessage();
			}
			cameraMessage.IsSent = false;
			cameraMessage.IsHandled = false;
			return cameraMessage;
		}

		public static CameraMessage Allocate(CameraMessage rSource)
		{
			CameraMessage cameraMessage = sPool.Allocate();
			if (cameraMessage == null)
			{
				cameraMessage = new CameraMessage();
			}
			cameraMessage.ID = rSource.ID;
			cameraMessage.Motor = rSource.Motor;
			cameraMessage.Continue = rSource.Continue;
			cameraMessage.IsSent = false;
			cameraMessage.IsHandled = false;
			return cameraMessage;
		}

		public static void Release(CameraMessage rInstance)
		{
			if (rInstance != null)
			{
				rInstance.Clear();
				rInstance.IsSent = true;
				rInstance.IsHandled = true;
				sPool.Release(rInstance);
			}
		}

		public new static void Release(IMessage rInstance)
		{
			if (rInstance != null)
			{
				rInstance.Clear();
				rInstance.IsSent = true;
				rInstance.IsHandled = true;
				if (rInstance is CameraMessage)
				{
					sPool.Release((CameraMessage)rInstance);
				}
			}
		}
	}
}
