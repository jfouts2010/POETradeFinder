using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Media;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace ItemWatcher2
{
    static class Program
    {
       


        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartupForm());
            Application.Run(new EditWatchedRares());
            Application.Run(new EditCraftables());
            Application.Run(new Form1());
           
           
        }
        
       


    }
}
