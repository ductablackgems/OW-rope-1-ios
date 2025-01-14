using UnityEngine;

namespace App.Util
{
    [RequireComponent(typeof(UISlider))]
    public class HealthBar : MonoBehaviour
    {
        public Health health;

        public UILabel label;

        public SpriteRenderer sliderSprite;

        private UISlider slider;

        private float lastMaxHealth = -1f;

        private float lastHealth = -1f;

        protected virtual void Awake()
        {
            slider = this.GetComponentSafe<UISlider>();
        }

        protected virtual void Update()
        {
            if (health != null)
            {
                sliderSprite.size = new Vector2(health.GetCurrentHealth() * 0.85f, 0.09f);
                slider.value = health.GetCurrentHealth();
                if (label != null && (lastHealth != health.GetCurrentHealthNumeric() || lastMaxHealth != health.maxHealth))
                {
                    lastHealth = health.GetCurrentHealthNumeric();
                    lastMaxHealth = health.maxHealth;
                    label.text = $"{(int)lastHealth}/{(int)lastMaxHealth}";
                }
            }
        }
    }
}
