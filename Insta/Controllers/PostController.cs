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
    public class PostController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAllPosts()
        {
            try
            {
                var allPosts = await _unitOfWork.PostRepository.GetAll().ToListAsync();

                return new SuccessModel
                {
                    Data = allPosts,
                    Message = "Posts retrieved",
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
        public async Task<ActionResult<object>> Create(Post post)
        {
            try
            {
                _unitOfWork.PostRepository.Create(post);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = post,
                    Message = "Post created",
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
        public async Task<ActionResult<object>> Update(Post post)
        {
            try
            {
                _unitOfWork.PostRepository.Update(post);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = post,
                    Message = "Post updated",
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
                Post post = _unitOfWork.PostRepository.GetById(id);
                _unitOfWork.PostRepository.Delete(post);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = post,
                    Message = "Post deleted",
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
