using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace ImageScroller
{
    public partial class ImageScrollerForm : Form
    {

        Bitmap PlayImg = Properties.Resources.playbutton40;
        Bitmap PlayBlackImg = Properties.Resources.PlayBack;
        Bitmap PauseImg = Properties.Resources.pausebutton40;
        Bitmap PauseBackImg = Properties.Resources.PauseBlack;
        Bitmap StopImg = Properties.Resources.stopbutton40;
        Bitmap StopBlackImg = Properties.Resources.StopBlack;
        Bitmap X1Img = Properties.Resources.x1button40;
        Bitmap X2Img = Properties.Resources.x2button40;
        Bitmap X4Img = Properties.Resources.x4button40;
        Bitmap X8Img = Properties.Resources.x8button40;
        Bitmap X16Img = Properties.Resources.x16button40;
        Bitmap X32Img = Properties.Resources.x32button40;
        Bitmap X1BlackImg = Properties.Resources.x1Black40;
        Bitmap X2BlackImg = Properties.Resources.x2Black4000;
        Bitmap X4BlackImg = Properties.Resources.x4Black40;
        Bitmap X8BlackImg = Properties.Resources.x8Black40;
        Bitmap X16BlackImg = Properties.Resources.x16Black40;
        Bitmap X32BlackImg = Properties.Resources.x32Black40;

        // your data table
        private DataTable dataTable = new DataTable();
        string db_Server;
        string db_Name;
        string db_UserID;
        string db_Password;
        string myIP;
        string FolderOPenpath;
        string batfilepath;
        string cf_UserS = "User_SnapShot";
        string TagType;
        string TagReason;
        string TagImage;
        String Channel_Tag;
        string Path_SysVideo;

        ChannelInfo[] ChannelInfos = new ChannelInfo[9];
        String cp_ID = frm_SaveChannel.cp_ID;
        String cp_proName = frm_SaveChannel.c_proName;
        int i = 0;
        private string conn;
        private MySqlConnection connect;
        private string Path_SaveSnapShot;

        private int scrollIndex = 1;
        private int scrollerMin = 1;
        private int scrollerMax = 86399;
        private String scrollMaxTime = "00:00:00";

        private String fileFormatType = "Number";
        private String fileNameExtension = "jpg";

        private String defaultDateFormat = "yyyy-MM-dd_HH-mm-ss";
        private String fileDateFormat = "yyyy-MM-dd_HH-mm-ss";

        private DateTime referenceDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, (DateTime.Now.Day));//-1
        private DateTime feedDate;

        private String fileNamePrefix = string.Empty;
        private String fileNameSuffix = string.Empty;

        private String fileNameFormat = "{0}{1}{2}.{3}";
        //private String fileBasePath = @"E:\Workspace\Konsultera\Resources\Cams\2017-09-20\{0}";

        Dictionary<int, ChannelInfo> channelSet = new Dictionary<int, ChannelInfo>();
        Dictionary<int, ChannelInfo> cs_ScreenShort = new Dictionary<int, ChannelInfo>();

        public ImageScrollerForm()
        {
            InitializeComponent();
            initChannelDisplayForm();
        }

        #region Data Base
        // DataBase Connection
        private void db_connection()
        {
            try
            {
                Get_DataBaseDetail();
                MySqlConnectionStringBuilder myCSB = new MySqlConnectionStringBuilder();
                myCSB.Server = db_Server;
                myCSB.Database = db_Name;
                myCSB.UserID = db_UserID;
                myCSB.Password = db_Password;
                connect = new MySqlConnection(myCSB.ConnectionString);
                connect.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open mysql connection ! ");
            }
        }

        // Get TextFile For Data Base
        private void Get_DataBaseDetail()
        {
            string txtFileName = "DataBaseConnection.txt";
            string ExeLoaction = System.Reflection.Assembly.GetEntryAssembly().Location; ;
            string txtFilePath = Path.GetDirectoryName(ExeLoaction); // Folder path
            string txtpath = txtFilePath + "\\" + txtFileName;
            try
            {
                if (File.Exists(txtpath))
                {
                    using (StreamReader sr = new StreamReader(txtpath))
                    {
                        while (sr.Peek() >= 0)
                        {
                            string ss = sr.ReadLine();
                            string[] txtsplit = ss.Split(';');

                            //now loop through   array
                            db_Server = txtsplit[0];
                            db_Name = txtsplit[1];
                            db_UserID = txtsplit[2];// user id
                            db_Password = txtsplit[3]; // password
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.ToString());
            }
        }

        // Get Project Id From db
        private void Get_dbProjectID(out string db_ProjectID, ref string db_ProjectName)
        {
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT Project_ID,Project_Type FROM project_detail WHERE Project_name = '" + CurrProjectName_label.Text + "' AND computer_IP = '" + myIP + "' ";
            cmd.Connection = connect;
            MySqlDataReader cChannelReder = cmd.ExecuteReader();
            while (cChannelReder.Read())
            {
                cp_ID = cChannelReder.GetString("Project_ID");
                TagType = cChannelReder.GetString("Project_Type");  // Current Project Type
            }
            cChannelReder.Close();
            connect.Close();
            db_ProjectID = cp_ID;
        }

        // Get Project Check Channel From db 
        private void Get_dbSaveChannel()
        {
            string[] ChannelNameArray = new string[9];
            CheckBox[] CheckBoxArray = new CheckBox[9];
            ChannelNameArray[0] = "Channel_1";
            ChannelNameArray[1] = "Channel_2";
            ChannelNameArray[2] = "Channel_3";
            ChannelNameArray[3] = "Channel_4";
            ChannelNameArray[4] = "Channel_5";
            ChannelNameArray[5] = "Channel_6";
            ChannelNameArray[6] = "Channel_7";
            ChannelNameArray[7] = "Channel_8";
            ChannelNameArray[8] = "Channel_9";

            // check Box array
            CheckBoxArray[0] = channel1_chk;
            CheckBoxArray[1] = channel2_chk;
            CheckBoxArray[2] = channel3_chk;
            CheckBoxArray[3] = channel4_chk;
            CheckBoxArray[4] = channel5_chk;
            CheckBoxArray[5] = channel6_chk;
            CheckBoxArray[6] = channel7_chk;
            CheckBoxArray[7] = channel8_chk;
            CheckBoxArray[8] = channel9_chk;

            string db_ProjectName = CurrProjectName_label.Text;
            string db_ProjectID;

            Get_dbProjectID(out db_ProjectID, ref db_ProjectName);

            for (int i = 0; i < 9; i++)
            {
                string Channel_Name = ChannelNameArray[i];
                db_connection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT path_Snapshot,path_Video,img_Format,file_Type,file_VideoName FROM channel_detail WHERE Channel_name = @Channel_name AND Project_ID = @Project_ID";
                cmd.Parameters.AddWithValue("@Channel_name", Channel_Name);
                cmd.Parameters.AddWithValue("@Project_ID", db_ProjectID);
                cmd.Connection = connect;
                MySqlDataReader PChannelReder = cmd.ExecuteReader();
                if (PChannelReder.HasRows)
                {
                    CheckBoxArray[i].Checked = true;
                }
                else
                {
                    CheckBoxArray[i].Hide();
                }
                PChannelReder.Close();
                connect.Close();
            }
        }
       

        // Get video Path & sanpshot Save from db
        private void Get_dbPath(out string db_VideoPath, out string db_SnapPath, ref string Channel_name) // Get path from db
        {
            db_VideoPath = "";
            db_SnapPath = "";

            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT path_Snapshot,path_Video,img_Format,file_Type FROM channel_detail WHERE Channel_name = @Channel_name AND Project_ID = @Project_ID";

            cmd.Parameters.AddWithValue("@Channel_name", Channel_name);
            cmd.Parameters.AddWithValue("@Project_ID", cp_ID);
            cmd.Connection = connect;
            MySqlDataReader PChannelReder = cmd.ExecuteReader();
            while (PChannelReder.Read())
            {
                String PathSnapshote = PChannelReder.GetString("path_Snapshot"); // Play Snapshot File path
                                                                                 //channelSet[i].FileBasePath = PathSnapshote;
                fileFormatType = PChannelReder.GetString("img_Format"); // image format file
                fileNameExtension = PChannelReder.GetString("file_Type"); // file type
                String Path_video = PChannelReder.GetString("path_Video"); // video path where we select
                Path_SaveSnapShot = Path_video + "\\" + cp_ID +"-" + cp_proName + "\\" + "User_SnapShot"; // save snapshot path
                string Path_User_SnapShot = Path_SaveSnapShot + "\\" + Channel_name; // 
                db_VideoPath = Path_video;
                db_SnapPath = Path_User_SnapShot;

            }
            PChannelReder.Close();
            connect.Close();
        }
        #endregion-----------------------------------------------------------------------------------------------------------------------------------------------------------


        //Save Snap Shot
        private void setChannelPath()//craete by Gaurav
        {
            channelSet.Clear();

            PictureBox[] pictureboxes = new PictureBox[9];
            CheckBox[] CheckBoxArray = new CheckBox[9];
            TextBox[] TextBoxArry = new TextBox[9];
            Panel[] Pannels = new Panel[9];
            string[] ChannelNameArray = new string[9];
            // check Box array
            CheckBoxArray[0] = channel1_chk;
            CheckBoxArray[1] = channel2_chk;
            CheckBoxArray[2] = channel3_chk;
            CheckBoxArray[3] = channel4_chk;
            CheckBoxArray[4] = channel5_chk;
            CheckBoxArray[5] = channel6_chk;
            CheckBoxArray[6] = channel7_chk;
            CheckBoxArray[7] = channel8_chk;
            CheckBoxArray[8] = channel9_chk;
            // picture box array
            pictureboxes[0] = pictureBox1;
            pictureboxes[1] = pictureBox2;
            pictureboxes[2] = pictureBox3;
            pictureboxes[3] = pictureBox4;
            pictureboxes[4] = pictureBox5;
            pictureboxes[5] = pictureBox6;
            pictureboxes[6] = pictureBox7;
            pictureboxes[7] = pictureBox8;
            pictureboxes[8] = pictureBox9;
            // Channel Name array
            //ChannelNameArray[0] = "Channel 1";
            //ChannelNameArray[1] = "Channel 2";
            //ChannelNameArray[2] = "Channel 3";
            //ChannelNameArray[3] = "Channel 4";
            //ChannelNameArray[4] = "Channel 5";
            //ChannelNameArray[5] = "Channel 6";
            //ChannelNameArray[6] = "Channel 7";
            //ChannelNameArray[7] = "Channel 8";
            //ChannelNameArray[8] = "Channel 9";
            ChannelNameArray[0] = "Channel_1";
            ChannelNameArray[1] = "Channel_2";
            ChannelNameArray[2] = "Channel_3";
            ChannelNameArray[3] = "Channel_4";
            ChannelNameArray[4] = "Channel_5";
            ChannelNameArray[5] = "Channel_6";
            ChannelNameArray[6] = "Channel_7";
            ChannelNameArray[7] = "Channel_8";
            ChannelNameArray[8] = "Channel_9";
            // pannels Array
            Pannels[0] = panel2;
            Pannels[1] = panel3;
            Pannels[2] = panel4;
            Pannels[3] = panel5;
            Pannels[4] = panel6;
            Pannels[5] = panel7;
            Pannels[6] = panel8;
            Pannels[7] = panel9;
            Pannels[8] = panel10;

            List<int> CKbox = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                if (CheckBoxArray[i].Checked == true)
                {
                    ChannelInfos[i] = new ChannelInfo();
                    ChannelInfos[i].ImageViewer = pictureboxes[i];
                    ChannelInfos[i].ChannelNumber = i;
                    ChannelInfos[i].FileBasePath = null;
                    channelSet.Add(i, ChannelInfos[i]);

                    channelSet[i].isSelected = CheckBoxArray[i].Checked;

                    string Channel_name = ChannelNameArray[i];

                    db_connection();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "SELECT path_Snapshot,path_Video,img_Format,file_Type FROM channel_detail WHERE Channel_name = @Channel_name AND Project_ID = @Project_ID";
                    cmd.Parameters.AddWithValue("@Channel_name", Channel_name);
                    cmd.Parameters.AddWithValue("@Project_ID", cp_ID);
                    cmd.Connection = connect;
                    MySqlDataReader PChannelReder = cmd.ExecuteReader();
                    while (PChannelReder.Read())
                    {
                        String PathSnapshote = PChannelReder.GetString("path_Snapshot"); // Play Snapshot File path
                        channelSet[i].FileBasePath = PathSnapshote;
                        fileFormatType = PChannelReder.GetString("img_Format"); // image format file
                        fileNameExtension = PChannelReder.GetString("file_Type"); // file type
                        String Path_video = PChannelReder.GetString("path_Video"); // video path where we select

                        // Path_SaveSnapShot = Path_video + "\\" + CurrProjectName_label.Text + "\\" + cf_UserS; // save snapshot path
                        Path_SaveSnapShot = Path_video + "\\" + cp_ID+"-" + cp_proName + "\\" + cf_UserS; // save snapshot path
                    }
                    PChannelReder.Close();
                    connect.Close();

                    //channelSet[i].FileBasePath = TextBoxArry[i].Text;
                    CKbox.Add(ChannelInfos[i].ChannelNumber); // check true channelSet put into CKbox
                    //ChannelInfos[i]
                }
            }
            int count = CKbox.Count;    // count 

            // 1 camera only
            if (count == 1)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (CheckBoxArray[i].Checked != true)
                    {
                        pictureboxes[i].Hide();
                        Pannels[i].Hide();
                    }
                    else
                    {
                        pictureboxes[i].Show(); // 1:1
                        //pictureboxes[i].Location = new System.Drawing.Point(10, 10);
                        pictureboxes[i].Size = new System.Drawing.Size(1140, 580);
                        //  pictureboxes[i].BackColor = System.Drawing.SystemColors.ActiveCaption;
                        Pannels[i].Show(); // 1:1
                        Pannels[i].Location = new System.Drawing.Point(10, 10);
                        Pannels[i].Size = new System.Drawing.Size(1140, 580);
                    }
                }
            }
            // 2:2 camera 4
            else if (count == 2 || count == 3 || count == 4)
            {
                int x = 15; int y = 15;
                int xx = 10; int yy = 10;
                int w = 560; int h = 280;
                for (int i = 0; i < 9; i++)
                {
                    if (CheckBoxArray[i].Checked != true)
                    {
                        pictureboxes[i].Hide();
                        Pannels[i].Hide(); // 1:1
                    }
                    else
                    {
                        pictureboxes[i].Show(); // 1:1
                        if (x == 15 && y == 15)
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                            x = x + w + xx; y = 15;
                        }
                        else if (x == 585 && y == 15) //1:2
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                            x = 15; y = y + h + yy;
                        }
                        else if (x == 15 && y == 305) //2:1
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                            x = x + w + xx; y = 305;
                        }
                        else if (x == 585 && y == 305) //2:2
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                        }
                        pictureboxes[i].Size = new System.Drawing.Size(w, h);
                        Pannels[i].Show(); // 1:1
                                           // pictureboxes[i].BackColor = System.Drawing.SystemColors.ActiveCaption;
                        Pannels[i].Size = new System.Drawing.Size(w, h);
                    }
                }
            }
            // 3:2 Camera 6
            else if (count == 5 || count == 6)
            {
                int x = 17; int y = 20;
                int xx = 17; int yy = 20;
                int w = 364; int h = 270;
                for (int i = 0; i < 9; i++)
                {
                    if (CheckBoxArray[i].Checked != true)
                    {
                        pictureboxes[i].Hide();
                        Pannels[i].Hide(); // 1:1
                    }
                    else
                    {
                        pictureboxes[i].Show(); // 1:1 & 1:2
                        if (x == 17 && y == 20 || (x == 398 && y == 20))
                        {
                            //pictureboxes[i].Location = new System.Drawing.Point(x, y);
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                            x = x + w + xx; y = 20;
                        }
                        else if (x == 779 && y == 20) //1:3
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                            x = 17; y = y + h + yy;
                        }
                        else if ((x == 17 && y == 310) || (x == 398 && y == 310))  //2:1 & 2:2
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                            x = x + w + xx; y = 310;
                        }
                        else if (x == 779 && y == 310) //2:3
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                        }
                        Pannels[i].Show(); // 1:1
                        pictureboxes[i].Size = new System.Drawing.Size(w, h);
                        // pictureboxes[i].BackColor = System.Drawing.SystemColors.ActiveCaption;
                        Pannels[i].Size = new System.Drawing.Size(w, h);
                    }
                }
            }
            // 3:3 camera 9
            else if (count == 7 || count == 8 || count == 9)
            {
                int x = 17; int y = 5;
                int xx = 17; int yy = 5;
                int w = 364; int h = 193;
                for (int i = 0; i < 9; i++)
                {
                    if (CheckBoxArray[i].Checked != true)
                    {
                        pictureboxes[i].Hide();
                        Pannels[i].Hide(); // 1:1
                    }
                    else
                    {
                        pictureboxes[i].Show(); // 1:1 & 1:2
                        if (x == 17 && y == 5 || (x == 398 && y == 5))
                        {
                            //pictureboxes[i].Location = new System.Drawing.Point(x, y);
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                            x = x + w + xx; y = 5;
                        }
                        else if (x == 779 && y == 5) //1:3
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                            x = 17; y = y + h + yy;
                        }
                        else if ((x == 17 && y == 203) || (x == 398 && y == 203))  //2:1 & 2:2
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                            x = x + w + xx; y = 203;
                        }
                        else if (x == 779 && y == 203) //2:3
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                            x = 17; y = y + h + yy;
                        }
                        else if ((x == 17 && y == 401) || (x == 398 && y == 401)) //3:1 & 3:2
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                            x = x + w + xx; y = 401;
                        }
                        else if (x == 779 && y == 401)//3:3
                        {
                            Pannels[i].Location = new System.Drawing.Point(x, y);
                        }
                        Pannels[i].Show(); // 1:1
                        pictureboxes[i].Size = new System.Drawing.Size(w, h);
                        // pictureboxes[i].BackColor = System.Drawing.SystemColors.ActiveCaption;
                        Pannels[i].Size = new System.Drawing.Size(w, h);
                    }
                }
            }
        }

        private void setFileNameSettings()
        {
            referenceDateTime = new DateTime(feedDate.Year, feedDate.Month, feedDate.Day);
        }

        private void initFeed()
        {
            if (fileFormatType.Equals("ToDate", StringComparison.InvariantCultureIgnoreCase))
            {
                scrollerMin = (int)feedDate.TimeOfDay.TotalSeconds;
                if (scrollerMin <= 0)
                {
                    scrollerMin = 1;
                }
            }
            else if (fileFormatType.Equals("Number", StringComparison.InvariantCultureIgnoreCase))
            {
                scrollerMin = 1;
            }
        }

        //Timer
        private void playTimer_Tick_1(object sender, EventArgs e)
        {
            if (scrollIndex >= scrollerMax)
            {
                scrollIndex = scrollerMax;
                pauseScroller();
            }
            else
            {
                scrollIndex++;
            }
            imageScroller.Value = scrollIndex;
        }

        #region Scroller
        // Scroller
        private void initScroller()
        {
            scrollIndex = scrollerMin;
            imageScroller.Minimum = scrollerMin;
            imageScroller.Maximum = scrollerMax;
            imageScroller.Value = scrollIndex;

            scrollMaxTime = getScrollTimeSpan(scrollerMax).ToString();
        }

        private TimeSpan getScrollTimeSpan(int timeSpanSeconds)
        {
            TimeSpan scrollTimeSpan = new TimeSpan();

            scrollTimeSpan = new TimeSpan(0, 0, timeSpanSeconds);

            return scrollTimeSpan;
        }

        private void playScroller()  // Play Scroller
        {
            playTimer.Enabled = true;
            scrollPause.Enabled = true;
            scrollPlay.Enabled = false;
            btn_snapshot.Enabled = true;
            groupBox_Channel.Enabled = false;
        }

        private void imageScroller_ValueChanged(object sender, EventArgs e)// Vlaue Change Scroller
        {
            scrollIndex = imageScroller.Value;
            loadNextImage();
        }

        private void loadNextImage()
        {
            String fileName = getCurrentImageFileName(scrollIndex);

            //imageViewer1.ImageLocation = string.Format(fileBasePath, fileName);
            //imageViewer2.ImageLocation = string.Format(fileBasePath, fileName);
            //imageViewer3.ImageLocation = string.Format(fileBasePath, fileName);
            //imageViewer4.ImageLocation = string.Format(fileBasePath, fileName);
            //imageViewer5.ImageLocation = string.Format(fileBasePath, fileName);
            //imageViewer6.ImageLocation = string.Format(fileBasePath, fileName);
            //imageViewer7.ImageLocation = string.Format(fileBasePath, fileName);
            //imageViewer8.ImageLocation = string.Format(fileBasePath, fileName);

            foreach (KeyValuePair<int, ChannelInfo> channelSetItem in channelSet)
            {
                ChannelInfo _channelInfo = channelSetItem.Value;
                PictureBox _imageViewer = new PictureBox();
                String _fileBasePath = _channelInfo.FileBasePath;

                if (_fileBasePath != null && !string.IsNullOrEmpty(_fileBasePath))
                {
                    if (!_fileBasePath.Substring(_fileBasePath.Length - 1, 1).Equals("\\"))
                    {
                        _fileBasePath = _fileBasePath + "\\";
                    }
                }

                _imageViewer = (PictureBox)_channelInfo.ImageViewer;
                _imageViewer.ImageLocation = _fileBasePath + fileName;
            }

            setCurrentDisplayLabel();
        }

        private void setCurrentDisplayLabel()
        {
            displayLabel.Text = getCurrentPlayTime(scrollIndex) + "/" + scrollMaxTime;
        }

        private void pauseScroller()// pause Scroller 
        {
            playTimer.Enabled = false;
            scrollPlay.Enabled = true;
            scrollPause.Enabled = false;
            btn_snapshot.Enabled = true;
        }

        private void unloadChannels() // Stop Scroller
        {
            channelSet.Clear();
            pauseScroller();
            initScroller();
            initChannels();
            unloadChannel_cleanup();
        }
        #endregion-----------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Snap Shot
        //Save Snap Shot
        private void beforeSave()
        {
            scrollPlay.Enabled = false;
            btn_snapshot.Enabled = false;
        }

        private void afterSave()
        {
            scrollPlay.Enabled = true;
            btn_snapshot.Enabled = true;
        }
        public void saveScreenshot2()
        {            
            TagReason = frm_Popoup._TagReason;
            //DateTime time = DateTime.Now;
            //string Time = time.ToString("h:mm:ss tt");
            //string SnapShortSave = Path_SaveSnapShot + "\\" + time;
            string SnapShortSave = Path_SaveSnapShot + "\\" + TagReason; // SnapShot Save path         
            string[] ChannelNameArray = new string[9];
            CheckBox[] CheckBoxArray = new CheckBox[9];
            // check Box array
            CheckBoxArray[0] = checkBox1;
            CheckBoxArray[1] = checkBox2;
            CheckBoxArray[2] = checkBox3;
            CheckBoxArray[3] = checkBox4;
            CheckBoxArray[4] = checkBox5;
            CheckBoxArray[5] = checkBox6;
            CheckBoxArray[6] = checkBox7;
            CheckBoxArray[7] = checkBox8;
            CheckBoxArray[8] = checkBox9;
            CheckBox[] CheckBoxArray2 = new CheckBox[9];
            CheckBoxArray2[0] = channel1_chk;
            CheckBoxArray2[1] = channel2_chk;
            CheckBoxArray2[2] = channel3_chk;
            CheckBoxArray2[3] = channel4_chk;
            CheckBoxArray2[4] = channel5_chk;
            CheckBoxArray2[5] = channel6_chk;
            CheckBoxArray2[6] = channel7_chk;
            CheckBoxArray2[7] = channel8_chk;
            CheckBoxArray2[8] = channel9_chk;
            string[] CNArray = new string[9];
            CNArray[0] = "Channel_1";
            CNArray[1] = "Channel_2";
            CNArray[2] = "Channel_3";
            CNArray[3] = "Channel_4";
            CNArray[4] = "Channel_5";
            CNArray[5] = "Channel_6";
            CNArray[6] = "Channel_7";
            CNArray[7] = "Channel_8";
            CNArray[8] = "Channel_9";
            for (int i = 0; i < 9; i++)
            {
                if (CheckBoxArray[i].Checked == true)
                {
                    ChannelInfos[i].cs_isSelected = CheckBoxArray[i].Checked;                    
                }
                else if(CheckBoxArray[i].Checked != true && CheckBoxArray2[i].Checked == true)
                {
                    ChannelInfos[i].cs_isSelected = false;
                }                                
            }
            if (SnapShortSave != null && !string.IsNullOrEmpty(SnapShortSave))
            {
                FileOperation fileOps = new FileOperation();
                fileOps.saveFile(channelSet, getCurrentImageFileName(scrollIndex), Path_SaveSnapShot, TagType, TagReason, out string String_Iamges, out string saveSuccess, out string destinationPath);
               
                ChannelInfos[i].cs_isSelected = false;
                if (saveSuccess == "true")
                {
                    MessageBox.Show("Screenshot saved");
                    TagImage = String_Iamges;
                    Channel_Tag = destinationPath;
                    for (int i = 0; i < 9; i++)
                    {
                        if (CheckBoxArray[i].Checked == true)
                        {
                            string Channel_Name = CNArray[i];
                            string Channel_Tag2 = Path_SaveSnapShot + "\\" + TagReason + "\\" + Channel_Name;
                            string pathWithoutLastFolder = Path_SaveSnapShot;
                            Path_SysVideo = Path.GetDirectoryName(pathWithoutLastFolder);
                            Insert_ProjectTag(TagReason, Path_SaveSnapShot, TagImage, Channel_Tag2, Channel_Name, Path_SysVideo);
                        }
                    }
                }
                else
                {
                    // MessageBox.Show("There was an error while attempting to save. Saving operation aborted.");
                    ChannelInfos[i].cs_isSelected = false;
                }
               
            }
            //ChannelInfos[i].cs_isSelected = CheckBoxArray[i].Checked;
            //ChannelInfos[i].cs_isSelected = false;
            afterSave();
            playScroller();
                    
        }

        public void Insert_ProjectTag(String TagReason, String Path_SaveSnapShot, String TagImage, String Channel_Tag, String Channel_Name, String Path_SysVideo)
        {
            string Path_Type = Path_SaveSnapShot + "\\" + TagType;
            string Path_reason = Path_SaveSnapShot + "\\" + TagReason; ;
            string SaveVideoPath = Channel_Tag.Replace("User_SnapShot", "Create_Video");
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "INSERT INTO project_tag(tag_Type,Path_Type,tag_reason,Path_reason,Project_id,tag_image," +
                        "Path_ChannelTag,Channel_Name,Path_SysVideo,Path_VideoTag) " +
                        "values(@TagType,@Path_Type,@TagReason,@Path_reason,@Project_id,@TagImage,@Channel_Tag,@Channel_Name," +
                        "@Path_SysVideo, @Path_VideoTag)";
            cmd.Parameters.AddWithValue("@TagType", TagType);
            cmd.Parameters.AddWithValue("@Path_Type", Path_Type);
            cmd.Parameters.AddWithValue("@TagReason", TagReason);
            cmd.Parameters.AddWithValue("@Path_reason", Path_reason);
            cmd.Parameters.AddWithValue("@Project_id", cp_ID);
            cmd.Parameters.AddWithValue("@TagImage", TagImage);
            cmd.Parameters.AddWithValue("@Channel_Tag", Channel_Tag);
            cmd.Parameters.AddWithValue("@Channel_Name", Channel_Name);
            cmd.Parameters.AddWithValue("@Path_SysVideo", Path_SysVideo);
            cmd.Parameters.AddWithValue("@Path_VideoTag", SaveVideoPath);
            cmd.Connection = connect;
            MySqlDataReader Channel = cmd.ExecuteReader();
            connect.Close();
        }

        private void saveScreenshot()
        {
           // string SnapShortSave = Path_SaveSnapShot;

            string[] ChannelNameArray = new string[9];
            CheckBox[] CheckBoxArray = new CheckBox[9];
            // check Box array
            CheckBoxArray[0] = checkBox1;
            CheckBoxArray[1] = checkBox2;
            CheckBoxArray[2] = checkBox3;
            CheckBoxArray[3] = checkBox4;
            CheckBoxArray[4] = checkBox5;
            CheckBoxArray[5] = checkBox6;
            CheckBoxArray[6] = checkBox7;
            CheckBoxArray[7] = checkBox8;
            CheckBoxArray[8] = checkBox9;

            for (int i = 0; i < 9; i++)
            {
                if (CheckBoxArray[i].Checked == true)
                {
                    ChannelInfos[i].cs_isSelected = CheckBoxArray[i].Checked;
                    if (Path_SaveSnapShot != null && !string.IsNullOrEmpty(Path_SaveSnapShot))
                    {
                        FileOperation fileOps = new FileOperation();

                        if (fileOps.saveFile_old(channelSet, getCurrentImageFileName(scrollIndex), Path_SaveSnapShot, TagType, TagReason))
                        {
                            //MessageBox.Show("Screenshot saved");
                        }
                        else
                        {
                            // MessageBox.Show("There was an error while attempting to save. Saving operation aborted.");
                            ChannelInfos[i].cs_isSelected = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please entera a valid path");
                    }
                }
            }
        }

        private String getCurrentImageFileName(int currentScrollIndex)
        {
            String currentImageFilename = String.Empty;

            if (fileFormatType.Equals("ToDate", StringComparison.InvariantCultureIgnoreCase))
            {
                currentImageFilename = string.Format(fileNameFormat, fileNamePrefix, getCurrentDateTimeString(scrollIndex, fileDateFormat), fileNameSuffix, fileNameExtension);
            }
            else if (fileFormatType.Equals("Number", StringComparison.InvariantCultureIgnoreCase))
            {
                currentImageFilename = string.Format(fileNameFormat, fileNamePrefix, currentScrollIndex, fileNameSuffix, fileNameExtension);
            }

            return currentImageFilename;
        }

        private String getCurrentDateTimeString(int currentScrollIndex, String _fileDateFormat)
        {
            String currentDateTimeString = String.Empty;

            currentDateTimeString = getCurrentScrollDateTime(currentScrollIndex).ToString(_fileDateFormat);

            return currentDateTimeString;
        }

        private DateTime getCurrentScrollDateTime(int currentScrollIndex)
        {
            DateTime currentScrollDateTime = referenceDateTime;

            currentScrollDateTime = currentScrollDateTime.Add(getScrollTimeSpan(currentScrollIndex));

            return currentScrollDateTime;
        }

        private String getCurrentPlayTime(int currentScrollIndex)
        {
            String currentPlayTime = String.Empty;

            currentPlayTime = getScrollTimeSpan(currentScrollIndex).ToString(); ;

            return currentPlayTime;
        }
        #endregion-----------------------------------------------------------------------------------------------------------------------------------------------------------



        // Convert Snap To Video 
        private void Set_SnaptoVideo2()
        {
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT Path_ChannelTag,Channel_Name,tag_reason,path_SysVideo FROM project_tag WHERE Project_ID = @Project_ID";

            cmd.Parameters.AddWithValue("@Project_ID", cp_ID);
            cmd.Connection = connect;
            MySqlDataReader PChannelReder = cmd.ExecuteReader();
            while (PChannelReder.Read())
            {
                string path_TagSnapShort = (PChannelReder["Path_ChannelTag"].ToString());
                string Channel = (PChannelReder["Channel_Name"].ToString());
                string Tag_Reason = (PChannelReder["tag_reason"].ToString());
                string VideoPath = (PChannelReder["path_SysVideo"].ToString());
               
                string _Tag_Reason = Tag_Reason.Replace(" ", "_");
                string ABC = VideoPath + "\\" + "User_SnapShot" + "\\" + Tag_Reason + "\\" + Channel;

                string tempFilename = Path.ChangeExtension(Path.GetTempFileName(), ".bat"); // create bat file
                string ip_Path_Images = ABC + "\\" + "/%%d.jpg";
                string p_Video = ABC;
                string op_path_Video = VideoPath + "\\" + "Create_Video" + "\\" + Tag_Reason + "\\" + Channel + "\\" + "output.mp4";
                String op_path_Video_2 = "\"" + op_path_Video + "\"";

                string TextFile = " MyImageList.txt";
                string batfilename = "temp.bat";
                batfilepath = ABC + "\\" + batfilename;
                string jpg = "(*.jpg)";
                string CreateTextFilepath = ABC + "\\" + "MyImageList.txt";
                string CreateTextFilepath_2 = "\"" + CreateTextFilepath + "\"";
                FolderOPenpath = VideoPath + "\\" + "Create_Video" + "\\" + Tag_Reason + "\\" + Channel;
                string FolderOPenpath2 = VideoPath + "\\" + "User_SnapShot" + "\\" + Tag_Reason + "\\" + Channel;
                if (Directory.Exists(FolderOPenpath2))
                {
                    System.IO.Directory.CreateDirectory(FolderOPenpath); // create folder
                    DirectoryInfo dr = new DirectoryInfo(FolderOPenpath2);
                    FileInfo[] mFile = dr.GetFiles();
                    // Create a Text file
                    using (StreamWriter sw = File.CreateText(CreateTextFilepath))
                    {
                        foreach (FileInfo fiTemp in mFile)
                        {
                            if (fiTemp.Extension == ".jpg")
                            {
                                string LINE = fiTemp.Name;
                                sw.WriteLine("file " + LINE);
                            }
                        }
                    }
                    // Create a bat File 
                    using (StreamWriter writer = new StreamWriter(batfilepath))
                    {
                        //writer.WriteLine("ffmpeg -y -r 1 -f concat -safe 0  -i "+ CreateTextFilepath + "  -c:v libx264 -vf fps=25 -pix_fmt yuv420p output.mp4");   
                        writer.WriteLine("ffmpeg -y -r 1 -f concat -i " + CreateTextFilepath_2 + "  -c:v libx264 -r 25 -pix_fmt yuv420p -t 15 " + op_path_Video_2);
                        //writer.WriteLine("PAUSE");
                    }
                    var startInfo = new ProcessStartInfo();
                    startInfo.WorkingDirectory = ABC;
                    startInfo.FileName = batfilename;
                    startInfo.CreateNoWindow = true;
                    Process process = Process.Start(startInfo);
                    process.WaitForExit();
                    File.Delete(batfilepath);
                    File.Delete(CreateTextFilepath);
                    System.Diagnostics.Process.Start(FolderOPenpath);
                }
            }
            MessageBox.Show("Auditing Completed");
            this.Close();
        }

        private void Set_SnaptoVideo()
        {
            List<int> CKbox = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                // Channel Name array
                string[] ChannelNameArray = new string[9];
                ChannelNameArray[0] = "Channel_1";
                ChannelNameArray[1] = "Channel_2";
                ChannelNameArray[2] = "Channel_3";
                ChannelNameArray[3] = "Channel_4";
                ChannelNameArray[4] = "Channel_5";
                ChannelNameArray[5] = "Channel_6";
                ChannelNameArray[6] = "Channel_7";
                ChannelNameArray[7] = "Channel_8";
                ChannelNameArray[8] = "Channel_9";

                string Channel_name = ChannelNameArray[i];

                Get_dbPath(out string db_VideoPath, out string db_SnapPath, ref Channel_name);

                string tempFilename = Path.ChangeExtension(Path.GetTempFileName(), ".bat"); // create bat file
                //string ip_Path_Images = "C:\\Gaurav\\ImageScroller\\Doc\\Gaurav\\720\\System_SnapShot/%%d.jpg";
                //string op_path_Video = "C:\\Gaurav\\ImageScroller\\Doc\\Gaurav\\720\\output.mp4";
                string ip_Path_Images = db_SnapPath + "/%%d.jpg";

                string p_Video = db_VideoPath + "\\" + cp_ID + "-" + cp_proName + "\\" + "Create_Video" + "\\" + Channel_name;
                //string op_path_Video = p_Video + "\\" + "output.mp4";
                string op_path_Video = p_Video + "\\" + Channel_name + "output.mp4";
                string TextFile = " MyImageList.txt";
                string batfilename = "temp.bat";
                batfilepath = db_SnapPath + "\\" + batfilename;
                string jpg = "(*.jpg)";
                string CreateTextFilepath = @db_SnapPath + "\\" + "MyImageList.txt";
                FolderOPenpath = @db_SnapPath;
                if (FolderOPenpath != "")
                {
                    System.IO.Directory.CreateDirectory(p_Video); // create folder
                    DirectoryInfo dr = new DirectoryInfo(FolderOPenpath);
                    FileInfo[] mFile = dr.GetFiles();
                    // Create a Text file
                    using (StreamWriter sw = File.CreateText(CreateTextFilepath))
                    {
                        foreach (FileInfo fiTemp in mFile)
                        {
                            if (fiTemp.Extension == ".jpg")
                            {
                                string LINE = fiTemp.Name;
                                sw.WriteLine("file " + LINE);
                            }
                        }
                    }
                    // Create a bat File 
                    using (StreamWriter writer = new StreamWriter(batfilepath))
                    {
                        //writer.WriteLine("ffmpeg -y -r 1 -f concat -safe 0  -i "+ CreateTextFilepath + "  -c:v libx264 -vf fps=25 -pix_fmt yuv420p output.mp4");     
                        writer.WriteLine("ffmpeg -y -r 1 -f concat -i " + CreateTextFilepath + "  -c:v libx264 -r 25 -pix_fmt yuv420p -t 15 " + op_path_Video);
                        //writer.WriteLine("PAUSE");
                    }
                    var startInfo = new ProcessStartInfo();
                    startInfo.WorkingDirectory = db_SnapPath;
                    startInfo.FileName = batfilename;
                    startInfo.CreateNoWindow = true;
                    Process process = Process.Start(startInfo);
                    process.WaitForExit();
                    File.Delete(batfilepath);
                    File.Delete(CreateTextFilepath);
                    System.Diagnostics.Process.Start(p_Video);
                }
                // Process.Start(FolderOPenpath);               
            }
        }

        // Import Excel All Db data
        private void Create_Excel()
        {
            DataTable dataTable = new DataTable { TableName = "MyTableName" };
            try
            {
                string Path_CSVFile;
                string Save_CSVFile;
               
                db_connection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT Project_id, tag_type, tag_reason, Channel_Name, tag_image, Path_ChannelTag, Path_VideoTag, Path_SysVideo, " +
                                    "date_time FROM `project_tag` WHERE Project_id = '" + cp_ID + "'";
                cmd.Connection = connect;               

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dataTable);                              
            
                    BindingSource bSource = new BindingSource();
                bSource.DataSource = dataTable;

                StringBuilder sb = new StringBuilder();

                IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dataTable.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }
                
                //string prijectid = (dataTable.Rows[-1][7]).ToString();
                Path_CSVFile = (dataTable.Rows[0][7]).ToString();  //Path_VideoTag
                string FileCSVName = "AuditComplete - " + cp_ID + " - " + cp_proName + ".csv";
                Save_CSVFile = Path_CSVFile + "\\" + FileCSVName;   
                File.WriteAllText(Save_CSVFile, sb.ToString());
                //File.WriteAllText("C:\\_Gaurav\\C#\\ImageScroller\\MySql\\Acti Audit\\Testing Video\\194-BB 2\\test.csv", sb.ToString());

                ////DataSet ds = new DataSet("New_DataSet");
                ////ds.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;
                ////da.Fill(dataTable);
                ////ds.Tables.Add(dataTable);

                ////dataTable.WriteXml("C:\\_Gaurav\\C#\\ImageScroller\\MySql\\Acti Audit\\Testing Video\\193-BB 1\\agent.xlsx");
                MessageBox.Show("Export Data Successfully");
                connect.Close();


                //Excel.Application oXL;
                //Excel._Workbook oWB;
                //Excel._Worksheet oSheet;
                //Excel.Range oRng;

                //////try
                //////{
                //////    //Start Excel and get Application object.
                //////    oXL = new Excel.Application();
                //////    oXL.Visible = true;

                //////    //Get a new workbook.
                //////    oWB = (Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
                //////    oSheet = (Excel._Worksheet)oWB.ActiveSheet;

                //////    //Add table headers going cell by cell.
                //////    oSheet.Cells[1, 1] = "First Name";
                //////    oSheet.Cells[1, 2] = "Last Name";
                //////    oSheet.Cells[1, 3] = "Full Name";
                //////    oSheet.Cells[1, 4] = "Salary";

                //////    //Format A1:D1 as bold, vertica
                //////}
                //////catch (Exception theException)
                //////{
                //////    String errorMessage;
                //////    errorMessage = "Error: ";
                //////    errorMessage = String.Concat(errorMessage, theException.Message);
                //////    errorMessage = String.Concat(errorMessage, " Line: ");
                //////    errorMessage = String.Concat(errorMessage, theException.Source);

                //////    MessageBox.Show(errorMessage, "Error");
                //////}
                //MySqlDataReader cChannelReder = cmd.ExecuteReader();
                //while (cChannelReder.Read())
                //{
                //    cp_ID = cChannelReder.GetString("Project_ID");
                //    TagType = cChannelReder.GetString("Project_Type");  // Current Project Type
                //}
                //cChannelReder.Close();
                //connect.Close();
                //db_ProjectID = cp_ID;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }     



        #region Load , Unload
        // init
        private void initChannelDisplayForm()
        {
            initSelection();
            initChannels();
        }
        private void initSelection()
        {
            unloadChannel_cleanup(); ;
        }
        private void unloadChannel_cleanup()
        {
            // controls_grp.Enabled = false;
            btn_snapshot.Enabled = false;
        }
        private void initChannels()
        {
            channelSet.Clear();
            btn_snapshot.Enabled = true;
            groupBox_Channel.Enabled = true;
            //btn_Back.Enabled = true;
        }
        
        // Play the Video & Load the Channels      
        private void loadChannels()
        {
            loadChannel_cleanup(); 
            setChannelPath();           
            setFileNameSettings();
            initFeed();
            initScroller();
        }
        private void loadChannel_cleanup()
        {          
            controls_grp.Enabled = true;
            btn_snapshot.Enabled = true;
            btn_Stop.Enabled = true;
        }
        #endregion-----------------------------------------------------------------------------------------------------------------------------------------------------------


        #region Load, Click
        // Load the Channel Set
        private void channel1_chk_Click(object sender, EventArgs e)
        {
            loadChannels(); // Load Channels
        }

        //
        private void FormLoad()
        {
            myIP = frm_SaveChannel.myIP;
            // CurrProjectName_label.Text = frm_SaveChannel.label2.Text;
            CurrProjectName_label.Text = frm_SaveChannel.c_proName;
            lab_ProjectType.Text = frm_SaveChannel.c_proType;
            Curr_ProjectID.Text = cp_ID;
            Get_dbSaveChannel(); // Get Chaecck DB Save Channel 
            loadChannels(); // Load Channels
            btn_Stop.Enabled = false;
            scrollPause.Enabled = false;
            btn_snapshot.Enabled = false;
        }
        // Form Load
        private void ImageScrollerForm_Load_1(object sender, EventArgs e)
        {
            FormLoad();
            PB_play.Image = PlayImg;
            PB_Stop.Image = StopImg;
            PB_X.Image = X1Img;
        }

        // btn Play
        private void scrollPlay_Click_1(object sender, EventArgs e)
        {
            btn_2x.Text = "1X";
            playTimer.Interval = 1000;
            playScroller(); // Play Channels
            btn_snapshot.Enabled = true;
            btn_Stop.Enabled = true;
            scrollPause.Enabled = true;
        }

        // btn Pause
        private void scrollPause_Click_1(object sender, EventArgs e)
        {
            pauseScroller();
        }

        // btn Stop
        private void btn_Stop_Click(object sender, EventArgs e)
        {
            unloadChannels();
            FormLoad();
        }

        // btn Snapshot
        private void btn_snapshot_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false && checkBox2.Checked == false && checkBox3.Checked == false && checkBox4.Checked == false && checkBox5.Checked == false &&
                checkBox6.Checked == false && checkBox7.Checked == false && checkBox8.Checked == false && checkBox9.Checked == false)
            {
                MessageBox.Show("Select Channel For SnapShot");
            }
            else
            {
                beforeSave();
                pauseScroller();
                // saveScreenshot();
                frm_Popoup fP = new frm_Popoup(this);
                fP.Show();
                //saveScreenshot();
                //afterSave();
            }
        }

        // btn Convert Snap To Video 
        private void btn_SnapTOVideo_Click(object sender, EventArgs e)
        {
            Set_SnaptoVideo2();
            //Create_Excel();

        }
        // Back & and Unload Channels
        private void btn_Back_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm_SaveChannel FSC = new frm_SaveChannel();
            FSC.Show();

            channelSet.Clear();
            pauseScroller();
            initScroller();
            initChannels();
            unloadChannel_cleanup();
        }

        private void ImageScrollerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //frm_SaveChannel C_FSC = new frm_SaveChannel();
            //C_FSC.Show();
        }

        private void btn_Load_Click(object sender, EventArgs e)
        {
            //loadChannels(); // Load Channels
        }

       
        #endregion-----------------------------------------------------------------------------------------------------------------------------------------------------------


        #region Mouse Click Event
        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {           
            if (checkBox1.Checked != true)
            {
                pictureBox1.Padding = new Padding(3);
                pictureBox1.BackColor = Color.Red;
                checkBox1.Checked = true;
            }
            else
            {
                pictureBox1.Padding = new Padding(0);
                pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
                checkBox1.Checked = false;
            }
        }
        private void panel3_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox2.Checked != true)
            {
                pictureBox2.Padding = new Padding(3);
                pictureBox2.BackColor = Color.Red;
                checkBox2.Checked = true;
            }
            else
            {
                pictureBox2.Padding = new Padding(0);
                pictureBox2.BackColor = System.Drawing.SystemColors.ActiveCaption;
                checkBox2.Checked = false;
            }
        }
        private void panel4_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox3.Checked != true)
            {
                pictureBox3.Padding = new Padding(3);
                pictureBox3.BackColor = Color.Red;
                checkBox3.Checked = true;
            }
            else
            {
                pictureBox3.Padding = new Padding(0);
                pictureBox3.BackColor = System.Drawing.SystemColors.ActiveCaption;
                checkBox3.Checked = false;
            }
        }
        private void panel5_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox4.Checked != true)
            {
                pictureBox4.Padding = new Padding(3);
                pictureBox4.BackColor = Color.Red;
                checkBox4.Checked = true;
            }
            else
            {
                pictureBox4.Padding = new Padding(0);
                pictureBox4.BackColor = System.Drawing.SystemColors.ActiveCaption;
                checkBox4.Checked = false;
            }
        }
        private void panel6_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox5.Checked != true)
            {
                pictureBox5.Padding = new Padding(3);
                pictureBox5.BackColor = Color.Red;
                checkBox5.Checked = true;
            }
            else
            {
                pictureBox5.Padding = new Padding(0);
                pictureBox5.BackColor = System.Drawing.SystemColors.ActiveCaption;
                checkBox5.Checked = false;
            }
        }
        private void panel7_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox6.Checked != true)
            {
                pictureBox6.Padding = new Padding(3);
                pictureBox6.BackColor = Color.Red;
                checkBox6.Checked = true;
            }
            else
            {
                pictureBox6.Padding = new Padding(0);
                pictureBox6.BackColor = System.Drawing.SystemColors.ActiveCaption;
                checkBox6.Checked = false;
            }
        }
        private void panel8_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox7.Checked != true)
            {
                pictureBox7.Padding = new Padding(3);
                pictureBox7.BackColor = Color.Red;
                checkBox7.Checked = true;
            }
            else
            {
                pictureBox7.Padding = new Padding(0);
                pictureBox7.BackColor = System.Drawing.SystemColors.ActiveCaption;
                checkBox7.Checked = false;
            }
        }
        private void panel9_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox8.Checked != true)
            {
                pictureBox8.Padding = new Padding(3);
                pictureBox8.BackColor = Color.Red;
                checkBox8.Checked = true;
            }
            else
            {
                pictureBox8.Padding = new Padding(0);
                pictureBox8.BackColor = System.Drawing.SystemColors.ActiveCaption;
                checkBox8.Checked = false;
            }
        }
        private void panel10_MouseClick(object sender, MouseEventArgs e)
        {

            if (checkBox9.Checked != true)
            {
                pictureBox9.Padding = new Padding(3);
                pictureBox9.BackColor = Color.Red;
                checkBox9.Checked = true;
            }
            else
            {
                pictureBox9.Padding = new Padding(0);
                pictureBox9.BackColor = System.Drawing.SystemColors.ActiveCaption;
                checkBox9.Checked = false;
            }
        }


        #endregion

        private void btn_2x_Click(object sender, EventArgs e)
        {
            if (btn_2x.Text == "1X")
            {
                playTimer.Interval = 500;
                scrollPlay.Enabled = true;
                btn_2x.Text = "2X";
            }
            else if (btn_2x.Text == "2X")
            {
                playTimer.Interval = 250;
                scrollPlay.Enabled = true;
                btn_2x.Text = "4X";
            }
            else if (btn_2x.Text == "4X")
            {
                playTimer.Interval = 125;
                scrollPlay.Enabled = true;
                btn_2x.Text = "8X";
            }
            else if (btn_2x.Text == "8X")
            {
                playTimer.Interval = 62;
                scrollPlay.Enabled = true;
                btn_2x.Text = "16X";
            }
            else if (btn_2x.Text == "16X")
            {
                playTimer.Interval = 31;
                scrollPlay.Enabled = true;
                btn_2x.Text = "32X";
            }
            else if (btn_2x.Text == "32X")
            {
                playTimer.Interval = 1000;
                scrollPlay.Enabled = true;
                btn_2x.Text = "1X";
            }
        }                  

        private void button1_Click_1(object sender, EventArgs e)
        {
            Create_Excel();
        }

        private void PB_play_Click(object sender, EventArgs e)
        {
            if (PB_play.Image == PlayImg || PB_play.Image == PlayBlackImg)
            {
                //btn_2x.Text = "1X";
                //playTimer.Interval = 1000;
                playScroller(); // Play Channels
                btn_snapshot.Enabled = true;
                btn_Stop.Enabled = true;
                scrollPause.Enabled = true;
                btn_SnapTOVideo.Enabled = false;
                button1.Enabled = false;
                PB_play.Image = PauseBackImg;
            }
            else
            {
                PB_play.Image = PlayBlackImg;
                pauseScroller();
            }
        }

        private void PB_Stop_Click(object sender, EventArgs e)
        {
            unloadChannels();
            FormLoad();
            PB_play.Image = PlayImg;
            btn_2x.Text = "1X";
            playTimer.Interval = 1000;
            PB_X.Image = X1Img;
            btn_SnapTOVideo.Enabled = true;
            button1.Enabled = true;
        }

        private void PB_X_Click(object sender, EventArgs e)
        {
            if (btn_2x.Text == "1X")
            {
                playTimer.Interval = 500;
                scrollPlay.Enabled = true;
                btn_2x.Text = "2X";
                PB_X.Image = X2BlackImg;
            }
            else if (btn_2x.Text == "2X")
            {
                playTimer.Interval = 250;
                scrollPlay.Enabled = true;
                btn_2x.Text = "4X";
                PB_X.Image = X4BlackImg;
            }
            else if (btn_2x.Text == "4X")
            {
                playTimer.Interval = 125;
                scrollPlay.Enabled = true;
                btn_2x.Text = "8X";
                PB_X.Image = X8BlackImg;
            }
            else if (btn_2x.Text == "8X")
            {
                playTimer.Interval = 62;
                scrollPlay.Enabled = true;
                btn_2x.Text = "16X";
                PB_X.Image = X16BlackImg;
            }
            else if (btn_2x.Text == "16X")
            {
                playTimer.Interval = 31;
                scrollPlay.Enabled = true;
                btn_2x.Text = "32X";
                PB_X.Image = X32BlackImg;
            }
            else if (btn_2x.Text == "32X")
            {
                playTimer.Interval = 1000;
                scrollPlay.Enabled = true;
                btn_2x.Text = "1X";
                PB_X.Image = X1BlackImg;
            }
        }

        private void PB_play_MouseEnter(object sender, EventArgs e)
        {
            if (PB_play.Image == PlayImg)
            {
                PB_play.Image = PlayBlackImg;
            }
            else if (PB_play.Image == PauseImg)
            {
                PB_play.Image = PauseBackImg;
            }
        }

        private void PB_play_MouseLeave(object sender, EventArgs e)
        {
            if (PB_play.Image == PlayBlackImg)
            {
                PB_play.Image = PlayImg;
            }
            else if (PB_play.Image == PauseBackImg)
            {
                PB_play.Image = PauseImg;
            }
        }

        private void PB_Stop_MouseEnter(object sender, EventArgs e)
        {
            PB_Stop.Image = StopBlackImg;
        }

        private void PB_Stop_MouseLeave(object sender, EventArgs e)
        {
            PB_Stop.Image = StopImg;
        }

        private void PB_X_MouseEnter(object sender, EventArgs e)
        {
            if (PB_X.Image == X1Img)
            { PB_X.Image = X1BlackImg; }
            else if (PB_X.Image == X2Img)
            { PB_X.Image = X2BlackImg; }
            else if (PB_X.Image == X4Img)
            { PB_X.Image = X4BlackImg; }
            else if (PB_X.Image == X8Img)
            { PB_X.Image = X8BlackImg; }
            else if (PB_X.Image == X16Img)
            { PB_X.Image = X16BlackImg; }
            else if (PB_X.Image == X32Img)
            { PB_X.Image = X32BlackImg; }
        }

        private void PB_X_MouseLeave(object sender, EventArgs e)
        {
            if (PB_X.Image == X1BlackImg)
            { PB_X.Image = X1Img; }
            else if (PB_X.Image == X2BlackImg)
            { PB_X.Image = X2Img; }
            else if (PB_X.Image == X4BlackImg)
            { PB_X.Image = X4Img; }
            else if (PB_X.Image == X8BlackImg)
            { PB_X.Image = X8Img; }
            else if (PB_X.Image == X16BlackImg)
            { PB_X.Image = X16Img; }
            else if (PB_X.Image == X32BlackImg)
            { PB_X.Image = X32Img; }
        }
    }
}
