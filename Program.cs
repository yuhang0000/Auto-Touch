using Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            GlobalStatus.IsDebug = Debugger.IsAttached;

            //临时变量
            bool launchform = true;     //启动主窗体

            //接受到启动参数
            if (args != null && args.Length > 0)
            {
                Point nowxy;
                DLL.GetCursorPos(out nowxy);
                int isMouseActionTask = 0;  //-1: 禁用 0: false 1: true
                int x = nowxy.X;
                int y = nowxy.Y;
                int delay = 0;
                int wheel = 0;
                List<string> action = new List<string>();
                List<MouseActionItem> items = new List<MouseActionItem>();

                //错误的命令
                void errcomm()
                {
                    Command.ConsoleLog4CMD("未知指令.");
                    Command.ConsoleLog4CMD("", trymsgbox: false);
                    Command.ConsoleLog4CMD(string.Join("\r\n", GlobalStatus.helptext), "帮助");
                    isMouseActionTask = -1;
                    items.Clear();
                }

                //尝试理解数字
                bool tryparse(string key, string value, out int num)
                {
                    if (int.TryParse(value, out num) == false)
                    {
                        unknowtype(key, value);
                        return false; ;
                    }
                    return true;
                }
                //该重载为非 type 类型
                void unknowtype(string key, string value, string type = "INT")
                {
                    string text = key + " " + value + "\r\n";
                    for (int ii = 0; ii < key.Length + 1; ii++)
                    {
                        text = text + " ";
                    }
                    text = text + "^";
                    for (int ii = 0; ii < value.Length - 1; ii++) //像 GCC 一样
                    {
                        text = text + "~";
                    }
                    text = text + "\r\n该重载为非 " + type + " 类型";

                    Command.ConsoleLog4CMD("", trymsgbox: false);
                    Command.ConsoleLog4CMD(text, "语法错误");
                    isMouseActionTask = -1;
                }

                //便利启动参数
                for (int i = 0; i < args.Length; i++)
                {
                    //帮助
                    if (string.Equals(args[i], "-h", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--help", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "/?", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        isMouseActionTask = -1;
                        launchform = false;
                        Command.ConsoleLog4CMD("", trymsgbox: false);
                        Command.ConsoleLog4CMD(string.Join("\r\n", GlobalStatus.helptext), "帮助");
                        items.Clear();
                        break;
                    }
                    //版本
                    else if (string.Equals(args[i], "-v", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "-ver", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--version", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        isMouseActionTask = -1;
                        launchform = false;
                        Command.ConsoleLog4CMD("Build Time: " + GlobalStatus.BuildTime, trymsgbox: false);
                        items.Clear();
                        break; //前面 ConsoleLog4CMD 初始化时, 已经打印版本信息了. 
                    }
                    //从预设启动
                    else if (string.Equals(args[i], "-p", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--profile", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        isMouseActionTask = -1;
                        launchform = false;
                        if (i + 1 < args.Length)
                        {
                            string path = GlobalStatus.AssumptionPath + args[i + 1] + ".txt";
                            //是预设名称
                            if (args[i + 1].IndexOf("/") == -1 && args[i + 1].IndexOf("\\") == -1 && File.Exists(path) == true)
                            {
                                //path = GlobalStatus.AssumptionPath + args[i + 1] + ".txt";
                            }
                            //是路径
                            else
                            {
                                path = Path.GetFullPath(args[i + 1]);
                            }

                            if (File.Exists(path) == true)
                            {
                                items.Clear();
                                try
                                {
                                    string[] file = File.ReadAllLines(path);
                                    foreach (string line in file)
                                    {
                                        if (line.Trim().Length == 0)
                                        {
                                            continue;
                                        }

                                        //行拆字
                                        string[] word = line.Trim().Split(';');
                                        string[] pos = word[1].Split(',');
                                        MouseActionItem item = new MouseActionItem(int.Parse(pos[0].Trim()), int.Parse(pos[1].Trim()));
                                        item.Delay = int.Parse(word[2].Substring(0, word[2].Length - 2));
                                        item.Wheel = int.Parse(word[3]);
                                        item.Action = word[4];
                                        items.Add(item);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Command.ConsoleLog4CMD("载入预设时出错了, 原因是: \r\n" + ex.ToString(), "Oops! ");
                                    break;
                                }

                                i++;
                                //break;
                            }
                            //预设不存在
                            else
                            {
                                //ConsoleLog4CMD("找不到该预设: " + args[i + 1]);
                                Command.ConsoleLog4CMD("找不到该预设: " + path);
                                items.Clear();
                                break;
                            }
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //X 轴
                    else if (string.Equals(args[i], "-x", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        if (isMouseActionTask != -1)
                        {
                            isMouseActionTask = 1;
                        }
                        launchform = false;
                        if (i + 1 < args.Length)
                        {
                            if (tryparse(args[i], args[i + 1], out x) == false)
                            {
                                break;
                            }
                            i++;
                            continue;
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //Y 轴
                    else if (string.Equals(args[i], "-y", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        if (isMouseActionTask != -1)
                        {
                            isMouseActionTask = 1;
                        }
                        launchform = false;
                        if (i + 1 < args.Length)
                        {
                            if (tryparse(args[i], args[i + 1], out y) == false)
                            {
                                break;
                            }
                            i++;
                            continue;
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //滚轮
                    else if (string.Equals(args[i], "-w", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--wheel", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        if (isMouseActionTask != -1)
                        {
                            isMouseActionTask = 1;
                        }
                        launchform = false;
                        if (i + 1 < args.Length)
                        {
                            if (tryparse(args[i], args[i + 1], out wheel) == false)
                            {
                                break;
                            }
                            i++;
                            continue;
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //时间
                    else if (string.Equals(args[i], "-t", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--time", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--delay", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        if (isMouseActionTask != -1)
                        {
                            isMouseActionTask = 1;
                        }
                        launchform = false;
                        if (i + 1 < args.Length)
                        {
                            string t = args[i + 1];
                            //HH:MM:SS
                            if (t.IndexOf(":") != -1)
                            {
                                string[] tt = t.Split(':');
                                //超过了三个冒号或者没有
                                if (tt.Length > 3 || tt.Length == 0)
                                {
                                    unknowtype(args[i], t, "HH:MM:SS");
                                    break;
                                }
                                //挨个转
                                bool err = false;
                                for (int ii = 0; ii < tt.Length; ii++)
                                {
                                    int num = 0;
                                    if (tt[ii].Length == 0 || int.TryParse(tt[ii], out num) == false)
                                    {
                                        unknowtype(args[i], t, "HH:MM:SS");
                                        err = true;
                                        break;
                                    }
                                    //检查时间有没有超过60 的
                                    if (tt.Length > 1 && ii > 0 && num > 59)
                                    {
                                        unknowtype(args[i], t, "HH:MM:SS");
                                        err = true;
                                        break;
                                    }
                                    else
                                    {
                                        delay = delay + (int)Math.Pow(60, -1 + tt.Length - ii) * num;
                                    }
                                }
                                //完成了吗?
                                if (err == true)
                                {
                                    break;
                                }
                                delay = delay * 1000;
                                //ConsoleLog4CMD(delay.ToString());
                            }
                            //假如是纯数字
                            else if (tryparse(args[i], args[i + 1], out delay) == false)
                            {
                                break;
                            }
                            i++;
                            continue;
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //动作
                    else if (string.Equals(args[i], "-a", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--action", StringComparison.CurrentCultureIgnoreCase) == true ||
                        string.Equals(args[i], "--button", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        if (isMouseActionTask != -1)
                        {
                            isMouseActionTask = 1;
                        }
                        launchform = false;
                        if (i + 1 < args.Length)
                        {
                            bool err = false;
                            //便利动作组
                            string[] acts = args[i + 1].ToLower().Split(',');
                            for (int ii = 0; ii < acts.Length; ii++)
                            {
                                err = false;
                                bool err2 = false;
                                string actss = acts[ii];
                                foreach (string mb in GlobalStatus.MouseButtons)
                                {
                                    if (mb == actss)
                                    {
                                        err2 = true;
                                        break;
                                    }
                                }
                                if (err2 == false && actss != "none")
                                {
                                    unknowtype(args[i], args[i + 1], "ACTION");
                                    err = true;
                                    break;
                                }
                                action.Add(actss);
                            }
                            if (err == true)
                            {
                                break;
                            }
                            else
                            {
                                i++;
                                continue;
                            }
                        }
                        //越界了
                        else
                        {
                            errcomm();
                            break;
                        }
                    }
                    //调试
                    else if (string.Equals(args[i], "--debug", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        GlobalStatus.IsDebug = true;
                        if (GlobalStatus.IsAttachConsole == -1)
                        {
                            Command.AttachConsole();
                        }
                    }
                    //未知命令
                    else
                    {
                        launchform = false;
                        errcomm();
                        break;
                    }
                }

                //如果是鼠标动作
                if (isMouseActionTask == 1)
                {
                    MouseActionItem item = new MouseActionItem(nowxy.X, nowxy.Y);
                    items.Add(item);
                    item = new MouseActionItem(x, y);
                    item.Delay = delay;
                    item.Wheel = wheel;
                    item.Action = string.Join("|", action);
                    items.Add(item);
                }

                //执行
                if (items.Count > 0)
                {
                    MouseSentInput msi = new MouseSentInput();
                    msi.Run(items);
                }

                //结束
                if (launchform == false)
                {
                    /*if (GlobalStatus.IsAttachConsole == 1)
                    {
                        DLL.FreeConsole();
                    }*/
                }

            }

            //启动主窗体
            if (launchform == true)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(GlobalStatus.main = new Main());
            }

            //尝试分离控制台
            if(GlobalStatus.IsAttachConsole == 1)
            {
                DLL.FreeConsole();
            }
        }

    }
}
