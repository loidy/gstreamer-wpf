using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GstreamerWpfApp
{
    public class Hvt1000 : INotifyPropertyChanged
    {
        private BitmapImage _streamImage;
        private string _content;

        public Hvt1000()
        {
            _content = "test";
            _streamImage = new BitmapImage();
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
