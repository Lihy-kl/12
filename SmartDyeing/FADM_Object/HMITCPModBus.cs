﻿using EasyModbus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace SmartDyeing.FADM_Object
{
    public class HMITCPModBus
    {
        public HMITCPModBus() { }

        //
        public ModbusClient _modbusClient;

        //是否已连接
        public bool _b_Connect = false;

        //连接IP
        public string _s_ip = "192.168.1.2";
        //连接端口
        public int _i_port = 502;

        //是否已经发送过开关盖状态
        public bool _b_isSendCoverStatus1 = false;
        public bool _b_isSendCoverStatus2 = false;
        public bool _b_isSendCoverStatus3 = false;
        public bool _b_isSendCoverStatus4 = false;
        public bool _b_isSendCoverStatus5 = false;
        public bool _b_isSendCoverStatus6 = false;
        public bool _b_isSendCoverStatus7 = false;
        public bool _b_isSendCoverStatus8 = false;
        public bool _b_isSendCoverStatus9 = false;
        public bool _b_isSendCoverStatus10 = false;
        public bool _b_isSendCoverStatus11 = false;
        public bool _b_isSendCoverStatus12 = false;
        //是否已经发送过开关盖状态（用于12杯）
        public bool[] _b_isSendCoverStatus= { false, false, false, false, false, false, false, false, false, false, false, false};


        //连接
        public int Connect()
        {
            try
            {
                _modbusClient = new ModbusClient(_s_ip, _i_port);  // IP地址和端口
                _modbusClient.Connect(); // 连接到服务器
                _b_Connect = true;
                return 0;
            }
            catch
            {
                return -1;
            }
        }
        //关闭连接
        public int Disconnect()
        {
            _modbusClient.Disconnect(); // 断开连接
            return 0;
        }

        //重连
        public int ReConnect()
        {
            Lib_Log.Log.writeLogException("注意,重连!!!!!");
            Console.WriteLine("注意,重连!!!!!");
            try
            {
                _modbusClient.Disconnect(); // 断开连接
                _modbusClient.Connect(); // 连接到服务器
                return 0;
            }
            catch { return -1; }
        }


        //读寄存器数据
        //startingAddress读取寄存器开始地址；num读取寄存器数量；values返回寄存器的值

        public int Read(int i_startingAddress, int i_num, ref int[] ia_values)
        {
            lock (this)
            {
                try
                {
                    //_modbusClient.Connect(); // 连接到服务器
                    ia_values = _modbusClient.ReadHoldingRegisters(i_startingAddress, i_num);
                    //_modbusClient.Disconnect(); // 断开连接
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("读寄存器数据异常" + ex.Message);
                    return -1;
                }
            }
        }

        //写寄存器数据
        //startingAddress读取寄存器开始地址；values返回寄存器的值
        public int Write(int i_startingAddress, int[] ia_values)
        {
            lock (this)
            {
            labRewrite:
                try
                {
                    //_modbusClient.Connect(); // 连接到服务器
                    _modbusClient.WriteMultipleRegisters(i_startingAddress, ia_values);
                    // _modbusClient.Disconnect(); // 断开连接
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("写寄存器数据异常" + ex.Message);
                    ReConnect();
                    goto labRewrite;
                    return -1;
                }
            }
        }

        public bool getConnect()
        {
            try
            {
                //bool _b_istrue =_modbusClient.Available(2000);
                return _modbusClient.Connected;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 染色/后处理读
        /// </summary>
        /// <param name="i_type">打板机类型 0 转子机 1 摇摆机 </param>
        /// <returns></returns>
        public List<Data> DyeRead(ref List<Data> lis_l,int i_type)
        {

            lock (this)
            {
                try
                {
                    if (i_type == 1 || i_type == 2|| i_type == 3)
                    {

                        for (int i = 0; i < 6; i++)
                        {
                            int[] ia_values = new int[64];
                            int i_ret = -1;
                            //if (i_type == 0)
                            {
                                i_ret = Read(500 + 64 * i, 64, ref ia_values);
                                if (i_ret == 0)
                                {
                                    //解析数据
                                    Data d = new Data();
                                    d._s_waitData = (ia_values[0]).ToString();
                                    d._s_isTotalFinish = (ia_values[1]).ToString();
                                    d._s_realTem = (ia_values[2]).ToString();
                                    d._s_currentCraft = (ia_values[3]).ToString();
                                    d._s_currentState = (ia_values[4]).ToString();
                                    d._s_currentStepNum = (ia_values[5]).ToString();
                                    d._s_holdTimes = (ia_values[6]).ToString();
                                    d._s_overTime = (ia_values[7]).ToString();
                                    d._s_openInplace = (ia_values[8]).ToString();
                                    d._s_addWater = (ia_values[9]).ToString();
                                    d._s_dripFail = (ia_values[10]).ToString();
                                    d._s_history = (ia_values[11]).ToString();
                                    d._s_lockUp = (ia_values[17]).ToString();
                                    d._s_Warm = (ia_values[19]).ToString();
                                    d._s_secondhistory = (ia_values[21]).ToString();
                                    d._s_secondopenInplace = (ia_values[22]).ToString();
                                    d._s_secondrealTem = (ia_values[44]).ToString();
                                    //int[] ia_values1 = new int[1];
                                    lis_l.Add(d);
                                    _b_Connect = true;
                                    //int ret1 = Read(508 + 64 * i, 1, ref ia_values1);
                                    //if (ret1 == 0)
                                    //{
                                    //    d._s_coverSign = (ia_values1[0]).ToString();
                                    //    lis_l.Add(d);
                                    //    _b_Connect = true;
                                    //}
                                    //else
                                    //{
                                    //    _b_Connect = false;
                                    //}
                                }
                                else
                                {
                                    _b_Connect = false;
                                }
                            }

                        }
                    }
                    else
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int[] ia_values = new int[16];
                            int i_ret = -1;
                            i_ret = Read(300 + 16 * i, 16, ref ia_values);
                            if (i_ret == 0)
                            {
                                //解析数据
                                Data d = new Data();
                                d._s_waitData = (ia_values[0]).ToString();
                                d._s_isTotalFinish = (ia_values[1]).ToString();
                                d._s_realTem = (ia_values[2]).ToString();
                                d._s_currentCraft = (ia_values[3]).ToString();
                                d._s_currentState = (ia_values[4]).ToString();
                                d._s_currentStepNum = (ia_values[5]).ToString();
                                d._s_overTemTimes = (ia_values[6]).ToString();
                                d._s_overTime = (ia_values[7]).ToString();
                                d._s_openInplace = (ia_values[8]).ToString();
                                d._s_addWater = (ia_values[9]).ToString();
                                d._s_dripFail = (ia_values[10]).ToString();
                                d._s_history = (ia_values[11]).ToString();
                                int[] ia_values1 = new int[1];
                                lis_l.Add(d);
                                _b_Connect = true;
                            }
                            else
                            {
                                _b_Connect = false;
                            }
                        }
                    }


                    //解析数据
                    return lis_l;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// 读取报警信息
        /// </summary>
        /// <param name="iStationID">站号</param>
        /// <returns></returns>
        public int  DyeReadAlarm()
        {

            lock (this)
            {
                try
                {

                    int[] values = new int[1];
                    int ret = -1;
                    ret = Read(460, 1, ref values);
                    if (ret == 0)
                    {
                        int num = values[0];
                        _b_Connect = true;
                        return num;
                    }
                    else
                    {
                        _b_Connect = false;
                        return -1;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// 染色/后处理数据交互结构
    /// </summary>
    public class Data
    {
        /// <summary>
        /// 等待数据
        /// </summary>
        public string _s_waitData;

        /// <summary>
        /// 总工艺完成
        /// </summary>
        public string _s_isTotalFinish;

        /// <summary>
        /// 实际温度
        /// </summary>
        public string _s_realTem;

        /// <summary>
        /// 副杯实际温度
        /// </summary>
        public string _s_secondrealTem;

        /// <summary>
        /// 当前工艺
        /// </summary>
        public string _s_currentCraft;

        /// <summary>
        /// 当前状态
        /// </summary>
        public string _s_currentState;

        /// <summary>
        /// 当前步号
        /// </summary>
        public string _s_currentStepNum;

        /// <summary>
        /// 当前保温时间
        /// </summary>
        public string _s_holdTimes;

        /// <summary>
        /// 超温度次数
        /// </summary>
        public string _s_overTemTimes;

        /// <summary>
        /// 超温度时间
        /// </summary>
        public string _s_overTime;

        /// <summary>
        /// 杯盖动作信号(或泄压信号)
        /// </summary>
        public string _s_openInplace;

        /// <summary>
        /// 副杯杯盖动作信号(或泄压信号)
        /// </summary>
        public string _s_secondopenInplace;

        /// <summary>
        /// 洗杯加水
        /// </summary>
        public string _s_addWater;

        /// <summary>
        /// 滴液失败信号
        /// </summary>
        public string _s_dripFail;

        /// <summary>
        /// 历史状态
        /// </summary>
        public string _s_history;

        /// <summary>
        /// 副杯历史状态
        /// </summary>
        public string _s_secondhistory;

        /// <summary>
        ///  请求开关盖信号
        /// </summary>
        public string _s_coverSign;

        /// <summary>
        ///  副杯请求开关盖信号
        /// </summary>
        public string _s_secondcoverSign;

        /// <summary>
        ///  锁止上状态0=无信号 1=锁止上信号
        /// </summary>
        public string _s_lockUp;

        /// <summary>
        ///  报警提示 0=超极限温度 1=电机电流过大 2=高于安全温度进入冷行 3=回原点超时
        /// </summary>
        public string _s_Warm;


    }
}
