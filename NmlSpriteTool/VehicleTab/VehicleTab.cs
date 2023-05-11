using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Eto.Forms;

namespace NmlSpriteTool {
	public partial class VehicleTab : DynamicLayout {
		private Dictionary<string, RawImageData> rawImageDatas;

		public VehicleTab(Dictionary<string, RawImageData> rawImageDatas)
		{
			this.rawImageDatas = rawImageDatas;
			this.BeginHorizontal();
			this.Add(this.CreateVehicleListWithButtons());
			
			TabControl vehicleSubTabs = new TabControl();
			vehicleSubTabs.Pages.Add(new TabPage(this.CreateGenericEditor()){Text = "Generic"});
			vehicleSubTabs.Pages.Add(new TabPage(this.CraeteSpriteSetEditor()){Text = "Sprites"});
			this.Add(vehicleSubTabs,true, true);
			this.EndHorizontal();
		}

		private void SetVehicle(Vehicle vehicle)
		{
			this.spriteSetEditor.Enabled         = vehicle != null;
			this.genericEditor.Enabled           = vehicle != null;
			this.selectedVehicle                 = vehicle;
			this.imageNameLabel.Text             = Path.GetFileName(vehicle?.ImageFileName ?? "");
			this.genericEditorVehicleName.Text   = vehicle?.Text ?? "";
			this.autoDetectSpritesButton.Enabled = vehicle?.ImageFileName != null;
			this.spriteSetList.Items.Clear();
			if (vehicle == null || vehicle.spriteSets.Length == 0)
			{
				this.SelectSpriteSet(-1);
			}
			else
			{
				foreach (SpriteSet spriteSet in vehicle.spriteSets)
				{
					this.spriteSetList.Items.Add(spriteSet.Name);
				}
				if (vehicle.spriteSets.Length >= this.selectedSpriteSet)
				{
					this.SelectSpriteSet(this.selectedSpriteSet);
				}
				else
				{
					this.SelectSpriteSet(0);
				}
			}
		}
	}
}