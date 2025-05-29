using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;

namespace OrderManagementApp
{
    public partial class MainPage : Page
    {
        private const string OrdersFilePath = "orders.json";

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded; // Attach the Loaded event
        }

        private void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadOrders(); // Refresh the orders when the page is loaded
        }

        private void LoadOrders()
        {
            // Clear the existing items in the OrdersList
            OrdersList.Children.Clear();

            // Check if the orders.json file exists
            if (!File.Exists(OrdersFilePath))
            {
                // If no file exists, display a message
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
                // Read and deserialize the orders from the JSON file
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

                // Dynamically add each order to the OrdersList
                foreach (var order in orders)
                {
                    // Create a Border for each order
                    var orderBorder = new Border
                    {
                        BorderBrush = System.Windows.Media.Brushes.Gray,
                        BorderThickness = new System.Windows.Thickness(1),
                        CornerRadius = new System.Windows.CornerRadius(5),
                        Margin = new System.Windows.Thickness(5),
                        Padding = new System.Windows.Thickness(10),
                        Background = System.Windows.Media.Brushes.LightGray
                    };

                    // Create a StackPanel to hold the order details
                    var orderPanel = new StackPanel();

                    // Add the customer's name
                    var customerNameText = new TextBlock
                    {
                        Text = $"Customer: {order.CustomerName}",
                        FontWeight = System.Windows.FontWeights.Bold,
                        FontSize = 14
                    };
                    orderPanel.Children.Add(customerNameText);

                    // Add the ordered item
                    var orderedItemText = new TextBlock
                    {
                        Text = $"Item: {order.OrderedItem}",
                        FontSize = 12
                    };
                    orderPanel.Children.Add(orderedItemText);

                    // Add the order status
                    var orderStatusText = new TextBlock
                    {
                        Text = $"Status: {order.OrderStatus}",
                        FontSize = 12,
                        Foreground = order.OrderStatus == "Pending"
                            ? System.Windows.Media.Brushes.Orange
                            : order.OrderStatus == "Shipped"
                                ? System.Windows.Media.Brushes.Blue
                                : order.OrderStatus == "Delivered"
                                    ? System.Windows.Media.Brushes.Green
                                    : System.Windows.Media.Brushes.Red
                    };
                    orderPanel.Children.Add(orderStatusText);

                    // Add the StackPanel to the Border
                    orderBorder.Child = orderPanel;

                    // Add the Border to the OrdersList
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
    }
}
