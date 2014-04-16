using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optimization;
using Optimization.Interfaces;
using Optimization.Solver.GLPK;

namespace EquusModel.Models
{

    public class OptimizationModel
    {
        public static void Solve(Control.SolverProperty sp)
        {
            using (ModelContext db = new ModelContext() ){
                //fill context with random numbers
                //db.FillContext();
                
                List<Food> SFoods = db.Foods.ToList();
                List<Nutrient> SNutrients = db.Nutrients.ToList();

                Model OptModel = new Model();
               
                var V_FoodQuantity = new VariableCollection<Food>(
                    (s => new StringBuilder("FoodQuantity[").Append(s.Description).Append("]")), 
                    0, 
                    double.PositiveInfinity, 
                    VariableType.Integer,
                    SFoods
                    );

                OptModel.AddObjective(
                    Expression.Sum(
                        SFoods.Select(
                            FOOD => V_FoodQuantity[FOOD] *
                                (db.FoodCharacteristics.Select(fc => fc).Where(fc => fc.FoodID == FOOD.ID).Single(fc => true) != null ?
                                 db.FoodCharacteristics.Select(fc => fc).Where(fc => fc.FoodID == FOOD.ID).Single(fc => true).Price : 0.0)
                        )
                    )
                );
                /*double d;
                FoodCharacteristic temp_fc;
                foreach (Food FOOD in db.Foods) 
                {
                    temp_fc = db.FoodCharacteristics.SingleOrDefault(food => food.ID == FOOD.ID);
                    temp_fc = db.FoodCharacteristics.Find(FOOD);
                    temp_fc = db.FoodCharacteristics.Find(FOOD.ID);
                    var query = db.FoodCharacteristics.Select(s => s).Where(s => s.FoodID == FOOD.ID);
                    temp_fc = db.FoodCharacteristics.Select(s => s).Where(s => s.FoodID == FOOD.ID).Single(s => true);
                    d = temp_fc.Price;
                }*/
                foreach(Nutrient NUTR in SNutrients){
                    if (db.NutrientCharacteristics.Select(nc => nc).Where(nc => nc.NutrientID == NUTR.ID).Single(nc => true) != null)
                    {
                        OptModel.AddConstraint(
                            Expression.Sum(SFoods.Select(
                                FOOD => 
                                    V_FoodQuantity[FOOD] *
                                    (db.N_F_Relations.Select(nf => nf).Where(nf => ((nf.FoodID == FOOD.ID) && (nf.NutrientID == NUTR.ID))).Single(nf => true).NutrientPerFood)
                                )) 
                            >=
                            db.NutrientCharacteristics.Select(nc => nc).Where(nc => nc.NutrientID == NUTR.ID).Single(nc => true).MinNutrient
                        );

                        OptModel.AddConstraint(
                            Expression.Sum(SFoods.Select(
                                FOOD =>
                                    V_FoodQuantity[FOOD] * (db.N_F_Relations.Select(nf => nf).Where(nf => ((nf.FoodID == FOOD.ID) && (nf.NutrientID == NUTR.ID))).Single(nf => true).NutrientPerFood)
                                ))
                            <=
                            db.NutrientCharacteristics.Select(nc => nc).Where(nc => nc.NutrientID == NUTR.ID).Single(nc => true).MaxNutrient
                        );
                    }
                }

                Optimization.Interfaces.ISolver solver = new GLPKSolver(Console.WriteLine);
                Optimization.Solution solution = solver.Solve(OptModel);

                #region Writes back to DB Context
                // no clear solution on how to do this effiently yet

                foreach (Food FOOD in db.Foods)
                {
                    db.V_FoodQuantity.Add(new V_FoodQuantity() { FoodID = FOOD.ID, Quantity = V_FoodQuantity[FOOD].Value });
                }
                db.SaveChanges();   
                #endregion
            }
			
        }
    }
}
