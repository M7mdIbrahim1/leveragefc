using Backend.Auth;
using Backend.Helper;
using Backend.Models.AuthViewModels;
using Backend.Models;
using Backend.Models.CommonViewModels;
using Backend.Models.CompanyViewModels;
using Backend.Models.LineOfBusinessViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.Services;
using System.Linq.Expressions;
using AutoMapper;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("/")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ICompanyService companyService;
        private readonly ILineOfBusinessService lineOfBusinessService;
        private readonly IMapper mapper;

        private readonly ApplicationDbContext applicationDbContext;

        public AuthenticateController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ICompanyService companyService,
            ILineOfBusinessService lineOfBusinessService,
            IMapper mapper,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            this.companyService = companyService;
            this.lineOfBusinessService = lineOfBusinessService;
            this.mapper = mapper;
            this.applicationDbContext = applicationDbContext;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password) && user.IsActive && !user.IsDeleted)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = CreateToken(authClaims);
                var refreshToken = GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                await _userManager.UpdateAsync(user);

                return Ok(new
                {
                    user,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                IsActive = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });


            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));


            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterViewModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                IsActive = true,

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.FDBAdmin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.FDBAdmin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.FDBAdmin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.FDBAdmin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        // [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route("register-superadmin")]
        public async Task<IActionResult> RegisterSuperAdmin([FromBody] RegisterViewModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                IsActive = true,

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.SuperAdmin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.SuperAdmin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.SuperAdmin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.SuperAdmin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenViewModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken;
            //string? refreshToken = tokenModel.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string username = principal.Identity.Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            var user = await _userManager.FindByNameAsync(username);
            //|| user.RefreshToken != refreshToken
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            var newAccessToken = CreateToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
                //user,
                Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken,
                //Expiration = token.ValidTo
                // accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                // refreshToken = newRefreshToken
            });
        }

        // [Authorize(Roles = "User")]
        [Authorize]
        [HttpPost]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (user == null) return BadRequest("Invalid user name");

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        // [Authorize(Roles = "User")]
        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route("EditUser")]
        public async Task<IActionResult> EditUser(UserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return BadRequest("Invalid user Id");

            user.IsActive = model.IsActive.Value;
            await _userManager.UpdateAsync(user);

            return NoContent();

        }

        [Authorize]
        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }

            return Ok(new Response { Status = "Success", Message = "User updated successfully!" });
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route("assign-roles")]
        public async Task<IActionResult> AssignRolesToUsers([FromBody] UserRolesViewModel model)
        {
            foreach (var userId in model.UserIds)
            {

                var user = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                //if (user == null)
                //return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User doesn't exists!" });

                foreach (string role in model.Roles)
                {

                    if (!await _roleManager.RoleExistsAsync(role))
                        await _roleManager.CreateAsync(new IdentityRole(role));


                    if (await _roleManager.RoleExistsAsync(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }
                if (await _roleManager.RoleExistsAsync(UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route("assign-company-to-users")]
        public async Task<IActionResult> AssignCompanyToUsers([FromBody] CompanyToUsersViewModel model)
        {
            var lineOfBusinesses = await lineOfBusinessService.GetLineOfBusinessesByIds(model.LineOfBusinessIds.ToList());
            var company = await companyService.GetCompanyById(model.CompanyId);
            foreach (var userId in model.UserIds)
            {

                //var user = await _userManager.FindByIdAsync(userId);


                var dbUser = applicationDbContext.Users.Where(x => x.Id == userId).Include(p => p.LineOfBusinesses).Include(p => p.Company).First();

                var lobs = dbUser.LineOfBusinesses.Select(x => x.Id);

                foreach (var item in lobs)
                {
                    dbUser.LineOfBusinesses.Remove(dbUser.LineOfBusinesses.First(x => x.Id == item));
                }

                dbUser.CompanyId = company.Id;
                dbUser.Company = null;
                dbUser.Company = new Company()
                {
                    Id = company.Id.Value,
                    Name = company.Name,
                    Description = company.Description,
                    OwnerId = company.OwnerId,
                };
                dbUser.LineOfBusinesses = lineOfBusinesses;

                // await _userManager.UpdateAsync(user);

                applicationDbContext.SaveChanges();



                // if (user != null)
                // {
                //     // user.CompanyId = model.CompanyId;
                //     user.LineOfBusinesses = lineOfBusinesses;
                //     // user.Company = new Company()
                //     // {
                //     //     Id = company.Id.Value,
                //     //     Name = company.Name,
                //     //     Description = company.Description,
                //     //     OwnerId = company.OwnerId,
                //     // };
                //     // user.LineOfBusinesses = lineOfBusinesses.Select(x => new LineOfBusiness()
                //     // {
                //     //     Id = x.Id,
                //     //     Name = x.Name,
                //     //     Description = x.Description,
                //     //     CompanyId = x.CompanyId,
                //     //     IsRetainer = x.IsRetainer,
                //     // }).ToList();
                // }
                // await _userManager.UpdateAsync(user);
            }


            return Ok(new Response { Status = "Success", Message = "Users updated successfully!" });
        }

        [Authorize]
        [HttpGet]
        [Route("get-user/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User doesn't exists!" });

            var userModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                //  Contact = user.PhoneNumber
            };


            return Ok(new
            {
                userModel
            });
        }

        [Authorize]
        [HttpPost]
        [Route("check-auth/")]
        public async Task<IActionResult> checkAuth()
        {
            //var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            return Ok();
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin)]
        [HttpGet]
        [Route("get-all-roles/")]
        public async Task<IActionResult> GetAllRoles()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var userRoles = await _userManager.GetRolesAsync(user);

            var roles = new List<string>();
            if (userRoles.Contains(UserRoles.SuperAdmin))
            {
                roles.Add(UserRoles.SuperAdmin);
            }
            roles.Add(UserRoles.FDBAdmin);
            roles.Add(UserRoles.CompanyAdmin);
            roles.Add(UserRoles.CommercialManager);
            roles.Add(UserRoles.CommercialUser);
            roles.Add(UserRoles.OperationManager);
            roles.Add(UserRoles.OperationUser);
            roles.Add(UserRoles.FinancialManager);
            roles.Add(UserRoles.FinancialUser);

            return Ok(new
            {
                roles
            });
        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.User)]
        [HttpGet]
        [Route("get-admin-users")]
        public async Task<ICollection<UserViewModel>> GetAllAdminUsers()
        {
            return await Task.Run(() =>
                 {
                     return _userManager.GetUsersInRoleAsync(UserRoles.CompanyAdmin).Result.Select(user => new UserViewModel()
                     {
                         //do your variable mapping here 
                         Id = user.Id,
                         UserName = user.UserName,
                         Email = user.Email,
                         IsActive = user.IsActive,
                         //   Contact = user.PhoneNumber
                     }).ToList();
                 });


        }

        [Authorize(Roles = UserRoles.SuperAdmin + ", " + UserRoles.FDBAdmin + ", " + UserRoles.CompanyAdmin + ", " + UserRoles.User)]
        [HttpPost]
        [Route("get-all-users")]
        public async Task<ICollection<UserViewModel>> GetAllUsers([FromBody] UserSearchViewModel<ApplicationUser> userSearchViewModel)
        {
            //TODO: handle user null

            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var userRoles = await _userManager.GetRolesAsync(user);


            Paging<ApplicationUser> paging = null;

            if (userSearchViewModel.Page != 0)
            {
                paging = new Paging<ApplicationUser>
                {
                    Skip = (userSearchViewModel.Page - 1) * userSearchViewModel.PageSize,
                    OrderBy = userSearchViewModel.orderBy != null ? userSearchViewModel.orderBy : x => x,
                    PageSize = userSearchViewModel.PageSize
                };
            }
            var roles = userSearchViewModel.roles != null && userSearchViewModel.roles.Count > 0 ? applicationDbContext.Roles.Where(x => userSearchViewModel.roles.Contains(x.Name)) : null;

            if (userRoles.Contains(UserRoles.SuperAdmin) || userRoles.Contains(UserRoles.FDBAdmin))
            {

                paging.TotalCount = await Task.Run(() =>
                     {
                         //.Where(x => userSearchViewModel.companiesIds.Contains(x.CompanyId.Value)).Where(x => x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Any(p2 => p2 == p.Id)))

                         return applicationDbContext.Users.Include(x => x.Company).Include(x => x.LineOfBusinesses).Where(x => userSearchViewModel.emailOrUserName != null || userSearchViewModel.emailOrUserName == "" ? x.Email.ToLower().Contains(userSearchViewModel.emailOrUserName.ToLower()) || x.UserName.Contains(userSearchViewModel.emailOrUserName) : 1 == 1)
                            .Where(x => userSearchViewModel.companiesIds != null && userSearchViewModel.companiesIds.Count > 0 ? userSearchViewModel.companiesIds.Contains(x.CompanyId.Value) : 1 == 1)
                            .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Any(p2 => p2 == p.Id)) : 1 == 1)
                       // .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? (userSearchViewModel.lineOfBusinessesIds.Count == 1 ? x.CompanyId.Value == userSearchViewModel.lineOfBusinessesIds.First() : x.LineOfBusinesses.Select(x => x.Id).Contains(userSearchViewModel.lineOfBusinessesIds.Last())) : 1 == 1)
                       // .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Skip(1).Any(p2 => p2 == p.Id)) : 1 == 1)
                       .Where(x => roles != null ? applicationDbContext.UserRoles.Any(r => roles.Any(y => y.Id == r.RoleId) && r.UserId == x.Id) : 1 == 1)
                          .Count();



                     });

                return await Task.Run(() =>
                 {


                     //.Where(x => userSearchViewModel.companiesIds.Contains(x.CompanyId.Value)).Where(x => x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Any(p2 => p2 == p.Id)))
                     //Skip(paging.Skip).Take(paging.PageSize)
                     var users = applicationDbContext.Users.Include(x => x.Company).Include(x => x.LineOfBusinesses).Where(x => userSearchViewModel.emailOrUserName != null || userSearchViewModel.emailOrUserName == "" ? x.Email.Contains(userSearchViewModel.emailOrUserName) || x.UserName.Contains(userSearchViewModel.emailOrUserName) : 1 == 1)
                            .Where(x => userSearchViewModel.companiesIds != null && userSearchViewModel.companiesIds.Count > 0 ? userSearchViewModel.companiesIds.Contains(x.CompanyId.Value) : 1 == 1)
                            .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Any(p2 => p2 == p.Id)) : 1 == 1)
                     // .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? (userSearchViewModel.lineOfBusinessesIds.Count == 1 ? x.CompanyId.Value == userSearchViewModel.lineOfBusinessesIds.First() : x.LineOfBusinesses.Select(x => x.Id).Contains(userSearchViewModel.lineOfBusinessesIds.Last())) : 1 == 1)
                     // .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Skip(1).Any(p2 => p2 == p.Id)) : 1 == 1)
                     .Where(x => roles != null ? applicationDbContext.UserRoles.Any(r => roles.Any(y => y.Id == r.RoleId) && r.UserId == x.Id) : 1 == 1)
                     .OrderByDescending(paging.OrderBy).Skip(paging.Skip).Take(paging.PageSize).Select(user => new UserViewModel()
                     {
                         //do your variable mapping here 
                         Id = user.Id,
                         UserName = user.UserName,
                         Email = user.Email,
                         IsActive = user.IsActive,
                         TotalCount = paging.TotalCount,
                         //  UserRoles = user.UserRoles.Select(x => x.Name).ToList(),
                         CompanyId = user.CompanyId,
                         //TODO: handle companyId null
                         Company = new CompanyViewModel()
                         {
                             Id = user.Company.Id,
                             Name = user.Company.Name,
                         },
                         //LineOfBusinessId = user.LineOfBusinessId,
                         //TODO: handle line of business null
                         LineOfBusinesses = user.LineOfBusinesses.Select(x => new LineOfBusinessViewModel()
                         {
                             Id = x.Id,
                             Name = x.Name
                         }).ToList(),


                         //   Contact = user.PhoneNumber
                     }).ToList();

                     //  if (userSearchViewModel.roles != null && userSearchViewModel.roles.Count > 0)
                     //  {

                     //      //  users = users.Where(x => x.UserRoles.Any(p => userSearchViewModel.roles.Any(p2 => p2 == p))).ToList();
                     //      users = users.Where(x => applicationDbContext.UserRoles.Any(r => roles.Any(y => y.Id == r.RoleId) && r.UserId == x.Id)).ToList();
                     //  }

                     //  if (userSearchViewModel.companiesIds != null && userSearchViewModel.companiesIds.Count > 0)
                     //  {
                     //      users = users.Where(x => userSearchViewModel.companiesIds.Contains(x.CompanyId.Value)).ToList();
                     //  }

                     //  if (userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0)
                     //  {
                     //      users = users.Where(x => x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Any(p2 => p2 == p.Id))).ToList();
                     //  }
                     var userRoles = applicationDbContext.UserRoles.Where(x => users.Select(y => y.Id).Contains(x.UserId)).ToList();
                     foreach (var user in users)
                     {
                         user.UserRoles = userRoles.Where(x => x.UserId == user.Id).Select(
                             x => applicationDbContext.Roles.First(y => y.Id == x.RoleId).Name
                         ).ToList();
                     }
                     return users;
                 });
            }
            else //CompanyAdmin
            {
                var companies = await companyService.GetCompanies(new CompanySearchViewModel()
                {
                    ownerId = user.Id
                }, null, false);
                var companiesIds = companies.Select(x => x.Id);

                paging.TotalCount = await Task.Run(() =>
                    {
                        return _userManager.Users.Include(x => x.Company).Include(x => x.LineOfBusinesses).Where(x => companiesIds.Contains(x.CompanyId)).Where(x => userSearchViewModel.emailOrUserName != null || userSearchViewModel.emailOrUserName == "" ? x.Email.Contains(userSearchViewModel.emailOrUserName) || x.UserName.Contains(userSearchViewModel.emailOrUserName) : 1 == 1)
                            .Where(x => userSearchViewModel.companiesIds != null && userSearchViewModel.companiesIds.Count > 0 ? userSearchViewModel.companiesIds.Contains(x.CompanyId.Value) : 1 == 1)
                            .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Any(p2 => p2 == p.Id)) : 1 == 1)
                     // .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? (userSearchViewModel.lineOfBusinessesIds.Count == 1 ? x.CompanyId.Value == userSearchViewModel.lineOfBusinessesIds.First() : x.LineOfBusinesses.Select(x => x.Id).Contains(userSearchViewModel.lineOfBusinessesIds.Last())) : 1 == 1)
                     //  .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Skip(1).Any(p2 => p2 == p.Id)) : 1 == 1)
                     .Where(x => roles != null ? applicationDbContext.UserRoles.Any(r => roles.Any(y => y.Id == r.RoleId) && r.UserId == x.Id) : 1 == 1)
                       .Count();
                    });


                return await Task.Run(() =>
            {
                //Skip(paging.Skip).Take(paging.PageSize)
                var users = _userManager.Users.Include(x => x.Company).Include(x => x.LineOfBusinesses).Where(x => companiesIds.Contains(x.CompanyId)).Where(x => userSearchViewModel.emailOrUserName != null || userSearchViewModel.emailOrUserName == "" ? x.Email.Contains(userSearchViewModel.emailOrUserName) || x.UserName.Contains(userSearchViewModel.emailOrUserName) : 1 == 1)
                            .Where(x => userSearchViewModel.companiesIds != null && userSearchViewModel.companiesIds.Count > 0 ? userSearchViewModel.companiesIds.Contains(x.CompanyId.Value) : 1 == 1)
                            .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Any(p2 => p2 == p.Id)) : 1 == 1)
                     // .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? (userSearchViewModel.lineOfBusinessesIds.Count == 1 ? x.CompanyId.Value == userSearchViewModel.lineOfBusinessesIds.First() : x.LineOfBusinesses.Select(x => x.Id).Contains(userSearchViewModel.lineOfBusinessesIds.Last())) : 1 == 1)
                     //  .Where(x => userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0 ? x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Skip(1).Any(p2 => p2 == p.Id)) : 1 == 1)
                     .Where(x => roles != null ? applicationDbContext.UserRoles.Any(r => roles.Any(y => y.Id == r.RoleId) && r.UserId == x.Id) : 1 == 1)
                .OrderByDescending(paging.OrderBy).Skip(paging.Skip).Take(paging.PageSize).Select(user => new UserViewModel()
                {
                    //do your variable mapping here 
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    TotalCount = paging.TotalCount,
                    UserRoles = _userManager.GetRolesAsync(user).Result.ToList(),
                    CompanyId = user.CompanyId,
                    //TODO: handle companyId null
                    Company = new CompanyViewModel()
                    {
                        Id = user.Company.Id,
                        Name = user.Company.Name,
                    },
                    //LineOfBusinessId = user.LineOfBusinessId,
                    //TODO: handle line of business null
                    LineOfBusinesses = user.LineOfBusinesses.Select(x => new LineOfBusinessViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList(),


                    //   Contact = user.PhoneNumber
                }).ToList();

                // if (userSearchViewModel.roles != null && userSearchViewModel.roles.Count > 0)
                // {
                //     var roles = applicationDbContext.Roles.Where(x => userSearchViewModel.roles.Contains(x.Name));
                //     //  users = users.Where(x => x.UserRoles.Any(p => userSearchViewModel.roles.Any(p2 => p2 == p))).ToList();
                //     users = users.Where(x => applicationDbContext.UserRoles.Any(r => roles.Any(y => y.Id == r.RoleId) && r.UserId == x.Id)).ToList();
                // }

                // if (userSearchViewModel.companiesIds != null && userSearchViewModel.companiesIds.Count > 0)
                // {
                //     users = users.Where(x => userSearchViewModel.companiesIds.Contains(x.CompanyId.Value)).ToList();
                // }

                // if (userSearchViewModel.lineOfBusinessesIds != null && userSearchViewModel.lineOfBusinessesIds.Count > 0)
                // {
                //     users = users.Where(x => x.LineOfBusinesses.Any(p => userSearchViewModel.lineOfBusinessesIds.Any(p2 => p2 == p.Id))).ToList();
                // }
                var userRoles = applicationDbContext.UserRoles.Where(x => users.Select(y => y.Id).Contains(x.UserId));
                foreach (var user in users)
                {
                    user.UserRoles = userRoles.Where(x => x.UserId == user.Id).Select(
                        x => applicationDbContext.Roles.First(y => y.Id == x.RoleId).Name
                    ).ToList();
                }
                return users;
            });
            }







            // return Ok(new
            // {
            //     users
            // });

        }

        // [Authorize(Roles = UserRoles.FDBAdmin)]
        // [HttpGet]
        // [Route("get-relevant-users")]
        // public async Task<ICollection<UserViewModel>> GetRelevantUsers(UserSearchViewModel<ApplicationUser> searchViewModel)
        // {
        //     Paging<ApplicationUser> paging = null;

        //     if (searchViewModel.Page != 0)
        //     {
        //         paging = new Paging<ApplicationUser>
        //         {
        //             Skip = (searchViewModel.Page - 1) * searchViewModel.PageSize,
        //             OrderBy = searchViewModel.orderBy,
        //             PageSize = searchViewModel.PageSize
        //         };
        //     }

        //     paging.TotalCount = await Task.Run(() =>
        //     { return _userManager.Users.Count(); });

        //     var user = await _userManager.GetUserAsync(HttpContext.User);

        //     //ToDo: get companies ids list 
        //     var companies = await companyService.GetCompanies(new SearchViewModel<Models.Company>()
        //     {
        //         predicate = x => x.OwnerId == user.Id
        //     });
        //     var companiesIds = companies.Select(x => x.Id);


        //     return await Task.Run(() =>
        //     {
        //         //Where(x => companiesIds.Contains(x.CompanyId))
        //         return _userManager.Users.Where(searchViewModel.predicate).OrderByDescending(paging.OrderBy).Skip(paging.Skip).Take(paging.PageSize).Select(user => new UserViewModel()
        //         {
        //             //do your variable mapping here 
        //             Id = user.Id,
        //             UserName = user.UserName,
        //             Email = user.Email,
        //             TotalCount = paging.TotalCount
        //             //  Contact = user.PhoneNumber
        //         }).ToList();
        //     });

        // }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }
    }
}