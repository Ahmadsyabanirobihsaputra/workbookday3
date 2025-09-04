using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MovingObjectClient
{
    public partial class ClientForm : Form
    {
        private PictureBox box;
        private TcpClient client;
        private Thread receiveThread;

        public ClientForm()
        {
            this.Text = "CLIENT";
            this.Width = 400;
            this.Height = 400;

            box = new PictureBox();
            box.BackColor = Color.Blue;
            box.Size = new Size(30, 30);
            box.Location = new Point(50, 50);
            this.Controls.Add(box);

            ConnectToServer();

            // aktifkan input keyboard
            this.KeyPreview = true;
            this.KeyDown += ClientForm_KeyDown;
        }

        private void ConnectToServer()
        {
            try
            {
                // ubah ke IP server jika beda komputer
                client = new TcpClient("127.0.0.1", 5000);
                receiveThread = new Thread(ReceiveData);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect to server: " + ex.Message);
            }
        }

        private void ReceiveData()
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    if (bytes > 0)
                    {
                        string msg = Encoding.UTF8.GetString(buffer, 0, bytes);

                        string[] parts = msg.Split(',');
                        if (parts.Length == 2 &&
                            int.TryParse(parts[0], out int x) &&
                            int.TryParse(parts[1], out int y))
                        {
                            this.Invoke((MethodInvoker)delegate {
                                box.Location = new Point(x, y);
                            });
                        }
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        private void ClientForm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string cmd = "";
                if (e.KeyCode == Keys.Up) cmd = "UP";
                else if (e.KeyCode == Keys.Down) cmd = "DOWN";
                else if (e.KeyCode == Keys.Left) cmd = "LEFT";
                else if (e.KeyCode == Keys.Right) cmd = "RIGHT";

                if (cmd != "")
                {
                    byte[] data = Encoding.UTF8.GetBytes(cmd);
                    client.GetStream().Write(data, 0, data.Length);
                }
            }
            catch { }
        }
    }
}
