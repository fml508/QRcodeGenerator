using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QRcodeGenerator
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.TextBox[] textBoxes;

        // Replace magic numbers with a clear enum for the selection type.
        private enum OptionType { None = -1, Wifi = 0, String = 1 }

        public Form1()
        {
            InitializeComponent();
            WireEvents();
        }
        private void WireEvents()
        {

            textBoxes = new System.Windows.Forms.TextBox[] { textBox1, textBox2, textBox3 };
            foreach (var textbox in textBoxes)
            {
                textbox.TextChanged += TextBox_TextChanged;
            }

        }


        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            // Use the enum casted from SelectedIndex to avoid reading comboBox state inside IsReady.
            IsReady((OptionType)comboBox2.SelectedIndex);
        }

        private void TurnOffAll()
        {
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;

            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;

            button1.Visible = false;

        }
        private void ResetTextboxes()
        {
            foreach (var textBox in textBoxes)
            {
                textBox.Text = "";
            }
        }
        private void QrCodeShowBox()
        {
            string latestQRCodeFile = FindLatestQRCode();

            if (latestQRCodeFile != null)
            {
                using (Image image = Image.FromFile(latestQRCodeFile))
                {

                    int imageWidth = image.Width;
                    int imageHeight = image.Height;


                    Form displayForm = new Form();
                    PictureBox pictureBox = new PictureBox
                    {
                        Image = image,
                        Dock = DockStyle.Fill,
                        SizeMode = PictureBoxSizeMode.AutoSize
                    };

                    displayForm.Controls.Add(pictureBox);

                    displayForm.ClientSize = new Size(imageWidth, imageHeight);
                    displayForm.ShowDialog();

                }
                try
                {
                    File.Delete(latestQRCodeFile);
                }
                catch (Exception e)
                {

                    Console.WriteLine(e);
                    //File.Delete(latestQRCodeFile);
                }

            }
        }

        private OptionType UpdateUiForSelection()
        {
            // Init UI according to comboBox selected index and return the semantic selection as an enum.
            OptionType sel = (OptionType)comboBox2.SelectedIndex;

            if (sel == OptionType.Wifi)
            {
                InitWIFIoption();
            }
            else if (sel == OptionType.String)
            {
                InitStringoption();
            }

            return sel;
        }

        private void InitStringoption()
        {
            TurnOffAll();
            label2.Text = "STRING";
            label2.Visible = true;
            textBox1.Visible = true;
        }

        private void InitWIFIoption()
        {
            label2.Text = "WIFI SECURITY";
            label3.Text = "WIFI SSID";
            label4.Text = "WIFI PASSWORD";

            label2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;

            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
        }

        private void IsReady(OptionType selectedOption)
        {
            // Kezdetben mindig rejtjük a gombot, aztán a paraméter alapján beállítjuk.
            button1.Visible = false;

            // Ha nincs kiválasztás, akkor ne legyen kész.
            if (selectedOption == OptionType.None)
            {
                return;
            }

            // WIFI (három mező kötelező) vs STRING (egy mező elegendő)
            if (selectedOption == OptionType.Wifi)
            {
                if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(textBox3.Text))
                {
                    button1.Visible = true;
                }
            }
            else if (selectedOption == OptionType.String)
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    button1.Visible = true;
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetTextboxes();
            TurnOffAll();
            OptionType sel = UpdateUiForSelection();
            // Miután frissítettük a UI-t, ellenőrizzük, hogy a jelenlegi állapot kész-e.
            IsReady(sel);
        }

        private bool PythonInstalled()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "python.exe",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    return output.Contains("Python");
                }
            }
            catch
            {
                return false;
            }
        }

        private string FindLatestQRCode()
        {
            string directoryPath = Path.Combine(Application.StartupPath, "QRcodes");
            string[] files = Directory.GetFiles(directoryPath, "BUUUUUU_*.png");


            if (files.Length > 0)
            {
                Array.Sort(files);
                return files[files.Length - 1];
            }

            return null;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (!PythonInstalled())
            {
                MessageBox.Show("Python is not installed on your machine.");
            }

            string arg1 = textBox1.Text;
            string arg2 = textBox2.Text;
            string arg3 = textBox3.Text;

            //TEMP SOLUTION: seems permanent af
            string pythonScriptPath = Path.Combine(Application.StartupPath, "Scripts", "qcode.py");
            string argumentsFilePath = Path.Combine(Application.StartupPath, "Scripts", "arguments.txt");

            File.WriteAllText(argumentsFilePath, $"{arg1} {arg2} {arg3}");
            string argumentsContent = $"{arg1} {arg2} {arg3}";
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "python.exe";
                process.StartInfo.Arguments = $"{pythonScriptPath} {argumentsContent}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();
                process.WaitForExit();

                QrCodeShowBox();

                ResetTextboxes();
            }

        }
    }
}