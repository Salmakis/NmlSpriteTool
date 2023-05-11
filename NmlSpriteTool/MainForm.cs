using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
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
		protected VehicleTab VehicleTab;

		private readonly Dictionary<string, RawImageData> imageMap = new Dictionary<string, RawImageData>();

		public MainForm()
		{
			Title       = "SpriteHelper";
			MinimumSize = new Size(512, 480);

			this.imageInfoPanel = new ImageInfoPanel();

			this.imageFileList  = new ListBox();
			imageFileList.Width = 300;

			this.imageFileList.SelectedKeyChanged += OnImageFileListOnSelectedKeyChanged;
			this.imageInfoPanel.ClearImage();

			this.VehicleTab = new VehicleTab(imageMap);

			TabControl tabControl = new TabControl();
			DynamicLayout imageInfoTabContent = new DynamicLayout();
			imageInfoTabContent.BeginHorizontal();
			imageInfoTabContent.Add(imageFileList);
			imageInfoTabContent.Add(imageInfoPanel);
			imageInfoTabContent.EndHorizontal();

			tabControl.Pages.Add(new TabPage(imageInfoTabContent) { Text = "Images", });
			tabControl.Pages.Add(new TabPage(this.VehicleTab) {Text       = "Vehicles"});

			DynamicLayout layout = new DynamicLayout();
			layout.BeginVertical();
			layout.BeginGroup(null);
			layout.Add(CraeteProjectFolderSelection());
			layout.EndGroup();
			layout.BeginHorizontal();
			layout.Add(tabControl);
			layout.EndHorizontal();
			layout.EndVertical();
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
			rawImage = imageMap[fullfilePath];
			this.imageInfoPanel.SetImage(rawImage);
		}
		
		private void LoadImagesFromWorkDir()
		{
			this.imageFileList.Items.Clear();
			this.imageMap.Clear();
			imageFileList.Enabled = false;
			Application.Instance.InvokeAsync(DoLoadImageFiles);
			imageFileList.Enabled = true;
		}

		[STAThread]
		private async void DoLoadImageFiles()
		{
			
			if (this.workDirectory == null)
			{
				return;
			}
			if (!Directory.Exists(this.workDirectory))
			{
				return;
			}

			ProgressBlocker blocker = new ProgressBlocker("Loading Images");
			bool canceled = false;
			blocker.Owner = this;
			blocker.Closed += (sender, args) =>
			{
				canceled = true;
			};
			blocker.ShowModalAsync();
			int processed = 0;
			var files = Directory.GetFiles(this.workDirectory, "*.png", SearchOption.AllDirectories);
			foreach (string file in files)
			{
				blocker.PushProgress(file.Replace(this.workDirectory + "\\", ""), processed, files.Length);
				await Task.Run(() =>
				{
					RawImageData rawImage;
					try
					{
						rawImage = new RawImageData(new MagickImage(new FileInfo(file)));
					}
					catch (Exception e)
					{
						rawImage = RawImageData.CreateInvalid();
					}
					imageMap.Add(file, rawImage);
				});
				processed++;
				this.imageFileList.Items.Add(file.Replace(this.workDirectory + "\\", ""));
				if (canceled)
				{
					return;
				}
			}
			blocker.Close();
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