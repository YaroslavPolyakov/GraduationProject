﻿using System;
using System.Linq;
using System.Text;
using System.Media;
using System.Windows;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Data;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using GraduationProject.Models;
using Microsoft.Win32;
using System.Collections.Generic;

namespace GraduationProject.Views
{
    public partial class TestMeasurement
    {
        private BluetoothEndPoint LocalEndpoint { get; set; }
        private BluetoothClient BluetoothClient { get; set; }
        private BluetoothClient BluetoothForkClient { get; set; }
        private BluetoothDeviceInfo BtDevice { get; set; }
        private BluetoothDeviceInfo ForkBtDevice { get; set; }
        private NetworkStream Stream { get; set; }
        private NetworkStream ForkStream { get; set; }
        private DispatcherTimer Timer { get; set; }

        private bool _isHasDiameterTwo;
        private bool _isMeasurable;
        private DataModel _dataModel;
        private MeasureValueModel _selectMeasure;

        public TestMeasurement()
        {
            InitializeComponent();
            SetStartupSettings();
        }

        private void Device_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtDevice = (sender as ComboBox)?.SelectedItem as BluetoothDeviceInfo;
            ParseStringToObject("$PLTIT,HV,4,M,235,D,5,D,6,M*7E");
            //if (BtDevice != null)
            //{
            //    if (BluetoothSecurity.PairRequest(BtDevice.DeviceAddress, "1111"))
            //    {
            //        if (BtDevice.Authenticated)
            //        {
            //            ViewModel.BluetoothDeviceInfo = BtDevice;
            //            EllipseDistance.Fill = Brushes.DarkGreen;

            //            BluetoothClient.SetPin("1111");
            //            BluetoothClient.BeginConnect(BtDevice.DeviceAddress, BluetoothService.SerialPort, Connect,
            //                BtDevice);
            //        }
            //        else
            //        {
            //            ViewModel.BluetoothDeviceInfo = null;
            //            MessageBox.Show("Аутентификация не пройдена. Попробуйте еще раз.");
            //        }
            //    }
            //    else
            //    {
            //        ViewModel.BluetoothDeviceInfo = null;
            //        MessageBox.Show("Сопряжение с устройством не установлено.");
            //    }
            //}
        }

        private void Connect(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                for (;;)
                {
                    Stream = BluetoothClient.GetStream();

                    if (Stream.CanRead)
                    {
                        var myReadBuffer = new byte[1024];
                        var myCompleteMessage = "";

                        do
                        {
                            Thread.Sleep(1000);
                            Stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                            myCompleteMessage += Encoding.ASCII.GetString(myReadBuffer).Replace("\0", "");
                        } while (Stream.DataAvailable);

                        Application.Current.Dispatcher.Invoke(
                            new ThreadStart(() => ParseStringToObject(myCompleteMessage)));
                    }
                    else
                    {
                        MessageBox.Show("Не удалось прочитать данные.");
                    }
                }
            }
        }

        private void ParseStringToObject(string message)
        {
            if (!string.IsNullOrWhiteSpace(message) && message != "$")
            {
                SystemSounds.Beep.Play();
                //MessageBox.Show(message);
                var arrayData = message.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                var indexHv = Array.IndexOf(arrayData, "HV") + 1;
                var indexM = Array.IndexOf(arrayData, "M") + 1;
                var indexD = Array.IndexOf(arrayData, "D") + 1;
                var indexHt = Array.IndexOf(arrayData, "HT") + 1;
                var indexD1 = indexD == 0 ? 0 : indexD + 2;
                if (_dataModel == null)
                {
                    _dataModel = new DataModel
                    {
                        Id = Interlocked.Increment(ref CurrentContext.GlobalId)
                    };
                }

                if (CheckMessage(message, arrayData))
                {
                    _dataModel.HorizontalDistance =
                        indexHv == 0 ? null : CurrentContext.ToDoubleParse(arrayData[indexHv]);
                    _dataModel.Azimuth = indexM == 0 ? null : CurrentContext.ToDoubleParse(arrayData[indexM]);
                    _dataModel.Bias = indexD == 0 ? null : CurrentContext.ToDoubleParse(arrayData[indexD]);
                    _dataModel.SlopeDistance = indexD1 == 0 ? null : CurrentContext.ToDoubleParse(arrayData[indexD1]);
                    _dataModel.Height = indexHt == 0 ? null : CurrentContext.ToDoubleParse(arrayData[indexHt]);
                    {
                        if (CurrentContext.DataList.Count == 0 && _selectMeasure.Id == 1)
                        {
                            _dataModel.X = CurrentContext.StartupX;
                            _dataModel.Y = CurrentContext.StartupY;
                        }
                        else if (CurrentContext.DataList.Count == 0)
                        {
                            _dataModel.X = Math.Round(CurrentContext.StartupX +
                                                      (_dataModel.HorizontalDistance.GetValueOrDefault() *
                                                       Math.Cos(_dataModel.Azimuth.GetValueOrDefault() + ViewModel.Sigma.GetValueOrDefault())), 2);


                            _dataModel.Y = Math.Round(CurrentContext.StartupY +
                                                      (_dataModel.HorizontalDistance.GetValueOrDefault() *
                                                       Math.Sin(_dataModel.Azimuth.GetValueOrDefault() + ViewModel.Sigma.GetValueOrDefault())), 2);
                        }
                        else
                        {
                            _dataModel.X = Math.Round(CurrentContext.DataList[CurrentContext.DataList.Count - 1].X +
                                                      (_dataModel.HorizontalDistance.GetValueOrDefault() *
                                                       Math.Cos(_dataModel.Azimuth.GetValueOrDefault() + ViewModel.Sigma.GetValueOrDefault())), 2);


                            _dataModel.Y = Math.Round(CurrentContext.DataList[CurrentContext.DataList.Count - 1].Y +
                                                      (_dataModel.HorizontalDistance.GetValueOrDefault() *
                                                       Math.Sin(_dataModel.Azimuth.GetValueOrDefault() + ViewModel.Sigma.GetValueOrDefault())), 2);
                        }

                        _dataModel.VerticalDistance = _dataModel.Height ?? Math.Round(Math.Sqrt(Math.Pow(_dataModel.SlopeDistance.GetValueOrDefault(), 2) - Math.Pow(_dataModel.HorizontalDistance.GetValueOrDefault(), 2)), 2);
                    }

                    if (_selectMeasure.Id != 1)
                    {
                        //_dataModel.DiameterTwo != 0
                        if (_dataModel.DiameterTwo != null && _isHasDiameterTwo)
                        {
                            ViewModel.Measurements.Add(_dataModel);
                            CurrentContext.DataList.Add(_dataModel);
                            _dataModel = null;
                            return;
                        }

                        if (!_isHasDiameterTwo && _dataModel.Species != null)
                        {
                            ViewModel.Measurements.Add(_dataModel);
                            CurrentContext.DataList.Add(_dataModel);
                            _dataModel = null;
                        }
                    }
                    else
                    {
                        ViewModel.Measurements.Add(_dataModel);
                        CurrentContext.DataList.Add(_dataModel);
                        _dataModel = null;
                    }
                }
            }
        }

        private void Fork_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ForkBtDevice = (sender as ComboBox)?.SelectedItem as BluetoothDeviceInfo;
            ParseForkStringToObject("$PHGF,SPC,2,ABC,*2B\n$PHGF,DIA,M,277,*2A");
            //if (ForkBtDevice != null && BluetoothSecurity.PairRequest(ForkBtDevice.DeviceAddress, "1234"))
            //{
            //    if (ForkBtDevice.Authenticated)
            //    {
            //        ViewModel.ForkDeviceInfo = ForkBtDevice;

            //        if (ViewModel.ForkDeviceInfo != null)
            //        {
            //            EllipseFork.Fill = Brushes.DarkGreen;
            //        }

            //        BluetoothForkClient.SetPin("1234");
            //        BluetoothForkClient.BeginConnect(
            //            ForkBtDevice.DeviceAddress,
            //            BluetoothService.SerialPort,
            //            ConnectToFork,
            //            ForkBtDevice);
            //    }
            //}
        }

        private void ConnectToFork(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                for (;;)
                {
                    ForkStream = BluetoothForkClient.GetStream();

                    if (ForkStream.CanRead)
                    {
                        var myReadBuffer = new byte[1024];
                        var myCompleteMessage = "";

                        while (ForkStream.DataAvailable)
                        {
                            Thread.Sleep(1000);
                            ForkStream.Read(myReadBuffer, 0, myReadBuffer.Length);
                            myCompleteMessage += Encoding.ASCII.GetString(myReadBuffer).Replace("\0", "");
                        }

                        if (!string.IsNullOrWhiteSpace(myCompleteMessage))
                        {
                            Application.Current.Dispatcher.Invoke(
                                new ThreadStart(() => ParseForkStringToObject(myCompleteMessage)));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось прочитать данные.");
                    }
                }
            }
        }

        private void ParseForkStringToObject(string message)
        {
            SystemSounds.Beep.Play();

            var temp = message.Split(new[] {','});
            var species = temp[3];
            var dia = int.Parse(temp[7]);

            if (_dataModel == null)
            {
                _dataModel = new DataModel
                {
                    Id = Interlocked.Increment(ref CurrentContext.GlobalId)
                };
            }

            if (_selectMeasure.Id == 5)
            {
                ViewModel.Measurements.Add(_dataModel);
                CurrentContext.DataList.Add(_dataModel);
                _dataModel = null;
                return;
            }
            if (_dataModel.Azimuth != null || _dataModel.Height != null)
            {
                //было  _dataModel.DiameterOne != 0
                if (_isHasDiameterTwo && _dataModel.DiameterOne != null && _selectMeasure.Id != 1)
                {
                    _dataModel.DiameterTwo = dia / 10;
                    ViewModel.Measurements.Add(_dataModel);
                    CurrentContext.DataList.Add(_dataModel);
                    _dataModel = null;
                }
                else
                {
                    _dataModel.Species = species;
                    _dataModel.DiameterOne = dia / 10;

                    if (!_isHasDiameterTwo)
                    {
                        ViewModel.Measurements.Add(_dataModel);
                        CurrentContext.DataList.Add(_dataModel);
                        _dataModel = null;
                    }
                }
            }
            else
            {
                //_dataModel.DiameterOne != 0
                if (_isHasDiameterTwo && _dataModel.DiameterOne != null && _selectMeasure.Id != 1)
                {
                    _dataModel.DiameterTwo = dia / 10;
                }
                else
                {
                    _dataModel.Species = species;
                    _dataModel.DiameterOne = dia / 10;
                }
            }
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Id,X,Y,HorizontalDistance,VerticalDistance,SlopeDistance,Azimuth,Bias,DiameterOne,DiameterTwo,Species");

            foreach (var item in ViewModel.Measurements)
            {
                stringBuilder.AppendLine(item.ToString());
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "СSV (*.csv)|*.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var sw = new StreamWriter(saveFileDialog.OpenFile(), Encoding.Default))
                {
                    sw.Write(stringBuilder.ToString());
                    sw.Close();
                }
            }
        }

        private void ButtonOpen_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "СSV (*.csv)|*.csv"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                var dataList = File.ReadAllLines(filename)
                    .Skip(1)
                    .Select(x => x.Split(','))
                    .Select(x => new DataModel
                    {
                        Id = x[0] != "" ? int.Parse(x[0]) : 0,
                        X = x[1] != "" ? int.Parse(x[1]) : 0,
                        Y = x[2] != "" ? double.Parse(x[2]) : 0,
                        HorizontalDistance = x[3] != "" ? double.Parse(x[3]) : 0,
                        VerticalDistance = x[4] != "" ? double.Parse(x[4]) : 0,
                        SlopeDistance = x[5] != "" ? double.Parse(x[5]) : 0,
                        Azimuth = x[6] != "" ? double.Parse(x[6]) : 0,
                        Bias = x[7] != "" ? double.Parse(x[7]) : 0,
                        DiameterOne = x[8] != "" ? double.Parse(x[8]) : 0,
                        DiameterTwo = x[9] != "" ? double.Parse(x[9]) : 0,
                        Species = x[10]
                    }).ToList();
            }
            //Доделать
            //foreach (var itemTemplateColumn in _selectMeasure.TemplateColumns)
            //{
            //    var column = new DataGridTextColumn
            //    {
            //        Header = itemTemplateColumn.Name,
            //        FontSize = 20,
            //        Binding = new Binding(itemTemplateColumn.BindingName)
            //    };

            //    DataGrid.Columns.Add(column);
            //}
        }

        private void SetStartupSettings()
        {
            try
            {
                LocalEndpoint = new BluetoothEndPoint(BluetoothAddress.Parse(CurrentContext.GetMacAddress()),
                    BluetoothService.SerialPort);
                BluetoothClient = new BluetoothClient(LocalEndpoint);
                BluetoothForkClient = new BluetoothClient(LocalEndpoint);
                Timer = new DispatcherTimer();
                Timer.Tick += UpdateBluetoothDevices;
                Timer.Interval = new TimeSpan(0, 0, 10);
                Timer.Start();
                ViewModel.Sigma = 0;
                ViewModel.HeightLevelEyes = 0;
                OpenDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Bluetooth не включен.");
                throw;
            }
        }

        private void OpenDialog()
        {
            if (MessageBox.Show("Загрузить последние измерения?", "Title", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //
            }
        }

        private async void UpdateBluetoothDevices(object sender, EventArgs e)
        {
            //await Task.Run(() => CurrentContext.UpdateDevices());
            ViewModel.Devices = new ObservableCollection<BluetoothDeviceInfo>(CurrentContext.Devices);

            if (!IsConnectToDistanceDevice(BtDevice))
            {
                EllipseDistance.Fill = Brushes.Red;
            }

            if (!IsConnectToForkDevice(ForkBtDevice))
            {
                EllipseFork.Fill = Brushes.Red;
            }
        }

        private void ButtonDeleteRow_OnClick(object sender, RoutedEventArgs e)
        {
            var measurement = DataGrid.SelectedItem as DataModel;
            var maxId = CurrentContext.DataList.Max(x => x.Id);

            if (maxId == measurement?.Id)
            {
                CurrentContext.GlobalId = maxId - 1;
            }

            ViewModel.Measurements.Remove(measurement);
            CurrentContext.DataList.RemoveAll(model => model.Id == measurement?.Id);
        }

        private bool IsConnectToDistanceDevice(BluetoothDeviceInfo bluetoothDevice)
        {
            if (bluetoothDevice != null && bluetoothDevice.Authenticated)
            {
                return true;
            }

            ViewModel.BluetoothDeviceInfo = null;
            return false;
        }

        private bool IsConnectToForkDevice(BluetoothDeviceInfo bluetoothDevice)
        {
            if (bluetoothDevice != null && bluetoothDevice.Authenticated)
            {
                return true;
            }

            ViewModel.ForkDeviceInfo = null;

            return false;
        }

        private bool CheckMessage(string message, string[] arrayData)
        {
            return message.Contains("HV") && message.Contains("D") && arrayData.Length >= 10 && _selectMeasure != null && !_isMeasurable && _selectMeasure.Id != 5
                ? true
                : message.Contains("HT") && _selectMeasure != null && _isMeasurable && _selectMeasure.Id != 5 ? true : false;
        }

        private void CheckBoxDiameter_Checked(object sender, RoutedEventArgs e)
        {
            _isHasDiameterTwo = true;
            var column = new DataGridTextColumn
            {
                Header = "Диаметр №2",
                FontSize = 20,
                Binding = new Binding("DiameterTwo")
            };

            if (!DataGrid.Columns.Contains(column))
            {
                DataGrid.Columns.Add(column);
            }
        }

        private void CheckBoxDiameter_Unchecked(object sender, RoutedEventArgs e)
        {
            _isHasDiameterTwo = false;
            var columnForRemove = DataGrid.Columns.FirstOrDefault(x => x.Header?.ToString() == "Диаметр №2");

            if (columnForRemove != null)
            {
                DataGrid.Columns.RemoveAt(columnForRemove.DisplayIndex);
            }
        }

        private void CheckBoxHeight_Checked(object sender, RoutedEventArgs e)
        {
            _isMeasurable = true;
        }

        private void CheckBoxHeight_Unchecked(object sender, RoutedEventArgs e)
        {
            _isMeasurable = false;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid != null)
            {
                if (DataGrid.Items.Count != 0)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("Id,X,Y,HorizontalDistance,VerticalDistance,SlopeDistance,Azimuth,Bias,DiameterOne,DiameterTwo,Species");

                    foreach (var item in ViewModel.Measurements)
                    {
                        stringBuilder.AppendLine(item.ToString());
                    }

                    var saveFileDialog1 = new SaveFileDialog
                    {
                        Filter = "СSV (*.csv)|*.csv"
                    };

                    if (saveFileDialog1.ShowDialog() == true)
                    {
                        using (var sw = new StreamWriter(saveFileDialog1.OpenFile(), Encoding.Default))
                        {
                            sw.Write(stringBuilder.ToString());
                            sw.Close();
                        }

                        CurrentContext.DataList = new List<DataModel>();
                        ViewModel.Measurements = new ObservableCollection<DataModel>();
                        CurrentContext.GlobalId = 0;
                    }
                }

                if (_selectMeasure != null)
                {
                    //удаление предыдущих
                    foreach (var itemColumn in _selectMeasure.TemplateColumns)
                    {
                        var columnForRemove = DataGrid.Columns.FirstOrDefault(x => x.Header?.ToString() == itemColumn.Name);

                        if (columnForRemove != null)
                        {
                            DataGrid.Columns.RemoveAt(columnForRemove.DisplayIndex);
                        }
                    }
                }
                ViewModel.SelectMeasure = (sender as ComboBox)?.SelectedItem as MeasureValueModel;
                _selectMeasure = ViewModel.SelectMeasure;

                if (_selectMeasure?.TemplateColumns != null)
                {
                    foreach (var itemTemplateColumn in _selectMeasure.TemplateColumns)
                    {
                        var column = new DataGridTextColumn
                        {
                            Header = itemTemplateColumn.Name,
                            FontSize = 20,
                            Binding = new Binding(itemTemplateColumn.BindingName)
                        };

                        DataGrid.Columns.Add(column);
                    }

                    if (_selectMeasure.Name == "ГИ")
                    {
                        DiameterDockPanel.Visibility = Visibility.Hidden;
                        HeightDockPanel.Visibility = Visibility.Hidden;
                        DiameterButton.IsChecked = false;
                        HeightButton.IsChecked = false;
                        DiameterButton.IsEnabled = true;
                        HeightButton.IsEnabled = true;
                    }
                    else if (_selectMeasure.Name == "РКП")
                    {
                        DiameterDockPanel.Visibility = Visibility.Visible;
                        HeightDockPanel.Visibility = Visibility.Visible;
                        DiameterButton.IsChecked = false;
                        HeightButton.IsChecked = false;
                        DiameterButton.IsEnabled = true;
                        HeightButton.IsEnabled = true;
                    }
                    else if (_selectMeasure.Name == "ППП")
                    {
                        DiameterDockPanel.Visibility = Visibility.Visible;
                        HeightDockPanel.Visibility = Visibility.Visible;
                        DiameterButton.IsChecked = true;
                        DiameterButton.IsEnabled = false;
                        HeightButton.IsChecked = false;
                        HeightButton.IsEnabled = true;
                    }
                    else if (_selectMeasure.Name == "ЗВ")
                    {
                        DiameterDockPanel.Visibility = Visibility.Visible;
                        HeightDockPanel.Visibility = Visibility.Visible;
                        DiameterButton.IsChecked = false;
                        DiameterButton.IsEnabled = false;
                        HeightButton.IsChecked = true;
                        HeightButton.IsEnabled = false;
                    }
                    else if (_selectMeasure.Name == "ЗД")
                    {
                        DiameterDockPanel.Visibility = Visibility.Visible;
                        HeightDockPanel.Visibility = Visibility.Hidden;
                        DiameterButton.IsChecked = false;
                        DiameterButton.IsEnabled = false;
                        HeightButton.IsChecked = false;
                        HeightButton.IsEnabled = false;
                    }
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DataGrid.IsReadOnly = false;
            DataGrid.CanUserAddRows = false;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DataGrid.IsReadOnly = true;
        }
    }
}