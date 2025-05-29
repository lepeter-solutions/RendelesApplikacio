using OrderManagementApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RendelesApplikacio
{
    public partial class ManageOrdersPage : Page
    {
        private const string OrdersFilePath = "orders.json";

        public ManageOrdersPage()
        {
            InitializeComponent();
            this.Loaded += ManageOrdersPage_Loaded;
        }
        private void ManageOrdersPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void RefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void DeleteAllOrders_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all orders?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (File.Exists(OrdersFilePath))
                {
                    File.Delete(OrdersFilePath);
                }
                LoadOrders();
            }
        }

        private void LoadOrders()
        {
            OrdersList.Items.Clear();

            if (!File.Exists(OrdersFilePath))
            {
                OrdersList.Items.Add(new TextBlock
                {
                    Text = "No orders found.",
                    FontStyle = FontStyles.Italic,
                    Margin = new Thickness(5)
                });
                return;
            }

            try
            {
                var ordersJson = File.ReadAllText(OrdersFilePath);
                var orders = JsonSerializer.Deserialize<List<Order>>(ordersJson);

                if (orders == null || orders.Count == 0)
                {
                    OrdersList.Items.Add(new TextBlock
                    {
                        Text = "No orders found.",
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(5)
                    });
                    return;
                }

                foreach (var order in orders)
                {
                    var orderBorder = new Border
                    {
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(5),
                        Margin = new Thickness(5),
                        Padding = new Thickness(10),
                        Background = Brushes.LightGray,
                        Tag = order 
                    };

                    var orderPanel = new StackPanel();

                    var customerNameText = new TextBlock
                    {
                        Text = $"Customer: {order.CustomerName}",
                        FontWeight = FontWeights.Bold,
                        FontSize = 14
                    };
                    orderPanel.Children.Add(customerNameText);

                    var orderedItemText = new TextBlock
                    {
                        Text = $"Item: {order.OrderedItem}",
                        FontSize = 12
                    };
                    orderPanel.Children.Add(orderedItemText);

                    var orderAddressText = new TextBlock
                    {
                        Text = $"Address: {order.OrderAddress}",
                        FontSize = 12
                    };
                    orderPanel.Children.Add(orderAddressText);

                    var orderAmountText = new TextBlock
                    {
                        Text = $"Amount: {order.Amount}",
                        FontSize = 12
                    };
                    orderPanel.Children.Add(orderAmountText);

                    var orderDateText = new TextBlock
                    {
                        Text = $"Date: {order.OrderDate:yyyy-MM-dd}",
                        FontSize = 12
                    };
                    orderPanel.Children.Add(orderDateText);

                    var orderStatusText = new TextBlock
                    {
                        Text = $"Status: {order.OrderStatus}",
                        FontSize = 12,
                        Foreground = order.OrderStatus switch
                        {
                            "Pending" => Brushes.Orange,
                            "In Progress" => Brushes.Blue,
                            "Delivered" => Brushes.Green,
                            "Canceled" => Brushes.Red,
                            _ => Brushes.Black 
                        }
                    };
                    orderPanel.Children.Add(orderStatusText);

                    orderBorder.Child = orderPanel;

                    OrdersList.Items.Add(orderBorder);
                }
            }
            catch (Exception ex)
            {
                OrdersList.Items.Add(new TextBlock
                {
                    Text = $"Error loading orders: {ex.Message}",
                    Foreground = Brushes.Red,
                    Margin = new Thickness(5)
                });
            }
        }

        private void SetStatusToCanceled_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrderStatus("Canceled");
        }

        private void SetStatusToInProgress_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrderStatus("In Progress");
        }

        private void SetStatusToDelivered_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrderStatus("Delivered");
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersList.SelectedItem is Border selectedBorder && selectedBorder.Tag is Order selectedOrder)
            {
                if (MessageBox.Show("Are you sure you want to delete this order?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DeleteOrder(selectedOrder);
                    LoadOrders();
                }
            }
            else
            {
                MessageBox.Show("Please select an order to delete.", "No Order Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateOrderStatus(string newStatus)
        {
            if (OrdersList.SelectedItem is Border selectedBorder && selectedBorder.Tag is Order selectedOrder)
            {
                selectedOrder.OrderStatus = newStatus;

                SaveOrdersToFile(selectedOrder);

                LoadOrders();
            }
            else
            {
                MessageBox.Show("Please select an order to update.", "No Order Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SaveOrdersToFile(Order updatedOrder)
        {
            if (File.Exists(OrdersFilePath))
            {
                var ordersJson = File.ReadAllText(OrdersFilePath);
                var orders = JsonSerializer.Deserialize<List<Order>>(ordersJson) ?? new List<Order>();

                for (int i = 0; i < orders.Count; i++)
                {
                    if (orders[i].CustomerName == updatedOrder.CustomerName &&
                        orders[i].OrderedItem == updatedOrder.OrderedItem &&
                        orders[i].OrderDate == updatedOrder.OrderDate)
                    {
                        orders[i] = updatedOrder;
                        break;
                    }
                }

                var updatedOrdersJson = JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(OrdersFilePath, updatedOrdersJson);
            }
        }


        private void DeleteOrder(Order orderToDelete)
        {
            if (File.Exists(OrdersFilePath))
            {
                var ordersJson = File.ReadAllText(OrdersFilePath);
                var orders = JsonSerializer.Deserialize<List<Order>>(ordersJson) ?? new List<Order>();

                orders.RemoveAll(o => o.CustomerName == orderToDelete.CustomerName &&
                                      o.OrderedItem == orderToDelete.OrderedItem &&
                                      o.OrderDate == orderToDelete.OrderDate);

                var updatedOrdersJson = JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(OrdersFilePath, updatedOrdersJson);
            }
        }

        private void OrdersList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (OrdersList.SelectedItem is Border selectedBorder && selectedBorder.Tag is Order selectedOrder)
            {
                NavigationService.Navigate(new OrderEditorPage(selectedOrder));
            }
            else
            {
                MessageBox.Show("Please select a valid order to edit.", "No Order Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



    }
}
