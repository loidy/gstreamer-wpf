using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace GstreamerWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Hvt1000 Camera { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Camera = new Hvt1000();
            streamImageComponent.DataContext = Camera;
            button.DataContext = Camera;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string temp = Random.Shared.NextInt64().ToString();
            Trace.WriteLine(temp);
            Camera.Content = temp;
        }
    }
}
