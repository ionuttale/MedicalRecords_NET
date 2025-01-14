using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Pharmacy.Controllers
{
    public class MedicinesController : Controller
    {
        private readonly MySqlConnection _connection;
        public MedicinesController(MySqlConnection connection){
            _connection = connection;
        }

        public IActionResult Index()
        {
            var medicines = new List<Medicine>();

            try{
                _connection.Open();
                var command = new MySqlCommand("SELECT * FROM Medicines", _connection);
                var reader = command.ExecuteReader();

                while(reader.Read()){
                    medicines.Add(new Medicine{
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Producer = reader.GetString("producer"),
                        Price = reader.GetInt32("price"),
                        ExpirationDate = reader.GetDateTime("expiration_date"),
                        Quantity = reader.GetInt32("quantity"),
                        Category = reader.GetString("category"),
                        MedicalPrescription = reader.GetString("medical_prescription")
                    });
                }
            }
            catch(Exception ex){
                Console.WriteLine($"MedicinesController: {ex.Message}");
            }
            finally{
                _connection.Close();
            }
            return View(medicines);
        }

        [HttpDelete]
        [Route("Medicines/Delete/{id}")]
        public IActionResult Delete(int id)
        {
             try
            {
                _connection.Open();
                var command = new MySqlCommand("DELETE FROM Medicines WHERE Id = @Id", _connection);
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

        // Add other actions like Create, Edit, Delete, etc.
    }
}
