using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Data.SqlClient;


namespace biblioteca
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=127.0.0.1;Port=3306;Database=biblioteca;Uid=root;Pwd=;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Conexion a la base de datos");

                    // tabla usuarios
                    string createUsuariosTable = @"
                   CREATE TABLE IF NOT EXISTS usuarios (
                   id INT NOT NULL AUTO_INCREMENT,
                   nombre VARCHAR(25) NOT NULL,
                   apellido VARCHAR(25) NOT NULL,
                   dni VARCHAR(10) NOT NULL UNIQUE,
                   telefono VARCHAR(25) NOT NULL UNIQUE,
                   email VARCHAR(50) NOT NULL UNIQUE,
                   creado_el TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                   actualizado_el TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                   estado TINYINT DEFAULT 1,
                   PRIMARY KEY (id));
                ";
                    ExecuteQuery(connection, createUsuariosTable);

                    // tabla generos
                    string createGenerosTable = @"
                    CREATE TABLE IF NOT EXISTS generos (
                    id INT NOT NULL AUTO_INCREMENT,
                    genero VARCHAR(25) NOT NULL UNIQUE,
                    PRIMARY KEY (id));
                ";
                    ExecuteQuery(connection, createGenerosTable);

                    // tabla libros
                    string createLibrosTable = @"
                    CREATE TABLE IF NOT EXISTS libros (
                    id INT NOT NULL AUTO_INCREMENT,
                    nombre_libro VARCHAR(25) NOT NULL,
                    autor VARCHAR(25) NOT NULL,
                    fecha_lanzamiento DATE DEFAULT NULL,
                    creado_el TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    actualizado_el TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                    estado TINYINT DEFAULT 1,
                    id_genero INT NOT NULL,
                    CONSTRAINT fk_genero FOREIGN KEY (id_genero) REFERENCES generos(id),
                    PRIMARY KEY (id));
                ";
                    ExecuteQuery(connection, createLibrosTable);

                    // tabla prestamos
                    string createPrestamosTable = @"
                   CREATE TABLE IF NOT EXISTS prestamos (
                   id INT NOT NULL AUTO_INCREMENT,
                   fecha_prestamo DATE DEFAULT NULL,
                   fecha_devolucion_estimada DATE DEFAULT NULL,
                   fecha_devolucion_real DATE DEFAULT NULL,
                   id_usuarios INT NOT NULL,
                   id_libros INT NOT NULL,
                   CONSTRAINT fk_usuarios FOREIGN KEY(id_usuarios) REFERENCES usuarios(id),
                   CONSTRAINT fk_libros FOREIGN KEY(id_libros) REFERENCES libros(id),
                   PRIMARY KEY(id));
                                    ";
                    ExecuteQuery(connection, createPrestamosTable);

                    Console.WriteLine("Tablas creadas exitosamente.");
                    connection.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR" + ex.Message);
                    connection.Close();
                }
            }

            int opcion = 0;
            while (opcion >= 0 && opcion < 16)
            {
                Console.Clear();
                {

                    Console.WriteLine("<------------ BIBLIOTECA ------------>");
                    Console.WriteLine("1. Crear un nuevo usuario");
                    Console.WriteLine("2. Actualizar un usuario");
                    Console.WriteLine("3. Borrar un usuario (cambia el estado de 1 a 0)");
                    Console.WriteLine("4. Agregar un libro");
                    Console.WriteLine("5. Actualizar un libro");
                    Console.WriteLine("6. Borrar un libro (cambia el estado de 1 a 0)");
                    Console.WriteLine("7. Agregar un genero");
                    Console.WriteLine("8. Actualizar un genero");
                    Console.WriteLine("9. Crear un prestamo");
                    Console.WriteLine("10. Actualizar un prestamo");
                    Console.WriteLine("11. Listar usuarios");
                    Console.WriteLine("12. Listar libros");
                    Console.WriteLine("13. Listar prestamos");
                    Console.WriteLine("14. Listar generos");
                    Console.WriteLine("15. Salir");
                    Console.WriteLine("Seleccione una opcion : ");

                    string entrada = Console.ReadLine();

                    if (int.TryParse(entrada, out opcion))
                    {
                        Console.WriteLine("Elije una opcion valida entre el 1 y el 15");
                    }
                    else
                    {
                        Console.WriteLine("ERROR Ingresaste una letra.");
                    }

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();


                        switch (opcion)
                        {
                            case 1:
                                Console.Clear();
                                CrearUsuario(connection);
                                Console.ReadKey();
                                break;
                            case 2:
                                Console.Clear();
                                ActualizarUsuario(connection);
                                Console.ReadKey();
                                break;
                            case 3:
                                Console.Clear();
                                 BorrarUsuario(connection);
                                Console.ReadKey();
                                break;
                            case 4:
                                Console.Clear();
                                CrearLibro(connection);
                                Console.ReadKey();
                                break;
                            case 5:
                                Console.Clear();
                                ActualizarLibro(connection);
                                Console.ReadKey();
                                break;
                            case 6:
                                Console.Clear();
                                BorrarLibro(connection);
                                Console.ReadKey();
                                break;
                            case 7:
                                Console.Clear();
                                CrearGenero(connection);
                                Console.ReadKey();
                                break;
                            case 8:
                                Console.Clear();
                                ActualizarGenero(connection);
                                Console.ReadKey();
                                break;
                            case 9:
                                Console.Clear();
                                CrearPrestamo(connection);
                                Console.ReadKey();
                                break;
                            case 10:
                                Console.Clear();
                                ActualizarPrestamo(connection);
                                Console.ReadKey();
                                break;
                            case 11:
                                Console.Clear();
                                ListarUsuario(connection);
                                Console.ReadKey();
                                break;
                            case 12:
                                Console.Clear();
                                ListarLibro(connection);
                                Console.ReadKey();
                                break;
                            case 13:
                                Console.Clear();
                                ListarPrestamo(connection);
                                Console.ReadKey();
                                break;
                            case 14:
                                Console.Clear();
                                ListarGenero(connection);
                                Console.ReadKey();
                                break;
                            case 15:
                                Console.Clear();
                                Console.WriteLine("Cerraste la app");
                                Environment.Exit(0);
                                break;
                            default:
                                Console.WriteLine("Elije una opcion correcta. Toca una letra para continuar");
                                opcion = 0;
                                Console.ReadKey();
                                break;
                        }
                    }




                }
            }

            void ExecuteQuery(MySqlConnection connection, string query)
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }


            void CrearUsuario(MySqlConnection connection)
            {
                Console.Clear();

                Console.Write("Ingrese el nombre: ");
                string nombre = Console.ReadLine();

                Console.Write("Ingrese el apellido: ");
                string apellido = Console.ReadLine();

                Console.Write("Ingrese el DNI: ");
                string dni = Console.ReadLine();

                Console.Write("Ingrese el teléfono: ");
                string telefono = Console.ReadLine();

                Console.Write("Ingrese el email: ");
                string email = Console.ReadLine();


                string query = "INSERT INTO Usuarios (nombre, apellido, dni, telefono, email, estado) VALUES (@nombre, @apellido, @dni, @telefono, @email, 1)";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);

                    cmd.Parameters.AddWithValue("@apellido", apellido);

                    cmd.Parameters.AddWithValue("@dni", dni);

                    cmd.Parameters.AddWithValue("@telefono", telefono);

                    cmd.Parameters.AddWithValue("@email", email);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Usuario creado exitosamente.");
                    Console.Write("Toque una letra para continuar");
                }
            }

            void ActualizarUsuario(MySqlConnection connection)
            {
                Console.Clear();
                int id = 0;

                Console.Write("Ingrese el ID del usuario a actualizar (recuerde haberlo cargado previamente al usuario) : ");
                string entrada = Console.ReadLine();

                if (int.TryParse(entrada, out id))
                {
                    

                    Console.Write("Ingrese el nuevo nombre : ");
                    string nombre = Console.ReadLine();
                    Console.Write("Ingrese el nuevo apellido : ");
                    string apellido = Console.ReadLine();
                    Console.Write("Ingrese el nuevo DNI : ");
                    string dni = Console.ReadLine();
                    Console.Write("Ingrese el nuevo teléfono : ");
                    string telefono = Console.ReadLine();
                    Console.Write("Ingrese el nuevo email : ");
                    string email = Console.ReadLine();

                    string query = "UPDATE usuarios SET nombre = @nombre, apellido = @apellido, dni = @dni, telefono = @telefono, email = @email WHERE id = @id";


                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@apellido", apellido);

                        cmd.Parameters.AddWithValue("@dni", dni);
                        cmd.Parameters.AddWithValue("@telefono", telefono);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@id", id);

                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Usuario actualizado exitosamente.");
                        Console.Write("Toque una letra para continuar");

                    }
                }
                else
                {
                    Console.WriteLine("Insertaste una letra como ID. Toca una letra para continuar");
                    return;
                }
            }


            void BorrarUsuario(MySqlConnection connection)
            {
                Console.Clear();
                int usuarioId=0;

                Console.Write("Ingrese el ID del usuario a borrar (cambia el estado a 0) (recuerde haberlo cargado previamente al usuario): ");
                string entrada = Console.ReadLine();

                if (int.TryParse(entrada, out usuarioId))
                {
                    

                    string updateQuery = "UPDATE Usuarios SET estado=0 WHERE id=@usuarioId";
                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection))
                    {
                        updateCmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        updateCmd.ExecuteNonQuery();
                        Console.WriteLine("Usuario inactivado exitosamente. ");
                        Console.Write("Toque una letra para continuar");
                    }
                }
                else {
                    Console.WriteLine("Insertaste una letra como ID. Toca una letra para continuar");
                    return;
                }
            }

            void CrearLibro(MySqlConnection connection)
            {
                Console.Clear();
                int id = 0;

                Console.Write("Ingrese el nombre del libro: ");
                string nombreLibro = Console.ReadLine();
                Console.Write("Ingrese el autor del libro: ");
                string autor = Console.ReadLine();
                Console.Write("Ingrese la fecha de lanzamiento formato (aaaa-mm-dd)\nRecuerde colocar bien el formato de la fecha (anio-mes-dia) (EJ: 2005-12-05) : ");
                string fechaLanzamiento = Console.ReadLine();
                Console.Write("Ingrese el ID del genero al que pertenece el libro (recuerde haberlo cargado previamente al genero) : ");
                string idgenero = Console.ReadLine();

                int.TryParse(idgenero, out id);

                string query = "INSERT INTO libros (nombre_libro, autor, fecha_lanzamiento, id_genero, estado) VALUES (@nombreLibro, @autor, @fechaLanzamiento, @idgenero, 1)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@nombreLibro", nombreLibro);
                    cmd.Parameters.AddWithValue("@autor", autor);
                    cmd.Parameters.AddWithValue("@fechaLanzamiento", fechaLanzamiento);
                    cmd.Parameters.AddWithValue("@idgenero", idgenero);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Libro agregado exitosamente.");
                    Console.Write("Toque una letra para continuar");
                }
            }

            void ActualizarLibro(MySqlConnection connection)
            {
                Console.Clear();
                int id_libro = 0;
                int id_genero = 0;

                Console.Write("Ingrese el ID del libro a actualizar (recuerde haberlo cargado previamente al libro) : ");
                string entrada3 = Console.ReadLine();
                

                if (int.TryParse(entrada3, out id_libro))
                {
                    

                    Console.Write("Ingrese el nuevo nombre del libro: ");
                    string nombreLibro = Console.ReadLine();
                    Console.Write("Ingrese el autor del libro: ");
                    string autor = Console.ReadLine();
                    Console.Write("Ingrese la fecha de lanzamiento formato (aaaa-mm-dd)\nRecuerde colocar bien el formato de la fecha (anio-mes-dia) (EJ: 2005-12-05) : ");
                    string fechaLanzamiento = Console.ReadLine();
                    Console.Write("Ingrese el ID del genero al que pertenece el libro (recuerde haberlo cargado previamente al genero) : ");
                    string idgenero = Console.ReadLine();

                   if  (!int.TryParse(idgenero, out id_genero)){
                        Console.WriteLine("Pusiste una letra como ID genero");
                            return;
                    };

                    string query = "UPDATE libros SET nombre_libro = @nombreLibro, autor = @autor, fecha_lanzamiento = @fechaLanzamiento, id_genero = @id_genero WHERE id = @id_libro";


                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nombreLibro", nombreLibro);
                        cmd.Parameters.AddWithValue("@autor", autor);

                        cmd.Parameters.AddWithValue("@fechaLanzamiento", fechaLanzamiento);
                        cmd.Parameters.AddWithValue("@id_genero", id_genero);
                        cmd.Parameters.AddWithValue("@id_libro", @id_libro);

                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Libro actualizado exitosamente.");
                        Console.Write("Toque una letra para continuar");

                    }
                }
                else
                {
                    Console.WriteLine("Insertaste una letra como ID. Toca una letra para continuar");
                    return;
                }
            }

            void BorrarLibro(MySqlConnection connection)
            {
                Console.Clear();
                int libroId = 0;

                Console.Write("Ingrese el ID del libro a borrar (cambia el estado a 0) (recuerde haberlo cargado previamente al libro) : ");
                string entrada4 = Console.ReadLine();

                if (int.TryParse(entrada4, out libroId))
                {
                    ;

                    string updateQuery = "UPDATE libros SET estado=0 WHERE id=@libroId";
                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection))
                    {
                        updateCmd.Parameters.AddWithValue("@libroId", libroId);
                        updateCmd.ExecuteNonQuery();
                        Console.WriteLine("Libro inactivado exitosamente. ");
                        Console.Write("Toque una letra para continuar");
                    }
                }
                else
                {
                    Console.WriteLine("Insertaste una letra como ID. Toca una letra para continuar");
                    return;
                }
            }



            void CrearGenero(MySqlConnection connection)
            {
               
                Console.Clear();
                Console.WriteLine("Ingrese el nombre del género que desea agregar : ");
                string genero = Console.ReadLine();

                string query = "INSERT INTO generos (genero) VALUES (@genero)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@genero", genero);
                    cmd.ExecuteNonQuery();
                     Console.WriteLine("Genero cargado exitosamente");
                    Console.Write("Toque una letra para continuar");
                }
            }

            void ActualizarGenero(MySqlConnection connection)
            {
                Console.Clear();

                int id = 0;

                Console.Write("Ingrese el ID del genero que quiere actualizar (recuerde haberlo cargado previamente al genero): ");
                string entrada1 = Console.ReadLine();

                if (int.TryParse(entrada1, out id))
                {
                    

                    Console.Write("Ingrese el nuevo nombre del genero : ");
                    string nombre = Console.ReadLine();
                    

                    string query = "UPDATE generos SET genero = @nombre WHERE id = @id";


                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@id", id);
                         cmd.ExecuteNonQuery();
                        Console.WriteLine("Genero actualizado exitosamente.");
                        Console.Write("Toque una letra para continuar");
                    }
                }
                else
                {
                    Console.WriteLine("Insertaste una letra como ID. Toca una letra para continuar");
                    return;
                }
            }

            void CrearPrestamo(MySqlConnection connection)
            {
                Console.Clear();
                int idUsuario = 0;
                int idLibro = 0;

                Console.Write("Ingrese la fecha actual del prestamo (aaaa-bb-cc)\nRecuerde colocar bien el formato de la fecha (anio-mes-dia) (EJ: 2005-12-05) : ");
                string fechaPrestamo = Console.ReadLine();
                Console.Write("Ingrese la fecha de devolucion estimada (aaaa-bb-cc)\nRecuerde colocar bien el formato de la fecha (anio-mes-dia) (EJ: 2005-12-05) : ");
                string fechaDevolucionEstimada = Console.ReadLine();
                Console.Write("Ingrese la fecha de devolucion real (aaaa-bb-cc)\nRecuerde colocar bien el formato de la fecha (anio-mes-dia) (EJ: 2005-12-05) : ");
                string fechaDevolucionReal = Console.ReadLine();

                Console.Write("Ingrese el ID del usuario a quien correspondera el prestamo (recuerde haberlo cargado previamente al usuario) : ");
                string usuario = Console.ReadLine();
                int.TryParse(usuario, out idUsuario);
                Console.Write("Ingrese el ID del libro a quien correspondera el prestamo (recuerde haberlo cargado previamente al libro) : ");
                string libro = Console.ReadLine();
                int.TryParse(libro, out idLibro);

                
                string query = "INSERT INTO prestamos (fecha_prestamo, fecha_devolucion_estimada, fecha_devolucion_real, id_usuarios, id_libros) VALUES (@fechaPrestamo, @fechaDevolucionEstimada, @fechaDevolucionReal, @idUsuario, @idLibro)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@fechaPrestamo", fechaPrestamo);
                    cmd.Parameters.AddWithValue("@fechaDevolucionEstimada", fechaDevolucionEstimada);
                    cmd.Parameters.AddWithValue("@fechaDevolucionReal", fechaDevolucionReal);
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@idLibro", idLibro);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Prestamo agregado exitosamente.");
                    Console.Write("Toque una letra para continuar");
                }
            }


            void ActualizarPrestamo(MySqlConnection connection)
            {
                Console.Clear();
                int id_usuario = 0;
                int id_libro = 0;
                int id_prestamo = 0;

                Console.Write("Ingrese el ID del prestamo a actualizar (recuerde haberlo cargado previamente al prestamo) : ");
                string entrada5 = Console.ReadLine();


                if (int.TryParse(entrada5, out id_prestamo))
                {
                   

                    Console.Write("Ingrese la nueva fecha actual del prestamo (aaaa-bb-cc)\nRecuerde colocar bien el formato de la fecha (anio-mes-dia) (EJ: 2005-12-05) : ");
                    string nuevaFecha = Console.ReadLine();
                    Console.Write("Ingrese la nueva fecha de devolucion estimada (aaaa-bb-cc)\nRecuerde colocar bien el formato de la fecha (anio-mes-dia) (EJ: 2005-12-05) : ");
                    
                    string nuevaFechaDevolucionEstimada = Console.ReadLine();
                    Console.Write("Ingrese la nueva fecha de devolucion real (aaaa-bb-cc)\nRecuerde colocar bien el formato de la fecha (anio-mes-dia) (EJ: 2005-12-05) : ");
                    
                    string fechaDevolucionReal = Console.ReadLine();

                    Console.Write("Ingrese el ID del nuevo usuario (recuerde haberlo cargado previamente al usuario) : ");
                    string idusuario = Console.ReadLine();
                    int.TryParse(idusuario, out id_usuario);
                    Console.Write("Ingrese el ID del nuevo libro (recuerde haberlo cargado previamente al libro) : ");
                    string idlibro = Console.ReadLine();
                    int.TryParse(idlibro, out id_libro);

                    string query = "UPDATE prestamos SET fecha_prestamo = @nuevaFecha, fecha_devolucion_estimada = @nuevaFechaDevolucionEstimada, fecha_devolucion_real = @fechaDevolucionReal, id_usuarios = @id_usuario, id_libros = @id_libro  WHERE id = @id_prestamo";


                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nuevaFecha", nuevaFecha);
                        cmd.Parameters.AddWithValue("@nuevaFechaDevolucionEstimada", nuevaFechaDevolucionEstimada);
                        cmd.Parameters.AddWithValue("@fechaDevolucionReal", fechaDevolucionReal);
                        cmd.Parameters.AddWithValue("@id_usuario", id_usuario);
                        cmd.Parameters.AddWithValue("@id_libro", @id_libro);
                        cmd.Parameters.AddWithValue("@id_prestamo", @id_prestamo);


                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Prestamo actualizado exitosamente.");
                        Console.Write("Toque una letra para continuar");

                    }
                }
                else
                {
                    Console.WriteLine("Insertaste una letra como ID. Toca una letra para continuar");
                    return;
                }
            }

            void ListarUsuario(MySqlConnection connection)
            {
                
                    Console.Clear();
                    string query = "SELECT * FROM usuarios WHERE estado = 1";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("<------ Usuarios Activos ------>");
                            Console.WriteLine("ID  | Nombre Completo       | DNI        | Teléfono       | Email");
                            Console.WriteLine("-----------------------------------------------------------------------");

                            while (reader.Read())
                            {
                                
                                Console.WriteLine($"{reader["id"],-3} | {reader["nombre"]} {reader["apellido"],-18} | {reader["dni"],-10} | {reader["telefono"],-12} | {reader["email"]}");
                            }
                        }
                    }
                    Console.WriteLine("Estos son todos los usuarios actualmente ACTIVOS");
                Console.Write("Toque una letra para continuar");
            }

            void ListarLibro(MySqlConnection connection)
            {

                Console.Clear();
                string query = "SELECT * FROM libros WHERE estado = 1";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("<------ Libros Activos ------>");
                        Console.WriteLine("ID  | Titulo     | Autor     | Fecha de Lanzamiento     |");
                        Console.WriteLine("-----------------------------------------------------------------------");
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["id"]}   | {reader["nombre_libro"]}     | {reader["autor"]}     | {reader["fecha_lanzamiento"]}");
                        }
                    }
                }
                Console.WriteLine("Estos son todos los libros actualmente ACTIVOS");
                Console.Write("Toque una letra para continuar");
            }

            void ListarPrestamo(MySqlConnection connection)
            {
                Console.Clear();

                string query = @"
            SELECT 
                prestamos.id AS PrestamoID,
                usuarios.nombre AS UsuarioNombre,
                usuarios.apellido AS UsuarioApellido,
                libros.nombre_libro AS LibroTitulo,
                prestamos.fecha_prestamo,
                prestamos.fecha_devolucion_estimada,
                prestamos.fecha_devolucion_real
            FROM 
                prestamos
            JOIN 
                usuarios ON prestamos.id_usuarios = Usuarios.id
            JOIN 
                Libros ON prestamos.id_libros = Libros.id";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("<------ Prestamos ------>");
                        Console.WriteLine("ID  | Usuario               | Libro                | Fecha Préstamo    | Fecha Devolución Estimada | Fecha Devolución Real");
                        Console.WriteLine("--------------------------------------------------------------------------------------------------------");

                        while (reader.Read())
                        {
                            
                            Console.WriteLine($"{reader["PrestamoID"],-3} | {reader["UsuarioNombre"]} {reader["UsuarioApellido"],-18} | {reader["LibroTitulo"],-20} | {reader["fecha_prestamo"],-15} | {reader["fecha_devolucion_estimada"],-25} | {reader["fecha_devolucion_real"],-25}");
                        }
                    }
                }
                Console.WriteLine("Estos son todos los prestamos actuales");
                Console.Write("Toque una letra para continuar");
            }

            void ListarGenero(MySqlConnection connection)
            {
                Console.Clear();
                string query = "SELECT * FROM generos";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("<------ Generos ------>");
                        Console.WriteLine("ID  | Genero");
                        Console.WriteLine("---------------");

                        while (reader.Read())
                        {
                            
                            Console.WriteLine($"{reader["id"],-3} | {reader["genero"]}");
                        }
                    }
                }
                Console.WriteLine("Estos son todos los generos actuales");
                Console.Write("Toque una letra para continuar");
            }


        }
    }
}



