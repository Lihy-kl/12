﻿using SmartDyeing.FADM_Auto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartDyeing.FADM_Control
{
    public partial class AbsBeater : UserControl
    {
        private SmartDyeing.FADM_Control.Cup _cup = null;

        public AbsBeater()
        {
            InitializeComponent();

            cup1.MouseDown += Cup_MouseDown;
            cup1.ContextMenuStrip = this.contextMenuStrip1;

            cup2.MouseDown += Cup_MouseDown;
            cup2.ContextMenuStrip = this.contextMenuStrip1;
        }

        void Cup_MouseDown(object sender, MouseEventArgs e)
        {
            _cup = (SmartDyeing.FADM_Control.Cup)sender;
        }

        private void tsm_Online_Click(object sender, EventArgs e)
        {
            string s_sql1 = "update abs_cup_details set Enable=1,Statues = '待机',IsUsing = 0  where CupNum = " + _cup.NO + " and Statues ='下线'; ";
            FADM_Object.Communal._fadmSqlserver.ReviseData(s_sql1);
        }

        private void tsm_Offline_Click(object sender, EventArgs e)
        {
            string s_sql = "UPDATE abs_cup_details SET IsUsing = 0, Statues = '下线',Enable=0 " +
                " WHERE CupNum = " + _cup.NO + " AND Statues = '待机'  and IsUsing = 0;";
            FADM_Object.Communal._fadmSqlserver.ReviseData(s_sql);
        }

        private void tsm_Stop_Click(object sender, EventArgs e)
        {
            if (FADM_Object.Communal._b_absErr)
                return;

            Thread thread = new Thread(() =>
            {
                //等待没有交互时再发送停止
                while (true)
                {
                    if (MyAbsorbance._abs_Temps[Convert.ToInt32(_cup.NO) - 1]._s_request == "0")
                    {
                        break;
                    }
                }

                //先发一个停止，再发一个洗杯
                int[] values1 = new int[1];
                values1[0] = 2;
                if (Convert.ToInt32(_cup.NO) == 1)
                    FADM_Object.Communal._tcpModBusAbs.Write(800, values1);
                else
                    FADM_Object.Communal._tcpModBusAbs.Write(810, values1);

                //判断待机后再发洗杯
                while (true)
                {
                    if (MyAbsorbance._abs_Temps[Convert.ToInt32(_cup.NO) - 1]._s_currentState == "1")
                        break;
                }

                //发送启动
                int[] values = new int[5];
                values[0] = 1;
                values[1] = 0;
                values[2] = 0;
                values[3] = 0;
                values[4] = 3;
                if (!FADM_Object.Communal._tcpModBusAbs._b_Connect)
                {
                    FADM_Object.Communal._tcpModBusAbs.ReConnect();
                }

                //写入测量数据
                int d_1 = 0;
                d_1 = Convert.ToInt32(FADM_Object.Communal._d_abs_total * 1000) / 65536;
                int i_d_11 = Convert.ToInt32(FADM_Object.Communal._d_abs_total * 1000) % 65536;

                int d_2 = 0;
                d_2 = (Convert.ToInt32(Lib_Card.Configure.Parameter.Other_AbsAddWater * 1000) + 10000) / 65536;
                int i_d_22 = (Convert.ToInt32(Lib_Card.Configure.Parameter.Other_AbsAddWater * 1000) + 10000) % 65536;

                int d_3 = 0;
                d_3 = Lib_Card.Configure.Parameter.Other_WashStirTime / 65536;
                int i_d_33 = Lib_Card.Configure.Parameter.Other_WashStirTime % 65536;

                int d_4 = 0;
                d_4 = Lib_Card.Configure.Parameter.Other_StirTime / 65536;
                int i_d_44 = Lib_Card.Configure.Parameter.Other_StirTime % 65536;

                int d_5 = 0;
                d_5 = Lib_Card.Configure.Parameter.Other_AspirationTime / 65536;
                int i_d_55 = Lib_Card.Configure.Parameter.Other_AspirationTime % 65536;

                int[] ia_array = new int[] { i_d_11, d_1, i_d_22, d_2, i_d_33, d_3 };
                if (Convert.ToInt32(_cup.NO) == 1)
                    FADM_Object.Communal._tcpModBusAbs.Write(1010, ia_array);
                else
                    FADM_Object.Communal._tcpModBusAbs.Write(1060, ia_array);

                if (Convert.ToInt32(_cup.NO) == 1)
                    FADM_Object.Communal._tcpModBusAbs.Write(800, values);
                else
                    FADM_Object.Communal._tcpModBusAbs.Write(810, values);

                string s_sql = "UPDATE abs_cup_details SET Statues='洗杯',IsUsing = 1,Type=0  WHERE CupNum = " + _cup.NO + " ;";
                FADM_Object.Communal._fadmSqlserver.ReviseData(s_sql);
            });
            thread.Start();
            
        }

        private void tsm_Reset_Click(object sender, EventArgs e)
        {
            string s_sql1 = "update abs_cup_details set Enable=1,IsUsing = 0,Statues = '待机',Cooperate=0  where CupNum = " + _cup.NO + " ; ";
            FADM_Object.Communal._fadmSqlserver.ReviseData(s_sql1);
        }
    }
}
