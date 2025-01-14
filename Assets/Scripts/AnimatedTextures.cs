using UnityEngine;

public class AnimatedTextures : MonoBehaviour
{
	public int colCount = 4;

	public int rowCount = 4;

	public int rowNumber;

	public int colNumber;

	public int totalCells = 4;

	public int fps = 10;

	private Vector2 offset;

	private void Update()
	{
		SetSpriteAnimation(colCount, rowCount, rowNumber, colNumber, totalCells, fps);
	}

	private void SetSpriteAnimation(int colCount, int rowCount, int rowNumber, int colNumber, int totalCells, int fps)
	{
		int num = (int)(Time.time * (float)fps) % totalCells;
		float x = 1f / (float)colCount;
		float y = 1f / (float)rowCount;
		Vector2 vector = new Vector2(x, y);
		int num2 = num % colCount;
		int num3 = num / colCount;
		float x2 = (float)(num2 + colNumber) * vector.x;
		float y2 = 1f - vector.y - (float)(num3 + rowNumber) * vector.y;
		Vector2 value = new Vector2(x2, y2);
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", value);
		GetComponent<Renderer>().material.SetTextureScale("_MainTex", vector);
	}
}
