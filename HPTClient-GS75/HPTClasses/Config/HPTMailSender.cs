using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;

namespace HPTClient
{
    [DataContract]
    public class HPTMailSender : Notifier
    {
        private string subject;
        [DataMember]
        public string Subject
        {
            get
            {
                return this.subject;
            }
            set
            {
                this.subject = value;
                OnPropertyChanged("Subject");
            }
        }

        private string body;
        [DataMember]
        public string Body
        {
            get
            {
                return this.body;
            }
            set
            {
                this.body = value;
                OnPropertyChanged("Body");
            }
        }

        private ObservableCollection<HPTMailRecipient> mailRecipients;
        [DataMember]
        public ObservableCollection<HPTMailRecipient> MailRecipients
        {
            get
            {
                return this.mailRecipients;
            }
            set
            {
                this.mailRecipients = value;
                OnPropertyChanged("MyProperty");
            }
        }

        private string hpt3FileName;
        [DataMember]
        public string HPT3FileName
        {
            get
            {
                return this.hpt3FileName;
            }
            set
            {
                this.hpt3FileName = value;
                OnPropertyChanged("HPT3FileName");
            }
        }

        private string atgSystemFileName;
        [DataMember]
        public string ATGSystemFileName
        {
            get
            {
                return this.atgSystemFileName;
            }
            set
            {
                this.atgSystemFileName = value;
                OnPropertyChanged("ATGSystemFileName");
            }
        }

        private bool attachATGSystemFile;
        [DataMember]
        public bool AttachATGSystemFile
        {
            get
            {
                return this.attachATGSystemFile;
            }
            set
            {
                this.attachATGSystemFile = value;
                OnPropertyChanged("AttachATGSystemFile");
            }
        }

        private bool attachHPT3File;
        [DataMember]
        public bool AttachHPT3File
        {
            get
            {
                return this.attachHPT3File;
            }
            set
            {
                this.attachHPT3File = value;
                OnPropertyChanged("AttachHPT3File");
            }
        }

        public void SendMail()
        {
            MailMessage mail = new System.Net.Mail.MailMessage();

            System.Net.NetworkCredential cred = new System.Net.NetworkCredential("hpt.travsystem", "Brickleberry2");

            foreach (HPTMailRecipient recipient in this.MailRecipients)
            {
                if (recipient.EMailAddress != null && recipient.EMailAddress != string.Empty)
                {
                    mail.To.Add(recipient.EMailAddress);
                }
            }
            mail.Subject = this.Subject;

            mail.ReplyToList.Add(HPTConfig.Config.EMailAddress);
            mail.From = new System.Net.Mail.MailAddress("hpt.travsystem@gmail.com", "Hjälp på Traven-system");
            mail.Sender = new System.Net.Mail.MailAddress("hpt.travsystem@gmail.com", "Hjälp på Traven-system");
            mail.IsBodyHtml = false;
            mail.Body = this.Body;

            if (this.AttachHPT3File && this.HPT3FileName != string.Empty)
            {
                FileStream fs1 = new FileStream(this.HPT3FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Attachment a1 = new Attachment(fs1, GetFilenameAndRemoveSwedishChars(this.HPT3FileName), System.Net.Mime.MediaTypeNames.Text.Xml);
                a1.ContentType.Name = Path.GetFileName(this.HPT3FileName);
                mail.Attachments.Add(a1);
            }
            if (this.AttachATGSystemFile && this.ATGSystemFileName != string.Empty)
            {
                FileStream fs2 = new FileStream(this.ATGSystemFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Attachment a2 = new Attachment(fs2, GetFilenameAndRemoveSwedishChars(this.ATGSystemFileName), System.Net.Mime.MediaTypeNames.Text.Xml);
                a2.ContentType.Name = Path.GetFileName(this.ATGSystemFileName);
                mail.Attachments.Add(a2);
            }  

            SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com");
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = cred;
            smtp.Port = 587;
            smtp.Send(mail);

            mail.Attachments.Clear();
        }

        private string GetFilenameAndRemoveSwedishChars(string longFilename)
        {
            string filename = Path.GetFileName(longFilename);
            filename = filename.Replace('å', 'a');
            filename = filename.Replace('ä', 'a');
            filename = filename.Replace('ö', 'o');
            filename = filename.Replace('Å', 'A');
            filename = filename.Replace('Ä', 'A');
            filename = filename.Replace('Ö', 'O');
            return filename;
        }

        public static void SendMail(string subject, string body, string hpt3FileName, string atgSystemFileName, IList<HPTMailRecipient> recipients)
        {
            MailMessage mail = new System.Net.Mail.MailMessage();

            System.Net.NetworkCredential cred = new System.Net.NetworkCredential("hpt.travsystem", "Brickleberry2");

            foreach (HPTMailRecipient recipient in recipients)
            {
                if (recipient.EMailAddress != string.Empty)
                {
                    mail.To.Add(recipient.EMailAddress);
                }
            }
            mail.Subject = "Systemtest " + DateTime.Now.ToShortTimeString();

            mail.From = new System.Net.Mail.MailAddress("hpt.travsystem@gmail.com", "Hjälp på Traven-system");
            mail.Sender = new System.Net.Mail.MailAddress("hpt.travsystem@gmail.com", "Hjälp på Traven-system");
            mail.IsBodyHtml = false;
            mail.Body = body;

            if (hpt3FileName != string.Empty)
            {
                mail.Attachments.Add(new Attachment(hpt3FileName));
            }
            if (atgSystemFileName != string.Empty)
            {
                mail.Attachments.Add(new Attachment(atgSystemFileName));
            }            

            SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com");
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = cred;
            smtp.Port = 587;
            smtp.Send(mail);
        }

        //public static void RetrieveMail()
        //{
        //    try
        //    {
        //        TcpClient tcpclient = new TcpClient(); // create an instance of TcpClient
        //        tcpclient.Connect("pop.gmail.com", 995); // HOST NAME POP SERVER and gmail uses port number 995 for POP 
        //        System.Net.Security.SslStream sslstream = new SslStream(tcpclient.GetStream()); // This is Secure Stream // opened the connection between client and POP Server
        //        sslstream.AuthenticateAsClient("pop.gmail.com"); // authenticate as client 
        //        //bool flag = sslstream.IsAuthenticated; // check flag 
        //        System.IO.StreamWriter sw = new StreamWriter(sslstream); // Asssigned the writer to stream
        //        System.IO.StreamReader reader = new StreamReader(sslstream); // Assigned reader to stream
        //        sw.WriteLine("USER hpt.travsystem@gmail.com"); // refer POP rfc command, there very few around 6-9 command
        //        sw.Flush(); // sent to server
        //        sw.WriteLine("PASS FamilyGuy9");
        //        sw.Flush();
        //        sw.WriteLine("RETR 1"); // this will retrive your first email 
        //        sw.Flush();
        //        sw.WriteLine("Quit "); // close the connection
        //        sw.Flush();
        //        string str = string.Empty;
        //        string strTemp = string.Empty;
        //        while ((strTemp = reader.ReadLine()) != null)
        //       {
        //            if (strTemp == ".") // find the . character in line
        //           {
        //                break;
        //           }
        //            if (strTemp.IndexOf("-ERR") != -1)
        //           {
        //                break;
        //           }
        //            str += strTemp;
        //        }
        //   }
        //    catch (Exception ex)
        //   {
                
        //    }
        //}
   }
}
