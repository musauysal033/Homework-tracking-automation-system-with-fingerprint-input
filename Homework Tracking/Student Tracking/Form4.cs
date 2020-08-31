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
    public partial class Form4 : Form
    {
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-2L70CIU\SQLEXPRESS;Initial Catalog=odevtakip;Integrated Security=True");
        public Form4()
        {
            InitializeComponent();
        }
        Form2 frm2 = new Form2();
        bool durum;
        //aynı numaraya kayıtlı olan ogrenciyi veri tabanında varmı kontol ettirip asagıda kullanılacak metot tanımladık
        void kontrol()
        {
            
                conn.Open();
                SqlCommand kmt = new SqlCommand("select * from ogrenci where ogrenci_no=@p1", conn);
                kmt.Parameters.AddWithValue("@p1", Convert.ToInt32(textBox2.Text));
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
        
        private void Form4_Load(object sender, EventArgs e)
        {

            //bu işlem veritabanından  sinif isimlerini form  acılınca comboboxlara tasıyor
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
                comboBox2.Items.Add(dr["sinif_adi"]);
                comboBox3.Items.Add(dr["sinif_adi"]);

            }

            dr.Close();
            conn.Close();
            //combobox1,2,3 nesnelerine 1. sıradaki veriyi seçili olarak getirir. Null değer döndürmemesi için önlem alınır.
            if(comboBox1.Items.Count>0&&comboBox2.Items.Count>0&&comboBox3.Items.Count>0)
            {
                comboBox1.Text = comboBox1.Items[0].ToString();
                comboBox2.Text = comboBox2.Items[0].ToString();
                comboBox3.Text = comboBox3.Items[0].ToString();
            }
            
            
            //bu  kod parcacıgı ögrenci tablosundan ögrenci ad soyad verisini comboxa getirir
            comboBox4.Items.Clear();

            SqlCommand sinifidsec = new SqlCommand("select sinif_id from sinif where sinif_adi = @c1", conn);
            sinifidsec.Parameters.AddWithValue("@c1", comboBox2.SelectedItem.ToString());
            conn.Open();
            sinifidsec.ExecuteNonQuery();
            SqlDataReader dr5 = sinifidsec.ExecuteReader();
            dr5.Read();
            int sinifid2 = Convert.ToInt32(dr5["sinif_id"]);
            dr5.Close();
            conn.Close();


            SqlCommand kmt2 = new SqlCommand( "select ogrenci_adsoyad from ogrenci where sinif_id = @c2",conn);
            kmt2.Parameters.AddWithValue("@c2", sinifid2);
            SqlDataReader dr2;
            conn.Open();
            dr2 = kmt2.ExecuteReader();
            while (dr2.Read())
            {
                comboBox4.Items.Add(dr2["ogrenci_adsoyad"]);                
            }

            dr2.Close();
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ErrorProvider provider = new ErrorProvider();
            //veritabanında not null özelliği taşıyan alanlarımız bulunduğundan dolayı boş değer girilmesini önler.
            if (textBox1.Text == "" || textBox2.Text == "" ||  textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("Lütfen Öğrenci Bilgilerini Tam Giriniz.");
                provider.SetError(textBox2,"boş değer");
            }
            else
            {
                kontrol();
                if (durum == true)
                {
                    //combobox1 deki seçili olan sinifin sinif tablosundan sinif_id verisini cekip değişkene attım
                    conn.Open();
                    SqlCommand kmt1 = new SqlCommand("select sinif_id from sinif where sinif_adi = @s1", conn);
                    kmt1.Parameters.AddWithValue("@s1", comboBox1.SelectedItem.ToString());
                    kmt1.ExecuteNonQuery();
                    SqlDataReader dr1 = kmt1.ExecuteReader();
                    dr1.Read();
                    string sinifid = dr1["sinif_id"].ToString();
                    conn.Close();


                    
                    //ogrenci tablosuna sinifid ile beraber ekleme yaptım 
                    conn.Open();
                    SqlCommand komut = new SqlCommand("insert into ogrenci (ogrenci_adsoyad,ogrenci_no,sinif_id,veli_adsoyad,veli_tel) values (@p1,@p2,@p3,@p4,@p5)", conn);
                    komut.Parameters.AddWithValue("@p1", textBox1.Text);
                    komut.Parameters.AddWithValue("@p2", Convert.ToInt64(textBox2.Text));
                    komut.Parameters.AddWithValue("@p3", sinifid);
                    komut.Parameters.AddWithValue("@p4", textBox3.Text);
                    komut.Parameters.AddWithValue("@p5", textBox4.Text);
                    komut.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("kayıt eklendi");
                }
                else
                {
                    MessageBox.Show("Bu Kayıt Zaten Var!!!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                //eklenen sinif adını tekrarsız bi sekilde combobox1'e ekledim
                comboBox1.Items.Clear();
                SqlCommand kmt = new SqlCommand();
                kmt.CommandText = "select distinct sinif_adi from sinif";
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

                //ekleme işlemindne sonra texboxları bosaltıp hata olmaması için comboboxtaki 0 indexteki sınıfı textine attım
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                comboBox1.Text = comboBox1.Items[0].ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ErrorProvider provider = new ErrorProvider();
            //güncelleme sırasında comboboxtaki secili olan deger ile textbox5 teki deger kontrol edilip ayni isimli ogrencinin kayıtı engellendi
            if (comboBox4.SelectedItem.ToString() == textBox5.Text && textBox5.TextLength<0)
            {
                MessageBox.Show("Bu Değer Daha Önce Kayıtlı/Güncel Öğrenci İsmini Girin");
                provider.SetError(textBox5, "boş değer");
            }
            else
            {
                SqlCommand ogrencisecme = new SqlCommand("select ogrenci_no from ogrenci where ogrenci_adsoyad = @p1",conn);
                ogrencisecme.Parameters.AddWithValue("@p1", comboBox4.SelectedItem.ToString());
                conn.Open();
                ogrencisecme.ExecuteNonQuery();
                SqlDataReader dr2 = ogrencisecme.ExecuteReader();
                dr2.Read();
                int ogrenciid = Convert.ToInt32(dr2["ogrenci_no"]);
                dr2.Close();
                conn.Close();
                //
                SqlCommand sinifsecme = new SqlCommand("select sinif_id from sinif where sinif_adi = @p2",conn);
                sinifsecme.Parameters.AddWithValue("@p2", comboBox3.SelectedItem.ToString());
                conn.Open();
                sinifsecme.ExecuteNonQuery();
                SqlDataReader dr3 = sinifsecme.ExecuteReader();
                dr3.Read();
                int sinifid = Convert.ToInt32(dr3["sinif_id"]);
                dr3.Close();
                conn.Close();
                //
                //
                conn.Open();
                SqlCommand komut = new SqlCommand("UPDATE ogrenci SET ogrenci_adsoyad=@a1 where ogrenci_no = @a2", conn);
                komut.Parameters.AddWithValue("@a1", textBox5.Text);
                komut.Parameters.AddWithValue("@a2", ogrenciid);
                komut.ExecuteNonQuery();
                conn.Close();
                //
                conn.Open();
                SqlCommand komut2 = new SqlCommand("UPDATE ogrenci SET sinif_id=@a1 where ogrenci_no = @a2", conn);
                komut2.Parameters.AddWithValue("@a1", sinifid);
                komut2.Parameters.AddWithValue("@a2", ogrenciid);
                komut2.ExecuteNonQuery();
                conn.Close();
                //
                conn.Open();
                SqlCommand komut3 = new SqlCommand("UPDATE ogrenci SET veli_adsoyad=@a1 where ogrenci_no = @a2", conn);
                komut3.Parameters.AddWithValue("@a1", textBox6.Text);
                komut3.Parameters.AddWithValue("@a2", ogrenciid);
                komut3.ExecuteNonQuery();
                conn.Close();
                //
                conn.Open();
                SqlCommand komut4 = new SqlCommand("UPDATE ogrenci SET veli_tel=@a1 where ogrenci_no = @a2", conn);
                komut4.Parameters.AddWithValue("@a1", textBox7.Text);
                komut4.Parameters.AddWithValue("@a2", ogrenciid);
                komut4.ExecuteNonQuery();
                MessageBox.Show("Öğrenci Verileri Güncellenmiştir");
                conn.Close();
                textBox2.Text = null;
                //güncellenen ögrenci isimlerini temizleyip listeliyor
                comboBox1.Items.Clear();
                SqlCommand kmt = new SqlCommand();
                kmt.CommandText = "select * from ogrenci";
                kmt.Connection = conn;
                kmt.CommandType = CommandType.Text;
                SqlDataReader dr;
                conn.Open();
                dr = kmt.ExecuteReader();
                while (dr.Read())
                {
                    comboBox1.Items.Add(dr["ogrenci_adsoyad"]);
                }

                conn.Close();
            }

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SqlCommand ogrencisecme = new SqlCommand("select ogrenci_no from ogrenci where ogrenci_adsoyad = @s1", conn);
            ogrencisecme.Parameters.AddWithValue("@s1", comboBox4.SelectedItem.ToString());
            conn.Open();
            ogrencisecme.ExecuteNonQuery();
            SqlDataReader dr6 = ogrencisecme.ExecuteReader();
            dr6.Read();
            int ogrencino = Convert.ToInt32(dr6["ogrenci_no"]);
            dr6.Close();
            conn.Close();

            //seçili ogrenciye alt tablolardan ait ödevleri sildik
            conn.Open();
            SqlCommand odevsil = new SqlCommand("delete odev where ogrenci_no = @s2", conn);
            odevsil.Parameters.AddWithValue("@s2", ogrencino);
            odevsil.ExecuteNonQuery();
            conn.Close();
            //seçili ogrenciye ait alt tablolardan notları sildik
            conn.Open();
            SqlCommand notsil = new SqlCommand("delete notlar where ogrenci_no = @s4", conn);
            notsil.Parameters.AddWithValue("@s4", ogrencino);
            notsil.ExecuteNonQuery();
            conn.Close();

            conn.Open();
            SqlCommand silKomutu = new SqlCommand("DELETE from ogrenci where ogrenci_no=@a1", conn);
            silKomutu.Parameters.AddWithValue("@a1", ogrencino);
            silKomutu.ExecuteNonQuery();
            MessageBox.Show("Kayıt Silindi...");
            //Silme işlemini gerçekleştirdikten sonra kullanıcıya mesaj verdik.
            conn.Close();
            //
            comboBox4.Items.Clear();
            SqlCommand kmt = new SqlCommand("select * from ogrenci",conn);
            SqlDataReader dr;
            conn.Open();
            dr = kmt.ExecuteReader();
            while (dr.Read())
            {
                comboBox4.Items.Add(dr["ogrenci_adsoyad"]);
            }

            conn.Close();
            comboBox4.Text = "";
        }

        private void textBox5_MouseClick(object sender, MouseEventArgs e)
        {
            textBox5.Text = "";
        }

        private void yuvarlakButon1_Click(object sender, EventArgs e)
        {
            frm2.Show();
            this.Hide();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //bu kod blogu combobox2 de secilen  sinif adi id'sine göre combobox4'e o sınıfa ait ogrenci verisini getirir
            conn.Open();
            SqlCommand kmt1 = new SqlCommand("select sinif_id from sinif where sinif_adi = @s1", conn);
            kmt1.Parameters.AddWithValue("@s1", comboBox2.SelectedItem.ToString());
            kmt1.ExecuteNonQuery();
            SqlDataReader dr1 = kmt1.ExecuteReader();
            dr1.Read();
            int c = Convert.ToInt32(dr1["sinif_id"]);
            conn.Close();



            comboBox4.Items.Clear();
            SqlCommand kmt = new SqlCommand("select ogrenci_adsoyad from ogrenci where sinif_id = @a1",conn);
            kmt.Parameters.AddWithValue("@a1", c);
            SqlDataReader dr;
            conn.Open();
            dr = kmt.ExecuteReader();
            while (dr.Read())
            {
                comboBox4.Items.Add(dr["ogrenci_adsoyad"]);
            }

            conn.Close();
            //burada bos deger girilmesini engellemek için combobox4 teki 0. indexteki ismi combobox4 textine attık 
            //ve yine bos girilmesini engellemek için combobox2 de secilen sınıfı combobox3'e atarak engellemiş olduk
            if(comboBox4.Items.Count>0)
            {
                comboBox4.Text = comboBox4.Items[0].ToString();
            }
            
            comboBox3.Text = comboBox2.SelectedItem.ToString();
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            //textBox4e girilecek karakter sayısını 11 ile sınırladık
            if (textBox1.TextLength == 11)
            {
                e.Handled = true;
            }
            //sadecerakam girişi için
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //textBox4e girilecek karakter sayısını 9 ile sınırladık
            if (textBox1.TextLength == 9)
            {
                e.Handled = true;
            }
            //sadecerakam girişi için
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //sadece harf girilmesini engellemek için
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar)
                 && !char.IsSeparator(e.KeyChar);
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //sadece harf girilmesini sağlamak için
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar)
                 && !char.IsSeparator(e.KeyChar);
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            //sadece harf girilmesini sağlamak için
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar)
                 && !char.IsSeparator(e.KeyChar);
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            //textBox7e girilecek karakter sayısını 11 ile sınırladık
            if (textBox1.TextLength == 11)
            {
                e.Handled = true;
            }
            //sadecerakam girişi için
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox5.Text = comboBox4.SelectedItem.ToString();
        }
    }
}
