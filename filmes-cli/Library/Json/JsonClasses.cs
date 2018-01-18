using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
namespace filmes_cli.Library
{
    public class cPost
    {
        public string name { get; set; }
        public string image { get; set; }
        public string link { get; set; }
    }
    public class cConfig
    {
        public int MaxItens { get; set; }
        public cConfig()
        {
            MaxItens = 5;
        }
    }
}
