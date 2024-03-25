using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using serverLis;
using System.Data.SqlClient;

namespace serverLis
{
    public partial class LIS : Form
    {
         string ip;
         int port;
        public LIS()
        {
            InitializeComponent();
        }

        // gate open button to transfer ipaddress of analizer to Server and open connection
        private void btnOpen_Click_1(object sender, EventArgs e)

        {
            timer1.Start();
            
        }

        public void ExecuteServer()
        {
            string[] arrIp;
            arrIp = ipadress1.Text.Split(new char[] { ':' });
            ip = arrIp[0];
            port = Int32.Parse(arrIp[1]);
            
            /* status1.Text = ip;
             resultPatient.Text = port.ToString();*/

            // Establish the local endpoint 
            // for the socket. Dns.GetHostName
            // returns the name of the host 
            // running the application.
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
           /* IPAddress ipAddr = IPAddress.Parse(ip);
            resultPatient.Text = ipAddr.ToString();*/
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr,11111);

            // Creation TCP/IP Socket using 
            // Socket Class Constructor
            Socket listener = new Socket(ipAddr.AddressFamily,
                         SocketType.Stream, ProtocolType.Tcp);

            try
            {
                
                // Using Bind() method we associate a
                // network address to the Server Socket
                // All client that will connect to this 
                // Server Socket must know this network
                // Address
                listener.Bind(localEndPoint);

                // Using Listen() method we create 
                // the Client list that will want
                // to connect to Server
                listener.Listen(10);

                
                    

                    // Suspend while waiting for
                    // incoming connection Using 
                    // Accept() method the server 
                    // will accept connection of client
                    Socket clientSocket = listener.Accept();
                      status1.Text = "connected to analizer";
                        
                        // Data buffer
                        byte[] bytes = new Byte[102400];
                        string data = null;

                        while (true)
                        {

                            int numByte = clientSocket.Receive(bytes);

                            data += Encoding.ASCII.GetString(bytes,
                                                       0, numByte);

                            if (data.IndexOf("<EOF>") > -1)
                                break;
                        }

                        
                        //resultPatient.Text = data;
                        //add result received from analyzer to SQL Database
                        AddDataToDataBase(data);
                        byte[] message = Encoding.ASCII.GetBytes("Test Server");

                        // Send a message to Client 
                        // using Send() method
                        clientSocket.Send(message);

                        // Close client Socket using the
                        // Close() method. After closing,
                        // we can use the closed Socket 
                        // for a new Client Connection
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                                   
                
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ExecuteServer();
        }

        private void AddDataToDataBase(string data)
        {
            /*Split data received from analizer 
             * into patient name, test name, result and add to database*/
            splitString(data);
            
            string query = @"Insert Into RESULT(NAME, TEST NAME, RESULT)
VALUES ($'{name}','jgjf')";
            using (SqlConnection connection = new SqlConnection(ConnectionString.connectionString))
            {
                connection.Open();
                SqlCommand addResult = new SqlCommand(query, connection);
                addResult.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void splitString(string s)
        {
            string str1 = s.Replace("|", " ");
            string[] str2 = str1.Split(new char[] { ' ' });

        }
    }



}
