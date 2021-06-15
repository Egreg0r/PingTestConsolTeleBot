using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Threading;
using System.Reflection;
using System.Runtime.ExceptionServices;
using log4net;
using System.IO;


namespace ConsolTeleBot
{
    class Program
    {

        static void Main(string[] args)
        {
            //Log4net
            Logger.InitLogger();
            Logger.log.Info("***********Ура заработало!***********");

            Logger.log.Info("check config files");
            try
            {
                fileConfig();
            }
            catch
            {
                Console.WriteLine("Ошибка при работе с Config. Не удалось скопировать файлы. ");
                Console.ReadKey();
                return;
            }


            //Start Bot
            TelegramBot.RunBot();
            Logger.log.Info("***********Завершена работа бота*************");
        }
        /// <summary>
        /// 
        /// </summary>
        private static void fileConfig()
        {
            string confDir = Directory.GetCurrentDirectory();
            string nameConfXml = "Xml.config";

            // Чекаем файл настроек Бота "Xml"
            FirstStart.copyIfNotExist(confDir, nameConfXml);
        }

    }

}

