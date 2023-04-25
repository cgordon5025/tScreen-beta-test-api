using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IPdfService<T>
{ 
    public T Renderer { get; }
    
    void SaveDocument(string name, string contents);
}