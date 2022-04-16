using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using TinyJson;

namespace Installer
{
    public partial class InstallerWindow : Window
    {
        public InstallerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LocationBox.Text = "C:\\Program Files (x86)\\DiscordRP";
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    LocationBox.Text = fbd.SelectedPath + "\\DiscordRP";
                }
            }
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            // Download the data to find the latest version
            WebClient wc = new WebClient();
            wc.Headers.Add("User-Agent", "request");
            string json = wc.DownloadString("https://api.github.com/repos/ghostkiller967/DiscordRP/releases/latest");
            Release release = json.FromJson<Release>();

            // Check if the directory already exists
            if (!Directory.Exists(LocationBox.Text))
            {
                // Downloads the latest version
                Directory.CreateDirectory(LocationBox.Text);
                wc.DownloadFile(release.assets[0].browser_download_url, LocationBox.Text + "\\update.zip");

                // Installs the latest version
                Directory.CreateDirectory(LocationBox.Text + "\\Program");
                ZipFile.ExtractToDirectory(LocationBox.Text + "\\update.zip", LocationBox.Text + "\\Program");
                File.Delete(LocationBox.Text + "\\update.zip");

                // Starts the latest version
                Process.Start(new ProcessStartInfo(LocationBox.Text + "\\Program\\DiscordRP.exe")
                {
                    WorkingDirectory = LocationBox.Text + "\\Program"
                });
                Environment.Exit(0);
            }
            else
            {
                System.Windows.MessageBox.Show("The directory already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class Release
    {
        public string url { get; set; }
        public string assets_url { get; set; }
        public string upload_url { get; set; }
        public string html_url { get; set; }
        public int id { get; set; }
        public User author { get; set; }
        public string node_id { get; set; }
        public string tag_name { get; set; }
        public string target_commitish { get; set; }
        public string name { get; set; }
        public bool draft { get; set; }
        public bool prerelease { get; set; }
        public DateTime created_at { get; set; }
        public DateTime published_at { get; set; }
        public Asset[] assets { get; set; }
        public string tarball_url { get; set; }
        public string zipball_url { get; set; }
        public string body { get; set; }
    }

    public class Asset
    {
        public string url { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public User uploader { get; set; }
        public string content_type { get; set; }
        public string state { get; set; }
        public int size { get; set; }
        public int download_count { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string browser_download_url { get; set; }
    }

    public class User
    {
        public string login { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }
}
