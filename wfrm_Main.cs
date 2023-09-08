using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

using LumiSoft.Net.STUN.Client;
using Microsoft.Win32;

namespace Warcraft_II_Port_Test
{
    /// <summary>
    /// Application main window.
    /// </summary>
    public class wfrm_Main : Form
    {
        private Label         mt_Server         = null;
        private TextBox       m_pServer         = null;        
        private Label         mt_LocalEndPoint  = null;
        private TextBox       m_pLocalEndPoint  = null;
        private NumericUpDown m_pPort           = null;
        private Label         mt_NetType        = null;
        private TextBox       m_pNetType        = null;
        private Label         mt_PublicEndPoint = null;
        private TextBox       m_pPublicEndPoint = null;
        private Button        m_pGet            = null;
        private Button        m_pClose          = null;

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
            this.ClientSize = new Size(380,155);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = "Warcraft II Port Test";

            mt_Server = new Label();
            mt_Server.Size = new Size(100,20);
            mt_Server.Location = new Point(0,10);
            mt_Server.TextAlign = ContentAlignment.MiddleRight;
            mt_Server.Text = "STUN server:";

            m_pServer = new TextBox();
            m_pServer.Size = new Size(200,20);
            m_pServer.Location = new Point(105,10);
            m_pServer.Text = "stun.1und1.de";

            m_pPort = new NumericUpDown();
            m_pPort.Size = new Size(60,20);
            m_pPort.Location = new Point(310,10);
            m_pPort.Minimum = 1;
            m_pPort.Maximum = 99999;
            m_pPort.Value = 3478;

            mt_LocalEndPoint = new Label();
            mt_LocalEndPoint.Size = new Size(100,20);
            mt_LocalEndPoint.Location = new Point(0,35);
            mt_LocalEndPoint.TextAlign = ContentAlignment.MiddleRight;
            mt_LocalEndPoint.Text = "Local end point:";

            int port = 6112;

            try 
            { 
                port = (int)Registry.GetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Battle.net\\Configuration", 
                    "Game Data Port", 
                    "6112"); 
            }
            catch {  }

            m_pLocalEndPoint = new TextBox();
            m_pLocalEndPoint.Size = new Size(265,20);
            m_pLocalEndPoint.Location = new Point(105,35);
            m_pLocalEndPoint.Text = new IPEndPoint(IPAddress.Any, port).ToString();

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
            mt_PublicEndPoint.Text = "Public end point:";

            m_pPublicEndPoint = new TextBox();
            m_pPublicEndPoint.Size = new Size(265,20);
            m_pPublicEndPoint.Location = new Point(105,85);
            m_pPublicEndPoint.ReadOnly = true;

            m_pGet = new Button();
            m_pGet.Size = new Size(70,20);
            m_pGet.Location = new Point(220,120);
            m_pGet.Text = "Get";
            m_pGet.Click += new EventHandler(m_pGet_Click);

            m_pClose = new Button();
            m_pClose.Size = new Size(70,20);
            m_pClose.Location = new Point(300,120);
            m_pClose.Text = "Close";
            m_pClose.Click += new EventHandler(m_pClose_Click);

            this.Controls.Add(mt_Server);
            this.Controls.Add(m_pPort);
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

                    STUN_Result result = STUN_Client.Query(m_pServer.Text,3478,socket);
                    m_pNetType.Text = result.NetType.ToString();
                    if(result.NetType != STUN_NetType.UdpBlocked){
                        m_pPublicEndPoint.Text = result.PublicEndPoints[0].ToString();
                    }
                    else{
                        m_pPublicEndPoint.Text = "";
                    }
                }
            }
            catch(Exception x){
                MessageBox.Show(this,"Error: " + x.ToString(),"Error:",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally{
                this.Cursor = Cursors.Default;
            }
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
