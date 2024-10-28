using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using NHotkey;
using NHotkey.WindowsForms;

namespace AUTOCLICKING
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vkey);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;
        private const int MOD_NOREPEAT = 0x4000;

        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;

        const int WM_KEYDOWN = 0x0100;
        const int WM_MBUTTONDOWN = 0x0207;

        public bool isClicking = false;
        private System.Windows.Forms.Timer clickTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer statusTimer = new System.Windows.Forms.Timer();
        public Form1()
        {
            InitializeComponent();

            HotkeyManager.Current.AddOrReplace("ToggleAutoClick", Keys.Divide, OnHotkeyPressed);

            Status();


            clickTimer = new System.Windows.Forms.Timer();
            clickTimer.Tick += clickTimer_Tick;

            statusTimer = new System.Windows.Forms.Timer();
            statusTimer.Interval = 100;
            statusTimer.Tick += statusTimer_Tick;
            statusTimer.Start();
        }

        private void Status()
        {
            if (isClicking)
            {
                label4.Text = "activated";
            }
            else
            {
                label4.Text = "deactivated";
            }
        }

        private void SimulateClick()
        {
            if(isClicking)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            isClicking = true;
            int interval = Convert.ToInt32(numericUpDown1.Value);
            clickTimer.Interval = interval;
            clickTimer.Start();
            buttonStop.Enabled = true;
            buttonStart.Enabled = false;

        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            isClicking = false;
            clickTimer.Stop();
            buttonStop.Enabled = false;
            buttonStart.Enabled = true;

        }

        private void clickTimer_Tick(object? sender, EventArgs e)
        {
            SimulateClick();
            Console.WriteLine("Tick");
        }

        private void statusTimer_Tick(object? sender, EventArgs e)
        {
            Status();
        }

        private void ToggleClickTimer()
        {
            if (isClicking)
            {
                isClicking = false;
                clickTimer.Stop();
                buttonStop.Enabled = false;
                buttonStart.Enabled = true;
            }
            else
            {
                isClicking = true;
                int interval = Convert.ToInt32(numericUpDown1.Value);
                clickTimer.Interval = interval;
                clickTimer.Start();
                buttonStop.Enabled = true;
                buttonStart.Enabled = false;
            }
        }

        private void OnHotkeyPressed(object? sender, HotkeyEventArgs e)
        {
            ToggleClickTimer();
            e.Handled = true;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            clickTimer.Enabled = true;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            clickTimer.Enabled = true;
        }
    }
}