using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using InTheHand.Net.Sockets;
using GraduationProject.Models;

namespace GraduationProject
{
    public static class CurrentContext
    {
        public static List<DataModel> DataList = new List<DataModel>();
        public static List<BluetoothDeviceInfo> Devices = new List<BluetoothDeviceInfo>();
        public static int GlobalId;
        public static double StartupX;
        public static double StartupY;

        public static List<MeasureValueModel> MeasureValues = new List<MeasureValueModel>
        {
            new MeasureValueModel {Id = 1,Name = "ГИ", TemplateColumns = new List<TemplateColumn> { new TemplateColumn {Name = "X", BindingName = "X"}, new TemplateColumn { Name = "Y", BindingName = "Y" }, new TemplateColumn { Name = "Горизонт. дистанция", BindingName = "HorizontalDistance" }, new TemplateColumn { Name = "Верт. дистанция", BindingName = "VerticalDistance" }, new TemplateColumn { Name = "Наклон. дистанция", BindingName = "SlopeDistance" }, new TemplateColumn { Name = "Азимут", BindingName = "Azimuth" }, new TemplateColumn { Name = "Уклон", BindingName = "Bias" } }},
            new MeasureValueModel {Id = 2, Name = "РКП", TemplateColumns = new List<TemplateColumn> { new TemplateColumn {Name = "X", BindingName = "X"}, new TemplateColumn { Name = "Y", BindingName = "Y" }, new TemplateColumn { Name = "Горизонт. дистанция", BindingName = "HorizontalDistance" }, new TemplateColumn { Name = "Верт. дистанция", BindingName = "VerticalDistance" }, new TemplateColumn { Name = "Наклон. дистанция", BindingName = "SlopeDistance" }, new TemplateColumn { Name = "Азимут", BindingName = "Azimuth" }, new TemplateColumn { Name = "Уклон", BindingName = "Bias" }, new TemplateColumn {Name = "Диаметр №1", BindingName = "DiameterOne" }, new TemplateColumn { Name = "Порода", BindingName = "Species" }, new TemplateColumn { Name = "Кат. 1", BindingName = "CategoryOne" }, new TemplateColumn {Name = "Кат. 2", BindingName = "CategoryTwo" } }},
            new MeasureValueModel {Id = 3, Name = "ППП", TemplateColumns = new List<TemplateColumn> { new TemplateColumn { Name = "Номер дерева", BindingName = "TreeNumber" }, new TemplateColumn { Name = "Горизонт. дистанция", BindingName = "HorizontalDistance" }, new TemplateColumn { Name = "Верт. дистанция", BindingName = "VerticalDistance" }, new TemplateColumn { Name = "Наклон. дистанция", BindingName = "SlopeDistance" }, new TemplateColumn { Name = "Азимут", BindingName = "Azimuth" }, new TemplateColumn { Name = "Уклон", BindingName = "Bias" }, new TemplateColumn { Name = "Диаметр №1", BindingName = "DiameterOne" }, new TemplateColumn {Name = "Диаметр №2", BindingName = "DiameterTwo" }, new TemplateColumn {Name = "Порода", BindingName = "Species" }, new TemplateColumn { Name = "Кат. 1", BindingName = "CategoryOne" }, new TemplateColumn {Name = "Кат. 2", BindingName = "CategoryTwo" }  }},
            new MeasureValueModel {Id = 4, Name = "ЗВ", TemplateColumns = new List<TemplateColumn> { new TemplateColumn { Name = "Верт. дистанция", BindingName = "VerticalDistance" }, new TemplateColumn { Name = "Азимут", BindingName = "Azimuth" }, new TemplateColumn {Name = "Диаметр №1", BindingName = "DiameterOne" }, new TemplateColumn {Name = "Порода", BindingName = "Species" }, new TemplateColumn { Name = "Кат. 1", BindingName = "CategoryOne" }, new TemplateColumn {Name = "Кат. 2", BindingName = "CategoryTwo" }  }},
            new MeasureValueModel {Id = 5, Name = "ЗД", TemplateColumns = new List<TemplateColumn> { new TemplateColumn {Name = "Диаметр №1", BindingName = "DiameterOne" }, new TemplateColumn {Name = "Порода", BindingName = "Species" }, new TemplateColumn { Name = "Кат. 1", BindingName = "CategoryOne" }, new TemplateColumn {Name = "Кат. 2", BindingName = "CategoryTwo" }  }}
        };

        public static async Task UpdateDevices()
        {
            var client = new BluetoothClient();
            var devices =  client.DiscoverDevices();
            Devices = new List<BluetoothDeviceInfo>(devices);
        }

        public static string GetMacAddress()
        {
            var mac = "";
            var result = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Down && !nic.Description.Contains("Virtual") &&
                    !nic.Description.Contains("Pseudo") && !nic.Description.Contains("Wireless"))
                {
                    if (nic.GetPhysicalAddress().ToString() != "")
                    {
                        mac = nic.GetPhysicalAddress().ToString();
                    }
                }
            }
            for (var i = 0; i < mac.Length; i++)
            {
                result += mac[i];

                if (i % 2 != 0 && mac.Length - 1 != i)
                {
                    result += ":";
                }
            }

            return result;
        }

        public static double? ToDoubleParse(string variable)
        {
            double result;
            if (double.TryParse(variable, out result))
            {
                return result;
            }
            return null;
        }
    }
}