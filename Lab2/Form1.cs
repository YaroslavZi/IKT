using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace Lab2
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Tree_BeforeExpand(object sender, TreeViewCancelEventArgs e) 
        { 
            Build(e.Node); 
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            string fullPath = selectedNode.FullPath;
            DirectoryInfo di = new DirectoryInfo(fullPath);
            FileInfo[] fiArray;
            DirectoryInfo[] diArray;
            try
            {
                fiArray = di.GetFiles();
                diArray = di.GetDirectories();
            }
            catch { return; }
            DataTable.Items.Clear(); 
            foreach (DirectoryInfo dirInfo in diArray) 
            {
                ListViewItem list = new ListViewItem(dirInfo.Name);
                list.Checked = true;
                list.SubItems.Add(" ");
                list.SubItems.Add("Папка с файлами");
                DataTable.Items.Add(list);
            }
            long small = 0;
            long average = 0; 
            long big = 0;
            long count = 0;
            int smallCount = 0;
            int averageCount = 0;
            int bigCount = 0;
            foreach (FileInfo fileInfo in fiArray) 
            {
                count += fileInfo.Length;
                string name = fileInfo.Name;
                ListViewItem list = new ListViewItem(name);
                list.Checked = true;
                list.Tag = fileInfo.FullName;
                list.SubItems.Add(fileInfo.Length.ToString() + " B"); 
                string type;
                Regex reg = new Regex(@"(\.\w{1,4}$)"); 
                foreach (Match wrd in reg.Matches(name))
                {
                    type = wrd.ToString();  
                    list.SubItems.Add(type);
                    DataTable.Items.Add(list);

                    if (type == ".png" || type == ".jpg" || type == ".bmp" || type == ".gif" || type == ".JPG")
                        list.BackColor = Color.Thistle; 
                    else if (type == ".docx" || type == ".xlsx" || type == ".pdf" || type == ".txt")
                        list.BackColor = Color.LightGreen;
                    else if (type == ".zip" || type == ".rar" || type == ".7z")
                        list.BackColor = Color.Khaki;
                    else if (type == ".exe" || type == ".dll" || type == ".ini")
                        list.BackColor = Color.Pink;
                }

                graph.Series.Clear();
                graph.Series.Add("Средний размер файлов");
                if (fileInfo.Length <= 10000)
                {
                    if (fileInfo.Length > small)
                        small = fileInfo.Length;
                }
                if (fileInfo.Length > 10000 && fileInfo.Length < 50000)
                {
                    if (fileInfo.Length > average)
                        average = fileInfo.Length;
                }
                if (fileInfo.Length >= 50000)
                {
                    if (fileInfo.Length > big)
                        big = fileInfo.Length;
                }
                graph.Series[0].Points.AddXY("Small", small);
                graph.Series[0].Points.AddXY("Average", average);
                graph.Series[0].Points.AddXY("Big", big);

            }
            if(smallCount != 0 )
            {
                graph.Series[0].Points.AddXY("small", small / smallCount);
            }
            else
            {
               graph.Series[0].Points.AddXY("small", small / 1);
            }

            if (averageCount != 0)
            {

                graph.Series[0].Points.AddXY("average", average / averageCount);
            }
            else
            {

                graph.Series[0].Points.AddXY("average", average / 1);
            }
            if (bigCount != 0)
            {

                graph.Series[0].Points.AddXY("big", big / bigCount);
            }
            else
            {

                graph.Series[0].Points.AddXY("big", big / 1);
            }

            statusStrip1.Items[0].Text = "Total bytes: " + count;
            statusStrip1.Items[1].Text = DataTable.CheckedItems.Count + " of " + DataTable.CheckedItems.Count + " items selected"; 
        }
        

        private void Build(TreeNode parent)
        {
            var path = parent.Tag as string;
            parent.Nodes.Clear();
            foreach (var dir in Directory.GetDirectories(path))
                parent.Nodes.Add(new TreeNode(Path.GetFileName(dir), new[] { new TreeNode("...") }) { Tag = dir });
            foreach (var file in Directory.GetFiles(path))
                parent.Nodes.Add(new TreeNode(Path.GetFileName(file), 1, 1) { Tag = file });
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                TreeNode node = new TreeNode() { Text = fbd.SelectedPath.ToString(), Tag = fbd.SelectedPath };
                treeView1.Nodes.Add(node);
                Build(node);
                node.Expand();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) 
        {
                this.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            FontDialog font = new FontDialog();
            if (font.ShowDialog() == DialogResult.OK)
                DataTable.Font = font.Font;
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog color = new ColorDialog();
            if (color.ShowDialog() == DialogResult.OK)
                DataTable.ForeColor = color.Color;
        }
        
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "C:/Users/admin/Desktop/icons/Новый Текстовый Документ.txt");
            File.WriteAllText(path, "Содержимое файла");
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            { 
                    System.Diagnostics.Process.Start("C:/Users/admin/Desktop/icons/Новый Текстовый Документ.txt");      
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            string message = "Тут должен быть справочник";
            MessageBox.Show(message);
        }      
    }
}
