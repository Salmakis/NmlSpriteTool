using Eto.Drawing;
using Eto.Forms;
using ImageMagick;

namespace NmlSpriteTool {
	public class ImageInfoPanel : DynamicLayout {
		private Label imageInfo = new Label();
		private ImageView paletteImageView;
		private ImageView currentImageView;
		private DynamicGroup palletteGroup;

		public ImageInfoPanel()
		{
			this.paletteImageView = new ImageView();
			this.currentImageView = new ImageView();
			// @formatter:off
			BeginVertical();
				BeginGroup("Meta");
						Add(imageInfo);
				EndGroup();
				palletteGroup = BeginGroup("Colors");
					Add(paletteImageView);
				EndGroup();
				palletteGroup = BeginGroup("ImageGroup");
					Add(currentImageView);
				EndGroup();
			EndVertical();
			// @formatter:on
		}

		public void ClearImage()
		{
			imageInfo.Text           = "no Image selected";
			palletteGroup.Visible    = false;
			paletteImageView.Visible = false;
			currentImageView.Visible = false;
		}

		public void SetImage(MagickImage image)
		{
			this.imageInfo.Text         = $"Size: {image.Width},{image.Height} colorMode:{image.ColorType.ToString()}";
			this.currentImageView.Image = new Bitmap(image.FileName);
			if (image.ColorType == ColorType.Palette || image.ColorType == ColorType.PaletteAlpha)
			{
				this.imageInfo.Text += $" colors:{image.ColormapSize}";
				this.RenderPalette(image);
				paletteImageView.Visible   = true;
				this.palletteGroup.Visible = true;
				currentImageView.Visible   = true;
			}
			else
			{
				this.imageInfo.Text      += $" colors:not indexed!";
				currentImageView.Visible =  true;
				palletteGroup.Visible    =  false;
				paletteImageView.Visible =  false;
			}
		}

		private void RenderImage(MagickImage image)
		{
//			Graphics gfx = new Graphics(this.paletteImage);
//			gfx.Clear(Color.FromArgb(0, 0, 0, 0));
//			int y = 0;
//			int x = 0;
//			this.paletteImageView.Image = paletteImage;
		}

		private void RenderPalette(MagickImage image, int size = 4)
		{
			Bitmap paletteImage = new Bitmap(16 * 4, 16 * 4, PixelFormat.Format32bppRgb);
			Graphics gfx = new Graphics(paletteImage);
			gfx.Clear(Color.FromArgb(0, 0, 0, 0));
			int y = 0;
			int x = 0;
			for (int i = 1; i < image.ColormapSize; i++)
			{
				IMagickColor<byte> color = image.GetColormapColor(i);
				x += size;
				if (i % 16 == 0)
				{
					x =  0;
					y += size;
				}
				gfx.FillRectangle(Color.FromPremultipliedArgb(color.R, color.G, color.B, 255), x, y, size, size);
			}
			gfx.Flush();

			this.paletteImageView.Image = paletteImage;
		}
	}
}