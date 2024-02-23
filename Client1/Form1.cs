using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
 
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
using System.Reflection;
using System.Threading;

namespace App1
{
    public partial class Form1 : Form
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
       

        public Form1()
        {
            InitializeComponent();
        }
        //=========
        private async void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text += (await Send_Data(DateTime.Now.ToString())) + Environment.NewLine;
        }

        //=========
        private async Task<string> Send_Data(string MyData)
        {
            try
            {
                using (NamedPipeClientStream clientStream = new NamedPipeClientStream(".", "SamplePipe", PipeDirection.InOut))
                {
                    if (clientStream.IsConnected == false) { clientStream.Connect(); }

                    // Send data to the server
                    byte[] buffer = Encoding.UTF8.GetBytes(MyData);
                    await clientStream.WriteAsync(buffer, 0, buffer.Length);

                    // Receive response from the server
                    buffer = new byte[1024];
                    int bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                    string responseData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Process the response
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }

        }

      

        //=======================================================================================

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //=========
    }
}
 