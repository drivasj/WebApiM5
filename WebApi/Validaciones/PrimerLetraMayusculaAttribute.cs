using System.ComponentModel.DataAnnotations;

namespace WebApi.Validaciones
{
    public class PrimerLetraMayusculaAttribute: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
          if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

          var primeraletra = value.ToString()[0].ToString();

            if(primeraletra != primeraletra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayuscula");
            }
            return ValidationResult.Success;
        }
    }
}
