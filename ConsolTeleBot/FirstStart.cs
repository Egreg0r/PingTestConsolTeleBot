using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ConsolTeleBot
{
    /// ***************************************
    /// <summary>
    /// Класс для создание файлов конфигураций при первом запуске программы. 
    /// </summary>
    /// *************************************
    static class FirstStart
    {
        /// <summary>
        /// Копирование одноименного файла "<paramref name="fname"/>" если его нет в папке "<paramref name="sourseDir"/>\Config"
        /// из папки <c>"<paramref name="sourseDir"/>\File_for_release"</c>
        /// <para>
        /// Каталоги должны располагаться в каталге с программой
        /// </para>
        /// </summary>
        /// <param name="sourseDir">Папка с программой. </param>
        /// <param name="fname"> Имя копируемого файла без пути.</param>
        public static void copyIfNotExist (string sourseDir, string fname)
        {
            string fileDir = null; // полное имя файла
            fileDir = Path.Combine(sourseDir, "Config", fname);
            Logger.log.Debug("Искомый файл: "+ fileDir);
            if (!File.Exists(fileDir))
            {
                try
                {
                    File.Copy(Path.Combine(sourseDir, "File_for_release", fname), fileDir);
                    Logger.log.DebugFormat("Файл настроек \"{0}\" создан успешно", fname);
                }
                catch(Exception ex)
                {
                    Logger.log.FatalFormat("Во время копирования файла \"{0}\" возникла ошибка.", fname);
                    Logger.log.DebugFormat(": {0}", ex);
                    throw ex;
                }
            }
        }

    }
}
