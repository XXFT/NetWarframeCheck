using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.IO;

namespace NetWarframe
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "星际战甲 工具箱 - V1.O ";
            programStart();
        }

        /// <summary>
        /// 程序入口
        /// </summary>
        static void programStart()
        {
            menu();
            switch (menuNumberInput())
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("\r\n正在为您检测网络状态,请稍后...");
                    networkState();
                    break;
                case 2:
                    Console.Clear();
                    replaceLog();
                    programStart();//返回入口
                    break;
                default:
                    Console.Clear();
                    menuError();
                    programStart();//返回入口
                    break;
            }
            //Console.WriteLine("\r\n按下回车关闭软件.");
            programStart();//返回程序入口
            //Console.ReadLine();//回车关闭
        }

        static void networkState()
        {
            // http://irc4.warframe.com.cn 国际服的IRC服务器????
            // 国服的聊天服务器的TCP协议...域名未知.

            ping("api-zhb.warframe.com.cn", "游戏服务器");

            GetHead("http://origin.zhb.warframe.changyou.com", "origin 效验服务器");//因为这里好像是动态的 所以只能404了
            GetHead("http://content.zhb.warframe.changyou.com/Lotus/Language/MOTD_zh.rtf.lzma", "content 效验服务器");
            //GetHead("http://api-zhb.warframe.com.cn/api/findSessions.php", "游戏服务器");//???
            GetHead("http://warframe.login.changyou.com", "登录服务器");
            GetHead("http://content-zhb.warframe.com.cn/dynamic/worldState.php", "警报服务器");
            GetHead("http://mon.changyou.com/client.html", "登录统计服务器");
            GetHead("https://arbiter-zhb.warframe.com.cn/api", "中继站服务器");
        }

        /// <summary>
        /// 获取Http头 输出状态码
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="description">描述</param>
        static void GetHead(string url, string description)
        {
            Console.WriteLine("\r\n----------HTTP 解析 " + description + " 等待---------");
            WebRequest Http = WebRequest.Create(url);
            Http.Method = "HEAD";//设置Method为HEAD
            try
            {
                HttpWebResponse response = (HttpWebResponse)Http.GetResponse();
                setFontColorGreen("服务器完美解析,测试结果:" + Convert.ToInt32(response.StatusCode) + " | " + response.StatusCode.ToString());//Statuscode 为枚举类型，200为正常，其他输出异常，需要转为int型才会输出状态码
                response.Close();
                Console.WriteLine("----------HTTP 解析 " + description + " 完毕----------");
            }
            catch (WebException ex)
            {
                //int statuscode;
                //console.writeline(ex.message);
                //    if(int.tryparse(, out statuscode))
                //{

                //}
                if (ex.Status.ToString() == "ProtocolError")//服务器有正常返回
                {
                    setFontColorGreen("服务器正常解析,测试结果:" + Convert.ToInt32(((HttpWebResponse)ex.Response).StatusCode) + " | " + ((HttpWebResponse)ex.Response).StatusCode.ToString());
                    //Console.WriteLine(ex.Message);
                }
                else if (ex.Status.ToString() == "NameResolutionFailure")//无法解析主机
                {
                    setFontColorRed("致命错误:无法解析服务器.");
                }
                else if (ex.Status.ToString() == "ConnectFailure")
                {
                    setFontColorRed("致命错误:无法进行Http连接.");
                }
                else
                {
                    Console.WriteLine("警告:未知的错误.");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.Status);
                }
                Console.WriteLine("----------HTTP 解析 " + description + " 完毕----------");
            }
        }

        /// <summary>
        /// ping
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="description">描述</param>
        static void ping(string host, string description)
        {
            Console.WriteLine("\r\n----------Ping 延迟 " + description + " 等待----------");
            //Ping 实例对象;
            Ping pingSender = new Ping();
            PingReply reply;
            //调用同步send方法发送数据，结果存入reply对象;
            try
            {
                reply = pingSender.Send(host, 10000);//十秒
            }
            catch (Exception e)//找不到主机就会异常,所以抛出异常.
            {
                Console.WriteLine("错误: " + description);
                Console.WriteLine("详细: " + e);
                return;
            }
            reply = pingSender.Send(host, 10000);//十秒
            Console.WriteLine("服务器:" + description);
            if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine("IP:" + reply.Address);
                setFontColorGreen("延迟:" + reply.RoundtripTime + " & " + "TTL:" + reply.Options.Ttl);
            }
            else if (reply.Status == IPStatus.TimedOut)
            {
                setFontColorRed("错误:连接超时 " + description);
            }
            else
            {
                setFontColorRed("错误:找不到 " + description);
            }
            Console.WriteLine("----------Ping 延迟 " + description + " 完毕----------");
        }

        /// <summary>
        /// 翻译
        /// </summary>
        /// <returns></returns>
        static string translation(string str)//乱七八糟写的
        {
            str = str.Replace(" Sys ", " 系统 ");
            str = str.Replace(" [Diag]", " [特征]");
            str = str.Replace(" [Info]", " [信息]");
            str = str.Replace(" [Error]", " [错误]");
            str = str.Replace(" [Warning]", " [警告]");
            str = str.Replace(" Script ", " 脚本 ");
            str = str.Replace(" Input ", " 输入 ");
            str = str.Replace(" Game ", " 游戏 ");
            str = str.Replace(" Net ", " 网络 ");
            str = str.Replace(" All ", " 全部 ");

            str = str.Replace("Harrow Quest Key Ornament Deco", " Harrow 神秘钥匙装饰");
            str = str.Replace("static entities created at runtime", "在运行时创建的静态实体");
            str = str.Replace("Animation channel", "动画通道");
            str = str.Replace("does not map to root channel", "没有映射到根通道");
            str = str.Replace("SetMorphValue failed", "值设定失败");
            str = str.Replace("entity is not animated", "实体未动画化");
            str = str.Replace("Strict NAT detected", "严格NAT检测");
            str = str.Replace("can't execute OnLoaded callback, invalid owner", "无法执行重载回调，无效所有者");
            str = str.Replace("Decoration", "装饰");
            str = str.Replace("TennoAvatar", "天诺动画");
            str = str.Replace("LotusEffectDecoration", "莲效果装饰");
            str = str.Replace("ScriptTrigger", "脚本触发程序");
            str = str.Replace("Keyword", "关键词");
            str = str.Replace("not found in", "找不到");
            str = str.Replace("Could not create", "无法创建");
            str = str.Replace("building", "建立中");
            str = str.Replace("loading", "加载中");
            str = str.Replace("object", "对象");
            str = str.Replace("can't import movie from url", "在URL中无法导入视频");
            str = str.Replace("not in cache manifest", "没有显示缓存");
            str = str.Replace("If this happens during gameplay it could cause performance problems!", "如果这在游戏过程中发生，可能会导致性能问题！");
            str = str.Replace("no player spawns", "找不到玩家子对象");
            str = str.Replace("Unable to run", "无法运行");
            str = str.Replace("Artifact with no item compat", "没有物品的神器");
            str = str.Replace("as a Item", "作为一个项目");
            str = str.Replace("Could not preprocess", "无法预处理");
            str = str.Replace("Could not find", "无法找到");
            str = str.Replace("Can't create header stream", "无法创建头文件流");
            str = str.Replace("Unknown property", "未知属性");
            str = str.Replace("Bad data from", "坏的数据来自");
            str = str.Replace("it's a", "他是一个");
            str = str.Replace("(can't replace with 1)", "(不能替换为1)");
            str = str.Replace("Keyword |COUNT| not found in", "常量关键词未找到");
            str = str.Replace("module version", "模块版本");
            str = str.Replace("Couldn't obtain the", "无法获取");
            str = str.Replace("game rules", "游戏规则");
            str = str.Replace("Failed to create", "创建失败");
            str = str.Replace("Could not use", "无法使用");
            str = str.Replace("memory was freed", "内存释放");
            str = str.Replace("Current directory", "当前目录");
            str = str.Replace("Current time", "当前时间");
            str = str.Replace("On developer network", "是否开发者网络");
            str = str.Replace("Windows computer-name", "Windows 电脑名字");
            str = str.Replace("Windows user-name", "Windows 用户名字");
            str = str.Replace("Process Command-line", "进程命令行");
            str = str.Replace("Processor", "处理器");
            str = str.Replace("Physical Memory", "物理内存");
            str = str.Replace("Address Space", "地址空间");
            str = str.Replace("Operating System", "操作系统");
            str = str.Replace("System Processes", "系统进程数");
            str = str.Replace("System Threads", "系统线程数");
            str = str.Replace("System Handles", "系统句柄数");
            str = str.Replace("System GDI Objects", "系统 GDI 对象");
            str = str.Replace("System USER Objects", "系统用户数");
            str = str.Replace("System Up-Time", "系统升级时间");
            str = str.Replace(" Downloading ", " 下载 ");
            str = str.Replace(" cache ", " 缓存 ");
            str = str.Replace(" Loaded ", " 加载 ");
            str = str.Replace(" for ", " 在 ");
            str = str.Replace(" package ", " 包 ");
            str = str.Replace("Finished load", "完成加载");
            str = str.Replace(" Finished ", " 完成 ");
            str = str.Replace(" load ", " 加载 ");
            str = str.Replace(" loaded ", " 加载完 ");
            str = str.Replace(" entries", " 条目");
            //str = str.Replace(" of ", " 关于 ");
            str = str.Replace(" Loading ", " 加载中 ");
            str = str.Replace("Adding", " 添加中 ");
            str = str.Replace("Creating", " 创建中 ");
            str = str.Replace("html_font", " 网页字体 ");
            str = str.Replace("Unknown html tag", " 未知的Html标签 ");
            str = str.Replace("ignored", " 忽略 ");

            str = str.Replace("Smart Pointer Report", "智能指针报告");
            str = str.Replace("Buffer underrun streaming", "内存缓冲溢出");
            str = str.Replace("It couldn't use", "它不能使用");
            str = str.Replace("wanted a Mesh", "需要网格(障碍)");
            str = str.Replace("because", "因为");
            str = str.Replace("multiple initial player spawns", "多个玩家初始子对象");
            str = str.Replace("it already has a tag", "它已经有个一个标签");
            str = str.Replace("cannot tag zone", "无法标记区域");
            str = str.Replace("material", "材料");
            str = str.Replace("Required by", "必须使用");
            









            return str;
        }

        /// <summary>
        /// 翻译Log文件 - 更好的查看Log文件
        /// </summary>
        /// <param name="i">输入方式</param>
        /// <returns>返回替换后的信息</returns>
        static string replaceLog()
        {
            /*
             * Environment.UserName//获得当前系统登陆用户名  
             * Environment.UserDomainName//获得当前计算机名
             * %username%
             * C:\Users\FTH\Desktop\
             */
            //Console.Clear();
            TextWriter oldOut = Console.Out;//???一定要声明式才可以继续切换输出
            string fileName = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Warframe\";
            string[] allTextLines = File.ReadAllLines(fileName + "EE.log", Encoding.UTF8); //读取当前文件的所有行
            //File.Delete(fileName + "warframeLogTranslation.txt");
            Console.WriteLine("开始分析,筛选重要部分...");
            StreamWriter sw = new StreamWriter(fileName + "warframeLogTranslation.txt", false, Encoding.UTF8);//直接写到C盘
            for (int j = 0; j < allTextLines.Length; j++)
            {
                Console.SetOut(sw);//写入一行
                Console.WriteLine(translation(allTextLines[j]));
                Console.SetOut(oldOut);//输出一行
                if (allTextLines[j].IndexOf("[Error]") != -1 || allTextLines[j].IndexOf("[Warning]") != -1)
                {
                    setFontColorRed(translation(allTextLines[j]));
                }
                else if (allTextLines[j].IndexOf("[Diag]") != -1)
                {
                    setFontColorGreen(translation(allTextLines[j]));
                }
                else
                {
                    Console.WriteLine(translation(allTextLines[j]));
                }

            }

            //sw.Flush();
            //sw.Close();
            Console.SetOut(oldOut);
            Console.WriteLine("保存完毕,在" + fileName + "warframeLogTranslation.txt");
            return "";
        }

        /// <summary>
        /// 菜单列表
        /// </summary>
        static void menu()
        {
            Console.WriteLine("-----------------");
            Console.WriteLine("菜单列表");
            Console.WriteLine("1.游戏网络检测");
            Console.WriteLine("2.翻译Log文件 - 不要重复使用");
            Console.WriteLine("执行功能后会清除屏幕");
            Console.WriteLine("-----------------");
            return;
        }

        /// <summary>
        /// 菜单选择
        /// </summary>
        /// <returns></returns>
        static int menuNumberInput()
        {
            Console.Write("输入操作编号:");
            bool isint = false;
            int i = -1;
            while (!(isint))
            {
                string str = Console.ReadLine();
                if (int.TryParse(str, out i) != true)
                {
                    menuError();
                }
                else
                {
                    isint = true;
                }
            }
            return i;
        }

        static void menuError()
        {
            setFontColorRed("\r\n没有找到该选项,请输入正确的数字!\r\n");
            return;
        }

        static void setFontColorRed(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        static void setFontColorGreen(string str)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
