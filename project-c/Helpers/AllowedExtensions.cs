using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace project_c.Helpers
{
    public class AllowedExtensionsAttribute:ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }
    
        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!((IList) _extensions).Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
        
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Dit type foto of bestand is niet toegestaan!";
        }
    }
}