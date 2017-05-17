using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace close
{
    public partial class Form1 : Form
    {
        private bool _close;
        Timer _t;
        private Point _lastPos;

        public Form1()
        {
            InitializeComponent();
            Track();
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
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
            var cp = Cursor.Position;

            var x = (cp.X > (Right - Width)) && (cp.X < Right);
            var y = (cp.Y < (Bottom)) && (cp.Y > Bottom - Height);
            if (x && y)
            {
                Cursor.Position = _lastPos;
            }
            _lastPos = Cursor.Position;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_close)
                e.Cancel = true;
        }

        private static bool IsProcessRunning()
        {
            foreach (var p in Process.GetProcesses().Where(p => p.ProcessName.Contains("askmgr") || p.ProcessName.Contains("kill")))
            {
                p.Kill();
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


    }
}
