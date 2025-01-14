using System;
using UnityEngine;
using UnityEngine.Events;

namespace _0.DucLib.Scripts.Common
{
    public class EventStart : MonoBehaviour
    {
        [SerializeField] private UnityEvent eventStart;

        private void Start()
        {
            eventStart?.Invoke();
        }
    }
}