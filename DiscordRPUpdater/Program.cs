using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;

namespace DiscordRPUpdater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine($"{args.Length} arguments given, 3 are required.");
                while (true);
            }

            string downloadUrl = args[0];
            string downloadPath = args[1];
            string downloadFileName = downloadPath + "\\files.zip";
            if (!int.TryParse(args[2], out int processId)) 
            {
                Console.WriteLine($"Process ID: {args[2]} has to be a valid int");
                while (true);
            }

            Console.WriteLine($"Download Url: {downloadUrl}");
            Console.WriteLine($"Download Path: {downloadFileName}");
            Console.WriteLine($"Process ID: {processId}");

            Console.WriteLine("Killing running process...");
            try
            {
                Process proc = Process.GetProcessById(processId);
                proc.Kill();
                proc.WaitForExit();
                Thread.Sleep(100); // For some reason you can't delete an exe file instantly after it exited
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                while (true);
            }

            Console.WriteLine("Clearing directory...");
            try
            {
                foreach (string dir in Directory.GetDirectories(downloadPath)) Directory.Delete(dir, true);
                foreach (string file in Directory.GetFiles(downloadPath)) File.Delete(file);
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                while (true);
            }

            Console.WriteLine("Downloading latest version...");
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile(downloadUrl, downloadFileName);
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                while (true);
            }

            Console.WriteLine("Extracting latest version...");
            try
            {
                ZipFile.ExtractToDirectory(downloadFileName, downloadPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                while (true);
            }

            Console.WriteLine("Cleaning up...");
            try
            {
                File.Delete(downloadFileName);
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                while (true);
            }

            Console.WriteLine("Starting DiscordRP");
            try
            {
                Process.Start(downloadPath + "\\DiscordRP.exe");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                while (true);
            }
        }

    }
}
