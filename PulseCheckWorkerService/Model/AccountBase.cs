
using Serilog;
using System.Diagnostics;
using System.Xml.Linq;

namespace Model
{
    public abstract class AccountBase
    {
        public string AccountName { get; set; }
        public int Frequency { get; set; }
        public string FileName { get; set; }
        public string WorkingDirectory { get; set; }

        public string LogPath { get; set; }

        public bool IsProcessRunning()
        {
            var exeName = FileName.Remove(FileName.LastIndexOf(".exe"));
            Process[] processes = Process.GetProcessesByName(exeName);
            if (processes.Length == 0)
            {
                var funcName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Log.Error("{funcname}: {name} is not running!", funcName, exeName);
                return false;
            }
            else
            {
                return true;

            }
        }

    }
}
