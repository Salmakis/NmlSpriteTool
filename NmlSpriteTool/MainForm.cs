using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using ImageMagick;

namespace NmlSpriteTool {
	public partial class MainForm : Form {
		private string workDirectory = null;

		private static readonly string txtSelectProjectFolder = "Select Project Folder";
		private static readonly string txtSelectADirectory = "<select a directory>";
		private static readonly string txtSelectFolderWithImages = "Select Folder with images";
		private static readonly string txtReload = "Reload";
		
		private ImageRenderer imageRenderer;

		private ListBox imageFileList;
		private ImageInfoPanel imageInfoPanel;

		private static Dictionary<string, RawImageData> imageMap = new Dictionary<string, RawImageData>();

		public MainForm()
		{
			Title       = "SpriteHelper";
			MinimumSize = new Size(512, 480);

	
			this.imageInfoPanel  = new ImageInfoPanel();

			this.imageFileList  = new ListBox();
			
			imageFileList.Width = 200;
			
			this.imageFileList.SelectedKeyChanged += OnImageFileListOnSelectedKeyChanged;
			this.imageInfoPanel.ClearImage();

			DynamicLayout layout = new DynamicLayout();
			// @formatter:off
			layout.BeginVertical();
				layout.BeginGroup(null);
					layout.Add(CraeteProjectFolderSelection());
				layout.EndGroup();
				layout.BeginGroup("Images");
					layout.BeginHorizontal();
						layout.Add(imageFileList);
						layout.Add(imageInfoPanel);
					layout.EndHorizontal();
				layout.EndGroup();
			layout.EndVertical();
			// @formatter:on
			this.Content = layout;
		}
		private void OnImageFileListOnSelectedKeyChanged(object sender, EventArgs args)
		{
			string fullfilePath = this.workDirectory + "\\" + this.imageFileList.SelectedKey;
			if (!File.Exists(fullfilePath))
			{
				return;
			}
			RawImageData rawImage;
			if (!imageMap.ContainsKey(fullfilePath))
			{
				try
				{
					rawImage = new RawImageData(new MagickImage(new FileInfo(fullfilePath)));
				}
				catch (Exception e)
				{
					this.imageInfoPanel.ClearImage();
					MessageBox.Show("Image could not be loaded\n" + e.Message);
					return;
				}
				imageMap.Add(fullfilePath, rawImage);
			}
			else
			{
				rawImage = imageMap[fullfilePath];
			}
			this.imageInfoPanel.SetImage(rawImage);
		}
		private void LoadImagesFromWorkDir()
		{
			this.imageFileList.Items.Clear();
			if (this.workDirectory == null)
			{
				return;
			}
			if (!Directory.Exists(this.workDirectory))
			{
				return;
			}
			foreach (string file in Directory.GetFiles(this.workDirectory, "*.png", SearchOption.AllDirectories))
			{
				this.imageFileList.Items.Add(file.Replace(this.workDirectory + "\\", ""));	
			}
		}

		private Control CraeteProjectFolderSelection()
		{
			StackLayout sl = new StackLayout()
			{
				Orientation = Orientation.Horizontal
			};

			Button selectFolderButton = new Button()
			{
				Text = txtSelectProjectFolder
			};
			Label folderLabel = new Label();
			folderLabel.Text = txtSelectADirectory;

			selectFolderButton.Command = new Command(((sender, args) =>
			{
				SelectFolderDialog sfd = new SelectFolderDialog
				{
					Title = txtSelectFolderWithImages
				};
				DialogResult result = sfd.ShowDialog(this);
				if (result == DialogResult.Ok)
				{
					this.workDirectory = sfd.Directory;
					this.LoadImagesFromWorkDir();
					folderLabel.Text = this.workDirectory ?? txtSelectADirectory;
				}
			}));

			var reloadButton = new Button()
			{
				Text    = txtReload,
				Command = new Command(((sender, args) => this.LoadImagesFromWorkDir()))
			};

			sl.Items.Add(selectFolderButton);
			sl.Items.Add(new StackLayoutItem(folderLabel, HorizontalAlignment.Center, true));
			sl.Items.Add(new StackLayoutItem(reloadButton, HorizontalAlignment.Right));
			return sl;
		}
	}
}