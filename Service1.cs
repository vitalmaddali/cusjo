using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        Thread mainThreadObj;

        DateTime currentDateTime;

        Thread objProcessThread;

        string recievermailid = "xxxxxxx";
        string subject = "Employee Alert Mail";
        string bodyText = "This is an Employee Alert Mail.";

        public Service1()
        {
            InitializeComponent();
        }

        public void start()
        {

            string[] args = default(string[]);
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            currentDateTime = DateTime.Now;

            mainThreadObj = new Thread(ProcessThread);

            mainThreadObj.Start();

        }

        protected override void OnStop()
        {
        }

        private void ProcessThread()
        {
            if (objProcessThread == null ||
            objProcessThread.ThreadState == System.Threading.ThreadState.Stopped ||
            objProcessThread.ThreadState == System.Threading.ThreadState.Unstarted)
            {


                while (DateTime.Now >= currentDateTime.AddHours(12))//Every 12 Hours
                {
                    objProcessThread = new Thread(threadProcessMethod);

                    objProcessThread.IsBackground = true;

                    objProcessThread.Start();

                    currentDateTime = currentDateTime.AddSeconds(10);

                    Thread.Sleep(12 * (60 * (1000 * 60))); //Every 12 Hours

                    ProcessThread();
                }

                Thread.Sleep(12 * (60 * (1000 * 60))); //Every 12 Hours

                ProcessThread();
            }
        }

        private void threadProcessMethod()
        {
            sendmail(recievermailid, subject, bodyText);
        }

        public bool sendmail(string recievermailid, string subject, string bodyText)
        {
            try
            {
                string senderId = "********"; // Sender EmailID
                string senderPassword = "*********"; // Sender Password

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.To.Add(recievermailid);
                mailMessage.From = new MailAddress(senderId);

                mailMessage.Subject = subject;
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

                mailMessage.Body = bodyText;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.IsBodyHtml = false;

                mailMessage.Priority = MailPriority.High;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Credentials = new System.Net.NetworkCredential(senderId, senderPassword);
                smtpClient.Port = 587;
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.EnableSsl = true;

                object userState = mailMessage;

                try
                {
                    smtpClient.Send(mailMessage);
                    return true;
                }
                catch (System.Net.Mail.SmtpException)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
