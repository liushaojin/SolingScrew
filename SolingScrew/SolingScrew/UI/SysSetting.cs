﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolingScrew
{
    using SolingScrew.FileOpAPI;
    using SolingScrew.DataDal;
    public partial class SysSetting : Form
    {
        string torsion2Up = string.Empty;
        string torsion2Down = string.Empty;
        string torsionUp = string.Empty;
        string torsionDown = string.Empty;
        string section = "SysSetting";
        string key1Up = "Screw1TorsionUp";
        string key1Down = "Screw1TorsionDown";
        string key2Up = "Screw2TorsionUp";
        string key2Down = "Screw2TorsionDown";
        string reDo = "ReDo";
        string baudRate = "BaudRate";
        int curBaudRate = 1;
        
        ModbusRtuDev rtuCom = ModbusRtuDev.GetRtuInstance();
        IniFileHelper iniFileOp = new IniFileHelper();
        public SysSetting()
        {
            InitializeComponent();
        }
        
        /*
        private void tBox_KeyPress(object sender, KeyPressEventArgs e)
        
        {
            if(e.KeyChar == 0x20)
            {
                e.KeyChar = (char)0;    //禁止空格键
            }
        
            if((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0))
            {
                return;    //处理负数
            }
        
            if(e.KeyChar > 0x20)
            {
                try
                {
                    double.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符
                }
            }
        }
        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            const string pattern = @"^\d+\.?\d+{1}";
            string content = ((TextBox)sender).Text;
        
            if(!(Regex.IsMatch(content, pattern)))
            {
                ErrorProvider.SetError((Control)sender, "只能输入数字!");
                e.Cancel = true;
            }
            else
            {
                ErrorProvider.SetError((Control)sender, null);
            }
        }
        */
        private void SysSetting_Load(object sender, EventArgs e)
        {
            InitBaudRate();
            torsionUpLimit.Text = iniFileOp.ReadString(section, key1Up, string.Empty);
            torsionDownLimit.Text = iniFileOp.ReadString(section, key1Down, string.Empty);
            torsion2UpLimit.Text = iniFileOp.ReadString(section, key2Up, string.Empty);
            torsion2DownLimit.Text = iniFileOp.ReadString(section, key2Down, string.Empty);
            string redo = iniFileOp.ReadString(section, reDo, string.Empty);
            checkBox1.Checked = (redo == "true") ? true : false;
            curBaudRate = Convert.ToInt32(iniFileOp.ReadString(section, baudRate, string.Empty));
            baudRateCb.SelectedIndex = curBaudRate;
        }
        
        private void InitBaudRate()
        {
            string baudRate = "9600 bit/s";
            baudRateCb.Items.Add(baudRate);
            baudRate = "19200 bit/s";
            baudRateCb.Items.Add(baudRate);
            baudRate = "38400 bit/s";
            baudRateCb.Items.Add(baudRate);
            baudRate = "57600 bit/s";
            baudRateCb.Items.Add(baudRate);
            baudRate = "115200 bit/s";
            baudRateCb.Items.Add(baudRate);
            baudRateCb.SelectedIndex = 1;
        }
        
        private void defaultBtn_Click(object sender, EventArgs e)
        {
            torsionUpLimit.Text = "4";
            torsionDownLimit.Text = "0";
            torsion2UpLimit.Text = "4";
            torsion2DownLimit.Text = "0";
            checkBox1.Checked = false;
            curBaudRate = 1;
        }
        
        private void saveBtn_Click(object sender, EventArgs e)
        {
            torsionUp = torsionUpLimit.Text.Trim().TrimEnd('0');
            torsionDown = torsionDownLimit.Text.Trim().TrimEnd('0');
            torsion2Up = torsion2UpLimit.Text.Trim().TrimEnd('0');
            torsion2Down = torsion2DownLimit.Text.Trim().TrimEnd('0');
            
            if(string.IsNullOrEmpty(torsionUp) || string.IsNullOrEmpty(torsionDown) || string.IsNullOrEmpty(torsion2Up) || string.IsNullOrEmpty(torsion2Down))
            {
                MessageBox.Show("检测到有参数未设置，请重新输入", "参数为空", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                iniFileOp.WriteString(section, key1Up, torsionUp);
                iniFileOp.WriteString(section, key1Down, torsionDown);
                iniFileOp.WriteString(section, key2Up, torsion2Up);
                iniFileOp.WriteString(section, key2Down, torsion2Down);
                string reScrew = checkBox1.Checked ? "true" : "false";
                iniFileOp.WriteString(section, reDo, reScrew);
                iniFileOp.WriteString(section, baudRate, curBaudRate.ToString());
                byte[] baud = new byte[2];
                baud[0] = 0;
                baud[1] = (byte)curBaudRate;
                rtuCom.WriteSingleReg(1, 2, 1, baud);
                MessageBox.Show("参数已保存，需重启软件生效", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
            }
        }
        
        private void baudRateCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            curBaudRate = baudRateCb.SelectedIndex;
        }
    }
}
