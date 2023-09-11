using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Linq;
using System.Net.Sockets;

using LumiSoft.Net.STUN.Client;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Collections;

namespace Warcraft_II_Port_Test
{
    /// <summary>
    /// Application main window.
    /// </summary>
    public class wfrm_Main : Form
    {
        private Label         mt_Server         = null;
        private ComboBox      m_pServer         = null;        
        private Label         mt_LocalEndPoint  = null;
        private TextBox       m_pLocalEndPoint  = null;
        private Label         mt_NetType        = null;
        private TextBox       m_pNetType        = null;
        private Label         mt_PublicEndPoint = null;
        private TextBox       m_pPublicEndPoint = null;
        private Button        m_pGet            = null;
        private Button        m_pClose          = null;
        private int           GamePort          = 6112;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public wfrm_Main()
        {
            InitUI();
        }

        #region mehtod InitUI

        /// <summary>
        /// Creates and initilizes UI.
        /// </summary>
        private void InitUI()
        {
            this.ClientSize = new Size(380,175);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = "Warcraft II Port Test";
            this.MaximizeBox = false;

            mt_Server = new Label();
            mt_Server.Size = new Size(100,20);
            mt_Server.Location = new Point(0,10);
            mt_Server.TextAlign = ContentAlignment.MiddleRight;
            mt_Server.Text = "STUN server:";

            m_pServer = new ComboBox();
            m_pServer.Size = new Size(265, 20);
            m_pServer.Location = new Point(105,10);
            m_pServer.Items.Add("stun.t-online.de:3478");
            m_pServer.Items.Add("stun.voipstunt.com:3478");
            m_pServer.SelectedIndex = 0;

            mt_LocalEndPoint = new Label();
            mt_LocalEndPoint.Size = new Size(100,20);
            mt_LocalEndPoint.Location = new Point(0,35);
            mt_LocalEndPoint.TextAlign = ContentAlignment.MiddleRight;
            mt_LocalEndPoint.Text = "Local end point:";

            try
            {
                GamePort = (int)Registry.GetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Battle.net\\Configuration",
                    "Game Data Port",
                    "6112");
            }
            catch { }

            m_pLocalEndPoint = new TextBox();
            m_pLocalEndPoint.Size = new Size(265,20);
            m_pLocalEndPoint.Location = new Point(105,35);
            m_pLocalEndPoint.Text = new IPEndPoint(IPAddress.Any, GamePort).ToString();
            //m_pLocalEndPoint.ReadOnly = true;

            mt_NetType = new Label();
            mt_NetType.Size = new Size(100,20);
            mt_NetType.Location = new Point(0,60);
            mt_NetType.TextAlign = ContentAlignment.MiddleRight;
            mt_NetType.Text = "NET type:";

            m_pNetType = new TextBox();
            m_pNetType.Size = new Size(265,20);
            m_pNetType.Location = new Point(105,60);
            m_pNetType.ReadOnly = true;
                        
            mt_PublicEndPoint = new Label();
            mt_PublicEndPoint.Size = new Size(100,20);
            mt_PublicEndPoint.Location = new Point(0,85);
            mt_PublicEndPoint.TextAlign = ContentAlignment.MiddleRight;
            mt_PublicEndPoint.Text = "Public end points:";

            m_pPublicEndPoint = new TextBox();
            m_pPublicEndPoint.Size = new Size(265,40);
            m_pPublicEndPoint.Location = new Point(105,85);
            m_pPublicEndPoint.ReadOnly = true;
            m_pPublicEndPoint.Multiline = true;

            m_pGet = new Button();
            m_pGet.Size = new Size(70,20);
            m_pGet.Location = new Point(220,140);
            m_pGet.Text = "Get";
            m_pGet.Click += new EventHandler(m_pGet_Click);

            m_pClose = new Button();
            m_pClose.Size = new Size(70,20);
            m_pClose.Location = new Point(300,140);
            m_pClose.Text = "Close";
            m_pClose.Click += new EventHandler(m_pClose_Click);

            this.Controls.Add(mt_Server);
            this.Controls.Add(m_pServer);
            this.Controls.Add(mt_NetType);
            this.Controls.Add(m_pNetType);
            this.Controls.Add(mt_LocalEndPoint);
            this.Controls.Add(m_pLocalEndPoint);
            this.Controls.Add(mt_PublicEndPoint);
            this.Controls.Add(m_pPublicEndPoint);
            this.Controls.Add(m_pGet);
            this.Controls.Add(m_pClose);
        }
                                
        #endregion


        #region Events Handling

        #region method m_pGet_Click

        private void m_pGet_Click(object sender,EventArgs e)
        {
            if (GamePort == 0)
                m_pLocalEndPoint.Text = new IPEndPoint(IPAddress.Any, 0).ToString();

            m_pNetType.Text = "";
            m_pPublicEndPoint.Text = "";

            this.Cursor = Cursors.WaitCursor;
            try{
                if(string.IsNullOrEmpty(m_pServer.Text)){
                    MessageBox.Show(this,"Please specify STUN server !","Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                                
                using(Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp)){
                    if(m_pLocalEndPoint.Text != string.Empty){
                        string[] ip_port = m_pLocalEndPoint.Text.Split(':');
                        socket.Bind(new IPEndPoint(IPAddress.Parse(ip_port[0]),Convert.ToInt32(ip_port[1])));
                    }
                    else{
                        socket.Bind(new IPEndPoint(IPAddress.Any,0));
                    }
                    m_pLocalEndPoint.Text = socket.LocalEndPoint.ToString();

                    string server = m_pServer.Text.Split(':')[0];
                    int port = 3478;

                    try { port = Convert.ToInt32(m_pServer.Text.Split(':')[1]); }
                    catch { }

                    STUN_Result result = STUN_Client.Query(server, port, socket);
                    m_pNetType.Text = result.NetType.ToString();
                    if(result.NetType != STUN_NetType.UdpBlocked){
                        m_pPublicEndPoint.Text = 
                            string.Join(" - ", result.PublicEndPoints.Select(t => t.ToString()).ToArray());
                    }
                    else{
                        m_pPublicEndPoint.Text = "";

                        if (m_pServer.SelectedIndex != 1) 
                        {
                            m_pServer.SelectedIndex = 1;
                            m_pGet.PerformClick();
                        }
                    }
                }
            }
            catch(Exception x){
                if (m_pServer.SelectedIndex != 1)
                {
                    m_pServer.SelectedIndex = 1;
                    m_pGet.PerformClick();
                }
                else
                {
                    MessageBox.Show(
                        this, 
                        "Error: " + x.ToString(), 
                        "Error:", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                }  
            }
            finally{
                this.Cursor = Cursors.Default;
            }

            GamePort = 0;
        }

        #endregion

        #region method m_pClose_Click

        private void m_pClose_Click(object sender,EventArgs e)
        {
            this.Close();
        }

        #endregion

        #endregion

    }
}
