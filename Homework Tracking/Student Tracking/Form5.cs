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
    public partial class Form5 : Form
    {
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-2L70CIU\SQLEXPRESS;Initial Catalog=odevtakip;Integrated Security=True");
        public Form5()
        {
            InitializeComponent();
        }
        Form2 frm2 = new Form2();
        bool durum;
        void kontrol()
        {

            conn.Open();
            SqlCommand kmt = new SqlCommand("select * from odev where odev_adi=@p1", conn);
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

        private void Form5_Load(object sender, EventArgs e)
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
                comboBox4.Items.Add(dr["sinif_adi"]);
                comboBox1.Items.Add(dr["sinif_adi"]);
            }

            dr.Close();
            conn.Close();
            //combobox4 nesnelerine 1. sıradaki veriyi seçili olarak getirir. Null değer döndürmemesi için önlem alınır.            
            if (comboBox4.Items.Count > 0||comboBox2.Items.Count>0)
            {
                comboBox4.Text = comboBox4.Items[0].ToString();
            }
            //bu  kod parcacıgı ögrenci tablosundan ögrenci ad soyad verisini comboxa getirir
            SqlCommand kmt2 = new SqlCommand();
            comboBox2.Items.Clear();
            kmt2.CommandText = "select * from ogrenci";
            kmt2.Connection = conn;
            kmt2.CommandType = CommandType.Text;
            SqlDataReader dr2;
            conn.Open();
            dr2 = kmt2.ExecuteReader();
            while (dr2.Read())
            {
                comboBox5.Items.Add(dr2["ogrenci_adsoyad"]);
                comboBox9.Items.Add(dr2["ogrenci_adsoyad"]);
                comboBox11.Items.Add(dr2["ogrenci_adsoyad"]);

            }

            dr2.Close();
            conn.Close();
            if (comboBox5.Items.Count > 0||comboBox9.Items.Count>0||comboBox11.Items.Count>0)
            {
                comboBox5.Text = comboBox5.Items[0].ToString();
                comboBox9.Text = comboBox9.Items[0].ToString();
                comboBox11.Text = comboBox11.Items[0].ToString();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ErrorProvider provider = new ErrorProvider();
            //veritabanında not null özelliği taşıyan alanlarımız bulunduğundan dolayı boş değer girilmesini önler.
            if (textBox1.Text == "" || comboBox3.SelectedItem.ToString() == "")
            {
                MessageBox.Show("Lütfen Ödev Bilgilerini Tam Giriniz.");
                provider.SetError(textBox1, "boş değer");
            }
            else
            {
                kontrol();
                if (durum == true)
                {
                    //combobox1 deki seçili olan sinifin sinif tablosundan sinif_id verisini cekip değişkene attım
                    conn.Open();
                    SqlCommand kmt1 = new SqlCommand("select sinif_id from ogrenci where ogrenci_adsoyad = @s1", conn);
                    kmt1.Parameters.AddWithValue("@s1", comboBox2.SelectedItem.ToString());
                    kmt1.ExecuteNonQuery();
                    SqlDataReader dr1 = kmt1.ExecuteReader();
                    dr1.Read();
                    int sinifid = Convert.ToInt32( dr1["sinif_id"]);
                    conn.Close();
                    //
                    conn.Open();
                    SqlCommand kmt2 = new SqlCommand("select ogrenci_no from ogrenci where ogrenci_adsoyad = @s2", conn);
                    kmt2.Parameters.AddWithValue("@s2", comboBox2.SelectedItem.ToString());
                    kmt2.ExecuteNonQuery();
                    SqlDataReader dr2 = kmt2.ExecuteReader();
                    dr2.Read();
                    int ogrencino = Convert.ToInt32(dr2["ogrenci_no"]);
                    conn.Close();



                    //ogrenci tablosuna sinifid ile beraber ekleme yaptım 
                    conn.Open();
                    SqlCommand komut = new SqlCommand("insert into odev (sinif_id,ogrenci_no,odev_adi,odev_tip,odev_baslama,odev_bitis) values (@p1,@p2,@p3,@p4,@p5,@p6)", conn);
                    komut.Parameters.AddWithValue("@p1", sinifid);
                    komut.Parameters.AddWithValue("@p2", ogrencino);
                    komut.Parameters.AddWithValue("@p3", textBox1.Text);
                    komut.Parameters.AddWithValue("@p4", comboBox3.SelectedItem.ToString());
                    komut.Parameters.AddWithValue("@p5", dateTimePicker1.Value);
                    komut.Parameters.AddWithValue("@p6", dateTimePicker2.Value);
                    komut.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("kayıt eklendi");
                }
                else
                {
                    MessageBox.Show("Bu Kayıt Zaten Var!!!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                //ekleme işlemindne sonra texboxları bosaltıp hata olmaması için comboboxtaki 0 indexteki sınıfı textine attım
                textBox1.Text = "";
                comboBox3.Text = "";
            }
        }

        private void yuvarlakButon1_Click(object sender, EventArgs e)
        {
            frm2.Show();
            this.Hide();
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.Text = "";
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            //bu kod blogu combobox4 de secilen  sinif adi id'sine göre combobox5'e o sınıfa ait ogrenci verisini getirir
            conn.Open();
            SqlCommand kmt1 = new SqlCommand("select sinif_id from sinif where sinif_adi = @s1", conn);
            kmt1.Parameters.AddWithValue("@s1", comboBox4.SelectedItem.ToString());
            kmt1.ExecuteNonQuery();
            SqlDataReader dr1 = kmt1.ExecuteReader();
            dr1.Read();
            int c = Convert.ToInt32(dr1["sinif_id"]);
            conn.Close();


            comboBox5.Text = "";
            comboBox5.Items.Clear();
            SqlCommand kmt = new SqlCommand("select ogrenci_adsoyad from ogrenci where sinif_id = @a1", conn);
            kmt.Parameters.AddWithValue("@a1", c);
            SqlDataReader dr;
            conn.Open();
            dr = kmt.ExecuteReader();
            while (dr.Read())
            {
                comboBox5.Items.Add(dr["ogrenci_adsoyad"]);
            }

            conn.Close();
            //burada bos deger girilmesini engellemek için combobox4 teki 0. indexteki ismi combobox4 textine attık 
            //ve yine bos girilmesini engellemek için combobox2 de secilen sınıfı combobox3'e atarak engellemiş olduk
            if (comboBox5.Items.Count > 0)
            {
                comboBox5.Text = comboBox5.Items[0].ToString();
            }
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            textBox2.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //bu satırda ogrenci adınagöre odev tablosuna ekleyeceğimizogrenci numarası verisiogrenci tablosundan cekilmiştir 
            conn.Open();
            SqlCommand kmt2 = new SqlCommand("select ogrenci_no from ogrenci where ogrenci_adsoyad = @s1", conn);
            kmt2.Parameters.AddWithValue("@s1", comboBox5.SelectedItem.ToString());
            kmt2.ExecuteNonQuery();
            SqlDataReader dr2 = kmt2.ExecuteReader();
            dr2.Read();
            string ogrencino = dr2["ogrenci_no"].ToString();
            conn.Close();           
            //parametreli ödev bilgileri günecelleme işlemi yapılmıstır 
            conn.Open();
            SqlCommand komut = new SqlCommand("UPDATE odev SET odev_adi=@a1 where ogrenci_no=@a2 ", conn);
            komut.Parameters.AddWithValue("@a1", textBox2.Text);
            komut.Parameters.AddWithValue("@a2", ogrencino);
            komut.ExecuteNonQuery();
            conn.Close();
            //
            conn.Open();
            SqlCommand komut1 = new SqlCommand("UPDATE odev SET odev_baslama=@a3 where ogrenci_no=@a4 ", conn);
            komut1.Parameters.AddWithValue("@a3", dateTimePicker3.Value);
            komut1.Parameters.AddWithValue("@a4", ogrencino);
            komut1.ExecuteNonQuery();
            conn.Close();
            //
            conn.Open();
            SqlCommand komut2 = new SqlCommand("UPDATE odev SET odev_bitis=@a5 where ogrenci_no=@a6 ", conn);
            komut2.Parameters.AddWithValue("@a5", dateTimePicker4.Value);
            komut2.Parameters.AddWithValue("@a6", ogrencino);
            komut2.ExecuteNonQuery();
            MessageBox.Show("Ödev Bilgileri Güncelleme İşlemi yapılmıstır");
            conn.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SqlCommand sinif = new SqlCommand("select sinif_id from sinif where sinif_adi = @s1", conn);
            sinif.Parameters.AddWithValue("@s1", comboBox4.SelectedItem.ToString());
            conn.Open();
            sinif.ExecuteNonQuery();
            SqlDataReader dr6 = sinif.ExecuteReader();
            dr6.Read();
            int sinifid = Convert.ToInt32(dr6["sinif_id"]);
            dr6.Close();
            conn.Close();
            //secili sinifa ait ödev verisinide sildik//
            conn.Open();
            SqlCommand notsil = new SqlCommand("delete notlar where sinif_id = @s4", conn);
            notsil.Parameters.AddWithValue("@s4", sinifid);
            notsil.ExecuteNonQuery();
            conn.Close();

            //seçili sınıfa ait ödevleri sildik
            conn.Open();
            SqlCommand odevsil = new SqlCommand("delete odev where sinif_id = @s2", conn);
            odevsil.Parameters.AddWithValue("@s2", sinifid);
            MessageBox.Show("Sinifa Ait Ödev Verisi Silinmiştir");
            odevsil.ExecuteNonQuery();
            conn.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SqlCommand ogrenci = new SqlCommand("select ogrenci_no from ogrenci where ogrenci_adsoyad = @s2", conn);
            ogrenci.Parameters.AddWithValue("@s2", comboBox5.SelectedItem.ToString());
            conn.Open();
            ogrenci.ExecuteNonQuery();
            SqlDataReader dr8 = ogrenci.ExecuteReader();
            dr8.Read();
            int ogrencino = Convert.ToInt32(dr8["ogrenci_no"]);
            dr8.Close();
            conn.Close();
            //secili ögrencinin not verisinide sildik ögrenciyi silerken 
            conn.Open();
            SqlCommand notsil = new SqlCommand("delete notlar where ogrenci_no= @s4", conn);
            notsil.Parameters.AddWithValue("@s4", ogrencino);
            notsil.ExecuteNonQuery();
            conn.Close();

            //seçili ogrenciye ait ödevleri sildik
            conn.Open();
            SqlCommand odevsil = new SqlCommand("delete odev where ogrenci_no = @s2", conn);
            odevsil.Parameters.AddWithValue("@s2", ogrencino);
            odevsil.ExecuteNonQuery();
            MessageBox.Show("Öğrenciye Ait Ödev Bilgisi Silinmiştir!!!");
            conn.Close();
        }

   

        private void button4_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand kmt1 = new SqlCommand("select sinif_id from ogrenci where ogrenci_adsoyad = @s1", conn);
            kmt1.Parameters.AddWithValue("@s1", comboBox9.SelectedItem.ToString());
            kmt1.ExecuteNonQuery();
            SqlDataReader dr1 = kmt1.ExecuteReader();
            dr1.Read();
            string sinifid = dr1["sinif_id"].ToString();
            conn.Close();
            //notlar tablosuna eklemek için ogrencino verisini ögrenci ismine göre aldık 
            conn.Open();
            SqlCommand kmt2 = new SqlCommand("select ogrenci_no from ogrenci where ogrenci_adsoyad = @s1", conn);
            kmt2.Parameters.AddWithValue("@s1", comboBox9.SelectedItem.ToString());
            kmt2.ExecuteNonQuery();
            SqlDataReader dr2 = kmt2.ExecuteReader();
            dr2.Read();
            string ogrencino = dr2["ogrenci_no"].ToString();
            conn.Close();
            //notlar tablosuna eklemek için ödevid verisini ögrenci ismine göre aldık 
            conn.Open();
            SqlCommand kmt3 = new SqlCommand("select odev_id from odev where ogrenci_no = @s1", conn);
            kmt3.Parameters.AddWithValue("@s1", ogrencino);
            kmt3.ExecuteNonQuery();
            SqlDataReader dr3 = kmt3.ExecuteReader();
            dr3.Read();
            string odevid = dr3["odev_id"].ToString();
            conn.Close();
            ErrorProvider provider = new ErrorProvider();
            //veritabanında not null özelliği taşıyan alanlarımız bulunduğundan dolayı boş değer girilmesini önler.
            if (textBox6.Text == "")
            {
                MessageBox.Show("Lütfen Not Bilgilerini Tam Giriniz.");
                provider.SetError(textBox1, "boş değer");
            }
            else
            {
                //ogrenci tablosuna sinifid ve ogrencino ile beraber ekleme yaptım 
                conn.Open();
                SqlCommand komut = new SqlCommand("insert into notlar (sinif_id,ogrenci_no,odev_id,notu) values (@p1,@p2,@p3,@p4)", conn);
                komut.Parameters.AddWithValue("@p1", sinifid);
                komut.Parameters.AddWithValue("@p2", ogrencino);
                komut.Parameters.AddWithValue("@p3", odevid);
                komut.Parameters.AddWithValue("@p4", textBox6.Text);
                komut.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("kayıt eklendi");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //bu satırda ogrenci adınagöre odev tablosuna ekleyeceğimizogrenci numarası verisiogrenci tablosundan cekilmiştir 
            conn.Open();
            SqlCommand kmt2 = new SqlCommand("select ogrenci_no from ogrenci where ogrenci_adsoyad = @s1", conn);
            kmt2.Parameters.AddWithValue("@s1", comboBox11.SelectedItem.ToString());
            kmt2.ExecuteNonQuery();
            SqlDataReader dr2 = kmt2.ExecuteReader();
            dr2.Read();
            string ogrencino = dr2["ogrenci_no"].ToString();
            conn.Close();
            //parametreli olarak güncelleme işlemi yapılmıstır 
            conn.Open();
            SqlCommand komut = new SqlCommand("UPDATE notlar SET notu=@a1 where ogrenci_no=@a2 ", conn);
            komut.Parameters.AddWithValue("@a1", textBox7.Text);
            komut.Parameters.AddWithValue("@a2", ogrencino);
            komut.ExecuteNonQuery();
            MessageBox.Show("Seçilen İşlem Güncellendi");
            conn.Close();

        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            //sadece sayi girilmesi gerektiğini belirttik 
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            //sadece sayi girilmesi gerektiğini belirttik 
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand kmt1 = new SqlCommand("select sinif_id from sinif where sinif_adi = @s1", conn);
            kmt1.Parameters.AddWithValue("@s1", comboBox1.SelectedItem.ToString());
            kmt1.ExecuteNonQuery();
            SqlDataReader dr1 = kmt1.ExecuteReader();
            dr1.Read();
            int c = Convert.ToInt32(dr1["sinif_id"]);
            conn.Close();



            comboBox2.Items.Clear();
            SqlCommand kmt = new SqlCommand("select ogrenci_adsoyad from ogrenci where sinif_id = @a1", conn);
            kmt.Parameters.AddWithValue("@a1", c);
            SqlDataReader dr;
            conn.Open();
            dr = kmt.ExecuteReader();
            while (dr.Read())
            {
                comboBox2.Items.Add(dr["ogrenci_adsoyad"]);
            }

            conn.Close();
            //burada bos deger girilmesini engellemek için combobox4 teki 0. indexteki ismi combobox4 textine attık 
            //ve yine bos girilmesini engellemek için combobox2 de secilen sınıfı combobox3'e atarak engellemiş olduk
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.Text = comboBox2.Items[0].ToString();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //bu satırda notunu sileceğimiz ögrencinin adına göre ogrenci nosunu aldık 
            conn.Open();
            SqlCommand kmt2 = new SqlCommand("select ogrenci_no from ogrenci where ogrenci_adsoyad = @s1", conn);
            kmt2.Parameters.AddWithValue("@s1", comboBox11.SelectedItem.ToString());
            kmt2.ExecuteNonQuery();
            SqlDataReader dr2 = kmt2.ExecuteReader();
            dr2.Read();
            string ogrencino = dr2["ogrenci_no"].ToString();
            conn.Close();
            //secili ögrencinin not verisini sildik 
            conn.Open();
            SqlCommand notsil = new SqlCommand("delete notlar where ogrenci_no= @s4", conn);
            notsil.Parameters.AddWithValue("@s4", ogrencino);
            notsil.ExecuteNonQuery();
            conn.Close();
            //MessageBox.Show("Öğrencinin not verisi silinmiştir");
        }
    }
}

