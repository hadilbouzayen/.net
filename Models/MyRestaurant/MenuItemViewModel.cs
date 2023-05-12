using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAppTest3.Models.MyRestaurant
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }

        public decimal Price { get; set; }
        public string Image { get; set; }
        public bool IsSelected { get; set; }
    }






}
