namespace GrapefruitNote.Mappers
{
    using GrapefruitNote.DataTransferObjects;
    using GrapefruitNote.Models;
    using System;
    using System.Linq.Expressions;

    public class CategoryMapper
    {
        public static Expression<Func<Category, CategoryModel>> ToCategoryModel
        {
            get
            {
                return category => new CategoryModel
                {
                    Name = category.Name,
                    CategoryId = category.CategoryId
                };
            }
        }

        public static Expression<Func<CategoryModel, Category>> ToCategoryEntity
        {
            get
            {
                return categoryModel => new Category
                {
                    Name = categoryModel.Name,
                };
            }
        }
    }
}
