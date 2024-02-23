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

namespace App1
{
    public partial class Form1 : Form
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string PipeName = "";

        public Form1()
        {
            InitializeComponent();
        }
        //=========
        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Run(() => SendDataToOtherAppAsync($"{PipeName}:" + DateTime.Now.ToString()));
        }

        //====
        private async Task SendDataToOtherAppAsync(string data)
        {
            using (NamedPipeClientStream pipeServer = new NamedPipeClientStream(".", "ServerPipe", PipeDirection.InOut))
            {
                try
                {
                    pipeServer.Connect(1000);

                    // Write data to the named pipe
                    using (StreamWriter writer = new StreamWriter(pipeServer)) {
                        await writer.WriteLineAsync(data);
                    }
                }catch {; }
            }
        }
        //=========
        private void Form1_Load(object sender, EventArgs e)
        {  timer1.Enabled = true;
        }
        //=========
        private async void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            await Task.Run(() => StartListeningForData());
        }
        //=========
        private async Task StartListeningForData()
        {
            PipeName = assembly.GetName().Name;
            using (NamedPipeServerStream pipeClient = new NamedPipeServerStream(PipeName, PipeDirection.InOut))
            {
                try {
                    pipeClient.WaitForConnection(); // Wait for connection

                    // Read data from the named pipe
                    using (StreamReader reader = new StreamReader(pipeClient))
                    {

                        string Recieved = await reader.ReadLineAsync();
                        Invoke(new Action(()  =>
                        {
                            textBox1.Text += Environment.NewLine + Recieved;
                            timer1.Enabled = true;
                        }));

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error receiving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //=========
    }
}
 