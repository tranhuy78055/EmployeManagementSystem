﻿using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Repositories.Implementations
{
    public class UserAccountRepository(IOptions<JwtSection> config, AppDbContext appDbContext) : IUserAccount
    {
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
           if (user is null) return new GeneralResponse(false, "Model is empty");

            var checkUser = await FindUserByEmail(user.Email!);
            if (checkUser != null) return new GeneralResponse(false, "User already exists");

            //Save user to database
            var applicationUser = await AddToDatabase( new ApplicationUser
            {
                Email = user.Email,
                Fullname = user.Fullname,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });
            // Check , create and assign role to user
            var checkAdminRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(x => x.Name!.ToLower()!.Equals(Constants.Admin));
            if (checkAdminRole is null)
            {
                var createAdminRole = await AddToDatabase(new SystemRole
                {
                    Name = Constants.Admin
                });
                await AddToDatabase(new UserRole() { RoleId = createAdminRole.Id, UserId = applicationUser.Id });
                return new GeneralResponse(true, "User created successfully");
            }

            var checkUserRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(x => x.Name!.ToLower()!.Equals(Constants.User));
            SystemRole response = new();
            if (checkUserRole is null)
            {
              response = await AddToDatabase(new SystemRole
              {
                  Name = Constants.User
              });
              await AddToDatabase(new UserRole() { RoleId = response.Id, UserId = applicationUser.Id });
            }
            else
            {
                await AddToDatabase(new UserRole() { RoleId = checkUserRole.Id, UserId = applicationUser.Id });
            }
            return new GeneralResponse(true, "User created successfully");
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
          if (user is null) return new LoginResponse(false, "Model is empty");
            var applicationUser = await FindUserByEmail(user.Email!);
            if (applicationUser is null) return new LoginResponse(false, "User does not exist");

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password)) return new LoginResponse(false, "Email or Password not valid");
            
            var getUserRole = await FindUserRole(applicationUser.Id);
            if (getUserRole is null) return new LoginResponse(false, "User does not have a role");

            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getRoleName is null) return new LoginResponse(false, "User does not have a role");

            string jwtToken = GenerateToken(applicationUser, getRoleName!.Name!);
            string refreshToken = GenerateRefreshToken();
            // Save the refresh token to database
            var findUser = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(x => x.UserId == applicationUser.Id);
            if (findUser is not null)
            {
                findUser!.Token = refreshToken;
                await appDbContext.SaveChangesAsync();
            }
            else
            {
                await AddToDatabase(new RefreshTokenInfo
                {
                    Token = refreshToken,
                    UserId = applicationUser.Id
                });
            }

            return new LoginResponse(true, "Login successful", jwtToken, refreshToken);


        }

        private string GenerateToken(ApplicationUser user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Fullname!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(
                issuer:config.Value.Issuer,
                audience:config.Value.Audience,
                claims:userClaims,
                expires: DateTime.Now.AddSeconds(2),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
      
        private async Task<ApplicationUser?> FindUserByEmail(string email)
        {
            return await appDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Email!.ToLower()!.Equals(email!.ToLower()));
        }
        private async Task<UserRole?> FindUserRole(int userId)
        {
            return await appDbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
        }
        private async Task<SystemRole?> FindRoleName(int roleId)
        {
            return await appDbContext.SystemRoles.FirstOrDefaultAsync(x => x.Id == roleId);
        }
        private async Task<T> AddToDatabase<T>(T model)
        { var result = appDbContext.Add(model!);
            await appDbContext.SaveChangesAsync();
            return (T)result.Entity;


        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            if (token is null) return new LoginResponse(false, "Model is empty");

            var findToken= await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(x => x.Token!.Equals(token.Token));
            if (findToken is null) return new LoginResponse(false, "Refresh token is required");

            // get user details
            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == findToken.UserId);
            if (user is null) return new LoginResponse(false, "Refresh token could not be generated because user not found");
            
            var userRole = await FindUserRole(user.Id);
            var roleName = await FindRoleName(userRole!.RoleId);
            string jwtToken = GenerateToken(user, roleName!.Name!);
            string refreshToken = GenerateRefreshToken();

            var updateRefreshToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(x=>x.UserId == user.Id);
            if (updateRefreshToken is null) return new LoginResponse(false, "Refresh token could not be generated because user has not signed in");

            updateRefreshToken.Token = refreshToken;
            await appDbContext.SaveChangesAsync();
            return new LoginResponse(true, "Refresh token generated successfully", jwtToken, refreshToken);
        }

    }
}
