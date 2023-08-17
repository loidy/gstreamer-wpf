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

            // Initialize GStreamer
            Gst.Application.Init();
            GtkSharp.GstreamerSharp.ObjectManager.Initialize();

            Camera = new Hvt1000();
            streamImageComponent.DataContext = Camera;
            button.DataContext = Camera;

            Camera.CreatePipeline();
            Camera.Play();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Camera.RenderImage();
        }
    }
}
