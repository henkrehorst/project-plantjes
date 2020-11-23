﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace project_c.Models.Plants
{
    public class Filter
    {
        [Key]
        public int FilterId { get; set; }
        
        [MaxLength(45, ErrorMessage = "Naam mag niet langer zijn dan 45 tekens!"), Required]
        public string Name { get; set; }
        
        [MaxLength(45, ErrorMessage = "Beschrijving mag niet langer zijn dan 45 tekens!"), Required]
        public string Description { get; set; }

        public int Position { get; set; }
        
        public List<Option> Options { get; set; }

    }
}