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

namespace EquusModel.Models
{

    public class OptimizationModel
    {
        public static void Solve(Control.SolverProperty sp, Workbook wb)
        {
            using (ModelContext db = new ModelContext() ){
                //fill context with random numbers
                //db.FillContext();
                Stopwatch sw = new Stopwatch();
                sw.Start();

                #region Set and Model Definition
                List<Food> SFoods = db.Foods.ToList();
                List<Nutrient> SNutrients = db.Nutrients.ToList();

                Optimization.Model OptModel = new Optimization.Model();
                ((Range)((Worksheet)wb.ActiveSheet).Range["B4"]).Value2 = sw.Elapsed.ToString();
                #endregion

                #region Variable Creation
                var V_FoodQuantity = new VariableCollection<Food>(
                    (s => new StringBuilder(s.ID.ToString())), 
                    0, 
                    double.PositiveInfinity, 
                    VariableType.Integer,
                    SFoods
                    );
                ((Range)((Worksheet)wb.ActiveSheet).Range["B5"]).Value2 = sw.Elapsed.ToString();
                #endregion

                #region Goal Definition
                OptModel.AddObjective(
                    Expression.Sum(
                        SFoods.Select(
                            FOOD => V_FoodQuantity[FOOD] *
                                (db.FoodCharacteristics.Find(FOOD.ID) != null ? db.FoodCharacteristics.Find(FOOD.ID).Price : 0.0)
                        )
                    )
                );
                ((Range)((Worksheet)wb.ActiveSheet).Range["B6"]).Value2 = sw.Elapsed.ToString();                
                #endregion

                #region Constraint Definition
                foreach (Nutrient NUTR in SNutrients){
                    if (db.NutrientCharacteristics.Find(NUTR.ID) != null)
                    {
                        var expression = 
                            Expression.Sum(SFoods.Select(s => s)
                                .Where(FOOD => db.N_F_Relations.Find(FOOD.ID, NUTR.ID).NutrientPerFood > 0).Select(
                                    FOOD => V_FoodQuantity[FOOD] * db.N_F_Relations.Find(FOOD.ID, NUTR.ID).NutrientPerFood
                                )
                            );
                        OptModel.AddConstraint(expression >= db.NutrientCharacteristics.Find(NUTR.ID).MinNutrient);
                        OptModel.AddConstraint(expression <= db.NutrientCharacteristics.Find(NUTR.ID).MaxNutrient);
                    }
                }
                ((Range)((Worksheet)wb.ActiveSheet).Range["B7"]).Value2 = sw.Elapsed.ToString();
                #endregion

                #region Solving 
                Optimization.Interfaces.ISolver solver = new GLPKSolver(Console.WriteLine);
                Optimization.Solution solution = solver.Solve(OptModel);
                ((Range)((Worksheet)wb.ActiveSheet).Range["B8"]).Value2 = sw.Elapsed.ToString();
                #endregion

                #region Writes back to DB Context
                // no clear solution on how to do this effiently yet
                foreach (Food FOOD in db.Foods)
                {
                    db.V_FoodQuantity.AddOrUpdate(
                        fq => fq.FoodID, 
                        new V_FoodQuantity() { FoodID = FOOD.ID, Quantity = solution.VariableValues[FOOD.ID.ToString()] }
                        );
                }
                db.SaveChanges();
                ((Range)((Worksheet)wb.ActiveSheet).Range["B9"]).Value2 = sw.Elapsed.ToString();
                #endregion
            }
        }
    }
}
