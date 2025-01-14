using App.Player;
using System;
using UnityEngine;

namespace App.Vehicles.Airplane.AI
{
	public class AirTraffic : MonoBehaviour
	{
		[SerializeField]
		private float airplaneDespawnDistance = 200f;

		protected bool isInitialized;

		protected AirplaneAIController airplane;

		protected PlayerModel player;

		private int layerMask;

		private BoundsValidator[] validators;

		private Action<AirTraffic> finished;

		public CurvySpline Spline
		{
			get;
			private set;
		}

		public bool IsRunning
		{
			get;
			protected set;
		}

		public bool IsReady => GetIsReady();

		public AirplaneAIController Airplane => airplane;

		public virtual bool IsFallback => false;

		public virtual bool IsLoop => false;

		protected virtual void OnInitialize()
		{
		}

		protected virtual void OnRun()
		{
		}

		protected virtual void OnStop()
		{
		}

		protected virtual void OnUpdate(float deltaTime)
		{
		}

		protected virtual void OnAirplanePathFinished()
		{
			IsRunning = false;
		}

		public void Initalize(Action<AirTraffic> finishedCallback)
		{
			if (!isInitialized)
			{
				layerMask = LayerMask.GetMask("Impact");
				finished = finishedCallback;
				Spline = GetComponentInChildren<CurvySpline>();
				player = ServiceLocator.GetPlayerModel();
				validators = GetComponentsInChildren<BoundsValidator>(includeInactive: true);
				OnInitialize();
				isInitialized = true;
			}
		}

		public void Run(AirplaneAIController prefab)
		{
			airplane = UnityEngine.Object.Instantiate(prefab, Spline.Segments[0].Position, Quaternion.identity);
			airplane.Initialize();
			airplane.gameObject.SetActive(value: true);
			IsRunning = true;
			airplane.Run(this, OnAirplanePathFinished);
			OnRun();
		}

		public void Stop(bool force = false)
		{
			OnStop();
			IsRunning = false;
			if (!(airplane == null) && force)
			{
				airplane.Clean();
				UnityEngine.Object.Destroy(airplane.gameObject);
				airplane = null;
			}
		}

		public Vector3 GetStartPosition()
		{
			return Spline.Segments[0].Position;
		}

		private void Update()
		{
			if (!IsRunning && airplane != null)
			{
				TryDespawnAirplane();
			}
			OnUpdate(Time.deltaTime);
		}

		private void OnAirplanePathFinished(AirplaneAIController airplane)
		{
			OnAirplanePathFinished();
		}

		protected virtual void TryDespawnAirplane()
		{
			if (!((player.Transform.position - airplane.transform.position).magnitude < airplaneDespawnDistance))
			{
				airplane.Clean();
				airplane.gameObject.SetActive(value: false);
				UnityEngine.Object.Destroy(airplane.gameObject);
				airplane = null;
				finished.SafeInvoke(this);
			}
		}

		private bool GetIsReady()
		{
			if (IsRunning)
			{
				return false;
			}
			if (airplane != null)
			{
				return false;
			}
			return Validate();
		}

		private bool Validate()
		{
			bool result = true;
			BoundsValidator[] array = validators;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].Validate(layerMask))
				{
					result = false;
					break;
				}
			}
			return result;
		}
	}
}
