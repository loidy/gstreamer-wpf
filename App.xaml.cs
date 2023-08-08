using Gst;
using System.Windows;

namespace GstreamerWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        private void Main(object sender, StartupEventArgs e)
        {
            // Initialize GStreamer
            Gst.Application.Init();

            // Build the pipeline
            var pipeline = Parse.Launch("udpsrc port=50008 ! application/x-rtp,payload=96,encoding-name=H264 ! rtph264depay ! avdec_h264 ! queue ! videorate ! videoconvert ! autovideosink");

            // Start playing
            pipeline.SetState(State.Playing);

            // Wait until error or EOS
            var bus = pipeline.Bus;
            var msg = bus.TimedPopFiltered(Constants.CLOCK_TIME_NONE, MessageType.Eos | MessageType.Error);

            // Free resources
            pipeline.SetState(State.Null);
        }

    }
}
