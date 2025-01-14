using UnityEngine;

public class GarbageCapacity : MonoBehaviour
{
    private static GarbageCapacity instance;

    private float nextValue;

    private float delay;

    private float startFillTime;

    private float freezeValue;

    private bool up;

    private bool canDoAction;

    private bool inDump;

    private float dumpingDelay;

    public UISlider slider;
    public SpriteRenderer spriteSlider;

    public GameObject text;

    public static GarbageCapacity Instance => instance;

    public bool IsActive
    {
        get;
        private set;
    }

    private void Awake()
    {
        instance = this;
        IsActive = true;
        Deactivate();
    }

    private void Update()
    {
        if (inDump)
        {
            slider.value = 1f - (Time.time - startFillTime) / dumpingDelay;
            if (slider.value <= 0f)
            {
                slider.value = 0f;
                inDump = false;
                dumpingDelay = 0f;
            }
        }
        else
        {
            if (!canDoAction)
            {
                return;
            }
            if (up)
            {
                if (slider.value < nextValue)
                {
                    slider.value = Mathf.LerpUnclamped(slider.value, nextValue, Time.deltaTime * 2f);
                    return;
                }
                slider.value = nextValue;
                canDoAction = false;
            }
            else if (slider.value > nextValue)
            {
                slider.value = Mathf.LerpUnclamped(slider.value, nextValue, Time.deltaTime * 2f);
            }
            else
            {
                slider.value = nextValue;
                canDoAction = false;
            }
        }
        spriteSlider.size = new Vector2(slider.value * 0.85f, 0.09f);
    }

    public void SetValue(int value, float inDelay, bool isUp)
    {
        if (value == 0)
        {
            nextValue = 0f;
        }
        else
        {
            nextValue = (float)value / 4f;
        }
        freezeValue = slider.value;
        delay = inDelay;
        up = isUp;
        canDoAction = true;
        startFillTime = Time.time;
    }

    public void Dump(float delay)
    {
        dumpingDelay = delay;
        inDump = true;
        startFillTime = Time.time;
    }

    public void StopDump()
    {
        inDump = false;
        dumpingDelay = 0f;
        startFillTime = 0f;
    }

    public void ResetSlider()
    {
        slider.value = 0f;
        nextValue = 0f;
        delay = 0f;
        startFillTime = 0f;
        freezeValue = 0f;
        up = false;
        canDoAction = false;
        inDump = false;
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            text.SetActive(value: true);
            slider.gameObject.SetActive(value: true);
        }
    }

    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            text.SetActive(value: false);
            slider.gameObject.SetActive(value: false);
        }
    }
}
