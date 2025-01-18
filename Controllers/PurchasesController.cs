using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using ZstdSharp.Unsafe;

namespace Pharmacy.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly MySqlConnection _connection;
        public PurchasesController(MySqlConnection connection){
            _connection = connection;
        }

        public IActionResult Index()
        {
            var purchases = new List<Purchase>();

            try{
                _connection.Open();
                var command = new MySqlCommand("SELECT pm.id AS id, p.name AS purchase_name, p.surname as purchase_surname, p.cnp AS purchase_cnp, m.name AS medicine_name, m.producer AS producer, pm.sale_date AS sale_date, pm.quantity AS quantity, m.price AS price FROM Patients_medicines pm JOIN Patients p ON pm.id = p.id JOIN Medicines m ON pm.id_medicine = m.id ORDER BY pm.sale_date DESC; ", _connection);
                var reader = command.ExecuteReader();

                while(reader.Read()){
                    purchases.Add(new Purchase{
                        Id = reader.GetInt32("id"),
                        Patient_Name = reader.GetString("purchase_name")+" "+reader.GetString("purchase_surname"),
                        Patient_CNP = reader.GetString("purchase_cnp"),
                        Medicine_Name = reader.GetString("medicine_name"),
                        Producer = reader.GetString("producer"),
                        SaleDate = reader.GetDateTime("sale_date"),
                        Quantity = reader.GetInt32("quantity"),
                        Price = (float)Math.Round(reader.GetFloat("price") * reader.GetInt32("quantity"), 3)
                    });
                }
            }
            catch(Exception ex){
                Console.WriteLine($"PurchasesController: {ex.Message}");
            }
            finally{
                _connection.Close();
            }
            return View(purchases);
        }

        [HttpDelete]
        [Route("Purchases/Delete/{id}")]
        public IActionResult Delete(int id)
        {
             try
            {
                _connection.Open();
                var command = new MySqlCommand("DELETE FROM Patients_medicines WHERE Id = @Id", _connection);
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

                var command = new MySqlCommand("SELECT * FROM Patients_medicines WHERE id = @purchase_id", _connection);
                command.Parameters.AddWithValue("@purchase_id", id);
                var reader = command.ExecuteReader();

                var purchase_id = 0;
                var medicine_id = 0;
                var sale_date = new DateTime();
                var quantity = 0;

                if (reader.Read())
                {
                    purchase_id = reader.GetInt32("id");
                    medicine_id = reader.GetInt32("id_medicine");
                    sale_date = reader.GetDateTime("sale_date");
                    quantity = reader.GetInt32("quantity");
                }
                
                reader.Close(); 

                command = new MySqlCommand("SELECT * FROM Patients WHERE id = @purchase_id", _connection);
                command.Parameters.AddWithValue("@purchase_id", purchase_id);
                reader = command.ExecuteReader();

                var purchase_name = new String("");
                var purchase_cnp = new String("");    
                if(reader.Read()){
                    purchase_name = reader.GetString("name") + " " + reader.GetString("surname");
                    purchase_cnp = reader.GetString("cnp");
                }

                reader.Close();

                command = new MySqlCommand("SELECT * FROM Medicines WHERE id = @medicine_id", _connection);
                command.Parameters.AddWithValue("@medicine_id", medicine_id);
                reader = command.ExecuteReader();

                var medicine_name = new String("");
                var medicine_producer = new String("");
                var medicine_price = new float();

                if(reader.Read()){
                    medicine_name = reader.GetString("name");
                    medicine_producer = reader.GetString("producer");
                    medicine_price = reader.GetFloat("price");
                }
                    
                var purchase = new Purchase
                {
                    Id = id,
                    Patient_Name = purchase_name,
                    Patient_CNP = purchase_cnp,
                    Medicine_Name = medicine_name,
                    Producer = medicine_producer,
                    SaleDate = sale_date,
                    Quantity = quantity,
                    Price = medicine_price
                };

                return View(purchase);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Purchase edit error: {ex.Message}");
            }
            finally
            {
                // Ensure that the connection is closed
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        [HttpPost]
        public IActionResult Edit(Purchase purchase)
        {
            try
            {
                _connection.Open();
                Console.WriteLine($"Received Patient Data: {purchase.Patient_CNP}, {purchase.Medicine_Name}, {purchase.Producer}, {purchase.SaleDate}");

                // Step 1: Check if the patient exists in the Patients table
                var patientCommand = new MySqlCommand("SELECT id FROM Patients WHERE cnp = @patient_cnp", _connection);
                patientCommand.Parameters.AddWithValue("@patient_cnp", purchase.Patient_CNP);

                var patientReader = patientCommand.ExecuteReader();
                if (!patientReader.Read())
                {
                    Console.WriteLine("PATIENT NOT FOUND");
                    TempData["ErrorMessage"] = "Patient not found.";
                    return View(purchase); // Or another page of your choice
                }

                Console.WriteLine("PATIENT FOUND");
                
                int patientId = patientReader.GetInt32("id");
                patientReader.Close(); // Close the reader after use

                // Step 2: Check if the medicine exists in the Medicines table
                var medicineCommand = new MySqlCommand("SELECT id FROM Medicines WHERE name = @medicine_name AND producer = @producer", _connection);
                medicineCommand.Parameters.AddWithValue("@medicine_name", purchase.Medicine_Name);
                medicineCommand.Parameters.AddWithValue("@producer", purchase.Producer);

                var medicineReader = medicineCommand.ExecuteReader();
                if (!medicineReader.Read())
                {
                    Console.WriteLine("MEDICINE NOT FOUND");
                    TempData["ErrorMessage"] = "Medicine not found.";
                    return View(purchase); // Or another page of your choice
                }

                Console.WriteLine("MEDICINE FOUND");

                int medicineId = medicineReader.GetInt32("id");
                medicineReader.Close(); // Close the reader after use

                // Step 3: Update the Patients_medicines table with new values
                var updateCommand = new MySqlCommand("UPDATE Patients_medicines SET id_patient = @id_patient, id_medicine = @id_medicine, sale_date = @sale_date, quantity = @quantity WHERE id = @id", _connection);
                updateCommand.Parameters.AddWithValue("@quantity", purchase.Quantity);
                updateCommand.Parameters.AddWithValue("@sale_date", purchase.SaleDate);
                updateCommand.Parameters.AddWithValue("@id_patient", patientId);
                updateCommand.Parameters.AddWithValue("@id_medicine", medicineId);
                updateCommand.Parameters.AddWithValue("@id", purchase.Id);

                int rowsAffected = updateCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Updated!");
                    return RedirectToAction("Index", "Purchases");
                }

                Console.WriteLine("Update failed!");
                TempData["ErrorMessage"] = "Update failed.";
                return View(purchase); // Or another page of your choice
            }
            catch (Exception ex)
            {
                // Log error and return status code 500 for error
                return StatusCode(500, $"Edit error: {ex.Message}");
            }
            finally
            {
                _connection.Close();
            }
        }


    }
}
