﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            App app = new();
            app.Run(new MainWindow());
        }
    }
}
