using System.IO.Ports;
using System.Text;
using Modbus.Device;
using Modbus.Utility;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace rtuSayac
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort = null;
        private string temp;
        private int sayac = 0;
        public Form1()
        {
            InitializeComponent();
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            byte slaveAdres = 95;
            ushort baslamaAdres = 1;
            ushort boyut = 12;
            try
            {
                IModbusMaster masterRtu = ModbusSerialMaster.CreateRtu(serialPort);
                ushort[] cikti = masterRtu.ReadHoldingRegisters(slaveAdres, baslamaAdres, boyut);
                //----------------------------------------------------------------------
                //ProgressBar deðer atamalarý Rtu sayaç üzerinden çekilen veriler dahilinde.
                //----------------------------------------------------------------------
                watsaatYuksek.Value=cikti[3];
                watsaatAlcak.Value = cikti[4];
                gerilimProgressbar.Value = Convert.ToInt32(cikti[5]) / 100;
                akimProgressbar.Value = cikti[6];
                frekansProgressbar.Value=cikti[7];
                powerfactorProgressbar.Value = cikti[8];
                aktifgucProgressbar.Value= cikti[9];
                reaktifgucProgressbar.Value=(int)cikti[10];
                gorunurgucProgressbar.Value=cikti[11];
                //----------------------------------------------------------------------

                //----------------------------------------------------------------------
                //ProgressBar içerisindeki textlere rtu sayaç üzerinden çekilen verilen atýlmasý.
                //----------------------------------------------------------------------
                wattSaat_yuksekGerilim.Text = Convert.ToString(cikti[3]);
                watSaat_dusukGerilim.Text = String.Format("{0} ", cikti[4]);
                gerilim.Text = String.Format("{0} ", Convert.ToInt32(cikti[5]) / 100);
                akim.Text = String.Format("{0} ", cikti[6]);
                frekans.Text = String.Format("{0} ", Convert.ToInt32(cikti[7]) / 100);
                powerFactor.Text = String.Format("{0} ", Convert.ToInt32(cikti[8]) / 1000);
                aktifGuc.Text = String.Format("{0} ", Convert.ToInt32(cikti[9]) / 100);
                reaktifGuc.Text = String.Format("{0} ", Convert.ToInt32(cikti[10]) / 100);
                //----------------------------------------------------------------------

                //----------------------------------------------------------------------
                //Modbus Adresini Textboxa yazdýrma iþlemi
                //----------------------------------------------------------------------
                //

                foreach (ushort veri in cikti)
                {
                    textBox1.Text+= String.Format("{0}/ ", veri);
                    sayac++;
                }
                if (sayac % 12 == 0)
                {
                 temp = textBox1.Text;
                    FileStream fs = new FileStream(@"C:\Users\ykarakaya\Desktop\Holding Register.txt", FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(temp);
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
                modbusAdresi.Text =Convert.ToString(cikti[0],16);
               

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw;
            }
           

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //---------------------------------------
            //form ilk ekrana geldiðinde gerekli kontrollerin yapýlmasý için 
            //butonlarýn enable özellikleri ve checkhboxýn görünürlüðü false deðeri verilerek butonlara basýlmasý engellenir. 
            holdingRegOku.Enabled = false;
            inputRegister.Enabled = false;
            baglantiKes.Enabled = false;
            canliVeri.Visible = false;
            //---------------------------------------
            //---------------------------------------
            //
            string[] portlar = SerialPort.GetPortNames(); 
            foreach (string portAdi in portlar)
            {
                portNum.Items.Add(portAdi);
            }
            //---------------------------------------

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            try
            {
                serialPort = new SerialPort(Convert.ToString(portNum.SelectedItem), Convert.ToInt32(baudRate.SelectedItem),Parity.None, 8, StopBits.One);
               

                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw;
            }
            holdingRegOku.Enabled = true;
            inputRegister.Enabled = true;
            button2.Enabled = false;
            baglantiKes.Enabled=true;
            canliVeri.Visible = true;


        }

        private void baglantiKes_Click(object sender, EventArgs e)
        {
            serialPort.Close();

            button2.Enabled=true;
            holdingRegOku.Enabled=false;
            inputRegister.Enabled = false;
            timer1.Stop();
            watsaatYuksek.Value = 0;
            watsaatAlcak.Value = 0;
            gerilimProgressbar.Value = 0;
            akimProgressbar.Value = 0;
            frekansProgressbar.Value = 0;
            powerfactorProgressbar.Value=0;
            aktifgucProgressbar.Value = 0;
            reaktifgucProgressbar.Value = 0;
            gorunurgucProgressbar.Value = 0;
            wattSaat_yuksekGerilim.Text = Convert.ToString("0");
            watSaat_dusukGerilim.Text = Convert.ToString("0");
            gerilim.Text = Convert.ToString("0");
            akim.Text = Convert.ToString("0");
            powerFactor.Text= Convert.ToString("0");
            frekans.Text= Convert.ToString("0");
            aktifGuc.Text= Convert.ToString("0");
            reaktifGuc.Text = Convert.ToString("0");
            gorunurGuc.Text=Convert.ToString("0");
            modbusAdresi.Clear();
            textBox1.Clear();
            portNum.SelectedIndex = -1;
            baudRate.SelectedIndex = -1;
            canliVeri.Visible=false;
            canliVeri.Checked=false;
            guna2ToggleSwitch1.Visible = false;
            role.Visible = false;
            fan.Visible = false;
            fanToggler.Visible = false;

        }

        private void canliVeri_CheckedChanged(object sender, EventArgs e)
        {
            //------------------------
            //canli veri checkboxý deðiþtiðinde eðer chexbox iþaretli ise timer 200 ms gecikme ile timer fonk. çalýþtýrýlýr. 
            //------------------------
            if (canliVeri.Checked)
            {
                timer1.Interval = 200;
                timer1.Start();
               
            }
            //-------canli veri iþaretli deðilse timer durdurulur.
            if (!(canliVeri.Checked))
            {
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //---------------------------------------
            //timer canli veri check olduðunda holding registerlarýný okuyan butonu çalýþtýrýr.
            //Bu þekilde buton döngü þeklinde çalýþmýþ olur.
            //---------------------------------------
               button1_Click(sender, e);
        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            IModbusMaster masterRtuToggle = ModbusSerialMaster.CreateRtu(serialPort);

            if (guna2ToggleSwitch1.Checked == true)
            {
                masterRtuToggle.WriteSingleCoil(1, 1, true);
            }
            else
            {
                masterRtuToggle.WriteSingleCoil(1, 1, false);
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Select(textBox1.TextLength, 0);
            textBox1.ScrollToCaret();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            IModbusMaster masterRtuInput = ModbusSerialMaster.CreateRtu(serialPort);
            ushort[] ciktiInput = masterRtuInput.ReadInputRegisters(1, 0, 6);
            foreach (ushort veri in ciktiInput)
            {
                textBox1.Text += String.Format("{0}/ ", veri);
                sayac++;
            }
            if (sayac % 6 == 0)
            {
                temp = textBox1.Text;

                FileStream fs = new FileStream(@"C:\Users\ykarakaya\Desktop\Input Register.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(temp);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        private void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            IModbusMaster masterRtuToggleFan = ModbusSerialMaster.CreateRtu(serialPort);

            if (fanToggler.Checked == true)
            {
                masterRtuToggleFan.WriteSingleCoil(1, 0, true);
            }
            else
            {
                masterRtuToggleFan.WriteSingleCoil(1, 0, false);
            }
        }

        private void temizle_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

    }
}