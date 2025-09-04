using System;
using System.Windows.Forms;

namespace MovingObjectServer
{
    static class Program
    {
        [STAThread]   // atribut yang dibutuhkan untuk aplikasi Windows Forms
        static void Main()
        {
            // Mengatur style form agar sesuai tema Windows
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Jalankan form utama (ServerForm) sebagai entry point program
            Application.Run(new ServerForm());
        }
    }
}
