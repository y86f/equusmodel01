using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using MySql.Data.Entity;

namespace EquusModel.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class ModelContext : DbContext
    {
        public DbSet<Food> Foods { get; set; }
        public DbSet<FoodCharacteristic> FoodCharacteristics { get; set; }
        public DbSet<V_FoodQuantity> V_FoodQuantity { get; set; }
        public DbSet<Nutrient> Nutrients { get; set; }
        public DbSet<NutrientCharacteristic> NutrientCharacteristics { get; set; }
        public DbSet<N_F_Relation> N_F_Relations { get; set; }

        public ModelContext()
            : base()
        {
        }
        public ModelContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Car>().MapToStoredProcedures();
        }
        public void FillContext()
        {
            Seed(this);
        }
        public static void Seed(ModelContext context, bool csvUse = false)
        {
            //  This method will be called after migrating to the latest version.
            StringBuilder[] csv = new StringBuilder[6];
            if (csvUse)
            {
                for (int i = 0; i < 6; i++)
                    csv[i] = new StringBuilder();
            }
            else { 
                // clears tables before populating
                context.Database.ExecuteSqlCommand("DELETE FROM FoodCharacteristics");
                context.Database.ExecuteSqlCommand("DELETE FROM NutrientCharacteristics");
                context.Database.ExecuteSqlCommand("DELETE FROM N_F_Relation");
                context.Database.ExecuteSqlCommand("DELETE FROM Foods");
                context.Database.ExecuteSqlCommand("DELETE FROM Nutrients");
                context.Database.ExecuteSqlCommand("DELETE FROM V_FoodQuantity");

                context.SaveChanges();
            }
            //  This method will be called after migrating to the latest version.
            //if (System.Diagnostics.Debugger.IsAttached == false) { }
            //    System.Diagnostics.Debugger.Launch(); 

            Random r = new Random();
            int nNutrients = 500;
            int sparcity = 4;
            int nFoods = nNutrients * sparcity;

            bool temp = true;

            Food[] food = new Food[nFoods];
            Nutrient[] nutr = new Nutrient[nNutrients];
            FoodCharacteristic[] foodChar = new FoodCharacteristic[nFoods];
            V_FoodQuantity[] vFoodQty = new V_FoodQuantity[nFoods];
            NutrientCharacteristic[] nutrChar = new NutrientCharacteristic[nNutrients];
            N_F_Relation nfRel;

            for (int i = 0; i < nFoods; i++)
            {
                food[i] = new Food() { ID = i, Description = String.Format("Food {0}", i) };
                if (csvUse)
                {
                    csv[0].Append(String.Format("{0},{1}{2}", 
                        food[i].ID.ToString(), 
                        food[i].Description, 
                        Environment.NewLine));
                }

                foodChar[i] = new FoodCharacteristic
                {
                    FoodID = i,
                    Price = r.Next(10, 20),
                    MinServings = r.Next(0, 3),
                    MaxServings = r.Next(3, 5)
                };
                if (csvUse)
                {
                    csv[2].Append(String.Format("{0},{1},{2},{3},{4}", 
                        foodChar[i].FoodID.ToString(), 
                        foodChar[i].MaxServings.ToString(), 
                        foodChar[i].Price.ToString(),
                        foodChar[i].MinServings.ToString(), 
                        Environment.NewLine));
                }

                vFoodQty[i] = new V_FoodQuantity
                {
                    FoodID = i,
                    Quantity = 0
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
                        if (csvUse)
                        {
                            csv[1].Append(String.Format("{0},{1}{2}", 
                                nutr[j].ID.ToString(), 
                                nutr[j].Description, 
                                Environment.NewLine));
                        }
                        nutrChar[j] =
                            new NutrientCharacteristic
                            {
                                NutrientID = j,
                                MinNutrient = r.Next(0, 20),
                                MaxNutrient = r.Next(20, 30)
                            };
                        if (csvUse)
                        {
                            csv[4].Append(String.Format("{0},{1},{2}{3}", 
                                nutrChar[j].NutrientID.ToString(), 
                                nutrChar[j].MaxNutrient.ToString(), 
                                nutrChar[j].MinNutrient.ToString(), 
                                Environment.NewLine));
                        }
                    }
                    // defines coefficients to half the combinations (upper left corner and bottom right corner of the matrix)
                    if (j * sparcity < (i + 1) && (i + 1) <= sparcity * (j + 1))
                    {
                        nfRel =
                            new N_F_Relation
                            {
                                FoodID = i,
                                NutrientID = j,
                                NutrientPerFood = r.Next(1, 4)
                            };
                        if (csvUse) {
                            csv[5].Append(String.Format("{0},{1},{2}{3}",
                                nfRel.FoodID.ToString(),
                                nfRel.NutrientID.ToString(),
                                nfRel.NutrientPerFood.ToString(),
                                Environment.NewLine));
                        } else {
                            context.N_F_Relations.Add(nfRel);
                        }
                    }
                }
                temp = false;
            }
            if (csvUse) {
                for (int i = 0; i < 6; i++)
                {
                    if (csv[i].Length > 0)
                    {
                        File.WriteAllText(
                            String.Format(@"C:\Users\User\Documents\YF\csv"+"_{0}.csv", i.ToString()), 
                            csv[i].ToString()
                        );
                    }
                }
            } else {
                context.Foods.AddRange(food);
                context.FoodCharacteristics.AddRange(foodChar);
                context.V_FoodQuantity.AddRange(vFoodQty);
                context.Nutrients.AddRange(nutr);
                context.NutrientCharacteristics.AddRange(nutrChar);
                context.SaveChanges();

            }

        }
    }
}
