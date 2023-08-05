namespace MyDev.BinanceApi.Auth
{
    public interface IUserRepository
    {
        UserDto GetUser(UserModel userModel);
    }
}
