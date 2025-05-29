using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace OrderManagementApp
{
    public partial class AddOrderPage : Page
    {
        private const string OrdersFilePath = "orders.json";

        public AddOrderPage()
        {
            InitializeComponent();
            // Populate CustomerList with sample data
            CustomerList.Items.Add("Customer 1");
            CustomerList.Items.Add("Customer 2");
            CustomerList.Items.Add("Customer 3");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the previous page
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void SubmitOrder_Click(object sender, RoutedEventArgs e)
        {
            // Create an Order object from the form inputs
            if (CustomerList.SelectedItem == null || string.IsNullOrWhiteSpace(OrderedItem.Text) || string.IsNullOrWhiteSpace(OrderAddress.Text) || string.IsNullOrWhiteSpace(Amount.Text) || OrderDate.SelectedDate == null || OrderStatus.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields before submitting the order.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var order = new Order
                {
                    CustomerName = CustomerList.SelectedItem.ToString(),
                    OrderAddress = OrderAddress.Text,
                    OrderedItem = OrderedItem.Text,
                    Amount = int.Parse(Amount.Text),
                    OrderStatus = (OrderStatus.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    OrderDate = OrderDate.SelectedDate.Value
                };

                // Save the order to a JSON file
                SaveOrderToJson(order);

                // Display success message
                MessageBox.Show($"Order Created and Saved:\n{order}", "Order Submitted", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear the form after submission
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveOrderToJson(Order order)
        {
            List<Order> orders;

            // Check if the file exists and load existing orders
            if (File.Exists(OrdersFilePath))
            {
                var existingOrdersJson = File.ReadAllText(OrdersFilePath);
                orders = JsonSerializer.Deserialize<List<Order>>(existingOrdersJson) ?? new List<Order>();
            }
            else
            {
                orders = new List<Order>();
            }

            // Add the new order to the list
            orders.Add(order);

            // Serialize the updated list to JSON and save it to the file
            var updatedOrdersJson = JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(OrdersFilePath, updatedOrdersJson);
        }

        private void ClearForm()
        {
            CustomerList.SelectedIndex = -1;
            OrderAddress.Clear();
            OrderedItem.Clear();
            Amount.Clear();
            OrderDate.SelectedDate = null;
            OrderStatus.SelectedIndex = -1;
        }
    }
}
