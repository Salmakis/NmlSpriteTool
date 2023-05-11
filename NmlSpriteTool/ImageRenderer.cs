using Eto.Drawing;
using Eto.Forms;
using Eto.Forms.ThemedControls;

namespace NmlSpriteTool {
	public class ImageRenderer {
		private Graphics gfx;
		private Bitmap image;
		private RawImageData currentRawImageData;

		private int zoom = 4;

		public readonly ImageView imageView;

		private int offsetX = 20;
		private int offsetY = 10;

		private int width = 0;
		private int height = 0;

		private PointF dragStartLocation = PointF.Empty;

		public ImageRenderer()
		{
			this.imageView             =  new ImageView();
			this.imageView.SizeChanged += (sender, args) => { SetSize(new Size(this.imageView.Size)); };

			this.imageView.MouseMove += (sender, args) =>
			{
				if (args.Buttons == MouseButtons.None)
				{
					this.dragStartLocation = PointF.Empty;
				}
				else
				{
					if (this.dragStartLocation == PointF.Empty)
					{
						this.dragStartLocation = args.Location;
					}
					else
					{
						PointF delta = args.Location - this.dragStartLocation;
						System.Diagnostics.Debug.Write("d:" + delta + "\n");
						this.MoveOffsetAndRepaint((int)delta.X, (int)delta.Y);
						this.dragStartLocation = args.Location;
					}
				}
			};

			this.imageView.MouseWheel += (sender, args) =>
			{
				System.Diagnostics.Debug.Write("d:" + args.Delta.Height + "\n");
				if (args.Delta.Height < 0)
				{
					this.ZoomInAndRepaint();
				}
				else if (args.Delta.Height > 0)
				{
					this.ZoomOutAndRepaint();
				}
			};
		}

		public void SetImageAndRepaint(RawImageData rawImage)
		{
			currentRawImageData = rawImage;
			this.offsetY        = 0;
			this.offsetX        = 0;
			this.Repaint();
		}

		public virtual void ZoomInAndRepaint()
		{
			if (this.zoom > 32)
			{
				return;
			}
			this.zoom++;
			this.Repaint();
		}

		public virtual void ZoomOutAndRepaint()
		{
			if (this.zoom < 2)
			{
				return;
			}
			this.zoom--;
			this.Repaint();
		}

		public virtual void MoveOffsetAndRepaint(int x, int y)
		{
			this.offsetY -= y / this.zoom;
			this.offsetX -= x / this.zoom;
			this.Repaint();
		}

		public void Repaint()
		{
			this.gfx.Clear();
			if (currentRawImageData != null)
			{
				for (int x = this.offsetX; x <= (this.width / this.zoom + this.offsetX); x++)
				{
					for (int y = this.offsetY; y <= (this.height  / this.zoom  + this.offsetY); y++)
					{
						if (y > this.currentRawImageData.Height || y < 0)
						{
							continue;
						}
						if (x > this.currentRawImageData.Width || x < 0)
						{
							continue;
						}
						int color = this.currentRawImageData.GetColorOrBlack(x, y);
						this.gfx.FillRectangle(Color.FromArgb(color), (x - this.offsetX) * zoom, (y - this.offsetY) * zoom, zoom, zoom);
//						this.gfx.FillRectangle(Color.FromArgb(0,0,0,255), x, y, 1, 1);
					}
				}
			}
			this.gfx.Flush();
			this.imageView.Image = this.image;
		}

		public void SetSize(Size size)
		{
			this.width  = size.Width / 2;
			this.height = size.Height / 2;
			if (this.image != null && !this.image.IsDisposed)
			{
				this.image.Dispose();
			}
			this.image                  = new Bitmap(this.width, this.height, PixelFormat.Format32bppRgb);
			this.gfx                    = new Graphics(this.image);
			this.gfx.ImageInterpolation = ImageInterpolation.None;
			this.gfx.AntiAlias          = false;
			this.Repaint();
		}
	}
}