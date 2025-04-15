using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Interface
{
    public interface IUsers
    {
        public Task<Response<dynamic>> CreateUser(RegisterUser registerUser);
        //public Task<Response<dynamic>> Login(UserLogin model);
        public Task<Response<dynamic>> GetUserProfile(Guid userId);
        public Task<Response<dynamic>> UpdateUserProfile(Guid userId, UserUpdateRequest updateRequest);
        public Task<Response<dynamic>> LoginUser(UserLogin loginRequest);


    }
}
