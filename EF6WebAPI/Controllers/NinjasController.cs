using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using EF6Model;

using EF6Model.RichModels;

namespace EF6WebAPI.Controllers
{
  public class NinjasController : ApiController
    {
        private NinjaContext db = new NinjaContext();

      
        public IQueryable<Ninja> GetNinjas()
        {
            return db.Ninjas;
        }

    
        [ResponseType(typeof(Ninja))]
        public IHttpActionResult GetNinja(int id)
        {
            Ninja ninja = db.Ninjas.Find(id);
            if (ninja == null)
            {
                return NotFound();
            }

            return Ok(ninja);
        }

      
        [ResponseType(typeof(void))]
        public IHttpActionResult PutNinja(int id, Ninja ninja)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ninja.Id)
            {
                return BadRequest();
            }

            db.Entry(ninja).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NinjaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

      
        [ResponseType(typeof(Ninja))]
        public IHttpActionResult PostNinja(Ninja ninja)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ninjas.Add(ninja);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = ninja.Id }, ninja);
        }

        // DELETE: api/Ninjas/5
        [ResponseType(typeof(Ninja))]
        public IHttpActionResult DeleteNinja(int id)
        {
            Ninja ninja = db.Ninjas.Find(id);
            if (ninja == null)
            {
                return NotFound();
            }

            db.Ninjas.Remove(ninja);
            db.SaveChanges();

            return Ok(ninja);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NinjaExists(int id)
        {
            return db.Ninjas.Count(e => e.Id == id) > 0;
        }
    }
}