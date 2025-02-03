using Microsoft.AspNetCore.Identity;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Interface
{
    public interface IUsers
    {
        public Task<Response2<dynamic>> CreateUser(RegisterUser registerUser);
        public Task<dynamic> Login(UserLogin request);
       

    }
}
