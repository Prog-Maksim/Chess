using Backend.Models.DB;
using Backend.Models.Request;
using Backend.Models.Response;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace Backend.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    
    private readonly PasswordHasher<Person> _passwordHasher = new();

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<BaseResponse> RegisterUserAsync(RegistrationUser user)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
        
        if (existingUser != null)
            return new BaseResponse { Message = "Пользователь с данным email уже существует", Error = "Forbidden", StatusCode = 403 };

        Person person = new Person
        {
            PersonId = Guid.NewGuid().ToString(),
            Nickname = user.Nickname,
            Email = user.Email,
        };
        person.Password = _passwordHasher.HashPassword(person, user.Password);

        UserData data = new UserData
        {
            PersonId = person.PersonId,
            Score = 0,
            UnlockedPotions = new List<string>(),
            GamesPlayerd = 0,
            Level = 1,
            Rating = 0,
            NumberOfWins = 0
        };
        
        await _userRepository.AddUserAsync(person, data);

        var accessToken = JwtService.GenerateJwtToken(person.PersonId, person.Nickname);

        return new Token { Success = true, Message = "Вы успешно создали аккаунт!", AccessToken = accessToken, PersonId = person.PersonId };
    }

    public async Task<BaseResponse> LoginUserAsync(AuthUser user)
    {
        var person = await _userRepository.GetUserByEmailAsync(user.Email);
        
        if (person == null)
            return new BaseResponse { Message = "Данный пользователь не найден", Error = "NotFound", StatusCode = 404 };
        if (_passwordHasher.VerifyHashedPassword(person, person.Password, user.Password) != PasswordVerificationResult.Success)
            return new BaseResponse { Message = "Логин или пароль не верен!", Error = "Forbidden", StatusCode = 403 };

        var accessToken = JwtService.GenerateJwtToken(person.PersonId, person.Nickname);
        
        return new Token { Success = true, Message = "Вы успешно авторизовались!", AccessToken = accessToken, PersonId = person.PersonId };
    }

    public async Task<BaseResponse> UpdatePasswordAsync(string playerId, string oldPassword, string newPassword)
    {
        var person = await _userRepository.GetUserByIdAsync(playerId);
        
        if (person == null)
            return new BaseResponse { Message = "Данный пользователь не найден", Error = "NotFound", StatusCode = 404 };
        
        if (_passwordHasher.VerifyHashedPassword(person, person.Password, oldPassword) != PasswordVerificationResult.Success)
            return new BaseResponse { Message = "Пароль не верен!", Error = "Forbidden", StatusCode = 403 };

        await _userRepository.UpdatePasswordAsync(playerId, newPassword);
        return new BaseResponse { Success = true, Message = "Пароль успешно обновлен!"};
    }
}