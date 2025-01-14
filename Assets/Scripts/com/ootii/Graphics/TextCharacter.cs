using com.ootii.Collections;
using UnityEngine;

namespace com.ootii.Graphics
{
	public class TextCharacter
	{
		public char Character;

		public Color[] Pixels;

		public int Width;

		public int Height;

		public int MinX;

		public int MinY;

		public int Advance;

		private static ObjectPool<TextCharacter> sPool = new ObjectPool<TextCharacter>(20, 5);

		public static int Length => sPool.Length;

		public static TextCharacter Allocate()
		{
			return sPool.Allocate();
		}

		public static void Release(TextCharacter rInstance)
		{
			if (rInstance != null)
			{
				rInstance.Character = '\0';
				rInstance.Pixels = null;
				rInstance.Width = 0;
				rInstance.Height = 0;
				sPool.Release(rInstance);
			}
		}
	}
}
