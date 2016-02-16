namespace Hidder.Models
{
    /// <summary>
    /// WindowStyleのEnum
    /// Hack:ProcessWindowStyleに項目を足したかったので作成。項目はProcessWindowStyleに準拠
    /// </summary>
    public enum WindowStyleEnum : int
    {
        /// <summary>
        /// 通常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 非表示
        /// </summary>
        Hidden = 1,
        /// <summary>
        /// 最小
        /// </summary>
        Minimized = 2,
        /// <summary>
        /// 最大
        /// </summary>
        Maximized = 3,
        /// <summary>
        /// プロセス停止
        /// </summary>
        None = 99
    }
}
