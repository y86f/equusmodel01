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
                var SFoods = db.FoodCharacteristics.ToList();
                var SNutrients = db.NutrientCharacteristics.ToList();
                var PFoodCost = db.FoodCharacteristics.ToDictionary(fc => fc.FoodID, fc => fc.Price);

                var PNutrientPerFood = 
                    db.N_F_Relations.ToDictionary(
                        nf => new Tuple<int, int>(nf.FoodID, nf.NutrientID), 
                        nf => nf.NutrientPerFood);

                Optimization.Model OptModel = new Optimization.Model();
                ((Range)((Worksheet)wb.ActiveSheet).Range["B4"]).Value2 = sw.Elapsed.ToString();
                #endregion

                #region Variable Creation
                var V_FoodQuantity = new VariableCollection<FoodCharacteristic>(
                    fc => new StringBuilder("[").Append(fc.FoodID.ToString()).Append("]"),
                    0,//fc => fc != null ? fc.MinServings.Value : 0, 
                    double.PositiveInfinity,//fc => fc != null ? fc.MaxServings.Value : double.PositiveInfinity, 
                    VariableType.Integer,
                    SFoods
                    );
                V_FoodQuantity.IndexValidation = false;
                
                ((Range)((Worksheet)wb.ActiveSheet).Range["B5"]).Value2 = sw.Elapsed.ToString();
                #endregion

                #region Goal Definition
                OptModel.AddObjective(Expression.Sum(SFoods.Select(FOOD => V_FoodQuantity[FOOD] * FOOD.Price)));

                ((Range)((Worksheet)wb.ActiveSheet).Range["B6"]).Value2 = sw.Elapsed.ToString();                
                #endregion

                #region Constraint Definition
                foreach (var NUTR in SNutrients){
                    var expression =
                        Expression.Sum(SFoods.Where(FOOD => PNutrientPerFood.ContainsKey(new Tuple<int, int>(FOOD.FoodID, NUTR.NutrientID)))
                            .Select(
                                FOOD => V_FoodQuantity[FOOD] * PNutrientPerFood[new Tuple<int, int>(FOOD.FoodID, NUTR.NutrientID)].Value)
                        );
                    
                    if (NUTR.MinNutrient != null)
                        OptModel.AddConstraint(expression >= NUTR.MinNutrient.Value);

                    if (NUTR.MaxNutrient != null)
                        OptModel.AddConstraint(expression <= NUTR.MaxNutrient.Value);
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
                foreach (var FOOD in SFoods)
                {
                    db.V_FoodQuantity.AddOrUpdate(
                        fq => fq.FoodID,
                        new V_FoodQuantity() { FoodID = FOOD.FoodID, Quantity = V_FoodQuantity[FOOD].Value }
                    );
                }
                db.SaveChanges();
                
                ((Range)((Worksheet)wb.ActiveSheet).Range["B9"]).Value2 = sw.Elapsed.ToString();
                #endregion
            }
        }
    }
}
