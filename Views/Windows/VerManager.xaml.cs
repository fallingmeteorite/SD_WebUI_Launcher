using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Awake.Models;
using Awake.Views.Pages;
using Wpf.Ui.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace Awake.Views.Windows
{
    public class ExtTagItem
    {
        public string? Ext { get; set; }
        public string? Hash { get; set; }
        public string? Message { get; set; }
        public string? Date { get; set; }
        public string? Tag { get; set; }
    }
    /// <summary>
    /// VerManager.xaml 的交互逻辑
    /// </summary>
    public partial class VerManager : UiWindow
    {
        private string currExt;
        private string currHash;
        private string branch_name = "";
        private string git_path_use = "";
        private string working_directory = "";
        private string giturl;

        private List<ExtTagItem> tags;
        private List<CommitItem> commits;

        private ObservableCollection<CommitItem> CommiteCollection = new();
        private ObservableCollection<CommitItem> CommiteTagCollection = new();
     

        public VerManager(string giturl, string ext)
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

            this.currExt = ext;
            this.giturl = giturl;

            InitializeComponent();

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
            startInfo.Arguments = " log --oneline --pretty=\"%h^^%s^^%cd\" --date=format:\"%Y-%m-%d %H:%M:%S\" -n 1";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = ext;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string msg = process.StandardOutput.ReadToEnd();

            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = "  remote -v";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = ext;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string msg2 = process.StandardOutput.ReadToEnd();
            Debug.WriteLine(giturl);

            lblCurrHash.Content = msg.Split("^^")[0];
            lblCurrDate.Content = msg.Split("^^")[2];
            lblCurrMessage.Content = msg.Split("^^")[1];
            lblCurrGit.Content = msg2.Split("\\n")[0].Split(" ")[0];
            currHash = (string)lblCurrHash.Content;

            InitializeData(ext);

            commit.ItemsSource = CommiteCollection;
        }

        private void InitializeData(string ext)
        {
            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = $"  --no-pager log {branch_name} --pretty=\"%h^^%s^^%cd\" --date=format:\"%Y-%m-%d %H:%M:%S\" -n 150";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = ext;

            process.StartInfo = startInfo;

            int idx = 0;
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
            System.Windows.Controls.Button btn = (System.Windows.Controls.Button)sender;
            string hash = btn.Tag.ToString();

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = $" reset --hard {hash}"; //回退版本到
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = currExt;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();


            currHash = hash;

            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = " log --oneline --pretty=\"%h^^%s^^%cd\" --date=format:\"%Y-%m-%d %H:%M:%S\" -n 1";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = currExt;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string msg = process.StandardOutput.ReadToEnd();

            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.FileName = git_path_use;
            startInfo.Arguments = " remote -v";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = currExt;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string msg2 = process.StandardOutput.ReadToEnd();

            lblCurrHash.Content = msg.Split("^^")[0];
            lblCurrDate.Content = msg.Split("^^")[2];
            lblCurrMessage.Content = msg.Split("^^")[1];
            lblCurrGit.Content = msg2.Split("\\n")[0].Split(" ")[0];
            currHash = (string)lblCurrHash.Content;

            CommiteCollection.Clear();
            CommiteTagCollection.Clear();

            InitializeData(currExt);

            commit.ItemsSource = CommiteCollection;
            System.Windows.MessageBox.Show("安装完成,请继续操作");
        }

    }



}