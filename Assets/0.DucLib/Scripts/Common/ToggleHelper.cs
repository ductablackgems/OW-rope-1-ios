using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _0.DucLib.Scripts.Common
{
    [RequireComponent(typeof(Toggle))]
    public abstract class ToggleHelper : MonoBehaviour
    {
        [SerializeField] protected Toggle toggle;
        [SerializeField] protected GameObject graphicObject;

        public Toggle Toggle => toggle;
        private void Awake()
        {
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    IsOnItem();
                }
                else
                {
                    IsOffItem();
                }
            });
        }
        
        public abstract void IsOnItem();
        protected abstract void IsOffItem();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (toggle == null) toggle = GetComponent<Toggle>();
        }
#endif
    }
}