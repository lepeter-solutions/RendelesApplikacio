using RendelesApplikacio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OrderManagementApp
{
    public partial class MainPage : Page
    {
        private const string OrdersFilePath = "orders.json";

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadOrders(); 
        }

     
        private void LoadOrders()
        {
            OrdersList.Children.Clear();

            if (!File.Exists(OrdersFilePath))
            {
                var noOrdersMessage = new TextBlock
                {
                    Text = "No orders found. Add some orders to get started!",
                    Margin = new System.Windows.Thickness(5),
                    FontStyle = System.Windows.FontStyles.Italic
                };
                OrdersList.Children.Add(noOrdersMessage);
                return;
            }

            try
            {
                var ordersJson = File.ReadAllText(OrdersFilePath);
                var orders = JsonSerializer.Deserialize<List<Order>>(ordersJson);

                if (orders == null || orders.Count == 0)
                {
                    var noOrdersMessage = new TextBlock
                    {
                        Text = "No orders found. Add some orders to get started!",
                        Margin = new System.Windows.Thickness(5),
                        FontStyle = System.Windows.FontStyles.Italic
                    };
                    OrdersList.Children.Add(noOrdersMessage);
                    return;
                }

                foreach (var order in orders)
                {
                    var orderBorder = new Border
                    {
                        BorderBrush = System.Windows.Media.Brushes.Gray,
                        BorderThickness = new System.Windows.Thickness(1),
                        CornerRadius = new System.Windows.CornerRadius(5),
                        Margin = new System.Windows.Thickness(5),
                        Padding = new System.Windows.Thickness(10),
                        Background = System.Windows.Media.Brushes.LightGray
                    };

                    var orderPanel = new StackPanel();

                    var customerNameText = new TextBlock
                    {
                        Text = $"Customer: {order.CustomerName}",
                        FontWeight = System.Windows.FontWeights.Bold,
                        FontSize = 14
                    };
                    orderPanel.Children.Add(customerNameText);

                    var orderedItemText = new TextBlock
                    {
                        Text = $"Item: {order.OrderedItem}",
                        FontSize = 12
                    };
                    orderPanel.Children.Add(orderedItemText);

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

                    OrdersList.Children.Add(orderBorder);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = new TextBlock
                {
                    Text = $"Error loading orders: {ex.Message}",
                    Margin = new System.Windows.Thickness(5),
                    Foreground = System.Windows.Media.Brushes.Red
                };
                OrdersList.Children.Add(errorMessage);
            }
        }

        private void AddOrders_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Navigate to the AddOrderPage
            NavigationService.Navigate(new AddOrderPage());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageOrdersPage());

        }
    }
}
