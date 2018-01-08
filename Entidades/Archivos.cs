using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Archivos
    {
        public int id { get; set; }
        public Usuarios usuarios { get; set; }
        public string nombre { get; set; }
        public string fecha { get; set; }
        public string ubicacion { get; set; }
        public Archivos()
        {
            usuarios = new Usuarios();
        }
        
    }
}
