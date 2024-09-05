using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

/*
    Requerimiento 1: Sobrecargar el constructor Lexico para que reciba como
                     argumento el nombre del archvo a compilar
    Requerimiento 2: Tener un contador de lineas    (DONE)
    Requerimiento 3: Agregar OperadorRelacional:
                     ==,>,>=,<,<=,<>,!=           (DONE)
    Requerimiento 4: Agregar OperadorLogico
                     &&,||,!                        (DONE)
*/
namespace Lexico1
{
    public class Lexico : Token, IDisposable
    {
        StreamReader archivo;
        StreamWriter log;
        StreamWriter asm;
        int linea = 0;
        public Lexico()
        {
            log = new StreamWriter("prueba.log");
            asm = new StreamWriter("prueba.asm");
            log.AutoFlush = true;
            asm.AutoFlush = true;
            if (File.Exists("prueba.cpp"))
            {
                archivo = new StreamReader("prueba.cpp");
            }
            else
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
        }
        /*
        public Lexico(string nombre)
        {
                Si nombre = suma.cpp
                LOG = suma.log
                ASM = suma.asm
                Y validar la extension del nombre del archivo
        }
        */
        public void Dispose()
        {
            archivo.Close();
            log.Close();
            asm.Close();
        }
        public void nextToken()
        {
            char c;
            string buffer = "";

            while (char.IsWhiteSpace(c = (char)archivo.Read()))
            {
                {
                    if (c == '\n')
                        linea++;
                }
            }
            buffer += c;
            if (char.IsLetter(c))
            {
                setClasificacion(Tipos.Identificador);
                while (char.IsLetterOrDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }
            }

            else if (char.IsDigit(c))
            {
                setClasificacion(Tipos.Numero);
                while (char.IsDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == ';')
            {
                setClasificacion(Tipos.FinSentencia);
            }
            else if (c == '{')
            {
                setClasificacion(Tipos.InicioBloque);
            }
            else if (c == '}')
            {
                setClasificacion(Tipos.FinBloque);
            }
            else if (c == '?')
            {
                setClasificacion(Tipos.OperadorTernario);
            }
            else if (c == '=')
            {
                setClasificacion(Tipos.Asignacion);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }

            }
            else if (c == '>')
            {
                setClasificacion(Tipos.MayorQue);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }

            }
            else if (c == '<')
            {
                setClasificacion(Tipos.MenorQue);
                if ((c = (char)archivo.Peek()) == '=' || c == '>')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }

            }
            else if (c == '!')
            {
                setClasificacion(Tipos.OperadorLogico);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }

            }

            else if (c == '&')
            {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '&')
                {
                    setClasificacion(Tipos.OperadorLogico);
                    buffer += c;
                    archivo.Read();
                }

            }
            else if (c == '|')
            {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '|')
                {
                    setClasificacion(Tipos.OperadorLogico);
                    buffer += c;
                    archivo.Read();
                }

            }
            else if (c == '+')
            {
                setClasificacion(Tipos.OperadorTermino);
                if ((c = (char)archivo.Peek()) == '+' || c == '=')
                {
                    setClasificacion(Tipos.IncrementoTermino);
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '-')
            {
                setClasificacion(Tipos.OperadorTermino);
                if ((c = (char)archivo.Peek()) == '-' || (c == '='))
                {
                    setClasificacion(Tipos.IncrementoTermino);
                    buffer += c;
                    archivo.Read();
                }
                else if (c == '>')
                {
                    setClasificacion(Tipos.Puntero);
                    buffer += c;
                    archivo.Read();
                }
            }
            else if (c == '*' || c == '/' || c == '%')
            {
                setClasificacion(Tipos.OperadorFactor);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.IncrementoFactor);
                    buffer += c;
                    archivo.Read();
                }
            }
            else
            {
                setClasificacion(Tipos.Caracter);
                Console.WriteLine("" + linea);
            }
            if (!finArchivo())
            {
                setContenido(buffer);
                log.WriteLine(getContenido() + " = " + getClasificacion());
            }
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}