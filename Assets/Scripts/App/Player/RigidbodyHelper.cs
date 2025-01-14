using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Player
{
	public class RigidbodyHelper : MonoBehaviour
	{
		private Rigidbody _rigidbody;

		private Animator animator;

		private HashSet<object> kinematicSetters = new HashSet<object>();

		private bool initialized;

		public event Action<bool> OnKinematic;

		public void SetKinematic(bool kinematic, object setBy)
		{
			if (!initialized)
			{
				Init();
			}
			if (kinematic)
			{
				kinematicSetters.Add(setBy);
				if (!_rigidbody.isKinematic)
				{
					_rigidbody.isKinematic = true;
					animator.applyRootMotion = false;
					if (this.OnKinematic != null)
					{
						this.OnKinematic(obj: true);
					}
				}
				return;
			}
			kinematicSetters.Remove(setBy);
			if (_rigidbody.isKinematic && kinematicSetters.Count == 0)
			{
				_rigidbody.isKinematic = false;
				if (this.OnKinematic != null)
				{
					this.OnKinematic(obj: false);
				}
			}
		}

		private void Awake()
		{
			if (!initialized)
			{
				Init();
			}
		}

		private void Init()
		{
			initialized = true;
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			animator = this.GetComponentSafe<Animator>();
		}
	}
}
