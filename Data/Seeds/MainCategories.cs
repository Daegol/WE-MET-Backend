using Data.Repos;
using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Seeds
{
    public static class MainCategories
    {
        public static void SeedAsync(IGenericRepository<MainCategory> repository)
        {
            if (repository.GetAll().Count == 0)
            {
                var mainCategories = new List<MainCategory>();
                mainCategories.Add(new MainCategory() { Name = "Złom stalowy" });
                mainCategories.Add(new MainCategory() { Name = "Aluminium" });
                mainCategories.Add(new MainCategory() { Name = "Stal Nierdzewna" });
                mainCategories.Add(new MainCategory() { Name = "Miedź" });
                mainCategories.Add(new MainCategory() { Name = "Mosiądz" });
                mainCategories.Add(new MainCategory() { Name = "Cynk/Ołów" });
                mainCategories.Add(new MainCategory() { Name = "Inne" });

                repository.BulkInsert(mainCategories);
            }
        }
    }
}
