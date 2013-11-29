using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using SystemHook;
namespace 钩子测试
{
    public partial class Form1 : Form
    {
        HookBarCode hbc;
        public Form1()
        {
            InitializeComponent();
        }

        private void ShowMSG(string MSG)
        {
            listBox1.Items.Add(DateTime.Now.ToString() + "::" + MSG);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hbc= new HookBarCode();
            hbc.Add(ShowMSG);
            hbc.StartSystemHook();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            hbc.StopHook();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}