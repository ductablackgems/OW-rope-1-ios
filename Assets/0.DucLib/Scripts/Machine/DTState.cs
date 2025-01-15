using UnityEngine;

namespace _0.DucLib.Scripts.Machine
{
    public abstract class DTState : MonoBehaviour
    {

        public virtual void Enter()
        {
            AddListeners();
        }

        public virtual void Exit()
        {
            RemoveListeners();
        }

        protected virtual void OnDestroy()
        {
            RemoveListeners();
        }

        protected virtual void AddListeners()
        {
        }

        protected virtual void RemoveListeners()
        {
        }

        public virtual void Reload()
        {
        }
    }
}