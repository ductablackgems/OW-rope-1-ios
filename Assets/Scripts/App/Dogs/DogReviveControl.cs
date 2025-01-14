using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.Dogs
{
	public class DogReviveControl : MonoBehaviour
	{
		public enum State
		{
			Unavailable,
			Available,
			Revived
		}

		[SerializeField]
		private GameObject unavailableObject;

		[SerializeField]
		private Button button;

		[SerializeField]
		private Text textRevived;

		[SerializeField]
		private Text textName;

		[SerializeField]
		private Transform dogPosition;

		private State state;

		private Dog dog;

		private Action<DogReviveControl> reviveRequest;

		public State CurrentState
		{
			get
			{
				return state;
			}
			set
			{
				SetState(value);
			}
		}

		public Dog Dog
		{
			get
			{
				return dog;
			}
			set
			{
				SetDog(value);
			}
		}

		public void Initialize(Action<DogReviveControl> reviveRequest)
		{
			this.reviveRequest = reviveRequest;
			button.onClick.AddListener(OnButtonClicked);
		}

		private void OnButtonClicked()
		{
			reviveRequest.SafeInvoke(this);
		}

		private void SetState(State newState)
		{
			state = newState;
			textRevived.gameObject.SetActive(state == State.Revived);
			unavailableObject.gameObject.SetActive(state == State.Unavailable);
			button.gameObject.SetActive(state == State.Available);
		}

		private void SetDog(Dog dog)
		{
			this.dog = dog;
			if (!(dog == null))
			{
				textName.text = dog.Name;
			}
		}
	}
}
