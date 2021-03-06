using Microsoft.AspNetCore.Mvc;
using cbsStudents.Models.Entities;
using CbsStudents.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace cbsStudents.Controllers;

[Authorize]
public class PostsController : Controller
{
    private CbsStudentsContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    
    public PostsController(CbsStudentsContext context, UserManager<IdentityUser> userManager){
        this._userManager = userManager;
        this._context = context;
    }
    
    
    [AllowAnonymous]
    public IActionResult Index(string SearchString = "")
    {
        if(SearchString == null)
        {
            SearchString = "";
        }
        var posts = from p in _context.Posts select p;
        posts = posts.Where(x => x.Title.Contains(SearchString)).Include(y => y.User);
        posts = posts.OrderBy(x => x.Created);
        
        ViewBag.SearchString = SearchString;
        var vm = new PostIndexVm{Posts = posts.ToList(), SearchString = SearchString};
        
        return View(vm);
    }
    public IActionResult Test()
    {
        var vm = from p in _context.Posts select new MyTestVm{
            Title = p.Title,
            Text = p.Text
        };
        vm = vm.OrderByDescending(p => p.Title.Length);
       
        return View(vm);
    }
    
    
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Title", "Text","Status")] Post post)
    {
    
        if(ModelState.IsValid){
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            post.UserId = user.Id;

            post.Created = DateTime.Now;
            _context.Posts.Add(post);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View();
    }



    public async Task<IActionResult> Edit(int? id)
    {
        if(id == null){
            return NotFound();
        }
        var post = await _context.Posts.Include(x => x.User).Include(x => x.Comments).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.PostId == id);
        
        if (post == null){
            return NotFound();
        }

        return View(post);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int Id, [Bind("PostId,Title,Text,Created,Status")] Post post)
    {
        if(Id != post.PostId)
        {
            return NotFound();
        }
        if(ModelState.IsValid)
        {
            
            post.Created = DateTime.Now;
            _context.Update(post);
            await _context.SaveChangesAsync();
            
        }
        return RedirectToAction("Index");
    }

    

}