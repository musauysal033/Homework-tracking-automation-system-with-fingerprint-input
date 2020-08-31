using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Student_Tracking
{
    public partial class Form3 : Form
    {
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-2L70CIU\SQLEXPRESS;Initial Catalog=odevtakip;Integrated Security=True");
        public Form3()
        {
            InitializeComponent();
        }
        Form2 frm2 = new Form2();
        bool durum;
        //bu metot texboxtaki  sinif adi verisini veri tabanı ile karsılastırıyor true false deger donduruyor
        void kontrol()
        {
            conn.Open();
            SqlCommand kmt = new SqlCommand("select * from sinif where sinif_adi=@p1", conn);
            kmt.Parameters.AddWithValue("@p1", textBox1.Text);
            SqlDataReader dr = kmt.ExecuteReader();
            if (dr.Read())
            {
                durum = false;
            }
            else
            {
                durum = true;
            }
            conn.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //bu işlem texbox daki yazılı olan degeri veri tabanında var mı kontrol edip 
            //yoksa veri tabanında eklemeye yarıyor  
            ErrorProvider provider = new ErrorProvider();
            if (textBox1.Text == "")
            {
                provider.SetError(textBox1, "Boş Değer Kaydedilemez");      
            }
            else
            {
                kontrol();
                if (durum == true)
                {
                    conn.Open();
                    SqlCommand komut = new SqlCommand("insert into sinif(sinif_adi) values(@p1)", conn);
                    komut.Parameters.AddWithValue("@p1", textBox1.Text);
                    komut.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("sınıf Eklendi");
                }
                else
                {
                    MessageBox.Show("Bu Sınıf Veritabanında Kayıtlı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                //comboboxtaki degerleri sıfırlayıp tekrar sinif adlarınının hepsini getiriyor
                comboBox1.Items.Clear();
                SqlCommand kmt = new SqlCommand();
                kmt.CommandText = "select * from sinif";
                kmt.Connection = conn;
                kmt.CommandType = CommandType.Text;
                SqlDataReader dr;
                conn.Open();
                dr = kmt.ExecuteReader();
                while (dr.Read())
                {
                    comboBox1.Items.Add(dr["sinif_adi"]);
                }
                conn.Close();
            }
            
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            //form acılınca sınıf adını combobox1 e gönderdik
            SqlCommand kmt = new SqlCommand();
            kmt.CommandText = "select * from sinif";
            kmt.Connection = conn;
            kmt.CommandType = CommandType.Text;
            SqlDataReader dr;
            conn.Open();
            dr = kmt.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(dr["sinif_adi"]);
            }
            conn.Close();
            if(comboBox1.Items.Count > 0)
            {
                comboBox1.Text = comboBox1.Items[0].ToString();
            }

           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //burada comboboxtaki secili deger texboxta varsa hata verdirip yoksa güncelleme işlemini gerceklestirdik
            if(comboBox1.SelectedItem.ToString()==textBox2.Text)
            {
                MessageBox.Show("Bu Sınıf Daha Önce Kaydedilmiş");
            }
            else
            {
                
                conn.Open();
                SqlCommand komut = new SqlCommand("UPDATE sinif SET sinif_adi=@a1 where sinif_adi=@a2", conn);
                komut.Parameters.AddWithValue("@a1", textBox2.Text);
                komut.Parameters.AddWithValue("@a2", comboBox1.SelectedItem.ToString());               
                komut.ExecuteNonQuery();
                MessageBox.Show("Sınıf Adı Güncellendi","Bilgi",MessageBoxButtons.OK);
                conn.Close();
                //güncellenen sınıf isimlerini temizleyip listeliyor
                comboBox1.Items.Clear();
                SqlCommand kmt = new SqlCommand();
                kmt.CommandText = "select * from sinif";
                kmt.Connection = conn;
                kmt.CommandType = CommandType.Text;
                SqlDataReader dr;
                conn.Open();
                dr = kmt.ExecuteReader();
                while (dr.Read())
                {
                    comboBox1.Items.Add(dr["sinif_adi"]);
                }

                conn.Close();
            }
            

        }

        private void button3_Click(object sender, EventArgs e)
        {    //seçili olan sınıfın id'sini aldık    
            SqlCommand sinif = new SqlCommand("select sinif_id from sinif where sinif_adi = @s1", conn);
            sinif.Parameters.AddWithValue("@s1", comboBox1.SelectedItem.ToString());
            conn.Open();
            sinif.ExecuteNonQuery();
            SqlDataReader dr6 = sinif.ExecuteReader();
            dr6.Read();
            int sinifid = Convert.ToInt32(dr6["sinif_id"]);
            dr6.Close();
            conn.Close();


            //seçili sınıfa ait ödevleri sildik
            conn.Open();
            SqlCommand odevsil = new SqlCommand("delete odev where sinif_id = @s2", conn);
            odevsil.Parameters.AddWithValue("@s2", sinifid);
            odevsil.ExecuteNonQuery();
            conn.Close();


            //seçili sınıfa ait öğrencileri sildik
            conn.Open();
            SqlCommand ogrencisil = new SqlCommand("delete ogrenci where sinif_id = @s3", conn);
            ogrencisil.Parameters.AddWithValue("@s3", sinifid);
            ogrencisil.ExecuteNonQuery();
            conn.Close();
            //seçili sınıfa ait notları sildik
            conn.Open();
            SqlCommand notsil = new SqlCommand("delete notlar where sinif_id = @s4", conn);
            notsil.Parameters.AddWithValue("@s4", sinifid);
            notsil.ExecuteNonQuery();
            conn.Close();

            //seçili sınıfın kaydını sildik
            conn.Open();
            SqlCommand silKomutu = new SqlCommand("DELETE from sinif where sinif_adi=@sinif_adi", conn);
            silKomutu.Parameters.AddWithValue("@sinif_adi", comboBox1.SelectedItem.ToString());
            silKomutu.ExecuteNonQuery();
            MessageBox.Show("Kayıt Silindi...","Bilgi",MessageBoxButtons.OK);
            conn.Close();
            //burada silme işlemi sonrası combobox verisini güncelledik
            comboBox1.Items.Clear();
            SqlCommand kmt = new SqlCommand();
            kmt.CommandText = "select * from sinif";
            kmt.Connection = conn;
            kmt.CommandType = CommandType.Text;
            SqlDataReader dr;
            conn.Open();
            dr = kmt.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(dr["sinif_adi"]);
            }

            conn.Close();

            comboBox1.Items.Clear();
            SqlCommand kmt2 = new SqlCommand();
            kmt2.CommandText = "select * from sinif";
            kmt2.Connection = conn;
            kmt2.CommandType = CommandType.Text;
            SqlDataReader dr7;
            conn.Open();
            dr7 = kmt.ExecuteReader();
            while (dr7.Read())
            {
                comboBox1.Items.Add(dr7["sinif_adi"]);
            }
            conn.Close();
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.Text = comboBox1.Items[0].ToString();
            }

        }    

        private void yuvarlakButon1_Click(object sender, EventArgs e)
        {
            frm2.Show();
            this.Hide();
            
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            textBox2.Text = "";
        }
    }
}
