using System;
using System.Windows.Forms;
using System.IO.Ports; // Librarie pentru comunicare intre porturi
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace CsharpCode
{
    public partial class Form1 : Form
    {
        //Crearea Obiectelor
        SerialPort myPort = new SerialPort();
        SpeechRecognitionEngine re = new SpeechRecognitionEngine();
        SpeechSynthesizer ss = new SpeechSynthesizer(); // Cand programul vrea sa vorbeasca cu noi 
        Choices commands = new Choices(); // Vom memora comenzile in acest obiect
        
       
        public Form1()
        {
            InitializeComponent();
            //Detalii placa Arduino
            myPort.PortName = "COM3"; // Alegeti pe ce COM este arduino-ul
            myPort.BaudRate = 9600;  // La fel ca comanda Serial.begin(9600) bits per second

            processing();
            
        }

        // Functia Processing
        void processing()
        { 
            //Toate Comenzile 
            commands.Add(new string[] { "Start Wake-up Alarm", "Food Break", "When are the solar phases", "What is the humidity in the room", "Ardexa who made you", "For what were you made" });

            //Vom crea un obiect de tip Grammar pentru a pasa comenzile ca argument
            Grammar gr = new Grammar(new GrammarBuilder(commands));

            // https://docs.microsoft.com/en-us/dotnet/api/system.speech.recognition?view=netframework-4.7.2

            re.RequestRecognizerUpdate(); // Oprim Recunoasterea Vocii cand incepem comanda
            re.LoadGrammarAsync(gr);
            re.SetInputToDefaultAudioDevice();// Folosim ca INPUT, Windows Default Input Device
            re.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(re_SpeechRecognized);
            
            
        }

        void re_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch(e.Result.Text)
            {
                ////To Start the Wake-Up Alaram
                // Implementare - Proiectul contine o alarma care te  trezeste si porneste un timer de setat de tine pana trb sa ajungi la birou
                case "Start Wake-up Alarm":
                    sendDataToArduino('c');
                    break;

                // Pauza de masa
                // Implementare - Poriectul contine un buton care porneste cronometrul pentru pauza de masa.
                case "Food Break":
                    sendDataToArduino('m');
                    break;

                // Fazele Soarelui
                // Proiectul Contine un ceas care monitorizeaza in timp real fazele soarelui
                case "When are the solar phases":
                    sendDataToArduino('s');
                    break;
                
                //Senzor Umiditate si Temperatura
                // Proiectul trebuie sa contina un senzor de umiditate si temperatura.
                case "What is the humidity in the room":
                    sendDataToArduino('u');
                    break;

                //Easter Eggs
                case "Ardexa who made you":
                    ss.SpeakAsync("Omar"); // speech synthesis object is used for this purpose
                    break;


                case "For what were you made":
                    ss.SpeakAsync("For the Arduino National Competition Hosted"); // speech synthesis object is used for this purpose
                    break;

                // Pentru Iesire
                case "Exit":
                    Application.Exit();
                    break;
            }
            txtCommands.Text += e.Result.Text.ToString() + Environment.NewLine;// Sa apara fiecare comanda text in cutie
        }

        void sendDataToArduino(char character) // Functie pentru a trimite data cu prin porturile ARDUINO
        {
            myPort.Open();
            myPort.Write(character.ToString());
            myPort.Close();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            re.RecognizeAsyncStop();
            //btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnStart.Enabled = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            
            re.RecognizeAsync(RecognizeMode.Multiple);
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            MessageBox.Show("Recunoasterea Vocii a fost activata! Program creat de Ramadan Omar pentru concursul Arduino by Bosch ", "A functionat!", MessageBoxButtons.OK, MessageBoxIcon.Information); // Afisare Mesaj
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
