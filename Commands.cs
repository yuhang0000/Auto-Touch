using Auto_Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Commands
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class GlobalStatus
    {
        /// <summary>
        /// 主窗体
        /// </summary>
        public static Main main;
        /// <summary>
        /// 版本号
        /// </summary>
        public static string Version;
        /// <summary>
        /// 编译时间
        /// </summary>
        public static DateTime BuildTime;
    }

    public static class Command
    {
        /// <summary>
        /// 关于
        /// </summary>
        public static void About()
        {
            string[] text =
            {
                Application.ProductName, 
                "By: " + Application.CompanyName,
                "BuildTime: " + GlobalStatus.BuildTime.ToString(),
                "Version: " + GlobalStatus.Version,
            };
            MessageBox.Show(string.Join("\r\n", text), "关于");
        }
    }
}
