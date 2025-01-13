using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Pharmacy.Controllers
{
    public class PatientsController : Controller
    {
        private readonly MySqlConnection _connection;
        public PatientsController(MySqlConnection connection){
            _connection = connection;
        }

        public int CalculateAge(DateTime birthday)
        {
            var today = DateTime.Today;
            var age = today.Year - birthday.Year;

            // If the birthday hasn't occurred this year, subtract 1
            if (birthday.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
        public IActionResult Index()
        {
            var patients = new List<Patient>();

            try{
                _connection.Open();
                var command = new MySqlCommand("SELECT * FROM Patients", _connection);
                var reader = command.ExecuteReader();

                while(reader.Read()){
                    patients.Add(new Patient{
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Surname = reader.GetString("surname"),
                        CNP = reader.GetString("cnp"),
                        Birthday = reader.GetDateTime("birthday"),
                        Age = CalculateAge(reader.GetDateTime("birthday")),
                        Gender = reader.GetString("gender"),
                        Address = reader.GetString("address"),
                        PhoneNumber = reader.GetString("phone_number"),
                        Email = reader.GetString("email"),
                        RegistrationDate = reader.GetDateTime("registration_date"),
                        Diagnosis = reader.GetString("diagnosis")
                    });
                }
            }
            catch(Exception ex){
                Console.WriteLine($"PatientsController: {ex.Message}");
            }
            finally{
                _connection.Close();
            }
            return View(patients);
        }

        [HttpDelete]
        [Route("Patients/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
               _connection.Open();
               var command = new MySqlCommand("DELETE FROM Patients WHERE Id = @Id", _connection);
               command.Parameters.AddWithValue("@Id", id);
               command.ExecuteNonQueryAsync();

                // Return Ok to indicate success
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Delete error:{ex.Message}");
            }
            finally{
                _connection.Close();
            }
        }

        // Add other actions like Create, Edit, Delete, etc.
    }
}
