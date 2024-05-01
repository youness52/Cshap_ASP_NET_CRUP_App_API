using CRUP_App.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CRUP_App.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DotnetdbContext _dotnetdbContext;

        public UserController(DotnetdbContext dotnetdbContext)
        {
            _dotnetdbContext = dotnetdbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {
            var userList = await _dotnetdbContext.User.ToListAsync();

            if (userList == null || userList.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return userList;
            }
        }

        [HttpPost]
        public async Task<ActionResult> InsertUser(User user)
        {
            _dotnetdbContext.User.Add(user);
            await _dotnetdbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _dotnetdbContext.User.FirstOrDefaultAsync(s => s.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return user;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _dotnetdbContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _dotnetdbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _dotnetdbContext.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _dotnetdbContext.User.Remove(user);
            await _dotnetdbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _dotnetdbContext.User.Any(e => e.Id == id);
        }
    }
}
