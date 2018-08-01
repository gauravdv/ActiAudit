using System;
using System.Collections.Generic;
using System.IO;

namespace ImageScroller
{
	public class FileOperation
	{
        string String_Iamges;
        
        public String saveFile(Dictionary<int, ChannelInfo> channelSet, String currentImageFilename, String destinationBasePath, String TagType, String TagReason, out string String_Iamges, out string saveSuccess, out string destinationPath)
		{
            saveSuccess = "false";
            String_Iamges = string.Empty;
            destinationPath = string.Empty;
            try
            {
                if (destinationBasePath != null && !string.IsNullOrEmpty(destinationBasePath) && currentImageFilename != null && !string.IsNullOrEmpty(currentImageFilename))
                {
                    foreach (KeyValuePair<int, ChannelInfo> channelSetItem in channelSet)
                    {
                        ChannelInfo _channelInfo = channelSetItem.Value;

                        if (_channelInfo.isSelected && _channelInfo.cs_isSelected && _channelInfo.FileBasePath != null && !string.IsNullOrEmpty(_channelInfo.FileBasePath))
                        {
                            System.Windows.Forms.PictureBox _imageViewer = new System.Windows.Forms.PictureBox();
                            String sourcePath = string.Empty;                           

                            int Add_Channe = _channelInfo.ChannelNumber;
                            int _channelNumber = Add_Channe + 1;
                            sourcePath = _channelInfo.FileBasePath;
                            if (!sourcePath.Substring(sourcePath.Length - 1, 1).Equals("\\"))
                            {
                                sourcePath = sourcePath + "\\";
                            }
                            if (!destinationBasePath.Substring(destinationBasePath.Length - 1, 1).Equals("\\"))
                            {
                                destinationBasePath = destinationBasePath + "\\";
                            }
                            string _Tag_Reason = TagReason.Replace(" ", "_");
                            string Tag_Reason2 = TagReason; 
                            String TagReasonPath = destinationBasePath + Tag_Reason2;
                            if (!Directory.Exists(TagReasonPath))
                            {
                                Directory.CreateDirectory(TagReasonPath);
                            }
                            destinationPath = TagReasonPath + "\\" + "Channel_" + _channelNumber.ToString() + "\\";
                            if (!Directory.Exists(destinationPath))
                            {
                                Directory.CreateDirectory(destinationPath);
                            }
                            if (File.Exists(sourcePath + currentImageFilename))
                            {
                                string jpg = Path.GetExtension(currentImageFilename);
                                string name = Path.GetFileNameWithoutExtension(currentImageFilename);
                                string value2 = String.Format("{0:D5}", name);
                                value2 = name.PadLeft(5, '0');
                                // string value = "0000" + name.ToString();
                                string CIFN = value2 + jpg;
                                File.Copy(sourcePath + currentImageFilename, destinationPath + CIFN, true);
                               
                                //A1
                                //int A1 = Int32.Parse(name) + 1;
                                string A1_name = (Int32.Parse(name) + 1).ToString();
                                string A1jpg = Int32.Parse(name) + 1 + jpg;
                                string A1value = String.Format("{0:D5}", A1jpg);
                                string A1value1 = A1_name.PadLeft(5, '0');
                                string A1CIFN = A1value1 + jpg;
                                File.Copy(sourcePath + A1jpg, destinationPath + A1CIFN, true);
                                //A2
                                string A2_name = (Int32.Parse(name) + 2).ToString();
                                string A2jpg = Int32.Parse(name) + 2 + jpg;
                                string A2value = String.Format("{0:D5}", A2jpg);
                                string A2value1 = A2_name.PadLeft(5, '0');
                                string A2CIFN = A2value1 + jpg;
                                File.Copy(sourcePath + A2jpg, destinationPath + A2CIFN, true);
                                //S1
                                string S1_name = (Int32.Parse(name) - 1).ToString();
                                int S1int = Int32.Parse(name) - 1;
                                string S1jpg = S1int.ToString() + jpg;
                                string S1value = String.Format("{0:D5}", S1jpg);
                                string S1value1 = S1_name.PadLeft(5, '0');
                                string S1CIFN = S1value1 + jpg;
                                File.Copy(sourcePath + S1jpg, destinationPath + S1CIFN, true);
                                //S2
                                string S2_name = (Int32.Parse(name) - 2).ToString();
                                int S2int = Int32.Parse(name) - 2;
                                string S2jpg = S2int.ToString() + jpg;
                                string S2value = String.Format("{0:D5}", S2jpg);
                                string S2value1 = S2_name.PadLeft(5, '0');
                                string S2CIFN = S2value1 + jpg;
                                File.Copy(sourcePath + S2jpg, destinationPath + S2CIFN, true);

                                String_Iamges = S2CIFN + ";" + S1CIFN + ";" + CIFN + ";" + A1CIFN + ";" + A2CIFN;
                                                              
                                saveSuccess = "true";                                
                            }
                            else
                            {
                                saveSuccess = "false";
                                break;
                            }
                        }                        
                    }
                }                
            }
            catch (Exception e)
            {
                saveSuccess = "false";
            }
            return saveSuccess;
        }

        public bool saveFile_old(Dictionary<int, ChannelInfo> channelSet, String currentImageFilename, String destinationBasePath, String TagType, String TagReason)
        {
            bool saveSuccess = false;

            try
            {
                if (destinationBasePath != null && !string.IsNullOrEmpty(destinationBasePath) && currentImageFilename != null && !string.IsNullOrEmpty(currentImageFilename))
                {
                    foreach (KeyValuePair<int, ChannelInfo> channelSetItem in channelSet)
                    {
                        ChannelInfo _channelInfo = channelSetItem.Value;

                        if (_channelInfo.isSelected && _channelInfo.cs_isSelected && _channelInfo.FileBasePath != null && !string.IsNullOrEmpty(_channelInfo.FileBasePath))
                        {
                            System.Windows.Forms.PictureBox _imageViewer = new System.Windows.Forms.PictureBox();
                            String sourcePath = string.Empty;
                            String destinationPath = string.Empty;

                            int Add_Channe = _channelInfo.ChannelNumber;
                            int _channelNumber = Add_Channe + 1;
                            sourcePath = _channelInfo.FileBasePath;

                            if (!sourcePath.Substring(sourcePath.Length - 1, 1).Equals("\\"))
                            {
                                sourcePath = sourcePath + "\\";
                            }
                            if (!destinationBasePath.Substring(destinationBasePath.Length - 1, 1).Equals("\\"))
                            {
                                destinationBasePath = destinationBasePath + "\\";
                            }
                            String TagReasonPath = destinationBasePath + TagReason;
                            if (!Directory.Exists(TagReasonPath))
                            {
                                Directory.CreateDirectory(TagReasonPath);
                            }
                            destinationPath = TagReasonPath + "\\" + "Channel_" + _channelNumber.ToString() + "\\";
                            if (!Directory.Exists(destinationPath))
                            {
                                Directory.CreateDirectory(destinationPath);
                            }
                            if (File.Exists(sourcePath + currentImageFilename))
                            {
                                string jpg = Path.GetExtension(currentImageFilename);
                                string name = Path.GetFileNameWithoutExtension(currentImageFilename);
                                string value2 = String.Format("{0:D5}", name);
                                value2 = name.PadLeft(5, '0');
                                // string value = "0000" + name.ToString();
                                string CIFN = value2 + jpg;
                                File.Copy(sourcePath + currentImageFilename, destinationPath + CIFN, true);
                                //for (int i = 1; i <= 2; i++)
                                //{
                                //A1
                                //int A1 = Int32.Parse(name) + 1;
                                string A1_name = (Int32.Parse(name) + 1).ToString();
                                string A1jpg = Int32.Parse(name) + 1 + jpg;
                                string A1value = String.Format("{0:D5}", A1jpg);
                                string A1value1 = A1_name.PadLeft(5, '0');
                                string A1CIFN = A1value1 + jpg;
                                File.Copy(sourcePath + A1jpg, destinationPath + A1CIFN, true);
                                //A2
                                string A2_name = (Int32.Parse(name) + 2).ToString();
                                string A2jpg = Int32.Parse(name) + 2 + jpg;
                                string A2value = String.Format("{0:D5}", A2jpg);
                                string A2value1 = A2_name.PadLeft(5, '0');
                                string A2CIFN = A2value1 + jpg;
                                File.Copy(sourcePath + A2jpg, destinationPath + A2CIFN, true);
                                //S1
                                string S1_name = (Int32.Parse(name) - 1).ToString();
                                int S1int = Int32.Parse(name) - 1;
                                string S1jpg = S1int.ToString() + jpg;
                                string S1value = String.Format("{0:D5}", S1jpg);
                                string S1value1 = S1_name.PadLeft(5, '0');
                                string S1CIFN = S1value1 + jpg;
                                File.Copy(sourcePath + S1jpg, destinationPath + S1CIFN, true);
                                //S2
                                string S2_name = (Int32.Parse(name) - 2).ToString();
                                int S2int = Int32.Parse(name) - 2;
                                string S2jpg = S2int.ToString() + jpg;
                                string S2value = String.Format("{0:D5}", S2jpg);
                                string S2value1 = S2_name.PadLeft(5, '0');
                                string S2CIFN = S2value1 + jpg;
                                File.Copy(sourcePath + S2jpg, destinationPath + S2CIFN, true);

                                String_Iamges = S2CIFN + "," + S1CIFN + "," + CIFN + "," + A1CIFN + "," + A2CIFN;

                                //}
                                saveSuccess = true;
                            }
                            else
                            {
                                saveSuccess = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                saveSuccess = false;
            }

            return saveSuccess;
        }
    }
}
