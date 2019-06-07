using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using InTheHand.Net.Sockets;
using GraduationProject.Models;

namespace GraduationProject.ViewModels
{
    public class TestMeasurementViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<BluetoothDeviceInfo> _devices;
        public ObservableCollection<BluetoothDeviceInfo> Devices
        {
            get => _devices;
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        private ObservableCollection<DataModel> _measurements;
        public ObservableCollection<DataModel> Measurements
        {
            get => _measurements;
            set
            {
                _measurements = value;
                OnPropertyChanged("Measurements");
            }
        }

        private BluetoothDeviceInfo _bluetoothDeviceInfo;
        public BluetoothDeviceInfo BluetoothDeviceInfo
        {
            get => _bluetoothDeviceInfo;
            set
            {
                _bluetoothDeviceInfo = value;
                OnPropertyChanged("BluetoothDeviceInfo");
            }
        }

        private BluetoothDeviceInfo _forkDeviceInfo;
        public BluetoothDeviceInfo ForkDeviceInfo
        {
            get => _forkDeviceInfo;
            set
            {
                _forkDeviceInfo = value;
                OnPropertyChanged("ForkDeviceInfo");
            }
        }

        private MeasureValueModel _selectMeasure;
        public MeasureValueModel SelectMeasure
        {
            get => _selectMeasure;
            set
            {
                _selectMeasure = value;
                OnPropertyChanged("SelectMeasure");
            }
        }

        private ObservableCollection<MeasureValueModel> _measureValues;
        public ObservableCollection<MeasureValueModel> MeasureValues
        {
            get => _measureValues;
            set
            {
                _measureValues = value;
                OnPropertyChanged("MeasureValues");

            }
        }

        private double? _sigma;
        public double? Sigma
        {
            get => _sigma;
            set
            {
                _sigma = value;
                OnPropertyChanged("Sigma");
            }
        }

        private double _heightLevelEyes;
        public double HeightLevelEyes
        {
            get => _heightLevelEyes;
            set
            {
                _heightLevelEyes = value;
                OnPropertyChanged("HeightLevelEyes");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public TestMeasurementViewModel()
        {
            CurrentContext.UpdateDevices();
            Devices = new ObservableCollection<BluetoothDeviceInfo>(CurrentContext.Devices);
            Measurements = new ObservableCollection<DataModel>(CurrentContext.DataList);
            MeasureValues = new ObservableCollection<MeasureValueModel>(CurrentContext.MeasureValues);
        }
    }
}