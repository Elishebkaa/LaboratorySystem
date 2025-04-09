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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LaboratorySystem
{
    /// <summary>
    /// EditServices код для редактирования
    /// </summary>
    
    
public class Service
{
    public int Id { get; set; }
    public string Name { get; set; }
}
public partial class EditServices : Window
    {
        
        public EditServices()
        {
            InitializeComponent();
            var Services = new Entities().Services.ToList();

           
            List<Service1> list1 = new List<Service1>();
            for (int i = 0; i < Services.Count(); i++)
            {
                list1.Add(new Service1(Services[i].id, Services[i].service, Services[i].price, Services[i].lead_time, Services[i].deviation_from, Services[i].deviation_to, false));
                for (int j = 0; j<LabAssistantWindow.list.Count(); j++)
                {
                    if (LabAssistantWindow.list[j].id_service == Services[i].id)
                    {
                        list1[i].tr = true;
                    }
                }
            }
            dbServe.ItemsSource = list1;
        }
        

     



        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var entities = new Entities();
            
            CheckBox chk = (CheckBox)(sender);
            var lo = chk.DataContext as Service1;
            int qwe = LabAssistantWindow.list[0].id_order;
            var jj = entities.ServicesForOrder.FirstOrDefault(x => x.id_order == qwe && x.id_service == lo.id);
            if (jj==null) {
                ServicesForOrder sfo = new ServicesForOrder();
                sfo.id_order = LabAssistantWindow.list[0].id_order;
                sfo.id_service = lo.id;
                sfo.id_status = 1;

                entities.ServicesForOrder.Add(sfo);
                entities.SaveChanges();
            }
          
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var entities = new Entities();

            CheckBox chk = (CheckBox)(sender);
            var lo = chk.DataContext as Service1;
            int qwe = LabAssistantWindow.list[0].id_order;

            var jj = entities.ServicesForOrder.FirstOrDefault(x => x.id_order == qwe && x.id_service == lo.id);
            entities.ServicesForOrder.Remove(jj);
            entities.SaveChanges();
        
        }
    }

}
