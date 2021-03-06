using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HikeFinder.Models;

namespace HikeFinder.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TrailsController : ControllerBase
  {
    private readonly DatabaseContext _context;

    public TrailsController(DatabaseContext context)
    {
      _context = context;
    }

    // GET: api/Trails
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Trail>>> GetTrails()
    {
      return await _context.Trails.ToListAsync();
    }

    // GET: api/Trails/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Trail>> GetTrail(int id)
    {
      // trail and the reviews 
      /* 
         SELECT * 
         FROM Trail
         JOIN Reviews ON Trail.Id = Review.TrailId
         WHERE Trail.Id = {id}
      */
      var trail = await _context
            .Trails
            .Include(trl => trl.Reviews)
            .FirstOrDefaultAsync(f => f.Id == id);

      if (trail == null)
      {
        return NotFound();
      }



      return trail;
    }

    // PUT: api/Trails/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    // more details see https://aka.ms/RazorPagesCRUD.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTrail(int id, Trail trail)
    {
      if (id != trail.Id)
      {
        return BadRequest();
      }

      _context.Entry(trail).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!TrailExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/Trails
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    // more details see https://aka.ms/RazorPagesCRUD.
    [HttpPost]
    public async Task<ActionResult<Trail>> PostTrail(Trail trail)
    {
      _context.Trails.Add(trail);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetTrail", new { id = trail.Id }, trail);
    }

    [HttpPost("{trailId}/reviews")]
    public async Task<ActionResult> AddReviewForTrail(int trailId, Review review)
    {
      //opt 1
      // adding review to the trial
      review.TrailId = trailId;
      // add the review to the database
      _context.Reviews.Add(review);
      await _context.SaveChangesAsync();
      // returning something
      return Ok(review);
      //   opt 2

      //   var trail = await _context.Trails.FirstOrDefaultAsync(f => f.Id == trailId);
      //   trail.Reviews.Add(review);
      //   await _context.SaveChangesAsync();

    }

    // DELETE: api/Trails/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Trail>> DeleteTrail(int id)
    {
      var trail = await _context.Trails.FindAsync(id);
      if (trail == null)
      {
        return NotFound();
      }

      _context.Trails.Remove(trail);
      await _context.SaveChangesAsync();

      return trail;
    }

    private bool TrailExists(int id)
    {
      return _context.Trails.Any(e => e.Id == id);
    }
  }
}
