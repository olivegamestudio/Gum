using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PerformanceMeasurementPlugin.ViewModels
{
    public class PerformanceViewModel : INotifyPropertyChanged
    {
        DispatcherTimer mTimer;

        SystemManagers systemManagers;

        public int DrawCallCount
        {
            get
            {
                return 0;
                //if(systemManagers == null)
                //{
                //    systemManagers = SystemManagers.Default;
                //}
                //if (systemManagers != null)
                //{
                //    // to do: need to replace this with proper calls
                //    return -1;
                //}
                //else
                //{
                //    return 0;
                //}
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PerformanceViewModel()
        {
            mTimer = new DispatcherTimer();
            mTimer.Tick += HandleTick;
            mTimer.Interval = TimeSpan.FromSeconds(1);

            mTimer.Start();
        }

        private void HandleTick(object sender, EventArgs e)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("DrawCallCount"));
            }
        }


    }
}
