using System;
using UnityEngine;

namespace App.Garage
{
	public class GarageDoors : MonoBehaviour
	{
		public float speed = 1f;

		public Transform openedPostionTransform;

		private Vector3 openedPosition;

		private Vector3 closedPosition;

		public Animator a;

		private AudioSource audio;

		public GarageDoorsState State
		{
			get;
			private set;
		}

		public event Action OnClosed;

		public event Action OnStartOpening;

		public void Open()
		{
			if (State == GarageDoorsState.Closed && this.OnStartOpening != null)
			{
				this.OnStartOpening();
			}
			State = GarageDoorsState.Opening;
		}

		public void Close()
		{
			State = GarageDoorsState.Closing;
		}

		private void Awake()
		{
			State = GarageDoorsState.Closed;
			openedPosition = openedPostionTransform.position;
			closedPosition = base.transform.position;
			if (a != null)
			{
				a.enabled = false;
			}
			audio = GetComponent<AudioSource>();
		}

		private void Update()
		{
			if (State == GarageDoorsState.Opening)
			{
				if (base.transform.position == openedPosition)
				{
					State = GarageDoorsState.Opened;
				}
				else
				{
					if (a.GetBool("PlayingAnim"))
					{
						return;
					}
					if (a != null)
					{
						if (!audio.isPlaying)
						{
							audio.Play();
						}
						a.enabled = true;
						a.SetInteger("Open", 1);
					}
					base.transform.position = Vector3.MoveTowards(base.transform.position, openedPosition, Time.deltaTime * speed);
				}
			}
			else if (State == GarageDoorsState.Closing)
			{
				if (base.transform.position == closedPosition)
				{
					State = GarageDoorsState.Closed;
					if (this.OnClosed != null)
					{
						this.OnClosed();
					}
				}
				else
				{
					if (a.GetBool("PlayingAnim"))
					{
						return;
					}
					if (a != null)
					{
						if (!audio.isPlaying)
						{
							audio.Play();
						}
						a.enabled = true;
						a.SetInteger("Open", 0);
					}
					base.transform.position = Vector3.MoveTowards(base.transform.position, closedPosition, Time.deltaTime * speed);
				}
			}
			if ((base.transform.position == openedPosition || base.transform.position == closedPosition) && !a.GetBool("PlayingAnim"))
			{
				if (a.enabled)
				{
					a.enabled = false;
				}
			}
			else if (!a.enabled)
			{
				a.enabled = true;
			}
		}
	}
}
