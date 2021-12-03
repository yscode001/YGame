public class AppConfig
{
    #region 版本号
    public const string Version = "1.0.0";
    public const string Build = "1";
    #endregion

    #region 帧率配置
    public const float FixedTimeInterval = 0.1f;
    #endregion

    #region 运行环境
    private const TRunEnv _runEnv = TRunEnv.develop;
    public static bool IsDevelop => _runEnv == TRunEnv.develop;
    public static bool IsOnline => _runEnv == TRunEnv.online;
    #endregion

    #region 日志控制
    public static bool IsOpenedLog => IsDevelop;
    #endregion
}