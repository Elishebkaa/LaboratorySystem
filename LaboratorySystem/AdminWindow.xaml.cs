using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Entity;
using System.Net;

namespace LaboratorySystem
{
    public partial class AdminWindow : Window
    {
        private int userId;
        private string userLogin;
        private string userName;
        private string userSurName;

        public AdminWindow(int userId, string login, string name, string surname)
        {
            InitializeComponent();
            img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/images/admin.png"));
            this.userId = userId;
            this.userLogin = login;
            this.userName = name;
            this.userSurName = surname;
            tbUserName.Text = name + " " + surname;
            LoadUsers();
            LoadLoginHistory();
        }
        /// <summary>
        /// Вывод пользователей в датагрид
        /// </summary>
        private void LoadUsers()
        {
            try
            {
                using (var db = new Entities())
                {
                    dgUsers.ItemsSource = db.Users.Select(u => new 
                        {
                            u.id,
                            u.login,
                            u.name,
                            Role = u.Role,
                            Lastenter = u.HistoryIn.OrderByDescending(h => h.lastenter).FirstOrDefault()
                        }).ToList();
                }
                tbStatus.Text = "Список пользователей загружен";
            }
            catch
            {
                MessageBox.Show($"Ошибка загрузки пользователей");
            }
        }

        private void LoadLoginHistory(string filterLogin = null)
        {
            try
            {
                using (var db = new Entities())
                {
                    var query = db.HistoryIn
                        .Include(h => h.Users)
                        .Include(h => h.Reasons)
                        .OrderByDescending(h => h.lastenter)
                        .AsQueryable();

                  ///<summary>
                  ///Фильтр
                  ///</summary>
                    if (!string.IsNullOrWhiteSpace(filterLogin))
                    {
                        query = query.Where(h => h.Users.login.Contains(filterLogin));
                    }

                    dgLoginHistory.ItemsSource = query.ToList();
                }
                tbStatus.Text = string.IsNullOrWhiteSpace(filterLogin)
                    ? "История входов загружена"
                    : $"Отфильтровано по логину: {filterLogin}";
            }
            catch
            {
                MessageBox.Show($"Ошибка загрузки истории входов");
            }
        }

        private void BtnRefreshUsers_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            CreatePersonalWindow createPersonalWindow = new CreatePersonalWindow();
            createPersonalWindow.Show();
        }
            

        


        private void BtnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadLoginHistory(txtFilterLogin.Text.Trim());
        }

        //очистка фильтра
        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            txtFilterLogin.Clear();
            LoadLoginHistory();
        }

        //кнопка выхода
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            AddToHistory(userId);
            new MainWindow().Show();
            Close();
        }

        private void AddToHistory(int userId)
        {
            try
            {
                using (var db = new Entities())
                {
                    var history = new HistoryIn
                    {
                        id_user = userId,
                        lastenter = DateTime.Now,
                        ip = GetLocalIPAddress(),
                        id_Reason = 3
                    };

                    db.HistoryIn.Add(history);
                    db.SaveChanges();
                }
            }
            catch { }
        }

        private string GetLocalIPAddress()
        {
            string ip;
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                ip = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            }
            else ip = "10.11.93.111";
            return ip;
        }

        private void dgLoginHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dgLoginHistory_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
