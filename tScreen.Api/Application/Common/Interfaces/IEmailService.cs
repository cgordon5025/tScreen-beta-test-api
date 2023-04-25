using System.Threading.Tasks;
using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IEmailService
{
    public Task SendAsync(string to, string from, string subject, string body, string tag= "general");
    public Task SendAsync<T>(string template, Email<T> email, string tag = "general") where T : IEmailData;
}