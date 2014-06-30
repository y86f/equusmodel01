using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optimization;
using Optimization.Interfaces;
using Optimization.Solver.GLPK;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using MySql.Data.MySqlClient;

namespace EquusModel.Models
{
    public class OptimizationModel
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
            string connectionString = "server=localhost;port=3300;database=FoodModel;uid=root;password=1234";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Create database if not exists
                using (ModelContext db = new ModelContext(connection, false))
                {
                    //db.FillContext();
                    if (db.Database.CreateIfNotExists())
                        db.FillContext();
                }

                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    using (ModelContext db = new ModelContext(connection, false))
                    {
                        //fill context with random numbers
                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        #region Set and Model Definition
                        var SFoods = db.Foods.ToList();
                        var SNutrients = db.Nutrients.ToList();

                        var PFoodCost = db.FoodCharacteristics.ToDictionary(fc => SFoods[fc.FoodID], fc => fc.Price);
                        var PMinNutrient = db.NutrientCharacteristics
                            .ToDictionary(nc => SNutrients[nc.NutrientID], nc => nc != null ? nc.MinNutrient.Value : 0.0);
                        var PMaxNutrient = db.NutrientCharacteristics
                            .ToDictionary(nc => SNutrients[nc.NutrientID], nc => nc != null ? nc.MaxNutrient.Value : double.PositiveInfinity);
                        var PNutrientPerFood =
                            db.N_F_Relations.ToDictionary(
                                nf => KEY.NK(SFoods[nf.FoodID], SNutrients[nf.NutrientID]),
                                nf => nf.NutrientPerFood);

                        Optimization.Model OptModel = new Optimization.Model();

                        ((Range)((Worksheet)wb.ActiveSheet).Range["B4"]).Value2 = sw.Elapsed.ToString();
                        #endregion

                        #region Variable Creation
                        var V_FoodQuantity = new VariableCollection<Food>(
                            f => new StringBuilder("[").Append(f.ID.ToString()).Append("]"),
                            0,//fc => fc != null ? fc.MinServings.Value : 0, 
                            double.PositiveInfinity,//fc => fc != null ? fc.MaxServings.Value : double.PositiveInfinity, 
                            VariableType.Continuous,
                            SFoods
                            );
                        //speeds up variable creation process
                        V_FoodQuantity.IndexValidation = false;

                        ((Range)((Worksheet)wb.ActiveSheet).Range["B5"]).Value2 = sw.Elapsed.ToString();
                        #endregion

                        #region Goal Definition
                        OptModel.AddObjective(
                            Expression.Sum(SFoods.Select(FOOD => V_FoodQuantity[FOOD] * PFoodCost[FOOD])));

                        ((Range)((Worksheet)wb.ActiveSheet).Range["B6"]).Value2 = sw.Elapsed.ToString();
                        #endregion

                        #region Constraint Definition
                        bool pmin, pmax;
                        foreach (var NUTR in SNutrients)
                        {
                            var expression =
                                Expression.Sum(SFoods
                                    .Where(FOOD => PNutrientPerFood.ContainsKey(KEY.NK(FOOD, NUTR)))
                                    .Select(
                                        FOOD => V_FoodQuantity[FOOD] * PNutrientPerFood[KEY.NK(FOOD, NUTR)].Value)
                                );

                            pmin = PMinNutrient.ContainsKey(NUTR);
                            pmax = PMaxNutrient.ContainsKey(NUTR);

                            if (pmin)
                                if (pmax)
                                    OptModel.AddConstraint(
                                        NUTR.ID.ToString() + NUTR.Description,
                                        PMinNutrient[NUTR], PMaxNutrient[NUTR], expression);
                                else
                                    OptModel.AddConstraint(
                                        NUTR.ID.ToString() + NUTR.Description,
                                        PMinNutrient[NUTR], double.PositiveInfinity, expression);
                            else if (pmax)
                                OptModel.AddConstraint(
                                        NUTR.ID.ToString() + NUTR.Description,
                                        0.0, PMaxNutrient[NUTR], expression);
                        }
                        ((Range)((Worksheet)wb.ActiveSheet).Range["B7"]).Value2 = sw.Elapsed.ToString();
                        #endregion

                        #region Solving
                        var solver = new GLPKSolver();
                        var solution = solver.Solve(OptModel);
                        
                        V_FoodQuantity.SetVariableValues(solution.VariableValues);

                        ((Range)((Worksheet)wb.ActiveSheet).Range["B8"]).Value2 = sw.Elapsed.ToString();
                        #endregion

                        #region Writes back to DB Context
                        V_FoodQuantity[] vFoodQty = new V_FoodQuantity[SFoods.Count];
                        int i = 0;
                        foreach (var FOOD in SFoods)
                        {
                            vFoodQty[i] = new V_FoodQuantity()
                            {
                                FoodID = FOOD.ID,
                                Quantity = V_FoodQuantity[FOOD].Value
                            };
                            i++;
                        }
                        db.Database.ExecuteSqlCommand("DELETE FROM V_FoodQuantity");
                        db.V_FoodQuantity.AddRange(vFoodQty);
                        db.SaveChanges();

                        ((Range)((Worksheet)wb.ActiveSheet).Range["B9"]).Value2 = sw.Elapsed.ToString();
                        #endregion
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
