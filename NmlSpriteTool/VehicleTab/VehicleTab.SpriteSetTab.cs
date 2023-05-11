using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Eto.Drawing;
using Eto.Forms;

namespace NmlSpriteTool {
	public partial class VehicleTab {
		private Vehicle selectedVehicle;
		private StackLayout spriteSetEditor;
		private Label imageNameLabel;
		private ListBox spriteSetList;
		private Button autoDetectSpritesButton;
		private FrameRenderer[] frameSmallPreviews;
		private int selectedSpriteSet = -1;

		public Control CraeteSpriteSetEditor()
		{
			spriteSetEditor = new StackLayout() { Orientation = Orientation.Vertical };

			StackLayout topArea = new StackLayout() { Orientation     = Orientation.Horizontal };
			StackLayout topLeftArea = new StackLayout() { Orientation = Orientation.Vertical };

			//top left area
			topLeftArea.Items.Add(new StackLayoutItem(this.CreateImageSelection()));
			topArea.Items.Add(new StackLayoutItem(topLeftArea));

			//top right area - spriteSet selector
			topArea.Items.Add(new StackLayoutItem(CreateSpriteList()));

			spriteSetEditor.Items.Add(new StackLayoutItem(topArea));
			spriteSetEditor.Items.Add(new StackLayoutItem(this.Create8FrameBox()));

			GroupBox frameEditor = new GroupBox() { Text = "Edit Selected Frame" };
			spriteSetEditor.Items.Add(new StackLayoutItem(frameEditor, true));
			spriteSetEditor.Enabled = false;
			return spriteSetEditor;
		}

		private GroupBox Create8FrameBox()
		{
			GroupBox framesSelection = new GroupBox() { Text         = "Frames of selected Spriteset" };
			StackLayout frameViews = new StackLayout() { Orientation = Orientation.Horizontal };
			frameSmallPreviews = new FrameRenderer[8];
			for (int i = 0; i < 8; i++)
			{
				StackLayout oneFrameView = new StackLayout() { Orientation = Orientation.Vertical };
				ImageView directionView = new ImageView() { Image                   = Bitmap.FromResource($"NmlSpriteTool.Images.frame{i + 1}.png", Assembly.GetCallingAssembly()) };
				oneFrameView.Items.Add(directionView);
				this.frameSmallPreviews[i] = new FrameRenderer();
				this.frameSmallPreviews[i].SetEmpty();
				oneFrameView.Items.Add(this.frameSmallPreviews[i].ImageView);
				frameViews.Items.Add(oneFrameView);
			}
			framesSelection.Content = frameViews;
			return framesSelection;
		}
		
		private void SelectSpriteSet(int spriteSetNumber)
		{
			if (spriteSetNumber < 0 || this.selectedVehicle.ImageFileName == null || !this.rawImageDatas.ContainsKey(this.selectedVehicle.ImageFileName))
			{
				for (int i = 0; i < 8; i++)
				{
					this.frameSmallPreviews[i].SetEmpty();
				}
			}
			else
			{
				for (int i = 0; i < 8; i++)
				{
					this.frameSmallPreviews[i].SetFrameAndImage(this.selectedVehicle.spriteSets[spriteSetNumber].frames[i], this.rawImageDatas[this.selectedVehicle.ImageFileName]);
				}
			}
		}

		private GroupBox CreateSpriteList()
		{
			StackLayout spriteSetsLayout = new StackLayout() { Orientation = Orientation.Horizontal };

			//the area left from the spritelist with actions
			GroupBox spriteActionsGroup = new GroupBox() { Text        = "Actions" };
			StackLayout buttonLayout = new StackLayout() { Orientation = Orientation.Vertical };
			autoDetectSpritesButton = new Button() { Text = "Auto Detect", Enabled = false };
			buttonLayout.Items.Add(this.autoDetectSpritesButton);
			buttonLayout.Items.Add(new Button() { Text = "Delete", Enabled = false });
			buttonLayout.Items.Add(new Button() { Text = "Add", Enabled    = false });
			spriteActionsGroup.Content = buttonLayout;
			spriteSetsLayout.Items.Add(spriteActionsGroup);

			//the list box with spritesets
			GroupBox spriteSetListGroup = new GroupBox() { Text = "Spritessets of this Vehicle" };
			spriteSetListGroup.Content              =  spriteSetsLayout;
			this.spriteSetList                      =  new ListBox();
			this.spriteSetList.SelectedIndexChanged += (sender, args) => { this.SelectSpriteSet(this.spriteSetList.SelectedIndex);};
			spriteSetsLayout.Items.Add(spriteSetList);

			//the area to view/edit proprties of the selected spriteset
			GroupBox spriteSetPropertyBox = new GroupBox() { Text           = "Properties" };
			StackLayout propertyBoxLayout = new StackLayout() { Orientation = Orientation.Vertical };
			spriteSetPropertyBox.Content = propertyBoxLayout;
			TextBox spriteNameBox = new TextBox();
			propertyBoxLayout.Items.Add(spriteNameBox);
			spriteSetsLayout.Items.Add(spriteSetPropertyBox);
			return spriteSetListGroup;
		}

		private GroupBox CreateImageSelection()
		{
			GroupBox imageSelection = new GroupBox() { Text                   = "Image for this Vehicle" };
			StackLayout imageSelectorLayout = new StackLayout() { Orientation = Orientation.Horizontal };
			this.imageNameLabel       = new Label() { Text = "" };
			this.imageNameLabel.Width = 200;
			this.imageNameLabel.Wrap  = WrapMode.None;
			imageSelectorLayout.Items.Add(new Button() { Text = "Select Image", Command = new Command(this.SelectImage) });
			imageSelectorLayout.Items.Add(this.imageNameLabel);
			imageSelection.Content = imageSelectorLayout;
			return imageSelection;
		}

		private void SelectImage(object sender, EventArgs args)
		{
			Dialog form = new Dialog();
			ListBox imageListBox = new ListBox();
			foreach (string imageName in this.rawImageDatas.Where(pair => pair.Value.imageType == RawImageData.ImageType.Indexed).Select(pair => pair.Key))
			{
				imageListBox.Items.Add(Path.GetFileName(imageName), imageName);
			}
			form.Content = imageListBox;
			imageListBox.SelectedValueChanged += (o, eventArgs) =>
			{
				selectedVehicle.ImageFileName = imageListBox.SelectedKey;
				form.Close();
			};
			form.ShowModal();
			SetVehicle(this.selectedVehicle);
		}
	}
}