using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // 1. GET: api/TodoItems (С ПАГИНАЦИЕЙ)
        [HttpGet]
        public async Task<ActionResult<PagedResult<TodoItem>>> GetTodoItems(int page = 1, int pageSize = 10)
        {
            var query = _context.TodoItems.AsQueryable();
            var rowCount = await query.CountAsync();
            var pageCount = (int)Math.Ceiling((double)rowCount / pageSize);

            var skip = (page - 1) * pageSize;
            var results = await query.Skip(skip).Take(pageSize).ToListAsync();

            return new PagedResult<TodoItem>
            {
                Results = results,
                CurrentPage = page,
                PageCount = pageCount,
                PageSize = pageSize,
                RowCount = rowCount
            };
        }

        // 2. GET: api/TodoItems/5 (ПОЛУЧЕНИЕ ОДНОЙ ЗАПИСИ)
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null) return NotFound();
            return todoItem;
        }

        // 3. POST: api/TodoItems (СОЗДАНИЕ - ТО ЧТО НАМ НУЖНО)
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // 4. PUT: api/TodoItems/5 (ОБНОВЛЕНИЕ)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id) return BadRequest();
            _context.Entry(todoItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 5. DELETE: api/TodoItems/5 (УДАЛЕНИЕ)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null) return NotFound();
            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}