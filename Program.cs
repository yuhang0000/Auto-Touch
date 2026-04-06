using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Forms;
using Commands;

namespace Auto_Touch
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //先处理全局变数
            GlobalStatus.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            GlobalStatus.BuildTime = System.IO.File.GetLastWriteTime(typeof(GlobalStatus).Assembly.Location);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args == null)
            {
                Application.Run(GlobalStatus.main = new Main());
            }
            else
            {
                Application.Run(GlobalStatus.main = new Main());
            }
        }
    }
}
