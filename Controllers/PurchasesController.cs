using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

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
                var command = new MySqlCommand("SELECT pm.id AS id, p.name AS patient_name, p.surname as patient_surname, m.name AS medicine_name, m.producer AS producer, pm.sale_date AS sale_date, pm.quantity AS quantity, m.price AS price FROM Patients_medicines pm JOIN Patients p ON pm.id_patient = p.id JOIN Medicines m ON pm.id_medicine = m.id ORDER BY pm.sale_date DESC; ", _connection);
                var reader = command.ExecuteReader();

                while(reader.Read()){
                    purchases.Add(new Purchase{
                        Id = reader.GetInt32("id"),
                        Patient_Name = reader.GetString("patient_name")+" "+reader.GetString("patient_surname"),
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

        // Add other actions like Create, Edit, Delete, etc.
    }
}
