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
using System.Net.Sockets; //biblio do ganiazd
using System.Threading; //biblio do watkow

namespace GniazdaClient 
{
    public partial class Form1 : Form
    {
        Socket gniazdo; //gniazdo na serwer, nowy obiekt klasy Socket
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e) //przycisk POŁACZ
        {
            try
            {
                IPAddress ipadddress = IPAddress.Parse(textBox2.Text); //przypisuje nr IP z textboxa2
                int port = int.Parse(textBox3.Text); //konwertuje nr portu z textBoxa3
                IPEndPoint localEP = new IPEndPoint(ipadddress, port); //inicjeuje wartości z textboxow 2 i 3 do localendpointa
                gniazdo = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //przypisuje do gniazda konstruktor i podaje podstawowe parametry
                gniazdo.Connect(localEP); //probuje polaczyc z serwerem, jak sie uda to przypisuje go do obiektu gniazdo
                if (gniazdo.Connected) // instrukcja warunkowa jesli uda sie polaczyc z gniazdem
                {
                    MessageBox.Show("Połączono z serwerem!", "POŁĄCZONO");
                    button2.Enabled = false; //wylaczam przycisk POLACZ
                    button4.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    Thread t = new Thread(this.odbierz); //przekazuje do nowego watku metode odbierz, 
                    t.Start(); //rozpoczynam watek i tym samym rozpoczynam metode odbierz
                }
            }
            catch(System.FormatException)
            {
                MessageBox.Show("Nie mozna nawiazac polaczenia", "Nie mozna nawiazac polaczenia!");
            }
            catch (System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("Numer portu jest nieprawidłowy", "Nieprawidlowy numer portu!");
            }

        }

        public void odbierz() //metoda inicjowana w watku t
        { 
            while (true) //petla nieskonczona do obierania wiadomosci
            {
                try
                {
                    byte[] bytes = new Byte[1024]; //inicjuje tablice bajtow z wartoscia int
                    int counter = gniazdo.Receive(bytes); //przypisuje zmienna int do tego co wyslal serwer 
                    string data = Encoding.ASCII.GetString(bytes, 0, counter); //przypisuje do zmiennej string, zdekodowana wiadomosc
                    richTextBox1.AppendText("Serwer: " + data + "\n"); //wyswietlam wiadomosc w richTextboxie
                }
                catch (System.Net.Sockets.SocketException)
                {
                }
                catch (System.ObjectDisposedException)
                {
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) //przycisk WYSLIJ
        {
            try
            {
                richTextBox1.AppendText("Klient: " + textBox1.Text + "\n");
                byte[] msg = Encoding.ASCII.GetBytes(textBox1.Text); // przypisuje to co w textBox1  do byte 
                gniazdo.Send(msg); //metoda wysyla wiadomosc w bajtach do gniazdo
                textBox1.Text = "";
            }
            catch (System.Net.Sockets.SocketException)
            {
                MessageBox.Show("Zerwano połączenie z serwerem");
            }
        }

        private void button3_Click(object sender, EventArgs e) //prszycisk WYJSCIE
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e) //przycisk DOMYSLNE
        {
            textBox2.Text = "127.0.0.1";
            textBox3.Text = "11000";
        }
    }
}
