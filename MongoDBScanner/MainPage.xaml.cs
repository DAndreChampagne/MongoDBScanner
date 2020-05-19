using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MongoDBScanner {
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();
        }

        void ScanButton_Clicked(System.Object sender, System.EventArgs e) {
            Console.WriteLine("ScanButton_Clicked");

            List<Task<PingReply>> tasks = new List<Task<PingReply>>();
            DateTime start = DateTime.Now, end = DateTime.Now;

            for (int i = 0; i <= 1; ++i) {
                for (int j = 0; j <= 255; ++j) {
                    var p = new System.Net.NetworkInformation.Ping();
                    tasks.Add(p.SendPingAsync($"192.168.{i}.{j}", 250));
                    Console.WriteLine($"sent ping to 192.168.{i}.{j}");
                }
            }

            Task.WaitAll(tasks.ToArray());

            tasks = tasks
                .Where(x => x.Result.Status == IPStatus.Success)
                .ToList();

            Console.WriteLine($"number of successful pings: {tasks.Count()}");

            foreach (var response in tasks) {
                string name = String.Empty;

                try {
                    IPHostEntry host = Dns.GetHostEntry(response.Result.Address);
                    name = host.HostName ?? String.Join(',', host.Aliases);
                } catch (Exception) {
                    // do nothing
                }

                Console.WriteLine($"{response.Result.Address} -- {response.Result.Status} -- {response.Result.RoundtripTime}ms -- {name}");
                this.ScanResults.Text += $"{response.Result.Address} -- {response.Result.Status} -- {response.Result.RoundtripTime}ms -- {name}\n";

            }

            end = DateTime.Now;

            Console.WriteLine($"Completed in {end - start}");
            this.ScanResults.Text += $"Completed in {end - start}\n";

        }

    }
}
