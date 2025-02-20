using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1484.Pages
{
    public class DashBoardModel : PageModel
    {
        [BindProperty] public int SelectedProject { get; set; }
        public String SelectMessage { get; set; }

        public List<P


        public void OnPostProjectSelect()
        {
            SelectMessage = "Selection Project was: " + SelectedProject;
        }
    }
}
