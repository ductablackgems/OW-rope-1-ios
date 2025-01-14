using System;
using UnityEngine;

namespace App.Vehicles.Bicycle
{
	public class BicycleAnimator : MonoBehaviour
	{
		public float wheelDiameter = 0.5f;

		public float pedalsRatio = 0.5f;

		public RotateObject frontWheel;

		public RotateObject backWheel;

		public RotateObject pedals;

		public Transform handles;

		private bool initialized;

		public void SetSpeed(float speed, float steerAngle)
		{
			initialized = true;
			float num = wheelDiameter * (float)Math.PI;
			float num2 = speed * 180f / num;
			frontWheel.rate = num2;
			backWheel.rate = num2;
			pedals.rate = num2 * pedalsRatio;
			handles.localRotation = Quaternion.Euler(0f, steerAngle, 0f);
		}

		private void Awake()
		{
			if (!initialized)
			{
				SetSpeed(0f, 0f);
			}
		}
	}
}
