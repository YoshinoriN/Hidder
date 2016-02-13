namespace Hidder.Models
{
    public enum ApplicationStatusEnum : int
    {
        /// <summary>
        /// 待機
        /// </summary>
        Ready = 0,
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
        Stopping = 3,
        /// <summary>
        /// 存在せず
        /// </summary>
        DoesntExist = 98,
        /// <summary>
        /// 失敗
        /// </summary>
        Faild = 99
    }
}
