﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Service
{
    public class UsersService : IUsers
    {
        private readonly TaskMagDbContext _Context;
        private ILogger<UsersService> _logger;
        public UsersService(TaskMagDbContext context, ILogger<UsersService> logger)
        {
            _Context = context;
            _logger = logger;
        }

        public async Task<Response2<int>> CreateUser(RegisterUser registerUser)
        {
            var User = await _Context.UserTables.FirstOrDefaultAsync(u => u.Email == registerUser.Email);
            if (User != null)
            {
                return new Response2<int>
                {
                    StatusCode = "96",
                    StatusMessage = "This User has already exist"
                };
            }
            var newUser = new UserTable
            { 
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Email = registerUser.Email,
                Password = registerUser.Password              
            };
            await _Context.UserTables.AddAsync(newUser);
            _Context.SaveChanges();
            return new Response2<int>
            {
                StatusCode = "00",
                StatusMessage = "Success",
                Data = newUser.Id
            };
        }

        public async Task<dynamic> Login(UserLogin request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return new Response
                    {
                        StatusCode = "96",
                        StatusMessage = "UserNameand and Password required"
                    };
                }
                var user = await _Context.UserTables.FirstOrDefaultAsync(s => s.Email == request.Email);
                if (user == null)
                {
                    return new Response
                    {
                        StatusCode = "96",
                        StatusMessage = "Invalid details"
                    };
                }
                if (user.Password != request.Password)
                {
                    return new Response
                    {
                        StatusCode = "96",
                        StatusMessage = "Invalid details"
                    };
                }
                return new Response
                {
                    StatusCode = "00",
                    StatusMessage = " Login Successful"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during login {ex.Message}");
                return new Response
                {
                    StatusCode = "99",
                    StatusMessage = "An Error occurrrd while login "
                };
            };
        }
    }
}
