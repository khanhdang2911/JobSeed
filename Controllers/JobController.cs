using JobSeed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobSeed.Controllers
{
    public class JobController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;
        private readonly ILogger<JobController> _logger;

        public JobController(AppDbContext context, IWebHostEnvironment environment, ILogger<JobController> logger)
        {
            _environment = environment;
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var allJob = (from p in _context.jobs where p.State==true select p).Include(p => p.JobType).ToList();
            return View(allJob);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([Bind("JobName,JobDescription,CompanyName,Location,FromSalary,ToSalary,JobTypeId,FormFile,CategoryId,Gender,Experiences,Responsibility,Qualifications,Benefits")] Job job)
        {
            if (!ModelState.IsValid)
            {
                LogModelStateErrors();
                return View();
            }
            if (job.FormFile != null)
            {
                var filepath = Path.Combine(_environment.WebRootPath, "uploads", job.FormFile.FileName);
                if (!System.IO.File.Exists(filepath))
                {
                    using FileStream fileStream = new FileStream(filepath, FileMode.Create);
                    job.FormFile.CopyTo(fileStream);
                }
                job.ImageLink = $"uploads/{job.FormFile.FileName}";
            }
            int usersId=int.Parse(User.Claims.FirstOrDefault(c=>c.Type=="Id").Value);
            job.EmployerId=usersId;
            if(User.IsInRole("Admin"))
            {
                job.State=true;
            }
            await _context.jobs.AddAsync(job);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            var kq = _context.jobs.Where(c => c.Id == id).FirstOrDefault();
            if (kq == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(kq);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, [Bind("JobName,JobDescription,CompanyName,Location,FromSalary,ToSalary,Status,JobTypeId,FormFile,CategoryId,Gender,Responsibility,Experiences,Qualifications,Benefits")] Job job)
        {
            if (!ModelState.IsValid)
            {
                LogModelStateErrors();
                return View();
            }
            var kq = _context.jobs.Where(c => c.Id == id).FirstOrDefault();
            if (kq == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            if (job.FormFile != null)
            {
                var filepath = Path.Combine(_environment.WebRootPath, "uploads", job.FormFile.FileName);
                if (!System.IO.File.Exists(filepath))
                {
                    using FileStream fileStream = new FileStream(filepath, FileMode.Create);
                    job.FormFile.CopyTo(fileStream);
                }
                job.ImageLink = $"uploads/{job.FormFile.FileName}";
            }

            kq.JobName = job.JobName;
            kq.JobDescription = job.JobDescription;
            kq.CompanyName = job.CompanyName;
            kq.Location = job.Location;
            kq.ImageLink = job.ImageLink;
            kq.FromSalary = job.FromSalary;
            kq.ToSalary = job.ToSalary;
            kq.JobTypeId = job.JobTypeId;
            kq.Gender = job.Gender;
            kq.Experiences = job.Experiences;
            kq.Qualifications = job.Qualifications;
            kq.Benefits = job.Benefits;
            kq.CategoryId = job.CategoryId;
            kq.Responsibility = job.Responsibility;
            _context.Entry(kq).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            var kq = _context.jobs.Where(c => c.Id == id).FirstOrDefault();
            if (kq == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            _context.jobs.Remove(kq);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Detail(int id)
        {
            var kq = _context.jobs.Where(p => p.Id == id).Include(c => c.JobType).FirstOrDefault();
            if (kq == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            //check xem da active hay chua
            if(User.IsInRole("Admin"))
            {

            }
            else if(kq.State==false)
            {
                return RedirectToAction("NotFound","Home");
            }
            return View(kq);
        }

        [AllowAnonymous]
        public IActionResult Search(string jobName)
        {
            var kq = _context.jobs.Where(p => p.JobName.Contains(jobName)).ToList();
            if (kq == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View("Index", kq);
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
        public IActionResult AllJobFilter(int page = 1)
        {
            int jobPerpages = 4;
            int totalJobs = _context.jobs.ToList().Count;
            int totalPage=(int)Math.Ceiling((double)totalJobs/jobPerpages); 

            var jobFilter = _context.jobs.Where(c=>c.State==true).Include(c => c.JobType).AsQueryable();
            if (page != 0)
            {
                jobFilter = jobFilter.Skip((page - 1) * jobPerpages).Take(jobPerpages);
            }
            ViewData["totalPage"] = totalPage;
            ViewData["currentPage"] = page;
            return View(jobFilter.ToList());
        }
        public IActionResult FilterJob(JobFilter jobFilter ,int? category,int page = 1)
        {
            var allJobFilter = _context.jobs.Where(c=>c.State==true).Include(c => c.JobType).AsQueryable();
            if (jobFilter.SearchKeyword != null)
            {
                allJobFilter = allJobFilter.Where(c => c.JobName.Contains(jobFilter.SearchKeyword)
                    || c.JobDescription.Contains(jobFilter.SearchKeyword)
                    || c.JobType.JobTitle.Contains(jobFilter.SearchKeyword)
                    || c.Category.CategoryName.Contains(jobFilter.SearchKeyword));
            }
            if (jobFilter.Location != null)
            {
                allJobFilter = allJobFilter.Where(c => c.Location.CompareTo(jobFilter.Location) == 0);
            }
            if (jobFilter.Category != null)
            {
                allJobFilter = allJobFilter.Where(c => c.CategoryId == jobFilter.Category);
            }
            if (jobFilter.JobType != null)
            {
                allJobFilter = allJobFilter.Where(c => c.JobTypeId == jobFilter.JobType);
            }
            if (jobFilter.Gender != null)
            {
                allJobFilter = allJobFilter.Where(c => c.Gender == jobFilter.Gender);
            }
            //check category(Truong hop dac biet)
            if(category!=null)
            {
                allJobFilter=_context.jobs.Where(c => c.CategoryId==category);
            }
            // Pagination
            int jobPerPages = 4;
            int totalJobs = allJobFilter.Count();
            int totalPage=(int)Math.Ceiling((double)totalJobs/jobPerPages); 

            if (page != 0)
            {
                allJobFilter = allJobFilter.Skip((page - 1) * jobPerPages).Take(jobPerPages);
            }

            ViewData["SearchKeyword"] = jobFilter.SearchKeyword;
            ViewData["Location"] = jobFilter.Location;
            ViewData["Category"] = jobFilter.Category;
            ViewData["JobType"] = jobFilter.JobType;
            ViewData["Gender"] = jobFilter.Gender;
            ViewData["totalPage"] = totalPage;
            ViewData["currentPage"] = page;
            return View("AllJobFilter", allJobFilter.ToList());
        }
        [Authorize(Roles="Admin")]

        public async Task<IActionResult> UpdateState(int id)
        {
            var job=await _context.jobs.Where(c=>c.Id==id).FirstOrDefaultAsync();
            if(job==null)
            {
                return RedirectToAction("NotFound","Home");
            }
            job.State=true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> JobNotActive()
        {
            var jobs=await _context.jobs.Where(c=>c.State==false).Include(c=>c.JobType).ToListAsync();
            return View("Index",jobs);
            
        }
        
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> BlockJob(int id)
        {
            var job=await _context.jobs.Where(c=>c.Id==id).FirstOrDefaultAsync();
            if(job==null)
            {
                return RedirectToAction("NotFound","Home");
            }
            job.State=false;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ApplyJob(UsersJob usersJob)
        {
            if (usersJob.FormFile != null)
            {
                Console.WriteLine("Co vao day khong cu");
                var filepath = Path.Combine(_environment.WebRootPath, "uploads", usersJob.FormFile.FileName);
                if (!System.IO.File.Exists(filepath))
                {
                    using FileStream fileStream = new FileStream(filepath, FileMode.Create);
                    usersJob.FormFile.CopyTo(fileStream);
                }
                usersJob.CV = $"uploads/{usersJob.FormFile.FileName}";
            }
            else{
                Console.WriteLine("Vao day khong huhu");
            }

            await _context.usersJobs.AddAsync(usersJob);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> DetailJobApply(int jobid, int usersid)
        {
            var userJob = await _context.usersJobs.Where(c => c.JobId == jobid && c.UsersId == usersid).FirstOrDefaultAsync();
            if(userJob==null)
            {
                return RedirectToAction("NotFound","Home");
            }
            ViewData["user"]=_context.users.Find(usersid);
            return View(userJob);
        }
        [Authorize]
        public async Task<IActionResult> UpdateStateJob(int jobid, int usersid,bool? state)
        {
            var userJob = await _context.usersJobs.Where(c => c.JobId == jobid && c.UsersId == usersid).FirstOrDefaultAsync();
            if(userJob==null)
            {
                return RedirectToAction("NotFound","Home");
            }
            userJob.State=state;
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewApplyJob","User",new{jobid=jobid,usersid=usersid});
        }
        
        

    }
}
