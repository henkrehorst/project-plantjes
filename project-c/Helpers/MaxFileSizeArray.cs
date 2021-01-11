using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace project_c.Helpers
{
    public class MaxFileSizeArrayAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeArrayAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var files = value as IFormFile[];

            foreach (var file in files)
            {
                if (file != null)
                {
                    if (file.Length > _maxFileSize)
                    {
                        return new ValidationResult(GetErrorMessage());
                    }
                }
            }
            
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"1 of meerdere foto's zijn groter dan { _maxFileSize / 1024 / 1024} mb, upload foto's die kleiner zijn dan { _maxFileSize / 1024 / 1024} mb.";
        }
    }
}