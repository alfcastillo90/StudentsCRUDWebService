using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;

namespace StudentsCRUDWebService
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class StudentsCRUDWebService : WebService
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["StudentDB"].ConnectionString;

        public StudentsCRUDWebService()
        {
            CreateDatabaseIfNotExists();
        }

        private void CreateDatabaseIfNotExists()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("IF DB_ID('StudentDB') IS NULL CREATE DATABASE StudentDB;", conn);
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"
                    IF OBJECT_ID('dbo.Students', 'U') IS NULL
                    CREATE TABLE Students (
                        Id INT PRIMARY KEY IDENTITY,
                        FirstName NVARCHAR(50),
                        LastName NVARCHAR(50),
                        DateOfBirth DATE,
                        Email NVARCHAR(100)
                    );";
                cmd.ExecuteNonQuery();
            }
        }

        [WebMethod]
        public DataSet GetAllStudents()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Students", conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }

        [WebMethod]
        public void AddStudent(string firstName, string lastName, DateTime dateOfBirth, string email)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("El nombre no puede estar vacío.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("El apellido no puede estar vacío.", nameof(lastName));
            if (dateOfBirth == default)
                throw new ArgumentException("La fecha de nacimiento no puede estar vacía.", nameof(dateOfBirth));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El correo electrónico no puede estar vacío.", nameof(email));

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Students (FirstName, LastName, DateOfBirth, Email) VALUES (@FirstName, @LastName, @DateOfBirth, @Email)", conn);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        [WebMethod]
        public void UpdateStudent(int id, string firstName, string lastName, DateTime dateOfBirth, string email)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser un número positivo.", nameof(id));
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("El nombre no puede estar vacío.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("El apellido no puede estar vacío.", nameof(lastName));
            if (dateOfBirth == default)
                throw new ArgumentException("La fecha de nacimiento no puede estar vacía.", nameof(dateOfBirth));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El correo electrónico no puede estar vacío.", nameof(email));

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE Students SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, Email = @Email WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        [WebMethod]
        public void DeleteStudent(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser un número positivo.", nameof(id));

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Students WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
