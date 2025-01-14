using System.Collections.Generic;
using GoogleMobileAds.Api;
using BG_Library.Common;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.NET.Native_custom.Elements
{
    public class NativeUIImages : NativeUIBase
    {
        [SerializeField] private RectTransform parentMask;
        [SerializeField] private KeepRatio ratio;
        [SerializeField] private RawImage img;
        private Texture _defaultImg;

        public override void Setup(NativeAd n, NativeUIManager nativeManager)
        {
            _defaultImg = nativeManager.defaultImage;
            
            List<Texture2D> i = n.GetImageTextures();
            var parentSize = parentMask.rect.size;

            if (i != null && i.Count > 0)
            {
                img.texture = i[0];
                ratio.SampleRatio = new Vector2(img.texture.width, img.texture.height);
                ratio.Resize();
                
                var collider =  img.GetComponent<BoxCollider>();
                collider.center = Vector3.zero;
                collider.size = new Vector3(parentSize.x, parentSize.y, 10);
                n.RegisterImageGameObjects(new List<GameObject> { gameObject });
            }
            else
            {
                SetDefaultWhenDestroy();
            }
        }

        public override void SetDefaultWhenDestroy()
        {
            if (!_defaultImg)
            {
                return;
            }
            img.texture = _defaultImg;
            ratio.SampleRatio = new Vector2(img.texture.width, img.texture.height);
            ratio.Resize();
        }
    }
}
