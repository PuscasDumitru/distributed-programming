using Data;
using Data.Entities;
using Data.Repositories.Implementation;
using Insta.Controllers;
using Insta.Interfaces;
using Insta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Insta.Tests
{
    public class PostControllerTests
    {
        [Fact]
        public async void GetAllPosts_ReturnsAllPosts()
        {
            var mockPhotoService = new Mock<IPhotoService>();
            
            var options = new DbContextOptionsBuilder<RepositoryDbContext>()
                .UseInMemoryDatabase(databaseName: "PostListDatabase")
                .Options;

            using (var context = new RepositoryDbContext(options))
            {
                context.Posts.Add(new Post { Id = 1, Title = "something", Image = "something" });
                context.Posts.Add(new Post { Id = 2, Title = "something else", Image = "something else" });
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(options))
            {
                var controller = new PostController(context, mockPhotoService.Object);

                var result = await controller.GetAllPosts();
                var actionResult = Assert.IsType<ActionResult<object>>(result);
                var returnValue = Assert.IsType<SuccessModel>(actionResult.Value);
                var exctractedPosts = Assert.IsAssignableFrom<ICollection<Post>>(returnValue.Data);

                Assert.Equal(2, exctractedPosts.Count);
            }
        }

        [Fact]
        public async void GetPostByIdAsync_ReturnsAPostById()
        {
            var testPostId = 1;
            var mockPhotoService = new Mock<IPhotoService>();

            var options = new DbContextOptionsBuilder<RepositoryDbContext>()
                .UseInMemoryDatabase(databaseName: "PostListDatabase")
                .Options;

            using (var context = new RepositoryDbContext(options))
            {
                context.Posts.Add(new Post { Id = testPostId, Title = "something", Image = "something" });
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(options))
            {
                var controller = new PostController(context, mockPhotoService.Object);

                var result = await controller.GetPostByIdAsync(testPostId);
                var actionResult = Assert.IsType<ActionResult<object>>(result);
                var returnValue = Assert.IsType<SuccessModel>(actionResult.Value);
                var extractedPost = Assert.IsAssignableFrom<Post>(returnValue.Data);

                Assert.Equal(testPostId, extractedPost.Id);
            }
        }

    }
}