namespace ChatAppServer.WebAPI.Dtos;

public sealed record class RegisterDto(
    string Name,
    IFormFile File);

