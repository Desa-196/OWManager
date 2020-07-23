using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Users
{
    public static class NetworkTester
    {
        private static bool IsStart = false;
        private static List<Thread> ListTread = new List<Thread>();

        static NetworkTester() { }

        public static void StartScan(ObservableCollection<User> ListUser)
        {
            IsStart = false;

            //Ждем пока все потоки запущенные с прошлого вызова, завершаться.
            while (true)
            {
                if (ListTread.Count == 0) break;

                bool thradeeStop = true;
                foreach (Thread tr in ListTread)
                {
                    thradeeStop = thradeeStop && (tr.ThreadState == ThreadState.Stopped);
                }
                if (thradeeStop) break;
            }

            IsStart = true;
            ListTread.Clear();

            Task.Run(() =>
            {
  
                for (int i = 0; i < ListUser.Count; i++)
                {
                    if (IsStart == false) break;

                    ListTread.Add(
                        new Thread(() =>
                           {
                               int index = i;
                               string text = ListUser[index].ComputerName;
                               string AddressIP = PingTest(ListUser[index].ComputerName);

                               if (IsStart == false) return;
                               if (AddressIP != null)
                               {
                                   ListUser[index].ComputerIP = AddressIP;
                                   ListUser[index].PingState = true;
                               }
                               else
                               {
                                   ListUser[index].PingState = false;
                               }

                           })
                    );
                    if (IsStart == false) break;
                    ListTread[i].Start();
                    Thread.Sleep(100);
                }
            });

        }
        public static void StopScan()
        {
            IsStart = false;
        }
        public static string PingTest(string Address)
        {
            using (Ping pinger = new Ping())
            {
                try
                {
                    PingReply reply = pinger.Send(Address, 1000);

                    if (reply.Status == IPStatus.Success)
                    {
                        return reply.Address.ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch { return null; }
            }
        }
        public static string PortTest(string Address, int Port)
        {
            string pingResult = PingTest(Address);
            if (pingResult != null)
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    var result = tcpClient.BeginConnect(Address, Port, null, null);

                    if (result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                    {
                        return pingResult;
                    }
                    else { return null; }


                }
            }
            else
            {
                return null;
            }
        }

    }
}
