﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace File_Monitoring_Windows_Service
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new FileMonitoring()
                };
                ServiceBase.Run(ServicesToRun);
            
        }
    }
}
