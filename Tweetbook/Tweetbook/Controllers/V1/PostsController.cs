using Microsoft.AspNetCore.Mvc;
using System;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(_postService.GetPosts());
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public IActionResult Get([FromRoute]Guid postId)
        {
            Post post = _postService.GetPostById(postId);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public IActionResult Update([FromRoute]Guid postId, [FromBody] UpdatePostRequest request)
        {

            Post post = new Post
            {
                Id = postId,
                Name = request.Name
            };

            bool updated = _postService.UpdatePost(post);
            if (updated)

            {
                return Ok(post);
            }

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public IActionResult Delete([FromRoute]Guid postId)
        {


            bool deleted = _postService.DeletePost(postId);
            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Create([FromBody] CreatePostRequest postRequest)
        {
            Post post = new Post { Id = postRequest.Id };
            if (post.Id != Guid.Empty)
            {
                post.Id = Guid.NewGuid();
            }

            _postService.GetPosts().Add(post);

            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            string locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());
            PostResponse response = new PostResponse { Id = post.Id };
            return Created(locationUri, response);
        }
    }
}
