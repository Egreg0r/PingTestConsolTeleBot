using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Linq;

// ============================================================
//         Методы запроса конфигураций из файла xml.config
// ===============================================================


namespace ConsolTeleBot
{
    public class Configuration
    {
        private static IConfiguration XmlConfig { get; set; }
        //private static string useProtokol = XmlConfig["ProxyServers:UseProtocol"];
        //public static string IdBot = XmlConfig["idBot"];

        private string useProtokol()
        {
            return XmlConfig["ProxyServers:UseProtocol"];
        }

        /// <summary>
        /// String get Telegramm id bot from xml.config tag: <c>idBot</c>
        /// </summary>
        /// <returns></returns>
        public static string IdBot()
        {
            return XmlConfig["idBot"];
        }

        public Configuration()
        {
            var builder = new ConfigurationBuilder()
                .AddXmlFile(@"Config/Xml.config", optional: true, reloadOnChange: true);
            XmlConfig = builder.Build();
        }

        /// <summary>
        /// Собирает справочник опрашиваемых серверов. <Key> = name server, <value> = Ip server
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> AskServersDict()
        {
            return GetChildElemInDict("Servers");
        }

        /// <summary>
        /// Собирает список с ID админов из Xml.config в теге <c>adminId</c>
        /// </summary>
        /// <returns>List Admin Id</returns>
        public List<string> AdminsIdList()
        {
            return GetChildElemInDict("adminsId").Values.ToList();
        }

        /// <summary>
        /// Формирует list прокси доступных прокси серверов
        /// </summary>
        /// <returns></returns>
        public List<Proxy> ProxyList()
        {
            List<Proxy> proxy = new List<Proxy>();
            string ip, name;
            int port;
            //Подтягиваем тип используемого протокола из Xml.config
            var prot = XmlConfig["ProxyServers:UseProtocol"];
            var proxs = XmlConfig.GetSection("ProxyServers:"+prot);
            //Находим первый доступный прокси из списка, работающий по указанному протоколу. 
            foreach (var x in proxs.GetChildren())
            {
                ip = x["ip"];
                port = Int32.Parse(x["port"]);
                name = x.Key;
                if (IpEcho.HostSuccess(ip) == true)
                {
                    Logger.log.DebugFormat("{0}: {1} Пинганулся успешно.", name, ip);
                    //if (IpEcho.HostSuccess(ip, port) == true)
                    //{
                    //    Logger.log.InfoFormat("Proxy {0}: {1}", x.Key, ip);
                        proxy.Add(new Proxy { Ip = ip, Port = port, Protocol = prot, Name = name });
                    //}
                    //else { };
                }
                else Logger.log.DebugFormat("{0}: {1} не пингуется.", name, ip);

            }
            return proxy;
        }

        /// <summary>
        /// Use to get list value for children element from xml.config 
        /// </summary>
        /// <param name="value">name element with children</param>
        /// <returns>dict(key, value)</returns>
        public Dictionary<string, string> GetChildElemInDict(string value)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var count = XmlConfig.GetSection(value).GetChildren();
            foreach (var p in count)
            {
                dict.Add(p.Key ,p.Value);
            }
            return dict;
        }

    }


}
