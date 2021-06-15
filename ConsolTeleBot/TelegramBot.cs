using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;
using MihaZupan;
using MethodExtension;

namespace ConsolTeleBot
{
    public static class TelegramBot
    {
        /// <summary>
        /// Сущьность Бота содержащая IdBot и Прокси для работы. 
        /// </summary>
        private static ITelegramBotClient botClient;

        /// <summary>
        /// token to access the HTTP API in Telegramm
        /// </summary>
        private static string idBot = Configuration.IdBot();

        /// <summary>
        /// Список серверов для автоматической проверки доступности. 
        /// </summary>
        private static Dictionary<string, string> serverAskList;

        // список недоступных прокси
        private static string lastecho;

        // используемый прокси сервер
        //private static Proxy proxyServer;
        //private static string protokol;

        //Массив с id пользователей которым отправляются автосообщения
        /// <summary>
        /// Список юзеров для отправления автособщений <c><adminsId></c> из Xml.config
        /// </summary>
        private static List<string> userSendList;

        public static void RunBot()
        {
            Configuration conect = new Configuration();
            List<Proxy> proxyServers = conect.ProxyList();


            // номер прокси из списка доступных прокси серверов
            int n = 0;

            //Текущий выбранный прокси сервер.
            Proxy proxyServer;
            User me = null;

            // кол-во доступных прокси серверов.
            int proxyCount = proxyServers.Count;

            
            lastecho = null;


            // Allows you to use proxies that are only allowing connections to Telegram
            // Needed for some proxies
            //proxy.ResolveHostnamesLocally = true;

            if (idBot.Length != 46)
            {
                Logger.log.FatalFormat("В файле Config.xml не указан или указан неверно id для используемого бота");
                return;
            }


            // Перебор доступных прокси и проверка на работспособность бота через них.  
            while (me == null)
            {
                if (proxyCount <= n)
                {
                    Logger.log.Fatal("Не удалось найти рабочий прокси. Добавьте прокси в Xml.config в элемент <ProxyServers> ");
                    //return;
                    return;
                }
                else proxyServer = proxyServers[n];

                try
                {
                    //создаем подключение к Боту
                    createBotClient(proxyServer);
                }
                catch (Exception ex)
                {
                    Logger.log.FatalFormat("void CreateBotClient: ошибка.  {0}", ex);
                }

                try
                {
                    if (proxyServer.Protocol == "NoProxy") Logger.log.InfoFormat("Используется подключение без Proxy");
                    else Logger.log.InfoFormat("Использую прокси: {0}. / {1}", proxyServer.Name , proxyServer.Protocol);
                    me = botClient.GetMeAsync().Result;
                    string st = $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."; // сообщаем об успешном подключении бота.
                    Logger.log.Info(st);
                    break;
                }
                catch (AggregateException e)
                {
                    Logger.log.ErrorFormat("Telegram Bot can't use this Proxy Server: {0}. \n ( \n  {1} \n )", proxyServer.Name, e.InnerException);
                    n++;
                }
            }

            botClient.OnMessage += BotOnMessageReceived;
            botClient.OnReceiveError += BotOnReceiveError;
            botClient.OnInlineResultChosen += BotOnChosenInlineResultReceived;

            botClient.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");
            try
            {
                BotOnServerOnlineAsync(120000);
                while (Console.ReadLine() != "exit")
                {
                    Console.WriteLine("Для выхода введите: exit");
                    // Console.ReadLine();
                }
            }
            catch(Exception ex) { Logger.log.Fatal(ex); }
            botClient.StopReceiving();
            

        }

        /// <summary>
        /// Определение протокола подключения и создание экземпляра <c>botClient</c>
        /// <para>
        /// Доступны протоколы:
        /// <list type="bullet">
        ///     <item><c>Socks5</c>,</item>
        ///     <item><c>HTTP</c></item>
        /// </list>
        /// </para>
        /// Присваивает <see cref="botClient"/> значение
        /// </summary>
        /// <param name="proxy">one exemple Proxy from <c>Xml.Config</c></param>
        private static void createBotClient(Proxy proxy)
        {
            switch (proxy.Protocol)
            {
                case "Socks5":
                    // Поключение к прокси серверу Soks5
                    HttpToSocks5Proxy proxyServerProtocol;
                    
                    proxyServerProtocol = new HttpToSocks5Proxy(proxy.Ip, proxy.Port)
                    {
                        // if you need credentials for your proxy server:
                        //Credentials = new NetworkCredential("USERNAME", "PASSWORD")

                    };
                    botClient = new TelegramBotClient(idBot, webProxy: proxyServerProtocol);
                    break;
                case "HTTP":
                    //Поключение к прокси серверу по HTTP
                    WebProxy proxyHttp;
                    proxyHttp = new WebProxy(proxy.Ip, proxy.Port)
                    {
                        // Credentials if needed:
                        //Credentials = new NetworkCredential("USERNMAE", "PASSWORD")
                        UseDefaultCredentials = true
                    };
                    botClient = new TelegramBotClient(idBot, webProxy: proxyHttp);
                    break;
                case "NoProxy":
                    // подключение без использование прокси.
                    botClient = new TelegramBotClient(idBot);
                    break;
                default:
                    Logger.log.FatalFormat("Не удалось определить протокол работы прокси сервера. Проверьте правильность файла  Xml.config");
                    throw new Exception();
            }
        }


        /// <summary>
        /// Обработка команд от пользователей для бота
        /// </summary>7/
        /// <param name="sender"></param>
        /// <param name="messageEventArgs"></param>

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            string send = "";
            Logger.log.InfoFormat("userId {0} ask command {1}", message.Chat.Id, message.Text.Replace("\n", " "));
            if (message == null || message.Type != MessageType.Text) return;
            string[] messageOnMass = message.Text.Split(new char[] { ' ' });
            switch (messageOnMass.First())
            {
                // send inline keyboard
                case "/inline":
                    await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                    // simulate longer running task
                    await Task.Delay(500);

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        // first row
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("1.1", "11"),
                            InlineKeyboardButton.WithCallbackData("1.2", "12"),
                        },
                        // second row
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("2.1", "21"),
                            InlineKeyboardButton.WithCallbackData("2.2", "22"),
                        }
                    });
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Choose",
                        replyMarkup: inlineKeyboard
                    );
                    break;

                // Пингует указанный хост. 
                case "/ping":
                    send = "Неверный формат команды.";
                    if (messageOnMass.Length > 1)
                    {
                        string ipstring = messageOnMass[1];

                        //проверяем ip на широковещательный запрос
                        if (ipstring.Contains("255"))
                            send = "Только 1 ip, широковещательный запрос недопустим. ";
                        else
                        {
                            send = IpEcho.PingHost(ipstring).ToString();
                        }
                    } 
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: send,
                        replyMarkup: new ReplyKeyboardRemove()
                        );
                    Logger.log.Info("Ответ:" + send);
                    break;

                // Показывает текущий статус серверов. 
                case "/ServersStatus":
                    if (messageOnMass.Length > 1)
                    {
                        string ipstring = messageOnMass[1];

                        //проверяем ip на широковещательный запрос
                    }
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "привет",
                        replyMarkup: new ReplyKeyboardRemove()
                        );
                    break;

                // **********Скрытые Команды**************
                // кол-во пользователей подключенных к боту. 
                case "/membercount":
                    var m = await botClient.GetChatMembersCountAsync(message.Chat.Id);
                    send = "Кол-во подключенных пользователей: " + m.ToString();
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: send,
                        replyMarkup: new ReplyKeyboardRemove()
                        );
                    Logger.log.Info("Ответ:" + send );
                    break;

                default:
                    const string usage = "Usage:\n" +
                        "/inline   - send inline keyboard\n" +
                        "/ping - 255.255.255.255 пингует указанный ip адресс \n" +
                        "/ServersStatus - curretn status list servers \n";
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: usage,
                        replyMarkup: new ReplyKeyboardRemove()
                    );
                    break;
            }
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        /// <summary>
        /// Опрашивает сервера из списка serverAskList  и оповещает пользователей из списка userSendArray в случае недоступности хоста.
        /// <para>
        /// <list type="bullet">
        /// <item><see cref="serverAskList"/> List опрашиваемых серверов из Xml.config <c>Servers</c></item>
        /// <item><see cref="Configuration.AskServersDict"/></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="timepause">Время между опросами пинга серверов (мс) </param>
        private static async Task BotOnServerOnlineAsync(int timepause = 120000)
        {
            int n = 0;
            while (true)
            {
                string echo = null;
                serverAskList = new Configuration().AskServersDict();
                // ожидание между Ping запросами
                for (int i = 0; i < serverAskList.Count; i++)
                {
                    var serv = serverAskList.ElementAt(i);
                    var echostat = IpEcho.HostNotSuccess(serv.Value);
                    if (echostat !="")
                    echo += string.Format("{0} {1} \n", serv.Key.ToString(), echostat);
                }

                // если список недоступных серверов изменился отправляется сообщение пользователю. 
                if (lastecho != echo)
                {
                    lastecho = echo;
                    n = 1;
                }
                if (n == 1 & lastecho == echo)
                {
                    n++;
                }

                if (n == 2)
                {
                    Logger.log.Error(echo);
                    userSendList = new Configuration().AdminsIdList();
                    for (int i = 0; i < userSendList.Count; i++)
                        await botClient.SendTextMessageAsync(
                            chatId: userSendList[i],
                            text: echo,
                            replyMarkup: new ReplyKeyboardRemove()
                            );
                    n++;
                }

                await Task.Delay(timepause);
               // Thread.Sleep(timepause);
            }
        }


        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Logger.log.ErrorFormat("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }

        /// <summary>
        ///  Запрашивает список рабочих IP указанного хоста и возвращает первый из них.   
        /// </summary>
        /// <param name="dns">Host DNS name</param>
        /// <returns> fist <c>IP</c> from <c>DNS</c></returns>
        private static IPAddress dnsToOneIp(string dns)
        {
            Logger.log.Debug("Преобразую Dns");
            IPAddress[] ipArray;
            ipArray = Dns.GetHostAddresses(dns);
           for (int i = 0 ; i< ipArray.Length; i++)
                { 
                Logger.log.DebugFormat("{0} IP address: " + ipArray[i].ToString());
                }
           return ipArray[1];
        }
    }


}
