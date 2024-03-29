﻿using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace project_c.Models.Plants
{
    public class Option
    {
        [Key]
        public int OptionId { get; set; }

        [MaxLength(20, ErrorMessage = "Naam mag niet langer zijn dan 45 tekens!"), Required]
        public string DisplayName { get; set; }
        
        public int Position { get; set; }
        
        public Filter.FilterSystemId FilterId { get; set; }
        
        public Filter Filter { get; set; }
    }
}