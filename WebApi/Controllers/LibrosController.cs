using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Entidades;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        //Permite acceder a todos los metodos a la cadena de conexión 
        private readonly ApplicationDbContext context;

        public LibrosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Libro>> Get(int id)
        {
            return await context.Libros.Include(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);
        }

       [HttpPost]
       public async Task<ActionResult> Post(Libro libro)
        {
            //Valida si el autor eiste
            var existeAutor = await context.Autores.AnyAsync(X => X.Id == libro.AutorId);
            if (!existeAutor)
            {
                return BadRequest($"No existe el autor de ID: {libro.AutorId}");
            } 

            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
