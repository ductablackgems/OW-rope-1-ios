using App.AI;
using App.Vehicles.Car.Navigation.Modes.Curve;
using System;
using UnityEngine;

namespace App.Vehicles.Car.Navigation
{
	public class VehicleStuckManager : AbstractAIScript
	{
		private AICarBoxSensor boxSensor;

		private float fallAsleepTime = -1f;

		private DurationTimer checkBoxSenserTimer = new DurationTimer();

		private DurationTimer forwardStuckTimer = new DurationTimer();

		private DurationTimer backStuckTimer = new DurationTimer();

		private BoxAnalysisResult boxAnalysisResult;

		private bool hasBoxAnalysisResult;

		public event Action OnImobileObstacle;

		public bool HitVehicle(Transform vehicle)
		{
			if (hasBoxAnalysisResult)
			{
				return boxAnalysisResult.HitVehicle(vehicle);
			}
			return false;
		}

		public void MarkSleeping()
		{
			if (fallAsleepTime < 0f)
			{
				fallAsleepTime = Time.time;
				checkBoxSenserTimer.Run(5f);
			}
			else if (checkBoxSenserTimer.Done())
			{
				checkBoxSenserTimer.Stop();
				if (boxSensor.Hit())
				{
					boxSensor.AnalyzeContent();
				}
				else
				{
					checkBoxSenserTimer.Run(5f);
				}
			}
		}

		public void MarkWakeUp()
		{
			fallAsleepTime = -1f;
			hasBoxAnalysisResult = false;
		}

		public bool Imobile()
		{
			if (forwardStuckTimer.Done())
			{
				return backStuckTimer.Done();
			}
			return false;
		}

		public void ClearImobile()
		{
			forwardStuckTimer.Stop();
			backStuckTimer.Stop();
		}

		public void UpdateStates(float vertical, float absoluteSpeed)
		{
			if (absoluteSpeed < 0.2f)
			{
				if (vertical > 0f)
				{
					if (backStuckTimer.InProgress())
					{
						backStuckTimer.Stop();
					}
					if (!forwardStuckTimer.Running())
					{
						forwardStuckTimer.Run(1.5f);
					}
				}
				else if (vertical < 0f)
				{
					if (forwardStuckTimer.InProgress())
					{
						forwardStuckTimer.Stop();
					}
					if (!backStuckTimer.Running())
					{
						backStuckTimer.Run(1.5f);
					}
				}
				else
				{
					if (forwardStuckTimer.InProgress())
					{
						forwardStuckTimer.Stop();
					}
					if (backStuckTimer.InProgress())
					{
						backStuckTimer.Stop();
					}
				}
			}
			else
			{
				forwardStuckTimer.Stop();
				backStuckTimer.Stop();
			}
		}

		private void Awake()
		{
			boxSensor = base.ComponentsRoot.GetComponentInChildrenSafe<AICarBoxSensor>();
			boxSensor.OnAnalysisDone += OnBoxAnalysisDone;
		}

		private void OnDestroy()
		{
			boxSensor.OnAnalysisDone -= OnBoxAnalysisDone;
		}

		private void OnBoxAnalysisDone(BoxAnalysisResult result)
		{
			boxAnalysisResult = result;
			hasBoxAnalysisResult = true;
			if (result.HitImobileObstacle(base.ComponentsRoot) && this.OnImobileObstacle != null)
			{
				this.OnImobileObstacle();
			}
		}
	}
}
