using App.GUI.Panels;
using App.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace App.GUI
{
    public class HackingPanel : AbstractPanel
    {
        public enum Result
        {
            Cancel,
            Success,
            Fail
        }

        private const int MAX_SEQUENCE_LENGTH = 5;

        private const int MAX_OPTIONS_LENGTH = 8;

        private const float CRITICAL_THRESHOLD = 0.33f;

        private const float WARNING_THRESHOLD = 0.66f;

        [Space]
        [SerializeField]
        private Sprite[] images;

        [SerializeField]
        private HackingItemControl itemPrefab;

        [Header("Controls")]
        [SerializeField]
        private Button buttonCancel;

        [SerializeField]
        private HorizontalLayoutGroup sequenceList;

        [SerializeField]
        private GridLayoutGroup optionsList;

        [Header("Indicators")]
        [SerializeField]
        private Image[] blinkIndicators;

        [SerializeField]
        private Image progressIndicator;

        [SerializeField]
        private RectTransform countdown;

        [SerializeField]
        private Color colorNormal;

        [SerializeField]
        private Color colorWarning;

        [SerializeField]
        private Color colorCritical;

        [SerializeField]
        private float blinkInterval = 0.5f;

        [Header("Sounds")]
        [SerializeField]
        private AudioClip clickSound;

        [SerializeField]
        private AudioClip successSound;

        private List<HackingItemControl> sequence = new List<HackingItemControl>(8);

        private List<HackingItemControl> options = new List<HackingItemControl>(16);

        private List<Sprite> gameSprites = new List<Sprite>(32);

        private List<Sprite> availableOptions = new List<Sprite>();

        private bool isInitialized;

        private bool isRunning;

        private float maxProgressWidth;

        private Coroutine blinkingCoroutine;

        private Coroutine errorBlinkCoroutine;

        private Text textMinutes;

        private Text textSeconds;

        private Text textMiliseconds;

        private Image imageSequnceBG;

        private Color colorSequnceBG;

        private float timeLimit;

        private float maxTimeLimit;

        private float timePenalization;

        private Action<Result> onClose;

        private CameraSounds sounds;

        public void StartGame(float hackingTimeLimit, float penalization, Action<Result> onClose)
        {
            Initialize();
            this.onClose = onClose;
            CleanControls(sequence);
            CleanControls(options);
            LoadGameSprites();
            GenerateSequence();
            StopBlinking();
            imageSequnceBG.color = colorSequnceBG;
            progressIndicator.color = colorNormal;
            ShowBlinkIndicators(show: false);
            maxTimeLimit = hackingTimeLimit;
            timeLimit = maxTimeLimit;
            timePenalization = penalization;
            isRunning = true;
        }

        public override PanelType GetPanelType()
        {
            return PanelType.HackingScreen;
        }

        private void Update()
        {
            if (!isRunning)
            {
                return;
            }
            timeLimit -= Time.unscaledDeltaTime;
            if (timeLimit <= 0f)
            {
                isRunning = false;
                OnClose(Result.Fail);
                return;
            }
            float num = timeLimit / maxTimeLimit;
            bool num2 = num < 0.33f;
            Color color = num2 ? colorCritical : ((num < 0.66f) ? colorWarning : colorNormal);
            int num3 = Mathf.FloorToInt(timeLimit / 60f);
            int num4 = Mathf.FloorToInt(timeLimit - (float)num3 * 60f);
            float num5 = (timeLimit - (float)num3 - (float)num4) * 100f;
            textMinutes.text = num3.ToString("00");
            textSeconds.text = num4.ToString("00");
            textMiliseconds.text = num5.ToString("00");
            progressIndicator.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * maxProgressWidth);
            progressIndicator.color = color;
            if (num2 && blinkingCoroutine == null && blinkInterval > 0f)
            {
                blinkingCoroutine = StartCoroutine(Blink_Coroutine());
            }
        }

        private void OnEnable()
        {
            isRunning = false;
        }

        private void OnDisable()
        {
            isRunning = false;
        }

        private void OnOptionsItemClicked(HackingItemControl selectedItem)
        {
            PlaySound(clickSound);
            if (!ValidateSelection(selectedItem))
            {
                StopErrorBlinking(resetColor: false);
                errorBlinkCoroutine = StartCoroutine(ErrorBlink_Coroutine());
                timeLimit -= timePenalization;
            }
            else if (GetCurrentSequnceItem() == null)
            {
                isRunning = false;
                PlaySound(successSound);
                OnClose(Result.Success);
            }
        }

        private void OnButtonCancelClicked()
        {
            CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_atm_button, () =>
            {
                PlaySound(clickSound);
                OnClose(Result.Cancel);
            });
        }

        private void Initialize()
        {
            if (!isInitialized)
            {
                textMinutes = countdown.GetComponentInChildren<Text>("TextMinutes", includeInactive: true);
                textSeconds = countdown.GetComponentInChildren<Text>("TextSeconds", includeInactive: true);
                textMiliseconds = countdown.GetComponentInChildren<Text>("TextMiliseconds", includeInactive: true);
                sounds = ServiceLocator.Get<CameraSounds>();
                imageSequnceBG = sequenceList.GetComponentInChildren<Image>("BG", includeInactive: true);
                colorSequnceBG = imageSequnceBG.color;
                buttonCancel.onClick.AddListener(OnButtonCancelClicked);
                GenerateItemControls();
                maxProgressWidth = progressIndicator.rectTransform.rect.width;
                Image[] array = blinkIndicators;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].color = colorCritical;
                }
                isInitialized = true;
            }
        }

        private void GenerateItemControls()
        {
            int num = 13;
            for (int i = 0; i < num; i++)
            {
                HackingItemControl hackingItemControl = UnityEngine.Object.Instantiate(itemPrefab);
                if (i < 5)
                {
                    hackingItemControl.transform.SetParent(sequenceList.transform);
                    hackingItemControl.Initialize(isReadOnly: true);
                    sequence.Add(hackingItemControl);
                }
                else
                {
                    hackingItemControl.transform.SetParent(optionsList.transform);
                    hackingItemControl.Initialize(isReadOnly: false);
                    hackingItemControl.OnClick += OnOptionsItemClicked;
                    options.Add(hackingItemControl);
                }
            }
        }

        private void LoadGameSprites()
        {
            gameSprites.Clear();
            gameSprites.AddRange(images);
            gameSprites.Shuffle();
            int num = Mathf.Max(0, gameSprites.Count - 8);
            int num2 = gameSprites.Count;
            while (num2-- > 0 && num > 0)
            {
                gameSprites.RemoveAt(num2);
                num--;
            }
            for (int i = 0; i < options.Count; i++)
            {
                options[i].Sprite = gameSprites[i];
            }
            GenerateSequence();
        }

        private void GenerateSequence()
        {
            gameSprites.Shuffle();
            for (int i = 0; i < 5; i++)
            {
                sequence[i].Sprite = gameSprites[i];
            }
        }

        private void CleanControls(List<HackingItemControl> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Sprite = null;
            }
        }

        private HackingItemControl GetCurrentSequnceItem()
        {
            for (int i = 0; i < sequence.Count; i++)
            {
                HackingItemControl hackingItemControl = sequence[i];
                if (hackingItemControl.Sprite != null)
                {
                    return hackingItemControl;
                }
            }
            return null;
        }

        private bool ValidateSelection(HackingItemControl selectedItem)
        {
            HackingItemControl currentSequnceItem = GetCurrentSequnceItem();
            if (selectedItem.Sprite == currentSequnceItem.Sprite)
            {
                currentSequnceItem.Sprite = null;
                return true;
            }
            return false;
        }

        private void ShowBlinkIndicators(bool show)
        {
            for (int i = 0; i < blinkIndicators.Length; i++)
            {
                blinkIndicators[i].gameObject.SetActive(show);
            }
        }

        private IEnumerator Blink_Coroutine()
        {
            bool isActive = true;
            while (isRunning)
            {
                ShowBlinkIndicators(isActive);
                yield return new WaitForSecondsRealtime(blinkInterval);
                isActive = !isActive;
            }
            blinkingCoroutine = null;
        }

        private void OnClose(Result result)
        {
            StopBlinking();
            StopErrorBlinking(resetColor: true);
            onClose(result);
        }

        private void StopBlinking()
        {
            if (blinkingCoroutine != null)
            {
                StopCoroutine(blinkingCoroutine);
                blinkingCoroutine = null;
            }
        }

        private void StopErrorBlinking(bool resetColor)
        {
            if (resetColor)
            {
                imageSequnceBG.color = colorSequnceBG;
            }
            if (errorBlinkCoroutine != null)
            {
                StopCoroutine(errorBlinkCoroutine);
                errorBlinkCoroutine = null;
            }
        }

        private void PlaySound(AudioClip clip)
        {
            if (!(sounds == null))
            {
                sounds.AudioSource.PlayOneShot(clip);
            }
        }

        private IEnumerator ErrorBlink_Coroutine()
        {
            imageSequnceBG.color = colorCritical;
            yield return new WaitForSecondsRealtime(0.2f);
            imageSequnceBG.color = colorSequnceBG;
            errorBlinkCoroutine = null;
        }
    }
}
