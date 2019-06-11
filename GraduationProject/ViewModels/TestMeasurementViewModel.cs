using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using InTheHand.Net.Sockets;
using GraduationProject.Models;
using System.Collections.Generic;
using Microsoft.Win32;
using System;

namespace GraduationProject.ViewModels
{
    public class TestMeasurementViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<BluetoothDeviceInfo> _devices;
        public ObservableCollection<BluetoothDeviceInfo> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        private ObservableCollection<DataModel> _measurements;
        public ObservableCollection<DataModel> Measurements
        {
            get { return _measurements; }
            set
            {
                _measurements = value;
                OnPropertyChanged("Measurements");
            }
        }

        private BluetoothDeviceInfo _bluetoothDeviceInfo;
        public BluetoothDeviceInfo BluetoothDeviceInfo
        {
            get { return _bluetoothDeviceInfo; }
            set
            {
                _bluetoothDeviceInfo = value;
                OnPropertyChanged("BluetoothDeviceInfo");
            }
        }

        private BluetoothDeviceInfo _forkDeviceInfo;
        public BluetoothDeviceInfo ForkDeviceInfo
        {
            get { return _forkDeviceInfo; }
            set
            {
                _forkDeviceInfo = value;
                OnPropertyChanged("ForkDeviceInfo");
            }
        }

        private MeasureValueModel _selectMeasure;
        public MeasureValueModel SelectMeasure
        {
            get { return _selectMeasure; }
            set
            {
                _selectMeasure = value;
                OnPropertyChanged("SelectMeasure");
            }
        }

        private ObservableCollection<MeasureValueModel> _measureValues;
        public ObservableCollection<MeasureValueModel> MeasureValues
        {
            get { return _measureValues; }
            set
            {
                _measureValues = value;
                OnPropertyChanged("MeasureValues");

            }
        }

        private ObservableCollection<string> _categoriesOne;
        public ObservableCollection<string> CategoriesOne
        {
            get { return _categoriesOne; }
            set
            {
                _categoriesOne = value;
                OnPropertyChanged("CategoriesOne");

            }
        }

        private ObservableCollection<string> _categoriesTwo;
        public ObservableCollection<string> CategoriesTwo
        {
            get { return _categoriesTwo; }
            set
            {
                _categoriesTwo = value;
                OnPropertyChanged("CategoriesTwo");

            }
        }

        private double? _sigma;
        public double? Sigma
        {
            get { return _sigma; }
            set
            {
                _sigma = value;
                OnPropertyChanged("Sigma");
            }
        }

        private double _heightLevelEyes;
        public double HeightLevelEyes
        {
            get { return _heightLevelEyes; }
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

            var file = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/cat.txt").Split('\n');
            CategoriesOne = new ObservableCollection<string>(file);
        }
    }
}