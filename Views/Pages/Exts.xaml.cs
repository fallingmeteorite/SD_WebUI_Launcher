using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Awake.Models;
using Awake.Services;
using Awake.Views.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace Awake.Views.Pages
{
    public partial class Exts
    {
        public string working_directory = "";
        public string git_path_use = "";

        public Exts()
        {
            if (initialize.enable_custom_path)
            {
                working_directory = initialize.本地路径;
                git_path_use = initialize.git_path;
                if (!File.Exists(git_path_use))
                {
                    System.Windows.MessageBox.Show("自定义GIT路径错误或未选择，程序错误即将关闭！");
                    Process.GetCurrentProcess().Kill();
                }
            }
            else
            {
                working_directory = initialize.工作路径;
                git_path_use = initialize.工作路径 + @"\GIT\mingw64\bin\git.exe";
                if (!File.Exists(git_path_use))
                {
                    System.Windows.MessageBox.Show("工作路径下即整合包未存在GIT，程序错误即将关闭！");
                    Process.GetCurrentProcess().Kill();
                }

            }

            InitializeComponent();
            Init.InitExtData();
            exts.ItemsSource = Store.extLocal;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Explorer.exe", working_directory + @"\extensions");
        }
        private void checkUpdateExt_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = " reset --hard";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = (string)btn.Tag;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = " pull";
            startInfo.UseShellExecute = true;
            startInfo.RedirectStandardOutput = false;
            startInfo.CreateNoWindow = false;
            startInfo.WorkingDirectory = (string)btn.Tag;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            Init.InitExtData();
            exts.ItemsSource = Store.extLocal;
            System.Windows.MessageBox.Show("安装完成,请继续操作");

        }
        private void openExt_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string ext = btn.Tag.ToString();
            Process.Start("Explorer.exe", ext);
        }
        private void Setup_Click(object sender, RoutedEventArgs e)
        {
            Awake.Views.Windows.ExtManager ext = new Awake.Views.Windows.ExtManager();
            ext.Show();
        }
        private void verManager_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int idx = int.Parse(btn.Tag.ToString());

            VerManager ma = new VerManager(Store.extLocal[idx].GitUrl, Store.extLocal[idx].Path);
            ma.Show();

        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Init.InitExtData();
            exts.ItemsSource = Store.extLocal;
        }
    }
    public class ExtRemote
    {
        public string name { get; set; }
        public string hash { get; set; }
        public string date { get; set; }
        public string url { get; set; }
    }
}
