#if PLATFORM_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEditor.Android;
using UnityEditor.Build;
using Graphics = System.Drawing.Graphics;

public class GameIconSetupWindow : EditorWindow
{
    private Texture2D gameIcon;
    private const string insideSuffix = "_inside";
    private const string foregroundSuffix = "_foreground";
    private const string imageTail = ".png";

    [MenuItem("Tools/Game icon setup")]
    static void Init()
    {
        var window = (GameIconSetupWindow)EditorWindow.GetWindow(typeof(GameIconSetupWindow));
        window.Show();
    }

    private void OnGUI()
    {
        gameIcon = (Texture2D)EditorGUILayout.ObjectField("Game Icon", gameIcon, typeof(Texture2D), false);

        if (GUILayout.Button("Generator & Setup"))
        {
            string gameIconPath = AssetDatabase.GetAssetPath(gameIcon);
            string imageName = gameIcon.name;
            string newInsideIconName = GetImageNameForSuffix(imageName, insideSuffix);
            string newForegroundIconName = GetImageNameForSuffix(imageName, foregroundSuffix);

            string savePath = gameIconPath.Replace(imageName + imageTail, "");
            string insideSavePath = Path.Combine(savePath, newInsideIconName);
            string foregroundSavePath = Path.Combine(savePath, newForegroundIconName);

            Image img = Image.FromFile(gameIconPath);
            Size imageSize = new Size(img.Width, img.Height);

            float widthAdd = (imageSize.Width / 0.75f) - imageSize.Width;
            float heightAdd = (imageSize.Height / 0.75f) - imageSize.Height;

            //create blank canvas + save
            Bitmap blankCanvas = new Bitmap(imageSize.Width, imageSize.Height);
            blankCanvas.Save(foregroundSavePath, ImageFormat.Png);

            // Draw image to blank canvas + save
            blankCanvas = new Bitmap(imageSize.Width + (int)widthAdd, imageSize.Height + (int)heightAdd);
            Graphics gfx = Graphics.FromImage(blankCanvas);
            gfx.DrawImage(img, widthAdd / 2, heightAdd / 2, img.Width, img.Height);
            blankCanvas.Save(insideSavePath, ImageFormat.Png);

            AssetDatabase.Refresh();
            SetPlayerSettingIcon(gameIcon,
                AssetDatabase.LoadAssetAtPath<Texture2D>(insideSavePath),
                AssetDatabase.LoadAssetAtPath<Texture2D>(foregroundSavePath));
        }
    }

    private void SetPlayerSettingIcon(Texture2D gameIcon, Texture2D insideGameIcon, Texture2D foreGroundGameIcon)
    {
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { gameIcon });
        NamedBuildTarget platform = NamedBuildTarget.Android;
        PlatformIconKind kind = AndroidPlatformIconKind.Adaptive;

        PlatformIcon[] icons = PlayerSettings.GetPlatformIcons(platform, kind);
        icons[0].SetTextures(new Texture2D[]
        {
            insideGameIcon,
            foreGroundGameIcon
        });
        PlayerSettings.SetPlatformIcons(platform, kind, icons);
    }

    private string GetImageNameForSuffix(string name, string suffix)
    {
        return name + suffix + imageTail;
    }
}
#endif