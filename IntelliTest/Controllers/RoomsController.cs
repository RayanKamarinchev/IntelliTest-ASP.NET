using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Chat;
using IntelliTest.Data.Entities;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace IntelliTest.Controllers
{
    [Authorize]
    [Route("Rooms")]
    public class RoomsController : Controller
    {
        private readonly IRoomService roomService;

        public RoomsController(IRoomService _roomService)
        {
            roomService = _roomService;

        }
        [HttpGet("Get")]
        public async Task<ActionResult<IEnumerable<RoomViewModel>>> Get()
        {
            var roomsViewModel = await roomService.GetAll(User.Id());
            return Ok(roomsViewModel);
        }

        [HttpGet("Get/{id}")]
        public async Task<ActionResult<Room>> Get(Guid id)
        {
            var roomViewModel = await roomService.GetById(id, User.Id());
            if (roomViewModel == null)
            {
                return NotFound();
            }
            return Ok(roomViewModel);
        }

        [HttpGet("Create")]
        public async Task<ActionResult<Room>> Create(string name)
        {
            RoomViewModel viewModel = new RoomViewModel()
            {
                Name = name
            };
            var createdRoom = await roomService.Create(viewModel, User.Id());
            if (createdRoom == null)
            {
                return BadRequest("Invalid room name or room already exists");
            }
            return CreatedAtAction(nameof(Get), new { id = createdRoom.Id }, createdRoom);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id,string name)
        {
            var status = await roomService.Edit(id, new RoomViewModel()
            {
                Name = name
            }, User.Id());
            if (status==HttpError.NotFound)
            {
                return NotFound();
            }

            if (status == HttpError.BadRequest)
            {
                return BadRequest("Невалидна стая.");
            }
            return Ok();
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool isOkay = await roomService.Delete(id, User.Id());
            if (!isOkay)
            {
                return NotFound();
            }
            return Ok();
        }
        [HttpGet("Join/{id}")]
        public async Task<IActionResult> Join(Guid id)
        {
            bool isOkay = await roomService.AddUser(id, User.Id());
            if (!isOkay)
            {
                return NotFound();
            }
            return RedirectToAction("Index", "Chat");
        }
    }
}
