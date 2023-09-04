using Microsoft.AspNetCore.Mvc;
using WebApi.Entidades;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebApi.Servicios;
using Microsoft.AspNetCore.Authorization;
using WebApi.Filtros;

namespace WebApi.Controllers
{
    [ApiController]
    //Ruta del controlador
    [Route("api/autores")] // nombre de la ruta (no cambia asi cambie el nombre del controlador)
    public class AutoresController : ControllerBase
    { 
        //Permite acceder a todos los metodos a la cadena de conexión 
        private readonly ApplicationDbContext context;
        private readonly IServicio servicio;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ILogger<AutoresController> logger;

        public AutoresController(ApplicationDbContext context, IServicio servicio, ServicioTransient servicioTransient,
            ServicioScoped servicioScoped, ServicioSingleton servicioSingleton, ILogger<AutoresController> logger)
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
            this.logger = logger;
        }

        [HttpGet("GUID")]
        // [ResponseCache(Duration =10)]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public ActionResult ObtenerGuid()
        {
            return Ok(new
            {
                AutoresController_Transient = servicioTransient.Guid,
                ServicioA_Transient = servicio.ObtenerTransient(),

                AutoresController_Scoped = servicioScoped.Guid,
                ServicioA_Scoped = servicio.ObtenerScoped(),

                AutoresController_Singleton = servicioSingleton.Guid,      
                ServicioA_Singleton = servicio.ObtenerSingleton()
            });
        }
        //Leer datos
        // Podemos cambiar la ruta de un metodo 
        [HttpGet] // api/autores/
        [HttpGet("listado")] // api/autores/listado
        [HttpGet("/listado")] // listado/autores/listado <- se agrego otra raiz api por listado y devuelve el mismo metodo
      //  [ResponseCache(Duration = 10)] // manejo de cahce
      //  [Authorize] // Solo ususarios autorizados
        public async Task<ActionResult<List<Autor>>> Get()
        {
            throw new  NotImplementedException(); // Filtro de excepcion
            logger.LogInformation("Estamos obteniendo los autores");
            logger.LogWarning("Este es un mensaje de prueba");
            //  servicio.RealizarTarea();
            return await context.Autores.Include(X => X.Libros).ToListAsync();
        }

        [HttpGet("primero")] // api/autores/primero?nombre=Daniel&apellido=Rivas
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int mivalor, [FromQuery] string nombre) // ModelBinding desde la cabecera, query
        {
            return await context.Autores.FirstOrDefaultAsync();
        }
        // En este metodo no estamos utilizando programación asincrona ya que no se esta consultando ningun recurso externo 
        [HttpGet("primero2")] // api/autores/primero
        public ActionResult <Autor> PrimerAutor2()
        {
            return new Autor() { Nombre = "inventado" };
        }

        [HttpGet("{id:int}")] // api/id/primero (Recueprar un registro especifico por id)   [HttpGet("{id:int}/{param2?}")] ejempplo de parametro 2 opcional
        public async Task<ActionResult<Autor>> Get(int id)
        {
           var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if(autor == null)
            {
                return NotFound();
            }
            return autor;
        }


        [HttpGet("{nombre}")] // api/id/primero (Recueprar un registro especifico por nombre)
        public async Task<ActionResult<Autor>> Get([FromRoute] string nombre) // ModelBinding desde la ruta
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (autor == null)
            {
                return NotFound();
            }
            return autor;
        }

        //Crear un autor en la BD
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]Autor autor) // ModelBinding desde el cuerpo
        {
            var existeAutorConElmismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

            if (existeAutorConElmismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre { autor.Nombre}");
            }


            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }
        //Update
        [HttpPut("{id:int}")] // api/autores/1
        public async Task<ActionResult> Put(Autor autor,int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id); // Validar si el registro existe

            if (!existe)
            {
                return NotFound();
            }

            if (autor.Id != id) // Si no es el mismo id ERROR
            {
                return BadRequest("El id del autor no coinde con el id de la URL");
            }
          
            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        //Delete
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id); // Validar si el registro existe
            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new Autor() { Id = id});
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
