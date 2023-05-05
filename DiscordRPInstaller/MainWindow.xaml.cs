using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Windows;
using System.Windows.Forms;
using TinyJson;

namespace DiscordRPInstaller
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fd = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            })
            {
                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    LocationBox.Text = fd.SelectedPath + "\\DiscordRP";
                }
            }
        }

        private async void Install_Click(object sender, RoutedEventArgs e)
        {
            // Download the data to find the latest version
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "DiscordRP Installer");
            HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/ghostkiller967/DiscordRP/releases/latest");
            string json = await response.Content.ReadAsStringAsync();
            Dictionary<string, object> release = json.FromJson<Dictionary<string, object>>();

            // Check if the directory already exists
            if (Directory.Exists(LocationBox.Text))
                Directory.Delete(LocationBox.Text, true);

            // Downloads the latest version
            Directory.CreateDirectory(LocationBox.Text);

            // Download the first asset from the assets list
            List<object> assets = (List<object>)release["assets"];
            Dictionary<string, object> asset = (Dictionary<string, object>)assets[0];

            response = await client.GetAsync((string)asset["browser_download_url"]);
            using (FileStream fs = new FileStream(LocationBox.Text + "\\files.zip", FileMode.CreateNew))
            {
                await response.Content.CopyToAsync(fs);
            }

            // Installs the latest version
            ZipFile.ExtractToDirectory(LocationBox.Text + "\\files.zip", LocationBox.Text);
            File.Delete(LocationBox.Text + "\\files.zip");

            // Starts the latest version
            Process.Start(new ProcessStartInfo(LocationBox.Text + "\\DiscordRP.exe")
            {
                WorkingDirectory = LocationBox.Text
            });
            Environment.Exit(0);
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
