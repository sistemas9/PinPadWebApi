using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace PinPadWebApi
{
  public class Program
  {
    [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern IntPtr GetStdHandle(int nStdHandle);
    [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern int AllocConsole();
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int STD_OUTPUT_HANDLE = -11;
    private const int MY_CODE_PAGE = 65001;
    private static bool showConsole = true; //Or false if you don't want to see the console
    //public static ContextMenu contextMenu1 = new ContextMenu();
    //public static MenuItem menuItem1 = new MenuItem();
    public static void Main(string[] args)
    {
      //contextMenu1.MenuItems.AddRange(new MenuItem[] { menuItem1});
      //menuItem1.Index = 0;
      //menuItem1.Text = "E&xit";
      //menuItem1.Click += new System.EventHandler(menuItem1_Click);
      //if (!showConsole)
      //{
      //  IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
      //  ShowWindow(h, 1);
      //  NotifyIcon icon = new NotifyIcon();
      //  icon.Icon = new System.Drawing.Icon("./atpicon.ico");
      //  icon.Visible = true;
      //  icon.BalloonTipText = "Hello from My Kitten";
      //  icon.BalloonTipTitle = "Cat Talk";
      //  icon.BalloonTipIcon = ToolTipIcon.Info;
      //  icon.ShowBalloonTip(2000);
      //  //icon.ContextMenu = contextMenu1;
      //  icon.Click += new EventHandler(menuItem1_Click);
      //}
      CreateHostBuilder(args).Build().Run();
    }

    private static void menuItem1_Click(object Sender, EventArgs e)
    {
      // Close the form, which closes the application.
      Console.WriteLine("123 por todos mis amigos y yo.........");
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>()
              .UseUrls("https://localhost:6001");
            });
  }
}
