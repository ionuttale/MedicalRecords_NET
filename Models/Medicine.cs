using System;
using Microsoft.VisualBasic;

namespace Models{
    public class Medicine{
        public required Int32 Id{get; set;}
        public required string Name{get; set;}
        public required string Producer{get; set;}
        public required float Price{get; set;}
        public required DateTime ExpirationDate{get; set;}
        public required Int32 Quantity{get; set;}
        public required string Category{get; set;}
        public required string MedicalPrescription{get; set;}
    }
}