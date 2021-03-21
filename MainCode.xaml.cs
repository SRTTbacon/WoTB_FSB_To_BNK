using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;
using WoTB_Voice_Mod_Creater;
using WoTB_Voice_Mod_Creater.Class;
using WoTB_Voice_Mod_Creater.FMOD;

public class Fmod_Player
{
    static Cauldron.FMOD.EventSystem ESystem_01 = new Cauldron.FMOD.EventSystem();
    public static Cauldron.FMOD.EventSystem ESystem
    {
        get { return ESystem_01; }
        set { ESystem_01 = value; }
    }
}
namespace WoTB_FSB_To_BNK
{
    public partial class MainCode : Window
    {
        string Voice_FSB_File = "";
        bool IsClosing = false;
        bool IsMessageShowing = false;
        Cauldron.FMOD.EVENT_LOADINFO ELI = new Cauldron.FMOD.EVENT_LOADINFO();
        Cauldron.FMOD.EventProject EP = new Cauldron.FMOD.EventProject();
        Cauldron.FMOD.EventGroup EG = new Cauldron.FMOD.EventGroup();
        Cauldron.FMOD.Event FE = new Cauldron.FMOD.Event();
        public MainCode()
        {
            InitializeComponent();
            try
            {
                string Path = Directory.GetCurrentDirectory();
                StreamWriter stw = File.CreateText(Path + "/Test.dat");
                stw.WriteLine("Test File");
                stw.Close();
                File.Delete(Path + "/Test.dat");
            }
            catch
            {
                System.Windows.MessageBox.Show("The folder could not be accessed. You need to move the software to another location.");
                System.Windows.Application.Current.Shutdown();
            }
            FMod_List_Clear();
            Voice_Rename_Init();
            Fmod_Player.ESystem.Init(128, Cauldron.FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
            FMOD_API.System FModSys = new FMOD_API.System();
            FMOD_API.Factory.System_Create(ref FModSys);
            Fmod_System.FModSystem = FModSys;
            Fmod_System.FModSystem.init(16, FMOD_API.INITFLAGS.NORMAL, IntPtr.Zero);
            System.Drawing.Size MaxSize = Screen.PrimaryScreen.WorkingArea.Size;
            MaxWidth = MaxSize.Width;
            MaxHeight = MaxSize.Height;
            Window_Show();
        }
        //変更後のファイル名の定義
        void Voice_Rename_Init()
        {
            Voice_Set.Voice_BGM_Change_List.Clear();
            for (int Number = 0; Number < 37; Number++)
            {
                Voice_Set.Voice_BGM_Change_List.Add(new List<string>());
            }
            string line;
            int Number_01 = -1;
            StreamReader str = new StreamReader(Voice_Set.Special_Path + "/Wwise/Change_To_Wwise.dat");
            while ((line = str.ReadLine()) != null)
            {
                if (line[0] == '・')
                {
                    Number_01++;
                    continue;
                }
                Voice_Set.Voice_BGM_Change_List[Number_01].Add(line.Trim());
            }
            str.Close();
        }
        //リストを初期化
        void FMod_List_Clear()
        {
            FSB_Details_L.Items.Clear();
            Voice_Add_List.Items.Clear();
            FSB_Details_L.Items.Add("FSB File:Not selected");
            FSB_Details_L.Items.Add("Number of voices:None");
            FSB_Details_L.Items.Add("Output:Unspecified");
            FSB_Details_L.Items.Add("SE:Enable(Can't be changed)");
            Voice_Select_T.Text = "Not selected";
        }
        //画面を表示
        async void Window_Show()
        {
            Opacity = 0;
            while (Opacity < 1 && !IsClosing)
            {
                Opacity += Sub_Code.Window_Feed_Time;
                await Task.Delay(1000 / 60);
            }
        }
        //戻る
        private async void Exit_B_Click(object sender, RoutedEventArgs e)
        {
            if (!IsClosing)
            {
                IsClosing = true;
                while (Opacity > 0)
                {
                    Opacity -= Sub_Code.Window_Feed_Time;
                    await Task.Delay(1000 / 60);
                }
                IsClosing = false;
                System.Windows.Application.Current.Shutdown();
            }
        }
        //テキストの表示
        async void Message_Feed_Out(string Message)
        {
            if (IsMessageShowing)
            {
                IsMessageShowing = false;
                await Task.Delay(1000 / 30);
            }
            Message_T.Text = Message;
            IsMessageShowing = true;
            Message_T.Opacity = 1;
            int Number = 0;
            while (Message_T.Opacity > 0 && IsMessageShowing)
            {
                Number++;
                if (Number >= 120)
                {
                    Message_T.Opacity -= 0.025;
                }
                await Task.Delay(1000 / 60);
            }
            IsMessageShowing = false;
            Message_T.Text = "";
            Message_T.Opacity = 1;
        }
        //FModの音声ファイルを選択
        private void Voice_Select_B_Click(object sender, RoutedEventArgs e)
        {
            if (IsClosing)
                return;
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Please select an FSB file.",
                Filter = "FSB File(*.fsb)|*.fsb",
                InitialDirectory = Sub_Code.Get_OpenFile_Path(),
                Multiselect = false
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Sub_Code.Set_OpenFile_Path(Path.GetDirectoryName(ofd.FileName));
                bool IsVoiceExist = false;
                FMod_List_Clear();
                List<string> Voices = Fmod_Class.FSB_GetNames(ofd.FileName);
                foreach (string File_Now in Voices)
                {
                    string File_Now_01 = File_Now.Replace(" ", "");
                    Voice_Add_List.Items.Add(File_Now_01);
                    if (File_Now_01.Contains("battle_01") || File_Now_01.Contains("battle_02") || File_Now_01.Contains("battle_03") || File_Now_01.Contains("start_battle_01"))
                    {
                        IsVoiceExist = true;
                    }
                }
                if (!IsVoiceExist)
                {
                    Message_Feed_Out("The specified file is not supported.");
                    Voice_Add_List.Items.Clear();
                    return;
                }
                Voices.Clear();
                Voice_FSB_File = ofd.FileName;
                FSB_Details_L.Items[0] = "FSB File:" + Path.GetFileName(ofd.FileName);
                FSB_Details_L.Items[1] = "Number of voices:" + Fmod_Class.FSB_GetLength(ofd.FileName) + "個";
                Voice_Select_T.Text = Path.GetFileName(ofd.FileName);
            }
        }
        //音声、BGMともに初期化
        private void Clear_B_Click(object sender, RoutedEventArgs e)
        {
            if (IsClosing)
                return;
            MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to clear the contents?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                FMod_List_Clear();
                Voice_FSB_File = "";
            }
        }
        //ヘルプ
        private void Help_B_Click(object sender, RoutedEventArgs e)
        {
            if (IsClosing)
                return;
            string Message_01 = "Crew voice created by FMod are limited to those created by other audio mods whose file names have not been changed. \n";
            string Message_02 = "Example1:Battle start->start_battle_01 | Penetration->armor_pierced_by_player_01\n";
            string Message_03 = "Example2:Battle start->battle_01 | Penetration->kantuu_01 etc...\n";
            string Message_04 = "It may not work well depending on the contents of FSB.";
            System.Windows.MessageBox.Show(Message_01 + Message_02 + Message_03 + Message_04);
        }
        //変換
        //fsbからwavファイルを抽出し、ファイル名を変更、adpcmの場合ファイルが破損しているため復元してから.bnkを作成
        private async void Chnage_To_Wwise_B_Click(object sender, RoutedEventArgs e)
        {
            if (IsClosing || Opacity < 1)
                return;
            try
            {
                IsClosing = true;
                BetterFolderBrowser sfd = new BetterFolderBrowser()
                {
                    Title = "Please specify the save destination folder.",
                    RootFolder = Sub_Code.Get_OpenDirectory_Path(),
                    Multiselect = false
                };
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Sub_Code.Set_Directory_Path(sfd.SelectedFolder);
                    FSB_Details_L.Items[2] = "Output:" + sfd.SelectedFolder.Substring(sfd.SelectedFolder.LastIndexOf("\\") + 1);
                    if (Directory.Exists(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices_TMP"))
                    {
                        Directory.Delete(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices_TMP", true);
                    }
                    if (Directory.Exists(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices"))
                    {
                        Directory.Delete(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices", true);
                    }
                    Message_T.Text = "Extracting audio from FSB file...";
                    await Task.Delay(50);
                    Fmod_File_Extract_V2.FSB_Extract_To_Directory(Voice_FSB_File, Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices_TMP");
                    Message_T.Text = "Encoding *.wav files...";
                    await Task.Delay(50);
                    await Multithread.Convert_To_Wav(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices_TMP", Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices", true);
                    Directory.Delete(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices_TMP", true);
                    Message_T.Text = "Renaming *.wav files...";
                    await Task.Delay(50);
                    Voice_Set.Voice_BGM_Name_Change_From_FSB(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices");
                    string[] Reload_Files = Directory.GetFiles(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices", "reload_*", SearchOption.TopDirectoryOnly);
                    foreach (string Reload_Now in Reload_Files)
                    {
                        FileInfo fi_reload = new FileInfo(Reload_Now);
                        if (fi_reload.Length == 290340 || fi_reload.Length == 335796 || fi_reload.Length == 336036 || fi_reload.Length == 445836 || fi_reload.Length == 497268 || fi_reload.Length == 541980)
                        {
                            fi_reload.Delete();
                        }
                    }
                    string[] Voice_Files = Directory.GetFiles(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices", "*.wav", SearchOption.TopDirectoryOnly);
                    //音声の場合はたいていファイル名の語尾に_01や_02と書いているため、書かれていないファイルは削除する
                    foreach (string Voice_Now in Voice_Files)
                    {
                        if (!Path.GetFileNameWithoutExtension(Voice_Now).Contains("_") || !Sub_Code.IsIncludeInt_From_String(Path.GetFileNameWithoutExtension(Voice_Now), "_"))
                        {
                            File.Delete(Voice_Now);
                        }
                    }
                    if (File.Exists(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices/lock_on.wav"))
                    {
                        File.Delete(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices/lock_on.wav");
                    }
                    Message_T.Text = "Creating a BNK file...";
                    await Task.Delay(50);
                    FileInfo fi = new FileInfo(Voice_Set.Special_Path + "/Wwise/WoTB_Sound_Mod/Actor-Mixer Hierarchy/Default Work Unit.wwu");
                    if (File.Exists(Voice_Set.Special_Path + "/Wwise/WoTB_Sound_Mod/Actor-Mixer Hierarchy/Backup.tmp") && fi.Length >= 800000)
                    {
                        File.Copy(Voice_Set.Special_Path + "/Wwise/WoTB_Sound_Mod/Actor-Mixer Hierarchy/Backup.tmp", Voice_Set.Special_Path + "/Wwise/WoTB_Sound_Mod/Actor-Mixer Hierarchy/Default Work Unit.wwu", true);
                    }
                    if (!File.Exists(Voice_Set.Special_Path + "/Wwise/WoTB_Sound_Mod/Actor-Mixer Hierarchy/Backup.tmp"))
                    {
                        File.Copy(Voice_Set.Special_Path + "/Wwise/WoTB_Sound_Mod/Actor-Mixer Hierarchy/Default Work Unit.wwu", Voice_Set.Special_Path + "/Wwise/WoTB_Sound_Mod/Actor-Mixer Hierarchy/Backup.tmp", true);
                    }
                    WoTB_Voice_Mod_Creater.Wwise_Class.Wwise_Project_Create Wwise = new WoTB_Voice_Mod_Creater.Wwise_Class.Wwise_Project_Create(Voice_Set.Special_Path + "/Wwise/WoTB_Sound_Mod");
                    Wwise.Sound_Add_Wwise(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices");
                    Wwise.Save();
                    Directory.Delete(Voice_Set.Special_Path + "/Wwise/FSB_Extract_Voices", true);
                    Wwise.Project_Build("voiceover_crew", sfd.SelectedFolder + "/voiceover_crew.bnk");
                    /*await Task.Delay(500);
                    Wwise.Project_Build("ui_battle", sfd.SelectedFolder + "/ui_battle.bnk");
                    await Task.Delay(500);
                    Wwise.Project_Build("ui_chat_quick_commands", sfd.SelectedFolder + "/ui_chat_quick_commands.bnk");
                    await Task.Delay(500);
                    Wwise.Project_Build("reload", sfd.SelectedFolder + "/reload.bnk");*/
                    Wwise.Clear();
                    if (File.Exists(Voice_Set.Special_Path + "/Wwise/WoTB_Sound_Mod/Actor-Mixer Hierarchy/Backup.tmp"))
                    {
                        File.Copy(Voice_Set.Special_Path + "/Wwise/WoTB_Sound_Mod/Actor-Mixer Hierarchy/Backup.tmp", Voice_Set.Special_Path + "/Wwise/WoTB_Sound_Mod/Actor-Mixer Hierarchy/Default Work Unit.wwu", true);
                    }
                    if (DVPL_C.IsChecked.Value)
                    {
                        Message_T.Text = "Encoding to DVPL format...";
                        await Task.Delay(50);
                        DVPL.DVPL_Pack(sfd.SelectedFolder + "/voiceover_crew.bnk", sfd.SelectedFolder + "/voiceover_crew.bnk.dvpl", true);
                        DVPL.DVPL_Pack(sfd.SelectedFolder + "/ui_battle.bnk", sfd.SelectedFolder + "/ui_battle.bnk.dvpl", true);
                        DVPL.DVPL_Pack(sfd.SelectedFolder + "/ui_chat_quick_commands.bnk", sfd.SelectedFolder + "/ui_chat_quick_commands.bnk.dvpl", true);
                        DVPL.DVPL_Pack(sfd.SelectedFolder + "/reload.bnk", sfd.SelectedFolder + "/reload.bnk.dvpl", true);
                    }
                    Message_Feed_Out("The operation is complete. If the bnk file is extremely small in size, it may have failed.");
                }
            }
            catch (Exception e1)
            {
                Sub_Code.Error_Log_Write(e1.Message);
                Message_Feed_Out("An error has occurred. See Log.txt for details.");
            }
            IsClosing = false;
        }
        private void FSB_Details_L_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FSB_Details_L.SelectedIndex = -1;
        }
        private void Play_B_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Stop_B_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}