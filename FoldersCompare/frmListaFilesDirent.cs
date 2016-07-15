using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoldersCompare
{
    public partial class frmListaFilesDirent : Form
    {
        
        public frmListaFilesDirent(List<string> files)
        {
            InitializeComponent();

            StringBuilder strBuilder = new StringBuilder();

            foreach (string file in files)
            {
                strBuilder.AppendLine(file);    
            }

            txtFiles.Text = strBuilder.ToString();
        }
              
    }
}
