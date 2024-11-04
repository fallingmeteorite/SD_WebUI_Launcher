using Awake.Views.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static Microsoft.Web.WebView2.Core.DevToolsProtocolExtension.CSS;

namespace Awake.Views.Pages
{
    public partial class DashboardPage/* : INavigableView<ViewModels.DashboardViewModel>*/
    {
        public ViewModels.DashboardViewModel ViewModel
        {
            get;
        }

        string _searchName = "";//搜索名称
        int 模型排列 = 0;//模型排列优先级状态量
        string 模型种类 = "[]";//默认所有模型种类
        int 加载页数 = 1;//第一页的模型
        private string 模型名称 = string.Empty;
        private string 模型封面地址 = string.Empty;
        private string 模型作者名称 = string.Empty;
        private string 模型作者头像地址 = string.Empty;
        private string 模型种类名称 = string.Empty;
        private string 模型UUID = string.Empty;


        private readonly List<string> _imagePaths;
        private DispatcherTimer _timer;
        private Random _random = new Random();

        public DashboardPage(ViewModels.DashboardViewModel viewModel)
        {
            try
            {
                ViewModel = viewModel;
                InitializeComponent();
                _imagePaths = new List<string> {"/img/001.png"};

            }
            catch (Exception error)
            {
                File.WriteAllText(@".\logs\error.txt", error.Message.ToString());
            }

        }
     
        private void 一键启动按钮_Click(object sender, RoutedEventArgs e)
        {

            shell WindowMain = new shell();//运行终端
            WindowMain.Show();

        }
      
        string programpath = System.Windows.Forms.Application.StartupPath.Substring(0, System.Windows.Forms.Application.StartupPath.IndexOf(':'));
        //获取执行文件所在盘符

       
        private void 打开WebUI文件夹_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", initialize.工作路径);
        }

        private void 打开文生图文件夹_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (programpath.Length == 1)
            {
                string 文生图文件路径 = initialize.工作路径 + "outputs\\txt2img-images";
                Process.Start("explorer.exe", 文生图文件路径);
            }
            else
            {
                string 文生图文件路径 = initialize.工作路径 + "\\outputs\\txt2img-images";
                Process.Start("explorer.exe", 文生图文件路径);
            }
        }

        private void 打开图生图文件夹_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (programpath.Length == 1)
            {
                string 图生图文件路径 = initialize.工作路径 + "outputs\\img2img-images";
                Process.Start("explorer.exe", 图生图文件路径);
            }
            else
            {
                string 图生图文件路径 = initialize.工作路径 + "\\outputs\\img2img-images";
                Process.Start("explorer.exe", 图生图文件路径);
            }

        }

        private void 统计生成图片数量_MouseDown(object sender, MouseButtonEventArgs e)
        {
            initialize.相册计数();
            图片数量展示.Text = initialize.相册图片数量;
        }

        private void 打开SD模型文件夹_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (programpath.Length == 1)
            {
                string SD模型文件路径 = initialize.工作路径 + "models\\Stable-diffusion";
                Process.Start("explorer.exe", SD模型文件路径);
            }
            else
            {
                string SD模型文件路径 = initialize.工作路径 + "\\models\\Stable-diffusion";
                Process.Start("explorer.exe", SD模型文件路径);
            }

        }

        private void 打开lora模型文件夹_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (programpath.Length == 1)
            {
                string LORA模型文件路径 = initialize.工作路径 + "models\\Lora";
                Process.Start("explorer.exe", LORA模型文件路径);
            }
            else
            {
                string LORA模型文件路径 = initialize.工作路径 + "\\models\\Lora";
                Process.Start("explorer.exe", LORA模型文件路径);
            }

        }

        private void 打开VAE模型文件夹_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (programpath.Length == 1)
            {
                string VAE模型文件路径 = initialize.工作路径 + "models\\VAE";
                Process.Start("explorer.exe", VAE模型文件路径);
            }
            else
            {
                string VAE模型文件路径 = initialize.工作路径 + "\\models\\VAE";
                Process.Start("explorer.exe", VAE模型文件路径);
            }


        }

        private void 打开EMB模型文件夹_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (programpath.Length == 1)
            {
                string EMB模型文件路径 = initialize.工作路径 + "embeddings";
                Process.Start("explorer.exe", EMB模型文件路径);
            }
            else
            {
                string EMB模型文件路径 = initialize.工作路径 + "\\embeddings";
                Process.Start("explorer.exe", EMB模型文件路径);
            }


        }

        private void 打开HYP模型文件夹_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (programpath.Length == 1)
            {
                string HYP模型文件路径 = initialize.工作路径 + "models\\hypernetworks";
                Process.Start("explorer.exe", HYP模型文件路径);
            }
            else
            {
                string HYP模型文件路径 = initialize.工作路径 + "\\models\\hypernetworks";
                Process.Start("explorer.exe", HYP模型文件路径);
            }


        }

        private void 打开扩展模型文件夹_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (programpath.Length == 1)
            {
                string 扩展模型文件夹 = initialize.工作路径 + "extensions";
                Process.Start("explorer.exe", 扩展模型文件夹);
            }
            else
            {
                string 扩展模型文件夹 = initialize.工作路径 + "\\extensions";
                Process.Start("explorer.exe", 扩展模型文件夹);
            }


        }

        private void 总滚动列表_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            
        }

    }

}