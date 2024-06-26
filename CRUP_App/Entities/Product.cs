﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRUP_App.Entities
{
    public class Product
    {
        [Key] // Specifies that this property is the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Specifies that this property is auto-generated by the database
        public int Id { get; set; }
        public string name { get; set; } = null!;

        public string description { get; set; }

        public decimal price { get; set; } = 0;

        public string category { get; set; } 

        // Navigation property representing the collection of orders associated with the product
        public ICollection<Order>? Orders { get; set; }
    }
}
