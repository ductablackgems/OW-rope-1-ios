using System.IO;
using UnityEngine;

namespace com.ootii.Helpers
{
	public static class GraphicsHelper
	{
		public static Sprite TextureToSprite(Texture2D texture)
		{
			if (texture == null)
			{
				return null;
			}
			return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		}

		public static Sprite TextureToSprite(Texture texture)
		{
			return TextureToSprite(texture as Texture2D);
		}

		public static Texture2D LoadImage(string filePath)
		{
			Texture2D texture2D = null;
			if (File.Exists(filePath))
			{
				byte[] data = File.ReadAllBytes(filePath);
				texture2D = new Texture2D(2, 2);
				texture2D.LoadImage(data);
			}
			return texture2D;
		}

		public static void SaveAsPNG(Texture2D texture, string filePath)
		{
			byte[] bytes = texture.EncodeToPNG();
			File.WriteAllBytes(filePath, bytes);
		}

		public static void SaveAsJPG(Texture2D texture, string filePath, int quality = 100)
		{
			byte[] bytes = texture.EncodeToJPG(quality);
			File.WriteAllBytes(filePath, bytes);
		}

		public static Texture2D TakeScreenshot(Camera camera, int width, int height)
		{
			RenderTexture renderTexture2 = camera.targetTexture = new RenderTexture(width, height, 24);
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
			camera.Render();
			RenderTexture.active = renderTexture2;
			texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
			camera.targetTexture = null;
			RenderTexture.active = null;
			UnityEngine.Object.Destroy(renderTexture2);
			return texture2D;
		}

		public static Texture2D CreateTexture(int width, int height, Color color)
		{
			Color[] array = new Color[width * height];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = color;
			}
			Texture2D texture2D = new Texture2D(width, height);
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D CreateTexture(int width, int height, Color textureColor, RectOffset border, Color borderColor)
		{
			int num = width;
			width += border.left;
			width += border.right;
			Color[] array = new Color[width * (height + border.top + border.bottom)];
			for (int i = 0; i < array.Length; i++)
			{
				if (i < border.bottom * width)
				{
					array[i] = borderColor;
				}
				else if (i >= border.bottom * width + height * width)
				{
					array[i] = borderColor;
				}
				else if (i % width < border.left)
				{
					array[i] = borderColor;
				}
				else if (i % width >= border.left + num)
				{
					array[i] = borderColor;
				}
				else
				{
					array[i] = textureColor;
				}
			}
			Texture2D texture2D = new Texture2D(width, height + border.top + border.bottom);
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}
	}
}
