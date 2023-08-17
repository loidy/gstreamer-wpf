using Gst;
using Gst.App;
using Gst.Video;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GstreamerWpfApp
{
    public class Hvt1000 : INotifyPropertyChanged
    {
        private BitmapImage _streamImage;
        private string _content;

        public bool IsLive { get; private set; }
        private Pipeline Pipeline;
        private AppSink VideoSink;

        public Hvt1000()
        {
            _content = "Capture";
            _streamImage = new BitmapImage();
        }

        public void CreatePipeline()
        {
            Pipeline = new Pipeline("hvt");

            var pipeline = Parse.Launch("udpsrc port=50008 ! application/x-rtp,payload=96,encoding-name=H264 ! rtph264depay ! avdec_h264 ! queue ! videorate ! videoconvert ! avenc_bmp ! appsink name=videoSink");
            Pipeline.Add(pipeline);

            var videoSinkElement = (pipeline as Gst.Pipeline).GetChildByName("videoSink");
            VideoSink = videoSinkElement as AppSink;

            VideoSink.Drop = true; // drop frames if cannot keep up
            VideoSink.Sync = true; // synchronized playback 
            VideoSink.MaxLateness = (1000 / 30) * 1000000; // maximum latency to achieve at least 30 fps
            VideoSink.MaxBuffers = 1; // no buffering for video sink
            VideoSink.Qos = true; // QoS for video sink 
            VideoSink.EnableLastSample = false; // no need for last sample as we are pulling samples 

            //Pipeline.Add(VideoSink);

            var ret = Pipeline.SetState(State.Ready);
        }

        public void Stop()
        {
            IsLive = false;

            if (Pipeline != null)
            {
                Pipeline.SetState(State.Null);
            }
            // StreamImage = null;
        }

        public bool Play()
        {
            //Stop();
            if (Pipeline == null)
            {
                return false;
            }
            var ret = Pipeline.SetState(State.Playing);

            if (ret == StateChangeReturn.Failure)
            {
                Trace.WriteLine("Unable to set the pipeline to the playing state.");
                Console.WriteLine("Unable to set the pipeline to the playing state.");
            }
            else if (ret == StateChangeReturn.NoPreroll)
            {
                IsLive = true;
                Trace.WriteLine("Playing a live stream.");
                Console.WriteLine("Playing a live stream.");
                ret = StateChangeReturn.Success;
            }

            return ret == StateChangeReturn.Success;
        }

        public void RenderImage()
        {
            var sink = VideoSink;

            if (sink == null)
            {
                return;
            }

            Sample sample = sink.TryPullSample(5);

            if (sample == null)
            {
                return;
            }

            Caps caps = sample.Caps;
            var cap = caps[0];

            string format;
            int width = 0;
            int height = 0;
            int fpsNumerator = 0;
            int fpsDenominator = 1;

            format = cap.GetString("format");
            cap.GetInt("width", out width);
            cap.GetInt("height", out height);
            cap.GetFraction("framerate", out fpsNumerator, out fpsDenominator);

            using (var buffer = sample.Buffer)
            {
                MapInfo map;
                if (buffer.Map(out map, MapFlags.Read))
                {
                    using (var stream = new MemoryStream(map.Data))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                        Trace.WriteLine(image);
                        StreamImage = image;
                        Content = "Captured";
                    }
                    buffer.Unmap(map);
                }
            }
            sample.Dispose();
        }

        public BitmapImage StreamImage
        {
            get => _streamImage;
            set
            {
                if (_streamImage != value)
                {
                    _streamImage = value;
                    OnPropertyChanged("StreamImage");
                }
            }
        }

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

    }
}
