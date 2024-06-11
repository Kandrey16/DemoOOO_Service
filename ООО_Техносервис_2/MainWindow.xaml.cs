using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ООО_Техносервис_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OrderFill();
            OrderStatus_Select_Fill();
            OrderClient_Select_Fill();
        }
        private void dgOrder_Loaded(object sender, RoutedEventArgs e)
        {
            OrderFill();
        }

        public void OrderFill()
        {
            Action action = () =>
            {
                DataBaseClass dataBaseClass = new DataBaseClass();
                string query = @"
                select o.ID_ORDER,
                o.DATE_ORDER,
                o.NAME_EQUIPMENT,
                o.TYPE_PROBLEM,
                o.DEACRIPTION_PROBLEM,
                u.LOGIN_USER,
                os.NAME_ORDER_STATUS
                from [dbo].[Order] o
                    inner join [dbo].[USER] u ON o.CLIENT_ID = u.ID_USER
                    inner join [dbo].[ORDER_STATUS] os ON o.STATUS_ID = os.ID_ORDER_STATUS";

                dataBaseClass.sqlExequte(query, DataBaseClass.act.select);
                dataBaseClass.dependency.OnChange += Dependency_OnChange_Order;
                dgOrder.ItemsSource = dataBaseClass.dataTable.DefaultView;
                dgOrder.Columns[0].Header = "Номер";
                dgOrder.Columns[1].Header = "Дата";
                dgOrder.Columns[2].Header = "Оборудование";
                dgOrder.Columns[3].Header = "Проблема";
                dgOrder.Columns[4].Header = "Описание";
                dgOrder.Columns[5].Header = "Клиент";
                dgOrder.Columns[6].Header = "Статус";
            };
            Dispatcher.Invoke(action);
        }

        public void Dependency_OnChange_Order(object sender, SqlNotificationEventArgs e)
        {
            if (e.Info != SqlNotificationInfo.Invalid)
                OrderFill();
        }

        public void OrderClient_Select_Fill()
        {
            Action action = () =>
            {
                DataBaseClass dataBaseClass = new DataBaseClass();
                string query = "select * from [dbo].[USER] where [ROLE_ID] = 2";

                dataBaseClass.sqlExequte(query, DataBaseClass.act.select);
                dataBaseClass.dependency.OnChange += Dependency_OnChange_Order;
                cbClient.ItemsSource = dataBaseClass.dataTable.DefaultView;
                cbClient.SelectedValuePath = dataBaseClass.dataTable.Columns[0].ColumnName;
                cbClient.DisplayMemberPath = dataBaseClass.dataTable.Columns[1].ColumnName;
            };
            Dispatcher.Invoke(action);
        }

        public void OrderStatus_Select_Fill()
        {
            Action action = () =>
            {
                DataBaseClass dataBaseClass = new DataBaseClass();
                string query = "select * from [dbo].[ORDER_STATUS]";

                dataBaseClass.sqlExequte(query, DataBaseClass.act.select);
                dataBaseClass.dependency.OnChange += Dependency_OnChange_Order;
                cbStatus.ItemsSource = dataBaseClass.dataTable.DefaultView;
                cbStatus.SelectedValuePath = dataBaseClass.dataTable.Columns[0].ColumnName;
                cbStatus.DisplayMemberPath = dataBaseClass.dataTable.Columns[1].ColumnName;
            };
            Dispatcher.Invoke(action);
        }

        private void dgOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dgOrder.SelectedItems.Count != 0 && dgOrder.Items.Count != 0)
            {
                DataRowView dataRowView = (DataRowView)dgOrder.SelectedItems[0];
                tbxNameEquipment.Text = dataRowView[2].ToString();
                tbxDescriptionProblem.Text = dataRowView[3].ToString();
                tbxTypeProblem.Text = dataRowView[4].ToString();
                cbClient.Text = dataRowView[5].ToString();
                cbStatus.Text = dataRowView[6].ToString();

            }
        }

        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataBaseClass dataBaseClass = new DataBaseClass();
                string query = string.Format("insert into [dbo].[Order] ([NAME_EQUIPMENT],[TYPE_PROBLEM],[DEACRIPTION_PROBLEM],[CLIENT_ID],[STATUS_ID]) values ('{0}','{1}','{2}',{3},{4})", tbxNameEquipment.Text, tbxTypeProblem.Text, tbxDescriptionProblem.Text, cbClient.SelectedValue, cbStatus.SelectedValue);
                dataBaseClass.sqlExequte(query, DataBaseClass.act.manipulation);

                OrderFill();

            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataBaseClass dataBaseClass = new DataBaseClass();
                DataRowView dataRow = (DataRowView)dgOrder.SelectedItems[0];
                int id_order = Convert.ToInt32(dataRow["ID_ORDER"]);

                string query = string.Format("update [NAME_EQUIPMENT] = '{0}', [TYPE_PROBLEM] = '{1}', [DEACRIPTION_PROBLEM] = '{2}', [CLIENT_ID] = {3}, [STATUS_ID] = {4} WHERE [ID_ORDER] = {5}", tbxNameEquipment.Text, tbxTypeProblem.Text, tbxDescriptionProblem.Text, cbClient.SelectedValue, cbStatus.SelectedValue, id_order);
                dataBaseClass.sqlExequte(query, DataBaseClass.act.manipulation);

                OrderFill();
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelOrder_Click(object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show("Вы действительно хотите удалить данную запись?", DataBaseClass.APP_NAME, MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    if(dgOrder.Items.Count != 0 && dgOrder.SelectedItems.Count != 0)
                    {
                        DataBaseClass dataBaseClass = new DataBaseClass();
                        DataRowView dataRow = (DataRowView)dgOrder.SelectedItems[0];
                        string query = string.Format("delete from [dbo].[ORDER] where [ID_ORDER] = {0}", dataRow[0]);
                        dataBaseClass.sqlExequte(query, DataBaseClass.act.manipulation);
                        OrderFill();
                    }
                    break;
            }
        }

        
    }
}
