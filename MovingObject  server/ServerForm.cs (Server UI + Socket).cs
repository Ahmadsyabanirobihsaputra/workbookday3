using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MovingObjectServer
{
    public partial class ServerForm : Form
    {
        private TcpListener server;
        private Thread serverThread;
        private PictureBox box;
        private static List<TcpClient> clients = new List<TcpClient>();

        private System.Windows.Forms.Timer moveTimer;
        private int dx = 5; // kecepatan horizontal
        private int dy = 5; // kecepatan vertikal
        private Random rand = new Random();

        public ServerForm()
        {
            this.Text = "SERVER";
            this.Width = 400;
            this.Height = 400;

            // Ganti warna background form 
            this.BackColor = Color.LightBlue;

            box = new PictureBox();
            box.BackColor = Color.Blue;
            box.Size = new Size(30, 30);
            box.Location = new Point(150, 150);
            this.Controls.Add(box);

          

        // Timer untuk gerakan otomatis
        moveTimer = new System.Windows.Forms.Timer();
            moveTimer.Interval = 50; 
            moveTimer.Tick += MoveTimer_Tick;
            moveTimer.Start();

            // Jalankan server socket
            serverThread = new Thread(StartServer);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            Point p = box.Location;
            p.X += dx;
            p.Y += dy;

            // memantul jika menabrak batas horizontal
            if (p.X <= 0 || p.X + box.Width >= this.ClientSize.Width)
            {
                dx = -dx;
                p.X += dx;
            }

            // memantul jika menabrak batas vertikal
            if (p.Y <= 0 || p.Y + box.Height >= this.ClientSize.Height)
            {
                dy = -dy;
                p.Y += dy;
            }

            // Sesekali ubah arah secara random (misal 5% kemungkinan setiap tick)
            if (rand.Next(100) < 5)
            {
                dx = rand.Next(-5, 6); // nilai antara -5 dan 5
                dy = rand.Next(-5, 6);
                if (dx == 0 && dy == 0) dx = 3; // hindari diam total
            }

            box.Location = p;

            // Broadcast posisi baru ke semua client
            BroadcastPosition(p.X, p.Y);
        }

        private void StartServer()
        {
            server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                lock (clients)
                {
                    clients.Add(client);
                }
                Thread t = new Thread(() => HandleClient(client));
                t.IsBackground = true;
                t.Start();
            }
        }

        private void HandleClient(TcpClient client)
        {
          
            var stream = client.GetStream();
            byte[] buffer = new byte[1024];
            try
            {
                while (stream.Read(buffer, 0, buffer.Length) > 0) { }
            }
            catch { }
            lock (clients) clients.Remove(client);
            client.Close();
        }

        private void BroadcastPosition(int x, int y)
        {
            byte[] data = Encoding.UTF8.GetBytes($"{x},{y}");
            lock (clients)
            {
                foreach (var client in clients)
                {
                    try { client.GetStream().Write(data, 0, data.Length); }
                    catch { }
                }
            }
        }
    }
}
