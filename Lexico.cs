using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

/*
    Requerimiento 1: Sobrecargar el constructor Lexico para que reciba como
                     argumento el nombre del archvo a compilar (Generar un segundo constructor que reciba como parametro el archivo que quiero compilar y crear los dos archivos .asm y .log)
                     (Googlear "Como obtener la extension de un archivo. WithoutExtension") (DONE)
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
        StreamReader archivo, archivo2;
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
            if (File.Exists("Lexico.cs"))
            {
                archivo2 = new StreamReader("Lexico.cs");
            }
            else
            {
                throw new Error("El archivo Lexico.cs no existe", log);
            }
        }

        public Lexico(string nombreArchivo)
        {
            string nombreArchivoWithoutExt = Path.GetFileNameWithoutExtension(nombreArchivo);   /* Obtenemos el nombre del archivo sin la extensión para poder crear el .log y .asm */
            if (File.Exists(nombreArchivo))
            {
                log = new StreamWriter(nombreArchivoWithoutExt + ".log");
                asm = new StreamWriter(nombreArchivoWithoutExt + ".asm");
                archivo = new StreamReader(nombreArchivo);
            }
            else
            {
                throw new FileNotFoundException("El archivo " + nombreArchivo + " no existe");    /* Defino una excepción que indica que existe un error con el archivo en caso de no ser encontrado */
            }
        }

        public void contadorLineas()
        {
            using (StreamReader archivo2 = new StreamReader("Lexico.cs"))
            {
                char c;
                linea = 1;
                while (!archivo2.EndOfStream)  /* Lee el archivo mientras no se cierre*/
                {
                    c = (char)archivo2.Read();      /* Lee caracter por caracter */
                    if (c == '\n')                  /* Si reconoce un salto de linea, se suma al contador*/
                    {
                        linea++;
                    }
                }
            }
            /* Console.WriteLine("Número de líneas: " + linea); */
            log.WriteLine("\nNúmero de líneas del archivo Lexico.cs = " + linea);
        }
        public void Dispose()
        {
            archivo2.Close();
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

            else if (c == '$')
            {
                setClasificacion(Tipos.Caracter);
                while (char.IsDigit(c = (char)archivo.Peek()))
                {
                    setClasificacion(Tipos.Moneda);
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