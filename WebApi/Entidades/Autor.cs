using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Validaciones;

namespace WebApi.Entidades
{
    public class Autor: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 30, ErrorMessage = "El campo {0} no debe tener mas de {1} carácteres")]
      //  [PrimerLetraMayuscula]
        public string Nombre { get; set; }

       [NotMapped] // No incluir en BD
        public int Edad { get; set; }
        public List<Libro>? Libros { get; set; }

        //public int Menor { get; set; }
        //public int Mayor { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraletra = Nombre[0].ToString();

                if (primeraletra != primeraletra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayuscula",
                        new string[] {nameof(Nombre)});
                }
            }
        }
    }
}
