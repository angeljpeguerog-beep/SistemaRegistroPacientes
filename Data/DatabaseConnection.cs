namespace SistemaRegistroPacientes.Data;

public static class DatabaseConnection
{
    // Para SQL Server local con autenticación de Windows:
    // Si usas SQL Server Express, cambia Server=localhost por Server=localhost\\SQLEXPRESS.
    public static string ConnectionString =
        "Server=localhost;Database=ClinicaDB;Trusted_Connection=True;TrustServerCertificate=True;";
}
