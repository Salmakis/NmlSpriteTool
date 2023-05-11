using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;

namespace NmlSpriteTool {
	public partial class VehicleTab {
		StackLayout genericEditor = new StackLayout() { Orientation = Orientation.Vertical };
		private TextBox genericEditorVehicleName;

		public Control CreateGenericEditor()
		{
			this.genericEditor = new StackLayout() { Orientation = Orientation.Vertical };

			GroupBox nameGroupBox = new GroupBox() { Text = "Name" };
			this.genericEditor.Items.Add(nameGroupBox);
			this.genericEditorVehicleName             =  new TextBox() { };
			this.genericEditorVehicleName.TextChanged += this.OnGenericEditorVehicleNameOnTextChanged;
			nameGroupBox.Content                      =  this.genericEditorVehicleName;
			genericEditor.Enabled                     =  false;
			return this.genericEditor;
		}
		
		//todo: use databinding stuff instead of this wonkyness
		private void OnGenericEditorVehicleNameOnTextChanged(object sender, EventArgs args)
		{
			this.genericEditorVehicleName.TextChanged -= OnGenericEditorVehicleNameOnTextChanged;
			if (this.selectedVehicle != null)
			{
				int caret = genericEditorVehicleName.CaretIndex;
				this.selectedVehicle.Text = genericEditorVehicleName.Text;
				IEnumerable<IListItem> items = this.vehicleList.Items.Select(x => x).ToList();
				object selected = this.vehicleList.SelectedValue;
				this.vehicleList.Items.Clear();
				this.vehicleList.Items.AddRange(items);
				this.vehicleList.SelectedValue      = selected;
				genericEditorVehicleName.CaretIndex = caret;
			}
			this.genericEditorVehicleName.TextChanged += OnGenericEditorVehicleNameOnTextChanged;
		}
	}
}