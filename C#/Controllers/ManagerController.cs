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
    public class ManagerController : ControllerBase
    {
        private readonly ConstructionCompanyDbContext _context;
        private readonly IMapper _mapper;

        public ManagerController(ConstructionCompanyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("employees")]
        [Authorize(Roles = "manager")]
        [LogUserAction(ActionDescription = "Працівників отримано")]
        public async Task<IActionResult> GetEployees()
        {
            var employees = await _context.Employees.ToListAsync();
            var employeeDTO = _mapper.Map<List<EmployeeDTO>>(employees);

            return Ok(employeeDTO);
        }

        [HttpPost("add-employee")]
        [Authorize(Roles = "manager")]
        [LogUserAction(ActionDescription = "Додано робітника")]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeDTO employeeDTO)
        {
            if (string.IsNullOrWhiteSpace(employeeDTO.FullName) ||
                string.IsNullOrWhiteSpace(employeeDTO.Position))
            {
                return BadRequest("All fields except employeeId are required.");
            }

            try
            {
                var employee = _mapper.Map<Employee>(employeeDTO);
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                if (employeeDTO.BrigadeId.HasValue)
                {
                    // Додай працівника до бригади через зв’язок many-to-many
                    var brigade = await _context.Brigades
                        .Include(b => b.Employees)
                        .FirstOrDefaultAsync(b => b.BrigadeId == employeeDTO.BrigadeId.Value);

                    if (brigade != null)
                    {
                        brigade.Employees.Add(employee);
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new { message = "Employee added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("brigades")]
        [Authorize(Roles = "manager")]
        [LogUserAction(ActionDescription = "Бригади отримано")]
        public async Task<IActionResult> GetBrigades()
        {
            var brigades = await _context.Brigades
                .Where(b => b.IsActive == true)
                .ToListAsync();

            var brigadeDTOs = _mapper.Map<List<BrigadeDTO>>(brigades);

            return Ok(brigadeDTOs);
        }


        [HttpGet("employeebrigade")]
        [Authorize(Roles = "manager")]
        [LogUserAction(ActionDescription = "Приналежність робітників до бригад отримано")]
        public async Task<IActionResult> GetEmployeeBrigadePairs()
        {
            var pairs = await _context.Employees
                .SelectMany(e => e.Brigades, (e, b) => new
                {
                    EmployeeName = e.FullName,
                    BrigadeName = b.Name
                })
                .ToListAsync();

            return Ok(pairs);
        }

        [Authorize(Roles = "manager")]
        [HttpGet("export/employees")]
        public async Task<IActionResult> ExportEmployeesToJson()
        {
            var logs = await _context.Employees.ToListAsync();
            return Ok(logs);
        }

        [HttpGet("company-info")]
        [Authorize(Roles = "manager")]
        [LogUserAction(ActionDescription = "Інформація про компанію отримана")]
        public async Task<IActionResult> GetCompanyInformation()
        {
            var projects = await _context.Projects.ToListAsync();
            var employees = await _context.Employees.ToListAsync();
            var brigades = await _context.Brigades
                .Include(b => b.Employees)
                .Include(b => b.Projects)
                .ToListAsync();
            var materials = await _context.Materials.ToListAsync();

            var info = new
            {
                Projects = projects.Select(p => new { p.Name, p.Status, p.Budget }),
                TotalBudget = projects.Sum(p => p.Budget ?? 0),
                ActiveBrigades = brigades.Where(b => b.IsActive == true).Select(b => b.Name),
                InactiveBrigades = brigades.Where(b => b.IsActive == false || b.IsActive == null).Select(b => b.Name),
                Employees = employees.Select(e => new { e.FullName, e.Position, e.Salary }),
                TotalSalaries = employees.Sum(e => e.Salary ?? 0),
                EmployeeBrigades = employees.SelectMany(e => e.Brigades, (e, b) => new { EmployeeName = e.FullName, BrigadeName = b.Name }),
                BrigadeProjects = brigades.SelectMany(b => b.Projects, (b, p) => new { BrigadeName = b.Name, ProjectName = p.Name }),
                TotalMaterialCost = materials.Sum(m => m.TotalPrice ?? 0)
            };

            return Ok(info);
        }

        [HttpPut("tasks/{taskId}")]
        [Authorize(Roles = "manager")]
        [LogUserAction(ActionDescription = "Завдання оновлено")]
        public async Task<IActionResult> UpdateTask(int taskId, [FromBody] TaskDTO dto)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null) return NotFound();

            if (dto.Deadline != null && task.Deadline != dto.Deadline)
                task.Deadline = dto.Deadline;

            if (!string.IsNullOrEmpty(dto.Description) && task.Description != dto.Description)
                task.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.Status) && task.Status != dto.Status)
                task.Status = dto.Status;
            if (dto.Deadline != null && dto.Deadline < DateOnly.FromDateTime(DateTime.Today))
                return BadRequest("Deadline не може бути в минулому");



            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("tasks/{brigadeId}")]
        [Authorize(Roles = "manager")]
        [LogUserAction(ActionDescription = "Отримано завдання бригади")]
        public async Task<IActionResult> GetTasksByBrigade(int brigadeId)
        {
            var tasks = await _context.Tasks
                .Include(t => t.Brigade)
                .Where(t => t.BrigadeId == brigadeId)
                .ToListAsync();

            var taskDTOs = _mapper.Map<List<TaskDTO>>(tasks);
            return Ok(taskDTOs);
        }

        [HttpPost("add-task")]
        [Authorize(Roles = "manager")]
        [LogUserAction(ActionDescription = "Завдання додано")]
        public async Task<IActionResult> AddTask([FromBody] TaskDTO taskDTO)
        {
            if (taskDTO == null)
                return BadRequest("Дані завдання не передані.");

            if (string.IsNullOrWhiteSpace(taskDTO.Description) || !taskDTO.BrigadeId.HasValue)
                return BadRequest("Опис і BrigadeId обов’язкові.");

            if (taskDTO.Deadline != null && taskDTO.Deadline < DateOnly.FromDateTime(DateTime.Today))
                return BadRequest("Deadline не може бути в минулому.");

            var brigade = await _context.Brigades.FindAsync(taskDTO.BrigadeId.Value);
            if (brigade == null)
                return NotFound($"Бригада з ID {taskDTO.BrigadeId.Value} не знайдена.");

            var task = _mapper.Map<Models.Task>(taskDTO);
            task.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Завдання успішно додано", taskId = task.TaskId });
        }

    }
}

