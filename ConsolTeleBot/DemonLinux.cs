using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
///##############################
/// <summary>
/// Реализация функционала для  Демонов linux. для работы Бота 
/// </summary>
/// ##############################
namespace ConsolTeleBot
{
    public static class DemonLinux
    {
        static readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        static readonly String outFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output");
        static readonly String outFilePath = Path.Combine(outFolderPath, $"{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
        public static void demon()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            if (!Directory.Exists(outFolderPath))
            {
                Directory.CreateDirectory(outFolderPath);
            }
            Console.WriteLine($"Output file: {outFilePath}");
            File.AppendAllLines(outFilePath, new List<String>() { "------------------- SERVICE START ------------------- " }, Encoding.UTF8);

            while (!tokenSource.Token.IsCancellationRequested)
            {
                File.AppendAllLines(outFilePath, new List<String>() { DateTime.Now.ToString() }, Encoding.UTF8);
                Console.WriteLine(DateTime.Now);
                Thread.Sleep(5000);
            }

            File.AppendAllLines(outFilePath, new List<String>() { "------------------- SERVICE STOP ------------------- " }, Encoding.UTF8);
        }
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            File.AppendAllLines(outFilePath, new List<String>() { "------------------- PROC EXIT SIGNAL ------------------- " }, Encoding.UTF8);
            tokenSource.Cancel();
        }

    }
}
