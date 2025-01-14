using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

namespace BG_Library.Common
{
    public class LoadSceneManager : MonoBehaviour
    {
        static LoadSceneManager _instance;
        public static LoadSceneManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Setup();
                }

                return _instance;
            }
        }

        LoadSceneSettings setting;

        static void Setup()
        {
            _instance = Instantiate(LoadSource.LoadObject<GameObject>("Load scene canvas")
                , DDOL.Instance.transform).GetComponent<LoadSceneManager>();

            _instance.setting = LoadSource.LoadObject<LoadSceneSettings>(LoadSceneSettings.AssetName);

            _instance.SettingCanvas();

            _instance.animDefault =
                _instance.SpawnAnim(_instance.setting.LoadSceneAnimDefault);

            SceneManager.sceneLoaded += (scene, sceneMode) =>
            {
                if (currentLoadType == LoadType.Default)
                {
                    IsLoading = false;
                }

                Instance.SettingCanvas();
                OnLoadSceneDone?.Invoke(scene.name);
            };
        }

        public static System.Action<string> OnLoadSceneDone;
        public static System.Action<string> OnSceneReady;

        public static System.Action BeforeExitScene;
        public static System.Action OnLoadingScene;

        public static bool IsLoading { get; private set; }
        static LoadType currentLoadType;

        public string CurrentScene => SceneManager.GetActiveScene().name;
        public string LastScene = "";

        [SerializeField] AnimLoadSceneBase animDefault;
        [SerializeField] List<AnimLoadSceneBase> listAnim = new List<AnimLoadSceneBase>();

        public void Preload()
		{

		}

        public AnimLoadSceneBase LoadScene(string sceneName, AnimLoadSceneBase animPrefab = null
            , System.Action onDone = null, LoadType loadSceneType = LoadType.LoadSceneAnim)
        {
            IsLoading = true;
            currentLoadType = loadSceneType;

            FreeRAM();

            if (loadSceneType == LoadType.LoadSceneAnim)
            {
                if (animPrefab == null)
                {
                    animDefault.BeforeLoad(sceneName, () =>
                    {
                        _ = Instance.StartCoroutine(LoadSceneAnimIE(sceneName, animDefault, onDone));
                    });

                    return animDefault;
                }
                else
                {
                    var _anim = GetAnim(animPrefab);
                    _anim.BeforeLoad(sceneName, () =>
                    {
                        _ = Instance.StartCoroutine(LoadSceneAnimIE(sceneName, _anim, onDone));
                    });

                    return _anim;
                }
            }
            else if (loadSceneType == LoadType.Default)
            {
                LoadSceneDefault(sceneName);
			}

			return null;
		}

        public void RemoveAllAnimObj()
        {
            while (listAnim.Count > 0)
            {
                Destroy(listAnim[0].gameObject);
                listAnim.RemoveAt(0);
            }
        }

        /// <summary>
        /// Check trong list truoc
        /// </summary>
        AnimLoadSceneBase GetAnim(AnimLoadSceneBase prefab)
        {
            var _anim = listAnim.Find(anim => anim.GetType() == prefab.GetType());

            if (_anim == null)
            {
                _anim = SpawnAnim(prefab);
                listAnim.Add(_anim);
            }

            return _anim;
        }

        /// <summary>
        /// Spawn 1 anim obj moi hoan toan
        /// </summary>
		AnimLoadSceneBase SpawnAnim(AnimLoadSceneBase prefab)
        {
            var anim = Instantiate(prefab.gameObject
                , _instance.transform).GetComponent<AnimLoadSceneBase>();

            if (anim == null)
            {
				Debug.LogError($"Cannot spawn anim {anim}");
            }
            anim.SetupDefault();

            return anim;
        }

        IEnumerator LoadSceneAnimIE(string sceneName, AnimLoadSceneBase anim
            , System.Action onDone = null)
        {
            anim.StartLoad(sceneName);

            var _lastScene = CurrentScene;

            BeforeExitScene?.Invoke();
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName.ToString());

            LastScene = _lastScene;

            bool check = true;
            while (!operation.isDone)
            {
                anim.UpdateLoad(operation.progress);

                if (operation.progress > 0.5f && check)
                {
                    OnLoadingScene?.Invoke();
                    check = false;
                }
                yield return null;
            }

            onDone?.Invoke();
            anim.EndLoad(sceneName, () =>
            {
                IsLoading = false;
                OnSceneReady?.Invoke(sceneName);
            });
        }

        void LoadSceneDefault(string sceneName)
        {
            BeforeExitScene?.Invoke();
            SceneManager.LoadScene(sceneName);
        }

        void SettingCanvas()
		{
            var canvas = _instance.GetComponent<Canvas>();

            //canvas.worldCamera = Camera.main;
            //canvas.sortingLayerName = _instance.setting.Layer;
            canvas.sortingOrder = _instance.setting.OrderInLayer;
        }

        void FreeRAM()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public enum LoadType
        {
            Default = 0,
            LoadSceneAnim = 1
        }
    }
}
