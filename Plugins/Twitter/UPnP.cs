using System;

using System.IO;

using System.Net;

using System.Net.NetworkInformation;

using System.Net.Sockets;

using System.Text;

namespace Progressive.PecaStarter5.Plugins.Twitter
{



    public class UPnP

    {

        public UPnP()

        {

        }



        private static int currentExternalPort = 0;

        private static string currentProtocol = "TCP";

        private static int currentInternalPort = 0;

        private static string currentInternalClient = "";

        private static string currentPortMappingDescription = "UPnP Sample";



        private static int currentPortMappingIndex = 0;



        public static string GetExternalIPAddress()

        {

            return Command("GetExternalIPAddress");

        }



        public static string AddPortMapping(int _externalPort, int _internalPort, string _protocol, string _portMappingDescription)

        {

            currentExternalPort = _externalPort;

            currentInternalPort = _internalPort;

            currentProtocol = _protocol;

            currentPortMappingDescription = _portMappingDescription;



            return Command("AddPortMapping");

        }



        public static string DeletePortMapping(int _externalPort, string _protocol)

        {

            currentExternalPort = _externalPort;

            currentProtocol = _protocol;

            return Command("DeletePortMapping");

        }



        public static string GetGenericPortMappingEntry(int _newPortMappingIndex)

        {

            currentPortMappingIndex = _newPortMappingIndex;

            return Command("GetGenericPortMappingEntry");

        }



        public static string GetRouterInformation()

        {

            return Command("");

        }



        private static string Command(string _command)

        {

            return GetIpAddressAnd(_command);

        }



        private static string GetIpAddressAnd(string _command)

        {

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)

            {

                try

                {

                    IPInterfaceProperties ipInterfaceProperties = networkInterface.GetIPProperties();

                    string machineIp = "";



                    foreach (var unicastAddress in ipInterfaceProperties.UnicastAddresses)

                        if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)

                            machineIp = currentInternalClient = unicastAddress.Address.ToString();



                    foreach (var gatewayIPAddressInformation in ipInterfaceProperties.GatewayAddresses)

                    {

                        try

                        {

                            return GetServicesAnd(machineIp, gatewayIPAddressInformation.Address.ToString(), _command);

                        }

                        catch (Exception ex)

                        {

                            Console.WriteLine("エラー発生！");

                            Console.WriteLine(ex.Message);

                            Console.WriteLine(ex.StackTrace);

                        }

                    }

                }

                catch (Exception ex)

                {

                    Console.WriteLine("エラー発生！");

                    Console.WriteLine(ex.Message);

                    Console.WriteLine(ex.StackTrace);

                }

            }



            return null;

        }



        private static string GetServicesAnd(string _machineIP, string _firewallIP, string _command)

        {

            int port = 0;

            string services = GetServicesFromDevice(ref port);

            if (_command == "")

                return services;



            string externalIpAddress = Soap(services, "urn:schemas-upnp-org:service:WANPPPConnection:1", _firewallIP, port, _command);

            if (externalIpAddress != null)

                return externalIpAddress;

            externalIpAddress = Soap(services, "urn:schemas-upnp-org:service:WANIPConnection:1", _firewallIP, port, _command);

            return externalIpAddress;

        }



        private static string GetServicesFromDevice(ref int _port)

        {

            string responseString = "";

            try

            {

                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1500);



                EndPoint endPoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);

                string requestString = "M-SEARCH * HTTP/1.1\r\n" +

                                       "HOST: 239.255.255.250:1900\r\n" +

                                       "ST: upnp:rootdevice\r\n" +

                                       "MAN: \"ssdp:discover\"\r\n" +

                                       "MX: 3\r\n" +

                                       "\r\n";

                byte[] requestByte = Encoding.ASCII.GetBytes(requestString);

                client.SendTo(requestByte, requestByte.Length, SocketFlags.None, endPoint);



                EndPoint endPoint2 = new IPEndPoint(IPAddress.Any, 0);

                byte[] responseByte = new byte[1024];

                client.ReceiveFrom(responseByte, ref endPoint2);

                responseString = Encoding.ASCII.GetString(responseByte);

            }

            catch (Exception ex)

            {

                Console.WriteLine("エラー発生！");

                Console.WriteLine(ex.Message);

                Console.WriteLine(ex.StackTrace);

            }



            if (responseString.Length == 0)

                return "";



            string location = "";

            string[] parts = responseString.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)

                if (part.ToLower().StartsWith("location"))

                {

                    location = part.Substring(part.IndexOf(':') + 1);

                    string port = location.Substring(location.LastIndexOf(':') + 1);

                    _port = int.Parse(port.Substring(0, port.IndexOf('/')));

                    break;

                }

            if (location.Length == 0)

                return "";



            using (WebClient webClient = new WebClient())

            {

                return webClient.DownloadString(location);

            }

        }



        private static string Soap(string _services, string _serviceType, string _firewallIP, int _port, string _command)

        {

            if (_services.Length == 0)

                return null;

            int serviceIndex = _services.IndexOf(_serviceType);

            if (serviceIndex == -1)

                return null;

            string controlUrl = _services.Substring(serviceIndex);

            string tag1 = "<controlURL>";

            string tag2 = "</controlURL>";

            controlUrl = controlUrl.Substring(controlUrl.IndexOf(tag1) + tag1.Length);

            controlUrl = controlUrl.Substring(0, controlUrl.IndexOf(tag2));



            string bodyString = CreateSoapBody(_command, _serviceType);

            byte[] bodyByte = UTF8Encoding.ASCII.GetBytes(bodyString);

            string headString = "POST " + controlUrl + " HTTP/1.1\r\n" +

                                "HOST: " + _firewallIP + ":" + _port + "\r\n" +

                                "CONTENT-LENGTH: " + bodyByte.Length + "\r\n" +

                                "CONTENT-TYPE: text/xml; charset=\"utf-8\"" + "\r\n" +

                                "SOAPACTION: \"" + _serviceType + "#" + _command + "\"\r\n" +

                                "\r\n";

            byte[] headByte = Encoding.ASCII.GetBytes(headString);

            byte[] requestByte = new byte[headByte.Length + bodyByte.Length];

            headByte.CopyTo(requestByte, 0);

            bodyByte.CopyTo(requestByte, headByte.Length);



            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(_firewallIP), _port);

            client.Connect(endPoint);

            client.Send(requestByte, requestByte.Length, SocketFlags.None);



            byte[] responseByte = new byte[1024];

            MemoryStream memoryStream = new MemoryStream();

            while (true)

            {

                int responseSize = client.Receive(responseByte, responseByte.Length, SocketFlags.None);

                if (responseSize == 0)

                    break;

                memoryStream.Write(responseByte, 0, responseSize);

            }

            string responseString = Encoding.ASCII.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);

            memoryStream.Close();



            client.Shutdown(SocketShutdown.Both);

            client.Close();



            return CreateReturnString(_command, responseString);

        }



        private static string CreateSoapBody(string _command, string _serviceType)

        {

            string bodyString = null;



            if (_command == "GetExternalIPAddress")

            {

                bodyString =

                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +

                    "<s:Envelope " + "xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" " + "s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +

                    " <s:Body>" +

                    "  <u:GetExternalIPAddress xmlns:u=\"" + _serviceType + "\">" + "</u:GetExternalIPAddress>" +

                    " </s:Body>" +

                    "</s:Envelope>";

            }

            else if (_command == "AddPortMapping")

            {

                bodyString =

                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +

                    "<s:Envelope " + "xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" " + "s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +

                    " <s:Body>" +

                    "  <u:AddPortMapping xmlns:u=\"" + _serviceType + "\">" +

                    "   <NewRemoteHost></NewRemoteHost>" +

                    "   <NewExternalPort>" + currentExternalPort + "</NewExternalPort>" +

                    "   <NewProtocol>" + currentProtocol + "</NewProtocol>" +

                    "   <NewInternalPort>" + currentInternalPort + "</NewInternalPort>" +

                    "   <NewInternalClient>" + currentInternalClient + "</NewInternalClient>" +

                    "   <NewEnabled>1</NewEnabled>" +

                    "   <NewPortMappingDescription>" + currentPortMappingDescription + "</NewPortMappingDescription>" +

                    "   <NewLeaseDuration>0</NewLeaseDuration>" +

                    "  </u:AddPortMapping>" +

                    " </s:Body>" +

                    "</s:Envelope>";

            }

            else if (_command == "DeletePortMapping")

            {

                bodyString =

                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +

                    "<s:Envelope " + "xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" " + "s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +

                    " <s:Body>" +

                    "  <u:DeletePortMapping xmlns:u=\"" + _serviceType + "\">" +

                    "   <NewRemoteHost></NewRemoteHost>" +

                    "   <NewExternalPort>" + currentExternalPort + "</NewExternalPort>" +

                    "   <NewProtocol>" + currentProtocol + "</NewProtocol>" +

                    "  </u:DeletePortMapping>" +

                    " </s:Body>" +

                    "</s:Envelope>";

            }

            else if (_command == "GetGenericPortMappingEntry")

            {

                bodyString =

                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +

                    "<s:Envelope " + "xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" " + "s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +

                    " <s:Body>" +

                    "  <u:GetGenericPortMappingEntry xmlns:u=\"" + _serviceType + "\">" +

                    "   <NewPortMappingIndex>" + currentPortMappingIndex + "</NewPortMappingIndex>" +

                    "  </u:GetGenericPortMappingEntry>" +

                    " </s:Body>" +

                    "</s:Envelope>";

            }



            return bodyString;

        }



        private static string CreateReturnString(string _command, string _response)

        {

            string response = _response;



            if (_command == "GetExternalIPAddress")

            {

                string tag3 = "<NewExternalIPAddress>";

                string tag4 = "</NewExternalIPAddress>";

                response = response.Substring(response.IndexOf(tag3) + tag3.Length);

                response = response.Substring(0, response.IndexOf(tag4));

            }

            else if (_command == "AddPortMapping")

                if (response.StartsWith("HTTP/1.1 200 OK"))

                    response = "Success!!";

                else

                    response = "???";

            else if (_command == "DeletePortMapping")

            {

                if (response.StartsWith("HTTP/1.1 200 OK"))

                    response = "Success!!";

                else

                    response = "???";

            }

            else if (_command == "GetGenericPortMappingEntry")

            {

                if (response.StartsWith("HTTP/1.1 200 OK"))

                {

                    string value = "";



                    string tag3 = "<NewRemoteHost>";

                    string tag4 = "</NewRemoteHost>";

                    value += response.Substring(response.IndexOf(tag3) + tag3.Length);

                    value = value.Substring(0, value.IndexOf(tag4));



                    value += " ";



                    tag3 = "<NewExternalPort>";

                    tag4 = "</NewExternalPort>";

                    value += response.Substring(response.IndexOf(tag3) + tag3.Length);

                    value = value.Substring(0, value.IndexOf(tag4));



                    value += " ";



                    tag3 = "<NewProtocol>";

                    tag4 = "</NewProtocol>";

                    value += response.Substring(response.IndexOf(tag3) + tag3.Length);

                    value = value.Substring(0, value.IndexOf(tag4));



                    value += " ";



                    tag3 = "<NewInternalPort>";

                    tag4 = "</NewInternalPort>";

                    value += response.Substring(response.IndexOf(tag3) + tag3.Length);

                    value = value.Substring(0, value.IndexOf(tag4));



                    value += " ";



                    tag3 = "<NewInternalClient>";

                    tag4 = "</NewInternalClient>";

                    value += response.Substring(response.IndexOf(tag3) + tag3.Length);

                    value = value.Substring(0, value.IndexOf(tag4));



                    value += " ";



                    tag3 = "<NewEnabled>";

                    tag4 = "</NewEnabled>";

                    value += response.Substring(response.IndexOf(tag3) + tag3.Length);

                    value = value.Substring(0, value.IndexOf(tag4));



                    value += " ";



                    tag3 = "<NewPortMappingDescription>";

                    tag4 = "</NewPortMappingDescription>";

                    value += response.Substring(response.IndexOf(tag3) + tag3.Length);

                    value = value.Substring(0, value.IndexOf(tag4));



                    value += " ";



                    tag3 = "<NewLeaseDuration>";

                    tag4 = "</NewLeaseDuration>";

                    value += response.Substring(response.IndexOf(tag3) + tag3.Length);

                    value = value.Substring(0, value.IndexOf(tag4));



                    response = value;

                }

                else

                    response = "???";

            }



            return response;

        }

    }
}
