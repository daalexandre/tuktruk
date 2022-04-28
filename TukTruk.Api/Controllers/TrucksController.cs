using Microsoft.AspNetCore.Mvc;
using TukTruk.Api.Models;
using TukTruk.Api.Core.IConfiguration;
using TukTruk.Api.Core.IRepositories;

namespace TukTruk.Api.Controllers
{
    [ApiController]
    [Route("trucks")]
    public class TrucksController : ControllerBase
    {
        private readonly ILogger<TrucksController> _logger;
        private readonly ITrucksRepository _repository;

        private readonly IUnitOfWork _unitOfWork;

        public TrucksController(
            ITrucksRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<TrucksController> logger
        )
        {
            _logger = logger;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var trucks = await _repository.All();

            return Ok(trucks);
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetTruckAsync))]
        public async Task<IActionResult> GetTruckAsync(Guid id)
        {
            var truck = await _repository.GetById(id);

            if (truck == null)
                return NotFound();

            return Ok(truck);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTruckAsync(Truck truck)
        {
            if (ModelState.IsValid)
            {
                truck.Id = Guid.NewGuid();
                await _repository.Add(truck);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction(nameof(GetTruckAsync), new { id = truck.Id.ToString() }, truck);
            }
            return new JsonResult("Erro ao adicionar caminhão")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTruckAsync(Guid id, Truck truck)
        {
            truck.Id = id;

            await _repository.Update(truck);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTruckAsync(Guid id)
        {
            var truck = await _repository.GetById(id);
            if (truck is null)
            {
                return BadRequest("Caminhão não existe");
            }

            await _repository.Delete(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}