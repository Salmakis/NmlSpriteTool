using System.Runtime.CompilerServices;
using System.Threading;
using ImageMagick;

namespace NmlSpriteTool {
	public struct RawImageData {
		//either the color (ARGB) or the index if its an idexed image
		private int[,] pixelData;
		private int[] paletteColors;

		public readonly bool isIndexed;

		public int Width => this.pixelData.GetLength(0);
		public int Height => this.pixelData.GetLength(1);
		public int PaletteColorCount => this.paletteColors.Length;

		public RawImageData(MagickImage magickImage)
		{
			if (magickImage.ColorType == ColorType.Palette || magickImage.ColorType == ColorType.PaletteAlpha)
			{
				this.isIndexed     = true;
				this.paletteColors = new int[magickImage.ColormapSize];
				for (int i = 0; i < magickImage.ColormapSize; i++)
				{
					IMagickColor<byte> color = magickImage.GetColormapColor(i);
					this.paletteColors[i] = color.R | color.G << 8 | color.B << 16 | color.A << 24;
				}

				this.pixelData = new int[magickImage.Width, magickImage.Height];
				IUnsafePixelCollection<byte> pixelData = magickImage.GetPixelsUnsafe();
				for (int x = 0; x < magickImage.Width; x++)
				{
					for (int y = 0; y < magickImage.Height; y++)
					{
						this.pixelData[x, y] = pixelData.GetPixel(x, y).GetChannel(3);
					}
				}
			}
			else
			{
				//todo:
				this.pixelData     = new int[,] { };
				this.paletteColors = new int[] { };
				this.isIndexed     = false;
			}
		}
		
		public int GetPaletteColor(int id)
		{
			return this.paletteColors[id];
		}
	}
}