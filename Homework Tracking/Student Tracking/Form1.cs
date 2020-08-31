using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
namespace Student_Tracking
{
    public partial class Form1 : Form
    {
        Socket listener;
       static Form self;
       static byte[] buffer= new byte[1];
        public Form1()
        {
            InitializeComponent();
            self = this;
        }
        Form2 frm2 = new Form2();
        private void Form1_Load(object sender, EventArgs e)
        {
            //bir tane server olusturduk 3000 portundan veri gönder dedik herhangi gir ip addresine bak dedik
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 3000);
            listener = new Socket(localEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEP);
            listener.Listen(10);//toplam dinleyici sayisını veriyoruz 
            listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);
          
        }
        public static void AcceptCallback(IAsyncResult ar)
        {
            // İstemci isteğini yapan soketi aldık. 
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar); 
            
            
            handler.BeginReceive(buffer, 0, buffer.Length, 0,
                new AsyncCallback(ReadCallback), handler);
        }
        public static void ReadCallback(IAsyncResult ar)
        {
            String mesaj = String.Empty;

      
            Socket handler = (Socket)ar.AsyncState;

            // İstemci soketindeki verileri okuduk.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)//okunan bayt deger sıfırdan büyük ise  
            {
                //gelen mesajı asci türüne ceviriyoruz 
                mesaj = Encoding.ASCII.GetString(
                   buffer, 0, bytesRead);
                //form2 yi ac self=this 
                self.Invoke(new MethodInvoker(delegate
                {
                    new Form2().Show();
                    self.Hide();
                }));
                
                handler.BeginReceive(buffer, 0, buffer.Length, 0,
                    new AsyncCallback(ReadCallback), handler);
               
            }
            else
            {
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frm2.Show();
            this.Hide();
        }
    }
}
