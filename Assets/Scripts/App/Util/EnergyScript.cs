using UnityEngine;

namespace App.Util
{
	public class EnergyScript : MonoBehaviour
	{
		private float maxEnergy = 1f;

		private float recoverySpeed = 0.08f;

		private float fastRunCost = 0.13f;

		private float laserCost = 0.35f;

		private float flyCost = 0.13f;

		private float currentEnergy;

		private bool initialized;

		public void SetMaxEnergySmoothly(float maxEnergy)
		{
			currentEnergy *= maxEnergy / this.maxEnergy;
			this.maxEnergy = maxEnergy;
		}

		public float GetCurrentEnergy()
		{
			if (!initialized)
			{
				Init();
			}
			return currentEnergy / maxEnergy;
		}

		public bool Full()
		{
			return currentEnergy == maxEnergy;
		}

		public void ConsumeEnergy(float amount)
		{
			currentEnergy -= amount;
			if (currentEnergy < 0f)
			{
				currentEnergy = 0f;
			}
		}

		public void ConsumeFastRunEnergy(float deltaTime)
		{
			currentEnergy -= fastRunCost * deltaTime;
			if (currentEnergy < 0f)
			{
				currentEnergy = 0f;
			}
		}

		public void ConsumeLaserEnergy(float deltaTime)
		{
			currentEnergy -= laserCost * deltaTime;
			if (currentEnergy < 0f)
			{
				currentEnergy = 0f;
			}
		}

		public void ConsumeFlyEnergy(float deltaTime)
		{
			currentEnergy -= flyCost * deltaTime;
			if (currentEnergy < 0f)
			{
				currentEnergy = 0f;
			}
		}

		public void Recover(float amount)
		{
			currentEnergy += amount;
			if (currentEnergy > maxEnergy)
			{
				currentEnergy = maxEnergy;
			}
		}

		private void Awake()
		{
			if (!initialized)
			{
				Init();
			}
		}

		private void Update()
		{
			currentEnergy += recoverySpeed * Time.deltaTime;
			if (currentEnergy > maxEnergy)
			{
				currentEnergy = maxEnergy;
			}
		}

		private void Init()
		{
			initialized = true;
			currentEnergy = maxEnergy;
		}
	}
}
