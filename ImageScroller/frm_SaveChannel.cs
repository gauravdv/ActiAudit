using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Net;

namespace ImageScroller
{
    public partial class frm_SaveChannel : Form
    {
        int X_Mouse;
        int Y_Mouse;

        Bitmap LoadBigImg = Properties.Resources.btn_LoadBig; // Set PlayBlue Image

        Bitmap BrowseGryImg = Properties.Resources.Browse_30_gry; // Set PlayBlue Image
        Bitmap BrowseOrgImg = Properties.Resources.Browse_30_Org; // Set PlayBlue Image
        Bitmap BrowsewhiteImg = Properties.Resources.Browse_30_white_; // Set PlayBlue Image

        Bitmap ClearOrgImg = Properties.Resources.Clear_30_Org; // Set PlayBlue Image
        Bitmap ClearWhiteImg = Properties.Resources.Clear_30_White; // Set PlayBlue Image

        Bitmap MergeOrg30 = Properties.Resources.merge_orange__30_; // Set PlayBlue Image
        Bitmap MeargWhite30 = Properties.Resources.merge_white__30; // Set PlayBlue Image

        Bitmap SpinnerOrgGif = Properties.Resources.Spinner_30_Org; // Set PlayBlue Image
        Bitmap VerifiredOrgGif = Properties.Resources.Verifired_30_Org; // Set PlayBlue Image

        string db_Server;
        string db_Name;
        string db_UserID;
        string db_Password;
        public static string myIP;

        public static string cp_ID; // Curr Project ID
        public static string cp_Name; // curr Project Name
        string cp_imgFormat; // Select Img Format
        string cp_fileType; // Select File Type
        string cp_prefix;
        string cp_suffix;
        string vfile_path;

        private string conn;
        private MySqlConnection connect;
        public static string c_proName;
        public static string c_proType;
        String p_Name;

        int i = 0;

        private String fileFormatType = "Number";

        private String defaultDateFormat = "yyyy-MM-dd_HH-mm-ss";

        private DateTime referenceDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, (DateTime.Now.Day));//-1
        private DateTime feedDate;

        private String fileNamePrefix = string.Empty;
        private String fileNameSuffix = string.Empty;

        private String fileNameFormat = "{0}{1}{2}.{3}";

        Dictionary<int, ChannelInfo> channelSet = new Dictionary<int, ChannelInfo>();
        FolderBrowserDialog folderDlg = new FolderBrowserDialog();

        public frm_SaveChannel()
        {
            InitializeComponent();
            initChannelDisplayForm();
            Get_ipAddress();

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
                MessageBox.Show("Can not open mysql connection !" + ex.Message);
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

        // Get ProjectID Name to Db
        private void Get_dbProjectID(out string db_ProjectID, ref string db_ProjectName)
        {
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT Project_ID,Project_Type FROM project_detail WHERE Project_name = '" + lab_SelectProjectName.Text + "' AND computer_IP = '" + myIP + "' ";
            cmd.Connection = connect;
            MySqlDataReader cChannelReder = cmd.ExecuteReader();
            while (cChannelReder.Read())
            {
                cp_ID = cChannelReder.GetString("Project_ID");
                lab_ProjectID.Text = cp_ID;
                lab_ProjectType.Text = cChannelReder.GetString("Project_Type");
            }
            cChannelReder.Close();
            connect.Close();
            db_ProjectID = cp_ID;
        }

        // Get ProjectID To Display in lab_ProjectID
        private void Get_dbProjectID_Display() // for the add project id
        {
            string Pro_ID = cp_Name;
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT Project_ID, Project_Type FROM project_detail WHERE Project_name = '" + txt_Newproject.Text + "' AND computer_IP = '" + myIP + "' ";
            cmd.Connection = connect;
            MySqlDataReader cChannelReder = cmd.ExecuteReader();
            while (cChannelReder.Read())
            {
                cp_ID = cChannelReder.GetString("Project_ID");
                lab_ProjectID.Text = cp_ID;
                lab_ProjectType.Text = cChannelReder.GetString("Project_Type");

            }
            cChannelReder.Close();
            connect.Close();
        }

        // db Channel Name to Db When Project Select
        private void Get_dbChannel()
        {
            string[] ChannelNameArray = new string[9];
            TextBox[] TextBoxArry = new TextBox[9];
            ChannelNameArray[0] = "Channel_1";
            ChannelNameArray[1] = "Channel_2";
            ChannelNameArray[2] = "Channel_3";
            ChannelNameArray[3] = "Channel_4";
            ChannelNameArray[4] = "Channel_5";
            ChannelNameArray[5] = "Channel_6";
            ChannelNameArray[6] = "Channel_7";
            ChannelNameArray[7] = "Channel_8";
            ChannelNameArray[8] = "Channel_9";
            // Text bo0x array           
            TextBoxArry[0] = channel1_txt;
            TextBoxArry[1] = channel2_txt;
            TextBoxArry[2] = channel3_txt;
            TextBoxArry[3] = channel4_txt;
            TextBoxArry[4] = channel5_txt;
            TextBoxArry[5] = channel6_txt;
            TextBoxArry[6] = channel7_txt;
            TextBoxArry[7] = channel8_txt;
            TextBoxArry[8] = channel9_txt;

            string db_ProjectName = cp_Name;
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
                while (PChannelReder.Read())
                {
                    string db_VideoPath = PChannelReder.GetString("path_Video");
                    string db_FileVideoName = PChannelReder.GetString("file_VideoName");
                    string db_PathWithFileName = db_VideoPath + "\\" + db_FileVideoName;
                    TextBoxArry[i].Text = db_PathWithFileName;
                }
                PChannelReder.Close();
                connect.Close();
            }
        }

        // Get Channel Name to Db When Save Or Update
        private void Get_ChannelNo(out string db_ChannelName, out string db_ProjectID, ref string _ChannelName)
        {
            db_ChannelName = "";
            string db_ProjectName = cp_Name;

            Get_dbProjectID(out db_ProjectID, ref db_ProjectName);

            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT path_Snapshot,path_Video,img_Format,file_Type,file_VideoName,Channel_name FROM channel_detail WHERE Channel_name = @Channel_name AND Project_ID = @Project_ID";
            cmd.Parameters.AddWithValue("@Channel_name", _ChannelName);
            cmd.Parameters.AddWithValue("@Project_ID", db_ProjectID);
            cmd.Connection = connect;
            MySqlDataReader PChannelReder = cmd.ExecuteReader();
            while (PChannelReder.Read())
            {
                string db_VideoPath = PChannelReder.GetString("path_Video");
                string db_FileVideoName = PChannelReder.GetString("file_VideoName");
                string db_PathWithFileName = db_VideoPath + "\\" + db_FileVideoName;
                //TextBoxArry[i].Text = db_PathWithFileName;
                db_ChannelName = PChannelReder.GetString("Channel_name");
            }

        }

        // Delete Project 
        private void Delete_Project(string SelectProject_Name)
        {
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "DELETE FROM project_detail WHERE Project_name = '" + SelectProject_Name + "' ";
            cmd.Connection = connect;
            MySqlDataReader cChannelReder = cmd.ExecuteReader();
            cChannelReder.Close();
            connect.Close();

        }
        #endregion-----------------------------------------------------------------------------------------------------------------------------------------------------------

        // Get the IP from the host name
        private void Get_ipAddress()
        {
            string myHost = System.Net.Dns.GetHostName();// Get the hostname           
            myIP = System.Net.Dns.GetHostByName(myHost).AddressList[0].ToString();  // Get the IP from the host name
            if (myIP == "")
            {
                MessageBox.Show("IP Address Not Found");
            }
            //MessageBox.Show(myIP, myHost);
        }

        // Create a New Project
        private void Insert_NewProject()
        {
            // Check Project Name Exist or not
            int Scroll_Index = 0;
            cp_Name = txt_Newproject.Text;
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT Project_name FROM project_detail WHERE Project_name LIKE '" + cp_Name + "' AND computer_IP = '" + myIP + "'  ORDER BY Project_name ASC";
            cmd.Connection = connect;
            // MySqlDataReader ChannelReder = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            int icount = dt.Rows.Count;
            connect.Close();
            if (dt == null || icount == 0)
            {
                if (cp_Name != "")// project name not blank
                {
                    // Insert Into db Project_detail
                    db_connection();
                    //MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = "INSERT INTO project_detail(Project_name,computer_IP,Project_Type, Scroll_Index) values(@Project_name,@computer_IP,@Project_Type, @Scroll_Index)";
                    cmd.Parameters.AddWithValue("@Project_name", cp_Name);
                    cmd.Parameters.AddWithValue("@computer_IP", myIP);
                    cmd.Parameters.AddWithValue("@Project_Type", cmb_TagMaster.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Scroll_Index", Scroll_Index);
                    cmd.Connection = connect;
                    MySqlDataReader ChannelReder = cmd.ExecuteReader();
                    connect.Close();
                    MessageBox.Show(" New " + cp_Name + " Project Created");

                    lab_SelectProjectName.Text = cp_Name.ToString();
                    lab_ProjectName.Text = cp_Name.ToString();
                }
            }
            else
            {
                MessageBox.Show("Same Project Name Allready Given");
                txt_Newproject.Text = "";
            }
        }

        // Text search
        private void Search_TxtBox()
        {
            if (txt_Search.Text != "")
            {
                foreach (ListViewItem item in listView_ProjectName.Items)
                {
                    if (item.Text.ToLower().Contains(txt_Search.Text.ToLower()))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        listView_ProjectName.Items.Remove(item);
                    }

                }
                if (listView_ProjectName.SelectedItems.Count == 1)
                {
                    listView_ProjectName.Focus();
                    txt_Search.Focus();
                }
            }
            else
            {
                Add_ListProjectName();
            }
        }

        // Gat Tag Type
        private void Get_CmbTagType()
        {
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT TagType_name FROM tag_type ORDER BY TagType_name ASC";
            cmd.Connection = connect;
            MySqlDataReader ChannelReder = cmd.ExecuteReader();
            while (ChannelReder.Read())
            {
                cmb_TagMaster.Items.Add(ChannelReder["TagType_name"].ToString());
            }
            ChannelReder.Close();
            connect.Close();
        }

        private void Get_CheckPictureBoxLoad(int i)
        {
            PictureBox[] pictureBoxArry = new PictureBox[9];
            String[] PBCheckName = new string[9];
            // Check Picture bOx Array
            pictureBoxArry[0] = PB_Check1;
            pictureBoxArry[1] = PB_Check2;
            pictureBoxArry[2] = PB_Check3;
            pictureBoxArry[3] = PB_Check4;
            pictureBoxArry[4] = PB_Check5;
            pictureBoxArry[5] = PB_Check6;
            pictureBoxArry[6] = PB_Check7;
            pictureBoxArry[7] = PB_Check8;
            pictureBoxArry[8] = PB_Check9;

            pictureBoxArry[i].BackColor = Color.Transparent;
            pictureBoxArry[i].Visible = true;
            pictureBoxArry[i].Image = SpinnerOrgGif;
            this.Cursor = Cursors.WaitCursor;
        }
        private void Get_CheckPictureBoxCheck(int i)
        {
            PictureBox[] pictureBoxArry = new PictureBox[9];
            String[] PBCheckName = new string[9];
            // Check Picture bOx Array
            pictureBoxArry[0] = PB_Check1;
            pictureBoxArry[1] = PB_Check2;
            pictureBoxArry[2] = PB_Check3;
            pictureBoxArry[3] = PB_Check4;
            pictureBoxArry[4] = PB_Check5;
            pictureBoxArry[5] = PB_Check6;
            pictureBoxArry[6] = PB_Check7;
            pictureBoxArry[7] = PB_Check8;
            pictureBoxArry[8] = PB_Check9;

            pictureBoxArry[i].BackColor = Color.Transparent;
            pictureBoxArry[i].Visible = true;
            pictureBoxArry[i].Image = VerifiredOrgGif;
            this.Cursor = Cursors.WaitCursor;
        }

        private void Get_CheckPictureBox(int i)
        {
            PictureBox[] pictureBoxArry = new PictureBox[9];
            // Picture bOx Array
            pictureBoxArry[0] = PB_Check1;
            pictureBoxArry[1] = PB_Check2;
            pictureBoxArry[2] = PB_Check3;
            pictureBoxArry[3] = PB_Check4;
            pictureBoxArry[4] = PB_Check5;
            pictureBoxArry[5] = PB_Check6;
            pictureBoxArry[6] = PB_Check7;
            pictureBoxArry[7] = PB_Check8;
            pictureBoxArry[8] = PB_Check9;

            //string ExeLoaction = System.Reflection.Assembly.GetEntryAssembly().Location; ;
            //string txtFilePath = Path.GetDirectoryName(ExeLoaction); // Folder path
            //string txtpath = txtFilePath + "\\" + "Check_mark.png";
            //pictureBoxArry[i].SizeMode = PictureBoxSizeMode.StretchImage;
            //pictureBoxArry[i].ImageLocation = @"C:\\Users\\konsultera\\Desktop\\SGDT\\Check.jpg"; 
            pictureBoxArry[i].Show();
            pictureBoxArry[i].SizeMode = PictureBoxSizeMode.CenterImage;
            //pictureBoxArry[i].ImageLocation = @txtpath;

            //pictureBoxArry[i].BackColor = Color.Transparent;
            // pictureBoxArry[i].Visible = true;
        }

        // Save Channel, VideoPath, SnapshortPath
        private void Save_Channel_Path()
        {

            CheckBox[] CheckBoxArray = new CheckBox[9];
            TextBox[] TextBoxArry = new TextBox[9];
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
            // Channel Name array
            ChannelNameArray[0] = "Channel_1";
            ChannelNameArray[1] = "Channel_2";
            ChannelNameArray[2] = "Channel_3";
            ChannelNameArray[3] = "Channel_4";
            ChannelNameArray[4] = "Channel_5";
            ChannelNameArray[5] = "Channel_6";
            ChannelNameArray[6] = "Channel_7";
            ChannelNameArray[7] = "Channel_8";
            ChannelNameArray[8] = "Channel_9";
            // Text bo0x array           
            TextBoxArry[0] = channel1_txt;
            TextBoxArry[1] = channel2_txt;
            TextBoxArry[2] = channel3_txt;
            TextBoxArry[3] = channel4_txt;
            TextBoxArry[4] = channel5_txt;
            TextBoxArry[5] = channel6_txt;
            TextBoxArry[6] = channel7_txt;
            TextBoxArry[7] = channel8_txt;
            TextBoxArry[8] = channel9_txt;

            for (int i = 0; i < 9; i++)
            {
                // Insert Channel To db
                if (CheckBoxArray[i].Checked == true && TextBoxArry[i].Text != "")
                {
                    ProgressDialog frm = new ProgressDialog();
                    Thread.Sleep(2000);
                    frm.Show();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Get_CheckPictureBoxLoad(i);
                        //Get_CheckPictureBox(i);
                    });

                    string _ChannelName = ChannelNameArray[i];
                    string db_ChannelName;
                    string db_ProjectID;
                    string op_path;
                    string Channel_name = ChannelNameArray[i];
                    //string path_Video = TextBoxArry[i].Text;
                    //string path_SnapShot = TextBoxArry[i].Text + "\\" + "SnapShot";

                    string vfolder_path = Path.GetDirectoryName(TextBoxArry[i].Text); // Folder path
                    string FileName = Path.GetFileName(TextBoxArry[i].Text); // get File name
                    string VideoFileName = System.IO.Path.GetFileNameWithoutExtension(FileName);

                    string cSnapshotFolder_path = vfolder_path + "\\" + cp_ID + "\\" + "System_SnapShot" + "\\" + VideoFileName; // create folder path    
                    string cSnapshotFolder_path_2 = vfolder_path + "\\" + cp_ID + "-" + cp_Name + "\\" + "System_SnapShot" + "\\" + VideoFileName; // create folder path   
                    //string cSnapshotFolder_path = vfolder_path + "\\" + cp_ID + "_" + _ProjectName + "\\" + "System_SnapShot" + "\\" + VideoFileName; // create folder path                
                    System.IO.Directory.CreateDirectory(cSnapshotFolder_path_2); // create folder

                    string tempFilename = Path.ChangeExtension(Path.GetTempFileName(), ".bat"); // create bat file
                    string vp_Path = TextBoxArry[i].Text;
                    string Vp_path_2 = "\"" + vp_Path + "\"";

                    Get_ChannelNo(out db_ChannelName, out db_ProjectID, ref _ChannelName);
                    if (_ChannelName == db_ChannelName)
                    {
                        db_connection();
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = "UPDATE Channel_detail SET Channel_name=@Channel_name, path_Video=@path_Video,path_SnapShot=@path_SnapShot,Project_ID=@Project_ID," +
                            "img_Format=@img_Format,file_Type=@file_Type,file_VideoName=@file_VideoName WHERE Channel_name = @Channel_name AND Project_ID = @Project_ID";

                        cmd.Parameters.AddWithValue("@Channel_name", Channel_name);
                        cmd.Parameters.AddWithValue("@path_Video", vfolder_path);
                        cmd.Parameters.AddWithValue("@path_SnapShot", cSnapshotFolder_path_2);
                        cmd.Parameters.AddWithValue("@Project_ID", cp_ID);
                        cmd.Parameters.AddWithValue("@img_Format", cp_imgFormat);
                        cmd.Parameters.AddWithValue("@file_Type", cp_fileType);
                        cmd.Parameters.AddWithValue("@file_VideoName", FileName);

                        cmd.Connection = connect;
                        MySqlDataReader Channel = cmd.ExecuteReader();
                        connect.Close();
                    }

                    else
                    {
                        db_connection();
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = "INSERT INTO Channel_detail(Channel_name, path_Video,path_SnapShot,Project_ID,img_Format,file_Type,file_VideoName) " +
                            "values(@Channel_name,@path_Video,@path_SnapShot,@Project_ID,@img_Format,@file_Type,@file_VideoName)";

                        cmd.Parameters.AddWithValue("@Channel_name", Channel_name);
                        cmd.Parameters.AddWithValue("@path_Video", vfolder_path);
                        cmd.Parameters.AddWithValue("@path_SnapShot", cSnapshotFolder_path_2);
                        cmd.Parameters.AddWithValue("@Project_ID", cp_ID);
                        cmd.Parameters.AddWithValue("@img_Format", cp_imgFormat);
                        cmd.Parameters.AddWithValue("@file_Type", cp_fileType);
                        cmd.Parameters.AddWithValue("@file_VideoName", FileName);
                        cmd.Connection = connect;
                        MySqlDataReader Channel = cmd.ExecuteReader();
                        connect.Close();
                    }

                    string batfilepath;
                    string batfilename = "temp.bat";
                    batfilepath = cSnapshotFolder_path_2 + "\\" + batfilename;

                    if (cp_fileType == "jpg")

                    { op_path = cSnapshotFolder_path_2 + "/%%d.jpg"; }
                    else
                    { op_path = cSnapshotFolder_path_2 + "/%%d.png"; }
                    string op_path_2 = "\"" + op_path + "\"";
                    using (StreamWriter writer = new StreamWriter(batfilepath)) // for the cmd 
                    {
                        //writer.WriteLine("hide");
                        //writer.WriteLine("cd..");
                        //writer.WriteLine("cd..");
                        //writer.WriteLine("cd ffmpeg\bin");
                        //writer.WriteLine("ffmpeg.exe");
                        writer.WriteLine("ffmpeg -y -i " + Vp_path_2 + " -vf fps=1 " + op_path_2 + "");
                        // writer.WriteLine("PAUSE");                      
                    }
                    //Process process = Process.Start(batfilepath);
                    //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //process.WaitForExit();
                    ////MessageBox.Show("Convert" + " " + Channel_name + " " + "Video into Images");
                    //File.Delete(batfilepath); // delete bat file  

                    var startInfo = new ProcessStartInfo();
                    startInfo.WorkingDirectory = cSnapshotFolder_path_2;
                    startInfo.FileName = batfilename;
                    startInfo.CreateNoWindow = true;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    Process process = Process.Start(startInfo);
                    //PB_VideoMerge.Image = LoadSImg;
                    //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.WaitForExit();
                    File.Delete(batfilepath);
                    //PB_VideoMerge.Image = CheckImg;
                    listView_VideoFile.Items.Clear();
                    frm.Close();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Get_CheckPictureBoxCheck(i);
                    });
                }
                this.Cursor = Cursors.Default;

            }
        }

        // Get Date & Number
        private void getFileFormatSample()
        {
            String sampleFormat = "Invalid Format";
            String fileType = string.Empty;
            String _prefix = string.Empty;
            String _suffix = string.Empty;

            if (fileType_combo.SelectedItem != null && !string.IsNullOrEmpty(fileType_combo.SelectedItem.ToString()))
            {
                fileType = fileType_combo.SelectedItem.ToString();
                cp_fileType = fileType;

                _prefix = prefix_txt.Text;
                _suffix = suffix_txt.Text;

                if (fileNameFormat_combo.SelectedItem.ToString().Equals("ToDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime sampleDateTime = feedDate_date.Value;
                    String dateFormat = dateFormat_txt.Text;

                    sampleFormat = string.Format(fileNameFormat, _prefix, sampleDateTime.ToString(dateFormat), _suffix, fileType);

                    dateFormat_grp.Enabled = true;
                    cp_imgFormat = "ToDate";
                }
                else if (fileNameFormat_combo.SelectedItem.ToString().Equals("Number", StringComparison.InvariantCultureIgnoreCase))
                {
                    sampleFormat = string.Format(fileNameFormat, _prefix, "1", _suffix, fileType);
                    dateFormat_grp.Enabled = false;
                    cp_imgFormat = "Number";
                }
            }
            fileNameSample_txt.Text = sampleFormat;
        }

        // ffmpeg Convert Video To Video
        private void Convert_VideoToVideo_FFmpeg()
        {
            ProgressDialog frm = new ProgressDialog();
            this.Invoke((MethodInvoker)delegate
            {
                //SetLoading(true);
                //Thread.Sleep(2000);
                frm.Show();
                int lvCount = listView_VideoFile.Items.Count;
                if (lvCount == 0 || lvCount == 1)
                {
                    MessageBox.Show("Please Select Video");
                }
                else
                {
                    string path_lvIteam = listView_VideoFile.Items[i].Text; // Get ListView Iteams
                    string batfilename = "temp.bat";
                    string vfolder_path = Path.GetDirectoryName(path_lvIteam); // Folder path
                    string FileName = Path.GetFileName(path_lvIteam); // get File name
                    string VideoFileName = System.IO.Path.GetFileNameWithoutExtension(FileName);
                    string path_SaveVideo = vfolder_path + "\\" + "Output_Video";
                    System.IO.Directory.CreateDirectory(path_SaveVideo); // create folder               

                    string batfilepath = vfolder_path + "\\" + batfilename;
                    string textfilepath = vfolder_path + "\\" + "mylist.txt";
                    string textfilepath_2 = "\"" + vfolder_path + "\\" + "mylist.txt" + "\"";
                    // Create Text File
                    string[] selected_item = new string[listView_VideoFile.Items.Count];
                    for (int i = 0; i < listView_VideoFile.Items.Count; i++)
                    {
                        string VideoName = listView_VideoFile.Items[i].Text;
                        string FileVideoName = System.IO.Path.GetFileName(VideoName);
                        //string FileVideoName2 = "file " + FileVideoName;
                        selected_item[i] = "file " + FileVideoName;
                        //string FileExtension = System.IO.Path.GetExtension(VideoName);
                    }
                    System.IO.File.WriteAllLines(textfilepath, selected_item);

                    string op_FileExtension = System.IO.Path.GetExtension(selected_item[0]);
                    string op_path_Video = path_SaveVideo + "\\" + "output" + op_FileExtension;
                    string op_path_Video_2 = "\"" + path_SaveVideo + "\\" + "output" + op_FileExtension + "\"";

                    // Create a bat File
                    using (StreamWriter writer = new StreamWriter(batfilepath))
                    {
                        writer.WriteLine("ffmpeg -y -f concat -i " + textfilepath_2 + " -c:v copy -an " + op_path_Video_2);
                        //writer.WriteLine("PAUSE");
                    }
                    try
                    {

                        //// FFmpeg Direct Run   ------------------ 
                        var startInfo = new ProcessStartInfo();
                        startInfo.WorkingDirectory = vfolder_path;
                        startInfo.FileName = batfilename;
                        startInfo.CreateNoWindow = true;
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        Process process = Process.Start(startInfo);
                        PB_VideoMerge.Image = SpinnerOrgGif;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        //for (int i = 0; ; i++)
                        //{
                        //    if (!process.HasExited)
                        //    {
                        //        //SetLoading(true);
                        //        //Thread.Sleep(2000);
                        //        process.Refresh();
                        //        frm.Show();
                        //        // Print working set to console.
                        //        //Console.WriteLine("Physical Memory Usage: " + process.WorkingSet.ToString());
                        //        //// Wait 2 seconds.                          
                        //    }
                        //    else
                        //    {
                        //        frm.Close();
                        //        //SetLoading(false);
                        //        break;
                        //    }
                        //}
                        process.WaitForExit();
                        File.Delete(batfilepath);
                        File.Delete(textfilepath);
                        //PB_VideoMerge.Image = CheckImg;
                        listView_VideoFile.Items.Clear();
                        System.Diagnostics.Process.Start(vfolder_path + "\\Output_Video\\"); //open a video path
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        //if (process != null) process.Dispose();
                    }
                }
                frm.Close();
                //SetLoading(false);
            });


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
                cmd.CommandText = "SELECT CD.Project_ID, PD.Project_name AS Project_Name, PD.Project_Type, PD.date_time AS Project_Create_Date, " +
                                  "CD.file_VideoName AS Video_Name, CD.path_Video AS Select_Video_Path, CD.path_Snapshot As SnapShot_Path, " +
                                  "CD.date_time AS Video_Upload_Date FROM channel_detail CD INNER JOIN project_detail PD ON PD.Project_id = CD.Project_ID " +
                                  "WHERE CD.Project_ID = '" + cp_ID + "'";

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

                Path_CSVFile = (dataTable.Rows[0][5]).ToString();  //Path_VideoTag
                string FileCSVName = "AuditChannel - " + cp_ID + " - " + cp_Name + ".csv";
                Save_CSVFile = Path_CSVFile + "\\" + cp_ID + "-" + cp_Name + "\\" + FileCSVName;
                File.WriteAllText(Save_CSVFile, sb.ToString());

                MessageBox.Show("Export Data Successfully");
                connect.Close();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        // Load the Channel Set
        private void Channel_Check(object sender, EventArgs e) // Color Change
        {
            CheckBox[] CheckBoxArray = new CheckBox[9];
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

            for (int i = 0; i < 9; i++)
            {
                if (CheckBoxArray[i].Checked == true)
                {
                    CheckBoxArray[i].ForeColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                }
                else
                {
                    CheckBoxArray[i].ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        #region List View
        // Project Name In list View
        private void Add_ListProjectName()
        {
            listView_ProjectName.GridLines = true;
            listView_ProjectName.View = View.Details;
            listView_ProjectName.Columns.Add("Project Name", 500);

            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT Project_name FROM project_detail WHERE computer_IP = '" + myIP + "' ORDER BY Project_name ASC";
            cmd.Connection = connect;
            MySqlDataReader ChannelReder = cmd.ExecuteReader();

            listView_ProjectName.Items.Clear();
            while (ChannelReder.Read())
            {
                ListViewItem lv = new ListViewItem(ChannelReder["Project_name"].ToString());
                listView_ProjectName.Items.Add(lv);
            }
            ChannelReder.Close();
            connect.Close();
        }

        private void listView_ProjectName_SelectedIndexChanged(object sender, EventArgs e)
        {
            Unload_Channeltext();

            if (listView_ProjectName.SelectedIndices.Count <= 0)
            {
                return;
            }
            int intselectedindex = listView_ProjectName.SelectedIndices[0];
            if (intselectedindex >= 0)
            {
                cp_Name = listView_ProjectName.Items[intselectedindex].Text;
                //string text = listView_ProjectName.SelectedItems[0].Text;
                lab_SelectProjectName.Text = cp_Name; // get curr project name
                lab_ProjectName.Text = cp_Name;
                //listView_ProjectName.ForeColor =  System.Drawing.ColorTranslator.FromHtml("#f26222");
            }
            Get_dbProjectID_Display();
            Get_dbChannel();
        }

        // Add List View For Video Name
        private void Add_ListVideoName()
        {
            listView_VideoFile.GridLines = true;
            listView_VideoFile.View = View.Details;
            listView_VideoFile.Columns.Add("Select Video Name", 500);
        }

        // Delete ListView Iteams
        private void listView_ProjectName_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                foreach (ListViewItem eachItem in listView_ProjectName.SelectedItems)
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure to Delete Project " + eachItem.Text + " Yes/No", "Remove", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        listView_ProjectName.Items.Remove(eachItem);
                        string SelectProject_Name = eachItem.Text;
                        Delete_Project(SelectProject_Name);
                    }
                }
            }
        }

        private void listView_VideoFile_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                foreach (ListViewItem eachItem in listView_VideoFile.SelectedItems)
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure to remove Yes/No", "Remove", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        listView_VideoFile.Items.Remove(eachItem);
                    }
                }
            }
        }


        #endregion-----------------------------------------------------------------------------------------------------------------------------------------------------------


        #region Load , Unload
        // init
        private void initChannelDisplayForm()
        {
            initSelection();
            initChannels();
        }

        private void initChannels() // Clear ChannelSet
        {
            channelSet.Clear();
            Unload_CheckPictureBox();
        }

        private void initSelection() // init Value
        {
            unloadChannel_cleanup();
            channel1_txt.Text = null;
            channel2_txt.Text = null;
            channel3_txt.Text = null;
            channel4_txt.Text = null;
            channel5_txt.Text = null;
            channel6_txt.Text = null;
            channel7_txt.Text = null;
            channel8_txt.Text = null;
            channel1_chk.Checked = false;
            fileNameFormat_combo.SelectedIndex = 0;
            fileType_combo.SelectedIndex = 0;
            dateFormat_txt.Text = defaultDateFormat;
            feedDate_date.Value = referenceDateTime;
            getFileFormatSample();
        }

        private void Unload_Form() // Unload Full Form
        {
            channel1_txt.Text = "";
            channel2_txt.Text = "";
            channel3_txt.Text = "";
            channel4_txt.Text = "";
            channel5_txt.Text = "";
            channel6_txt.Text = "";
            channel7_txt.Text = "";
            channel8_txt.Text = "";
            channel9_txt.Text = "";
            channel1_chk.Checked = false;
            channel2_chk.Checked = false;
            channel3_chk.Checked = false;
            channel4_chk.Checked = false;
            channel5_chk.Checked = false;
            channel6_chk.Checked = false;
            channel7_chk.Checked = false;
            channel8_chk.Checked = false;
            channel9_chk.Checked = false;
            txt_Newproject.Text = "";
            lab_SelectProjectName.Text = "";
            lab_ProjectName.Text = "";
            lab_ProjectID.Text = "";
            lab_ProjectType.Text = "";
            Unload_CheckPictureBox();
        }

        private void Unload_CheckPictureBox() // Unload Check sign PictureBox
        {
            PictureBox[] pictureBoxArry = new PictureBox[9];
            // Picture bOx Array
            pictureBoxArry[0] = PB_Check1;
            pictureBoxArry[1] = PB_Check2;
            pictureBoxArry[2] = PB_Check3;
            pictureBoxArry[3] = PB_Check4;
            pictureBoxArry[4] = PB_Check5;
            pictureBoxArry[5] = PB_Check6;
            pictureBoxArry[6] = PB_Check7;
            pictureBoxArry[7] = PB_Check8;
            pictureBoxArry[8] = PB_Check9;
            for (int i = 0; i < 9; i++)
            {
                pictureBoxArry[i].Visible = false;
            }
        }

        private void unloadChannel_cleanup() // Unload Text
        {
            channel1_txt.Enabled = true;
            channel2_txt.Enabled = true;
            channel3_txt.Enabled = true;
            channel4_txt.Enabled = true;
            channel5_txt.Enabled = true;
            channel6_txt.Enabled = true;
            channel7_txt.Enabled = true;
            channel8_txt.Enabled = true;
            fileName_grp.Enabled = true;
            dateFormat_grp.Enabled = true;
        }

        private void Unload_Channeltext() // unload Channel Text
        {
            channel1_txt.Text = "";
            channel2_txt.Text = "";
            channel3_txt.Text = "";
            channel4_txt.Text = "";
            channel5_txt.Text = "";
            channel5_txt.Text = "";
            channel6_txt.Text = "";
            channel7_txt.Text = "";
            channel8_txt.Text = "";
            channel9_txt.Text = "";
        }
        #endregion-----------------------------------------------------------------------------------------------------------------------------------------------------------


        #region Load, Click
        // Form Load
        private void frm_SaveChannel_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            Add_ListProjectName();
            Get_CmbTagType();
            //button1.Hide();
            PB_VideoMerge.Visible = false;
        }

        // btn Add New Project
        private void btn_AddProject_Click(object sender, EventArgs e)
        {
            if (txt_Newproject.Text != "" && cmb_TagMaster.SelectedItem != null)
            {
                Unload_Channeltext();
                Insert_NewProject();
                Add_ListProjectName();
                Get_dbProjectID_Display(); // Display Project ID in lab
                txt_Newproject.Text = "";
            }
            else
            {
                if (txt_Newproject.Text == "")
                {
                    MessageBox.Show("Please Enter Project Name");
                    txt_Newproject.Focus();
                }
                else
                {
                    MessageBox.Show("Please Select Project Type");
                    cmb_TagMaster.Focus();
                }
            }
        }
        private void btn_Newproject_Click(object sender, EventArgs e)
        {
            if (txt_Newproject.Text != "" && cmb_TagMaster.SelectedItem != null)
            {
                Unload_Channeltext();
                Insert_NewProject();
                Add_ListProjectName();
                Get_dbProjectID_Display(); // Display Project ID in lab
                txt_Newproject.Text = "";
            }
            else
            {
                if (txt_Newproject.Text == "")
                {
                    MessageBox.Show("Please Enter Project Name");
                    txt_Newproject.Focus();
                }
                else
                {
                    MessageBox.Show("Please Select Project Type");
                    cmb_TagMaster.Focus();
                }
            }
        }

        // Text search      
        private void txt_Search_TextChanged(object sender, EventArgs e)
        {
            Search_TxtBox();
        }

        // Data & Number
        private void dateFormat_txt_KeyUp(object sender, KeyEventArgs e)
        {
            getFileFormatSample();
        }

        private void feedDate_date_ValueChanged(object sender, EventArgs e)
        {
            getFileFormatSample();
        }

        private void prefix_txt_KeyUp(object sender, KeyEventArgs e)
        {
            getFileFormatSample();
        }

        private void suffix_txt_KeyUp(object sender, KeyEventArgs e)
        {
            getFileFormatSample();
        }

        // For the save the video path & convert into Snapshot
        private void button2_Click(object sender, EventArgs e)
        {
            if (lab_SelectProjectName.Text != "")
            {
                try
                {
                    Thread threadInput = new Thread(Click_StartAuditing);
                    threadInput.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                MessageBox.Show("Please Select Any Project");
            }
        }
        private void Click_StartAuditing()
        {
            this.Invoke((MethodInvoker)delegate
            {
                Save_Channel_Path();
                c_proName = lab_SelectProjectName.Text;
                c_proType = lab_ProjectType.Text;
                ImageScrollerForm ISF = new ImageScrollerForm();
                ISF.Show();
                Unload_Form();
            });
        }

        // For Combo Box
        private void fileNameFormat_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            getFileFormatSample();
        }

        private void fileType_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            getFileFormatSample();
        }

        //btn Video convert
        private void btn_VideoConvert_Click(object sender, EventArgs e)
        {
            //Thread tid1 = new Thread(new ThreadStart(SetLoad));
            //Thread tid2 = new Thread(new ThreadStart(Convert_VideoToVideo_FFmpeg));
            //tid1.Start();
            //tid2.Start();
            //MessageBox.Show("Done");
            try
            {
                Thread threadInput = new Thread(Convert_VideoToVideo_FFmpeg);
                threadInput.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            //Convert_VideoToVideo();    
        }
        private void btn_ConVideo_Click(object sender, EventArgs e)
        {
            //Thread tid1 = new Thread(new ThreadStart(SetLoad));
            //Thread tid2 = new Thread(new ThreadStart(Convert_VideoToVideo_FFmpeg));
            //tid1.Start();
            //tid2.Start();
            //MessageBox.Show("Done");
            try
            {
                Thread threadInput = new Thread(Convert_VideoToVideo_FFmpeg);
                threadInput.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            //Convert_VideoToVideo();          
        }

        // Loading Picture Box      
        private void SetLoading(bool displayLoader)
        {
            if (displayLoader)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    PB_VideoMerge.BackColor = Color.Transparent;
                    PB_VideoMerge.Visible = true;
                    this.Cursor = Cursors.WaitCursor;
                    //Size size = new Size(200, 150);
                    //PB_VideoMerge.Size = size;                              
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    PB_VideoMerge.Visible = false;
                    this.Cursor = Cursors.Default;
                });
            }
        }

        // btn video Browse Click
        private void Browser_Video_Click(object sender, EventArgs e)
        {
            Add_ListVideoName();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            DialogResult dr = ofd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                foreach (String file in ofd.FileNames)
                {
                    vfile_path = file; // file path               
                    ListViewItem lv = new ListViewItem(vfile_path);
                    listView_VideoFile.Items.Add(lv);
                }
            }
        }
        private void btn_BrowserVideo_Click(object sender, EventArgs e)
        {
            Add_ListVideoName();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            DialogResult dr = ofd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                foreach (String file in ofd.FileNames)
                {
                    vfile_path = file; // file path               
                    ListViewItem lv = new ListViewItem(vfile_path);
                    listView_VideoFile.Items.Add(lv);
                }
            }
            //OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Title = "My Video Browser";
            //ofd.Multiselect = true;
            //if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    vfile_path = ofd.FileName; // file path               
            //    ListViewItem lv = new ListViewItem(vfile_path);
            //    listView_VideoFile.Items.Add(lv);
            //}
        }

        // btn list view clear
        private void Btn_ClearPB_Click(object sender, EventArgs e)
        {
            listView_VideoFile.Items.Clear();
        }
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            listView_VideoFile.Items.Clear();
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

        // For The btn_browser
        private void Browse_PB1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel1_txt.Text = vfile_path;
            }
        }

        private void Browse_PB2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel2_txt.Text = vfile_path;
            }
        }

        private void Browse_PB3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel3_txt.Text = vfile_path;
            }
        }

        private void Browse_PB4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel4_txt.Text = vfile_path;
            }
        }

        private void Browse_PB5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel5_txt.Text = vfile_path;
            }
        }

        private void Browse_PB6_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel6_txt.Text = vfile_path;
            }
        }

        private void Browse_PB7_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel7_txt.Text = vfile_path;
            }
        }

        private void Browse_PB8_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel8_txt.Text = vfile_path;
            }
        }

        private void Browse_PB9_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel9_txt.Text = vfile_path;
            }

            //folderDlg.ShowNewFolderButton = true;
            //DialogResult result = folderDlg.ShowDialog();
            //if (result == DialogResult.OK)
            //{
            //    channel9_txt.Text = folderDlg.SelectedPath;
            //    Environment.SpecialFolder root = folderDlg.RootFolder;
            //}
        }       

        #endregion-----------------------------------------------------------------------------------------------------------------------------------------------------------


        #region Mouse Enter/Leave/Move
        // Mouse Enter in picturebox 
        private void Browse_MouseEnter(object sender, EventArgs e)
        {
            string LableText = ((PictureBox)sender).Name;
            switch (LableText)
            {
                case "Browse_PB1":
                    Browse_PB1.Image = BrowseOrgImg;
                    break;
                case "Browse_PB2":
                    Browse_PB2.Image = BrowseOrgImg;
                    break;
                case "Browse_PB3":
                    Browse_PB3.Image = BrowseOrgImg;
                    break;
                case "Browse_PB4":
                    Browse_PB4.Image = BrowseOrgImg;
                    break;
                case "Browse_PB5":
                    Browse_PB5.Image = BrowseOrgImg;
                    break;
                case "Browse_PB6":
                    Browse_PB6.Image = BrowseOrgImg;
                    break;
                case "Browse_PB7":
                    Browse_PB7.Image = BrowseOrgImg;
                    break;
                case "Browse_PB8":
                    Browse_PB8.Image = BrowseOrgImg;
                    break;
                case "Browse_PB9":
                    Browse_PB9.Image = BrowseOrgImg;
                    break;
                case "Browser_Video":
                    Browser_Video.Image = BrowseOrgImg;
                    break;
                case "Btn_ClearPB":
                    Btn_ClearPB.Image = ClearOrgImg;
                    break;
                case "btn_VideoConvert":
                    btn_VideoConvert.Image = MergeOrg30;
                    break;
                default:
                    break;
            }
        }

        // Mouse Leave in picturebox 
        private void Browse_MouseLeave(object sender, EventArgs e)
        {
            string LableText = ((PictureBox)sender).Name;
            //string LableText = this.Name;
            switch (LableText)
            {
                case "Browse_PB1":
                    Browse_PB1.Image = BrowseGryImg;
                    break;
                case "Browse_PB2":
                    Browse_PB2.Image = BrowseGryImg;
                    break;
                case "Browse_PB3":
                    Browse_PB3.Image = BrowseGryImg;
                    break;
                case "Browse_PB4":
                    Browse_PB4.Image = BrowseGryImg;
                    break;
                case "Browse_PB5":
                    Browse_PB5.Image = BrowseGryImg;
                    break;
                case "Browse_PB6":
                    Browse_PB6.Image = BrowseGryImg;
                    break;
                case "Browse_PB7":
                    Browse_PB7.Image = BrowseGryImg;
                    break;
                case "Browse_PB8":
                    Browse_PB8.Image = BrowseGryImg;
                    break;
                case "Browse_PB9":
                    Browse_PB9.Image = BrowseGryImg;
                    break;
                case "Browser_Video":
                    Browser_Video.Image = BrowsewhiteImg;
                    break;
                case "Btn_ClearPB":
                    Btn_ClearPB.Image = ClearWhiteImg;
                    break;
                case "btn_VideoConvert":
                    btn_VideoConvert.Image = MeargWhite30;
                    break;
                default:
                    break;
            }

            mv_Channel.Text = "";
            mv_SelectVideo.Text = "";
        }

        // Case Mouse Hover -- move for PictueBox
        private void Browse_PB1_MouseMove(object sender, MouseEventArgs e)
        {
            //String Location_Mouse = " X-Coordinate : " + e.X + "  Y -Coordinate : " + e.Y;
            //X_Mouse = e.X;
            //Y_Mouse = e.Y;
            //var screenPosition = this.PointToScreen(Browse_PB1.Location);
            //this.mv_Channel.Location = new Point(X_Mouse + 10 + screenPosition.X, Y_Mouse + 10 + screenPosition.Y);
            //mv_Channel.Text = "Add New Project";

            String Location_Mouse = " X-Coordinate : " + e.X + "  Y -Coordinate : " + e.Y;
            X_Mouse = e.X;
            Y_Mouse = e.Y;
            string LableText = ((PictureBox)sender).Name;
            switch (LableText)
            {
                case "Browse_PB1":
                    Browse_PB1.Image = BrowseOrgImg;
                    var screenPosition = this.PointToScreen(Browse_PB1.Location);
                    this.mv_Channel.Location = new Point(X_Mouse + 10 + screenPosition.X, Y_Mouse + 10 + screenPosition.Y);
                    mv_Channel.Text = "Browse Video";
                    break;
                case "Browse_PB2":
                    Browse_PB2.Image = BrowseOrgImg;
                    var screenPosition2 = this.PointToScreen(Browse_PB2.Location);
                    this.mv_Channel.Location = new Point(X_Mouse + 10 + screenPosition2.X, Y_Mouse + 10 + screenPosition2.Y);
                    mv_Channel.Text = "Browse Video";
                    break;
                case "Browse_PB3":
                    Browse_PB3.Image = BrowseOrgImg;
                    var screenPosition3 = this.PointToScreen(Browse_PB3.Location);
                    this.mv_Channel.Location = new Point(X_Mouse + 10 + screenPosition3.X, Y_Mouse + 10 + screenPosition3.Y);
                    mv_Channel.Text = "Browse Video";
                    break;
                case "Browse_PB4":
                    Browse_PB4.Image = BrowseOrgImg;
                    var screenPosition4 = this.PointToScreen(Browse_PB4.Location);
                    this.mv_Channel.Location = new Point(X_Mouse + 10 + screenPosition4.X, Y_Mouse + 10 + screenPosition4.Y);
                    mv_Channel.Text = "Browse Video";
                    break;
                case "Browse_PB5":
                    Browse_PB5.Image = BrowseOrgImg;
                    var screenPosition5 = this.PointToScreen(Browse_PB5.Location);
                    this.mv_Channel.Location = new Point(X_Mouse + 10 + screenPosition5.X, Y_Mouse + 10 + screenPosition5.Y);
                    mv_Channel.Text = "Browse Video";
                    break;
                case "Browse_PB6":
                    Browse_PB6.Image = BrowseOrgImg;
                    var screenPosition6 = this.PointToScreen(Browse_PB6.Location);
                    this.mv_Channel.Location = new Point(X_Mouse + 10 + screenPosition6.X, Y_Mouse + 10 + screenPosition6.Y);
                    mv_Channel.Text = "Browse Video";
                    break;
                case "Browse_PB7":
                    Browse_PB7.Image = BrowseOrgImg;
                    var screenPosition7 = this.PointToScreen(Browse_PB7.Location);
                    this.mv_Channel.Location = new Point(X_Mouse + 10 + screenPosition7.X, Y_Mouse + 10 + screenPosition7.Y);
                    mv_Channel.Text = "Browse Video";
                    break;
                case "Browse_PB8":
                    Browse_PB8.Image = BrowseOrgImg;
                    var screenPosition8 = this.PointToScreen(Browse_PB8.Location);
                    this.mv_Channel.Location = new Point(X_Mouse + 10 + screenPosition8.X, Y_Mouse + 10 + screenPosition8.Y);
                    mv_Channel.Text = "Browse Video";
                    break;
                case "Browse_PB9":
                    Browse_PB9.Image = BrowseOrgImg;
                    var screenPosition9 = this.PointToScreen(Browse_PB9.Location);
                    this.mv_Channel.Location = new Point(X_Mouse + 10 + screenPosition9.X, Y_Mouse + 10 + screenPosition9.Y);
                    mv_Channel.Text = "Browse Video";
                    break;
                case "Browser_Video":
                    Browser_Video.Image = BrowseOrgImg;
                    var XY = this.PointToScreen(Browser_Video.Location);
                    this.mv_SelectVideo.Location = new Point(X_Mouse + 10 + XY.X, Y_Mouse + 10 + XY.Y);
                    mv_SelectVideo.Text = "Browse Video";
                    break;
                case "Btn_ClearPB":
                    Btn_ClearPB.Image = ClearOrgImg;
                    var Btn_ClearPBXY = this.PointToScreen(Btn_ClearPB.Location);
                    this.mv_SelectVideo.Location = new Point(X_Mouse + 10 + Btn_ClearPBXY.X, Y_Mouse + 10 + Btn_ClearPBXY.Y);
                    mv_SelectVideo.Text = "Clear";
                    break;
                case "btn_VideoConvert":
                    btn_VideoConvert.Image = MergeOrg30;
                    var btn_VideoConvertXY = this.PointToScreen(btn_VideoConvert.Location);
                    this.mv_SelectVideo.Location = new Point(X_Mouse + 10 + btn_VideoConvertXY.X, Y_Mouse + 10 + btn_VideoConvertXY.Y);
                    mv_SelectVideo.Text = "Start Convert";
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
                case "btn_AddProject":
                    btn_AddProject.ForeColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                    btn_AddProject.BackColor = System.Drawing.Color.WhiteSmoke;
                    break;
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
                case "btn_AddProject":
                    btn_AddProject.ForeColor = System.Drawing.Color.WhiteSmoke;
                    btn_AddProject.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
                    break;
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

            mv_NewProject.Text = "";
            mv_Header.Text = "";
        }

        // Case Mouse Hover -- move for Lable
        private void btn_AddProject_MouseMove(object sender, MouseEventArgs e)
        {
            String Location_Mouse = " X-Coordinate : " + e.X + "  Y -Coordinate : " + e.Y;
            X_Mouse = e.X;
            Y_Mouse = e.Y;
            string LableText = ((Label)sender).Name;
            switch (LableText)
            {
                case "btn_AddProject":
                    var screenPosition = this.PointToScreen(btn_AddProject.Location);
                    this.mv_NewProject.Location = new Point(X_Mouse + 10 + screenPosition.X, Y_Mouse + 10 + screenPosition.Y);
                    mv_NewProject.Text = "Add Project";
                    break;
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



    }
}





