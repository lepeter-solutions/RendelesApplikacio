using Microsoft.Win32; 
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
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void SubmitOrder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CustomerName.Text) ||
                string.IsNullOrWhiteSpace(OrderedItem.Text) ||
                string.IsNullOrWhiteSpace(OrderAddress.Text) ||
                string.IsNullOrWhiteSpace(Amount.Text) ||
                OrderDate.SelectedDate == null)
            {
                MessageBox.Show("Please fill in all fields before submitting the order.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var order = new Order
                {
                    CustomerName = CustomerName.Text,
                    OrderAddress = OrderAddress.Text,
                    OrderedItem = OrderedItem.Text,
                    Amount = int.Parse(Amount.Text),
                    OrderStatus = "Pending",
                    OrderDate = OrderDate.SelectedDate.Value
                };

                SaveOrderToJson(order);

                MessageBox.Show($"Order Created and Saved:\n{order}", "Order Submitted", MessageBoxButton.OK, MessageBoxImage.Information);

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

            if (File.Exists(OrdersFilePath))
            {
                var existingOrdersJson = File.ReadAllText(OrdersFilePath);
                orders = JsonSerializer.Deserialize<List<Order>>(existingOrdersJson) ?? new List<Order>();
            }
            else
            {
                orders = new List<Order>();
            }

            orders.Add(order);

            var updatedOrdersJson = JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(OrdersFilePath, updatedOrdersJson);
        }

        private void ClearForm()
        {
            CustomerName.Clear();
            OrderAddress.Clear();
            OrderedItem.Clear();
            Amount.Clear();
            OrderDate.SelectedDate = null;
        }

        private void ExportJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON Files (*.json)|*.json",
                    Title = "Export Orders"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    if (File.Exists(OrdersFilePath))
                    {
                        File.Copy(OrdersFilePath, saveFileDialog.FileName, overwrite: true);
                        MessageBox.Show("Orders exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No orders to export.", "Export Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "JSON Files (*.json)|*.json",
                    Title = "Import Orders"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var importedOrdersJson = File.ReadAllText(openFileDialog.FileName);
                    var importedOrders = JsonSerializer.Deserialize<List<Order>>(importedOrdersJson) ?? new List<Order>();

                    if (File.Exists(OrdersFilePath))
                    {
                        var existingOrdersJson = File.ReadAllText(OrdersFilePath);
                        var existingOrders = JsonSerializer.Deserialize<List<Order>>(existingOrdersJson) ?? new List<Order>();

                        existingOrders.AddRange(importedOrders);

                        var updatedOrdersJson = JsonSerializer.Serialize(existingOrders, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(OrdersFilePath, updatedOrdersJson);
                    }
                    else
                    {
                        File.WriteAllText(OrdersFilePath, importedOrdersJson);
                    }

                    MessageBox.Show("Orders imported successfully.", "Import Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
