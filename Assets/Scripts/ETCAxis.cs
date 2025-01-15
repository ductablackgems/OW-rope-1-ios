using System;
using UnityEngine;

[Serializable]
public class ETCAxis
{
	public enum DirectAction
	{
		Rotate,
		RotateLocal,
		Translate,
		TranslateLocal,
		Scale,
		Force,
		RelativeForce,
		Torque,
		RelativeTorque,
		Jump
	}

	public enum AxisInfluenced
	{
		X,
		Y,
		Z
	}

	public enum AxisValueMethod
	{
		Classical,
		Curve
	}

	public enum AxisState
	{
		None,
		Down,
		Press,
		Up,
		DownUp,
		DownDown,
		DownLeft,
		DownRight,
		PressUp,
		PressDown,
		PressLeft,
		PressRight
	}

	public enum ActionOn
	{
		Down,
		Press
	}

	public string name;

	public bool autoLinkTagPlayer;

	public string autoTag = "Player";

	public GameObject player;

	public bool enable;

	public bool invertedAxis;

	public float speed;

	public float deadValue;

	public AxisValueMethod valueMethod;

	public AnimationCurve curveValue;

	public bool isEnertia;

	public float inertia;

	public float inertiaThreshold;

	public bool isAutoStab;

	public float autoStabThreshold;

	public float autoStabSpeed;

	private float startAngle;

	public bool isClampRotation;

	public float maxAngle;

	public float minAngle;

	public bool isValueOverTime;

	public float overTimeStep;

	public float maxOverTimeValue;

	public float axisValue;

	public float axisSpeedValue;

	public float axisThreshold;

	public bool isLockinJump;

	private Vector3 lastMove;

	public AxisState axisState;

	[SerializeField]
	private Transform _directTransform;

	public DirectAction directAction;

	public AxisInfluenced axisInfluenced;

	public ActionOn actionOn;

	public CharacterController directCharacterController;

	public Rigidbody directRigidBody;

	public float gravity;

	public float currentGravity;

	public bool isJump;

	public string unityAxis;

	public bool showGeneralInspector;

	public bool showDirectInspector;

	public bool showInertiaInspector;

	public bool showSimulatinInspector;

	public Transform directTransform
	{
		get
		{
			return _directTransform;
		}
		set
		{
			_directTransform = value;
			if (_directTransform != null)
			{
				directCharacterController = _directTransform.GetComponent<CharacterController>();
				directRigidBody = _directTransform.GetComponent<Rigidbody>();
			}
			else
			{
				directCharacterController = null;
			}
		}
	}

	public ETCAxis(string axisName)
	{
		name = axisName;
		enable = true;
		speed = 15f;
		invertedAxis = false;
		isEnertia = false;
		inertia = 0f;
		inertiaThreshold = 0.08f;
		axisValue = 0f;
		axisSpeedValue = 0f;
		gravity = 0f;
		isAutoStab = false;
		autoStabThreshold = 0.01f;
		autoStabSpeed = 10f;
		maxAngle = 90f;
		minAngle = 90f;
		axisState = AxisState.None;
		maxOverTimeValue = 1f;
		overTimeStep = 1f;
		isValueOverTime = false;
		axisThreshold = 0.5f;
		deadValue = 0.1f;
		actionOn = ActionOn.Press;
	}

	public void InitAxis()
	{
		if (autoLinkTagPlayer)
		{
			player = GameObject.FindGameObjectWithTag(autoTag);
			if ((bool)player)
			{
				directTransform = player.transform;
			}
		}
		startAngle = GetAngle();
	}

	public void UpdateAxis(float realValue, bool isOnDrag, ETCBase.ControlType type, bool deltaTime = true)
	{
		if ((autoLinkTagPlayer && player == null) || ((bool)player && !player.activeSelf))
		{
			player = GameObject.FindGameObjectWithTag(autoTag);
			if ((bool)player)
			{
				directTransform = player.transform;
			}
		}
		if (isAutoStab && axisValue == 0f && (bool)_directTransform)
		{
			DoAutoStabilisation();
		}
		if (invertedAxis)
		{
			realValue *= -1f;
		}
		if (isValueOverTime && realValue != 0f)
		{
			axisValue += overTimeStep * Mathf.Sign(realValue) * Time.deltaTime;
			if (Mathf.Sign(axisValue) > 0f)
			{
				axisValue = Mathf.Clamp(axisValue, 0f, maxOverTimeValue);
			}
			else
			{
				axisValue = Mathf.Clamp(axisValue, 0f - maxOverTimeValue, 0f);
			}
		}
		ComputAxisValue(realValue, type, isOnDrag, deltaTime);
	}

	public void UpdateButton()
	{
		if ((autoLinkTagPlayer && player == null) || ((bool)player && !player.activeSelf))
		{
			player = GameObject.FindGameObjectWithTag(autoTag);
			if ((bool)player)
			{
				directTransform = player.transform;
			}
		}
		if (isValueOverTime)
		{
			axisValue += overTimeStep * Time.deltaTime;
			axisValue = Mathf.Clamp(axisValue, 0f, maxOverTimeValue);
		}
		else if (axisState == AxisState.Press || axisState == AxisState.Down)
		{
			axisValue = 1f;
		}
		else
		{
			axisValue = 0f;
		}
		switch (actionOn)
		{
		case ActionOn.Down:
			axisSpeedValue = axisValue * speed;
			if (axisState == AxisState.Down)
			{
				DoDirectAction();
			}
			break;
		case ActionOn.Press:
			axisSpeedValue = axisValue * speed * Time.deltaTime;
			if (axisState == AxisState.Press)
			{
				DoDirectAction();
			}
			break;
		}
	}

	public void ResetAxis()
	{
		if (!isEnertia || (isEnertia && Mathf.Abs(axisValue) < inertiaThreshold))
		{
			axisValue = 0f;
			axisSpeedValue = 0f;
		}
	}

	public void DoDirectAction()
	{
		if (!directTransform)
		{
			return;
		}
		Vector3 influencedAxis = GetInfluencedAxis();
		switch (directAction)
		{
		case DirectAction.Rotate:
			directTransform.Rotate(influencedAxis * axisSpeedValue, Space.World);
			break;
		case DirectAction.RotateLocal:
			directTransform.Rotate(influencedAxis * axisSpeedValue, Space.Self);
			break;
		case DirectAction.Translate:
			if (directCharacterController == null)
			{
				directTransform.Translate(influencedAxis * axisSpeedValue, Space.World);
			}
			else if (directCharacterController.isGrounded || !isLockinJump)
			{
				Vector3 motion2 = influencedAxis * axisSpeedValue;
				directCharacterController.Move(motion2);
				lastMove = influencedAxis * (axisSpeedValue / Time.deltaTime);
			}
			else
			{
				directCharacterController.Move(lastMove * Time.deltaTime);
			}
			break;
		case DirectAction.TranslateLocal:
			if (directCharacterController == null)
			{
				directTransform.Translate(influencedAxis * axisSpeedValue, Space.Self);
			}
			else if (directCharacterController.isGrounded || !isLockinJump)
			{
				Vector3 motion = directCharacterController.transform.TransformDirection(influencedAxis) * axisSpeedValue;
				directCharacterController.Move(motion);
				lastMove = directCharacterController.transform.TransformDirection(influencedAxis) * (axisSpeedValue / Time.deltaTime);
			}
			else
			{
				directCharacterController.Move(lastMove * Time.deltaTime);
			}
			break;
		case DirectAction.Scale:
			directTransform.localScale += influencedAxis * axisSpeedValue;
			break;
		case DirectAction.Force:
			if (directRigidBody != null)
			{
				directRigidBody.AddForce(influencedAxis * axisValue * speed);
			}
			else
			{
				UnityEngine.Debug.LogWarning("ETCAxis : " + name + " No rigidbody on gameobject : " + _directTransform.name);
			}
			break;
		case DirectAction.RelativeForce:
			if (directRigidBody != null)
			{
				directRigidBody.AddRelativeForce(influencedAxis * axisValue * speed);
			}
			else
			{
				UnityEngine.Debug.LogWarning("ETCAxis : " + name + " No rigidbody on gameobject : " + _directTransform.name);
			}
			break;
		case DirectAction.Torque:
			if (directRigidBody != null)
			{
				directRigidBody.AddTorque(influencedAxis * axisValue * speed);
			}
			else
			{
				UnityEngine.Debug.LogWarning("ETCAxis : " + name + " No rigidbody on gameobject : " + _directTransform.name);
			}
			break;
		case DirectAction.RelativeTorque:
			if (directRigidBody != null)
			{
				directRigidBody.AddRelativeTorque(influencedAxis * axisValue * speed);
			}
			else
			{
				UnityEngine.Debug.LogWarning("ETCAxis : " + name + " No rigidbody on gameobject : " + _directTransform.name);
			}
			break;
		case DirectAction.Jump:
			if (directCharacterController != null && !isJump)
			{
				isJump = true;
				currentGravity = speed;
			}
			break;
		}
		if (isClampRotation && directAction == DirectAction.RotateLocal)
		{
			DoAngleLimitation();
		}
	}

	public void DoGravity()
	{
		if (directCharacterController != null && gravity != 0f)
		{
			if (!isJump)
			{
				Vector3 a = new Vector3(0f, 0f - gravity, 0f);
				directCharacterController.Move(a * Time.deltaTime);
			}
			else
			{
				currentGravity -= gravity * Time.deltaTime;
				Vector3 a2 = new Vector3(0f, currentGravity, 0f);
				directCharacterController.Move(a2 * Time.deltaTime);
			}
			if (directCharacterController.isGrounded)
			{
				isJump = false;
				currentGravity = 0f;
			}
		}
	}

	private void ComputAxisValue(float realValue, ETCBase.ControlType type, bool isOnDrag, bool deltaTime)
	{
		if (enable)
		{
			if (type == ETCBase.ControlType.Joystick)
			{
				if (valueMethod == AxisValueMethod.Classical)
				{
					float num = Mathf.Max(Mathf.Abs(realValue), 0.001f);
					float num2 = Mathf.Max(num - deadValue, 0f) / (1f - deadValue) / num;
					realValue *= num2;
				}
				else
				{
					realValue = curveValue.Evaluate(Mathf.Abs(realValue)) * Mathf.Sign(realValue);
				}
			}
			if (isEnertia)
			{
				realValue -= axisValue;
				realValue /= inertia;
				axisValue += realValue;
				if (Mathf.Abs(axisValue) < inertiaThreshold && !isOnDrag)
				{
					axisValue = 0f;
				}
			}
			else if (!isValueOverTime || (isValueOverTime && realValue == 0f))
			{
				axisValue = realValue;
			}
			if (deltaTime)
			{
				axisSpeedValue = axisValue * speed * Time.deltaTime;
			}
			else
			{
				axisSpeedValue = axisValue * speed;
			}
		}
		else
		{
			axisValue = 0f;
			axisSpeedValue = 0f;
		}
	}

	private Vector3 GetInfluencedAxis()
	{
		Vector3 result = Vector3.zero;
		switch (axisInfluenced)
		{
		case AxisInfluenced.X:
			result = Vector3.right;
			break;
		case AxisInfluenced.Y:
			result = Vector3.up;
			break;
		case AxisInfluenced.Z:
			result = Vector3.forward;
			break;
		}
		return result;
	}

	private float GetAngle()
	{
		float num = 0f;
		if (_directTransform != null)
		{
			switch (axisInfluenced)
			{
			case AxisInfluenced.X:
				num = _directTransform.localRotation.eulerAngles.x;
				break;
			case AxisInfluenced.Y:
				num = _directTransform.localRotation.eulerAngles.y;
				break;
			case AxisInfluenced.Z:
				num = _directTransform.localRotation.eulerAngles.z;
				break;
			}
			if (num <= 360f && num >= 180f)
			{
				num -= 360f;
			}
		}
		return num;
	}

	private void DoAutoStabilisation()
	{
		float num = GetAngle();
		if (num <= 360f && num >= 180f)
		{
			num -= 360f;
		}
		if (num > startAngle - autoStabThreshold || num < startAngle + autoStabThreshold)
		{
			float num2 = 0f;
			Vector3 euler = Vector3.zero;
			if (num > startAngle - autoStabThreshold)
			{
				num2 = num + autoStabSpeed / 100f * Mathf.Abs(num - startAngle) * Time.deltaTime * -1f;
			}
			if (num < startAngle + autoStabThreshold)
			{
				num2 = num + autoStabSpeed / 100f * Mathf.Abs(num - startAngle) * Time.deltaTime;
			}
			switch (axisInfluenced)
			{
			case AxisInfluenced.X:
				euler = new Vector3(num2, _directTransform.localRotation.eulerAngles.y, _directTransform.localRotation.eulerAngles.z);
				break;
			case AxisInfluenced.Y:
				euler = new Vector3(_directTransform.localRotation.eulerAngles.x, num2, _directTransform.localRotation.eulerAngles.z);
				break;
			case AxisInfluenced.Z:
				euler = new Vector3(_directTransform.localRotation.eulerAngles.x, _directTransform.localRotation.eulerAngles.y, num2);
				break;
			}
			_directTransform.localRotation = Quaternion.Euler(euler);
		}
	}

	private void DoAngleLimitation()
	{
		Quaternion localRotation = _directTransform.localRotation;
		localRotation.x /= localRotation.w;
		localRotation.y /= localRotation.w;
		localRotation.z /= localRotation.w;
		localRotation.w = 1f;
		float num = 0f;
		switch (axisInfluenced)
		{
		case AxisInfluenced.X:
			num = 114.59156f * Mathf.Atan(localRotation.x);
			num = Mathf.Clamp(num, 0f - minAngle, maxAngle);
			localRotation.x = Mathf.Tan((float)Math.PI / 360f * num);
			break;
		case AxisInfluenced.Y:
			num = 114.59156f * Mathf.Atan(localRotation.y);
			num = Mathf.Clamp(num, 0f - minAngle, maxAngle);
			localRotation.y = Mathf.Tan((float)Math.PI / 360f * num);
			break;
		case AxisInfluenced.Z:
			num = 114.59156f * Mathf.Atan(localRotation.z);
			num = Mathf.Clamp(num, 0f - minAngle, maxAngle);
			localRotation.z = Mathf.Tan((float)Math.PI / 360f * num);
			break;
		}
		_directTransform.localRotation = localRotation;
	}

	public void InitDeadCurve()
	{
		curveValue = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		curveValue.postWrapMode = WrapMode.PingPong;
		curveValue.preWrapMode = WrapMode.PingPong;
	}
}
