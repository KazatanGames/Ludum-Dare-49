namespace KazatanGames.Framework
{
	// Only works on ARGB32, RGB24 and Alpha8 textures that are marked readable

	using UnityEngine;

	public class TextureScale
	{
		public static Color[] BilinearScaleDirect(Color[] inputs, int oldWidth, int oldHeight, int width, int height)
		{
			if (oldWidth == width && oldHeight == height) return inputs;

			float rX = 1.0f / ((float)width / oldWidth);
			float rY = 1.0f / ((float)height / oldHeight);

			Color[] results = new Color[width * height];
			for (int y = 0; y < height; y++)
			{
				int yFloor = (int)Mathf.Floor(y * rY);
				int y1 = yFloor * oldWidth;
				int y2 = (yFloor + 1) * oldHeight;
				int yw = y * width;

				for (int x = 0; x < width; x++)
				{
					int xFloor = (int)Mathf.Floor(x * rX);
					float xLerp = x * rX - xFloor;
					results[yw + x] = ColorLerpUnclamped(
						ColorLerpUnclamped(inputs[y1 + xFloor], inputs[y1 + xFloor + 1], xLerp),
						ColorLerpUnclamped(inputs[y2 + xFloor], inputs[y2 + xFloor + 1], xLerp),
						y * rY - yFloor
					);
				}
			}
			return results;
		}

		private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
		{
			return new Color(c1.r + (c2.r - c1.r) * value,
							  c1.g + (c2.g - c1.g) * value,
							  c1.b + (c2.b - c1.b) * value,
							  c1.a + (c2.a - c1.a) * value);
		}
	}
}