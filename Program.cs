using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lexico l = new Lexico())
                {
                    while (!l.finArchivo())
                    {
                        l.nextToken();
                    }
                    l.contadorLineas();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }
}
