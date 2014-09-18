namespace GrapefruitNote.Web.Controllers
{
    using GrapefruitNote.Data;
    using GrapefruitNote.Web.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using GrapefruitNote.Mappers;
    using GrapefruitNote.DataTransferObjects;
    using GrapefruitNote.Models;

    [Authorize]
    public class CategoryController : BaseApiController
    {
        private readonly IUserIdProvider userIdProvider;

        public CategoryController()
            :this(new GrapefruitNoteData(), new AspNetUserIdProvider())
        {

        }

        public CategoryController(IGrapefruitNoteData data, IUserIdProvider userIdProvider)
            :base(data)
        {
            this.userIdProvider = userIdProvider;
        }

        [HttpGet]
        public IHttpActionResult All()
        {
            var currentUserId = this.userIdProvider.GetUserId();

            var allCategories = this.data.Categories
                .All()
                .Where(c => c.UserId == currentUserId)
                .Select(CategoryMapper.ToCategoryModel);
            return Ok(allCategories);
        }

        [HttpPost]
        public IHttpActionResult Create(CategoryModel categoryModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var currentUserId = this.userIdProvider.GetUserId();

            var compiledModelToEntityExpression = CategoryMapper.ToCategoryEntity.Compile();
            var newCategory = compiledModelToEntityExpression(categoryModel);
            newCategory.UserId = currentUserId;

            this.data.Categories.Add(newCategory);
            this.data.SaveChanges();

            categoryModel.CategoryId = newCategory.CategoryId;

            return Ok(categoryModel);
        }

        [HttpPut]
        public IHttpActionResult Update(int id, CategoryModel categoryModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var existingCategory = this.data.Categories.All().FirstOrDefault(c => c.CategoryId == id);

            if (existingCategory == null)
            {
                return BadRequest("No category with such id exists.");
            }

            existingCategory.Name = categoryModel.Name;
            this.data.SaveChanges();

            return Ok(categoryModel);
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var existingCategory = this.data.Categories.All().FirstOrDefault(c => c.CategoryId == id);

            if (existingCategory == null)
            {
                return BadRequest("No category with such id exists.");
            }

            this.data.Categories.Delete(existingCategory);
            this.data.SaveChanges();

            return this.Ok();
        }
    }
}