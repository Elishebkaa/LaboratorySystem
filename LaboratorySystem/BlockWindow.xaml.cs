using System;
using System.Windows;
using System.Windows.Threading;

namespace LaboratorySystem
{
    public partial class BlockWindow : Window
    {
        private readonly DispatcherTimer timer;
        private TimeSpan remainingTime;

        public BlockWindow(TimeSpan blockDuration)
        {
            InitializeComponent();
            remainingTime = blockDuration;
            UpdateTimerDisplay();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
            UpdateTimerDisplay();

            if (remainingTime <= TimeSpan.Zero)
            {
                timer.Stop();
                DialogResult = true;
                Close();
            }
        }

        private void UpdateTimerDisplay()
        {
            tbTimer.Text = remainingTime.Seconds.ToString();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            timer?.Stop();
        }
    }
}
