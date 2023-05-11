using Eto.Drawing;
using Eto.Forms;

namespace NmlSpriteTool {
	public class FrameRenderer {
		private Frame frame;
		private RawImageData rawImageData;
		private ImageView imageView;
		public ImageView ImageView => this.imageView;
		
		private const int ZOOM = 4;

		public FrameRenderer()
		{
			this.imageView = new ImageView();
		}

		public void SetFrameAndImage(Frame newFrame, RawImageData newRawImageData)
		{
			if (Equals(newFrame, Frame.EmptyFrame))
			{
				SetEmpty();
				return;
			}
			this.frame        = newFrame;
			this.rawImageData = newRawImageData;
			this.Repaint();
		}

		public void SetEmpty()
		{
			this.rawImageData = null;
			this.frame = Frame.EmptyFrame;
			this.PaintAsEmpty();
		}

		private void PaintAsEmpty()
		{
			Bitmap img;
			if (this.imageView.Image != null && this.imageView.Image.Width == this.frame.sizeX * ZOOM && this.imageView.Image.Height == this.frame.sizeY * ZOOM)
			{
				img = this.imageView.Image as Bitmap;
			}
			else
			{
				if (this.imageView.Image != null)
				{
					this.imageView.Image.Dispose();
				}
				img                  = new Bitmap(64, 64, PixelFormat.Format32bppRgb);
			}
			Graphics gfx = new Graphics(img);
			gfx.AntiAlias          = false;
			gfx.ImageInterpolation = ImageInterpolation.None;
			gfx.Clear();
			gfx.DrawText(new Font(FontFamilies.Monospace, 8,FontStyle.None,FontDecoration.None),Color.FromArgb(255,0,0,255),0,16,"no frame");
			gfx.Flush();
			this.imageView.Image = img;
		}

		private void Repaint()
		{
			Bitmap img;
			if (this.imageView.Image != null && this.imageView.Image.Width == this.frame.sizeX * ZOOM && this.imageView.Image.Height == this.frame.sizeY * ZOOM)
			{
				img = this.imageView.Image as Bitmap;
			}
			else
			{
				if (this.imageView.Image != null)
				{
					this.imageView.Image.Dispose();
				}
				img                  = new Bitmap(this.frame.sizeX * ZOOM, this.frame.sizeY * ZOOM, PixelFormat.Format32bppRgb);
			}
			Graphics gfx = new Graphics(img);
			gfx.AntiAlias          = false;
			gfx.ImageInterpolation = ImageInterpolation.None;
			gfx.Clear();
			for (int x = this.frame.posX; x <= (this.frame.sizeX / ZOOM + this.frame.posX); x++)
			{
				for (int y = this.frame.posY; y <= (this.frame.sizeY  / ZOOM +this.frame.posY); y++)
				{
					if (y > this.rawImageData.Height || y < 0)
					{
						continue;
					}
					if (x > this.rawImageData.Width || x < 0)
					{
						continue;
					}
					int color = this.rawImageData.GetColorOrBlack(x, y);
					gfx.FillRectangle(Color.FromArgb(color), (x - this.frame.posY) * ZOOM, (y - this.frame.posX) * ZOOM, ZOOM, ZOOM);
				}
			}
			gfx.Flush();
			this.imageView.Image = img;
		}
	}
}