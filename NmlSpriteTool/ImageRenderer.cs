using System;
using Eto.Drawing;
using Eto.Forms;
using ImageMagick;
using Color = System.Drawing.Color;

namespace NmlSpriteTool {
	public class ImageRenderer {
		private Graphics gfx;
		private Bitmap image;
		private RawImageData currentRawImageData;
		
		public readonly ImageView imageView;

		public int offsetX = 0;
		public int offsetY = 0;

		public ImageRenderer()
		{
			this.imageView = new ImageView();
			SetSize(new Size(100, 100));
		}
		
		public void SetImageAndRepaint(RawImageData rawImage)
		{
			currentRawImageData = rawImage;
			this.Repaint();
		}

		public void Repaint()
		{
			this.gfx.Clear();
			
			this.gfx.Flush();
		}

		public void SetSize(Size size)
		{
			if (this.image != null && !this.image.IsDisposed)
			{
				this.image.Dispose();
			}

			this.image = new Bitmap(size.Width, size.Width, PixelFormat.Format32bppRgb);
			this.gfx   = new Graphics(this.image);
			this.Repaint();
		}
	}
}