using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstOne.Models;
using Microsoft.AspNetCore.Authorization;
using PaginList;

namespace FirstOne.Controllers
{
    
    public class ClientsController : Controller
    {
        private readonly ClientContext _context;

        public ClientsController(ClientContext context)
        {
            _context = context;
        }

        //public ActionResult GetPaginProgs()
        //{
        //    ViewBag.Pages = PaginList.PaginatedList();
        //    ViewBag.Progs = GetAllPrograms();
        //    return View();
        //}

        // GET: Clients
        public async Task<IActionResult> Index(
            string sort,
            string currentFilter,
            string search,
            int? page)
        {
            ViewData["CurrentSort"] = sort;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sort) ? "name_sort" : "name_sort";
            ViewData["SurnameSortParm"] = String.IsNullOrEmpty(sort) ? "surname_sort" : "surname_sort";
            ViewData["CitySortParm"] = String.IsNullOrEmpty(sort) ? "city_sort" : "city_sort";
            ViewData["ProgSortParm"] = String.IsNullOrEmpty(sort) ? "prog_sort" : "prog_sort";

            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = currentFilter;
            }

            ViewData["CurrentFilter"] = search;

            var clients = from s in _context.Client
                          select s;
            if (!String.IsNullOrEmpty(search))
            {
                clients = clients.Where(s => s.Name.Contains(search)
                || s.Surname.Contains(search)
                || s.City.Contains(search));
            }

            switch (sort)
            {
                case "name_sort":
                    clients = clients.OrderBy(s => s.Name);
                    break;
                case "surname_sort":
                    clients = clients.OrderBy(s => s.Surname);
                    break;
                case "city_sort":
                    clients = clients.OrderBy(s => s.City);
                    break;
                case "prog_sort":
                    clients = clients.OrderBy(s => s.Program);
                    break;
                default:
                    clients = clients.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Client>.CreateAsync(clients.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: Clients/Details/5
        //TODO: wyświetlanie nazwy klienta zamiast ID
        [Route("Clients/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .SingleOrDefaultAsync(m => m.ClientID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }
        
        // GET: Clients/Create
        [Authorize]
        public IActionResult Create()
        {
            var programs = GetAllPrograms();

            var model = new Client();

            model.Programs = GetSelectListItems(programs);

            return View(model);
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientID,Name,Surname,City,Program")] Client client)
        {
            var programs = GetAllPrograms();

            client.Programs = GetSelectListItems(programs);

            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }
        
        // GET: Clients/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.SingleOrDefaultAsync(m => m.ClientID == id);
            if (client == null)
            {
                return NotFound();
            }

            var programs = GetAllPrograms();
            client.Programs = GetSelectListItems(programs);

            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientID,Name,Surname,City,Program")] Client client)
        {
            if (id != client.ClientID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .SingleOrDefaultAsync(m => m.ClientID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Client.SingleOrDefaultAsync(m => m.ClientID == id);
            _context.Client.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.ClientID == id);
        }

        private IEnumerable<string> GetAllPrograms()
        {
            return new List<string>
            {
                "ShadowProtect SPX Desktop for Windows",
                "ShadowProtect SPX Server for Windows",
                "ESET NOD32 Antivirus",
                "ESET Security Pack - 3 komputery i 3 smartfony",
            };
        }

        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<string> elements)
        {
            var selectList = new List<SelectListItem>();


            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element,
                    Text = element
                });
            }

            return selectList;
        }
    }
}
