using Microsoft.AspNetCore.Identity;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Interface
{
    public interface IUsers
    {
        public Task<Response<dynamic>> CreateUser(RegisterUser registerUser);
        public Task<Response<dynamic>> Login(UserLogin model);
       

    }
}
