using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidder.Models
{
    public enum RunningStatusEnum : int
    {
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 0,
        /// <summary>
        /// 開始中
        /// </summary>
        Starting  = 1,
        /// <summary>
        /// 稼働中
        /// </summary>
        Running = 2,
        /// <summary>
        /// 停止中
        /// </summary>
        Stopping = 3
    }
}
