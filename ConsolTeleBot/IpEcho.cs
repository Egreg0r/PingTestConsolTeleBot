using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ConsolTeleBot
{
    class IpEcho
    {
        /// <summary>
        /// Пингует указаный хост
        /// </summary>
        /// <param name="hostIp">строка с IP</param>
        public static string PingHost(string hostIp)
        {
            string repIp = null;
            Ping pingSender = new Ping();
            Logger.log.InfoFormat("Ping IpAdress: {0}", hostIp);
            IPAddress ip = stringToIpAdress(hostIp);
            if (ip != null)
            {
                PingReply reply = pingSender.Send(ip);
                if (reply.Status == IPStatus.Success)
                {
                    repIp += string.Format("Address: {0} - Доступен \n ", reply.Address);
                    repIp += string.Format("RoundTrip time: {0}  \n ", reply.RoundtripTime);
                    //следующие строки в Linux Не работают. 
                    //repIp += string.Format("Time to live: {0}  \n ", reply.Options.Ttl);
                    //Logger.log.Debug("repIP: " + repIp);
                    //repIp += string.Format("Don't fragment: {0}  \n ", reply.Options.DontFragment);
                    //Logger.log.Debug("repIP: " + repIp);
                    //repIp += string.Format("Buffer size: {0}  \n ", reply.Buffer.Length);
                    //Logger.log.Debug("repIP: " + repIp);
                    return repIp;
                }
                else
                {
                    Logger.log.WarnFormat("Хост: {0} {1}", hostIp, reply.Status);
                    return reply.Status.ToString();
                }
            }
            else return string.Format("Неверный формат IP адресса. Введено: {0} \n", hostIp);
        }



        public static bool HostSuccess(string hostIp)
        {
            return (hostSuccess(hostIp) == IPStatus.Success);
            //    return true;
            //else return false;
        }

        public static bool HostSuccess(string hostIp, int port)
        {
            bool res;
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(hostIp, port);
                    Logger.log.Debug(hostIp + " - открыт");
                    res = true;
                }
                catch (Exception)
                {
                    res = false;
                    Logger.log.Debug(hostIp + " - закрыт");
                }
                finally
                {
                    tcpClient.Close();
                }
            }
            return res;
        }

        /// <summary>
        /// Определяет хосты без пинга
        /// </summary>
        /// <param name="address">Ip address for server</param>
        /// <param name="tsleep">Time wait before ping (in milisecond)</param>
        /// <returns>"" or IPStatus.toString  </returns>
        public static string HostNotSuccess(string address, int tsleep = 500)
        {
            string str = "";
                IPStatus iPStatus = hostSuccess(address);
                Thread.Sleep(tsleep);
                if (iPStatus != IPStatus.Success)
                {
                    str = string.Format("Ip: {0} !!!!! {1}", address, iPStatus.ToString());
                }
            return str;
        }


        #region Private
        /// <summary>
        /// ehco ping status host
        /// </summary>
        /// <param name="hostIp">Ip adress host</param>
        /// <returns>IPStatus</returns>
        private static IPStatus hostSuccess(string hostIp)
        {
            Ping pingSender = new Ping();
            PingReply reply;
            try { 
                reply = pingSender.Send(hostIp, 1000);
            }
            catch (Exception ex)
            {
                Logger.log.ErrorFormat("Во время преобразования IP: {0} возникла непредвиденная ошибка. Проверьте правильность указания Ip или domain. {1}", hostIp, ex);
                return IPStatus.Unknown;
            }
            return reply.Status;
        }

        /// <summary>
        /// преобразование ip из строки в IPAdress не используется. 
        /// </summary>
        /// <param name="hostIp">строка с IP</param>
        private static IPAddress stringToIpAdress (string hostIp)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(hostIp);
                return ip;
            }
            catch (ArgumentNullException e)
            {
                Logger.log.Error("ArgumentNullException caught!!!");
                Logger.log.Error("Source : " + e.Source);
                Logger.log.Error("Message : " + e.Message);
                return null;
            }
            catch (FormatException e)
            {
                Logger.log.Error("FormatException caught!!!");
                Logger.log.Error("Source : " + e.Source);
                Logger.log.Error("Message : " + e.Message);
                return null;
            }
            catch (Exception e)
            {
                Logger.log.Error("Exception caught!!!");
                Logger.log.Error("Source : " + e.Source);
                Logger.log.Error("Message : " + e.Message);
                return null;
            }
        }
        #endregion
    }
}
