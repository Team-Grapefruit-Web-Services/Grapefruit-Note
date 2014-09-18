namespace GrapefruitNote.Services.Controllers
{
    using GrapefruitNote.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using GrapefruitNote.Mappers;
    using GrapefruitNote.DataTransferObjects;
    using GrapefruitNote.Models;
    
    public class CategoryController : BaseApiController
    {
        public CategoryController()
            :this(new GrapefruitNoteData())
        {
        }

        public CategoryController(IGrapefruitNoteData data)
            :base(data)
        {
        }

        [HttpGet, ActionName("categories")]
        public IHttpActionResult All(string sessionKey)
        {
            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Invalid user");
            }

            var allCategories = this.data.Categories
                .All()
                .Where(c => c.UserId == currentUser.UserId)
                .Select(CategoryMapper.ToCategoryModel);

            return Ok(allCategories);
        }

        [HttpPost, ActionName("create")]
        public IHttpActionResult Create(string sessionKey, CategoryModel categoryModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Invalid user");
            }

            var compiledModelToEntityExpression = CategoryMapper.ToCategoryEntity.Compile();
            var newCategory = compiledModelToEntityExpression(categoryModel);
            newCategory.UserId = currentUser.UserId;

            this.data.Categories.Add(newCategory);
            this.data.SaveChanges();

            categoryModel.CategoryId = newCategory.CategoryId;

            return Ok(categoryModel);
        }

        [HttpPut, ActionName("update")]
        public IHttpActionResult Update(string sessionKey, int id, CategoryModel categoryModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var existingCategory = this.GetCategoryById(id);

            if (existingCategory == null)
            {
                return BadRequest("No category with such id exists.");
            }

            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Invalid user");
            }

            existingCategory.Name = categoryModel.Name;
            this.data.SaveChanges();

            return Ok(categoryModel);
        }

        [HttpDelete, ActionName("delete")]
        public IHttpActionResult Delete(string sessionKey, int id)
        {
            var existingCategory = this.GetCategoryById(id);

            if (existingCategory == null)
            {
                return BadRequest("No category with such id exists.");
            }

            var currentUser = this.GetUserBySessionKey(sessionKey);

            if (currentUser == null)
            {
                return BadRequest("Invalid user");
            }

            this.data.Categories.Delete(existingCategory);
            this.data.SaveChanges();

            return this.Ok();
        }

        private Category GetCategoryById(int id)
        {
            return this.data.Categories.All().FirstOrDefault(c => c.CategoryId == id);
        }

        private User GetUserBySessionKey(string sessionKey)
        {
            return this.data.Users.All().FirstOrDefault(u => u.SessionKey == sessionKey);
        }
    }
}