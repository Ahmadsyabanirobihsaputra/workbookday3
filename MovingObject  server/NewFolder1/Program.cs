using System;
using System.Windows.Forms;
using System.Threading;

namespace MovingObjectClient
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            int jumlahClient = 3; // ubah angka ini sesuai kebutuhan

            for (int i = 0; i < jumlahClient; i++)
            {
                Thread t = new Thread(() =>
                {
                    Application.Run(new ClientForm());
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
        }
    }
}
