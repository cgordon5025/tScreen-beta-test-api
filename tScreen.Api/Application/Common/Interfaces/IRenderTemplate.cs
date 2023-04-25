namespace Application.Common.Interfaces;

public interface IRenderTemplate
{
    /// <summary>
    /// The template file name found in the base path <see cref="BasePath"/>
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// An array of paths inside of the executing assembly directory. The array of paths is
    /// transformed to a path that correctly uses the folder path separator of the executing
    /// environment
    /// </summary>
    public string BasePath { get; }
}

public interface IRenderTemplatePartial
{
    /// <summary>
    /// The template file name found in the base path <see cref="BasePath"/>
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// An array of paths inside of the executing assembly directory. The array of paths is
    /// transformed to a path that correctly uses the folder path separator of the executing
    /// environment
    /// </summary>
    public string BasePath { get; }
}


public interface IRenderTemplateStyle
{
    /// <summary>
    /// The template file name found in the base path <see cref="BasePath"/>
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// An array of paths inside of the executing assembly directory. The array of paths is
    /// transformed to a path that correctly uses the folder path separator of the executing
    /// environment
    /// </summary>
    public string BasePath { get; }
}