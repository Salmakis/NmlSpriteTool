using Eto.Forms;

namespace NmlSpriteTool {
	public class Vehicle : IListItem {
		public SpriteSet[] spriteSets { get; set; }
		public string ImageFileName { get; set; }

		public Vehicle(string name)
		{
			this.name       = name;
			//debug
			this.spriteSets         = new SpriteSet[1];
			this.spriteSets[0]      = new SpriteSet();
			this.spriteSets[0].Name = "default";
			Frame[] frames = this.spriteSets[0].frames;
			frames[0] = new Frame(0,0,16,16);
			frames[1] = new Frame(16,0,16,16);
			frames[2] = new Frame(32,0,16,16);
			frames[3] = new Frame(48,0,16,16);
			frames[4] = new Frame(64,0,16,16);
			frames[5] = new Frame(96,0,16,16);
			frames[6] = new Frame(128,0,16,16);
			frames[7] = new Frame(144,0,16,16);
		}

		public int GetSpriteSetNumberByName(string name)
		{
			for (int i = 0; i < spriteSets.Length; i++)
			{
				if (spriteSets[i].Name.Equals(name))
				{
					return i;
				}
			}
			return-1;
		}
		
		private string name;

		public string Text
		{
			get => this.name;
			set => this.name = value;
		}

		public string Key => this.name;

		public Vehicle Copy()
		{
			Vehicle copy = new Vehicle($"copy of {this.name}");
			copy.ImageFileName = this.ImageFileName;
			return copy;
		}
	}
}