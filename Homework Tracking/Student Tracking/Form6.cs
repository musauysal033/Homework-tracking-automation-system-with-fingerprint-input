using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Student_Tracking
{
    public partial class Form6 : Form
    {
        
        public Form6()
        {
            InitializeComponent();
        }
        Form2 frm2 = new Form2();
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-2L70CIU\SQLEXPRESS;Initial Catalog=odevtakip;Integrated Security=True");
        private void yuvarlakButon1_Click(object sender, EventArgs e)
        {
            frm2.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //burada butona basarak bütün tablodaki verileri datagride cektik dataset sana tablo olusturmamız için gerekliydi
            conn.Open();
            SqlDataAdapter da = new SqlDataAdapter("select sinif.sinif_adi,ogrenci.ogrenci_adsoyad,veli_adsoyad,veli_tel,odev.odev_adi,odev_tip,odev_baslama,odev_bitis,notlar.notu from sinif   inner join ogrenci on (sinif.sinif_id=ogrenci.sinif_id)  inner join odev on ogrenci.ogrenci_no=odev.ogrenci_no  inner join notlar on (odev.ogrenci_no=notlar.ogrenci_no)", conn);
            DataSet ds = new DataSet();
            da.Fill(ds,"veriler");
            dataGridView1.DataSource = ds.Tables["veriler"];
            conn.Close();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            
        }
    }
}
