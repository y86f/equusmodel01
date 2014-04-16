namespace EquusModel.Migrations
{
    using EquusModel.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EquusModel.Models.ModelContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EquusModel.Models.ModelContext context)
        {
            //  This method will be called after migrating to the latest version.

            //if (System.Diagnostics.Debugger.IsAttached == false) { }
            //    System.Diagnostics.Debugger.Launch(); 

            // clears tables before populating
            context.Database.ExecuteSqlCommand("DELETE FROM FoodCharacteristics");
            context.Database.ExecuteSqlCommand("DELETE FROM NutrientCharacteristics");
            context.Database.ExecuteSqlCommand("DELETE FROM N_F_Relation");
            context.Database.ExecuteSqlCommand("DELETE FROM Foods");
            context.Database.ExecuteSqlCommand("DELETE FROM Nutrients");

            context.SaveChanges();
            //  This method will be called after migrating to the latest version.
            Random r = new Random();
            int nFoods = 100;
            int nNutrients = 10;
            bool temp = true;

            Food[] food = new Food[nFoods];
            Nutrient[] nutr = new Nutrient[nNutrients];
            FoodCharacteristic[] foodChar = new FoodCharacteristic[nFoods];
            NutrientCharacteristic[] nutrChar = new NutrientCharacteristic[nNutrients];
            N_F_Relation[] nfRel = new N_F_Relation[nFoods * nNutrients];

            for (int i = 0; i < nFoods; i++)
            {
                food[i] = new Food() { ID = i, Description = String.Format("Food {0}", i) };
                foodChar[i] = new FoodCharacteristic
                {
                    FoodID = i,
                    Price = r.Next(10, 20),
                    MinServings = r.Next(0, 3),
                    MaxServings = r.Next(3, 5)
                };
                for (int j = 0; j < nNutrients; j++)
                {
                    if (temp)
                    {
                        nutr[j] =
                            new Nutrient()
                            {
                                ID = j,
                                Description = String.Format("Nutrient {0}", j)
                            };
                        nutrChar[j] =
                            new NutrientCharacteristic
                            {
                                NutrientID = j,
                                MinNutrient = r.Next(0, 20),
                                MaxNutrient = r.Next(20, 30)
                            };
                    }
                    nfRel[i * nNutrients + j] =
                        new N_F_Relation
                        {
                            FoodID = i,
                            NutrientID = j,
                            NutrientPerFood = r.Next(0, 4)
                        };
                }
                temp = false;
            }
            context.Foods.AddRange(food);
            context.FoodCharacteristics.AddRange(foodChar);
            context.Nutrients.AddRange(nutr);
            context.NutrientCharacteristics.AddRange(nutrChar);
            context.N_F_Relations.AddRange(nfRel);

            context.SaveChanges();
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
