using FluentTransitions;
using FluentTransitions.Methods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bootstrapperv2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label1.Location = new Point(12, -46);
            label2.Location = new Point(12, -46);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            Transition.RunChain
            (
            Transition
            .With(label2, nameof(Top), 54)
            .With(label1, nameof(Top), 9)
            .Build(new EaseInEaseOut(TimeSpan.FromSeconds(1)))
            );

            Transition.RunChain
            (
            Transition
            .With(this, nameof(Opacity), 1.0)
            .Build(new EaseInEaseOut(TimeSpan.FromSeconds(1))),
            Transition
            .With(guna2ProgressBar1, nameof(Top), 122)
            .With(guna2ProgressBar1, nameof(Left), 0)
            .With(guna2ProgressBar1, nameof(Width), 357)
            .HookOnCompletionInUiThread(SynchronizationContext.Current, () => finishanimating())
            .Build(new EaseInEaseOut(TimeSpan.FromSeconds(1)))
            );

        }

        private async void finishanimating()
        {
            try
            {
                label2.Text = "checking version...";
                WebClient wb = new WebClient();
                string version = wb.DownloadString("https://ghost-storage.7m.pl/ver.txt");

                if (int.Parse(version) > 16) //If the server replies with a int higher than 16 it will stop the launching
                {
                    DialogResult dialogResult = MessageBox.Show("Looks like your bootstrapper is outdated, do you want to join the discord?", "Outdated bootstrapper", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Process.Start("https://discord.gg/FnnHGWpgk8");
                    }
                    return;
                }
            }
            catch //Catches any error that may occur checking for updates
            {
                DialogResult dialogResult = MessageBox.Show("Couldn't check for updates! Do you want to join the discord?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (dialogResult == DialogResult.Yes)
                {
                    Process.Start("https://discord.gg/FnnHGWpgk8");
                }
                return;
            }
            guna2ProgressBar1.Value = 30;
            label2.Text = "Creating directories";
            if (!Directory.Exists("bin"))
            {

                Directory.CreateDirectory("bin");
            }

            if (!Directory.Exists("bin/Scripts"))
            {
                Directory.CreateDirectory("bin/Scripts");
            }

            if (!Directory.Exists("bin/lite"))
            {
                Directory.CreateDirectory("bin/lite");

            }

            label2.Text = "Downloading some files...";
            guna2ProgressBar1.Value = 50;
            await Task.Delay(2000);
            if (!File.Exists("bin/Ghost.exe.config"))
            {
                using (WebClient config = new WebClient())
                {
                    config.DownloadFile(new Uri("https://ghost-storage.7m.pl/Ghost.exe.config"), Application.StartupPath + "/bin/Ghost.exe.config");
                }

            }


            if (!File.Exists("bin/lite/FastColoredTextBox.dll"))
            {
                using (WebClient fastcolored = new WebClient())
                {
                    fastcolored.DownloadFile(new Uri("https://ghost-storage.7m.pl/FastColoredTextBox"), Application.StartupPath + "/bin/lite/FastColoredTextBox.dll");
                }
            }

            if (!File.Exists("bin/lite/FluxAPI.dll"))
            {
                using (WebClient fluxapi = new WebClient())
                {
                    fluxapi.DownloadFile(new Uri("https://ghost-storage.7m.pl/FluxAPI.dll"), Application.StartupPath + "/bin/lite/FluxAPI.dll");
                }
            }
            if (!File.Exists("bin/lite/ghost_lite.exe"))
            {
                using (WebClient ghlite = new WebClient())
                {
                    ghlite.DownloadFile(new Uri("https://ghost-storage.7m.pl/Ghost_lite"), Application.StartupPath + "/bin/lite/Ghost_lite.exe");
                }
            }
            label2.Text = "Finishing...";
            guna2ProgressBar1.Value = 80;
            launchghost();
            
        }

        private void launchghost()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                    wc.DownloadFile(new Uri("https://ghost-storage.7m.pl/Ghost2.1"), Application.StartupPath + "/bin/ghost.exe");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error occurred");
            }

            Completed();
        }

        private void Completed()
        {
            if (File.Exists("bin/ghost.exe"))
            {
                var startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = "bin";
                startInfo.FileName = "ghost.exe";
                Process proc = Process.Start(startInfo);
                guna2ProgressBar1.Value = 100;
                Transition
                .With(this, nameof(Opacity), 0.0)
                .With(label1, nameof(Top), -5)
                .With(label2, nameof(Top), -5)
                .With(guna2ProgressBar1, nameof(Width), 0)
                .With(guna2ProgressBar1, nameof(Left), 171)
                .With(guna2ProgressBar1, nameof(Top), 154)
                .HookOnCompletionInUiThread(SynchronizationContext.Current, () => Application.Exit())
                .EaseInEaseOut(TimeSpan.FromSeconds(0.5));
            }
            else
            {
                MessageBox.Show("The GHOST executable has been deleted, this is usually caused by an active antivirus","Executable missing");
            }
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //guna2ProgressBar1.Value = e.ProgressPercentage;
        }
    }
}
