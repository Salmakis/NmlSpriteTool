using System;
using Eto.Drawing;
using Eto.Forms;
using ImageMagick;

namespace NmlSpriteTool {
	public class ImageInfoPanel : DynamicLayout {
		private Label imageInfo = new Label();
		private ImageView paletteImageView;
		private ImageRenderer imageRenderer;
		private readonly DynamicGroup palletteGroup;
		private readonly DynamicGroup imageGroup;

		public ImageInfoPanel()
		{
			this.paletteImageView = new ImageView();
			this.imageRenderer    = new ImageRenderer();
			// @formatter:off
			BeginVertical();
				BeginGroup("Meta");
						Add(imageInfo);
				EndGroup();
					palletteGroup = BeginGroup("Colors");
					Add(paletteImageView);
				EndGroup();
					imageGroup = BeginGroup("Image Preview");
					Add(imageRenderer.imageView,true, true);
				EndGroup();
			EndVertical();
			// @formatter:on
		}

		public void ClearImage()
		{
			imageInfo.Text           = "no Image selected";
			palletteGroup.Visible    = false;
			paletteImageView.Visible = false;
		}

		public void SetImage(RawImageData rawImage)
		{
			switch (rawImage.imageType)
			{
				case RawImageData.ImageType.Invalid:
					this.imageInfo.Text      = $"Invalid image / wrong format / not readable";
					palletteGroup.Visible    = false;
					paletteImageView.Visible = false;
					this.imageRenderer.SetImageAndRepaint(rawImage);
					break;
				case RawImageData.ImageType.Indexed:
					this.imageInfo.Text =  $"Size: {rawImage.Width},{rawImage.Height}";
					this.imageInfo.Text += $" colors:{rawImage.PaletteColorCount.ToString()}";
					this.RenderPalette(rawImage);
					this.imageRenderer.SetImageAndRepaint(rawImage);
					paletteImageView.Visible   = true;
					this.palletteGroup.Visible = true;
					break;
				case RawImageData.ImageType.TrueColor:
					this.imageInfo.Text      =  $"Size: {rawImage.Width},{rawImage.Height}";
					this.imageInfo.Text      += $" colors:not indexed!";
					palletteGroup.Visible    =  false;
					paletteImageView.Visible =  false;
					this.imageRenderer.SetImageAndRepaint(rawImage);
					break;
				default:
					throw new ArgumentOutOfRangeException();
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

		private void RenderPalette(RawImageData rawImage, int size = 8)
		{
			Bitmap paletteImage = new Bitmap(16 * size, 16 * size, PixelFormat.Format32bppRgb);
			Graphics gfx = new Graphics(paletteImage);
			gfx.Clear(Color.FromArgb(0, 0, 0, 0));
			int y = 0;
			int x = 0;
			for (int i = 1; i < rawImage.PaletteColorCount; i++)
			{
				int color = rawImage.GetPaletteColor(i);
				x += size;
				if (i % 16 == 0)
				{
					x =  0;
					y += size;
				}
				gfx.FillRectangle(Color.FromArgb(color), x, y, size, size);
			}
			gfx.Flush();

			this.paletteImageView.Image = paletteImage;
		}
	}
}