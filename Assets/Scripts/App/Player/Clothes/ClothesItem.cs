using System;
using UnityEngine;

namespace App.Player.Clothes
{
    public class ClothesItem : MonoBehaviour
    {
        public string tid;

        public ClothesKind kind;

        public int price;

        public Material[] material;

        [NonSerialized]
        public bool buyed;

        [NonSerialized]
        public bool weared;

        public void SetActive(bool active, SkinnedMeshRenderer[] renderer = null)
        {
            if (active && renderer != null && material.Length > 0)
            {
                for (int i = 0; i < renderer.Length; i++)
                {
                    renderer[i].material = material[i];
                }
            }
            base.gameObject.SetActive(active);
        }
    }
}
