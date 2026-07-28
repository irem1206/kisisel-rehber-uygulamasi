using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace odev.rehber
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("rehber.xml"))
            {
                XmlTextWriter dosya = new XmlTextWriter("rehber.xml", Encoding.UTF8);
                dosya.Formatting = Formatting.Indented;
                dosya.WriteStartDocument();
                dosya.WriteStartElement("Kisiler");
                dosya.WriteEndElement();
                dosya.Close();
            }


            KayitlariGetir();
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtad.Text = row.Cells["Ad"]?.Value?.ToString() ?? "";
                txtsoyad.Text = row.Cells["soyad"]?.Value?.ToString() ?? "";
                txttelefon.Text = row.Cells["telefon"]?.Value?.ToString() ?? "";
                txteposta.Text = row.Cells["eposta"]?.Value?.ToString() ?? "";

            }
        }
        private void KayitlariGetir()
        {
            if (File.Exists("rehber.xml"))
            {
                DataSet dset = new DataSet();
                XmlReader reader = XmlReader.Create("rehber.xml");
                dset.ReadXml(reader);
                reader.Close();

                if (dset.Tables.Count > 0)
                {
                    dataGridView1.DataSource = dset.Tables[0];

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["eposta"].Value != null)
                        {
                            string eposta = row.Cells["eposta"].Value.ToString();
                            string adSoyad = row.Cells["Ad"].Value?.ToString() + " " + row.Cells["soyad"].Value?.ToString();
                            row.Cells["eposta"].Value = eposta;
                        }
                    }
                }
                else
                {
                }
            }
        }

        private void btnekle_Click(object sender, EventArgs e)
        {
            string ad = txtad.Text.Trim();
            string soyad = txtsoyad.Text.Trim();
            string telefon = txttelefon.Text.Trim();
            string eposta = txteposta.Text.Trim();

            if (string.IsNullOrWhiteSpace(ad))
            {
                MessageBox.Show("Ad  boş olamaz."); return;
            }

            if (!Regex.IsMatch(telefon, @"^05\d{9}$"))

            {
                MessageBox.Show("Telefon hatalı. 05xxxxxxxxx formatında giriniz."); return;
            }
            if (!Regex.IsMatch(eposta, @"^[\w\.-]+@[\w\.-]+\.\w+$"))
            {
                MessageBox.Show("Geçerli bir e-posta adresi giriniz.");
                return;
            }


            XDocument xdoc = XDocument.Load("rehber.xml");
            bool kisiVarMi = xdoc.Root.Elements("Kisi")
        .Any(k =>
            (string)k.Element("Ad") == ad &&
            (string)k.Element("soyad") == soyad
        );

            if (kisiVarMi)
            {
                MessageBox.Show("Bu isimle bir kişi zaten var!");
                return;
            }
            XElement yeniKisi = new XElement("Kisi",
                new XElement("Ad", ad),
                new XElement("soyad", soyad),
                new XElement("telefon", telefon),
                new XElement("eposta", eposta)
            );
            xdoc.Root.Add(yeniKisi);
            xdoc.Save("rehber.xml");

            MessageBox.Show("Kayıt eklendi.");

            txteposta.Clear();

            KayitlariGetir();
            txtad.Clear();
            txtsoyad.Clear();
            txttelefon.Clear();
            txteposta.Clear();


            bool kisiVarni = xdoc.Root.Elements("Kisi")
                .Any(k =>
                    (string)k.Element("Ad") == txtad.Text.Trim() &&
                    (string)k.Element("soyad") == txtsoyad.Text.Trim()
                );

            if (kisiVarMi)
            {
                MessageBox.Show("Bu isimle bir kişi zaten var!");
                return;
            }
        }

        private void btnsil_Click(object sender, EventArgs e)
        {
            XDocument xdoc = XDocument.Load("rehber.xml");

            string ad = txtad.Text.Trim().ToLower();
            XElement kisi = xdoc.Root.Elements("Kisi")
                .FirstOrDefault(k => ((string)k.Element("Ad"))?.ToLower() == ad);


            if (kisi != null)
            {
                kisi.Remove();
                xdoc.Save("rehber.xml");
                MessageBox.Show("Kayıt silindi.");
                KayitlariGetir();
            }
            else
            {
                MessageBox.Show("Silinecek kişi bulunamadı.");
            }
        }

        private void btnguncelle_Click(object sender, EventArgs e)
        {
            XDocument xdoc = XDocument.Load("rehber.xml");

            XElement kisi = xdoc.Root.Elements("Kisi")
                .FirstOrDefault(k => (string)k.Element("Ad") == txtad.Text);

            if (kisi != null)
            {
                kisi.Remove();
                XElement yeniKisi = new XElement("Kisi",
                    new XElement("Ad", txtad.Text),
                    new XElement("soyad", txtsoyad.Text),
                    new XElement("telefon", txttelefon.Text),
                    new XElement("eposta", txteposta.Text)
                );

                xdoc.Root.Add(yeniKisi);
                xdoc.Save("rehber.xml");
                KayitlariGetir();

                MessageBox.Show("Kayıt güncellendi.");
            }
            else
            {
                MessageBox.Show("Güncellenecek kişi bulunamadı.");

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}