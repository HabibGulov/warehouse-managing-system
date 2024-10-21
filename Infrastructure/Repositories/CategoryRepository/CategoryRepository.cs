using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly string _pathData;

    public CategoryRepository(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration("1.0", "utf-8", "true");
            XElement xElement = new XElement(XmlElements.DataSource, new XElement(XmlElements.Categories));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }
    public IEnumerable<Category> GetCategories()
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            return document.Root!.Element(XmlElements.Categories)!.Elements(XmlElements.Category)
                .Select(x => new Category
                {
                    Id = (int)x.Element(XmlElements.CategoryId)!,
                    Name = (string)x.Element(XmlElements.CategoryName)!,
                    Description = (string)x.Element(XmlElements.CategoryDescription)!
                }).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Category? GetCategoryById(int id)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);
            var category = xDocument.Root!.Element(XmlElements.Categories)!.Elements(XmlElements.Category)
                .Where(x => (int)x.Element(XmlElements.CategoryId)! == id)
                .Select(x => new Category
                {
                    Id = (int)x.Element(XmlElements.CategoryId)!,
                    Name = (string)x.Element(XmlElements.CategoryName)!,
                    Description = (string)x.Element(XmlElements.CategoryDescription)!
                }).FirstOrDefault();
            return category;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public bool CreateCategory(Category category)
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            int maxId = document.Element(XmlElements.DataSource)!.Element(XmlElements.Categories)!.Elements(XmlElements.Category)
                .Select(x => (int)x.Element(XmlElements.CategoryId)!).DefaultIfEmpty(0).Max();

            bool isNameExists = document.Element(XmlElements.DataSource)!.Element(XmlElements.Categories)!
                .Elements(XmlElements.Category).Any(x => (string)x.Element(XmlElements.CategoryName)! == category.Name);
            if (isNameExists) return false;

            XElement xmlXElement = new XElement(XmlElements.Category,
                new XElement(XmlElements.CategoryId, maxId + 1),
                new XElement(XmlElements.CategoryName, category.Name),
                new XElement(XmlElements.CategoryDescription, category.Description));

            document.Element(XmlElements.DataSource)!.Element(XmlElements.Categories)!.Add(xmlXElement);
            document.Save(_pathData);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public bool UpdateCategory(Category category)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);
            var categoryToUpdate = xDocument.Root!.Element(XmlElements.Categories)!.Elements(XmlElements.Category)
                .FirstOrDefault(x => (int)x.Element(XmlElements.CategoryId)! == category.Id);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.SetElementValue(XmlElements.CategoryName, category.Name);
                categoryToUpdate.SetElementValue(XmlElements.CategoryDescription, category.Description);

                xDocument.Save(_pathData);
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public bool DeleteCategory(int id)
    {
        try
        {
            XDocument document = XDocument.Load(_pathData);
            var categoryToDelete = document.Root!.Element(XmlElements.Categories)!.Elements(XmlElements.Category)
                .FirstOrDefault(x => (int)x.Element(XmlElements.CategoryId)! == id);
            if (categoryToDelete != null)
            {
                categoryToDelete.Remove();
                document.Save(_pathData);
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    public IEnumerable<CategoryWithProductCount> GetCategoriesWithProductCount()
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);

            var categories = from category in xDocument.Element(XmlElements.DataSource)!.Element(XmlElements.Categories)!.Elements(XmlElements.Category)
                             let categoryId = (int)category.Element(XmlElements.CategoryId)!
                             let productCount = xDocument.Element(XmlElements.DataSource)!.Element("products")!.Elements("product")
                                 .Count(p => (int)p.Element(XmlElements.CategoryId)! == categoryId)
                             select new CategoryWithProductCount
                             {
                                 Id = categoryId,
                                 Name = category.Element(XmlElements.CategoryName)?.Value!,
                                 ProductCount = productCount
                             };

            return categories;
        }
        catch(Exception e)
        {
            System.Console.WriteLine(e.Message);
            return new List<CategoryWithProductCount>();
        }
    }
}

file class XmlElements
{
    public const string PathData = "PathData";
    public const string DataSource = "source";
    public const string Categories = "categories";
    public const string Category = "category";
    public const string CategoryId = "id";
    public const string CategoryName = "name";
    public const string CategoryDescription = "description";
}
