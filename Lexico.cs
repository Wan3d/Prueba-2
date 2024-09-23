using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace Lexico1
{
    public class Lexico : Token, IDisposable
    {
        public StreamReader archivo;
        public StreamWriter log;
        public StreamWriter asm;

        public int linea = 1;

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

        public Lexico(string nombreArchivo)
        {
            string nombreArchivoWithoutExt = Path.GetFileNameWithoutExtension(nombreArchivo);   /* Obtenemos el nombre del archivo sin la extensión para poder crear el .log y .asm */
            if (File.Exists(nombreArchivo))
            {
                log = new StreamWriter(nombreArchivoWithoutExt + ".log");
                asm = new StreamWriter(nombreArchivoWithoutExt + ".asm");
                log.AutoFlush = true;
                asm.AutoFlush = true;
                archivo = new StreamReader(nombreArchivo);
            }
            else if (Path.GetExtension(nombreArchivo) != ".cpp")
            {
                throw new ArgumentException("El archivo debe ser de extensión .cpp");
            }
            else
            {
                throw new FileNotFoundException("La extensión " + Path.GetExtension(nombreArchivo) + " no existe");    /* Defino una excepción que indica que existe un error con el archivo en caso de no ser encontrado */
            }
        }
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
                if (c == '\n')
                {
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
            /* Clasificaciones de números / U-II */
            /* ---------------------------------------------- */
            else if (char.IsDigit(c))
            {
                setClasificacion(Tipos.Numero);
                if ((c = (char)archivo.Peek()) == '.')
                {
                    buffer += c;
                    archivo.Read();
                    if (!char.IsDigit(c = (char)archivo.Peek()))
                    {
                        throw new Error("léxico", log, linea);
                    }
                    while (char.IsDigit(c = (char)archivo.Peek()))
                    {
                        buffer += c;
                        archivo.Read();
                    }
                    if ((c = (char)archivo.Peek()) == 'e' || (c = (char)archivo.Peek()) == 'E')
                    {
                        buffer += c;
                        archivo.Read();
                        if ((c = (char)archivo.Peek()) == '-' || (c = (char)archivo.Peek()) == '+')
                        {
                            buffer += c;
                            archivo.Read();
                            if (!char.IsDigit(c = (char)archivo.Peek()))
                            {
                                throw new Error("léxico", log, linea);
                            }
                            while (char.IsDigit(c = (char)archivo.Peek()))
                            {
                                buffer += c;
                                archivo.Read();
                            }
                        }
                        else if (char.IsDigit(c))
                        {
                            while (char.IsDigit(c = (char)archivo.Peek()))
                            {
                                buffer += c;
                                archivo.Read();
                            }
                        }
                        else
                        {
                            throw new Error("léxico", log, linea);
                        }
                    }
                    else
                    {
                        throw new Error("léxico", log, linea);
                    }
                }
                while (char.IsDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                    if ((c = (char)archivo.Peek()) == 'e' || (c = (char)archivo.Peek()) == 'E')
                    {
                        buffer += c;
                        archivo.Read();
                        if ((c = (char)archivo.Peek()) == '-' || (c = (char)archivo.Peek()) == '+')
                        {
                            buffer += c;
                            archivo.Read();
                            if (!char.IsDigit(c = (char)archivo.Peek()))
                            {
                                throw new Error("léxico", log, linea);
                            }
                            while (char.IsDigit(c = (char)archivo.Peek()))
                            {
                                buffer += c;
                                archivo.Read();
                            }
                        }
                        else if (char.IsDigit(c))
                        {
                            while (char.IsDigit(c = (char)archivo.Peek()))
                            {
                                buffer += c;
                                archivo.Read();
                            }
                        }
                        else
                        {
                            throw new Error("léxico", log, linea);
                        }
                    }
                    if ((c = (char)archivo.Peek()) == '.')
                    {
                        buffer += c;
                        archivo.Read();
                        if (!char.IsDigit(c = (char)archivo.Peek()))
                        {
                            throw new Error("léxico", log, linea);
                        }
                        while (char.IsDigit(c = (char)archivo.Peek()))
                        {
                            buffer += c;
                            archivo.Read();
                        }
                        if ((c = (char)archivo.Peek()) == 'e' || (c = (char)archivo.Peek()) == 'E')
                        {
                            buffer += c;
                            archivo.Read();
                            if ((c = (char)archivo.Peek()) == '-' || (c = (char)archivo.Peek()) == '+')
                            {
                                buffer += c;
                                archivo.Read();
                                if (!char.IsDigit(c = (char)archivo.Peek()))
                                {
                                    throw new Error("léxico", log, linea);
                                }
                                while (char.IsDigit(c = (char)archivo.Peek()))
                                {
                                    buffer += c;
                                    archivo.Read();
                                }
                            }
                            else if (char.IsDigit(c))
                            {
                                while (char.IsDigit(c = (char)archivo.Peek()))
                                {
                                    buffer += c;
                                    archivo.Read();
                                }
                            }
                            else
                            {
                                throw new Error("léxico", log, linea);
                            }
                        }
                    }
                }
            }
            /* ---------------------------------------------- */
            /* Clasificaciones de cadena / U-II */
            else if (c == '"')
            {
                setClasificacion(Tipos.Cadena);
                while ((c = (char)archivo.Peek()) != '"' && (c = (char)archivo.Peek()) != '\n')
                {
                    buffer += c;
                    archivo.Read();
                    if ((c = (char)archivo.Peek()) == '\n')
                    {
                        throw new Error("léxico", log, linea);
                    } /* Lanza una excepción si llega al final de la línea sin leer ninguna comilla doble */
                }
                buffer += c;
                archivo.Read();
            }
            /* Clasificaciones carácter */
            else if (c == '\'') 
            {
                setClasificacion(Tipos.Caracter);
                if ((c = (char)archivo.Peek()) == '\'')
                {
                    buffer += c;
                    archivo.Read();
                    if ((c = (char)archivo.Peek()) == '\'')
                    {
                        buffer += c;
                        archivo.Read();
                    }
                    else if ((c = (char)archivo.Peek()) != '\'')
                    {
                        throw new Error("léxico", log, linea);
                    }
                }
                else if (char.IsWhiteSpace(c = (char)archivo.Peek()))
                {
                    throw new Error("léxico", log, linea);
                }
                else if (char.IsLetterOrDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                    if ((c = (char)archivo.Peek()) == '\'')
                    {
                        buffer += c;
                        archivo.Read();
                    }
                    else if ((c = (char)archivo.Peek()) != '\'')
                    {
                        throw new Error("léxico", log, linea);
                    }
                }
                else if ((c = (char)archivo.Peek()) == '@')
                {
                    buffer += c;
                    archivo.Read();
                    if ((c = (char)archivo.Peek()) == '\'')
                    {
                        buffer += c;
                        archivo.Read();
                    }
                    else if ((c = (char)archivo.Peek()) != '\'')
                    {
                        throw new Error("léxico", log, linea);
                    }
                }
            }
            else if (c == '#')
            {
                setClasificacion(Tipos.Caracter);
                while (char.IsDigit(c = (char)archivo.Peek()))
                {
                    buffer += c;
                    archivo.Read();
                }
            }
            /* -------------------------------------------------------------- */
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
                setClasificacion(Tipos.OperadorRelacional);
                if ((c = (char)archivo.Peek()) == '=')
                {
                    setClasificacion(Tipos.OperadorRelacional);
                    buffer += c;
                    archivo.Read();
                }

            }
            else if (c == '<')
            {
                setClasificacion(Tipos.OperadorRelacional);
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