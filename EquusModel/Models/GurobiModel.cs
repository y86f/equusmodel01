using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurobi;
using Optimization.Interfaces;
using Optimization.Solver.GLPK;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using MySql.Data.MySqlClient;
using System.Transactions;
 
namespace EquusModel.Models
{
    public class GurobiModel
    {
        // -- key helper variable definition --//
        private static class KEY
        {
            static public Tuple<Food, Nutrient> NK(Food f, Nutrient n)
            {
                return new Tuple<Food, Nutrient>(f, n);
            }
            static public Tuple<Food, Nutrient, Food> NK(Food f, Nutrient n, Food n2)
            {
                return new Tuple<Food, Nutrient, Food>(f, n, n2);
            }
        }
        public static void Solve(Control.SolverProperty sp, Workbook wb)
        {
            // variable declaration on broader scope
            List<Food> SFoods;
            List<Nutrient> SNutrients;
            Dictionary<Food, double> PFoodCost;
            Dictionary<Nutrient, double> PMinNutrient;
            Dictionary<Nutrient, double> PMaxNutrient;
            Dictionary<Tuple<Food, Nutrient>, double?> PNutrientPerFood;
            Dictionary<Food, GRBVar> V_FoodQuantity;
            GRBEnv env = new GRBEnv();
            GRBModel OptModel = new GRBModel(env);

            Stopwatch sw = new Stopwatch();
            
            string connectionString = "server=localhost;port=3300;database=FoodModel;uid=root;password=1234";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                #region Database connection - Data Retrieving
                // Create database if not exists
                using (ModelContext db = new ModelContext(connection, false))
                {
                    //db.FillContext();
                    if (db.Database.CreateIfNotExists())
                        db.FillContext();
                }
                connection.Open();
                using (var tran = new TransactionScope())
                { 
                    //MySqlTransaction transaction = connection.BeginTransaction();
                    try
                    {
                        using (ModelContext db = new ModelContext(connection, false))
                        {
                            //fill context with random numbers
                            sw.Start();

                            #region Set and Model Definition
                            SFoods = db.Foods.ToList();
                            SNutrients = db.Nutrients.ToList();

                            PFoodCost = db.FoodCharacteristics.ToDictionary(fc => SFoods[fc.FoodID], fc => fc.Price);
                            PMinNutrient = db.NutrientCharacteristics
                                .ToDictionary(nc => SNutrients[nc.NutrientID], nc => nc != null ? nc.MinNutrient.Value : 0.0);
                            PMaxNutrient = db.NutrientCharacteristics
                                .ToDictionary(nc => SNutrients[nc.NutrientID], nc => nc != null ? nc.MaxNutrient.Value : double.PositiveInfinity);
                            PNutrientPerFood =
                                db.N_F_Relations.ToDictionary(
                                    nf => KEY.NK(SFoods[nf.FoodID], SNutrients[nf.NutrientID]),
                                    nf => nf.NutrientPerFood);
                        
                            OptModel.Set(GRB.StringAttr.ModelName, "Diet Model");

                            ((Range)((Worksheet)wb.ActiveSheet).Range["B4"]).Value2 = sw.Elapsed.ToString();
                            #endregion
                        }
                    }

                    catch
                    {
                        //transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        tran.Complete();
                    }
                }
                #endregion

                #region Variable Creation
                V_FoodQuantity = new Dictionary<Food, GRBVar>();
                // initialize variables
                foreach (var FOOD in SFoods)
                {
                    V_FoodQuantity[FOOD] =
                        OptModel.AddVar(0, GRB.INFINITY,
                            PFoodCost[FOOD],
                            GRB.CONTINUOUS,
                            FOOD.Description);
                }

                // Update model to integrate new variables
                OptModel.Update();

                ((Range)((Worksheet)wb.ActiveSheet).Range["B5"]).Value2 = sw.Elapsed.ToString();
                #endregion

                #region Goal Definition
                // The objective is to minimize the costs
                OptModel.Set(GRB.IntAttr.ModelSense, 1);
                ((Range)((Worksheet)wb.ActiveSheet).Range["B6"]).Value2 = sw.Elapsed.ToString();
                #endregion

                #region Constraint Definition
                bool pmin, pmax;
                foreach (var NUTR in SNutrients)
                {
                    GRBLinExpr expression = 0.0;
                    foreach (var FOOD in SFoods)
                    {
                        if (PNutrientPerFood.ContainsKey(KEY.NK(FOOD, NUTR)))
                            expression.AddTerm(PNutrientPerFood[KEY.NK(FOOD, NUTR)].Value, V_FoodQuantity[FOOD]);
                    }
                    pmin = PMinNutrient.ContainsKey(NUTR);
                    pmax = PMaxNutrient.ContainsKey(NUTR);
                    if (pmin)
                        OptModel.AddConstr(expression >= PMinNutrient[NUTR], NUTR.Description);
                    if (pmax)
                        OptModel.AddConstr(expression <= PMaxNutrient[NUTR], NUTR.Description);
                }
                ((Range)((Worksheet)wb.ActiveSheet).Range["B7"]).Value2 = sw.Elapsed.ToString();
                #endregion

                #region Solving
                OptModel.Check();
                OptModel.Update();
                OptModel.Optimize();
                OptModel.Write(@"E:\diet_model.mps");
                OptModel.Write(@"E:\diet_model.sol");
                ((Range)((Worksheet)wb.ActiveSheet).Range["B8"]).Value2 = sw.Elapsed.ToString();
                #endregion

                #region Writes back to DB Context
                //connection.Open();
                //transaction = connection.BeginTransaction();
                //try
                {
                    using (ModelContext db = new ModelContext(connection, false)) {

                        V_FoodQuantity[] vFoodQty = new V_FoodQuantity[SFoods.Count];
                        int i = 0;
                        foreach (var FOOD in SFoods)
                        {
                            vFoodQty[i] = new V_FoodQuantity()
                            {
                                FoodID = FOOD.ID,
                                Quantity = V_FoodQuantity[FOOD].Get(GRB.DoubleAttr.X)
                            };
                            i++;
                        }
                        //db.Database.ExecuteSqlCommand("DELETE FROM V_FoodQuantity");
                        db.V_FoodQuantity.AddOrUpdate(vFoodQty);
                        db.SaveChanges();

                        // Dispose of model and env
                        OptModel.Dispose();
                        env.Dispose();
                        ((Range)((Worksheet)wb.ActiveSheet).Range["B9"]).Value2 = sw.Elapsed.ToString();
                    }
                }
                //catch
                {

                }
                connection.Close();
                #endregion
            }
        }
                
     }
}
