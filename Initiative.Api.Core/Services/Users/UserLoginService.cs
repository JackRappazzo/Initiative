﻿using Initiative.Api.Core.Identity;
using Initiative.Api.Core.Services.Authentication;
using Initiative.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Initiative.Api.Core.Services.Users
{
    public class UserLoginService : IUserLoginService
    {
        IUserManager<Core.Identity.ApplicationIdentity> userManager;
        IJwtService jwtService;

        public UserLoginService(IUserManager<Core.Identity.ApplicationIdentity> userManager, IJwtService jwtService)
        {
            this.userManager = userManager;
            this.jwtService = jwtService;
        }

        public async Task<LoginResult> LoginAndFetchTokens(string email, string password, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = await userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new LoginResult()
                    {
                        ErrorType = LoginErrorType.EmailDoesNotExist,
                        Success = false,
                        Jwt = null,
                        RefreshToken = null
                    };
                }
                else
                {

                    var passwordMatch = await userManager.CheckPasswordAsync(user, password);
                    if (passwordMatch)
                    {
                        var jwt = jwtService.GenerateToken(user);

                        var refresh = await jwtService.GenerateAndStoreRefreshToken(user, DateTime.UtcNow.AddDays(60), cancellationToken);

                        return new LoginResult()
                        {
                            Success = true,
                            ErrorType = LoginErrorType.None,
                            Jwt = jwt,
                            RefreshToken = refresh.RefreshToken
                        };
                    }
                    else
                    {
                        return new LoginResult()
                        {
                            ErrorType = LoginErrorType.PasswordMismatch,
                            Success = false,
                            Jwt = null,
                            RefreshToken = null
                        };
                    }
                }
            }
            else
            {
                throw new ArgumentException("Missing arguments");
            }
        }
    }
}
