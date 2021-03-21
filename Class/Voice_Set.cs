using System.Collections.Generic;
using System.IO;

namespace WoTB_Voice_Mod_Creater
{
    public class Voice_Set
    {
        public static List<List<string>> Voice_BGM_Change_List = new List<List<string>>();
        static List<string> Voice_Lists = new List<string>();
        static List<bool> SE_Enable_Disable = new List<bool>();
        static string Special_Path_Dir = Directory.GetCurrentDirectory() + "\\Resources";
        static string Server_Name = "";
        static string User_Name = "";
        static string WoTB_Location = "";
        static int Voice_Number = 0;
        static bool IsBusy = false;
        public static List<string> Voice_Files
        {
            get { return Voice_Lists; }
            set { Voice_Lists = value; }
        }
        public static string SRTTbacon_Server_Name
        {
            get { return Server_Name; }
            set { Server_Name = value; }
        }
        public static int Voice_Files_Number
        {
            get { return Voice_Number; }
            set { Voice_Number = value; }
        }
        public static string UserName
        {
            get { return User_Name; }
            set { User_Name = value; }
        }
        public static List<bool> SE_Enable_List
        {
            get { return SE_Enable_Disable; }
            set { SE_Enable_Disable = value; }
        }
        public static bool App_Busy
        {
            get { return IsBusy; }
            set { IsBusy = value; }
        }
        public static string WoTB_Path
        {
            get { return WoTB_Location; }
            set { WoTB_Location = value; }
        }
        public static string Special_Path
        {
            get { return Special_Path_Dir; }
            set { Special_Path_Dir = value; }
        }
        //ファイル名が既に変更されたものか
        public static bool Voice_Name_Hide(string File)
        {
            string a = File;
            if (a.Contains("mikata") || a.Contains("danyaku") || a.Contains("hikantuu") || a.Contains("kantuu") || a.Contains("tokusyu") || a.Contains("tyoudan") || a.Contains("syatyou") ||
                a.Contains("souzyuusyu") || a.Contains("tekikasai") || a.Contains("gekiha") || a.Contains("enjinhason") || a.Contains("enjintaiha") || a.Contains("enjinhukkyuu") || a.Contains("kasai") ||
                a.Contains("syouka") || a.Contains("nenryou") || a.Contains("housinhason") || a.Contains("housintaiha") || a.Contains("housinhukkyuu") || a.Contains("housyu") || a.Contains("soutensyu") ||
                a.Contains("musen") || a.Contains("musensyu") || a.Contains("battle") || a.Contains("kansokuhason") || a.Contains("kansokutaiha") || a.Contains("kansokuhukkyuu") || a.Contains("ritaihason") ||
                a.Contains("ritaitaiha") || a.Contains("ritaihukkyuu") || a.Contains("houtouhason") || a.Contains("houtoutaiha") || a.Contains("houtouhukkyuu") || a.Contains("taiha") || a.Contains("hakken") ||
                a.Contains("lamp") || a.Contains("ryoukai") || a.Contains("kyohi") || a.Contains("help") || a.Contains("attack") || a.Contains("attack_now") || a.Contains("capture") || a.Contains("defence") ||
                a.Contains("keep") || a.Contains("lock") || a.Contains("unlock") || a.Contains("reload") || a.Contains("map") || a.Contains("battle_end"))
            {
                return true;
            }
            return false;
        }
        //音声の日本語の種類を取得
        public static string Get_Voice_Type_Japanese_Name(string Voice_Name)
        {
            if (Voice_Name == "mikata")
            {
                return "フレンドリーファイヤー";
            }
            else if (Voice_Name == "danyaku")
            {
                return "弾薬庫損傷";
            }
            else if (Voice_Name == "hikantuu")
            {
                return "敵への非貫通";
            }
            else if (Voice_Name == "kantuu")
            {
                return "敵への貫通";
            }
            else if (Voice_Name == "tokusyu")
            {
                return "敵へのモジュール損傷";
            }
            else if (Voice_Name == "tyoudan")
            {
                return "敵への跳弾";
            }
            else if (Voice_Name == "syatyou")
            {
                return "車長負傷";
            }
            else if (Voice_Name == "souzyuusyu")
            {
                return "操縦手負傷";
            }
            else if (Voice_Name == "tekikasai")
            {
                return "敵車両火災発生";
            }
            else if (Voice_Name == "gekiha")
            {
                return "敵車両撃破";
            }
            else if (Voice_Name == "enjinhason")
            {
                return "エンジン損傷";
            }
            else if (Voice_Name == "enjintaiha")
            {
                return "エンジン大破";
            }
            else if (Voice_Name == "enjinhukkyuu")
            {
                return "エンジン復旧";
            }
            else if (Voice_Name == "kasai")
            {
                return "自車両火災発生";
            }
            else if (Voice_Name == "syouka")
            {
                return "自車両消火";
            }
            else if (Voice_Name == "nenryou")
            {
                return "燃料タンク損傷";
            }
            else if (Voice_Name == "housinhason")
            {
                return "砲身破損";
            }
            else if (Voice_Name == "housintaiha")
            {
                return "砲身大破";
            }
            else if (Voice_Name == "housinhukkyuu")
            {
                return "砲身復旧";
            }
            else if (Voice_Name == "housyu")
            {
                return "砲手負傷";
            }
            else if (Voice_Name == "soutensyu")
            {
                return "装填手負傷";
            }
            else if (Voice_Name == "musen")
            {
                return "無線機破損";
            }
            else if (Voice_Name == "musensyu")
            {
                return "無選手負傷";
            }
            else if (Voice_Name == "battle")
            {
                return "戦闘開始";
            }
            else if (Voice_Name == "kansokuhason")
            {
                return "観測装置破損";
            }
            else if (Voice_Name == "kansokutaiha")
            {
                return "観測装置大破";
            }
            else if (Voice_Name == "kansokuhukkyuu")
            {
                return "観測装置復旧";
            }
            else if (Voice_Name == "ritaihason")
            {
                return "履帯破損";
            }
            else if (Voice_Name == "ritaitaiha")
            {
                return "履帯大破";
            }
            else if (Voice_Name == "ritaihukkyuu")
            {
                return "履帯復旧";
            }
            else if (Voice_Name == "houtouhason")
            {
                return "砲塔破損";
            }
            else if (Voice_Name == "houtoutaiha")
            {
                return "砲塔大破";
            }
            else if (Voice_Name == "houtouhukkyuu")
            {
                return "砲塔復旧";
            }
            else if (Voice_Name == "taiha")
            {
                return "自車両大破";
            }
            else if (Voice_Name == "hakken")
            {
                return "敵発見";
            }
            else if (Voice_Name == "lamp")
            {
                return "第六感";
            }
            else if (Voice_Name == "ryoukai")
            {
                return "了解";
            }
            else if (Voice_Name == "kyohi")
            {
                return "拒否";
            }
            else if (Voice_Name == "help")
            {
                return "救援を請う";
            }
            else if (Voice_Name == "attack")
            {
                return "攻撃せよ！";
            }
            else if (Voice_Name == "attack_now")
            {
                return "攻撃中";
            }
            else if (Voice_Name == "capture")
            {
                return "陣地を占領せよ！";
            }
            else if (Voice_Name == "defence")
            {
                return "陣地を防衛せよ！";
            }
            else if (Voice_Name == "keep")
            {
                return "固守せよ！";
            }
            else if (Voice_Name == "lock")
            {
                return "ロックオン";
            }
            else if (Voice_Name == "unlock")
            {
                return "アンロック";
            }
            else if (Voice_Name == "reload")
            {
                return "装填完了";
            }
            else if (Voice_Name == "map")
            {
                return "マップクリック時";
            }
            else if (Voice_Name == "battle_end")
            {
                return "戦闘終了時";
            }
            else if (Voice_Name == "battle_bgm")
            {
                return "戦闘BGM";
            }
            else
            {
                return "";
            }
        }
        //音声の日本語の種類を取得(V2)
        public static string Get_Voice_Type_Japanese_Name_V2(int Voice_Number)
        {
            if (Voice_Number == 0)
            {
                return "フレンドリーファイヤー";
            }
            else if (Voice_Number == 1)
            {
                return "弾薬庫損傷";
            }
            else if (Voice_Number == 2)
            {
                return "敵への非貫通";
            }
            else if (Voice_Number == 3)
            {
                return "敵への貫通";
            }
            else if (Voice_Number == 4)
            {
                return "敵へのモジュール損傷";
            }
            else if (Voice_Number == 5)
            {
                return "敵への跳弾";
            }
            else if (Voice_Number == 6)
            {
                return "車長負傷";
            }
            else if (Voice_Number == 7)
            {
                return "操縦手負傷";
            }
            else if (Voice_Number == 8)
            {
                return "敵車両火災発生";
            }
            else if (Voice_Number == 9)
            {
                return "敵車両撃破";
            }
            else if (Voice_Number == 10)
            {
                return "エンジン損傷";
            }
            else if (Voice_Number == 11)
            {
                return "エンジン大破";
            }
            else if (Voice_Number == 12)
            {
                return "エンジン復旧";
            }
            else if (Voice_Number == 13)
            {
                return "自車両火災発生";
            }
            else if (Voice_Number == 14)
            {
                return "自車両消火";
            }
            else if (Voice_Number == 15)
            {
                return "燃料タンク損傷";
            }
            else if (Voice_Number == 16)
            {
                return "砲身破損";
            }
            else if (Voice_Number == 17)
            {
                return "砲身大破";
            }
            else if (Voice_Number == 18)
            {
                return "砲身復旧";
            }
            else if (Voice_Number == 19)
            {
                return "砲手負傷";
            }
            else if (Voice_Number == 20)
            {
                return "装填手負傷";
            }
            else if (Voice_Number == 21)
            {
                return "無線機破損";
            }
            else if (Voice_Number == 22)
            {
                return "無選手負傷";
            }
            else if (Voice_Number == 23)
            {
                return "戦闘開始";
            }
            else if (Voice_Number == 24)
            {
                return "観測装置破損";
            }
            else if (Voice_Number == 25)
            {
                return "観測装置大破";
            }
            else if (Voice_Number == 26)
            {
                return "観測装置復旧";
            }
            else if (Voice_Number == 27)
            {
                return "履帯破損";
            }
            else if (Voice_Number == 28)
            {
                return "履帯大破";
            }
            else if (Voice_Number == 29)
            {
                return "履帯復旧";
            }
            else if (Voice_Number == 30)
            {
                return "砲塔破損";
            }
            else if (Voice_Number == 31)
            {
                return "砲塔大破";
            }
            else if (Voice_Number == 32)
            {
                return "砲塔復旧";
            }
            else if (Voice_Number == 33)
            {
                return "自車両大破";
            }
            else if (Voice_Number == 34)
            {
                return "敵発見";
            }
            else if (Voice_Number == 35)
            {
                return "第六感";
            }
            else if (Voice_Number == 36)
            {
                return "了解";
            }
            else if (Voice_Number == 37)
            {
                return "拒否";
            }
            else if (Voice_Number == 38)
            {
                return "救援を請う";
            }
            else if (Voice_Number == 39)
            {
                return "攻撃せよ！";
            }
            else if (Voice_Number == 40)
            {
                return "攻撃中";
            }
            else if (Voice_Number == 41)
            {
                return "陣地を占領せよ！";
            }
            else if (Voice_Number == 42)
            {
                return "陣地を防衛せよ！";
            }
            else if (Voice_Number == 43)
            {
                return "固守せよ！";
            }
            else if (Voice_Number == 44)
            {
                return "ロックオン";
            }
            else if (Voice_Number == 45)
            {
                return "アンロック";
            }
            else if (Voice_Number == 46)
            {
                return "装填完了";
            }
            else if (Voice_Number == 47)
            {
                return "マップクリック時";
            }
            else if (Voice_Number == 48)
            {
                return "戦闘終了時";
            }
            else if (Voice_Number == 49)
            {
                return "戦闘BGM";
            }
            else
            {
                return "";
            }
        }
        //インデックスからローマ字の音声タイプを取得
        public static string Get_Voice_Type_Romaji_Name(int Index)
        {
            if (Index == 0)
            {
                return "mikata";
            }
            else if (Index == 1)
            {
                return "danyaku";
            }
            else if (Index == 2)
            {
                return "hikantuu";
            }
            else if (Index == 3)
            {
                return "kantuu";
            }
            else if (Index == 4)
            {
                return "tokusyu";
            }
            else if (Index == 5)
            {
                return "tyoudan";
            }
            else if (Index == 6)
            {
                return "syatyou";
            }
            else if (Index == 7)
            {
                return "souzyuusyu";
            }
            else if (Index == 8)
            {
                return "tekikasai";
            }
            else if (Index == 9)
            {
                return "gekiha";
            }
            else if (Index == 10)
            {
                return "enjinhason";
            }
            else if (Index == 11)
            {
                return "enjintaiha";
            }
            else if (Index == 12)
            {
                return "enjinhukkyuu";
            }
            else if (Index == 13)
            {
                return "kasai";
            }
            else if (Index == 14)
            {
                return "syouka";
            }
            else if (Index == 15)
            {
                return "nenryou";
            }
            else if (Index == 16)
            {
                return "housinhason";
            }
            else if (Index == 17)
            {
                return "housintaiha";
            }
            else if (Index == 18)
            {
                return "housinhukkyuu";
            }
            else if (Index == 19)
            {
                return "housyu";
            }
            else if (Index == 20)
            {
                return "soutensyu";
            }
            else if (Index == 21)
            {
                return "musen";
            }
            else if (Index == 22)
            {
                return "musensyu";
            }
            else if (Index == 23)
            {
                return "battle";
            }
            else if (Index == 24)
            {
                return "kansokuhason";
            }
            else if (Index == 25)
            {
                return "kansokutaiha";
            }
            else if (Index == 26)
            {
                return "kansokuhukkyuu";
            }
            else if (Index == 27)
            {
                return "ritaihason";
            }
            else if (Index == 28)
            {
                return "ritaitaiha";
            }
            else if (Index == 29)
            {
                return "ritaihukkyuu";
            }
            else if (Index == 30)
            {
                return "houtouhason";
            }
            else if (Index == 31)
            {
                return "houtoutaiha";
            }
            else if (Index == 32)
            {
                return "houtouhukkyuu";
            }
            else if (Index == 33)
            {
                return "taiha";
            }
            else if (Index == 34)
            {
                return "hakken";
            }
            else if (Index == 35)
            {
                return "lamp";
            }
            else if (Index == 36)
            {
                return "ryoukai";
            }
            else if (Index == 37)
            {
                return "kyohi";
            }
            else if (Index == 38)
            {
                return "help";
            }
            else if (Index == 39)
            {
                return "attack";
            }
            else if (Index == 40)
            {
                return "attack_now";
            }
            else if (Index == 41)
            {
                return "capture";
            }
            else if (Index == 42)
            {
                return "defence";
            }
            else if (Index == 43)
            {
                return "keep";
            }
            else if (Index == 44)
            {
                return "lock";
            }
            else if (Index == 45)
            {
                return "unlock";
            }
            else if (Index == 46)
            {
                return "reload";
            }
            else if (Index == 47)
            {
                return "map";
            }
            else if (Index == 48)
            {
                return "battle_end";
            }
            else if (Index == 49)
            {
                return "battle_bgm";
            }
            else
            {
                return "";
            }
        }
        public static void Voice_BGM_Name_Change_From_FSB(string Dir_Path)
        {
            if (!Directory.Exists(Dir_Path))
            {
                return;
            }
            string[] To_File_Name = { "mikata", "danyaku", "hikantuu", "kantuu", "tokusyu", "tyoudan", "syatyou", "souzyuusyu", "tekikasai", "gekiha", "enjinhason", "enjintaiha", "enjinhukkyuu"
            ,"kasai","syouka","nenryou","housinhason","housintaiha","housinhukkyuu","housyu","soutensyu","musen","musensyu","battle","kansokuhason","kansokutaiha"
            ,"kansokuhukkyuu","ritaihason","ritaitaiha","ritaihukkyuu","houtouhason","houtoutaiha","houtouhukkyuu","taiha","battle_bgm","reload","touzyouin"};
            string[] Files = Directory.GetFiles(Dir_Path, "*.wav", SearchOption.TopDirectoryOnly);
            foreach (string File_Now in Files)
            {
                if (Path.GetFileNameWithoutExtension(File_Now).Contains("_"))
                {
                    string Name = Path.GetFileNameWithoutExtension(File_Now).Substring(0, Path.GetFileNameWithoutExtension(File_Now).LastIndexOf('_'));
                    Name = Name.Trim();
                    for (int Number = 0; Number < 37; Number++)
                    {
                        foreach (string Voice_Name in Voice_BGM_Change_List[Number])
                        {
                            if (Name.Contains(Voice_Name))
                            {
                                if (Number == 36)
                                {
                                    File.Copy(File_Now, Sub_Code.File_Rename_Get_Name(Path.GetDirectoryName(File_Now) + "\\syatyou") + ".wav", true);
                                    File.Copy(File_Now, Sub_Code.File_Rename_Get_Name(Path.GetDirectoryName(File_Now) + "\\souzyuusyu") + ".wav", true);
                                    File.Copy(File_Now, Sub_Code.File_Rename_Get_Name(Path.GetDirectoryName(File_Now) + "\\housyu") + ".wav", true);
                                    File.Copy(File_Now, Sub_Code.File_Rename_Get_Name(Path.GetDirectoryName(File_Now) + "\\housyu") + ".wav", true);
                                    File.Copy(File_Now, Sub_Code.File_Rename_Get_Name(Path.GetDirectoryName(File_Now) + "\\soutensyu") + ".wav", true);
                                    Sub_Code.File_Move(File_Now, Sub_Code.File_Rename_Get_Name(Path.GetDirectoryName(File_Now) + "\\musensyu") + ".wav", true);
                                    continue;
                                }
                                Sub_Code.File_Rename_Number(File_Now, To_File_Name[Number]);
                            }
                        }
                    }
                }
            }
        }
    }
}
public class Voice_Mod_Create
{
    //例:"battle_01.mp3"->"battle"
    public static string Get_Voice_Type_V1(string FilePath)
    {
        string NameOnly = Path.GetFileName(FilePath);
        if (!NameOnly.Contains("_"))
        {
            return NameOnly;
        }
        return NameOnly.Substring(0, NameOnly.LastIndexOf('_'));
    }
    public static string Get_Voice_Type_V2(string FilePath)
    {
        string NameOnly = Path.GetFileName(FilePath);
        if (!NameOnly.Contains("_"))
        {
            return NameOnly;
        }
        return NameOnly.Substring(0, NameOnly.IndexOf('_'));
    }
    //音声の種類のみを抽出
    //種類が被っていたらスキップ
    public static string[] Get_Voice_Type_Only(string[] Voices)
    {
        List<string> Type_List = new List<string>();
        foreach (string Type in Voices)
        {
            string Type_Name = Get_Voice_Type_V1(Type);
            bool IsOK = true;
            for (int Number = 0; Number <= Type_List.Count - 1; Number++)
            {
                if (Type_Name == Type_List[Number])
                {
                    IsOK = false;
                    break;
                }
            }
            if (IsOK)
            {
                Type_List.Add(Type_Name);
            }
        }
        return Type_List.ToArray();
    }
}