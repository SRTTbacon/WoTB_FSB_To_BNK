using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WoTB_Voice_Mod_Creater.Class
{
    public class Multithread
    {
        static readonly List<string> From_Files = new List<string>();
        //マルチスレッドで.mp3や.oggを.wav形式にエンコード
        //拡張子とファイル内容が異なっていた場合実行されない(ファイル拡張子が.mp3なのに実際は.oggだった場合など)
        public static async Task Convert_To_Wav(string From_Dir, bool IsFromFileDelete)
        {
            await Convert_To_Wav(From_Dir, From_Dir, IsFromFileDelete);
        }
        public static async Task Convert_To_Wav(string From_Dir, string To_Dir, bool IsFromFileDelete)
        {
            try
            {
                if (!Directory.Exists(To_Dir))
                {
                    Directory.CreateDirectory(To_Dir);
                }
                From_Files.Clear();
                string[] Ex = new string[] { ".mp3", ".aac", ".ogg", ".flac", ".wma", ".wav" };
                From_Files.AddRange(DirectoryEx.GetFiles(From_Dir, SearchOption.TopDirectoryOnly, Ex));
                var tasks = new List<Task>();
                for (int i = 0; i < From_Files.Count; i++)
                {
                    tasks.Add(To_WAV(i, To_Dir, IsFromFileDelete));
                }
                await Task.WhenAll(tasks);
                From_Files.Clear();
            }
            catch (Exception ex)
            {
                From_Files.Clear();
                Sub_Code.Error_Log_Write(ex.Message);
            }
        }
        static async Task<bool> To_WAV(int File_Number, string To_Dir, bool IsFromFileDelete)
        {
            if (!File.Exists(From_Files[File_Number]))
            {
                return false;
            }
            string Encode_Style = "-y -vn -ac 2 -ar 44100 -acodec pcm_s24le -f wav";
            StreamWriter stw = File.CreateText(Voice_Set.Special_Path + "/Encode_Mp3/Audio_Encode" + File_Number + ".bat");
            stw.WriteLine("chcp 65001");
            stw.Write("\"" + Voice_Set.Special_Path + "/Encode_Mp3/ffmpeg.exe\" -i \"" + From_Files[File_Number] + "\" " + Encode_Style + " \"" + To_Dir + "\\" +
                      Path.GetFileNameWithoutExtension(From_Files[File_Number]) + ".wav\"");
            stw.Close();
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = Voice_Set.Special_Path + "/Encode_Mp3/Audio_Encode" + File_Number + ".bat",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process p = Process.Start(processStartInfo);
            await Task.Run(() =>
            {
                p.WaitForExit();
                if (IsFromFileDelete)
                {
                    File.Delete(From_Files[File_Number]);
                }
                File.Delete(Voice_Set.Special_Path + "/Encode_Mp3/Audio_Encode" + File_Number + ".bat");
            });
            return true;
        }
    }
}
//フォルダ内のファイルを取得(複数の拡張子を指定できます)
public static class DirectoryEx
{
    public static string[] GetFiles(string path, params string[] extensions)
    {
        return Directory
            .GetFiles(path, "*.*")
            .Where(c => extensions.Any(extension => c.EndsWith(extension)))
            .ToArray();
    }
    public static string[] GetFiles(string path, SearchOption searchOption, params string[] extensions)
    {
        return Directory
            .GetFiles(path, "*.*", searchOption)
            .Where(c => extensions.Any(extension => c.EndsWith(extension)))
            .ToArray();
    }
}