using Eto.Drawing;
using ImageMagick;

namespace NmlSpriteTool {
	public class RawImageData {
		//either the color (ARGB) or the index if its an idexed image
		private int[,] pixelData;
		private int[] paletteColors;

		public enum ImageType {
			Invalid = 0,
			Indexed = 1,
			TrueColor = 2
		}

		public readonly ImageType imageType;

		public int Width => this.pixelData.GetLength(0);
		public int Height => this.pixelData.GetLength(1);
		public int PaletteColorCount => this.paletteColors.Length;
		public object Size => new Size(Width, Height);

		public static RawImageData CreateInvalid()
		{
			return new RawImageData();
		}

		private RawImageData()
		{
			this.imageType     = ImageType.Invalid;
			this.pixelData     = new int[0, 0];
			this.paletteColors = new int[0];
		}
		
		public RawImageData(MagickImage magickImage)
		{
			if (magickImage.ColorType == ColorType.Palette || magickImage.ColorType == ColorType.PaletteAlpha)
			{
				imageType          = ImageType.Indexed;
				this.paletteColors = new int[magickImage.ColormapSize];
				for (int i = 0; i < magickImage.ColormapSize; i++)
				{
					IMagickColor<byte> color = magickImage.GetColormapColor(i);
					this.paletteColors[i] = color.B | color.G << 8 | color.R << 16 | color.A << 24;
				}

				this.pixelData = new int[magickImage.Width, magickImage.Height];
				IUnsafePixelCollection<byte> pixelCollection = magickImage.GetPixelsUnsafe();
				for (int x = 0; x < magickImage.Width; x++)
				{
					for (int y = 0; y < magickImage.Height; y++)
					{
						this.pixelData[x, y] = pixelCollection.GetPixel(x, y).GetChannel(3);
					}
				}
			}
			else
			{
				this.imageType     = ImageType.TrueColor;
				this.pixelData     = new int[magickImage.Width, magickImage.Height];
				this.paletteColors = null;
				IUnsafePixelCollection<byte> pixelCollection = magickImage.GetPixelsUnsafe();
				for (int x = 0; x < magickImage.Width; x++)
				{
					for (int y = 0; y < magickImage.Height; y++)
					{
						IPixel<byte> pixel = pixelCollection.GetPixel(x, y);
						this.pixelData[x, y] = pixel.GetChannel(2) | pixel.GetChannel(1) << 8 | pixel.GetChannel(0) << 16 | 255 << 24;;
					}
				}
			}
		}
		
		public int GetPaletteColor(int id)
		{
			return this.paletteColors[id];
		}

		public int GetColorOrBlack(int x, int y)
		{
			if (this.imageType == ImageType.Invalid)
			{
				return 0;
			}
			if (x < 0 || y < 0)
			{
				return 255 << 24;
			}
			if (x >= this.Width || y >= this.Height)
			{
				return 255 << 24;
			}
			if (this.imageType == ImageType.Indexed)
			{
				return this.paletteColors[this.pixelData[x, y]];
			}
			else
			{
				return this.pixelData[x, y];
			}
		}
	}
}