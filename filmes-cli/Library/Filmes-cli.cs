using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
namespace filmes_cli.Library
{
    class cFilmes
    {
        private string pURL { get; set; }
        private string[] pPages { get; set; }
        public cFilmes(string url)
        {
            
            pURL = url;
            pPages = new string[] {
                "search.php?nome=",
                "play.php?link="
            };
        }
        private bool checkProcess()
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (p.MainWindowTitle == "peerflix")
                    return true;
            }
            return false;
        }
        public void startVLC(string magnet)
        {
            if (checkProcess())
            {
                MessageBox.Show("Close other peerflix process!");
                return;
            }
            File.WriteAllText("abrir.bat", $"peerflix \"{magnet}\" --vlc");               
            Process.Start("abrir.bat");
        }
        public void startFileVLC(string torrentLocal)
        {
            if (checkProcess())
            {
                MessageBox.Show("Close other peerflix process!");
                return;
            }
            File.WriteAllText("abrir.bat", $"peerflix \"{torrentLocal}\" -a --vlc");
            Process.Start("abrir.bat");
        }
        public string getMagnet(string link)
        {
            return new WebClient().DownloadString(pURL + pPages[1] + link);
        }
        public cPost[] searchFilme(string name)
        {
            try
            {
                List<cPost> arrReturn = new List<cPost>();
                WebClient wc = new WebClient();
                wc.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                string webResponse = wc.DownloadString( pURL + pPages[0] + name );

                if (!webResponse.Contains("name"))
                    return null;

                cPost[] jsResponse = JsonConvert.DeserializeObject<cPost[]>(webResponse);

                if (jsResponse.Length <= 0)
                    return null;

                arrReturn.AddRange(jsResponse);

                return arrReturn.ToArray();
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
