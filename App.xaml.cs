using System.Runtime.InteropServices;
using System.Windows;

namespace WoTB_FSB_To_BNK
{
    public partial class App : Application
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);
        public App()
        {
            //dllの位置を変更
            string dllPath = System.IO.Path.Combine(System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName, @"Resources");
            SetDllDirectory(dllPath);
        }
    }
}