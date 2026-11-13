using SistemaRegistroPacientes.Data;
using SistemaRegistroPacientes.Models;
using SistemaRegistroPacientes.Repositories;

namespace SistemaRegistroPacientes;

internal class Program
{
    private static readonly PatientRepository Repository = new(DatabaseConnection.ConnectionString);

    private static void Main()
    {
        Console.Title = "Sistema de Registro de Pacientes";

        try
        {
            Repository.TestConnection();
            Console.WriteLine("Conexión con SQL Server establecida correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("No fue posible conectar con la base de datos.");
            Console.WriteLine($"Detalle: {ex.Message}");
            Console.WriteLine("\nVerifica el nombre del servidor y que SQL Server esté iniciado.");
            Console.WriteLine("Presiona una tecla para cerrar...");
            Console.ReadKey();
            return;
        }

        bool continuar = true;

        while (continuar)
        {
            Console.Clear();
            MostrarMenu();
            Console.Write("Selecciona una opción: ");
            string? opcion = Console.ReadLine();

            try
            {
                switch (opcion)
                {
                    case "1":
                        RegistrarPaciente();
                        break;
                    case "2":
                        Repository.ListAll().ForEach(MostrarPaciente);
                        Pausar();
                        break;
                    case "3":
                        BuscarPaciente();
                        break;
                    case "4":
                        ActualizarPaciente();
                        break;
                    case "5":
                        EliminarPaciente();
                        break;
                    case "0":
                        continuar = false;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intenta nuevamente.");
                        Pausar();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nOcurrió un error: {ex.Message}");
                Pausar();
            }
        }

        Console.WriteLine("Gracias por utilizar el sistema.");
    }

    private static void MostrarMenu()
    {
        Console.WriteLine("======================================");
        Console.WriteLine("       REGISTRO DE PACIENTES");
        Console.WriteLine("======================================");
        Console.WriteLine("1. Registrar paciente");
        Console.WriteLine("2. Mostrar todos los pacientes");
        Console.WriteLine("3. Buscar paciente por correo");
        Console.WriteLine("4. Actualizar paciente");
        Console.WriteLine("5. Eliminar paciente");
        Console.WriteLine("0. Salir");
        Console.WriteLine("======================================");
    }

    private static void RegistrarPaciente()
    {
        Console.Clear();
        Console.WriteLine("--- REGISTRAR PACIENTE ---");

        string nombre = LeerObligatorio("Nombre completo: ");
        string email = LeerEmail("Correo electrónico: ");
        string telefono = LeerObligatorio("Teléfono: ");
        DateTime nacimiento = LeerFecha("Fecha de nacimiento (dd/mm/aaaa): ");
        string direccion = LeerObligatorio("Dirección: ");

        var paciente = new Patient
        {
            FullName = nombre,
            Email = email,
            Phone = telefono,
            BirthDate = nacimiento,
            Address = direccion
        };

        Repository.Add(paciente);
        Console.WriteLine("\nPaciente registrado correctamente.");
        Pausar();
    }

    private static void BuscarPaciente()
    {
        Console.Clear();
        Console.WriteLine("--- BUSCAR PACIENTE ---");
        string email = LeerEmail("Correo electrónico: ");

        Patient? paciente = Repository.GetByEmail(email);

        if (paciente is null)
            Console.WriteLine("No se encontró un paciente con ese correo.");
        else
            MostrarPaciente(paciente);

        Pausar();
    }

    private static void ActualizarPaciente()
    {
        Console.Clear();
        Console.WriteLine("--- ACTUALIZAR PACIENTE ---");
        string email = LeerEmail("Correo del paciente que deseas modificar: ");

        Patient? paciente = Repository.GetByEmail(email);

        if (paciente is null)
        {
            Console.WriteLine("No se encontró el paciente.");
            Pausar();
            return;
        }

        MostrarPaciente(paciente);
        Console.WriteLine("\nEscribe los nuevos datos. Deja un campo vacío para conservar el valor actual.");

        string? nombre = LeerOpcional($"Nombre [{paciente.FullName}]: ");
        string? telefono = LeerOpcional($"Teléfono [{paciente.Phone}]: ");
        string? direccion = LeerOpcional($"Dirección [{paciente.Address}]: ");
        DateTime? nacimiento = LeerFechaOpcional($"Fecha de nacimiento [{paciente.BirthDate:dd/MM/yyyy}]: ");

        if (!string.IsNullOrWhiteSpace(nombre)) paciente.FullName = nombre;
        if (!string.IsNullOrWhiteSpace(telefono)) paciente.Phone = telefono;
        if (!string.IsNullOrWhiteSpace(direccion)) paciente.Address = direccion;
        if (nacimiento.HasValue) paciente.BirthDate = nacimiento.Value;

        bool actualizado = Repository.Update(paciente);
        Console.WriteLine(actualizado ? "\nDatos actualizados correctamente." : "\nNo se realizaron cambios.");
        Pausar();
    }

    private static void EliminarPaciente()
    {
        Console.Clear();
        Console.WriteLine("--- ELIMINAR PACIENTE ---");
        string email = LeerEmail("Correo del paciente: ");

        Patient? paciente = Repository.GetByEmail(email);

        if (paciente is null)
        {
            Console.WriteLine("No se encontró el paciente.");
            Pausar();
            return;
        }

        MostrarPaciente(paciente);
        Console.Write("¿Seguro que deseas eliminar este registro? (S/N): ");
        string? confirmacion = Console.ReadLine();

        if (string.Equals(confirmacion, "S", StringComparison.OrdinalIgnoreCase))
        {
            bool eliminado = Repository.Delete(paciente.ContactId);
            Console.WriteLine(eliminado ? "Registro eliminado." : "No se pudo eliminar el registro.");
        }
        else
        {
            Console.WriteLine("Operación cancelada.");
        }

        Pausar();
    }

    private static void MostrarPaciente(Patient paciente)
    {
        Console.WriteLine("--------------------------------------");
        Console.WriteLine($"Código:       {paciente.ContactId}");
        Console.WriteLine($"Nombre:       {paciente.FullName}");
        Console.WriteLine($"Correo:       {paciente.Email}");
        Console.WriteLine($"Teléfono:     {paciente.Phone}");
        Console.WriteLine($"Nacimiento:   {paciente.BirthDate:dd/MM/yyyy}");
        Console.WriteLine($"Dirección:    {paciente.Address}");
        Console.WriteLine($"Registrado:   {paciente.CreatedAt:dd/MM/yyyy HH:mm}");
    }

    private static string LeerObligatorio(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string? valor = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(valor))
                return valor;

            Console.WriteLine("Este campo es obligatorio.");
        }
    }

    private static string LeerEmail(string mensaje)
    {
        while (true)
        {
            string email = LeerObligatorio(mensaje);

            try
            {
                var direccion = new System.Net.Mail.MailAddress(email);
                if (direccion.Address == email)
                    return email;
            }
            catch (FormatException) { }

            Console.WriteLine("Escribe un correo electrónico válido.");
        }
    }

    private static DateTime LeerFecha(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            if (DateTime.TryParse(Console.ReadLine(), out DateTime fecha) &&
                fecha.Date <= DateTime.Today &&
                fecha.Year >= 1900)
                return fecha.Date;

            Console.WriteLine("Fecha no válida. Usa una fecha real entre 1900 y hoy.");
        }
    }

    private static DateTime? LeerFechaOpcional(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string? texto = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(texto))
                return null;

            if (DateTime.TryParse(texto, out DateTime fecha) &&
                fecha.Date <= DateTime.Today &&
                fecha.Year >= 1900)
                return fecha.Date;

            Console.WriteLine("Fecha no válida. Intenta nuevamente.");
        }
    }

    private static string? LeerOpcional(string mensaje)
    {
        Console.Write(mensaje);
        string? valor = Console.ReadLine()?.Trim();
        return string.IsNullOrWhiteSpace(valor) ? null : valor;
    }

    private static void Pausar()
    {
        Console.WriteLine("\nPresiona una tecla para continuar...");
        Console.ReadKey();
    }
}
