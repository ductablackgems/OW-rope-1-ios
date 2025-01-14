using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG_Library.Common
{
    public class LoadSceneDemo : MonoBehaviour
    {
        [SerializeField] string sceneName;

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.L))
			{
				LoadSceneManager.Instance.LoadScene(sceneName);
			}
		}
	}
}