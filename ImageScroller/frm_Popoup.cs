using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ImageScroller
{
    public partial class frm_Popoup : Form
    {
        string db_Server;//for server
        string db_Name;
        string db_UserID;
        string db_Password;       
        string TagReason_Others;
        private ImageScrollerForm _ImageScrollerForm;
        public static string _TagType;
        public static string _TagReason;
        private MySqlConnection connect;
        //private ImageScrollerForm frm_IamgeS;

        public frm_Popoup(ImageScrollerForm imageScrollerForm)
        {
            InitializeComponent();
            _ImageScrollerForm = imageScrollerForm;

        }
       
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
       
        //Gat Tag Reason
        private void Get_CmbTagReason()
        {
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "SELECT TagReason_name FROM tag_reason ORDER BY TagReason_name ASC";
            cmd.Connection = connect;
            MySqlDataReader ChannelReder = cmd.ExecuteReader();
            while (ChannelReder.Read())
            {
                cmb_TagReason.Items.Add(ChannelReder["TagReason_name"].ToString());
            }
            ChannelReder.Close();
            connect.Close();
        }            

        private void cmb_TagReason_SelectedIndexChanged(object sender, EventArgs e)
        {
            string TagReason = cmb_TagReason.SelectedItem.ToString();        
            if (TagReason == "Others")
            {              
                txt_EnterInput.Show();
                txt_EnterInput.Focus();
                MessageBox.Show("Enter Other Reason");                 
            }
            else
            {                
                TagReason_Others = cmb_TagReason.SelectedItem.ToString();               
            }

        }

        // Save Also Resons & insert into Db
        private void Save_Resons()
        {
            if (cmb_TagReason.SelectedItem != null)
            {
                string TagReason = cmb_TagReason.SelectedItem.ToString();
                if (TagReason == "Others")
                {
                    TagReason_Others = txt_EnterInput.Text;
                    if (TagReason_Others != "")
                    {
                        db_connection();
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = "INSERT INTO tag_reason(TagReason_name) values(@TagReason_name)";
                        cmd.Parameters.AddWithValue("@TagReason_name", TagReason_Others);
                        cmd.Connection = connect;
                        MySqlDataReader Channel = cmd.ExecuteReader();
                        connect.Close();
                    }
                    else
                    {
                        MessageBox.Show("Enter The Tag ReaSon In Given Text Box");
                    }
                }
                else
                {
                    TagReason_Others = cmb_TagReason.SelectedItem.ToString();
                }
                //_TagType = TagType;
                _TagReason = TagReason_Others;
                _ImageScrollerForm.saveScreenshot2(); // go to the SaveScreenShot Function 
                this.Close();
            }
            else
            {
                MessageBox.Show("Select Reason");
            }
        }
        
        // Load Form
        private void frm_Popoup_Load(object sender, EventArgs e)
        {
            txt_EnterInput.Hide();            
            Get_CmbTagReason(); // cmb Tag Reason
        }

        // btn_Save
        private void btn_Save_Click(object sender, EventArgs e)
        {
            Save_Resons();
            //FileOperation fileOps = new FileOperation();
            //ImageScrollerForm frm_SnapShort = new ImageScrollerForm();
            //this.Close();
            //frm_IamgeS = new ImageScrollerForm();
          
        }

        private void frm_Popoup_FormClosed(object sender, FormClosedEventArgs e)
        {
            _ImageScrollerForm.playScroller();
        }

        private void lab_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lab_Close_MouseEnter(object sender, EventArgs e)
        {
            lab_Close.ForeColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
            lab_Close.BackColor = System.Drawing.Color.WhiteSmoke;
        }

        private void lab_Close_MouseLeave(object sender, EventArgs e)
        {
            lab_Close.ForeColor = System.Drawing.Color.WhiteSmoke;
            lab_Close.BackColor = System.Drawing.ColorTranslator.FromHtml("#f26222");
        }
    }
   }
