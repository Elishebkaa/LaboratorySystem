using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mail_LIB;
using System.Security.Cryptography;

namespace LaboratorySystem
{

    public partial class CreatePersonalWindow : Window
    {
        Entities entities = new Entities();
        bool Check = false;

        public CreatePersonalWindow()
        {
            InitializeComponent();
            role.ItemsSource = entities.Role.ToList();
        }

        private void CheckPol()
        {
            Validator validator = new Validator();

            var check = validator.CheckLogin(txtLogin.Text);
            if (!check.isValid)
            {
                MessageBox.Show(check.message);
                return;
            }
            string password = chkShowPassword.IsChecked == true ? txtPasswordVisible.Text : txtPassword.Password;
            check = validator.CheckPassword(password);
            if (!check.isValid)
            {
                MessageBox.Show(check.message);

                return;
            }
            check = validator.CheckMail(txtEmail.Text);
            if (!check.isValid)
            {
                MessageBox.Show(check.message);
                return;
            }
            if (!Regex.IsMatch(txtPhoneNumber.Text, @"^7\d{10}$"))
            {
                MessageBox.Show("Телефон не подходит");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Имя не может быть пустым.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtSurName.Text))
            {
                MessageBox.Show("Фамилия не может быть пустой.");
                return;
            }
            if (txtBirthday.SelectedDate > DateTime.Now || txtBirthday.SelectedDate == null)
            {
                MessageBox.Show("Дата рождения не может быть пустой.");
                return;
            }
            if (string.IsNullOrEmpty(txtPasport.Text) || txtPasport.Text.Length != 10)
            {
                MessageBox.Show("Паспорт должен содержать ровно 10 символов.");
                return;
            }
        }

        private void chkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPassword.Password = txtPasswordVisible.Text;
            txtPasswordVisible.Visibility = Visibility.Collapsed;
            txtPassword.Visibility = Visibility.Visible;
        }

        private void chkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            txtPasswordVisible.Text = txtPassword.Password;
            txtPassword.Visibility = Visibility.Collapsed;
            txtPasswordVisible.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckPol();
            AddUsers();
            if (Check == true) MessageBox.Show("Сотрудник добавлен");
            Close();
        }

        /// <summary>
        /// Хэширование пароля
        /// </summary>
        private void AddUsers()
        {


            var pass = txtPassword.Password;
            byte[] salt;
            byte[] buffer2;
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(pass, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            pass = Convert.ToBase64String(dst);
            try
            {
                Users users = new Users
                {
                    id_role = 1,
                    login = txtLogin.Text,
                    password = txtPassword.Password,
                    email = txtEmail.Text,
                    phone_number = Convert.ToDouble(txtPhoneNumber.Text),
                    name = txtName.Text,
                    surname = txtSurName.Text,
                    birthday = txtBirthday.SelectedDate,
                    pasport = Convert.ToDouble(txtPasport.Text),
                };
                entities.Users.Add(users);
                entities.SaveChanges();
                Check = true;
            }
            catch
            {
                MessageBox.Show("Данные заполнены неверно!");
            }

        }
    }
}
