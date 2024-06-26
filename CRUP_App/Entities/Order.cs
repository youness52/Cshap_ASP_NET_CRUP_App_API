﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace CRUP_App.Entities
{
    
        public class Order
        {
            [Key] // Specifies that this property is the primary key
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Specifies that this property is auto-generated by the database
            public int OrderId { get; set; }

            public DateTime DateOrder { get; set; }

            public int UserId { get; set; } // Foreign key referencing the User table

            public int ProductId { get; set; } // Foreign key referencing the Product table

        // Navigation properties for relating orders to users and products
            public User? User { get; set; } 

            public Product? Product { get; set; }
        }
    
}
