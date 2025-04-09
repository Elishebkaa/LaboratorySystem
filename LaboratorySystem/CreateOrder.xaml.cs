using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace LaboratorySystem
{
    /// <summary>
    /// Окно CreateOrder
    /// </summary>
    public partial class CreateOrder : Window
    {
        public ObservableCollection<Service> Services { get; set; }
        public static Entities entities = new Entities();
        List<Users> listUser = entities.Users.Where(x => x.id_role == 1).ToList();


        public CreateOrder()
        {
            InitializeComponent();

            cmbPatient.ItemsSource = listUser;
            var Services = new Entities().Services.ToList();


            List<Service1> list1 = new List<Service1>();
            for (int i = 0; i < Services.Count(); i++)
            {
                list1.Add(new Service1(Services[i].id, Services[i].service, Services[i].price, Services[i].lead_time, Services[i].deviation_from, Services[i].deviation_to, false));
               
            }
            dbServe.ItemsSource = list1;
        }



        private void dbServe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            var entities = new Entities();

                var idUser = listUser.FirstOrDefault(x => x.login == cmbPatient.Text).id;
            Order order = new Order()
            {
               id_user = idUser,
               id_status = 1,
               date_create = DateTime.Now

            };
                entities.Order.Add(order);
                entities.SaveChanges();
                MessageBox.Show("Заказ создан");
                dbServe.IsEnabled = true;
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так");
            }
            
            

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var entities = new Entities();
            CheckBox chk = (CheckBox)(sender);
            var lo = chk.DataContext as Service1;
            int qwe = entities.Order.ToList().Last().id;
            var jj = entities.ServicesForOrder.FirstOrDefault(x => x.id_order == qwe && x.id_service == lo.id);
            if (jj == null)
            {
                ServicesForOrder sfo = new ServicesForOrder();
                sfo.id_order = qwe;
                sfo.id_service = lo.id;
                sfo.id_status = 1;

                entities.ServicesForOrder.Add(sfo);
                entities.SaveChanges();
             
            }
            else
            {
                MessageBox.Show("Данная услуга уже есть в заказе");
            }

        }

        

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)(sender);
            var lo = chk.DataContext as Service1;
            int qwe = entities.Order.ToList().Last().id;

            var jj = entities.ServicesForOrder.FirstOrDefault(x => x.id_order == qwe && x.id_service == lo.id); 
            entities.ServicesForOrder.Remove(jj);
            entities.SaveChanges();
        }
        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CreatePacient_Click(object sender, RoutedEventArgs e)
        {
            RegWindow createPacient = new RegWindow();
            createPacient.Show();
        }

        private void dbServe_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
