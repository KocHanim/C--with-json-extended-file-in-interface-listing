using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using PagedList;
using System.Linq;

namespace Task
{
    public partial class Form1 : Form
    {
        private List<user> allUsers; // Tüm kullanıcıları içeren liste
        private List<user> displayedUsers; // Gösterilen kullanıcıları içeren liste
        private int pageNumber = 1; // Mevcut sayfa numarası
        private int pageSize = 10; // Sayfa boyutu

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string jsonFilePath = "C:/Users/benru/Desktop/Task/file.json"; //dosya yolu alma(dosya yolunu değiştirmeyi unutmayın)
            string json = File.ReadAllText(jsonFilePath); //dosya okuma
            allUsers = JsonConvert.DeserializeObject<List<user>>(json); //nesne dönüşümü

            UpdateDisplayedUsers();// İlk sayfayı göstermek için kullanıcıları güncelle
        }
        private void UpdatePageNumberLabel()
        {
            PageNumbers.Text = "Sayfa: " + pageNumber.ToString(); //hangi sayfada olduğumuzu göster
        }
        private void UpdateDisplayedUsers()
        {
            // Gösterilecek kullanıcıları belirle
            int startIndex = (pageNumber - 1) * pageSize;
            displayedUsers = allUsers.Skip(startIndex).Take(pageSize).ToList();

            // DataGridView'e güncellenmiş verileri atama
            dataGridView1.DataSource = new BindingSource { DataSource = displayedUsers };
            dataGridView1.Refresh();
 
            UpdatePageNumberLabel();// Sayfa numarasını güncelle
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            string searchText = SearchBox.Text; // TextBox'tan arama metnini alma
            string searchKeyword = searchText.Substring(0, 1).ToUpper() + searchText.Substring(1);

            // Arama sonuçlarını saklamak için yeni bir liste oluşturma
            List<user> searchResults = allUsers.Where(u =>
                u.JobTitle.IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                u.EmailAddress.IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                u.FirstNameLastName.IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToList();// Büyük küçüh hassasiyeti ortadan kaldırmak için case-insensitive yapıyoruz.

            // Tüm kullanıcıları dolaşarak arama yapma
            foreach (user currentUser in allUsers)
            {
                // Kullanıcının bütün özelliğini dolaşarak arama metnini kontrol ediyoruz
                foreach (var property in typeof(user).GetProperties())
                {
                    if (property.GetValue(currentUser) is string propertyValue && propertyValue.Contains(searchText))
                    {
                        // Eşleşme bulunduğunda kullanıcıyı arama sonuçlarına ekleyin
                        searchResults.Add(currentUser);
                        break; // Döngüden çık.(çünkü diğer özellikleri kontrol etmeye gerek yok)
                    }
                    else if (property.GetValue(currentUser) is int intValue && intValue.ToString().Contains(searchText))//int değeri olması İD ye int değeri verdik
                    {
                        // Eşleşme bulunduğunda kullanıcıyı arama sonuçlarına ekleyin
                        searchResults.Add(currentUser);
                        break; // Döngüden çık.(çünkü diğer özellikleri kontrol etmeye gerek yok)
                    }
                }
            }

            // Arama sonuçlarını DataGridView'e veya başka bir listeleyiciye atayarak kullanıcıya gösterin
            dataGridView1.DataSource = new BindingSource { DataSource = searchResults };
            dataGridView1.Refresh();
        }
        private void next_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)allUsers.Count / pageSize); //Toplam sayfa sayısını hesaplıyoruz

            if (pageNumber < totalPages) // Son sayfada değilsek ileri gidebiliriz
            {
                pageNumber++; // Sonraki sayfaya geçiş

                UpdateDisplayedUsers();// Güncellenmiş kullanıcıları göster
            }
        }
        private void previos_Click(object sender, EventArgs e)
        {
            if (pageNumber > 1) // İlk sayfada değilsek geriye gidebiliriz
            {
                pageNumber--; // Önceki sayfaya geçiş

                UpdateDisplayedUsers();// Güncellenmiş kullanıcıları göster
            }
        }
    }
}
