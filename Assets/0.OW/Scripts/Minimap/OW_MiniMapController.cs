using System;
using System.Collections.Generic;
using _0.DucLib.Scripts.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _0.OW.Scripts.Minimap
{
    public class OW_MiniMapController : MonoBehaviour
    {
        [ReadOnly] public Transform playerIcon;
        public Transform miniMapCamera;
        public Transform player;
        public Transform mapPlane;
        public GameObject playerIconPrefab;

        public Transform exclamationMark;
        public Transform questionMask;
        public Transform starMask;

        public Transform collectZone;
        public Transform defeatZone;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            playerIcon = Instantiate(playerIconPrefab).transform;
        }

        private void FixedUpdate()
        {
            playerIcon.transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
            playerIcon.transform.position = new Vector3(player.position.x, mapPlane.position.y + 5f, player.position.z);
            // playerIcon.position = new Vector3(player.position.x, playerIcon.position.y, player.position.z);
            miniMapCamera.position = new Vector3(player.position.x, miniMapCamera.position.y, player.position.z);
        }

        public void HideMask()
        {
            exclamationMark.HideObject();
            questionMask.HideObject();
            starMask.HideObject();
        }

        public void ShowExclamationMask(Vector3 pos)
        {
            HideMask();
            exclamationMark.transform.position = new Vector3(pos.x, exclamationMark.position.y, pos.z);
            exclamationMark.ShowObject();
        }

        public void ShowQuestionMask(Vector3 pos)
        {
            HideMask();
            questionMask.transform.position = new Vector3(pos.x, questionMask.position.y, pos.z);
            questionMask.ShowObject();
        }

        public void ShowStarMask(Vector3 pos)
        {
            HideMask();
            starMask.transform.position = new Vector3(pos.x, starMask.position.y, pos.z);
            starMask.ShowObject();
        }

        public void ShowCollectZone(List<Transform> points)
        {
            HideMask();
            if (points == null || points.Count == 0) return;

            Vector3 sum = Vector3.zero;

            foreach (var point in points)
            {
                sum += point.position;
            }

            var result = sum / points.Count;
            collectZone.position = new Vector3(result.x, collectZone.position.y, result.z);
            collectZone.ShowObject();
        }
    }
}