using System;
using System.Collections.Generic;

namespace FluffyUnderware.Curvy.Utils
{
	[Serializable]
	internal class EdgeLoop
	{
		public int[] vertexIndex;

		public int vertexCount => vertexIndex.Length - 1;

		public int TriIndexCount => vertexCount * 6;

		public EdgeLoop(List<int> verts)
		{
			vertexIndex = verts.ToArray();
		}

		public void ShiftIndices(int by)
		{
			for (int i = 0; i < vertexIndex.Length; i++)
			{
				vertexIndex[i] += by;
			}
		}

		public override string ToString()
		{
			string text = "";
			int[] array = vertexIndex;
			for (int i = 0; i < array.Length; i++)
			{
				int num = array[i];
				text = text + num.ToString() + ", ";
			}
			return text;
		}
	}
}
