using System;
using Eto.Forms;

namespace NmlSpriteTool {
	public partial class VehicleTab  {
		private ListBox vehicleList;
	
		private Control CreateVehicleListWithButtons()
		{
			StackLayout buttonsAndList = new StackLayout() { Orientation = Orientation.Vertical };

			StackLayout topButtonPanel = new StackLayout() { Orientation = Orientation.Horizontal };

			topButtonPanel.Items.Add(new StackLayoutItem(new Button() { Text = "New", Width    = 50, Command = new Command(this.NewVehicle) }, HorizontalAlignment.Left, true));
			topButtonPanel.Items.Add(new StackLayoutItem(new Button() { Text = "Copy", Width   = 50, Command = new Command(this.CopyVehicle) }, HorizontalAlignment.Center, true));
			topButtonPanel.Items.Add(new StackLayoutItem(new Button() { Text = "Delete", Width = 50, Command = new Command(this.DeleteVehicle) }, HorizontalAlignment.Right, true));
			topButtonPanel.Items.Add(new StackLayoutItem(new Button() { Text = "↑", Width      = 25, Command = new Command(this.OrderUp) }, HorizontalAlignment.Right, false));
			topButtonPanel.Items.Add(new StackLayoutItem(new Button() { Text = "↓", Width      = 25, Command = new Command(this.OrderDown) }, HorizontalAlignment.Right, false));

			buttonsAndList.Items.Add(new StackLayoutItem(topButtonPanel));

			vehicleList       = new ListBox();
			vehicleList.Width = 200;
			this.vehicleList.SelectedValueChanged += (sender, args) =>
			{
				this.SetVehicle((Vehicle)this.vehicleList.SelectedValue);
			};
			buttonsAndList.Items.Add(new StackLayoutItem(this.vehicleList, true));
			return buttonsAndList;
		}

		private void NewVehicle(object sender, EventArgs e)
		{
			this.vehicleList.Items.Add(new Vehicle("new Vehicle"));
			this.vehicleList.SelectedIndex = this.vehicleList.Items.Count - 1;
		}

		private void CopyVehicle(object sender, EventArgs e)
		{
			if (this.vehicleList.SelectedValue != null)
			{
				this.vehicleList.Items.Add(((Vehicle)this.vehicleList.SelectedValue).Copy());
				this.vehicleList.SelectedIndex = this.vehicleList.Items.Count - 1;
			}
		}

		private void DeleteVehicle(object sender, EventArgs e)
		{
			if (this.vehicleList.SelectedValue != null)
			{
				this.vehicleList.Items.Remove((Vehicle)this.vehicleList.SelectedValue);
			}
		}
		private void OrderUp(object sender, EventArgs e)
		{
			for (int i = 0; i < this.vehicleList.Items.Count; i++)
			{
				if (this.vehicleList.Items[i] == this.vehicleList.SelectedValue)
				{
					if (i == 0)
					{
						return;
					}
					Vehicle bfore = this.vehicleList.Items[i - 1] as Vehicle;
					this.vehicleList.Items[i - 1]  = (Vehicle)this.vehicleList.SelectedValue;
					this.vehicleList.Items[i]      = bfore;
					this.vehicleList.SelectedIndex = i - 1;
					return;
				}
			}
		}

		private void OrderDown(object sender, EventArgs e)
		{
			for (int i = 0; i < this.vehicleList.Items.Count; i++)
			{
				if (this.vehicleList.Items[i] == this.vehicleList.SelectedValue)
				{
					if (i >= this.vehicleList.Items.Count - 1)
					{
						return;
					}
					Vehicle after = this.vehicleList.Items[i + 1] as Vehicle;
					this.vehicleList.Items[i + 1]  = (Vehicle)this.vehicleList.SelectedValue;
					this.vehicleList.Items[i]      = after;
					this.vehicleList.SelectedIndex = i + 1;
					return;
				}
			}
		}
	}
}