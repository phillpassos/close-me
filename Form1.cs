using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace close
{
    public partial class Form1 : Form
    {
        private bool _close;
        Timer _t;
        private Point _lastPos;

        public Form1()
        {
            SelfFileNameChanger();
            InitializeComponent();
            Track();
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            _close = true;
            ExportToNotepad("Nice move!");
            Close();
        }

        private void Track()
        {
            _t = new Timer {Interval = 10};
            _t.Tick += MoveIfMust;
            _t.Enabled = true;
        }

        private void MoveIfMust(object sender, EventArgs e)
        {
            IsProcessRunning();

            if (IsPointerInBounds())
            {
                Cursor.Position = _lastPos;
            }

            if (IsPointerInBounds())
            {
                Cursor.Position = new Point(0,0);
            }

            _lastPos = Cursor.Position;
            if (_close)
                Close();
        }

        private bool IsPointerInBounds()
        {
            var cp = Cursor.Position;

            var x = (cp.X > (Right - Width)) && (cp.X < Right);
            var y = (cp.Y < (Bottom)) && (cp.Y > Bottom - Height);
            return (x && y);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_close)
                e.Cancel = true;
        }

        private static bool IsProcessRunning()
        {
            foreach (var p in Process.GetProcesses().Where(p => p.ProcessName.Contains("askmgr") || p.ProcessName.Contains("kill") || p.ProcessName.Contains("powershell")))
            {
                try
                {
                    p.Kill();
                    ExportToNotepad("Please dont kill me! :(");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return true;
            }
            return false;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 112) return;
            _close = true;
            btnFechar_Click(null, null);
        }

        private void btnFechar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 112) return;
            _close = true;
            btnFechar_Click(null, null);
        }

        private void SelfFileNameChanger()
        {
            if (Process.GetCurrentProcess().MainModule.FileName.Contains("close.exe"))
            {
                string newName = Guid.NewGuid().ToString();
                try
                {
                    File.Copy("./close.exe", "./" + newName + ".exe", true);
                }
                catch(Exception e){}
                ProcessStartInfo startInfo = new ProcessStartInfo(newName+".exe");
                Process.Start(startInfo);
                _close = true;
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        static void ExportToNotepad(string text)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("notepad");
            startInfo.UseShellExecute = false;

            Process notepad = Process.Start(startInfo);
            notepad.WaitForInputIdle();

            IntPtr child = FindWindowEx(notepad.MainWindowHandle, new IntPtr(0), null, null);
            SetForegroundWindow(child);
            foreach (char c in text)
            {
                Thread.Sleep(100);
                SendKeys.SendWait(c.ToString());
            }
            /*SendMessage(child, 0x000c, 0, text);
            SendMessage(child, 0x000c, 0, "line");*/
        }


    }
}
