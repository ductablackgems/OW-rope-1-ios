using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BG_Library.Common 
{
	/// <summary>
	/// Viet lai cac trang thai cua Panel load scene, khi co load scene xay ra
	/// </summary>
	public abstract class AnimLoadSceneBase : MonoBehaviour
	{
		public void SetupDefault()
		{
			gameObject.SetActive(false);
		}

		/// <summary>
		/// Neu ghi de, bat buoc phai goi "onComplete"
		/// </summary>
		public virtual void BeforeLoad(string sceneName, System.Action onComplete)
		{
			onComplete?.Invoke();
		}

		/// <summary>
		/// Bat dau load scene nao do
		/// </summary>
		public abstract void StartLoad(string sceneName);

        /// <summary>
        /// Update tien do load scene
        /// </summary>
        public virtual void UpdateLoad(float progress)
        {

        }

        /// <summary>
        /// Ket thuc load scene
        /// </summary>
        public abstract void EndLoad(string sceneName, System.Action onComplete = null);
	}
}