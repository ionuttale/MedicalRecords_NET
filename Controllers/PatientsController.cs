using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ZstdSharp.Unsafe;

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
                Console.WriteLine($"Patients size:{patients.Count}");
                return View(patients);
            }
            catch(Exception ex){
                Console.WriteLine($"PatientsController: {ex.Message}");
                return StatusCode(500, $"Index:{ex.Message}");
            }
            finally{
                _connection.Close();
            }
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

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Delete error:{ex.Message}");
            }
            finally{
                _connection.CloseAsync();
            }
        }
        [HttpGet]
        public IActionResult Edit(int id){
            try{
                _connection.Open();
                var command = new MySqlCommand("SELECT * FROM Patients WHERE id = @patient_Id", _connection);
                command.Parameters.AddWithValue("@patient_Id", id);
                var reader = command.ExecuteReader();
                if(reader.Read()){
                    var patient = new Patient{
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
                    };
                    return View(patient);
                }
                return NotFound();
            }
            catch(Exception ex){
                return StatusCode(500, $"Edit error:{ex.Message}");
            }
            finally{
                _connection.Close();
            }
        }

        [HttpPost]
        public IActionResult Edit(Patient patient){
            try{
                _connection.Open();
                Console.WriteLine($"Received Patient Data: {patient.Name}, {patient.Surname}, {patient.PhoneNumber}");
                var command = new MySqlCommand("UPDATE Patients SET name = @Name, surname = @Surname, phone_number = @PhoneNumber, address = @Address, email = @Email, diagnosis = @Diagnosis WHERE id = @Id", _connection);
                command.Parameters.AddWithValue("@Id", patient.Id);
                command.Parameters.AddWithValue("@Name", patient.Name);
                command.Parameters.AddWithValue("@Surname", patient.Surname);
                command.Parameters.AddWithValue("@PhoneNumber", patient.PhoneNumber);
                command.Parameters.AddWithValue("@Address", patient.Address);
                command.Parameters.AddWithValue("@Email", patient.Email);
                command.Parameters.AddWithValue("@Diagnosis", patient.Diagnosis);

                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected > 0){
                    return RedirectToAction("Index", "Patients");
                }
                return NotFound();
            }
            catch(Exception ex){
                return StatusCode(500, $"Edit error:{ex.Message}");
            }
            finally{
                _connection.Close();
            }
        }

        // Add other actions like Create, Edit, Delete, etc.
    }
}
