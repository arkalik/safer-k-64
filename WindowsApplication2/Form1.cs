using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WindowsApplication1
{
    public partial class Form1 : Form
    {

        public string direcoryPath;

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            try
            {

                System.Diagnostics.Stopwatch objStopwatch = new System.Diagnostics.Stopwatch();
                objStopwatch.Start();

                StreamReader my_file = new StreamReader(textBox1.Text, System.Text.Encoding.GetEncoding(1251));
                string source = my_file.ReadToEnd();

                Safer sfr = new Safer();
                sfr.Text = source;
                sfr.Key = tKey.Text;
                sfr.Round = int.Parse(comboBox1.SelectedItem.ToString());
                my_file.Close();
                progressBar1.Value = 20;

                byte[] result = sfr.encrypt();
                progressBar1.Value = 70;

                StreamWriter str_wr = new StreamWriter(textBox1.Text + ".safer", false, System.Text.Encoding.GetEncoding(1251));
                string p = System.Text.Encoding.Default.GetString(result);
                str_wr.Write(p);
                str_wr.Close();
                progressBar1.Value = 100;
                objStopwatch.Stop();
                label4.Text = "Encryption time " + objStopwatch.Elapsed;
                MessageBox.Show("File encrypted");
            }
            catch
            {
                MessageBox.Show("Key length must be 8 latin symbols or digits");
            }
            progressBar1.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            try
            {

                System.Diagnostics.Stopwatch objStopwatch = new System.Diagnostics.Stopwatch();
                objStopwatch.Start();

                StreamReader my_file = new StreamReader(textBox1.Text, System.Text.Encoding.GetEncoding(1251));
                string source = my_file.ReadToEnd();

                Safer sfr = new Safer();
                progressBar1.Value = 20;
                sfr.Text = source;
                sfr.Key = tKey.Text;
                sfr.Round = int.Parse(comboBox1.SelectedItem.ToString());
                my_file.Close();

                byte[] result = sfr.decrypt();
                progressBar1.Value = 80;
                string filePath = textBox1.Text;
                filePath = filePath.Replace(".safer", "");
    
                StreamWriter str_wr = new StreamWriter(filePath, false, System.Text.Encoding.GetEncoding(1251));
                string p = System.Text.Encoding.Default.GetString(result);
                str_wr.Write(p);
                str_wr.Close();
                progressBar1.Value = 100;
                objStopwatch.Stop();
                label4.Text = "Decryption time " + objStopwatch.Elapsed;
                MessageBox.Show("File decrypted");
            }
            catch
            {
                MessageBox.Show("Key length must be 8 latin symbols or digits");
            }
            progressBar1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                comboBox1.SelectedIndex = 0;
                treeView1.Nodes.Clear();
                string[] drives = Environment.GetLogicalDrives();
                foreach (string drv in drives)
                {
                    TreeNode node = new TreeNode();
                    node.Text = drv;
                    treeView1.Nodes.Add(node);
                    FillDirectory(drv, node, 0);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void DisplayFiles(string dirName)
        {
            try
            {
                listBox1.Items.Clear();
                DirectoryInfo dir = new DirectoryInfo(dirName);
                if (!dir.Exists)
                    throw new DirectoryNotFoundException
                        ("directory does not exist:" + dirName);                
                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    string str = "[Dir] " + di.Name;
                    listBox1.Items.Add(str);
                }

                foreach (FileInfo fi in dir.GetFiles())
                {
                    listBox1.Items.Add(fi.Name);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void FillDirectory(string drv, TreeNode parent, int level)
        {
            try
            {
                level++;
                if (level > 1)
                    return;
                DirectoryInfo dir = new DirectoryInfo(drv);
                if (!dir.Exists)
                    throw new DirectoryNotFoundException
                        ("directory does not exist:" + drv);
                parent.Nodes.Clear();
                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    TreeNode child = new TreeNode();
                    child.Text = di.Name;
                    parent.Nodes.Add(child);

                    FillDirectory(child.FullPath, child, level);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TreeNode node = e.Node;
                string strFullPath = node.FullPath;
                DisplayFiles(strFullPath);
                textBox1.Text = treeView1.SelectedNode.FullPath;
                this.direcoryPath = textBox1.Text;
                FillDirectory(treeView1.SelectedNode.FullPath, node, 0);
            }
            catch { }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = this.direcoryPath + "\\" + listBox1.SelectedItem.ToString().Replace("[Dir]", "").Trim();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1.ActiveForm.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DisplayFiles(textBox1.Text);
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Brush myBrush = Brushes.Black;
            if (((ListBox)sender).Items[e.Index].ToString().Contains("[Dir]"))
            {
                myBrush = Brushes.Blue;
            }
            e.Graphics.DrawString( ((ListBox)sender).Items[e.Index].ToString().Replace("[Dir]", "").Trim(),
                e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);

            e.DrawFocusRectangle();
        }

    }
}