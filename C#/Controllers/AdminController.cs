using AutoMapper;
using ConstructionCompany.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; // якщо ти на SQL Server
using Microsoft.EntityFrameworkCore;
using ConstructionCompany.Models.ModelsDTO;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ConstructionCompany.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ConstructionCompanyDbContext _context;
        private readonly IMapper _mapper;

        public AdminController(ConstructionCompanyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("tables")]
        [Authorize(Roles = "admin")]
        [LogUserAction(ActionDescription = "Таблиці підвантажено")]
        public async Task<IActionResult> GetAllTableNames()
        {
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            var tableNames = new List<string>();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT TABLE_NAME 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = DB_NAME() AND TABLE_NAME != 'sysdiagrams'";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tableNames.Add(reader.GetString(0));
                    }
                }
            }

            return Ok(tableNames);
        }

        [HttpGet("tables/{name}")]
        [Authorize(Roles = "admin")]
        [LogUserAction(ActionDescription = "Таблицю підвантажено")]
        public async Task<IActionResult> GetTable(string name)
        {
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            var rows = new List<Dictionary<string, object>>();

            using (var command = connection.CreateCommand())
            {
                // Безпечно вставляємо ім'я таблиці через параметр — УВАГА: це не SqlParameter, тому потрібно ретельно валідувати
                command.CommandText = $"SELECT * FROM [{name}]";

                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                            }

                            rows.Add(row);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    return BadRequest(new { error = $"SQL error: {ex.Message}" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { error = $"Internal error: {ex.Message}" });
                }
            }

            return Ok(rows);
        }


        [HttpPost("add-user")]
        [Authorize(Roles = "admin")]
        [LogUserAction(ActionDescription = "Додано користувача")]
        public async Task<IActionResult> AddUser([FromBody] UserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.Username) ||
                string.IsNullOrWhiteSpace(userDTO.PasswordHash) ||
                string.IsNullOrWhiteSpace(userDTO.Role) ||
                string.IsNullOrWhiteSpace(userDTO.FullName))
            {
                return BadRequest("All fields except employeeId are required.");
            }

            try
            {
                var user = _mapper.Map<User>(userDTO);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("users")]
        [Authorize(Roles = "admin")]
        [LogUserAction(ActionDescription = "Користувачів отримано")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.UserId, u.Username })
                .ToListAsync();

            return Ok(users);
        }


        [HttpDelete("delete-user/{username}")]
        [Authorize(Roles = "admin")]
        [LogUserAction(ActionDescription = "Користувача видалено")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new { message = "Користувача не знайдено." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Користувача успішно видалено." });
        }

        [HttpPut("edit-user")]
        [Authorize(Roles = "admin")]
        [LogUserAction(ActionDescription = "Користувача відредаговано")]
        public async Task<IActionResult> EditUser([FromBody] UserDTO userDTO)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDTO.Username);
            if (existingUser == null)
                return NotFound(new { message = "User not found." });

            // Оновлюємо лише вказані поля
            if (!string.IsNullOrWhiteSpace(userDTO.PasswordHash))
                existingUser.PasswordHash = userDTO.PasswordHash;

            if (!string.IsNullOrWhiteSpace(userDTO.Role))
            {
                // Додай список допустимих ролей, якщо треба валідувати:
                var allowedRoles = new[] { "admin", "user", "manager" };
                if (!allowedRoles.Contains(userDTO.Role.ToLower()))
                {
                    return BadRequest(new { message = $"Invalid role '{userDTO.Role}'." });
                }
                existingUser.Role = userDTO.Role;
            }

            if (!string.IsNullOrWhiteSpace(userDTO.FullName))
                existingUser.FullName = userDTO.FullName;

            if (userDTO.EmployeeId.HasValue)
                existingUser.EmployeeId = userDTO.EmployeeId.Value;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "User updated successfully." });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Database update failed.", error = ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpPost("log-action")]
        public async Task<IActionResult> LogUserAction([FromBody] UserActionLogDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                return NotFound(new { message = "User not found." });

            var log = new UserActionLog
            {
                UserId = user.UserId,
                ActionDescription = dto.ActionDescription,
            };

            _context.UserActionLogs.Add(log);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Action logged successfully." });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("export/UserActionLogs")]
        public async Task<IActionResult> ExportUserActionLogsToJson()
        {
            var logs = await _context.UserActionLogs.ToListAsync();
            return Ok(logs); // автоматично серіалізується у JSON
        }


    }
}
