using System;

namespace Models{
    public class Patient{
        public required int Id {get; set; }
        public required string Name {get; set; }
        public required string Surname{get; set;}
        public required string CNP{get; set;}
        public required DateTime Birthday{get; set;}
        public required Int32 Age{get; set;}
        public required string Gender{get; set;}
        public required string Address{get; set;}
        public required string PhoneNumber{get; set;}
        public required string Email{get;  set;}
        public required DateTime RegistrationDate{get; set;}
        public required string Diagnosis{get; set;}
    }
}