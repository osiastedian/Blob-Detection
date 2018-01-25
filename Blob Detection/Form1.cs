using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blob_Detection
{
    public partial class Form1 : Form
    {
        CCL target;
        Bitmap currentBitmap;
        public Form1()
        {
            InitializeComponent();
            target = new CCL();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (File.Exists(openFileDialog1.FileName)) {
                currentBitmap = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = currentBitmap;
                pictureBox1.Refresh();
            }
        }

        private void detectBlobsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentBitmap != null)
            { 
                Bitmap b = target.coloredBlobs(currentBitmap);
                pictureBox1.Image = b;
                pictureBox1.Refresh();
            }

        }
    }
    
}
