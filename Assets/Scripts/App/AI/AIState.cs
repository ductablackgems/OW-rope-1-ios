using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public abstract class AIState
	{
		protected List<AIState> Followers = new List<AIState>(4);

		private DurationTimer timer = new DurationTimer();

		public Func<Type, AIState> GetState
		{
			get;
			set;
		}

		protected IAIEntity AIEntity
		{
			get;
			private set;
		}

		protected NavMeshAgent NavMeshAgent => AIEntity.NavMeshAgent;

		protected Vector3 Position => AIEntity.Owner.transform.position;

		protected float TimerInterval
		{
			get;
			set;
		}

		public AIState(IAIEntity entity)
		{
			AIEntity = entity;
		}

		public abstract AIState GetNextState();

		protected virtual void OnEnter()
		{
		}

		protected virtual void OnUpdate(float deltaTime)
		{
		}

		protected virtual void OnTimerTick()
		{
		}

		protected virtual void OnExit()
		{
		}

		protected virtual bool OnIsValid()
		{
			return true;
		}

		public void Enter()
		{
			OnEnter();
			if (TimerInterval > 0f)
			{
				timer.FakeDone(TimerInterval);
			}
		}

		public void Update(float deltaTime)
		{
			OnUpdate(deltaTime);
			if (CanTimerTick())
			{
				OnTimerTick();
			}
		}

		public void Exit()
		{
			timer.Stop();
			OnExit();
		}

		public bool IsValid()
		{
			return OnIsValid();
		}

		protected T GetFollower<T>() where T : AIState
		{
			return GetState(typeof(T)) as T;
		}

		private bool CanTimerTick()
		{
			if (TimerInterval <= 0f)
			{
				return true;
			}
			bool num = timer.Done();
			if (num)
			{
				timer.Run(TimerInterval);
			}
			return num;
		}
	}
}
