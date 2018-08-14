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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace ImageScroller
{
    public partial class ImageScrollerForm : Form
    {
        int X_Mouse;
        int Y_Mouse;

        string db_Server; // Database
        string db_Name;
        string db_UserID;
        string db_Password;
        string myIP;  // Computer IP
        private string conn;
        private MySqlConnection connect;

        String cp_ID = frm_SaveChannel.cp_ID;  // Project ID
        String cp_proName = frm_SaveChannel.c_proName; // Project Name

        private DataTable dataTable = new DataTable();

        private int scrollIndex;
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

        Dictionary<int, ChannelInfo> channelSet = new Dictionary<int, ChannelInfo>();
        Dictionary<int, ChannelInfo> cs_ScreenShort = new Dictionary<int, ChannelInfo>();
        ChannelInfo[] ChannelInfos = new ChannelInfo[9];
       
        Bitmap PlayImg = Properties.Resources.playbutton40; // Set PlayBlue Image
        Bitmap PlayBlackImg = Properties.Resources.PlayBack;
        Bitmap PauseImg = Properties.Resources.pausebutton40;
        Bitmap PauseBackImg = Properties.Resources.PauseBlack;
        Bitmap StopImg = Properties.Resources.stopbutton40;
        Bitmap StopBlackImg = Properties.Resources.StopBlack;       

        Bitmap ContinueGryImg = Properties.Resources.Continue_30_grey;
        Bitmap ContinueOrgImg = Properties.Resources.Continue_30_orange;

        Bitmap AuditGryImg = Properties.Resources.audit_30_grey;
        Bitmap AuditOrgImg = Properties.Resources.audit_30_orange;

        Bitmap FileGryImg = Properties.Resources.file_30_grey;
        Bitmap FileOrgImg = Properties.Resources.file_30_orange;

        Bitmap StopGryImg = Properties.Resources.stop_30_grey;
        Bitmap StopOrgImg = Properties.Resources.stop_30_orange;

        Bitmap PauseGryImg = Properties.Resources.pause_30_grey;
        Bitmap PauseOrgImg = Properties.Resources.pause_30_orange;

        Bitmap PlayGryImg = Properties.Resources.play_30_grey;
        Bitmap PlayOrgImg = Properties.Resources.play_30_orange;

        Bitmap x1GryImg = Properties.Resources._1x_30_gry;
        Bitmap X1OrgImg = Properties.Resources._1x_30_org;
        Bitmap x2GryImg = Properties.Resources._2x_30_gry;
        Bitmap X2OrgImg = Properties.Resources._2x_30_org;
        Bitmap x4GryImg = Properties.Resources._4x_30_gry;
        Bitmap X4OrgImg = Properties.Resources._4x_30_org;
        Bitmap x8GryImg = Properties.Resources._8x_30_gry;
        Bitmap X8OrgImg = Properties.Resources._8x_30_org;
        Bitmap x16GryImg = Properties.Resources._16x_30_gry;
        Bitmap X16rgImg = Properties.Resources._16x_30_org;
        Bitmap x32ryImg = Properties.Resources._32x_30_gry;
        Bitmap X32rgImg = Properties.Resources._32x_30_org;

        Bitmap Spinner_org_148px = Properties.Resources.Spinner_Org_148px;

        private string Path_SaveSnapShot;

        string OpenPath_VideoFile;      
        string FolderOPenpath;
        string batfilepath;
        string cf_UserS = "User_SnapShot";
        string TagType;
        string TagReason;
        string TagImage;
        string Channel_Tag;
        string Path_SysVideo;
        string Path_Project;

        int i = 0;

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

        // Save ScrollIndex in Db
        private void Save_ScrollIndex()
        {
            try
            {
                int _scrollIndex = scrollIndex;
                db_connection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE project_detail SET Scroll_Index='" + scrollIndex + "' WHERE 	Project_ID = '" + cp_ID + "'";

                cmd.Connection = connect;
                MySqlDataReader RRChannel = cmd.ExecuteReader();
                connect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Get ScrollIndex from Db
        private void Get_ScrollIndex()
        {
            try
            {
                db_connection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "SELECT Scroll_Index FROM project_detail Where Project_ID = '" + cp_ID + "'";
                cmd.Connection = connect;
                MySqlDataReader cChannelReder = cmd.ExecuteReader();

                while (cChannelReder.Read())
                {
                    String SIndex = cChannelReder.GetString("Scroll_Index");
                    scrollIndex = Int32.Parse(SIndex);
                }
                cChannelReder.Close();

                connect.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                    CheckBoxArray[i].ForeColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
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
                        Path_SaveSnapShot = Path_video + "\\" + cp_ID + "-" + cp_proName + "\\" + cf_UserS; // save snapshot path
                    }
                    PChannelReder.Close();
                    connect.Close();

                    //channelSet[i].FileBasePath = TextBoxArry[i].Text;
                    CKbox.Add(ChannelInfos[i].ChannelNumber); // check true channelSet put into CKbox
                    //ChannelInfos[i]
                }
                else
                {
                    CheckBoxArray[i].ForeColor = System.Drawing.ColorTranslator.FromHtml("#898989");
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

        // Convert Snap To Video 
        private void Set_SnaptoVideo()
        {
            SetLoading(true);
            Thread.Sleep(4000);
            this.Invoke((MethodInvoker)delegate
            {
                ProgressDialog frm = new ProgressDialog();
                try
                {
                    frm.Show();
                    db_connection();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "SELECT Path_ChannelTag,Channel_Name,tag_reason,path_SysVideo FROM project_tag WHERE Project_ID = @Project_ID";

                    cmd.Parameters.AddWithValue("@Project_ID", cp_ID);
                    cmd.Connection = connect;
                    MySqlDataReader PChannelReder = cmd.ExecuteReader();

                    if (PChannelReder.HasRows != false)
                    {
                        while (PChannelReder.Read())
                        {
                            string path_TagSnapShort = (PChannelReder["Path_ChannelTag"].ToString());
                            string Channel = (PChannelReder["Channel_Name"].ToString());
                            string Tag_Reason = (PChannelReder["tag_reason"].ToString());
                            string VideoPath = (PChannelReder["path_SysVideo"].ToString());

                            string _Tag_Reason = Tag_Reason.Replace(" ", "_");  // Replace blank to "_"
                            string ABC = VideoPath + "\\" + "User_SnapShot" + "\\" + Tag_Reason + "\\" + Channel;  // SnapShot & Video Path

                            string tempFilename = Path.ChangeExtension(Path.GetTempFileName(), ".bat"); // create bat file
                            string ip_Path_Images = ABC + "\\" + "/%%d.jpg";
                            string p_Video = ABC;
                            string op_path_Video = VideoPath + "\\" + "Create_Video" + "\\" + Tag_Reason + "\\" + Channel + "\\" + "output.mp4";
                            string VideoSave = ABC + "\\" + "output.mp4";
                            String op_path_Video_2 = "\"" + VideoSave + "\"";

                            string TextFile = " MyImageList.txt";
                            string batfilename = "temp.bat";
                            batfilepath = ABC + "\\" + batfilename;
                            string jpg = "(*.jpg)";

                            string CreateTextFilepath = ABC + "\\" + "MyImageList.txt";   // Text file path
                            string CreateTextFilepath_2 = "\"" + CreateTextFilepath + "\""; // Text file path " "

                            FolderOPenpath = VideoPath + "\\" + "Create_Video" + "\\" + Tag_Reason + "\\" + Channel;
                            string FolderOPenpath2 = VideoPath + "\\" + "User_SnapShot" + "\\" + Tag_Reason + "\\" + Channel;
                            if (Directory.Exists(FolderOPenpath2))
                            {
                                //System.IO.Directory.CreateDirectory(FolderOPenpath); // create folder
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
                                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                Process process = Process.Start(startInfo);
                                process.WaitForExit();
                                File.Delete(batfilepath);
                                File.Delete(CreateTextFilepath);
                                //System.Diagnostics.Process.Start(FolderOPenpath); //open a video path
                            }

                            OpenPath_VideoFile = VideoPath;
                        }

                        System.Diagnostics.Process.Start(OpenPath_VideoFile); //open a video path
                        PB_ImgVideo.Visible = false;
                        this.Cursor = Cursors.Default;
                        SetLoading(false);
                        frm.Close();
                        MessageBox.Show("Auditing Completed");
                        this.Close();
                    }
                    else
                    {
                        SetLoading(false);
                        frm.Close();
                        MessageBox.Show("No snapshot Taken Yet ......." + " Take SnapShot First");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        // Import Excel All Db data
        private void Create_Excel()
        {
            SetLoading(true);
            Thread.Sleep(4000);
            this.Invoke((MethodInvoker)delegate
            {
                DataTable dataTable = new DataTable { TableName = "MyTableName" };
                try
                {
                    db_connection();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "SELECT PT.Project_id AS Project_ID, PD.Project_name AS Project_Name, PD.date_time AS Project_Create_DateTime, " +
                                        "PT.tag_type As Project_Type, PT.tag_reason AS SnapShot_Reason, " +
                                        "PT.tag_image AS SnapShot_Images, PT.Channel_Name AS Channel_Name, " +
                                        "PT.Path_VideoTag AS Create_Video_Path, PT.Path_SysVideo AS Project_Path, PT.date_time AS SnapShot_Create_DateTime" + " " +
                                        "FROM project_tag PT INNER JOIN project_detail PD ON PT.Project_id = PD.Project_ID " +
                                        "WHERE PT.Project_id = '" + cp_ID + "'";
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
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            Path_Project = (dataTable.Rows[0][8]).ToString();  //Path_SysVideo 
                            string FileCSVName = "AuditComplete - " + cp_ID + " - " + cp_proName + ".csv";
                            string Save_CSVFile = Path_Project + "\\" + FileCSVName;  // Save CSV file path   
                            File.WriteAllText(Save_CSVFile, sb.ToString());
                        }
                        SetLoading(false);
                        MessageBox.Show("Export Data Successfully");
                        System.Diagnostics.Process.Start(Path_Project); //open file save path
                    }
                    else
                    {
                        MessageBox.Show("No SnapShort Data Found .....");
                    }
                    connect.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });

            SetLoading(false);

        }

        // Loading Picture Box for spinner
        private void SetLoading(bool displayLoader)
        {
            if (displayLoader)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    PB_ImgVideo.BackColor = Color.Transparent;
                    PB_ImgVideo.Visible = true;
                    Size size = new Size(170, 140);
                    PB_ImgVideo.Size = size;
                    this.Cursor = Cursors.WaitCursor;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    PB_ImgVideo.Visible = false;
                    this.Cursor = Cursors.Default;
                });
            }
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

        public void playScroller()  // Play Scroller
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
            //string SaveVideoPath = Channel_Tag.Replace("User_SnapShot", "Create_Video");
            string SaveVideoPath = Channel_Tag;
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
        

        #region Mouse Enter/Leave/Move
        // Mouse Enter in PictureBox 
        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            string LableText = ((PictureBox)sender).Name;
            //string LableText = this.Name;
            switch (LableText)
            {
                case "PB_play":
                    if (PB_play.Image == PlayGryImg)
                    { PB_play.Image = PlayOrgImg; }
                    else if (PB_play.Image == PauseGryImg)
                    { PB_play.Image = PauseOrgImg; }
                    break;
                case "PB_Continue":
                    PB_Continue.Image = ContinueOrgImg;
                    break;
                case "PB_X":
                    if (PB_X.Image == x1GryImg)
                    { PB_X.Image = X1OrgImg; }
                    else if (PB_X.Image == x2GryImg)
                    { PB_X.Image = X2OrgImg; }
                    else if (PB_X.Image == x4GryImg)
                    { PB_X.Image = X4OrgImg; }
                    else if (PB_X.Image == x8GryImg)
                    { PB_X.Image = X8OrgImg; }
                    else if (PB_X.Image == x16GryImg)
                    { PB_X.Image = X16rgImg; }
                    else if (PB_X.Image == x32ryImg)
                    { PB_X.Image = X32rgImg; }
                    break;
                case "PB_CompAuditing":
                    PB_CompAuditing.Image = AuditOrgImg;
                    break;
                case "PB_File":
                    PB_File.Image = FileOrgImg;
                    break;
                case "PB_Stop":
                    PB_Stop.Image = StopOrgImg;
                    break;
                default:
                    break;
            }
        }

        // Mouse Leave in PictureBox 
        private void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            string LableText = ((PictureBox)sender).Name;
            //string LableText = this.Name;
            switch (LableText)
            {
                case "PB_play":
                    if (PB_play.Image == PlayOrgImg)
                    { PB_play.Image = PlayGryImg; }
                    else if (PB_play.Image == PauseOrgImg)
                    { PB_play.Image = PauseGryImg; }
                    break;
                case "PB_X":
                    if (PB_X.Image == X1OrgImg)
                    { PB_X.Image = x1GryImg; }
                    else if (PB_X.Image == X2OrgImg)
                    { PB_X.Image = x2GryImg; }
                    else if (PB_X.Image == X4OrgImg)
                    { PB_X.Image = x4GryImg; }
                    else if (PB_X.Image == X8OrgImg)
                    { PB_X.Image = x8GryImg; }
                    else if (PB_X.Image == X16rgImg)
                    { PB_X.Image = x16GryImg; }
                    else if (PB_X.Image == X32rgImg)
                    { PB_X.Image = x32ryImg; }
                    mv_Control.Text = "";
                    break;
                case "PB_Continue":
                    PB_Continue.Image = ContinueGryImg;
                    break;
                case "PB_CompAuditing":
                    PB_CompAuditing.Image = AuditGryImg;
                    break;
                case "PB_File":
                    PB_File.Image = FileGryImg;
                    break;
                case "PB_Stop":
                    PB_Stop.Image = StopGryImg;
                    break;

                default:
                    break;
            }

            mv_Control.Text = "";
        }

        // Case Mouse Hover -- move for PictureBox
        private void PB_play_MouseMove(object sender, MouseEventArgs e)
        {
            String Location_Mouse = " X-Coordinate : " + e.X + "  Y -Coordinate : " + e.Y;
            X_Mouse = e.X;
            Y_Mouse = e.Y;
            string LableText = ((PictureBox)sender).Name;

            switch (LableText)
            {
                case "PB_play":
                    var screenPosition1 = this.PointToScreen(PB_play.Location);
                    this.mv_Control.Location = new Point(X_Mouse + 10 + screenPosition1.X, Y_Mouse + 10 + screenPosition1.Y);
                    if (PB_play.Image == PlayGryImg || PB_play.Image == PlayOrgImg)
                    { mv_Control.Text = "Play"; }
                    else
                    { mv_Control.Text = "Pause"; }
                    break;
                case "PB_Stop":
                    var screenPosition2 = this.PointToScreen(PB_Stop.Location);
                    this.mv_Control.Location = new Point(X_Mouse + 10 + screenPosition2.X, Y_Mouse + 10 + screenPosition2.Y);
                    mv_Control.Text = "Stop";
                    break;
                case "PB_X":
                    var screenPosition3 = this.PointToScreen(PB_X.Location);
                    this.mv_Control.Location = new Point(X_Mouse + 10 + screenPosition3.X, Y_Mouse + 10 + screenPosition3.Y);
                    mv_Control.Text = "Fast Forward";
                    break;
                case "PB_Continue":
                    var screenPosition4 = this.PointToScreen(PB_Continue.Location);
                    this.mv_Control.Location = new Point(X_Mouse + 10 + screenPosition4.X, Y_Mouse + 10 + screenPosition4.Y);
                    mv_Control.Text = "Continue Audit";
                    break;
                case "PB_CompAuditing":
                    var screenPosition5 = this.PointToScreen(PB_CompAuditing.Location);
                    this.mv_Control.Location = new Point(X_Mouse + 10 + screenPosition5.X, Y_Mouse + 10 + screenPosition5.Y);
                    mv_Control.Text = "Complete Audit";
                    break;
                case "PB_File":
                    var screenPosition6 = this.PointToScreen(PB_File.Location);
                    this.mv_Control.Location = new Point(X_Mouse + 10 + screenPosition6.X, Y_Mouse + 10 + screenPosition6.Y);
                    mv_Control.Text = "Expose CSV";
                    break;
                default:
                    break;
            }
        }

        // Mouse Enter in Lable 
        private void Lable_MouseEnter(object sender, EventArgs e)
        {
            string LableText = ((Label)sender).Name;
            //string LableText = this.Name;
            switch (LableText)
            {
                case "lab_Close":
                    lab_Close.ForeColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                    lab_Close.BackColor = System.Drawing.Color.WhiteSmoke;
                    break;
                case "lab_Max":
                    lab_Max.ForeColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                    lab_Max.BackColor = System.Drawing.Color.WhiteSmoke;
                    break;
                case "lab_mini":
                    lab_mini.ForeColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                    lab_mini.BackColor = System.Drawing.Color.WhiteSmoke;
                    break;
                default:
                    break;
            }
        }

        // Mouse Leave in Lable 
        private void Lable_MouseLeave(object sender, EventArgs e)
        {
            string LableText = ((Label)sender).Name;
            //string LableText = this.Name;
            switch (LableText)
            {
                case "lab_Close":
                    lab_Close.ForeColor = System.Drawing.Color.WhiteSmoke;
                    lab_Close.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                    break;
                case "lab_Max":
                    lab_Max.ForeColor = System.Drawing.Color.WhiteSmoke;
                    lab_Max.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                    break;
                case "lab_mini":
                    lab_mini.ForeColor = System.Drawing.Color.WhiteSmoke;
                    lab_mini.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                    break;
                default:
                    break;
            }
            mv_Header.Text = "";
        }

        // Case Mouse Hover -- move for Lable
        private void lab_Close_MouseMove(object sender, MouseEventArgs e)
        {
            String Location_Mouse = " X-Coordinate : " + e.X + "  Y -Coordinate : " + e.Y;
            X_Mouse = e.X;
            Y_Mouse = e.Y;
            string LableText = ((Label)sender).Name;
            switch (LableText)
            {
                case "lab_mini":
                    var screenPosition1 = this.PointToScreen(lab_mini.Location);
                    this.mv_Header.Location = new Point(X_Mouse + 2 + screenPosition1.X, Y_Mouse + screenPosition1.Y);
                    mv_Header.Text = "Minimize";
                    break;
                case "lab_Max":
                    var screenPosition2 = this.PointToScreen(lab_Max.Location);
                    this.mv_Header.Location = new Point(X_Mouse + 2 + screenPosition2.X, Y_Mouse + screenPosition2.Y);
                    mv_Header.Text = "Maximize";
                    break;
                case "lab_Close":
                    var screenPosition3 = this.PointToScreen(lab_Close.Location);
                    this.mv_Header.Location = new Point(X_Mouse + 2 + screenPosition3.X, Y_Mouse + screenPosition3.Y);
                    mv_Header.Text = "Close";
                    break;
                default:
                    break;
            }
        }

        #endregion------------------------------------------------------------------------------------------------------------------


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
            //controls_grp.Enabled = true;
            btn_snapshot.Enabled = true;
            btn_Stop.Enabled = true;
        }
        #endregion-----------------------------------------------------------------------------------------------------------------------------------------------------------
        

        #region Load, Click

        // PAneel Click For snap short
        private void pbPannel_MouseClick(object sender, MouseEventArgs e)
        {
            string Name_Panel = ((Panel)sender).Name;
            switch (Name_Panel)
            {
                case "panel2":
                    if (checkBox1.Checked != true)
                    {
                        pictureBox1.Padding = new Padding(3);
                        pictureBox1.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                        checkBox1.Checked = true;
                    }
                    else
                    {
                        pictureBox1.Padding = new Padding(0);
                        pictureBox1.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                        checkBox1.Checked = false;
                    }
                    break;
                case "panel3":
                    if (checkBox2.Checked != true)
                    {
                        pictureBox2.Padding = new Padding(3);
                        pictureBox2.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                        checkBox2.Checked = true;
                    }
                    else
                    {
                        pictureBox2.Padding = new Padding(0);
                        pictureBox2.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                        checkBox2.Checked = false;
                    }
                    break;
                case "panel4":
                    if (checkBox3.Checked != true)
                    {
                        pictureBox3.Padding = new Padding(3);
                        pictureBox3.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                        checkBox3.Checked = true;
                    }
                    else
                    {
                        pictureBox3.Padding = new Padding(0);
                        pictureBox3.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                        checkBox3.Checked = false;
                    }
                    break;
                case "panel5":
                    if (checkBox4.Checked != true)
                    {
                        pictureBox4.Padding = new Padding(3);
                        pictureBox4.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                        checkBox4.Checked = true;
                    }
                    else
                    {
                        pictureBox4.Padding = new Padding(0);
                        pictureBox4.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                        checkBox4.Checked = false;
                    }
                    break;
                case "panel6":
                    if (checkBox5.Checked != true)
                    {
                        pictureBox5.Padding = new Padding(3);
                        pictureBox5.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                        checkBox5.Checked = true;
                    }
                    else
                    {
                        pictureBox5.Padding = new Padding(0);
                        pictureBox5.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                        checkBox5.Checked = false;
                    }
                    break;
                case "panel7":
                    if (checkBox6.Checked != true)
                    {
                        pictureBox6.Padding = new Padding(3);
                        pictureBox6.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                        checkBox6.Checked = true;
                    }
                    else
                    {
                        pictureBox6.Padding = new Padding(0);
                        pictureBox6.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                        checkBox6.Checked = false;
                    }
                    break;
                case "panel8":
                    if (checkBox7.Checked != true)
                    {
                        pictureBox7.Padding = new Padding(3);
                        pictureBox7.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                        checkBox7.Checked = true;
                    }
                    else
                    {
                        pictureBox7.Padding = new Padding(0);
                        pictureBox7.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                        checkBox7.Checked = false;
                    }
                    break;
                case "panel9":
                    if (checkBox8.Checked != true)
                    {
                        pictureBox8.Padding = new Padding(3);
                        pictureBox8.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                        checkBox8.Checked = true;
                    }
                    else
                    {
                        pictureBox8.Padding = new Padding(0);
                        pictureBox8.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                        checkBox8.Checked = false;
                    }
                    break;
                case "panel10":
                    if (checkBox9.Checked != true)
                    {
                        pictureBox9.Padding = new Padding(3);
                        pictureBox9.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                        checkBox9.Checked = true;
                    }
                    else
                    {
                        pictureBox9.Padding = new Padding(0);
                        pictureBox9.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                        checkBox9.Checked = false;
                    }
                    break;
                default:
                    break;
                    //Single 
                    //private void panel2_MouseClick(object sender, MouseEventArgs e)
                    //{           
                    //    if (checkBox1.Checked != true)
                    //    {
                    //        pictureBox1.Padding = new Padding(3);
                    //        pictureBox1.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                    //        checkBox1.Checked = true;
                    //    }
                    //    else
                    //    {
                    //        pictureBox1.Padding = new Padding(0);
                    //        pictureBox1.BackColor = System.Drawing.ColorTranslator.FromHtml("#f5f5f5");
                    //        //pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
                    //        checkBox1.Checked = false;
                    //    }
                    //}
            }
        }
       
        // Load the Channel Set
        private void channel1_chk_Click(object sender, EventArgs e)
        {
            loadChannels(); // Load Channels
        }

        // Load 
        private void FormLoad()
        {
            myIP = frm_SaveChannel.myIP;  // Computer IP
            CurrProjectName_label.Text = frm_SaveChannel.c_proName;  // Project Name
            lab_ProjectType.Text = frm_SaveChannel.c_proType;  // Project Type
            Curr_ProjectID.Text = cp_ID; // Project ID
            Get_dbSaveChannel(); // Get Chaecck DB Save Channel 
            loadChannels(); // Load Channels
            btn_Stop.Enabled = false;
            scrollPause.Enabled = false;
            btn_snapshot.Enabled = false;
        }
        
        // Form Load
        private void ImageScrollerForm_Load_1(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            SetLoading(false);
            FormLoad();
            PB_play.Image = PlayGryImg;
            //PB_Stop.Image = StopImg;
            PB_X.Image = x1GryImg;
            //Get_ScrollIndex();
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
        private void PB_play_Click(object sender, EventArgs e)
        {
            if (channel1_chk.Checked != false || channel2_chk.Checked != false || channel3_chk.Checked != false || channel4_chk.Checked != false ||
                channel5_chk.Checked != false || channel6_chk.Checked != false || channel7_chk.Checked != false || channel8_chk.Checked != false || channel9_chk.Checked != false)
            {
                if (PB_play.Image == PlayGryImg || PB_play.Image == PlayOrgImg)
                {
                    //btn_2x.Text = "1X";
                    //playTimer.Interval = 1000;
                    playScroller(); // Play Channels
                    btn_snapshot.Enabled = true;
                    btn_Stop.Enabled = true;
                    scrollPause.Enabled = true;
                    btn_SnapTOVideo.Enabled = true;
                    button1.Enabled = true;
                    PB_play.Image = PauseOrgImg;                    
                }
                else
                {                  
                    PB_play.Image = PlayOrgImg;
                    btn_SnapTOVideo.Enabled = true;
                    pauseScroller();
                }
            }
            else
            {
                MessageBox.Show("Check Atlist One Channel");
            }

                
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
        private void PB_Stop_Click(object sender, EventArgs e)
        {
            unloadChannels();
            FormLoad();
            PB_play.Image = PlayGryImg; // Set Play image
            PB_X.Image = x1GryImg;   // Set X image
            btn_2x.Text = "1X";
            playTimer.Interval = 1000;
            btn_SnapTOVideo.Enabled = true; // SnapShort
            button1.Enabled = true; // Btn Reset       
        }
      
        // Btn X1,X2.....X32
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
        private void PB_X_Click(object sender, EventArgs e)
        {
            if (btn_2x.Text == "1X")
            {
                playTimer.Interval = 500;
                scrollPlay.Enabled = true;
                btn_2x.Text = "2X";
                PB_X.Image = X2OrgImg;
            }
            else if (btn_2x.Text == "2X")
            {
                playTimer.Interval = 250;
                scrollPlay.Enabled = true;
                btn_2x.Text = "4X";
                PB_X.Image = X4OrgImg;
            }
            else if (btn_2x.Text == "4X")
            {
                playTimer.Interval = 125;
                scrollPlay.Enabled = true;
                btn_2x.Text = "8X";
                PB_X.Image = X8OrgImg;
            }
            else if (btn_2x.Text == "8X")
            {
                playTimer.Interval = 62;
                scrollPlay.Enabled = true;
                btn_2x.Text = "16X";
                PB_X.Image = X16rgImg;
            }
            else if (btn_2x.Text == "16X")
            {
                playTimer.Interval = 31;
                scrollPlay.Enabled = true;
                btn_2x.Text = "32X";
                PB_X.Image = X32rgImg;
            }
            else if (btn_2x.Text == "32X")
            {
                playTimer.Interval = 1000;
                scrollPlay.Enabled = true;
                btn_2x.Text = "1X";
                PB_X.Image = X1OrgImg;
            }
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

        // Ctr + S for snapshot
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == 256)
            {
                if (keyData == (Keys.Control | Keys.S))
                {
                    pauseScroller();
                    if (checkBox1.Checked == false && checkBox2.Checked == false && checkBox3.Checked == false && checkBox4.Checked == false && checkBox5.Checked == false &&
                 checkBox6.Checked == false && checkBox7.Checked == false && checkBox8.Checked == false && checkBox9.Checked == false)
                    {
                        MessageBox.Show("Select Channel For SnapShot");
                    }
                    else
                    {
                        beforeSave();                       
                        // saveScreenshot();
                        frm_Popoup fP = new frm_Popoup(this);
                        fP.Show();
                        //saveScreenshot();
                        //afterSave();
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        //Btn continue
        private void PB_Continue_Click(object sender, EventArgs e)
        {
            if (channel1_chk.Checked != false || channel2_chk.Checked != false || channel3_chk.Checked != false || channel4_chk.Checked != false ||
               channel5_chk.Checked != false || channel6_chk.Checked != false || channel7_chk.Checked != false || channel8_chk.Checked != false || channel9_chk.Checked != false)
            {
                Get_ScrollIndex();
                if (scrollIndex != 0)
                {
                    PB_play.Image = PauseGryImg;
                    btn_2x.Text = "1X";
                    playTimer.Interval = 1000;
                    PB_X.Image = x1GryImg;
                    btn_SnapTOVideo.Enabled = true;
                    button1.Enabled = true;
                    playScroller(); // Play Channels
                }
                else
                {
                    MessageBox.Show("First Time Auditing For This Project");
                }
            }
            else
            {
                MessageBox.Show("Check Atlist One Channel");
            }
        }
        private void Btn_Reset_Click(object sender, EventArgs e)
        {
            if (channel1_chk.Checked != false || channel2_chk.Checked != false || channel3_chk.Checked != false || channel4_chk.Checked != false ||
               channel5_chk.Checked != false || channel6_chk.Checked != false || channel7_chk.Checked != false || channel8_chk.Checked != false || channel9_chk.Checked != false)
            {
                Get_ScrollIndex();
                if (scrollIndex != 0)
                {
                    PB_play.Image = PauseImg;
                    btn_2x.Text = "1X";
                    playTimer.Interval = 1000;
                    PB_X.Image = x1GryImg;
                    btn_SnapTOVideo.Enabled = true;
                    button1.Enabled = true;
                    playScroller(); // Play Channels
                }
                else
                {
                    MessageBox.Show("First Time Auditing For This Project");
                }
            }
            else
            {
                MessageBox.Show("Check Atlist One Channel");
            }
        }

        // btn Export CSV
        private void PB_File_Click(object sender, EventArgs e)
        {
            try
            {
                Thread threadInput = new Thread(Create_Excel); // Create csv file from db
                threadInput.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                Thread threadInput = new Thread(Create_Excel); // Create csv file from db
                threadInput.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        // btn Convert Snap To Video 
        private void PB_CompAuditing_Click(object sender, EventArgs e)
        {
            Save_ScrollIndex(); // Save scroll index in Db            
            pauseScroller();
            try
            {
                Thread threadInput = new Thread(Set_SnaptoVideo); // ffmpeg Snap to Video    
                threadInput.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void btn_SnapTOVideo_Click(object sender, EventArgs e)
        {
            Save_ScrollIndex(); // Save scroll index in Db            

            try
            {
                Thread threadInput = new Thread(Set_SnaptoVideo); // ffmpeg Snap to Video    
                threadInput.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        // btn Form Close
        private void lab_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //btn Form Max
        private void lab_Max_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
            }
            else if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
            }
        }

        //btn Form Mini
        private void lab_mini_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }


        //Not Used
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
           
                     
    }
}
