using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using Awake.Models;
using Wpf.Ui.Common.Interfaces;
using Awake.Views.Windows;
using System.Windows.Controls;
using Awake.Services;
using System.Security.Policy;
using System.Threading;
using System.Windows.Interop;
using System.Text.RegularExpressions;


namespace Awake.Views.Pages
{
    public partial class Code
    {
        public string currHash;
        public List<CommitItem> commits;
        public int number_add = 30;
        public int check_add = 0;
        public string branch_name = "";
        public string working_directory = "";
        public string git_path_use = "";
        public ObservableCollection<CommitItem> CommiteCollection = new();

        public Code()
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

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = " rev-parse --abbrev-ref HEAD";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = working_directory;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            branch_name = process.StandardOutput.ReadToEnd();
            string strRet = "";
            MatchCollection results = Regex.Matches(branch_name, "[A-Za-z0-9]");
            foreach (var v in results)
            {
                strRet += v.ToString();
            }
            branch_name = strRet;

            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = $" log --oneline --pretty=\"%h^^%s^^%cd\" --date=format:\"%Y-%m-%d %H:%M:%S\" -n 1";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = working_directory;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string load_info_01 = process.StandardOutput.ReadToEnd();
            currHash = load_info_01.Split("^^")[0];


            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = " remote -v";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = working_directory;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string load_info_02 = process.StandardOutput.ReadToEnd();

            CommiteCollection.Clear();
            InitializeData(25);
            InitializeComponent();

            lblCurrHash.Content = currHash;
            lblCurrDate.Content = load_info_01.Split("^^")[2];
            lblCurrMessage.Content = load_info_01.Split("^^")[1];
            lblCurrGit.Content = load_info_02.Split("\\n")[0].Split(" ")[0];

            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = $" log --oneline origin/{branch_name} --pretty=\"%h^^%s^^%cd\" --date=format:\"%Y-%m-%d %H:%M:%S\" -n 1";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = working_directory;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string load_info_03 = process.StandardOutput.ReadToEnd();
            currHash = load_info_03.Split("^^")[0];

            commit.ItemsSource = CommiteCollection;
        }




        private void InitializeData(int setting_show)
        {

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = $" --no-pager log origin/{branch_name} --pretty=\"%h^^%s^^%cd\" --date=format:\"%Y-%m-%d %H:%M:%S\" -n 1000";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = working_directory;

            process.StartInfo = startInfo;

            int idx = 0;
            int number_show = 0;
            commits = new List<CommitItem>();
            process.ErrorDataReceived += new DataReceivedEventHandler(delegate (object sender, DataReceivedEventArgs e)
            {

            });
            process.OutputDataReceived += new DataReceivedEventHandler(delegate (object sender, DataReceivedEventArgs e)
            {
                if (e.Data == null) return;
                CommitItem item1 = new CommitItem();
                string[] itemarr = e.Data.Split("^^");
                if (itemarr.Length < 3)
                {
                    return;
                }

                if (number_show != setting_show)
                {
                    number_show += 1;
                    item1.Hash = itemarr[0];
                    item1.Message = itemarr[1];
                    item1.Date = itemarr[2];
                    item1.Id = idx++;
                    item1.use_start = true;
                    item1.Checked = false;


                    if (currHash == item1.Hash)
                    {
                        item1.use_start = false;
                        item1.Checked = true;
                    }

                    commits.Add(item1);
                }


            });

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();

            for (int i = 0; i < commits.Count(); i++)
            {
                CommiteCollection.Add(commits[i]);
            }

        }



        private void setup_Click(object sender, RoutedEventArgs e)
        {

            Button btn = (Button)sender;
            string hash = btn.Tag.ToString();

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = $" reset --hard {hash}"; //回退版本到
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = working_directory;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
         
            CommiteCollection.Clear();

            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = $" log --oneline --pretty=\"%h^^%s^^%cd\" --date=format:\"%Y-%m-%d %H:%M:%S\" -n 1";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = working_directory;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string msg = process.StandardOutput.ReadToEnd();
            currHash = msg.Split("^^")[0];

            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = "  remote -v";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = working_directory;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string msg2 = process.StandardOutput.ReadToEnd();

            CommiteCollection.Clear();
            InitializeData(25);
            InitializeComponent();

            lblCurrHash.Content = currHash;
            lblCurrDate.Content = msg.Split("^^")[2];
            lblCurrMessage.Content = msg.Split("^^")[1];
            lblCurrGit.Content = msg2.Split("\\n")[0].Split(" ")[0];

            commit.ItemsSource = CommiteCollection;
            System.Windows.MessageBox.Show("安装完成,请继续操作");
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommiteCollection.Clear();

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = $" log --oneline --pretty=\"%h^^%s^^%cd\" --date=format:\"%Y-%m-%d %H:%M:%S\" -n 1";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = working_directory;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string msg = process.StandardOutput.ReadToEnd();
            currHash = msg.Split("^^")[0];

            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = " remote -v";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = working_directory;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string msg2 = process.StandardOutput.ReadToEnd();

            CommiteCollection.Clear();
            InitializeData(25);
            InitializeComponent();

            lblCurrHash.Content = currHash;
            lblCurrDate.Content = msg.Split("^^")[2];
            lblCurrMessage.Content = msg.Split("^^")[1];
            lblCurrGit.Content = msg2.Split("\\n")[0].Split(" ")[0];

            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = $" log --oneline origin/{branch_name} --pretty=\"%h^^%s^^%cd\" --date=format:\"%Y-%m-%d %H:%M:%S\" -n 1";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = working_directory;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            msg = process.StandardOutput.ReadToEnd();
            currHash = msg.Split("^^")[0];

            commit.ItemsSource = CommiteCollection;

        }
        private void UpdateCode_Click(object sender, RoutedEventArgs e)
        {
            Process process2 = new Process();
            ProcessStartInfo startInfo2 = new ProcessStartInfo();
            startInfo2.FileName = git_path_use;
            startInfo2.Arguments = " pull ";
            startInfo2.UseShellExecute = true;
            startInfo2.RedirectStandardOutput = false;
            startInfo2.CreateNoWindow = false;
            startInfo2.WorkingDirectory = working_directory;

            process2.StartInfo = startInfo2;
            process2.Start();
            process2.WaitForExit();

            btnUpdateCode.IsEnabled = false;

            CommiteCollection.Clear();
            InitializeData(25);
            InitializeComponent();

            commit.ItemsSource = CommiteCollection;
        }

        private void DataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {


            if (check_add == 0)
            {
                check_add += 1;
                var scrollViewer = e.OriginalSource as ScrollViewer;
                if (e.VerticalOffset != 0 && e.VerticalOffset == scrollViewer.ScrollableHeight)
                {

                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = git_path_use;
                    startInfo.Arguments = $" log --oneline --pretty=\"%h^^%s^^%cd\" --date=format:\"%Y-%m-%d %H:%M:%S\" -n 1";
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = false;
                    startInfo.CreateNoWindow = true;
                    startInfo.WorkingDirectory = working_directory;

                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();

                    string msg = process.StandardOutput.ReadToEnd();
                    currHash = msg.Split("^^")[0];

                    process = new Process();
                    startInfo = new ProcessStartInfo();
                    startInfo.FileName = git_path_use;
                    startInfo.Arguments = " remote -v";
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = false;
                    startInfo.CreateNoWindow = true;
                    startInfo.WorkingDirectory = working_directory;

                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();

                    string msg2 = process.StandardOutput.ReadToEnd();


                    number_add += 10;
                    CommiteCollection.Clear();
                    InitializeData(number_add);
                    InitializeComponent();

                    lblCurrHash.Content = currHash;
                    lblCurrDate.Content = msg.Split("^^")[2];
                    lblCurrMessage.Content = msg.Split("^^")[1];
                    lblCurrGit.Content = msg2.Split("\\n")[0].Split(" ")[0];

                    process = new Process();
                    startInfo = new ProcessStartInfo();
                    startInfo.FileName = git_path_use;
                    startInfo.Arguments = $" log --oneline origin/{branch_name} --pretty=\"%h^^%s^^%cd\" --date=format:\"%Y-%m-%d %H:%M:%S\" -n 1";
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = false;
                    startInfo.CreateNoWindow = true;
                    startInfo.WorkingDirectory = working_directory;

                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();

                    msg = process.StandardOutput.ReadToEnd();
                    currHash = msg.Split("^^")[0];

                    commit.ItemsSource = CommiteCollection;
                }
            }

            var scrollViewer1 = e.OriginalSource as ScrollViewer;
            if (e.VerticalOffset != 0 && e.VerticalOffset != scrollViewer1.ScrollableHeight)
            {
                check_add = 0;
            }

        }

    }
    public class TagItem
    {
        public string? Hash { get; set; }

        public string? Message { get; set; }

        public string? Date { get; set; }
    }

}