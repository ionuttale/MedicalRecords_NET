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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand("SELECT * FROM Medicines WHERE id = @medicine_Id", _connection);
                command.Parameters.AddWithValue("@medicine_Id", id);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    var medicine = new Medicine  // Change 'Patient' to 'Medicine'
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Producer = reader.GetString("producer"),
                        Price = reader.GetFloat("price"),
                        ExpirationDate = reader.GetDateTime("expiration_date"),
                        Quantity = reader.GetInt32("quantity"),
                        Category = reader.GetString("category"),
                        MedicalPrescription = reader.GetString("medical_prescription") // Get 'medical_prescription' value
                    };

                    return View(medicine);  // Return 'medicine' object to view
                }
                
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Edit error:{ex.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }


        [HttpPost]
        public IActionResult Edit(Medicine medicine)  // Change 'Patient' to 'Medicine'
        {
            try
            {
                _connection.Open();
                Console.WriteLine($"Received Medicine Data: {medicine.Name}, {medicine.Producer}, {medicine.Price}, {medicine.MedicalPrescription}");

                var command = new MySqlCommand("UPDATE Medicines SET name = @Name, producer = @Producer, price = @Price, expiration_date = @ExpirationDate, quantity = @Quantity, category = @Category, medical_prescription = @MedicalPrescription WHERE id = @Id", _connection);
                
                // Update all parameters
                command.Parameters.AddWithValue("@Id", medicine.Id);
                command.Parameters.AddWithValue("@Name", medicine.Name);
                command.Parameters.AddWithValue("@Producer", medicine.Producer);
                command.Parameters.AddWithValue("@Price", medicine.Price);
                command.Parameters.AddWithValue("@ExpirationDate", medicine.ExpirationDate);
                command.Parameters.AddWithValue("@Quantity", medicine.Quantity);
                command.Parameters.AddWithValue("@Category", medicine.Category);
                command.Parameters.AddWithValue("@MedicalPrescription", medicine.MedicalPrescription);  // Update 'medical_prescription'

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return RedirectToAction("Index", "Medicines");  // Redirect to medicines list after successful update
                }
                
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Edit error:{ex.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }


        // Add other actions like Create, Edit, Delete, etc.
    }
}
