public interface ICategoryRepository
{
    IEnumerable<Category> GetCategories();
    Category? GetCategoryById(int id);
    bool CreateCategory(Category category);
    bool UpdateCategory(Category category);
    bool DeleteCategory(int id);
    IEnumerable<CategoryWithProductCount> GetCategoriesWithProductCount();
}
