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
    public class GroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAllGroups()
        {
            try
            {
                var allGroups = await _unitOfWork.PostRepository.GetAll().ToListAsync();

                return new SuccessModel
                {
                    Data = allGroups,
                    Message = "Groups retrieved",
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

        [HttpPost]
        public async Task<ActionResult<object>> Create(Group group)
        {
            try
            {
                _unitOfWork.PostRepository.Create(group);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = group,
                    Message = "Group created",
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

        [HttpPost]
        public async Task<ActionResult<object>> Update(Group group)
        {
            try
            {
                _unitOfWork.PostRepository.Update(group);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = group,
                    Message = "Group updated",
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

        [HttpDelete]
        public async Task<ActionResult<object>> Delete(int id)
        {
            try
            {
                Post post = _unitOfWork.GroupRepository.GetById(id);
                _unitOfWork.GroupRepository.Delete(post);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = post,
                    Message = "Group deleted",
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
