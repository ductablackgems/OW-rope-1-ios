using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.AI
{
	public abstract class AIStateMachine
	{
		protected List<AIState> States = new List<AIState>(8);

		public bool IsRunning
		{
			get;
			set;
		}

		protected AIState Current
		{
			get;
			private set;
		}

		protected IAIEntity AIEntity
		{
			get;
			private set;
		}

		protected abstract AIState DefaultState
		{
			get;
		}

		protected abstract void GenerateStates(List<AIState> states);

		protected virtual void OnInitialize()
		{
		}

		public AIStateMachine(IAIEntity aiEntity)
		{
			AIEntity = aiEntity;
		}

		public void Update()
		{
			if (IsRunning)
			{
				TrySetNewState();
				Current.Update(Time.deltaTime);
			}
		}

		public void Initialize()
		{
			GenerateStates(States);
			foreach (AIState state in States)
			{
				state.GetState = GetState;
			}
			OnInitialize();
		}

		private AIState GetNextState()
		{
			if (Current == null)
			{
				return DefaultState;
			}
			AIState nextState = Current.GetNextState();
			if (nextState == null)
			{
				return DefaultState;
			}
			return nextState;
		}

		private void TrySetNewState()
		{
			AIState current = Current;
			AIState nextState = GetNextState();
			if (current != nextState)
			{
				current?.Exit();
				nextState?.Enter();
				Current = nextState;
			}
		}

		private AIState GetState(Type type)
		{
			for (int i = 0; i < States.Count; i++)
			{
				AIState aIState = States[i];
				if (aIState.GetType() == type)
				{
					return aIState;
				}
			}
			return null;
		}
	}
}
