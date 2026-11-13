using Microsoft.Data.SqlClient;
using SistemaRegistroPacientes.Models;

namespace SistemaRegistroPacientes.Repositories;

public class PatientRepository
{
    private readonly string _connectionString;

    public PatientRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void TestConnection()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
    }

    public void Add(Patient patient)
    {
        const string sql = @"
            INSERT INTO Contacts (FullName, Email, Phone, BirthDate, Address)
            VALUES (@FullName, @Email, @Phone, @BirthDate, @Address);";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        AddPatientParameters(command, patient);
        connection.Open();

        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            throw new InvalidOperationException("Ya existe un paciente registrado con ese correo.", ex);
        }
    }

    public List<Patient> ListAll()
    {
        const string sql = @"
            SELECT ContactId, FullName, Email, Phone, BirthDate, Address, CreatedAt
            FROM Contacts
            ORDER BY FullName;";

        var patients = new List<Patient>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
            patients.Add(MapPatient(reader));

        return patients;
    }

    public Patient? GetByEmail(string email)
    {
        const string sql = @"
            SELECT ContactId, FullName, Email, Phone, BirthDate, Address, CreatedAt
            FROM Contacts
            WHERE Email = @Email;";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 150).Value = email;
        connection.Open();

        using SqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapPatient(reader) : null;
    }

    public bool Update(Patient patient)
    {
        const string sql = @"
            UPDATE Contacts
            SET FullName = @FullName,
                Phone = @Phone,
                BirthDate = @BirthDate,
                Address = @Address
            WHERE ContactId = @ContactId;";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        AddPatientParameters(command, patient);
        command.Parameters.Add("@ContactId", System.Data.SqlDbType.Int).Value = patient.ContactId;
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int contactId)
    {
        const string sql = "DELETE FROM Contacts WHERE ContactId = @ContactId;";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@ContactId", System.Data.SqlDbType.Int).Value = contactId;
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    private static void AddPatientParameters(SqlCommand command, Patient patient)
    {
        command.Parameters.Add("@FullName", System.Data.SqlDbType.NVarChar, 120).Value = patient.FullName;
        command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 150).Value = patient.Email;
        command.Parameters.Add("@Phone", System.Data.SqlDbType.NVarChar, 25).Value = patient.Phone;
        command.Parameters.Add("@BirthDate", System.Data.SqlDbType.Date).Value = patient.BirthDate;
        command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar, 250).Value = patient.Address;
    }

    private static Patient MapPatient(SqlDataReader reader)
    {
        return new Patient
        {
            ContactId = reader.GetInt32(reader.GetOrdinal("ContactId")),
            FullName = reader.GetString(reader.GetOrdinal("FullName")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Phone = reader.GetString(reader.GetOrdinal("Phone")),
            BirthDate = reader.GetDateTime(reader.GetOrdinal("BirthDate")),
            Address = reader.GetString(reader.GetOrdinal("Address")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}
