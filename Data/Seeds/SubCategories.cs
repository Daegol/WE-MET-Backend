using Data.Repos;
using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Seeds
{
    public static class SubCategories
    {
        public static void SeedAsync(IGenericRepository<SubCategory> repository)
        {
            if (repository.GetAll().Count == 0)
            {
                var subCategories = new List<SubCategory>();

                subCategories.Add(new SubCategory() { Name = "W1", MainCategoryId = 1 });
                subCategories.Add(new SubCategory() { Name = "N10", MainCategoryId = 1 });
                subCategories.Add(new SubCategory() { Name = "Żeliwo", MainCategoryId = 1 });
                subCategories.Add(new SubCategory() { Name = "Ocynk", MainCategoryId = 1 });
                subCategories.Add(new SubCategory() { Name = "Mix", MainCategoryId = 1 });
                subCategories.Add(new SubCategory() { Name = "Poprodukcja", MainCategoryId = 1 });

                subCategories.Add(new SubCategory() { Name = "AL. Plastyka", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Felga", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Odlew", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Sektor", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Linka", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Offset", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Profil kolor", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Profil Czysty", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Zanieczyszczone", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Izoprofil", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Folia", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Chłodnice", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Gary", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL ALMG", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Wióry", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Żaluzje", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Rolety", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "AL Kable", MainCategoryId = 2 });
                subCategories.Add(new SubCategory() { Name = "Puszki", MainCategoryId = 2 });

                subCategories.Add(new SubCategory() { Name = "KO", MainCategoryId = 3 });
                subCategories.Add(new SubCategory() { Name = "KO wióry", MainCategoryId = 3 });
                subCategories.Add(new SubCategory() { Name = "KO 316", MainCategoryId = 3 });
                subCategories.Add(new SubCategory() { Name = "KO 316 wióry", MainCategoryId = 3 });
                subCategories.Add(new SubCategory() { Name = "KO gary", MainCategoryId = 3 });
                subCategories.Add(new SubCategory() { Name = "Chrom", MainCategoryId = 3 });

                subCategories.Add(new SubCategory() { Name = "CU", MainCategoryId = 4 });
                subCategories.Add(new SubCategory() { Name = "CU Millberry", MainCategoryId = 4 });
                subCategories.Add(new SubCategory() { Name = "CU kawałek", MainCategoryId = 4 });
                subCategories.Add(new SubCategory() { Name = "CU kawałek świecący", MainCategoryId = 4 });
                subCategories.Add(new SubCategory() { Name = "CU piecyki", MainCategoryId = 4 });
                subCategories.Add(new SubCategory() { Name = "CU cewka", MainCategoryId = 4 });
                subCategories.Add(new SubCategory() { Name = "CU granulat", MainCategoryId = 4 });
                subCategories.Add(new SubCategory() { Name = "CU pobiał", MainCategoryId = 4 });
                subCategories.Add(new SubCategory() { Name = "CU wióry", MainCategoryId = 4 });
                subCategories.Add(new SubCategory() { Name = "CU Kable Mix", MainCategoryId = 4 });
                subCategories.Add(new SubCategory() { Name = "CU wiązka samochodowa czysta", MainCategoryId = 4 });
                subCategories.Add(new SubCategory() { Name = "CU wiązka brudna", MainCategoryId = 4 });

                subCategories.Add(new SubCategory() { Name = "MS", MainCategoryId = 5 });
                subCategories.Add(new SubCategory() { Name = "MS żółty", MainCategoryId = 5 });
                subCategories.Add(new SubCategory() { Name = "MS łuski", MainCategoryId = 5 });
                subCategories.Add(new SubCategory() { Name = "MS chłodnice", MainCategoryId = 5 });
                subCategories.Add(new SubCategory() { Name = "MS wióry", MainCategoryId = 5 });
                subCategories.Add(new SubCategory() { Name = "MS poprodukcja", MainCategoryId = 5 });
                subCategories.Add(new SubCategory() { Name = "BR magnetyczny", MainCategoryId = 5 });
                subCategories.Add(new SubCategory() { Name = "BR niemagnetyczny", MainCategoryId = 5 });

                subCategories.Add(new SubCategory() { Name = "ZN blacha", MainCategoryId = 6 });
                subCategories.Add(new SubCategory() { Name = "ZN twardy", MainCategoryId = 6 });
                subCategories.Add(new SubCategory() { Name = "Anody ZN z FE", MainCategoryId = 6 });
                subCategories.Add(new SubCategory() { Name = "Znal", MainCategoryId = 6 });
                subCategories.Add(new SubCategory() { Name = "PB czysty", MainCategoryId = 6 });
                subCategories.Add(new SubCategory() { Name = "PB brudny", MainCategoryId = 6 });
                subCategories.Add(new SubCategory() { Name = "PB odważniki do kół", MainCategoryId = 6 });
                subCategories.Add(new SubCategory() { Name = "AK Akumulatory", MainCategoryId = 6 });

                subCategories.Add(new SubCategory() { Name = "SN 99,9", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "SN 90", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "SN 80", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "SN 70", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "SN 60", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "SN 50", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "SN 40", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "SN 30", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "SN 20", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "Chłodnice Al/Cu", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "W1 silniki Elektryczne", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "W1 alternatory", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "W1 rozruszniki", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "W1 wirniki/Stojany", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "W1 silniki spalinowe", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "W1 agregaty", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "Tytan", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "Wolfram", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "Nikiel", MainCategoryId = 7 });
                subCategories.Add(new SubCategory() { Name = "Baterie", MainCategoryId = 7 });

                repository.BulkInsert(subCategories);
            }
        }
    }
}
