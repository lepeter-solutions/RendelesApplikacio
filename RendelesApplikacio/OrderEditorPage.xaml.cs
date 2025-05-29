using OrderManagementApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace RendelesApplikacio
{
    public partial class OrderEditorPage : Page
    {
        private Order _order;

        public OrderEditorPage(Order order)
        {
            InitializeComponent();
            _order = order;

            CustomerNameTextBox.Text = _order.CustomerName;
            OrderedItemTextBox.Text = _order.OrderedItem;
            OrderAddressTextBox.Text = _order.OrderAddress;
            AmountTextBox.Text = _order.Amount.ToString();
            OrderStatusComboBox.SelectedItem = OrderStatusComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == _order.OrderStatus);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return; 
            }

   
            _order.CustomerName = CustomerNameTextBox.Text;
            _order.OrderedItem = OrderedItemTextBox.Text;
            _order.OrderAddress = OrderAddressTextBox.Text;
            _order.Amount = int.Parse(AmountTextBox.Text);
            _order.OrderStatus = (OrderStatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;

            SaveOrderChanges(_order);

            
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private bool ValidateFields()
        {
         
            if (string.IsNullOrWhiteSpace(CustomerNameTextBox.Text))
            {
                MessageBox.Show("Customer Name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

           
            if (string.IsNullOrWhiteSpace(OrderedItemTextBox.Text))
            {
                MessageBox.Show("Ordered Item cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

           
            if (string.IsNullOrWhiteSpace(OrderAddressTextBox.Text))
            {
                MessageBox.Show("Order Address cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

         
            if (!int.TryParse(AmountTextBox.Text, out var amount) || amount <= 0)
            {
                MessageBox.Show("Amount must be a valid positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

          
            if (OrderStatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an Order Status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void SaveOrderChanges(Order updatedOrder)
        {
           
            var ordersJson = File.ReadAllText("orders.json");
            var orders = JsonSerializer.Deserialize<List<Order>>(ordersJson) ?? new List<Order>();

            
            var orderToUpdate = orders.FirstOrDefault(order => order.OrderId == updatedOrder.OrderId);

            if (orderToUpdate != null)
            {
              
                orderToUpdate.CustomerName = updatedOrder.CustomerName;
                orderToUpdate.OrderedItem = updatedOrder.OrderedItem;
                orderToUpdate.OrderAddress = updatedOrder.OrderAddress;
                orderToUpdate.Amount = updatedOrder.Amount;
                orderToUpdate.OrderStatus = updatedOrder.OrderStatus;
            }
            else
            {
                MessageBox.Show("Order not found in the file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

           
            var updatedOrdersJson = JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("orders.json", updatedOrdersJson);

            
            Console.WriteLine("Order changes saved successfully.");
        }
    }
}
