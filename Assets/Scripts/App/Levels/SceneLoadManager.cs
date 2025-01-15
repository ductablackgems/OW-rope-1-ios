using App.SaveSystem;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace App.Levels
{
    public class SceneLoadManager : MonoBehaviour
    {
        public Image loadingUI;
        public Text valueLoadingUI;

        private SettingsSaveEntity settingsSave;

        public AudioClip[] soundForLoading;

        private AudioSource audio;

        private float a;

        private double b;

        private void Start()
        {
            settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
            GameObject gameObject = ServiceLocator.GetGameObject("MusicPlayer", showError: false);
            audio = GetComponent<AudioSource>();
        }

        public void LoadMainMenu()
        {
            StartCoroutine(PlayLoading("Menu"));
        }

        public void LoadGame()
        {
            Debug.Log("Load game");
            StartCoroutine(PlayLoading("SmallCity (Simple)"));
        }

        private IEnumerator PlayLoading(string NameLevel)
        {
            loadingUI.transform.parent.gameObject.SetActive(value: true);
            if (SceneManager.GetActiveScene().name == "Menu")
            {
                yield return new WaitForSeconds(1f);
                if (settingsSave.memoryRam < 1100f)
                {
                    yield return new WaitForSeconds(2f);
                }
            }
            AsyncOperation operation = SceneManager.LoadSceneAsync(NameLevel);
            operation.allowSceneActivation = false;
            while (!operation.isDone)
            {
                yield return operation.isDone;
                a = operation.progress;
                b = Math.Round(a, 2);
                loadingUI.fillAmount = (float)b;
                valueLoadingUI.text = ((float)b).ToString("P1");
                if (operation.progress >= 0.89f)
                {
                    yield return new WaitForSeconds(4f);
                    operation.allowSceneActivation = true;
                }
            }
            UnityEngine.Debug.Log("load done");
            audio.PlayOneShot(soundForLoading[1]);
        }

        public void ProgressSoundLoading()
        {
            audio.PlayOneShot(soundForLoading[0]);
        }
    }
}
