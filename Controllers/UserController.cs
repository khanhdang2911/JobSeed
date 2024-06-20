using Microsoft.AspNetCore.Mvc;
using JobSeed.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.EntityFrameworkCore;
namespace JobSeed.Controllers;

public class UserController : Controller
{

    public static string EmailEnter { set; get; }
    public static int _randomCode { set; get; }
    private readonly ILogger<UserController> _logger;
    private readonly HashPasswordByBC _hashPasswordByBC;
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;
    public UserController(ILogger<UserController> logger, AppDbContext context, HashPasswordByBC hashPasswordByBC, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
        _hashPasswordByBC = hashPasswordByBC;
    }

    public IActionResult Index()
    {
        return RedirectToAction("NotFound", "Home");
    }
    public IActionResult Forbidden()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Register([Bind("Email,Password,Name,Phone")] Users users, string ConfirmPassword)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        var checkUserExists1 = _context.users.Where(u => u.Email == users.Email).FirstOrDefault();
        if (checkUserExists1 != null)
        {
            ModelState.AddModelError("", "Email already exists");

        }
        var checkUserExists2 = _context.users.Where(u => u.Phone == users.Phone).FirstOrDefault();
        if (checkUserExists2 != null)
        {
            ModelState.AddModelError("", "Phone already exists");

        }
        if (string.IsNullOrEmpty(ConfirmPassword) == true)
        {
            ModelState.AddModelError("", "You have not re-entered your password");

        }
        if (users.Password != ConfirmPassword)
        {
            ModelState.AddModelError("", "Re-enter incorrect password");
        }
        if (ModelState.ErrorCount >= 1)
        {
            return View();
        }
        users.Password = _hashPasswordByBC.HashPassword(users.Password);
        _context.users.Add(users);
        _context.SaveChanges();
        return RedirectToAction("Login", "User");
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(string Email, string Password)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        var kq = _context.users.Where(u => u.Email == Email).FirstOrDefault();
        if (kq == null)
        {
            ModelState.AddModelError("", "Wrong account name or password");
            return View();
        }
        if (kq != null)
        {
            if (_hashPasswordByBC.VerifyPassword(Password, kq.Password) == false)
            {
                ModelState.AddModelError("", "Password is not correct");
                return View();
            }
        }

        List<Claim> claims = new List<Claim>();

        var CheckRoleNameForUser = (from u in _context.users join r in _context.usersRoles on kq.Id equals r.UsersId select r).ToList();
        if (CheckRoleNameForUser.Count == 0)
        {
            // claims.Add(new Claim(ClaimTypes.Role,"Guest"));
            // Role role=new Role();
            // var checkRoleExists=_context.roles.Where(r=>r.RoleName=="Guest").FirstOrDefault();
            // if(checkRoleExists==null)
            // {
            //     role.RoleName="Guest";
            //     _context.roles.Add(role);
            //     _context.SaveChanges();
            // }
            // UsersRole roleUsers=new UsersRole();
            // roleUsers.UsersId=kq.Id;
            // roleUsers.RoleId=_context.roles.Where(r=>r.RoleName=="Student").Select(r=>r.Id).FirstOrDefault();

            // _context.usersRoles.Add(roleUsers);
            // _context.SaveChanges();

        }
        else
        {
            foreach (var item in CheckRoleNameForUser)
            {
                var rolename = _context.roles.Where(r => r.Id == item.RoleId).Select(r => r.RoleName).FirstOrDefault();
                claims.Add(new Claim(ClaimTypes.Role, rolename));
            }
        }
        claims.Add(new Claim("Id", kq.Id + ""));
        var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimPrincipal = new ClaimsPrincipal(claimIdentity);
        await HttpContext.SignInAsync(claimPrincipal);
        return RedirectToAction("Index", "Home");
    }
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }
    [HttpPost]
    public IActionResult ForgotPasswordConfirmCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            ModelState.AddModelError("", "Must enter code to confirm");
            return View();
        }
        else if (code != _randomCode.ToString())
        {
            Console.WriteLine(code + "  " + _randomCode);
            ModelState.AddModelError("", "The confirmation code you entered is incorrect, please check again");
            return View();
        }
        var user = _context.users.Where(u => u.Email == EmailEnter).FirstOrDefault();
        return RedirectToAction("EnterNewPassword", new { UsersId = user.Id });
    }
    [HttpGet]
    public IActionResult EnterNewPassword(int UsersId)
    {
        Users kq = _context.users.Where(u => u.Id == UsersId).First();
        if (kq == null)
        {
            return RedirectToAction("NotFound", "Home");
        }
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> EnterNewPassword([FromForm] string newpassword, [FromForm] string newpasswordConfirm, [FromQuery] int UsersId)
    {
        Users kq = (from u in _context.users where u.Id == UsersId select u).FirstOrDefault();
        if (string.IsNullOrEmpty(newpassword))
        {
            ModelState.AddModelError("", "You have not entered a new password");
            return View(kq);
        }

        if (string.IsNullOrEmpty(newpasswordConfirm))
        {
            ModelState.AddModelError("", "You have not reconfirmed your new password");
            return View(kq);
        }
        if (newpassword != newpasswordConfirm)
        {
            ModelState.AddModelError("", "Re-enter the password incorrectly");
            return View(kq);
        }

        _context.Entry(kq).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        kq.Password = _hashPasswordByBC.HashPassword(newpassword);
        await _context.SaveChangesAsync();
        return RedirectToAction("Login");
    }
    [HttpPost]
    public IActionResult ForgotPassword(string? Email)
    {
        if (string.IsNullOrEmpty(Email))
        {
            ModelState.AddModelError("", "Enter your Email");
            return View();
        }
        var user = _context.users.Where(u => u.Email == Email).FirstOrDefault();
        if (user == null)
        {
            ModelState.AddModelError("", "Email does not exist");
            return View();
        }
        EmailEnter = Email;
        Random rnd = new Random();
        int randomCode = rnd.Next(100000, 999999);
        _randomCode = randomCode;
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress("khanhdang", "khanhdang3152@gmail.com"));
        email.To.Add(new MailboxAddress("", $"{Email}"));

        email.Subject = "CONFIRM USER EMAIL";
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $"<b>Your confirmation code is: {_randomCode}</b>"
        };
        Console.WriteLine("Ma code:" + _randomCode);

        using (var smtp = new SmtpClient())
        {
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);


            // Note: only needed if the SMTP server requires authentication
            smtp.Authenticate("khanhdang3152@gmail.com", "tlol kfvg mubs yyyl");

            smtp.Send(email);
            smtp.Disconnect(true);
        }



        return View("ForgotPasswordConfirmCode", (object)randomCode);
    }


    [HttpGet]
    [Authorize]
    public IActionResult EditUser(int UsersId)
    {
        if (User.IsInRole("Admin"))
        {

        }
        else if (UsersId.ToString() != User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value)
        {
            return RedirectToAction("NotFound", "Home");
        }


        var kq = _context.users.Where(u => u.Id == UsersId).FirstOrDefault();
        if (kq == null)
        {
            return RedirectToAction("NotFound", "Home");

        }
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> EditUser(int UsersId, [Bind("Email,Password,Name,Phone,FormFile")] Users users)
    {

        var kq = _context.users.Where(u => u.Id == UsersId).FirstOrDefault();
        if (kq == null)
        {
            return RedirectToAction("NotFound", "Home");
        }

        if (!ModelState.IsValid)
        {
            LogModelStateErrors();
            return View(kq);
        }
        if (users.FormFile != null)
        {
            Console.WriteLine("Da vao day luon roi");
            var filepath = Path.Combine(_environment.WebRootPath, "uploads", users.FormFile.FileName);
            if (!System.IO.File.Exists(filepath))
            {
                using FileStream fileStream = new FileStream(filepath, FileMode.Create);
                users.FormFile.CopyTo(fileStream);
            }
            users.ImageLink = $"uploads/{users.FormFile.FileName}";
        }
        var checkUserExists1 = _context.users.Where(u => u.Email == users.Email && u.Id != UsersId).FirstOrDefault();
        if (checkUserExists1 != null)
        {
            ModelState.AddModelError("", "Email already exists");
            return View(kq);
        }
        var checkUserExists2 = _context.users.Where(u => u.Phone == users.Phone && u.Id != UsersId).FirstOrDefault();
        if (checkUserExists2 != null)
        {
            ModelState.AddModelError("", "Phone already exists");
            return View(kq);
        }

        _context.Entry(kq).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        kq.Email = users.Email;
        kq.Password = users.Password;
        kq.Name = users.Name;
        kq.Phone = users.Phone;
        if (users.ImageLink != null)
        {
            kq.ImageLink = users.ImageLink;
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }
    [Authorize]
    public IActionResult ChangePassword(int id)
    {
        var kq = _context.users.Where(u => u.Id == id).FirstOrDefault();
        if (kq == null)
        {
            return RedirectToAction("NotFound", "Home");
        }
        return View(kq);
    }
    [HttpPost]
    public IActionResult ChangePassword(int id, string oldPassword, string newpassword, string newpasswordConfirm)
    {
        var kq = _context.users.Where(u => u.Id == id).FirstOrDefault();
        if (kq == null)
        {
            return RedirectToAction("NotFound", "Home");
        }
        if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newpassword) || string.IsNullOrWhiteSpace(newpasswordConfirm))
        {
            ModelState.AddModelError("", "You did not enter enough information, please re-enter");
            return View(kq);
        }
        if (_hashPasswordByBC.VerifyPassword(oldPassword, kq.Password) == false)
        {
            ModelState.AddModelError("", "The old password is incorrect");

        }

        if (newpassword != newpasswordConfirm)
        {
            ModelState.AddModelError("", "Re-enter incorrect password");

        }
        if (_hashPasswordByBC.VerifyPassword(newpassword, kq.Password))
        {
            ModelState.AddModelError("", "The new password must not be the same as the old password");

        }
        if (ModelState.ErrorCount > 0)
        {
            return View(kq);
        }
        _context.Entry(kq).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        kq.Password = _hashPasswordByBC.HashPassword(newpassword);
        _context.SaveChanges();
        return RedirectToAction("EditUser", new { UsersId = id });
    }

    private void LogModelStateErrors()
    {
        var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { x.Key, x.Value.Errors })
            .ToArray();

        foreach (var error in errors)
        {
            foreach (var subError in error.Errors)
            {
                _logger.LogError("Field: {Field}, Error: {Error}", error.Key, subError.ErrorMessage);
            }
        }
    }
    [Authorize]
    public async Task<IActionResult> HistoryUploadJob(int usersId)
    {
        var user = await _context.users.Where(c => c.Id == usersId).FirstOrDefaultAsync();
        if (user == null)
        {
            return RedirectToAction("NotFound", "Home");
        }

        return View(user);
    }


}
