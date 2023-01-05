using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using System;
using System.Linq;

namespace TIAHW
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TiaPortal portal = TiaPortal.GetProcesses().First().Attach();
                Project project = portal.Projects.First();
                foreach (Device device in project.Devices)
                {
                    SetDeviceNameWithNum(device);
                }
                foreach (DeviceGroup group in project.DeviceGroups)
                {
                    foreach (Device device in group.Devices)
                    {
                        SetDeviceNameWithNum(device);
                    }
                }
                foreach (Device device in project.UngroupedDevicesGroup.Devices)
                {
                    SetDeviceNameWithNum(device);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            Console.Write("按任意键退出...");
            Console.ReadKey(true);
        }
        static void SetFailSafeDestAddress(Device device)
        {
            Console.WriteLine("Device: " + device.Name + " : " + device.TypeIdentifier);
            foreach (DeviceItem item in device.DeviceItems)
            {
                //if (item.Classification == DeviceItemClassifications.HM)
                {
                    Console.WriteLine("    Item: " + item.Name + " : " + item.Classification);

                    foreach (DeviceItem module in item.DeviceItems)
                    {
                        Console.WriteLine("        Module: " + module.Name + " : " + module.Classification);
                        foreach (var info in module.GetAttributeInfos())
                        {
                            Console.WriteLine("            Info:" + info.Name);
                            if (info.Name == "Failsafe_FDestinationAddress")
                            {
                                ulong fds = 233;
                                module.SetAttribute("Failsafe_FDestinationAddress", fds);
                            }
                        }
                        //NetworkInterface net = module.GetService<NetworkInterface>();
                        //if (net != null && net.InterfaceType == NetType.Ethernet && net.InterfaceOperatingMode == InterfaceOperatingModes.IoDevice)
                        {
                            //string oldName = device.Name;
                            //device.Name = item.Name;
                            //device.SetAttribute("Author", "WuTong");
                            //item.SetAttribute("Author", "WuTong");
                            //Console.WriteLine("已将设备名称 [" + oldName + "] 同步为模块名 [" + item.Name + "] .");

                            /*Node node = net.Nodes.First();
                            if (node != null && node.NodeType == NetType.Ethernet)
                            {
                                foreach (IoConnector con in net.IoConnectors)
                                {
                                    if (con.ConnectedToIoSystem != null)
                                    {
                                        //string ip = node.GetAttribute("Address").ToString();
                                        //int num = int.Parse(ip.Substring(ip.LastIndexOf(".") + 1));
                                        //string oldNum = con.GetAttribute("PnDeviceNumber").ToString();
                                        //con.SetAttribute("PnDeviceNumber", num);
                                        //Console.WriteLine("模块 [" + item.Name + "] 的设备编号根据 IP[" + ip + "] 由 [" + oldNum + "] 改为 [" + num + "].");
                                    }
                                }
                            }*/

                            foreach (var it in module.DeviceItems)
                            {
                                Console.WriteLine("            Sub: " + it.Name + " : " + it.Classification);
                            }
                            //break;
                        }
                    }
                    //break;
                }
            }
        }

        static void SetDeviceNameAndNum(Device device)
        {
            foreach (DeviceItem item in device.DeviceItems)
            {
                if (item.Classification == DeviceItemClassifications.HM)
                {
                    foreach (DeviceItem module in item.DeviceItems)
                    {
                        NetworkInterface net = module.GetService<NetworkInterface>();
                        if (net != null && net.InterfaceType == NetType.Ethernet && net.InterfaceOperatingMode == InterfaceOperatingModes.IoDevice)
                        {
                            if (device.Name != item.Name)
                            {
                                string oldName = device.Name;
                                device.Name = item.Name;
                                Console.WriteLine("已将设备名称 [" + oldName + "] 同步为模块名 [" + item.Name + "] .");
                            }
                            device.SetAttribute("Author", "WuTong");
                            item.SetAttribute("Author", "WuTong");

                            Node node = net.Nodes.First();
                            if (node != null && node.NodeType == NetType.Ethernet)
                            {
                                foreach (IoConnector con in net.IoConnectors)
                                {
                                    if (con.ConnectedToIoSystem != null)
                                    {
                                        int oldNum = (int)con.GetAttribute("PnDeviceNumber");
                                        string ip = node.GetAttribute("Address").ToString();
                                        int num = int.Parse(ip.Substring(ip.LastIndexOf(".") + 1));
                                        if (oldNum != num)
                                        {
                                            con.SetAttribute("PnDeviceNumber", num);
                                            Console.WriteLine("模块 [" + item.Name + "] 的设备编号根据 IP[" + ip + "] 由 [" + oldNum + "] 改为 [" + num + "].");
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }
        static void SetDeviceNameWithNum(Device device)
        {
            foreach (DeviceItem item in device.DeviceItems)
            {
                if (item.Classification == DeviceItemClassifications.HM)
                {
                    foreach (DeviceItem module in item.DeviceItems)
                    {
                        NetworkInterface net = module.GetService<NetworkInterface>();
                        if (net != null && net.InterfaceType == NetType.Ethernet && net.InterfaceOperatingMode == InterfaceOperatingModes.IoDevice)
                        {
                            Node node = net.Nodes.First();
                            if (node != null && node.NodeType == NetType.Ethernet)
                            {
                                foreach (IoConnector con in net.IoConnectors)
                                {
                                    if (con.ConnectedToIoSystem != null)
                                    {
                                        int oldNum = (int)con.GetAttribute("PnDeviceNumber");
                                        string ip = node.GetAttribute("Address").ToString();
                                        int num = int.Parse(ip.Substring(ip.LastIndexOf(".") + 1));
                                        if (oldNum != num)
                                        {
                                            con.SetAttribute("PnDeviceNumber", num);
                                            Console.WriteLine("模块 [" + item.Name + "] 的设备编号根据 IP[" + ip + "] 由 [" + oldNum + "] 改为 [" + num + "].");
                                        }
                                        if (!item.Name.EndsWith("}"))
                                        {
                                            item.Name += "{" + num + "}";
                                            Console.WriteLine("模块名改为带IP [" + item.Name + "].");
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }
    }
}
