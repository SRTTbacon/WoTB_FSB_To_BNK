using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace WoTB_Voice_Mod_Creater
{
    public class Sub_Code
    {
        public const double Window_Feed_Time = 0.04;
        //必要なdllがない場合そのdll名のリストを返す
        public static List<string> DLL_Exists()
        {
            string DLL_Path = Directory.GetCurrentDirectory() + "/dll";
            List<string> DLL_List = new List<string>();
            if (!File.Exists(DLL_Path + "/bass.dll"))
            {
                DLL_List.Add("bass.dll");
            }
            if (!File.Exists(DLL_Path + "/bass_fx.dll"))
            {
                DLL_List.Add("bass_fx.dll");
            }
            if (!File.Exists(DLL_Path + "/DdsFileTypePlusIO_x86.dll"))
            {
                DLL_List.Add("DdsFileTypePlusIO_x86.dll");
            }
            if (!File.Exists(DLL_Path + "/fmod_event.dll"))
            {
                DLL_List.Add("fmod_event.dll");
            }
            if (!File.Exists(DLL_Path + "/fmodex.dll"))
            {
                DLL_List.Add("fmodex.dll");
            }
            return DLL_List;
        }
        //.dvplを抜いたファイルパスからファイルが存在するか
        //例:sounds.yaml.dvpl -> DVPL_File_Exists(sounds.yaml) -> true,false
        public static bool DVPL_File_Exists(string File_Path)
        {
            if (File.Exists(File_Path) || File.Exists(File_Path + ".dvpl"))
            {
                return true;
            }
            return false;
        }
        //WoTBのディレクトリを取得
        public static bool WoTB_Get_Directory()
        {
            try
            {
                RegistryKey rKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Valve\\Steam");
                string location = (string)rKey.GetValue("InstallPath");
                rKey.Close();
                string driveRegex = @"[A-Z]:\\";
                if (File.Exists(location + "/steamapps/common/World of Tanks Blitz/wotblitz.exe"))
                {
                    StreamWriter stw = File.CreateText(Voice_Set.Special_Path + "/Temp_WoTB_Path.dat");
                    stw.Write(location + "/steamapps/common/World of Tanks Blitz");
                    stw.Close();
                    using (var eifs = new FileStream(Voice_Set.Special_Path + "/Temp_WoTB_Path.dat", FileMode.Open, FileAccess.Read))
                    {
                        using (var eofs = new FileStream(Directory.GetCurrentDirectory() + "/WoTB_Path.dat", FileMode.Create, FileAccess.Write))
                        {
                            FileEncode.FileEncryptor.Encrypt(eifs, eofs, "WoTB_Directory_Path_Pass");
                        }
                    }
                    File.Delete(Voice_Set.Special_Path + "/Temp_WoTB_Path.dat");
                    Voice_Set.WoTB_Path = location + "/steamapps/common/World of Tanks Blitz";
                    return true;
                }
                string[] configLines = File.ReadAllLines(location + "/steamapps/libraryfolders.vdf");
                foreach (var item in configLines)
                {
                    Match match = Regex.Match(item, driveRegex);
                    if (item != string.Empty && match.Success)
                    {
                        string matched = match.ToString();
                        string item2 = item.Substring(item.IndexOf(matched));
                        item2 = item2.Replace("\\\\", "\\");
                        item2 = item2.Replace("\"", "\\steamapps\\common\\");
                        if (File.Exists(item2 + "World of Tanks Blitz\\wotblitz.exe"))
                        {
                            StreamWriter stw = File.CreateText(Voice_Set.Special_Path + "/Temp_WoTB_Path.dat");
                            stw.Write(item2 + "World of Tanks Blitz");
                            stw.Close();
                            using (var eifs = new FileStream(Voice_Set.Special_Path + "/Temp_WoTB_Path.dat", FileMode.Open, FileAccess.Read))
                            {
                                using (var eofs = new FileStream(Directory.GetCurrentDirectory() + "/WoTB_Path.dat", FileMode.Create, FileAccess.Write))
                                {
                                    FileEncode.FileEncryptor.Encrypt(eifs, eofs, "WoTB_Directory_Path_Pass");
                                }
                            }
                            File.Delete(Voice_Set.Special_Path + "/Temp_WoTB_Path.dat");
                            Voice_Set.WoTB_Path = item2 + "World of Tanks Blitz";
                            return true;
                        }
                    }
                }
                MessageBox.Show("WoTBのインストール先を取得できませんでした。SteamにWoTBがインストールされていないか、32BitOSを使用している可能性があります。");
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show("WoTBのインストール先を取得できませんでした。SteamにWoTBがインストールされていないか、32BitOSを使用している可能性があります。");
                Error_Log_Write(e.Message);
                return false;
            }
        }
        //ディレクトリをコピー(サブフォルダを含む)
        public static void Directory_Copy(string From_Dir, string To_Dir)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(From_Dir);
                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException("指定したディレクトリが存在しません。\n" + From_Dir);
                }
                DirectoryInfo[] dirs = dir.GetDirectories();
                Directory.CreateDirectory(To_Dir);
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(To_Dir, file.Name);
                    file.CopyTo(tempPath, false);
                }
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(To_Dir, subdir.Name);
                    Directory_Copy(subdir.FullName, tempPath);
                }
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
            }
        }
        //.dvplを抜いたファイルをコピーする
        public static bool DVPL_File_Copy(string FromFilePath, string ToFilePath, bool IsOverWrite)
        {
            FromFilePath = FromFilePath.Replace(".dvpl", "");
            ToFilePath = ToFilePath.Replace(".dvpl", "");
            if (File.Exists(FromFilePath) || File.Exists(FromFilePath + ".dvpl"))
            {
                if (File.Exists(FromFilePath))
                {
                    try
                    {
                        File.Copy(FromFilePath, ToFilePath, IsOverWrite);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Error_Log_Write(e.Message);
                    }
                }
                if (File.Exists(FromFilePath + ".dvpl"))
                {
                    try
                    {
                        File.Copy(FromFilePath + ".dvpl", ToFilePath + ".dvpl", IsOverWrite);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Error_Log_Write(e.Message);
                    }
                }
            }
            return false;
        }
        //.dvplを抜いたファイルを削除する
        public static bool DVPL_File_Delete(string FilePath)
        {
            bool IsDelected = false;
            FilePath = FilePath.Replace(".dvpl", "");
            if (File.Exists(FilePath))
            {
                try
                {
                    File.Delete(FilePath);
                    IsDelected = true;
                }
                catch (Exception e)
                {
                    Error_Log_Write(e.Message);
                }
            }
            if (File.Exists(FilePath + ".dvpl"))
            {
                try
                {
                    File.Delete(FilePath + ".dvpl");
                    IsDelected = true;
                }
                catch (Exception e)
                {
                    Error_Log_Write(e.Message);
                }
            }
            return IsDelected;
        }
        //.dvplを抜いたファイルを移動
        public static bool DVPL_File_Move(string From_File, string To_File, bool IsOverWrite)
        {
            bool IsMoved = false;
            From_File = From_File.Replace(".dvpl", "");
            if (File.Exists(From_File))
            {
                IsMoved = File_Move(From_File, To_File, IsOverWrite);
            }
            if (File.Exists(From_File + ".dvpl"))
            {
                IsMoved = File_Move(From_File + ".dvpl", To_File + ".dvpl", IsOverWrite);
            }
            return IsMoved;
        }
        //ファイルを移動(正確にはコピーして元ファイルを削除)
        public static bool File_Move(string From_File_Path, string To_File_Path, bool IsOverWrite)
        {
            if (!File.Exists(From_File_Path))
            {
                return false;
            }
            if (File.Exists(To_File_Path) && !IsOverWrite)
            {
                return false;
            }
            try
            {
                File.Copy(From_File_Path, To_File_Path, true);
                File.Delete(From_File_Path);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //↑の拡張子を指定しないバージョン
        public static bool File_Move_V2(string From_File_Path, string To_File_Path, bool IsOverWrite)
        {
            string From_Path = File_Get_FileName_No_Extension(From_File_Path);
            string To_Path = To_File_Path + Path.GetExtension(From_Path);
            return File_Move(From_Path, To_Path, IsOverWrite);
        }
        //ファイル拡張子を指定しないでファイルをコピーする
        public static bool File_Copy(string From_File_Path, string To_File_Path, bool IsOverWrite)
        {
            string File_Path = "";
            string Dir = Path.GetDirectoryName(From_File_Path);
            string Name = Path.GetFileName(From_File_Path);
            var files = Directory.GetFiles(Dir, Name + ".*");
            if (files.Length > 0)
            {
                File_Path = files[0];
            }
            if (File_Path == "" || !File.Exists(From_File_Path))
            {
                return false;
            }
            if (File_Exists(To_File_Path) && !IsOverWrite)
            {
                return false;
            }
            try
            {
                File.Copy(File_Path, To_File_Path + Path.GetExtension(File_Path), true);
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
                return false;
            }
            return true;
        }
        //ファイル拡張子を指定しないでファイルを削除
        public static bool File_Delete(string File_Path)
        {
            string Dir = Path.GetDirectoryName(File_Path);
            string Name = Path.GetFileName(File_Path);
            var files = Directory.GetFiles(Dir, Name + ".*");
            if (files.Length > 0)
            {
                File.Delete(files[0]);
                return true;
            }
            return false;
        }
        public static bool File_Delete_V2(string File_Path)
        {
            try
            {
                File.Delete(File_Path);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //ファイル拡張子なしでファイルが存在するか取得
        //戻り値:存在した場合true,それ以外はfalse
        public static bool File_Exists(string File_Path)
        {
            try
            {
                string Dir = Path.GetDirectoryName(File_Path);
                string Name = Path.GetFileName(File_Path);
                var files = Directory.GetFiles(Dir, Name + ".*");
                if (files.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        //音声の種類が存在するか
        public static bool File_Exist_Voice_Type(string Voice_Dir, string File_Path)
        {
            try
            {
                string Path_01 = Voice_Dir + "/" + File_Path;
                string Dir = Path.GetDirectoryName(Path_01);
                string Name = Path.GetFileName(Path_01);
                var files = Directory.GetFiles(Dir, Name + "_01.*");
                if (files.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
                return false;
            }
        }
        //ファイル拡張子なしのパスから拡張子付きのファイルパスを取得
        //戻り値:拡張子付きのファイル名
        public static string File_Get_FileName_No_Extension(string File_Path)
        {
            try
            {
                string Dir = Path.GetDirectoryName(File_Path);
                string Name = Path.GetFileName(File_Path);
                var files = Directory.GetFiles(Dir, Name + ".*");
                if (files.Length > 0)
                {
                    return files[0];
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
                return "";
            }
        }
        //音声タイプとそのタイプのファイル数を取得
        //引数:取得するフォルダ,ref 音声タイプを入れるリスト,ref 音声数を入れるリスト   (リストは初期化される)
        public static void Get_Voice_Type_And_Index(string Dir_Path, ref List<string> Voice_Type, ref List<int> Voice_Number)
        {
            string[] Dir_List = Directory.GetFiles(Dir_Path, "*", SearchOption.TopDirectoryOnly);
            List<string> Voice_List_Type = new List<string>();
            for (int Number = 0; Number <= Dir_List.Length - 1; Number++)
            {
                if (Voice_Set.Voice_Name_Hide(Dir_List[Number]))
                {
                    Voice_List_Type.Add(Path.GetFileName(Dir_List[Number]));
                }
            }
            List<string> Voice_Type_Ref = new List<string>();
            List<int> Voice_Type_Number_Ref = new List<int>();
            int Voice_Type_Number = 0;
            string Name_Now = "";
            for (int Number = 0; Number < Voice_List_Type.Count; Number++)
            {
                string Name_Only = Voice_List_Type[Number].Substring(0, Voice_List_Type[Number].LastIndexOf('_'));
                if (Name_Now != Name_Only)
                {
                    if (Name_Now != "")
                    {
                        Voice_Type_Number_Ref.Add(Voice_Type_Number);
                    }
                    Name_Now = Name_Only;
                    Voice_Type_Number = 1;
                    Voice_Type_Ref.Add(Voice_Set.Get_Voice_Type_Japanese_Name(Name_Only));
                }
                else
                {
                    Voice_Type_Number++;
                }
            }
            Voice_Type_Number_Ref.Add(Voice_Type_Number);
            Voice_Type = Voice_Type_Ref;
            Voice_Number = Voice_Type_Number_Ref;
        }
        //音声タイプの名前に変換
        //例:Indexが2で既にそのタイプのファイル数が3個ある場合 -> danyaku_04.mp3
        public static string Set_Voice_Type_Change_Name_By_Index(string Dir, List<List<string>> Lists)
        {
            int Romaji_Number = 0;
            foreach (List<string> Index in Lists)
            {
                int File_Number = 1;
                foreach (string File_Path in Index)
                {
                    try
                    {
                        if (File_Number < 10)
                        {
                            File.Copy(File_Path, Dir + "/" + Voice_Set.Get_Voice_Type_Romaji_Name(Romaji_Number) + "_0" + File_Number + Path.GetExtension(File_Path), true);
                        }
                        else
                        {
                            File.Copy(File_Path, Dir + "/" + Voice_Set.Get_Voice_Type_Romaji_Name(Romaji_Number) + "_" + File_Number + Path.GetExtension(File_Path), true);
                        }
                        File_Number++;
                    }
                    catch (Exception e)
                    {
                        Error_Log_Write(e.Message);
                        return "ファイルをコピーできませんでした。";
                    }
                }
                Romaji_Number++;
            }
            return "";
        }
        //現在の時間を文字列で取得
        //引数:DateTime.Now,間に入れる文字,どの部分から開始するか,どの部分で終了するか(その数字の部分は含まれる)
        //First,End->1 = Year,2 = Month,3 = Date,4 = Hour,5 = Minutes,6 = Seconds
        public static string Get_Time_Now(DateTime dt, string Between, int First, int End)
        {
            if (First > End)
            {
                return "";
            }
            if (First == End)
            {
                return Get_Time_Index(dt, First);
            }
            string Temp = "";
            for (int Number = First; Number <= End; Number++)
            {
                if (Number != End)
                {
                    Temp += Get_Time_Index(dt, Number) + Between;
                }
                else
                {
                    Temp += Get_Time_Index(dt, Number);
                }
            }
            return Temp;
        }
        static string Get_Time_Index(DateTime dt, int Index)
        {
            if (Index > 0 && Index < 7)
            {
                if (Index == 1)
                {
                    return dt.Year.ToString();
                }
                else if (Index == 2)
                {
                    return dt.Month.ToString();
                }
                else if (Index == 3)
                {
                    return dt.Day.ToString();
                }
                else if (Index == 4)
                {
                    return dt.Hour.ToString();
                }
                else if (Index == 5)
                {
                    return dt.Minute.ToString();
                }
                else if (Index == 6)
                {
                    return dt.Second.ToString();
                }
            }
            return "";
        }
        //文字列に日本語が含まれていたらtrueを返す
        public static bool IsTextIncludeJapanese(string text)
        {
            bool isJapanese = Regex.IsMatch(text, @"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]+");
            return isJapanese;
        }
        //エラーをログに記録(改行コードはあってもなくてもよい)
        public static void Error_Log_Write(string Text)
        {
            DateTime dt = DateTime.Now;
            string Time = Get_Time_Now(dt, ".", 1, 6);
            if (Text.EndsWith("\n"))
            {
                File.AppendAllText(Directory.GetCurrentDirectory() + "/Error_Log.txt", Time + ":" + Text);
            }
            else
            {
                File.AppendAllText(Directory.GetCurrentDirectory() + "/Error_Log.txt", Time + ":" + Text + "\n");
            }
        }
        //ファイル名に使用できない文字を_に変更
        public static string File_Replace_Name(string FileName)
        {
            string valid = FileName;
            char[] invalidch = Path.GetInvalidFileNameChars();
            foreach (char c in invalidch)
            {
                valid = valid.Replace(c, '_');
            }
            return valid;
        }
        //指定したファイルが.wav形式だった場合true
        public static bool Audio_IsWAV(string File_Path)
        {
            bool Temp = false;
            try
            {
                using (FileStream fs = new FileStream(File_Path, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        if (Encoding.ASCII.GetString(br.ReadBytes(4)) == "RIFF")
                        {
                            Temp = true;
                        }
                    }
                }
            }
            catch
            {
            }
            return Temp;
        }
        //音声ファイルを指定した拡張子へエンコード
        public static bool Audio_Encode_To_Other(string From_Audio_File, string To_Audio_File, string Encode_Mode, bool IsFromFileDelete)
        {
            try
            {
                if (!File.Exists(From_Audio_File))
                {
                    return false;
                }
                Encode_Mode = Encode_Mode.Replace(".", "");
                string Encode_Style = "";
                //変換先に合わせて.batファイルを作成
                if (Encode_Mode == "aac")
                {
                    Encode_Style = "-y -vn -strict experimental -c:a aac -b:a 256k";
                }
                else if (Encode_Mode == "flac")
                {
                    Encode_Style = "-y -vn -ac 2 -ar 44100 -acodec flac -f flac";
                }
                else if (Encode_Mode == "mp3")
                {
                    Encode_Style = "-y -vn -ac 2 -ar 44100 -ab 128k -acodec libmp3lame -f mp3";
                }
                else if (Encode_Mode == "ogg")
                {
                    Encode_Style = "-y -vn -ac 2 -ar 44100 -ab 128k -acodec libvorbis -f ogg";
                }
                else if (Encode_Mode == "wav")
                {
                    Encode_Style = "-y -vn -ac 2 -ar 44100 -acodec pcm_s24le -f wav";
                }
                else if (Encode_Mode == "webm")
                {
                    Encode_Style = "-y -vn -f opus -acodec libopus -ab 128k";
                }
                else if (Encode_Mode == "wma")
                {
                    Encode_Style = "-y -vn -ac 2 -ar 44100 -ab 128k -acodec wmav2 -f asf";
                }
                StreamWriter stw = File.CreateText(Voice_Set.Special_Path + "/Encode_Mp3/Audio_Encode.bat");
                stw.WriteLine("chcp 65001");
                stw.Write("\"" + Voice_Set.Special_Path + "/Encode_Mp3/ffmpeg.exe\" -i \"" + From_Audio_File + "\" " + Encode_Style + " \"" + To_Audio_File + "\"");
                stw.Close();
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = Voice_Set.Special_Path + "/Encode_Mp3/Audio_Encode.bat",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process p = Process.Start(processStartInfo);
                p.WaitForExit();
                if (!File.Exists(To_Audio_File))
                {
                    return false;
                }
                if (IsFromFileDelete)
                {
                    File.Delete(From_Audio_File);
                }
                File.Delete(Voice_Set.Special_Path + "/Encode_Mp3/Audio_Encode.bat");
                return true;
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
                return false;
            }
        }
        //ファイルを暗号化
        //引数:元ファイルのパス,暗号先のパス,元ファイルを削除するか
        public static bool File_Encrypt(string From_File, string To_File, string Password, bool IsFromFileDelete)
        {
            try
            {
                if (!File.Exists(From_File))
                {
                    return false;
                }
                using (var eifs = new FileStream(From_File, FileMode.Open, FileAccess.Read))
                {
                    using (var eofs = new FileStream(To_File, FileMode.Create, FileAccess.Write))
                    {
                        FileEncode.FileEncryptor.Encrypt(eifs, eofs, Password);
                    }
                }
                if (IsFromFileDelete)
                {
                    File.Delete(From_File);
                }
                return true;
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
                return false;
            }
        }
        //ファイルを復号化
        //引数:元ファイルのパス,復号先のパス,元ファイルを削除するか
        public static bool File_Decrypt(string From_File, string To_File, string Password, bool IsFromFileDelete)
        {
            try
            {
                if (!File.Exists(From_File))
                {
                    return false;
                }
                using (var eifs = new FileStream(From_File, FileMode.Open, FileAccess.Read))
                {
                    using (var eofs = new FileStream(To_File, FileMode.Create, FileAccess.Write))
                    {
                        FileEncode.FileEncryptor.Decrypt(eifs, eofs, Password);
                    }
                }
                if (IsFromFileDelete)
                {
                    File.Delete(From_File);
                }
                return true;
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
                return false;
            }
        }
        //フォルダ選択画面の初期フォルダを取得
        public static string Get_OpenDirectory_Path()
        {
            string InDir = "C:\\";
            if (File.Exists(Voice_Set.Special_Path + "/OpenDirectoryPath.dat"))
            {
                try
                {
                    File_Decrypt(Voice_Set.Special_Path + "/OpenDirectoryPath.dat", Voice_Set.Special_Path + "/OpenDirectoryPath.tmp", "Directory_Save_SRTTbacon", false);
                    StreamReader str = new StreamReader(Voice_Set.Special_Path + "/OpenDirectoryPath.tmp");
                    string Read = str.ReadLine();
                    str.Close();
                    if (Directory.Exists(Read))
                    {
                        InDir = Read;
                    }
                    File.Delete(Voice_Set.Special_Path + "/OpenDirectoryPath.tmp");
                }
                catch
                {
                }
            }
            return InDir;
        }
        //フォルダ選択画面の初期フォルダを更新
        public static bool Set_Directory_Path(string Dir)
        {
            if (!Directory.Exists(Dir))
            {
                return false;
            }
            try
            {
                StreamWriter stw = File.CreateText(Voice_Set.Special_Path + "/OpenDirectoryPath.tmp");
                stw.Write(Dir);
                stw.Close();
                File_Encrypt(Voice_Set.Special_Path + "/OpenDirectoryPath.tmp", Voice_Set.Special_Path + "/OpenDirectoryPath.dat", "Directory_Save_SRTTbacon", true);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //ファイル選択画面の初期フォルダを取得
        public static string Get_OpenFile_Path()
        {
            string InDir = "C:\\";
            if (File.Exists(Voice_Set.Special_Path + "/OpenFilePath.dat"))
            {
                try
                {
                    File_Decrypt(Voice_Set.Special_Path + "/OpenFilePath.dat", Voice_Set.Special_Path + "/OpenFilePath.tmp", "OpenFile_Save_SRTTbacon", false);
                    StreamReader str = new StreamReader(Voice_Set.Special_Path + "/OpenFilePath.tmp");
                    string Read = str.ReadLine();
                    str.Close();
                    if (Directory.Exists(Read))
                    {
                        InDir = Read;
                    }
                    File.Delete(Voice_Set.Special_Path + "/OpenFilePath.tmp");
                }
                catch
                {
                }
            }
            return InDir;
        }
        //ファイル選択画面の初期フォルダを更新
        public static bool Set_OpenFile_Path(string Dir)
        {
            if (!Directory.Exists(Dir))
            {
                return false;
            }
            try
            {
                StreamWriter stw = File.CreateText(Voice_Set.Special_Path + "/OpenFilePath.tmp");
                stw.Write(Dir);
                stw.Close();
                File_Encrypt(Voice_Set.Special_Path + "/OpenFilePath.tmp", Voice_Set.Special_Path + "/OpenFilePath.dat", "OpenFile_Save_SRTTbacon", true);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //フォルダ内のファイルを削除
        public static void Directory_Delete(string Dir)
        {
            string[] Files = Directory.GetFiles(Dir, "*", SearchOption.AllDirectories);
            foreach (string File_Now in Files)
            {
                try
                {
                    File.Delete(File_Now);
                }
                catch
                {
                }
            }
            try
            {
                Directory.Delete(Dir, true);
            }
            catch
            {
            }
        }
        //.wemファイルを指定した形式に変換
        public static bool WEM_To_File(string From_WEM_File, string To_Audio_File, string Encode_Mode, bool IsFromFileDelete)
        {
            try
            {
                if (!File.Exists(From_WEM_File))
                {
                    return false;
                }
                Process wwToOgg = new Process();
                wwToOgg.StartInfo.FileName = Voice_Set.Special_Path + "/Wwise/ww2ogg.exe";
                wwToOgg.StartInfo.WorkingDirectory = Voice_Set.Special_Path + "/Wwise";
                wwToOgg.StartInfo.Arguments = "--pcb packed_codebooks_aoTuV_603.bin -o \"" + Voice_Set.Special_Path + "\\Wwise\\Temp.ogg\" \"" + From_WEM_File + "\"";
                wwToOgg.StartInfo.CreateNoWindow = true;
                wwToOgg.StartInfo.UseShellExecute = false;
                wwToOgg.StartInfo.RedirectStandardError = true;
                wwToOgg.StartInfo.RedirectStandardOutput = true;
                wwToOgg.Start();
                wwToOgg.WaitForExit();
                Process revorb = new Process();
                revorb.StartInfo.FileName = Voice_Set.Special_Path + "/Wwise/revorb.exe";
                revorb.StartInfo.Arguments = "\"" + Voice_Set.Special_Path + "\\Wwise\\Temp.ogg\"" + "\"";
                revorb.StartInfo.CreateNoWindow = true;
                revorb.StartInfo.UseShellExecute = false;
                revorb.StartInfo.RedirectStandardError = true;
                revorb.Start();
                revorb.WaitForExit();
                if (File.Exists(Voice_Set.Special_Path + "\\Wwise\\Temp.ogg"))
                {
                    Audio_Encode_To_Other(Voice_Set.Special_Path + "\\Wwise\\Temp.ogg", To_Audio_File, Encode_Mode, true);
                    if (IsFromFileDelete)
                    {
                        File.Delete(From_WEM_File);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
                return false;
            }
        }
        //サウンドファイル(.mp3や.oggなど)を.wem形式に変換
        //内容によってかなり時間がかかります。
        public static bool File_To_WEM(string From_WAV_File, string To_WEM_File, bool IsOverWrite, bool IsFromFileDelete = false)
        {
            try
            {
                if (!File.Exists(From_WAV_File))
                {
                    return false;
                }
                if (File.Exists(To_WEM_File) && !IsOverWrite)
                {
                    return false;
                }
                using (FileStream fs = new FileStream(From_WAV_File, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        if (Encoding.ASCII.GetString(br.ReadBytes(4)) != "RIFF")
                        {
                            Audio_Encode_To_Other(From_WAV_File, Voice_Set.Special_Path + "/Wwise/Project/Originals/SFX/song.wav", "wav", false);
                        }
                        else
                        {
                            File.Copy(From_WAV_File, Voice_Set.Special_Path + "/Wwise/Project/Originals/SFX/song.wav", true);
                        }
                    }
                }
                Create_Wwise_Project_XML(Voice_Set.Special_Path + "\\Wwise\\Project");
                Process wwToOgg = new Process();
                wwToOgg.StartInfo.FileName = Voice_Set.Special_Path + "/Wwise/x64/Release/bin/WwiseCLI.exe";
                wwToOgg.StartInfo.WorkingDirectory = Voice_Set.Special_Path + "/Wwise/x64/Release/bin";
                wwToOgg.StartInfo.Arguments = "\"" + Voice_Set.Special_Path + "\\Wwise\\Project\\Template.wproj\" -GenerateSoundBanks";
                wwToOgg.StartInfo.CreateNoWindow = true;
                wwToOgg.StartInfo.UseShellExecute = false;
                wwToOgg.StartInfo.RedirectStandardError = true;
                wwToOgg.StartInfo.RedirectStandardOutput = true;
                wwToOgg.Start();
                wwToOgg.WaitForExit();
                File.Delete(Voice_Set.Special_Path + "/Wwise/Project/Originals/SFX/song.wav");
                string GetWEMFile = Directory.GetFiles(Voice_Set.Special_Path + "/Wwise/Project/.cache/Windows/SFX", "*.wem", SearchOption.TopDirectoryOnly)[0];
                File_Move(GetWEMFile, To_WEM_File, true);
                if (File.Exists(To_WEM_File))
                {
                    if (IsFromFileDelete)
                    {
                        File.Delete(From_WAV_File);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
                return false;
            }
        }
        static void Create_Wwise_Project_XML(string To_Project_Dir)
        {
            try
            {
                StreamWriter stw = File.CreateText(To_Project_Dir + "/GeneratedSoundBanks/Windows/SoundBanksInfo.xml");
                stw.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                          "<SoundBanksInfo Platform=\"Windows\" SchemaVersion=\"10\" SoundbankVersion=\"112\">" +
                          "	<RootPaths>" +
                          "		<ProjectRoot>" + To_Project_Dir + "\\</ProjectRoot>" +
                          "		<SourceFilesRoot>" + To_Project_Dir + "\\.cache\\Windows\\</SourceFilesRoot>\n" +
                          "		<SoundBanksRoot>" + To_Project_Dir + "\\GeneratedSoundBanks\\Windows\\</SoundBanksRoot>\n" +
                          "		<ExternalSourcesInputFile></ExternalSourcesInputFile>\n" +
                          "		<ExternalSourcesOutputRoot>" + To_Project_Dir + "\\GeneratedSoundBanks\\Windows</ExternalSourcesOutputRoot>\n" +
                          "	</RootPaths>\n" +
                          "	<DialogueEvents/>\n" +
                          "	<StreamedFiles>\n" +
                          "		<File Id=\"1071015983\" Language=\"SFX\">\n" +
                          "			<ShortName>song.wav</ShortName>\n" +
                          "			<Path>SFX\\song_B7537E32.wem</Path>\n" +
                          "		</File>\n" +
                          "	</StreamedFiles>\n" +
                          "	<SoundBanks>\n" +
                          "		<SoundBank Id=\"1355168291\" Language=\"SFX\">\n" +
                          "			<ShortName>Init</ShortName>\n" +
                          "			<Path>Init.bnk</Path>\n" +
                          "		</SoundBank>\n" +
                          "		<SoundBank Id=\"2289279978\" Language=\"SFX\">\n" +
                          "			<ShortName>RS_SOUNDBANK</ShortName>\n" +
                          "			<Path>RS_SOUNDBANK.bnk</Path>\n" +
                          "			<ReferencedStreamedFiles>\n" +
                          "				<File Id=\"1071015983\"/>\n" +
                          "			</ReferencedStreamedFiles>\n" +
                          "			<IncludedMemoryFiles>\n" +
                          "				<File Id=\"1071015983\" Language=\"SFX\">\n" +
                          "					<ShortName>song.wav</ShortName>\n" +
                          "					<Path>SFX\\song_B7537E32.wem</Path>\n" +
                          "					<PrefetchSize>0</PrefetchSize>\n" +
                          "				</File>\n" +
                          "			</IncludedMemoryFiles>\n" +
                          "		</SoundBank>\n" +
                          "	</SoundBanks>\n" +
                          "</SoundBanksInfo>\n");
                stw.Close();
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
            }
        }
        //音声のIDをすべて取得(国によって異なるので引数にjaやenを渡します)
        public static List<string> Get_Voices_ID(string Language)
        {
            List<string> Temp = new List<string>();
            if (!File.Exists(Voice_Set.Special_Path + "/Wwise/SoundbanksInfo.json"))
            {
                return Temp;
            }
            try
            {
                StreamReader str = new StreamReader(Voice_Set.Special_Path + "/Wwise/SoundbanksInfo.json");
                string Read_Line = "";
                int Number = 0;
                while ((Read_Line = str.ReadLine()) != null)
                {
                    if (Read_Line == "        \"Language\": \"" + Language + "\",")
                    {
                        string ID = File.ReadLines(Voice_Set.Special_Path + "/Wwise/SoundbanksInfo.json").Skip(Number - 1).First().Replace("        \"Id\": \"", "");
                        string Name = File.ReadLines(Voice_Set.Special_Path + "/Wwise/SoundbanksInfo.json").Skip(Number + 1).First().Replace("        \"ShortName\": \"", "");
                        Temp.Add(Name.Replace("\",", "") + "|" + ID.Replace("\",", ""));
                    }
                    Number++;
                }
                str.Close();
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
            }
            return Temp;
        }
        //SEのIDをすべて取得
        public static List<string> Get_SE_ID(string SE_Name)
        {
            //SEのIDは規則性がないため手動で入力
            List<string> Temp = new List<string>();
            if (SE_Name == "reload")
            {
                Temp.Add("howitzer_load_04.wav|197288924");
                Temp.Add("howitzer_load_01.wav|329813567");
                Temp.Add("howitzer_load_03.wav|379442700");
                Temp.Add("howitzer_load_05.wav|437991333");
            }
            else if (SE_Name == "command")
            {
                Temp.Add("chat_all.wav|680945491");
                Temp.Add("chat_allies.wav|281698299");
                Temp.Add("chat_squad.wav|564473327");
                Temp.Add("quick_commands_attack.wav|521032820");
                Temp.Add("quick_commands_attack_target.wav|553358501");
                Temp.Add("quick_commands_capture_base.wav|569608305");
                Temp.Add("quick_commands_defend_base.wav|326643922");
                Temp.Add("quick_commands_help_me.wav|754223183");
                Temp.Add("quick_commands_negative.wav|34803346");
                Temp.Add("quick_commands_positive.wav|611400702");
                Temp.Add("quick_commands_reloading.wav|1040105232");
            }
            else if (SE_Name == "battle_streamed")
            {
                Temp.Add("adrenalin_off.wav|5395403");
                Temp.Add("adrenalin.wav|145322336");
                Temp.Add("auto_target_off.wav|184826681");
                Temp.Add("auto_target_on.wav|535724311");
                Temp.Add("capture_tick_04.wav|533332128");
                Temp.Add("capture_tick_02.wav|592199082");
                Temp.Add("capture_tick_01.wav|704285192");
                Temp.Add("capture_tick_03.wav|801627646");
                Temp.Add("sirene_01.wav|2052318");
                Temp.Add("capture_end.wav|358568085");
                Temp.Add("shot_no.wav|815477745");
                Temp.Add("shot_no_no.wav|434456653");
                Temp.Add("shot_yes.wav|629250465");
                Temp.Add("shot_yes_yes.wav|982137706");
                Temp.Add("enemy_sighted.wav|244729890");
                Temp.Add("fire_extinguisher_01.wav|836180377");
                Temp.Add("furious_off.wav|236092940");
                Temp.Add("furious.wav|119583868");
                Temp.Add("lamp_01.wav|1033024224");
                Temp.Add("medikit_03.wav|732908901");
                Temp.Add("perk_shooter1.wav|394994028");
                Temp.Add("perk_shooter_max.wav|435001957");
                Temp.Add("quick_commands_close.wav|55835868");
                Temp.Add("quick_commands_open.wav|927236219");
                Temp.Add("repair_08.wav|446072416");
                Temp.Add("repair_06.wav|697244477");
                Temp.Add("restorer.wav|750716839");
                Temp.Add("shell_choose_01.wav|990572193");
                Temp.Add("shell_close_01.wav|971410303");
                Temp.Add("shell_open_01.wav|625795726");
                Temp.Add("sight_convergence_01.wav|361548454");
                Temp.Add("sight_convergence_03.wav|815083652");
                Temp.Add("snipermode_off.wav|990674039");
                Temp.Add("snipermode_on.wav|248236283");
                Temp.Add("stripe.wav|272832027");
                Temp.Add("timer.wav|382538041");
                Temp.Add("zoom_in.wav|66011054");
                Temp.Add("zoom_out.wav|890232967");
            }
            return Temp;
        }
        //音声ファイルの言語を変更
        //例:/Data/WwiseSound/ja/voiceover_crew.bnk -> /Data/WwiseSound/en/voiceover_crew.bnk
        //bnk内のIDが異なるためそのままコピーすることはできません。
        public static void Voice_Change_Language(string From_BNK_File, string To_BNK_File, string Set_Language)
        {
            if (!File.Exists(Voice_Set.Special_Path + "/Wwise/SoundbanksInfo.json") || !File.Exists(From_BNK_File))
            {
                return;
            }
            try
            {
                Wwise_Class.Wwise_File_Extract_V2 Wwise_Bnk = new Wwise_Class.Wwise_File_Extract_V2(From_BNK_File);
                if (Directory.Exists(Voice_Set.Special_Path + "/Voice_Temp"))
                {
                    Directory.Delete(Voice_Set.Special_Path + "/Voice_Temp", true);
                }
                Wwise_Bnk.Wwise_Extract_To_WEM_Directory(Voice_Set.Special_Path + "/Voice_Temp", 1);
                string Get_Language_ID = Wwise_Bnk.Wwise_Get_Name(0);
                Wwise_Bnk.Bank_Clear();
                StreamReader str = new StreamReader(Voice_Set.Special_Path + "/Wwise/SoundbanksInfo.json");
                string Read_Line = "";
                int Number = 0;
                string Get_Language_Now = "";
                while ((Read_Line = str.ReadLine()) != null)
                {
                    if (Read_Line == "        \"Id\": \"" + Get_Language_ID + "\"")
                    {
                        Get_Language_Now = File.ReadLines(Voice_Set.Special_Path + "/Wwise/SoundbanksInfo.json").Skip(Number + 1).First().Replace("        \"Language\": \"", "").Replace("\"", "");
                    }
                    Number++;
                }
                str.Close();
                if (Get_Language_Now != "")
                {
                    List<string> Replace_Name_Voice = Get_Voices_ID(Get_Language_Now);
                    foreach (string Replace_Name_Now in Replace_Name_Voice)
                    {
                        string Name_Only = Replace_Name_Now.Substring(0, Replace_Name_Now.IndexOf(':'));
                        string ID_Only = Replace_Name_Now.Substring(Replace_Name_Now.IndexOf(':') + 1);
                        File_Move(Voice_Set.Special_Path + "/Voice_Temp/" + ID_Only + ".wem", Voice_Set.Special_Path + "/Voice_Temp/" + Name_Only.Replace(".wav", ".wem"), true);
                    }
                    List<string> Get_Set_Language_ID = Get_Voices_ID(Set_Language);
                    Wwise_Class.Wwise_File_Extract_V2 Wwise_Bnk_02 = new Wwise_Class.Wwise_File_Extract_V2(Voice_Set.Special_Path + "/Voice_Temp/voiceover_crew.bnk");
                    List<string> New_ID = Wwise_Bnk_02.Wwise_Get_Names();
                    for (int Number_01 = 0; Number_01 < New_ID.Count; Number_01++)
                    {
                        foreach (string ID in Get_Set_Language_ID)
                        {
                            string Name_Only = ID.Substring(0, ID.IndexOf(':')).Replace(".wav", ".wem");
                            string ID_Only = ID.Substring(ID.IndexOf(':') + 1);
                            if (ID_Only == New_ID[Number_01])
                            {
                                Wwise_Bnk_02.Bank_Edit_Sound(Number_01, Voice_Set.Special_Path + "/Voice_Temp/" + Name_Only, false);
                            }
                        }
                    }
                    Wwise_Bnk_02.Bank_Save(To_BNK_File);
                    Wwise_Bnk_02.Bank_Clear();
                }
                else
                {
                    Error_Log_Write("指定された.bnkファイルは音声ファイルでない可能性があります。");
                }
            }
            catch (Exception e)
            {
                Error_Log_Write(e.Message);
            }
        }
        //ファイル名を変更
        public static void File_Rename_Number(string From_File, string To_File_Name_Only)
        {
            if (!File.Exists(From_File))
            {
                return;
            }
            if (!To_File_Name_Only.Contains("/") && !To_File_Name_Only.Contains("\\"))
            {
                To_File_Name_Only = Path.GetDirectoryName(From_File) + "\\" + To_File_Name_Only;
            }
            int Number = 1;
            while (true)
            {
                if (Number < 10)
                {
                    if (!File_Exists(To_File_Name_Only + "_0" + Number))
                    {
                        File_Move(From_File, To_File_Name_Only + "_0" + Number + Path.GetExtension(From_File), true);
                        return;
                    }
                }
                else
                {
                    if (!File_Exists(To_File_Name_Only + "_" + Number))
                    {
                        File_Move(From_File, To_File_Name_Only + "_" + Number + Path.GetExtension(From_File), true);
                        return;
                    }
                }
                Number++;
            }
        }
        public static string File_Rename_Get_Name(string To_File_Name_Only)
        {
            int Number = 1;
            while (true)
            {
                if (Number < 10)
                {
                    if (!File_Exists(To_File_Name_Only + "_0" + Number))
                    {
                        return To_File_Name_Only + "_0" + Number;
                    }
                }
                else
                {
                    if (!File_Exists(To_File_Name_Only + "_" + Number))
                    {
                        return To_File_Name_Only + "_" + Number;
                    }
                }
                Number++;
            }
        }
        //指定した文字の後に数字があるか(含まれていたらtrue)
        public static bool IsIncludeInt_From_String(string All_String, string Where)
        {
            if (!All_String.Contains(Where))
            {
                return false;
            }
            for (int Number = 0; Number < 10; Number++)
            {
                if (All_String.Contains(Where + Number))
                {
                    return true;
                }
            }
            return false;
        }
    }
}