using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Entities;
using Data.Repositories.Implementation;
using Data.Repositories.Interfaces;
using Insta.Models;

namespace Insta.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);  
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAllUsers()
        {
            try
            {
                var allUsers = await _unitOfWork.UserRepository.GetAll().ToListAsync();

                return new SuccessModel
                {
                    Data = allUsers,
                    Message = "Users retrieved",
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }

    }
}
