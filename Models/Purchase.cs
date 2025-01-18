using System;

namespace Models{
    public class Purchase{
        public required Int32 Id{get; set;}
        public required String Patient_Name{get; set;}
        public required String Patient_CNP{get; set;}
        public required String Medicine_Name{get; set;}
        public required String Producer{get; set;}
        public required DateTime SaleDate{get; set;}
        public required Int32 Quantity{get; set;}
        public required float Price{get; set;}
       
    }
}