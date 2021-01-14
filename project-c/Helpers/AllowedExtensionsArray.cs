using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace project_c.Helpers
{
    public class AllowedExtensionsArrayAttribute:ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsArrayAttribute(string[] extensions)
        {
            _extensions = extensions;
        }
    
        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var files = value as IFormFile[];
            
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {

                        if (file.ContentType != "image/pjpeg" && file.ContentType != "image/jpeg" &&
                            file.ContentType != "image/jpg")
                        {
                            return new ValidationResult(GetErrorMessage());
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Alleen foto's in jpg formaat zijn toegestaan, upload alleen foto's in jpg formaat!";
        }
    }
}