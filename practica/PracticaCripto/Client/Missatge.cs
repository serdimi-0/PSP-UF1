using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Missatge
    {
        private string usuari;
        private string text;
        private DateTime hora;

        public Missatge(string usuari, string text)
        {
            Usuari = usuari;
            Text = text;
            Hora = DateTime.Now;
        }

        public string Usuari { get => usuari; set => usuari = value; }
        public string Text { get => text; set => text = value; }
        public DateTime Hora { get => hora; set => hora = value; }

        public string HoraString
        {
            get
            {
                return Hora.ToString("HH:mm:ss");
            }
        }
    }
}
