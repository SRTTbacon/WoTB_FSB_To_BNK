using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
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
        bool IsMouseDown = false;
        bool IsPause = false;
        bool IsPlaying = false;
        bool IsLocationMouseChange = false;
        FMOD_API.Sound MainSound = new FMOD_API.Sound();
        FMOD_API.Sound SubSound = new FMOD_API.Sound();
        FMOD_API.Channel FModChannel = new FMOD_API.Channel();
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
            //Sliderにクリック判定がないため強制的に判定を付ける
            Location_S.AddHandler(MouseDownEvent, new MouseButtonEventHandler(Location_MouseDown), true);
            Location_S.AddHandler(MouseUpEvent, new MouseButtonEventHandler(Location_MouseUp), true);
            Volume_S.Value = 50;
            Position_Change();
            Window_Show();
        }
        async void Position_Change()
        {
            //サウンドの位置をSliderに反映
            while (!IsClosing)
            {
                if (Voice_FSB_File != "")
                {
                    bool IsPaused = false;
                    FModChannel.getPaused(ref IsPaused);
                    FModChannel.isPlaying(ref IsPlaying);
                    if (!IsMouseDown)
                    {
                        if (!IsPaused && !IsPlaying)
                        {
                            Sound_Start();
                        }
                        if (!IsPaused && !IsLocationMouseChange)
                        {
                            Set_Position_TextBlock(true);
                        }
                    }
                }
                await Task.Delay(1000 / 30);
            }
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
                float Volume_Down = (float)(Volume_S.Value / 100 / 30);
                while (Opacity > 0)
                {
                    Opacity -= Sub_Code.Window_Feed_Time;
                    float Volume_Now = 1f;
                    FModChannel.getVolume(ref Volume_Now);
                    FModChannel.setVolume(Volume_Now - Volume_Down);
                    await Task.Delay(1000 / 60);
                }
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
                SubSound.release();
                MainSound.release();
                Location_T.Text = "00:00";
                Location_S.Value = 0;
                Location_S.Maximum = 0;
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
                Voice_FSB_File = ofd.FileName;
                FSB_Details_L.Items[0] = "FSB File:" + Path.GetFileName(ofd.FileName);
                FSB_Details_L.Items[1] = "Number of voices:" + Voices.Count + "個";
                Voice_Select_T.Text = Path.GetFileName(ofd.FileName);
                Voices.Clear();
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
                Voice_FSB_File = "";
                FModChannel.setPaused(true);
                FMod_List_Clear();
                Location_S.Value = 0;
                Location_S.Maximum = 0;
                Location_T.Text = "00:00";
                FModChannel = new FMOD_API.Channel();
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
            if (Voice_FSB_File == "")
            {
                Message_Feed_Out("FSB file is not selected.");
                return;
            }
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
                    FModChannel.setPaused(true);
                    await Task.Delay(50);
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
                        /*DVPL.DVPL_Pack(sfd.SelectedFolder + "/ui_battle.bnk", sfd.SelectedFolder + "/ui_battle.bnk.dvpl", true);
                        DVPL.DVPL_Pack(sfd.SelectedFolder + "/ui_chat_quick_commands.bnk", sfd.SelectedFolder + "/ui_chat_quick_commands.bnk.dvpl", true);
                        DVPL.DVPL_Pack(sfd.SelectedFolder + "/reload.bnk", sfd.SelectedFolder + "/reload.bnk.dvpl", true);*/
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
        private void FSB_Details_L_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FSB_Details_L.SelectedIndex = -1;
        }
        private void Play_B_Click(object sender, RoutedEventArgs e)
        {
            if (IsClosing)
            {
                return;
            }
            if (Voice_Add_List.SelectedIndex == -1)
            {
                Message_Feed_Out("Voice File is not selected.");
                return;
            }
            FModChannel.setPaused(false);
        }
        private void Stop_B_Click(object sender, RoutedEventArgs e)
        {
            FModChannel.setPaused(true);
        }
        private void Voice_Add_List_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (IsClosing || Voice_Add_List.SelectedIndex == -1)
            {
                return;
            }
            Sound_Start();
            uint Sound_Length = 0;
            SubSound.getLength(ref Sound_Length, FMOD_API.TIMEUNIT.MS);
            Location_S.Value = 0;
            Location_S.Maximum = Sound_Length;
            Location_T.Text = "00:00";
        }
        private void Volume_S_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Volume_T.Text = "Volume:" + (int)Volume_S.Value;
            FModChannel.setVolume((float)(Volume_S.Value / 100));
        }
        //選択されている音声を再生
        void Sound_Start()
        {
            if (Voice_FSB_File == "" || Voice_Add_List.SelectedIndex == -1)
            {
                return;
            }
            FModChannel.setPaused(true);
            FModChannel = new FMOD_API.Channel();
            SubSound.release();
            MainSound.release();
            Fmod_System.FModSystem.createSound(Voice_FSB_File, FMOD_API.MODE.CREATESTREAM, ref MainSound);
            MainSound.getSubSound(Voice_Add_List.SelectedIndex, ref SubSound);
            Fmod_System.FModSystem.playSound(FMOD_API.CHANNELINDEX.FREE, SubSound, true, ref FModChannel);
            FModChannel.setVolume((float)(Volume_S.Value / 100));
        }
        //サウンドの現在の時間をテキストボックスに反映
        void Set_Position_TextBlock(bool IsSetSoundPosition)
        {
            if (IsSetSoundPosition && !IsLocationMouseChange)
            {
                uint Position_Now = 0;
                FModChannel.getPosition(ref Position_Now, FMOD_API.TIMEUNIT.MS);
                Location_S.Value = Position_Now;
            }
            TimeSpan Time = TimeSpan.FromMilliseconds(Location_S.Value);
            string Minutes = Time.Minutes.ToString();
            string Seconds = Time.Seconds.ToString();
            if (Time.Minutes < 10)
            {
                Minutes = "0" + Time.Minutes;
            }
            if (Time.Seconds < 10)
            {
                Seconds = "0" + Time.Seconds;
            }
            Location_T.Text = Minutes + ":" + Seconds;
        }
        //音声の再生位置を変更
        private void Location_Board_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            FModChannel.getPaused(ref IsPause);
            FModChannel.setPaused(true);
            System.Drawing.Point p = new System.Drawing.Point();
            int w = Screen.GetBounds(p).Width;
            double Width_Display_From_1920 = (double)w / 1920;
            int Location_Mouse_X_Display = Math.Abs((int)Location_S.PointToScreen(new Point()).X - System.Windows.Forms.Cursor.Position.X) - 10;
            double Percent = Location_Mouse_X_Display / (260 * Width_Display_From_1920);
            Location_S.Value = Location_S.Maximum * Percent;
            FModChannel.setPosition((uint)Location_S.Value, FMOD_API.TIMEUNIT.MS);
            if (!IsPause)
            {
                FModChannel.setPaused(false);
            }
            Set_Position_TextBlock(false);
            IsMouseDown = false;
        }
        //マウスがある位置まで再生時間を移動
        void Location_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IsMouseDown = true;
            FModChannel.getPaused(ref IsPause);
            FModChannel.setPaused(true);
            //計算大変だった...
            System.Drawing.Point p = new System.Drawing.Point();
            int w = Screen.GetBounds(p).Width;
            double Width_Display_From_1920 = (double)w / 1920;
            int Location_Mouse_X_Display = Math.Abs((int)Location_S.PointToScreen(new Point()).X - System.Windows.Forms.Cursor.Position.X) - 10;
            double Percent = Location_Mouse_X_Display / (double)(260 * Width_Display_From_1920);
            Location_S.Value = Location_S.Maximum * Percent;
        }
        //Sliderの位置をサウンドに反映
        void Location_MouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FModChannel.setPosition((uint)Location_S.Value, FMOD_API.TIMEUNIT.MS);
            IsLocationMouseChange = true;
            IsMouseDown = false;
            if (!IsPause)
            {
                bool IsPauseNow = false;
                FModChannel.getPaused(ref IsPauseNow);
                if (IsPauseNow)
                {
                    FModChannel.setPaused(false);
                }
            }
            Set_Position_TextBlock(true);
            IsLocationMouseChange = false;
        }
        private void Location_S_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsMouseDown)
            {
                Set_Position_TextBlock(false);
            }
        }
    }
}