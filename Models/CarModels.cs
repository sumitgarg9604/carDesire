using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Assignment1_v9.Models
{
    public class CarModels
    {
        [Required]
        [Display(Name = "Car ID")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Car Make")]
        public string CarMake { get; set; }

        [Required]
        [Display(Name = "Car Model")]
        public string CarModel { get; set; }

        [Required]
        [Display(Name = "Seating capacity")]
        public int Capacity { get; set; }

        [Required]
        [Display(Name = "Car Type")]
        public string Category { get; set; }

        [Display(Name = "Chasis Number")]
        public string ChasisNum { get; set; }

        [Required]
        [Display(Name = "Transmission Type")]
        public string Transmission { get; set; }

        [Required]
        [Display(Name = "Fuel Consumption")]
        public double Mileage { get; set; }

        [Required]
        [Display(Name = "Hourly Rate")]
        public double HourlyRate { get; set; }

    }
}