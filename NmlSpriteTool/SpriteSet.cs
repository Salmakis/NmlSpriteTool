using System.Collections.Generic;

namespace NmlSpriteTool {
	//represents 8 frames for the 8 directions
	public class SpriteSet {
		public string Name { get; set; }
		public readonly Frame[] frames = new Frame[8];
	}

	public struct Frame {
		public static readonly Frame EmptyFrame = new Frame(-1, -1, -1, -1);

		public int posX;
		public int posY;
		public int sizeX;
		public int sizeY;

		public Frame(int x, int y, int w, int h)
		{
			this.posX  = x;
			this.posY  = y;
			this.sizeX = w;
			this.sizeY = h;
		}
	}
}