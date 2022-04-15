using DiscordRPC;
using IWshRuntimeLibrary;
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace DiscordRP
{
    public partial class MainWindow : Window
    {
        public DiscordRpcClient Client;
        public GitHubClient GithubClient;

        private NotifyIcon Notify;
        private System.Timers.Timer Loop = new System.Timers.Timer(60000);
        private System.Timers.Timer AnimationLoop = new System.Timers.Timer(10);
        private MediaElement VideoPlayer;
        private Border VideoHolder;

        public MainWindow()
        {
            InitializeComponent();
            GithubClient = new GitHubClient(new ProductHeaderValue("DiscordRP"));
            CheckUpdates();
            StartAnimation();
            Notify = new NotifyIcon();
            Notify.Icon = Properties.Resources.Discord;
            Notify.Text = "DiscordRP";
            System.Windows.Forms.ContextMenu context = new System.Windows.Forms.ContextMenu();
            context.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
            {
                new System.Windows.Forms.MenuItem("Open", OpenMenuItem_Click),
                new System.Windows.Forms.MenuItem("Run on Startup", RunOnStartupMenuItem_Click),
                new System.Windows.Forms.MenuItem("Exit", ExitMenuItem_Click)
            });
            Notify.ContextMenu = context;
            Notify.Visible = true;
            
            AnimationLoop.Elapsed += Animation_Elapsed;
            Loop.Elapsed += Loop_Elapsed;
            Loop.Start();

            string clientId = LoadSettings();
            if (clientId != "")
            {
                Connect(clientId);
            }
            else
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
                StopAnimation();
            }

            Startup();
        }

        public async void CheckUpdates()
        {
            IReadOnlyList<Release> releases;

            try
            {
                releases = await GithubClient.Repository.Release.GetAll("ghostkiller967", "DiscordRP");
            }
            catch
            {
                System.Windows.MessageBox.Show("There was an error fetching new updates, maybe check your internet connection");
                return;
            }

            Release latest = releases[0];

            Version currentVersion = Version.Parse(System.Windows.Forms.Application.ProductVersion);
            Version latestVersion = Version.Parse(latest.TagName);

            if (currentVersion.CompareTo(latestVersion) < 0)
            {
                if (System.Windows.MessageBox.Show("New update detected, do you want to update?", "Update", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (System.IO.File.Exists(Directory.GetParent(Environment.CurrentDirectory) + "\\update.zip"))
                    {
                        System.IO.File.Delete(Directory.GetParent(Environment.CurrentDirectory) + "\\update.zip");
                    }
                    new WebClient().DownloadFile(latest.Assets[0].BrowserDownloadUrl, Directory.GetParent(Environment.CurrentDirectory) + "\\update.zip");

                    string script = $"Stop-Process -Name \"{System.Windows.Forms.Application.ProductName}\"\nStart-Sleep -Seconds 2\nRemove-Item -LiteralPath \"{Environment.CurrentDirectory}\" -Recurse -Force -Confirm:$false\nNew-Item -ItemType Directory -Force -Path \"{Environment.CurrentDirectory}\"\nExpand-Archive -LiteralPath \"{Directory.GetParent(Environment.CurrentDirectory) + "\\update.zip"}\" -DestinationPath \"{Environment.CurrentDirectory}\"\nStart-Process -FilePath \"{Environment.CurrentDirectory}\\DiscordRP.exe\" -WorkingDirectory \"{Environment.CurrentDirectory}\"";
                    System.IO.File.WriteAllText(Directory.GetParent(Environment.CurrentDirectory).FullName + "\\install.ps1", script);

                    Process process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            Arguments = $"\"&'{Directory.GetParent(Environment.CurrentDirectory).FullName + "\\install.ps1"}'\"",
                            WorkingDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
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
            Startup();
        }

        public void Startup()
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
                string path = Directory.GetParent(Environment.CurrentDirectory).FullName + "\\settings.json";
                if (System.IO.File.Exists(path))
                {
                    Settings settings = JsonConvert.DeserializeObject<Settings>(System.IO.File.ReadAllText(path));

                    ClientIDBox.Text = settings.ClientID;
                    DetailsBox.Text = settings.Details;
                    StateBox.Text = settings.State;
                    PartySizeBox.Text = settings.PartySize;
                    PartyMaxBox.Text = settings.PartyMax;
                    TimestampBox.SelectedIndex = settings.TimestampIndex;
                    LargeImageKeyBox.SelectedItem = settings.LargeImageKey;
                    LargeImageTextBox.Text = settings.LargeImageText;
                    SmallImageKeyBox.SelectedItem = settings.SmallImageKey;
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
            string path = Directory.GetParent(Environment.CurrentDirectory).FullName + "\\settings.json";

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
            string serialized = JsonConvert.SerializeObject(settings, Formatting.Indented);
            System.IO.File.WriteAllText(path, serialized);
        }

        private void Loop_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!LargeImageKeyBox.IsDropDownOpen && !SmallImageKeyBox.IsDropDownOpen)
            {
                Dispatcher.Invoke(() =>
                {
                    LoadAssets();
                    SaveSettings();
                });
            }
        }

        public void LoadAssets()
        {
            LargeImageKeyBox.Items.Clear();
            SmallImageKeyBox.Items.Clear();
            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString($"https://discordapp.com/api/oauth2/applications/{Client.ApplicationID}/assets");
                List<ImageAssets> response = JsonConvert.DeserializeObject<List<ImageAssets>>(json);
                response.ForEach(asset =>
                {
                    LargeImageKeyBox.Items.Add(asset.Name);
                    SmallImageKeyBox.Items.Add(asset.Name);
                });
            }
        }

        public void Connect(string id)
        {
            ClientIDBox.Text = id;
            if (Client != null)
            {
                if (!Client.IsDisposed)
                {
                    Client.Dispose();
                }
            }

            Client = new DiscordRpcClient(id);
            Client.OnReady += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
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
                    LoadAssets();
                    SetPresence();
                    StopAnimation();
                });
            };

            Client.OnClose += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    StartAnimation();
                });
            };

            Client.Initialize();
        }

        public void Disconnect()
        {
            if (!Client.IsDisposed)
            {
                Client.Dispose();
            }
        }

        #region Animation

        private void Animation_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan position = TimeSpan.Zero;
            TimeSpan duration = TimeSpan.MaxValue;
            Dispatcher.Invoke(() =>
            {
                if (VideoPlayer.NaturalDuration.HasTimeSpan)
                {
                    position = VideoPlayer.Position;
                    duration = VideoPlayer.NaturalDuration.TimeSpan;
                }
                else
                {
                    return;
                }
            });
            if (position >= duration)
            {
                Dispatcher.Invoke(() =>
                {
                    VideoPlayer.Source = new Uri("Resources/Loading.mp4", UriKind.Relative);
                    VideoPlayer.Play();
                });
            }
        }

        public void StartAnimation()
        {
            if (!AnimationLoop.Enabled)
            {
                VideoHolder = new Border()
                {
                    CornerRadius = new CornerRadius(10),
                    Background = new SolidColorBrush(Color.FromRgb(47, 49, 53))
                };
                MainGrid.Children.Add(VideoHolder);

                VideoPlayer = new MediaElement()
                {
                    LoadedBehavior = MediaState.Manual,
                    UnloadedBehavior = MediaState.Manual,
                    Source = new Uri("Resources/Loading.mp4", UriKind.Relative),
                    Stretch = Stretch.None
                };
                VideoHolder.Child = VideoPlayer;
                VideoHolder.Visibility = Visibility.Visible;
                AnimationLoop.Start();
                VideoPlayer.Play();
            }
        }

        public void StopAnimation()
        {
            VideoPlayer.Close();
            VideoHolder.Visibility = Visibility.Hidden;
            AnimationLoop.Stop();
        }

        #endregion

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
                    LargeImageKey = (string)LargeImageKeyBox.SelectedItem,
                    LargeImageText = LargeImageTextBox.Text,
                    SmallImageKey = (string)SmallImageKeyBox.SelectedItem,
                    SmallImageText = SmallImageTextBox.Text
                }
            };

            if (PartySizeBox.Text != "" && PartyMaxBox.Text != "")
            {
                Debug.WriteLine("step 1");
                if (int.TryParse(PartySizeBox.Text, out int partySize) && int.TryParse(PartyMaxBox.Text, out int partyMax))
                {
                    Debug.WriteLine("step 2");
                    presence.Party = new Party
                    {
                        ID = "DiscordRP",
                        Size = partySize,
                        Max = partyMax
                    };
                }
            }

            List<DiscordRPC.Button> buttons = new List<DiscordRPC.Button>();
            if (Button1TextBox.Text != "" && Button1UrlBox.Text != "")
            {
                buttons.Add(new DiscordRPC.Button()
                {
                    Label = Button1TextBox.Text,
                    Url = Button1UrlBox.Text
                });
            }

            if (Button2TextBox.Text != "" && Button2UrlBox.Text != "")
            {
                buttons.Add(new DiscordRPC.Button()
                {
                    Label = Button2TextBox.Text,
                    Url = Button2UrlBox.Text
                });
            }
            presence.Buttons = buttons.ToArray();

            if (TimestampBox.SelectedIndex == 1)
            {
                presence.Timestamps = new Timestamps(DateTime.UtcNow.Subtract(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)), null);
            }
            else if (TimestampBox.SelectedIndex == 2)
            {
                presence.Timestamps = new Timestamps(DateTime.UtcNow.Subtract(UpTime), null);
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
    }

    [Serializable]
    public class Settings
    {
        public string ClientID;
        public string Details;
        public string State;
        public string PartySize;
        public string PartyMax;
        public int TimestampIndex;

        public string LargeImageKey;
        public string LargeImageText;
        public string SmallImageKey;
        public string SmallImageText;

        public string Button1Text;
        public string Button1Url;
        public string Button2Text;
        public string Button2Url;

        public bool RunOnStartup;
    }

    public struct ImageAssets
    {
        public string ID;
        public string Type;
        public string Name;
    }
}
