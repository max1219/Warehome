namespace Warehome.Domain.Entities;

public class Category<T>
{
    public string Path { get; init; }

    public string Name
    {
        get
        {
            if (_name == null)
            {
                int lastIndex = Path.LastIndexOf('/');
                _name = lastIndex >= 0 ? Path.Substring(lastIndex + 1) : Path;
            }

            return _name;
        }
    }

    private string? _name;

    public static Category<T> FromNameAndParent(string name, string parentPath)
    {
        return new Category<T> {Path = $"{parentPath}/{name}"};
    }
}