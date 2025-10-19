using Task3.Models.Entities;

namespace Task3.Services.Interfaces;

public interface IDisplayer
{
    void Display(RendererBlueprint options);
}