using Eto.Drawing;
using Eto.Forms;
using Eto.Forms.ThemedControls;

namespace NmlSpriteTool {
	public class ProgressBlocker : Dialog {
		
		private Label currentActivity;
		private ProgressBar progressBar;

		public ProgressBlocker(string progressCaption)
		{
			this.Title  = progressCaption;
			this.Width  = 400;
			this.Height = 100;

			StackLayout sl = new StackLayout();
			sl.Width   = this.Width;
			sl.Height  = this.Height;

			this.currentActivity = new Label() {Width = this.Width};
			this.progressBar     = new ProgressBar() {Width = this.Width};
		
			sl.Items.Add(new StackLayoutItem(this.currentActivity,false));
			sl.Items.Add(new StackLayoutItem(this.progressBar, false));
			sl.Items.Add(new StackLayoutItem(new Button(((sender, args) => this.Close())){Text = "cancel"}, HorizontalAlignment.Center));
			
			this.Content = sl;
		}

		public void PushProgress(string activityText, int cur, int max)
		{
			this.currentActivity.Text = activityText;
			this.progressBar.Value    = cur;
			this.progressBar.MaxValue = max;
		}
	}
}