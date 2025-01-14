using App.Effects;
using App.GUI;
using App.Player;
using System;
using UnityEngine;

namespace App.Quests
{
	public abstract class GameplayObjective : MonoBehaviour, INavigationItem
	{
		public enum State
		{
			Inactive,
			Active,
			Finished,
			Failed
		}

		[Serializable]
		private class DataManagement
		{
			public bool CleanVehicle;

			public bool CleanNPC;
		}

		[SerializeField]
		private bool isVisbleOnMap = true;

		[Space]
		[SerializeField]
		private float timeLimit;

		[Header("Texts")]
		[LocalizeID]
		[SerializeField]
		private int shortDescriptionID;

		[LocalizeID]
		[SerializeField]
		private int longDescriptionID;

		[LocalizeID]
		[SerializeField]
		private int dialogHeaderTextID;

		[Header("Effects")]
		[SerializeField]
		private bool disableDefaultEffect;

		[Space]
		[SerializeField]
		private GameplayEffect customEffect;

		[SerializeField]
		private Vector3 effectOffset;

		[Header("Data Management")]
		[SerializeField]
		private DataManagement dataManagement;

		protected string shortDescriptionOrg;

		protected string longDescriptionOrg;

		protected ObjectiveData data;

		private State currentState;

		private Action stateChanged;

		private DurationTimer timer = new DurationTimer();

		private string shortDescription;

		private string longDescription;

		private GameplayEffect effect;

		public State CurrentState
		{
			get
			{
				return currentState;
			}
			private set
			{
				SetState(value);
			}
		}

		public string ShortDecription
		{
			get
			{
				return shortDescription;
			}
			protected set
			{
				SetShortDescription(value);
			}
		}

		public string LongDescription
		{
			get
			{
				return longDescription;
			}
			protected set
			{
				SetLongDescription(value);
			}
		}

		public int HeaderTextID => dialogHeaderTextID;

		public bool IsActive => currentState == State.Active;

		public Vector3 Position => base.transform.position;

		public float RemainingTime => GetRemainingTime();

		public PlayerModel Player
		{
			get;
			private set;
		}

		public EffectsManager EffectManager
		{
			get;
			private set;
		}

		public Action TextChanged
		{
			get;
			set;
		}

		Vector3 INavigationItem.Position => GetNavigationPosition();

		bool INavigationItem.ShowOnMap => isVisbleOnMap;

		protected virtual void OnInitialized()
		{
		}

		protected virtual void OnActivated()
		{
		}

		protected virtual void OnDeactivated()
		{
		}

		protected virtual void OnReset()
		{
		}

		protected virtual void OnStateChanged()
		{
		}

		protected virtual void OnUpdate(float deltaTime)
		{
		}

		protected virtual Vector3 GetNavigationPosition()
		{
			return Position;
		}

		protected virtual string GetShortDescription()
		{
			return shortDescription;
		}

		protected virtual string GetLongDescription()
		{
			return longDescription;
		}

		protected virtual void OnTimeIsUp()
		{
			Fail();
		}

		public void Initialize()
		{
			ResetState();
			Player = ServiceLocator.GetPlayerModel();
			EffectManager = ServiceLocator.Get<EffectsManager>();
			shortDescription = GetText(shortDescriptionID);
			longDescription = GetText(longDescriptionID);
			shortDescriptionOrg = shortDescription;
			longDescriptionOrg = longDescription;
			OnInitialized();
			SetIsActive(isActive: false);
		}

		public void Activate(ObjectiveData data, Action objectiveChanged)
		{
			this.data = data;
			stateChanged = objectiveChanged;
			if (dataManagement.CleanVehicle)
			{
				data.CleanVehicle();
			}
			if (dataManagement.CleanNPC)
			{
				data.CleanNPC();
			}
			CurrentState = State.Active;
			OnActivated();
		}

		public void Deactivate()
		{
			CurrentState = State.Inactive;
			OnDeactivated();
		}

		public void ResetState()
		{
			CurrentState = State.Inactive;
			OnReset();
		}

		private void Update()
		{
			if (CurrentState == State.Active)
			{
				UpdateEffect();
				if (data != null)
				{
					data.Update(this);
				}
				OnUpdate(Time.deltaTime);
				if (timeLimit > 0f && timer.Done())
				{
					timer.Stop();
					OnTimeIsUp();
				}
			}
		}

		protected void Finish()
		{
			CurrentState = State.Finished;
		}

		protected void Fail()
		{
			CurrentState = State.Failed;
		}

		private void SetState(State state)
		{
			SetIsActive(state == State.Active);
			if (state != currentState)
			{
				currentState = state;
				if (stateChanged != null)
				{
					stateChanged();
				}
				OnStateChanged();
				switch (state)
				{
				case State.Active:
					ActivateEffect();
					break;
				case State.Inactive:
					DeactivateEffect();
					stateChanged = null;
					break;
				}
			}
		}

		private void SetIsActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
			if (isActive && timeLimit > 0f)
			{
				timer.Run(timeLimit);
			}
			else
			{
				timer.Stop();
			}
		}

		private float GetRemainingTime()
		{
			if (timeLimit <= 0f)
			{
				return 0f;
			}
			return timeLimit - timer.GetProgress() * timeLimit;
		}

		private void ActivateEffect()
		{
			GameplayEffect gameplayEffect = (!disableDefaultEffect) ? EffectManager.objectiveEffectPrefab : customEffect;
			if (!(gameplayEffect == null))
			{
				effect = EffectManager.GetEffect(gameplayEffect);
				effect.Activate(GetEffectPosition());
			}
		}

		private void DeactivateEffect()
		{
			if (!(effect == null))
			{
				effect.Deactivate();
				effect = null;
			}
		}

		private void UpdateEffect()
		{
			if (!(effect == null))
			{
				effect.transform.position = GetEffectPosition();
			}
		}

		private void SetShortDescription(string text)
		{
			shortDescription = text;
			OndescriptionChanged();
		}

		private void SetLongDescription(string text)
		{
			longDescription = text;
			OndescriptionChanged();
		}

		private void OndescriptionChanged()
		{
			if (TextChanged != null)
			{
				TextChanged();
			}
		}

		private Vector3 GetEffectPosition()
		{
			return GetNavigationPosition() + effectOffset;
		}

		private string GetText(int textID)
		{
			return LocalizationManager.Instance.GetText(textID);
		}

		private void OnDrawGizmos()
		{
			Color color = Gizmos.color;
			Color blue = Color.blue;
			Gizmos.color = new Color(blue.r, blue.g, blue.b, 0.75f);
			Gizmos.DrawSphere(base.transform.position, 0.6f);
			Gizmos.color = color;
		}
	}
}
