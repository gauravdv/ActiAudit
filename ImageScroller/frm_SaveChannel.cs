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

namespace ImageScroller
{
    public partial class frm_SaveChannel : Form
    {
        
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
            cmd.CommandText = "SELECT Project_ID,Project_Type FROM project_detail WHERE Project_name = '" + lab_SelectProjectName.Text + "' AND computer_IP = '" + myIP +"' ";
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
            if (txt_Newproject.Text != "")
            {
                foreach (ListViewItem item in listView_ProjectName.Items)
                {
                    if (item.Text.ToLower().Contains(txt_Newproject.Text.ToLower()))
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
                    txt_Newproject.Focus();
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


        private void Get_CheckPictureBox(int i)
        {
            PictureBox[] pictureBoxArry = new PictureBox[9];
            // Picture bOx Array
            pictureBoxArry[0] = pictureBox1;
            pictureBoxArry[1] = pictureBox2;
            pictureBoxArry[2] = pictureBox3;
            pictureBoxArry[3] = pictureBox4;
            pictureBoxArry[4] = pictureBox5;
            pictureBoxArry[5] = pictureBox6;
            pictureBoxArry[6] = pictureBox7;
            pictureBoxArry[7] = pictureBox8;
            pictureBoxArry[8] = pictureBox9;

            //string ExeLoaction = System.Reflection.Assembly.GetEntryAssembly().Location; ;
            //string txtFilePath = Path.GetDirectoryName(ExeLoaction); // Folder path
            //string txtpath = txtFilePath + "\\" + "Check_mark.png";
            //pictureBoxArry[i].SizeMode = PictureBoxSizeMode.StretchImage;
            //pictureBoxArry[i].ImageLocation = @"C:\\Users\\konsultera\\Desktop\\SGDT\\Check.jpg"; 
            pictureBoxArry[i].Show();
            pictureBoxArry[i].SizeMode = PictureBoxSizeMode.StretchImage;
            //pictureBoxArry[i].ImageLocation = @txtpath;
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
                    string Vp_path_2 = "\""+ vp_Path + "\"";

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
                    Process process = Process.Start(batfilepath);
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.WaitForExit();
                    //MessageBox.Show("Convert" + " " + Channel_name + " " + "Video into Images");
                    File.Delete(batfilepath); // delete bat file  

                    Get_CheckPictureBox(i);
                }
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
        private void Convert_VideoToVideo()
        {
            int lvCount = listView_VideoFile.Items.Count;
            if (lvCount != 0)
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
                    writer.WriteLine("PAUSE");
                }
                try
                {
                    var startInfo = new ProcessStartInfo();
                    startInfo.WorkingDirectory = vfolder_path;
                    startInfo.FileName = batfilename;
                    startInfo.CreateNoWindow = true;
                    Process process = Process.Start(startInfo);
                    process.WaitForExit();
                    File.Delete(batfilepath);
                    File.Delete(textfilepath);
                    listView_VideoFile.Items.Clear();
                    System.Diagnostics.Process.Start(path_SaveVideo);
                }
                catch
                {
                    //if (process != null) process.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Please Select Video");
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

        private void listView_ProjectName_SelectedIndexChanged_1(object sender, EventArgs e)
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

        private void listView_VideoFile_SelectedIndexChanged(object sender, EventArgs e)
        {
           
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
            lab_ProjectID.Text = "";
            lab_ProjectType.Text = "";
            Unload_CheckPictureBox();
        }

        private void Unload_CheckPictureBox() // Unload Check sign PictureBox
        {
            PictureBox[] pictureBoxArry = new PictureBox[9];
            // Picture bOx Array
            pictureBoxArry[0] = pictureBox1;
            pictureBoxArry[1] = pictureBox2;
            pictureBoxArry[2] = pictureBox3;
            pictureBoxArry[3] = pictureBox4;
            pictureBoxArry[4] = pictureBox5;
            pictureBoxArry[5] = pictureBox6;
            pictureBoxArry[6] = pictureBox7;
            pictureBoxArry[7] = pictureBox8;
            pictureBoxArry[8] = pictureBox9;
            for (int i = 0; i < 9; i++)
            {
                pictureBoxArry[i].Hide();
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
            Add_ListProjectName();
            Get_CmbTagType();
            //button1.Hide();
        }

        // btn Add New Project
        private void btn_Newproject_Click_1(object sender, EventArgs e)
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

        // Delete Project 
        private void listView_ProjectName_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }
       
        // Text search
        private void txt_Newproject_TextChanged(object sender, EventArgs e)
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
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (lab_SelectProjectName.Text != "")
            {
                Save_Channel_Path();
                c_proName = lab_SelectProjectName.Text;
                c_proType = lab_ProjectType.Text;
                ImageScrollerForm ISF = new ImageScrollerForm();
                ISF.Show();
                Unload_Form();
            }
            else
            {
                MessageBox.Show("Please Select Any Project");
            }
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

        private void btn_ConVideo_Click(object sender, EventArgs e)
        {
            Convert_VideoToVideo();
            //Convert_VideoToVideo_FFmpeg();
        }

        // For The btn_VideoName
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

        // Btn_Clear
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            listView_VideoFile.Items.Clear();
        }

        // For The btn_browser
        private void btn_browser1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel1_txt.Text = vfile_path;
            }
        }
        private void btn_browser2_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel2_txt.Text = vfile_path;
            }
        }
        private void btn_browser3_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel3_txt.Text = vfile_path;
            }
        }
        private void btn_browser4_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel4_txt.Text = vfile_path;
            }
        }
        private void btn_browser5_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel5_txt.Text = vfile_path;
            }
        }
        private void btn_browser6_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel6_txt.Text = vfile_path;
            }
        }
        private void btn_browser7_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel7_txt.Text = vfile_path;
            }
        }
        private void btn_browser8_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vfile_path = ofd.FileName; // file path
                channel8_txt.Text = vfile_path;
            }
        }
        private void btn_browser9_Click_1(object sender, EventArgs e)
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

        private void button1_Click_1(object sender, EventArgs e)
        {           
            //// If Exists Then Delete
            //if (File.Exists("sample.mp3"))
            //    try
            //    {
            //        File.Delete("sample.mp3");
            //    }
            //    catch
            //    {
            //        return;
            //    }

            //string strCmdText;
            //strCmdText = @"/C ffmpeg.exe -i C:\\Gaurav\\ImageScroller\\Doc\\Video_images\\output.mp4 -vf fps=1 C:\\Gaurav\\ImageScroller\\Doc\\Video_images\\image2/%%d.jpg"; /// this is just argument example 
            ////strCmdText = @"/C ffmpeg.exe - framerate 1 - f image2 - i C:\Gaurav\ImageScroller\Doc\Video_images\image2 /%% d.jpg - c:v libx264 -vf fps = 25 - pix_fmt yuv420p video.mp4";
            //var proc = System.Diagnostics.Process.Start("CMD.exe", strCmdText);
            //string s = proc.StandardOutput.ReadToEnd();
            //textBox1.Text = s;

            //Process process = new System.Diagnostics.Process();
            //process.StartInfo.FileName = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) +
            //            @"C:\ffmpeg\bin\ffmpeg.exe"; // Change the directory where ffmpeg.exe is.  
            //process.EnableRaisingEvents = false;
            ////process.StartInfo.WorkingDirectory = @"D:\ffmpegx86"; // The output directory  
            //process.StartInfo.Arguments = "-i output.mp4 -vf fps=1 image2 /%% d.jpg";           
            //process.StartInfo.UseShellExecute = true;
            //process.StartInfo.CreateNoWindow = true;
            //process.Start();
            //StreamReader reader = process.StandardOutput;
            //string output = reader.ReadToEnd();
            //Console.WriteLine(output);
            //process.WaitForExit();
            //Close();

            // Text bo0x array        
            //TextBox[] TextBoxArry = new TextBox[9];
            //TextBoxArry[0] = channel1_txt;
            //TextBoxArry[1] = channel2_txt;
            //TextBoxArry[2] = channel3_txt;
            //TextBoxArry[3] = channel4_txt;
            //TextBoxArry[4] = channel5_txt;
            //TextBoxArry[5] = channel6_txt;
            //TextBoxArry[6] = channel7_txt;
            //TextBoxArry[7] = channel8_txt;
            //TextBoxArry[8] = channel9_txt;


            //for (int i = 0; i < 9; i++)
            //{
            //    if (TextBoxArry[i].Text != "")
            //    {
            //        string vfolder_path = Path.GetDirectoryName(TextBoxArry[i].Text); // Folder path
            //        string FileName = Path.GetFileName(TextBoxArry[i].Text); // get File name
            //        string cSnapshot_path = System.IO.Path.GetFileNameWithoutExtension(FileName);
            //        string cSnapshotFolder_path = vfolder_path + "\\" + cSnapshot_path; // create folder path                

            //        System.IO.Directory.CreateDirectory(cSnapshotFolder_path); // create folder

            //        string tempFilename = Path.ChangeExtension(Path.GetTempFileName(), ".bat"); // create bat file
            //        string vp_Path = TextBoxArry[i].Text;
            //        string op_path = cSnapshotFolder_path + "/%%d.jpg";
            //        //string vp_Path = "C:\\Gaurav\\ImageScroller\\Doc\\Video_images\\output.mp4";
            //        //string op_path = "C:\\Gaurav\\ImageScroller\\Doc\\Video_images\\image2/%%d.jpg";
            //        using (StreamWriter writer = new StreamWriter(tempFilename))
            //        {
            //            writer.WriteLine("hide");
            //            writer.WriteLine("cd..");
            //            writer.WriteLine("cd..");
            //            //writer.WriteLine("cd ffmpeg\bin");
            //            //writer.WriteLine("ffmpeg.exe");
            //            writer.WriteLine("ffmpeg -i " + vp_Path + " -vf fps=1 " + op_path + "");
            //            //writer.WriteLine("PAUSE");                      
            //        }
            //        Process process = Process.Start(tempFilename);
            //        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //        process.WaitForExit();
            //        MessageBox.Show("MP4 TO jpg");
            //        File.Delete(tempFilename);
            //    }
            //}

        }




        #endregion-----------------------------------------------------------------------------------------------------------------------------------------------------------

        private void listView_ProjectName_MouseClick(object sender, MouseEventArgs e)
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

        private void listView_VideoFile_MouseClick(object sender, MouseEventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (lab_ProjectID.Text != "" && lab_SelectProjectName.Text != "")
            {
                Create_Excel();
            }
            else
            {
                MessageBox.Show("Select Any Project ....");
            }
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }

}

