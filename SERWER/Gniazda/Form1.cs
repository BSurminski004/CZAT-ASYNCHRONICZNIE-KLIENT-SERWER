using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets; //biblioteka do gniazd
using System.Net;
using System.Threading; //biblio da watkow
using System.Windows.Forms;

namespace Gniazda
{
    public partial class Form1 : Form

    { 
        Socket listener; //nowy obiekt klasy gniazdo, do nasluchiwania polaczenia
        Socket ClientSock; //nowy obiekt typu gniazdo, ktorym bedzie klient ktory sie polaczy
        public Form1()
        {
            InitializeComponent();
            IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000); //ustawiam IP i PORT serwera
             listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp); //ustawiam parametry dla obiektu listener, czyli jaka siec, jaki rodzaj gniazda i protokol
            listener.Bind(localEP); //lacze gniazdo listened z localEP
            listener.Listen(10); //ustawia gniazdo listener w nasluchiwanie 
            Thread t = new Thread(this.acpS); //tworzy nowy watek (obiekt klasy Thread) i ktory rozpoczyna metode acpS (przekazuje metode do watku)
            t.Start(); //rozpoczynam dzialanie watku z metoda acpS

        }

        public void acpS() //metoda przekazana do watku t, ktora odpowiada za komunikacje miedzy klientem a serwerem
        {
            while (true) //petla nieskonczona
            {
                ClientSock = listener.Accept(); //za obiekt ClientSock przyjmuje to gniazdo z ktorym sie polacze
                Thread t = new Thread(odbierz); //tworze nowy watek wewnatrz metody, ktory 
                t.Start(); //rozpoczynam dzialanie watku czyli zaczynam metode odbierz
            }
        }

        private void button1_Click(object sender, EventArgs e) //przycisk WYSLIJ WIADOMOSC
        {
            try
            {
                richTextBox1.AppendText("Serwer: " + textBox1.Text + "\n");
                byte[] msg = Encoding.ASCII.GetBytes(textBox1.Text); //przekazuje to tablicy msg zakodowana w ASCII wiadomosc z textBoxa
                ClientSock.Send(msg); //przekazuje zakodowana tablice znakow ASCII do gniazda klienta, metoda Send zwraca liczbe bajtow
                textBox1.Text = "";
            }
            catch (System.ObjectDisposedException)
            {
                MessageBox.Show("Zerwano polaczenie z klientem, wysyłanie nie dostępne", "Brak polaczenia z klientem");
            }
            catch (System.NullReferenceException)
            {
                MessageBox.Show("Zerwano polaczenie z serwerem, wysyłanie nie dostępne", "Brak polaczenia z serwerem");
            }
        }

        public void odbierz() //metoda wywolywana w watku t w metodze acpS
        {
            while (true) //petla nieskonczona, aby w nieskonczonosc wysylac i odbierac wiadomosc
            {
                try
                {
                    byte[] bytes = new Byte[1024]; //tablica bajtow reprezentujaca 32bitowy int
                    int counter = ClientSock.Receive(bytes);  //zmienna int do ktorej przypisuje to co wyslal klient i przypisuje to do tablicy bytes
                    string data = Encoding.ASCII.GetString(bytes, 0, counter); //zamieniam kod ASCII na ciag znakow (string) i przypisuje do zmiennej data
                    richTextBox1.AppendText("Klient: " + data + "\n");  //wyswietlam to co dostalem od klienta w richTextBoxie
                }
                catch (System.Net.Sockets.SocketException)
                {
                    //MessageBox.Show("Zerwano połączenie z klientem");
                }
                catch (System.ObjectDisposedException)
                {
                    //MessageBox.Show("Zerwano połączenie z klientem");
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e) //nic nie robi
        {

        }

        private void Form1_Load(object sender, EventArgs e) //nic nie robi
        {

        }

        private void button2_Click(object sender, EventArgs e) //przycisk WYJSCIE
        {
            this.Close();
        }
    }
}
