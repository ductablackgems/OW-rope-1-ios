using com.ootii.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Graphics
{
	public class TextFont
	{
		public Font Font;

		public Texture2D Texture;

		public int MinX;

		public int MaxX;

		public int MinY;

		public int MaxY;

		public Dictionary<char, TextCharacter> Characters = new Dictionary<char, TextCharacter>();

		private static ObjectPool<TextFont> sPool = new ObjectPool<TextFont>(20, 5);

		public static int Length => sPool.Length;

		public static TextFont Allocate()
		{
			return sPool.Allocate();
		}

		public static void Release(TextFont rInstance)
		{
			if (rInstance != null)
			{
				if (rInstance.Texture != null)
				{
					UnityEngine.Object.Destroy(rInstance.Texture);
					rInstance.Texture = null;
				}
				foreach (TextCharacter value in rInstance.Characters.Values)
				{
					TextCharacter.Release(value);
				}
				rInstance.Font = null;
				rInstance.Characters.Clear();
				rInstance.MinX = 0;
				rInstance.MaxX = 0;
				rInstance.MinY = 0;
				rInstance.MaxY = 0;
				sPool.Release(rInstance);
			}
		}
	}
}
