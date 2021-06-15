using System;
using System.Collections.Generic;
using System.Text;

namespace ConsolTeleBot
{
    /// <summary>
    /// Класс для прокси серверов
    /// </summary>
    public class Proxy
    {
        public string Ip { get; set; } = ""; // server ip adress 
        public int Port { get; set; } = 0; // server port 
        public string Protocol { get; set; } = ""; // use server operation protocol
        public string Name { get; set; } = ""; // <proxy1> 
        //public Proxy(string ip, int port, string prot)
        //{
        //    Ip = ip;
        //    Port = port;
        //    Protocol = prot;
        //}
    }

    /// <summary>
    /// Класс серверов для опроса на доступность.
    /// </summary>
    public class ServersWork
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Succsess { get; set; }
    }
}
