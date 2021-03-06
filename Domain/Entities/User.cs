﻿using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User: Entity
    {        

        [StringLength(100)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}