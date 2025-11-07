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
            // Az IsReady most paraméterként várja a combobox selected indexet.
            IsReady(comboBox2.SelectedIndex);
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

        private int Selected()
        {
            // A UI-t inicializáljuk a combobox aktuális értéke szerint,
            // majd visszaadjuk az aktuális selected indexet, hogy az IsReady-t meg tudjuk hívni a TextChanged eseményben is.
            int sel = comboBox2.SelectedIndex;

            if (sel == 0)
            {
                InitWIFIoption();
            }
            else
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

        private void IsReady(int selectedIndex)
        {
            // Kezdetben mindig rejtjük a gombot, aztán a paraméter alapján beállítjuk.
            button1.Visible = false;

            // Ha nincs kiválasztás (selectedIndex == -1), akkor ne legyen kész.
            if (selectedIndex < 0)
            {
                return;
            }

            // selectedIndex == 0 => WIFI (három mező kötelező), különben STRING (egy mező elegendő)
            if (selectedIndex == 0)
            {
                if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(textBox3.Text))
                {
                    button1.Visible = true;
                }
            }
            else
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
            int sel = Selected();
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