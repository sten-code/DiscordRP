using DiscordRPC;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace DiscordRP
{
    public partial class MainWindow : Window
    {
        public DiscordRpcClient Client;
        public WebClient WebClient;

        private NotifyIcon Notify;
        private System.Timers.Timer AssetRetrieveLoop = new System.Timers.Timer(60000);
        private MediaElement VideoPlayer;
        private Border VideoHolder;

        public MainWindow()
        {
            InitializeComponent();
            WebClient = new WebClient();
            WebClient.Headers["User-Agent"] = "DiscordRP WebClient";

            Notify = new NotifyIcon
            {
                Icon = Properties.Resources.Discord,
                Text = "DiscordRP",
                Visible = true,
                ContextMenu = new System.Windows.Forms.ContextMenu
                {
                    MenuItems =
                    {
                        new System.Windows.Forms.MenuItem("Open", OpenMenuItem_Click),
                        new System.Windows.Forms.MenuItem("Run on Startup", RunOnStartupMenuItem_Click),
                        new System.Windows.Forms.MenuItem("Exit", ExitMenuItem_Click)
                    }
                }
            };

            CreateShortcut();
        }

        public void CheckUpdates()
        {
            string json = WebClient.DownloadString("https://api.github.com/repos/sten-code/DiscordRP/releases/latest");
            Dictionary<string, object> repo = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            Version currentVersion = Version.Parse(System.Windows.Forms.Application.ProductVersion);
            Version latestVersion = Version.Parse((string)repo["tag_name"]);
            Debug.WriteLine("Current version: " + currentVersion);
            Debug.WriteLine("Latest version: " + latestVersion);
            if (currentVersion.CompareTo(latestVersion) < 0)
            {
                if (System.Windows.MessageBox.Show("New update detected, do you want to update?", "Update", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    List<Dictionary<string, object>> assets = ((JArray)repo["assets"]).ToObject<List<Dictionary<string, object>>>();
                    string downloadUrl = (string)assets[0]["browser_download_url"];
                    Debug.WriteLine(downloadUrl);
                    if (!System.IO.File.Exists(Config.UpdaterPath))
                    {
                        System.IO.File.WriteAllBytes(Config.UpdaterPath, Properties.Resources.DiscordRPUpdater);
                    }
                    Process updater = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = Config.UpdaterPath,
                            Arguments = $"\"{downloadUrl}\" \"{Environment.CurrentDirectory}\" \"{Process.GetCurrentProcess().Id}\""
                        }
                    };
                    updater.Start();
                }
            }
        }
        
        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void RunOnStartupMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MenuItem item = (System.Windows.Forms.MenuItem)sender;
            item.Checked = !item.Checked;
            SaveSettings();
            CreateShortcut();
        }

        public void CreateShortcut()
        {
            System.Windows.Forms.MenuItem item = Notify.ContextMenu.MenuItems[1];
            string linkPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\DiscordRP.lnk";
            if (item.Checked && !System.IO.File.Exists(linkPath))
            {
                IWshShortcut wshShortcut = (IWshShortcut)new WshShell().CreateShortcut(linkPath);
                wshShortcut.Description = "Custom Discord Rich Presence";
                wshShortcut.TargetPath = Environment.CurrentDirectory + "\\DiscordRP.exe";
                wshShortcut.WorkingDirectory = Environment.CurrentDirectory + "\\";
                wshShortcut.Save();
            }
            else if (!item.Checked && System.IO.File.Exists(linkPath))
            {
                System.IO.File.Delete(linkPath);
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Notify.Dispose();
            Environment.Exit(0);
        }

        public string LoadSettings()
        {
            string clientId = "";

            Dispatcher.Invoke(() =>
            {
                if (System.IO.File.Exists(Config.SettingsPath))
                {
                    Settings settings = JsonConvert.DeserializeObject<Settings>(System.IO.File.ReadAllText(Config.SettingsPath));

                    ClientIDBox.Text = settings.ClientID;
                    DetailsBox.Text = settings.Details;
                    StateBox.Text = settings.State;
                    PartySizeBox.Text = settings.PartySize;
                    PartyMaxBox.Text = settings.PartyMax;
                    TimestampBox.SelectedIndex = settings.TimestampIndex;
                    LargeImageKeyBox.Tag = settings.LargeImageKey;
                    LargeImageTextBox.Tag = settings.LargeImageText;
                    SmallImageKeyBox.Text = settings.SmallImageKey;
                    SmallImageTextBox.Text = settings.SmallImageText;
                    Button1TextBox.Text = settings.Button1Text;
                    Button1UrlBox.Text = settings.Button1Url;
                    Button2TextBox.Text = settings.Button2Text;
                    Button2UrlBox.Text = settings.Button2Url;
                    Notify.ContextMenu.MenuItems[1].Checked = settings.RunOnStartup;
                    clientId = settings.ClientID;
                }
                else
                {
                    SaveSettings();
                }
            });
            return clientId;
        }

        public void SaveSettings()
        {
            Settings settings = new Settings
            {
                ClientID = ClientIDBox.Text,
                Details = DetailsBox.Text,
                State = StateBox.Text,
                PartySize = PartySizeBox.Text,
                PartyMax = PartyMaxBox.Text,
                TimestampIndex = TimestampBox.SelectedIndex,
                LargeImageKey = LargeImageKeyBox.Text,
                LargeImageText = LargeImageTextBox.Text,
                SmallImageKey = SmallImageKeyBox.Text,
                SmallImageText = SmallImageTextBox.Text,
                Button1Text = Button1TextBox.Text,
                Button1Url = Button1UrlBox.Text,
                Button2Text = Button2TextBox.Text,
                Button2Url = Button2UrlBox.Text,
                RunOnStartup = Notify.ContextMenu.MenuItems[1].Checked
            };
            string serialized = JsonConvert.SerializeObject(settings);
            FileInfo fileInfo = new FileInfo(Config.SettingsPath);
            if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();
            System.IO.File.WriteAllText(Config.SettingsPath, serialized);
        }

        private void AssetRetrieveLoop_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (!LargeImageKeyBox.IsDropDownOpen && !SmallImageKeyBox.IsDropDownOpen)
                {
                    Dispatcher.Invoke(() =>
                    {
                        LoadAssets();
                        SaveSettings();
                    });
                }
            });
        }

        public void LoadAssets()
        {
            string largeSelected = LargeImageKeyBox.Text;
            string smallSelected = SmallImageKeyBox.Text;
            LargeImageKeyBox.Items.Clear();
            SmallImageKeyBox.Items.Clear();
            LargeImageKeyBox.Items.Add("None");
            SmallImageKeyBox.Items.Add("None");
            string json = WebClient.DownloadString($"https://discordapp.com/api/oauth2/applications/{Client.ApplicationID}/assets");
            List<ImageAssets> response = JsonConvert.DeserializeObject<List<ImageAssets>>(json);
            foreach (ImageAssets asset in response)
            {
                LargeImageKeyBox.Items.Add(asset.Name);
                SmallImageKeyBox.Items.Add(asset.Name);
            }
            LargeImageKeyBox.Text = largeSelected;
            SmallImageKeyBox.Text = smallSelected;
        }

        public bool IsIdValid(string id)
        {
            try
            {
                string json = WebClient.DownloadString($"https://discordapp.com/api/oauth2/applications/{id}/assets");
                return json[0] != '{';
            }
            catch
            {
                return false;
            }
        }

        public void Connect(string id)
        {
            if (!IsIdValid(id))
            {
                ConnectButton.Content = "Connect";
                DetailsBox.IsEnabled = false;
                StateBox.IsEnabled = false;
                PartySizeBox.IsEnabled = false;
                PartyMaxBox.IsEnabled = false;
                TimestampBox.IsEnabled = false;
                LargeImageKeyBox.IsEnabled = false;
                LargeImageTextBox.IsEnabled = false;
                SmallImageKeyBox.IsEnabled = false;
                SmallImageTextBox.IsEnabled = false;
                Button1TextBox.IsEnabled = false;
                Button1UrlBox.IsEnabled = false;
                Button2TextBox.IsEnabled = false;
                Button2UrlBox.IsEnabled = false;
                Notify.ContextMenu.MenuItems[1].Checked = false;
                return;
            }

            ClientIDBox.Text = id;
            if (Client != null && !Client.IsDisposed)
            {
                Client.Dispose();
            }

            Client = new DiscordRpcClient(id);
            LoadAssets();
            LargeImageKeyBox.Text = (string)LargeImageKeyBox.Tag;
            SmallImageKeyBox.Text = (string)SmallImageKeyBox.Tag;

            DetailsBox.IsEnabled = true;
            StateBox.IsEnabled = true;
            PartySizeBox.IsEnabled = true;
            PartyMaxBox.IsEnabled = true;
            TimestampBox.IsEnabled = true;
            LargeImageKeyBox.IsEnabled = true;
            LargeImageTextBox.IsEnabled = true;
            SmallImageKeyBox.IsEnabled = true;
            SmallImageTextBox.IsEnabled = true;
            Button1TextBox.IsEnabled = true;
            Button1UrlBox.IsEnabled = true;
            Button2TextBox.IsEnabled = true;
            Button2UrlBox.IsEnabled = true;
            ConnectButton.Content = "Reconnect";
            SetPresence();

            Client.Initialize();
        }

        public void Disconnect()
        {
            if (!Client.IsDisposed)
            {
                Client.Dispose();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            SetPresence();
        }

        public void SetPresence()
        {
            RichPresence presence = new RichPresence
            {
                Details = DetailsBox.Text,
                State = StateBox.Text,
                Assets = new Assets
                {
                    LargeImageKey = LargeImageKeyBox.Text == "None" ? "" : LargeImageKeyBox.Text,
                    LargeImageText = LargeImageTextBox.Text,
                    SmallImageKey = SmallImageKeyBox.Text == "None" ? "" : SmallImageKeyBox.Text,
                    SmallImageText = SmallImageTextBox.Text
                },
            };

            // Add a party size only if both boxes are filled in
            if (PartySizeBox.Text != "" && PartyMaxBox.Text != "")
            {
                // values must be an int, if not, don't add it
                if (int.TryParse(PartySizeBox.Text, out int partySize) && int.TryParse(PartyMaxBox.Text, out int partyMax))
                {
                    presence.Party = new Party
                    {
                        ID = "DiscordRP",
                        Size = partySize,
                        Max = partyMax
                    };
                }
            }

            // Add buttons to the rich presence, only if both boxes are filled in.
            List<DiscordRPC.Button> buttons = new List<DiscordRPC.Button>();
            if (Button1TextBox.Text != "" && Button1UrlBox.Text != "")
            {
                try
                {
                    buttons.Add(new DiscordRPC.Button()
                    {
                        Label = Button1TextBox.Text,
                        Url = Button1UrlBox.Text
                    });
                } 
                catch (Exception ex)
                {
                    // The filled in boxes weren't valid, most likely because of an invalid uri
                    Debug.WriteLine(ex.Message);
                }
            }

            if (Button2TextBox.Text != "" && Button2UrlBox.Text != "")
            {
                try
                {
                    buttons.Add(new DiscordRPC.Button()
                    {
                        Label = Button2TextBox.Text,
                        Url = Button2UrlBox.Text
                    });
                }
                catch (Exception ex)
                {
                    // The filled in boxes weren't valid, most likely because of an invalid uri
                    Debug.WriteLine(ex.Message);
                }
            }
            presence.Buttons = buttons.ToArray();

            // If index 0 (None in the combobox) is chosen, don't add a timestamp
            switch (TimestampBox.SelectedIndex)
            {
                case 1:
                    presence.Timestamps = new Timestamps(DateTime.UtcNow.Subtract(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)));
                    break;
                case 2:
                    presence.Timestamps = new Timestamps(DateTime.UtcNow.Subtract(UpTime));
                    break;
                default:
                    break;
            }

            Client.SetPresence(presence);
        }

        public TimeSpan UpTime
        {
            get
            {
                using (PerformanceCounter pc = new PerformanceCounter("System", "System Up Time"))
                {
                    pc.NextValue();
                    return TimeSpan.FromSeconds(pc.NextValue());
                }
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            Connect(ClientIDBox.Text);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveSettings();
            Notify.Dispose();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckUpdates();

            string clientId = LoadSettings();
            Connect(clientId);

            AssetRetrieveLoop.Elapsed += AssetRetrieveLoop_Elapsed;
            AssetRetrieveLoop.Start();
        }

    }

    public class Settings
    {
        public string ClientID { get; set; }
        public string Details { get; set; }
        public string State { get; set; }
        public string PartySize { get; set; }
        public string PartyMax { get; set; }
        public int TimestampIndex { get; set; }

        public string LargeImageKey { get; set; }
        public string LargeImageText { get; set; }
        public string SmallImageKey { get; set; }
        public string SmallImageText { get; set; }

        public string Button1Text { get; set; }
        public string Button1Url { get; set; } 
        public string Button2Text { get; set; }
        public string Button2Url { get; set; }

        public bool RunOnStartup { get; set; }
    }

    public class ImageAssets
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
