﻿using Lib_Card.Base;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Lib_Card.ADT8940A1.InPut
{
    /// <summary>
    /// 改线版
    /// </summary>
    public class InPut_Basic : InPut
    {
        public override int InPutStatus(int iInPutNo)
        {
        label:
            int iRes = CardObject.OA1.ReadInPut(iInPutNo);
            if (-1 == iRes)
                return -1;

            if (iInPutNo == ADT8940A1_IO.InPut_Sunx_A ||
                iInPutNo == ADT8940A1_IO.InPut_Sunx_B ||
                iInPutNo == ADT8940A1_IO.InPut_Stop)
            {
                if (0 == iRes)
                    return 0;
                else
                    return 1;
            }
            else
            {
                if (0 == iRes)
                {
                    //for (int i = 0; i < 2; i++)
                    //{
                    //    iRes = CardObject.OA1.ReadInPut(iInPutNo);
                    //    if (-1 == iRes)
                    //        return -1;
                    //    else if (iRes == 1)
                    //    {
                    //        var stacktrace = new StackTrace();
                    //        string Name = "";
                    //        for (var p = 0; p < stacktrace.FrameCount; p++)
                    //        {
                    //            if (p == 1)
                    //            {
                    //                var method = stacktrace.GetFrame(p).GetMethod();
                    //                Name = method.Name; break;
                    //            }
                    //        }
                    //        Lib_Log.Log.writeLogException(Name+"输入点：" + iInPutNo + ";次数：" + i);
                    //        goto label;
                    //    }
                    //}
                    return 1;
                }
                else
                {
                    //for (int i = 0; i < 2; i++)
                    //{
                    //    iRes = CardObject.OA1.ReadInPut(iInPutNo);
                    //    if (-1 == iRes)
                    //        return -1;
                    //    else if (iRes == 0)
                    //    {
                    //        var stacktrace = new StackTrace();
                    //        string Name = "";
                    //        for (var p = 0; p < stacktrace.FrameCount; p++)
                    //        {
                    //            if (p == 1)
                    //            {
                    //                var method = stacktrace.GetFrame(p).GetMethod();
                    //                Name = method.Name; break;
                    //            }
                    //        }
                    //        Lib_Log.Log.writeLogException(Name + "输入点：" + iInPutNo + ";次数：" + i);
                    //        goto label;
                    //    }
                    //}
                    return 0;
                   

                }
            }

        }
    }
}
