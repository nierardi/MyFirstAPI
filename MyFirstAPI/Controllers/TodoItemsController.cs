using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Models;
using MyFirstAPI.Utils.Authorization;

namespace MyFirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext dbContext;

        public TodoItemsController(/* DI */ TodoContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/TodoItems
        //[RequirePermission(Permission = "ViewAllTodos")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await dbContext.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await dbContext.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            dbContext.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
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

        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            dbContext.TodoItems.Add(todoItem);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await dbContext.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            dbContext.TodoItems.Remove(todoItem);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return dbContext.TodoItems.Any(e => e.Id == id);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTodoItem(int id, [FromBody] JsonPatchDocument patchDocument)
        {
            var todo = dbContext.TodoItems.FirstOrDefault(t => t.Id == id);
            if (patchDocument != null && todo != null)
            {
                try
                {
                    patchDocument.ApplyTo(todo);
                    await dbContext.SaveChangesAsync();
                    return AcceptedAtAction(nameof(GetTodoItem), new {id = todo.Id}, todo);
                }
                catch (JsonPatchException)
                {
                    return BadRequest("The specified field does not exist");
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
