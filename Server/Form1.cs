using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App2
{
    public partial class Form1 : Form
    {

        private NamedPipeServerStream serverStream;

        public Form1()
        {
            InitializeComponent();
           
        }
        //============================================================================================================================================
        private async void Form1_Load(object sender, EventArgs e)
        {
            
            await Task.Run(() => ServerThreadMethod());

        }
        //============================================================================================================================================

        private async Task ServerThreadMethod()
        {
            try
            {
                serverStream = new NamedPipeServerStream("SamplePipe", PipeDirection.InOut);

                if (serverStream.IsConnected == false) { serverStream.WaitForConnection(); }
                 
                // Read data from the client
                byte[] buffer = new byte[1024];
              
                int bytesRead = await serverStream.ReadAsync(buffer, 0, buffer.Length);
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);


                // Send a response back to the client
                string responseData = "Server:" + DateTime.Now.ToString();
                byte[] responseBuffer = Encoding.UTF8.GetBytes(responseData);
                await serverStream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
                // Process the received data
                textBox1.Text += receivedData + Environment.NewLine;

                
                // Disconnect the pipe
                serverStream.Disconnect();
                serverStream.Dispose();
                await ServerThreadMethod();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Server Error: {ex.Message}");
            }
        }
        //============================================================================================================================================
    }
}
