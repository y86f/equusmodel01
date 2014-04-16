﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Optimization;
using Optimization.Interfaces;
using Optimization.Solver.GLPK;

namespace $rootnamespace$.Samples.Optimization.Framework
{
    class MultiTransportSample
    {
        public static void Run()
		{
			Model model = BuildModel();
			Solution solution = SolveModel(model);
		}

		static Model BuildModel()
		{
			#region Data

			var multiModel = new MultiTransportModel { Limit = 625 };
			
			var bands = new Product { Id = 0, Name = "bands" };
			var coils = new Product { Id = 1, Name = "coils" };
			var plate = new Product { Id = 2, Name = "plate" };

			var products = new List<Product> { bands, coils, plate };

			var gary = new Origin
			{
				Id = 0,
				Name = "GARY",
				Supply = new Dictionary<Product, int>
				{
					{bands, 400},
					{coils, 800},
					{plate, 200}
				}
			};
			var clev = new Origin
			{
				Id = 1,
				Name = "CLEV",
				Supply = new Dictionary<Product, int>
				{
					{bands, 700},
					{coils, 1600},
					{plate, 300}
				}
			};
			var pitt = new Origin
			{
				Id = 2,
				Name = "PITT",
				Supply = new Dictionary<Product, int>
				{
					{bands, 800},
					{coils, 1800},
					{plate, 300}
				}
			};

			var origins = new List<Origin> { gary, clev, pitt };

			Destination fra = new Destination
			{
				Id = 0,
				Name = "FRA",
				Demand = new Dictionary<Product, int>
				{
					{bands, 300},
					{coils, 500},
					{plate, 100}
				}
			};

			Destination det = new Destination
			{
				Id = 1,
				Name = "DET",
				Demand = new Dictionary<Product, int>
				{
					{bands, 300},
					{coils, 750},
					{plate, 100}
				}
			};

			Destination lan = new Destination
			{
				Id = 2,
				Name = "LAN",
				Demand = new Dictionary<Product, int>
				{
					{bands, 100},
					{coils, 400},
					{plate, 0}
				}
			};
			Destination win = new Destination
			{
				Id = 3,
				Name = "WIN",
				Demand = new Dictionary<Product, int>
				{
					{bands, 75},
					{coils, 250},
					{plate, 50}
				}
			};
			Destination stl = new Destination
			{
				Id = 4,
				Name = "STL",
				Demand = new Dictionary<Product, int>
				{
					{bands, 650},
					{coils, 950},
					{plate, 200}
				}
			};
			Destination fre = new Destination
			{
				Id = 5,
				Name = "FRE",
				Demand = new Dictionary<Product, int>
				{
					{bands, 225},
					{coils, 850},
					{plate, 100}
				}
			};
			Destination laf = new Destination
			{
				Id = 6,
				Name = "LAF",
				Demand = new Dictionary<Product, int>
				{
					{bands, 250},
					{coils, 500},
					{plate, 250}
				}
			};

			var destinations = new List<Destination> { fra, det, lan, win, stl, fre, laf };

			gary.Cost = new List<Tuple<Destination, Product, int>>
			{
				new Tuple<Destination,Product,int>(fra,bands,30),
				new Tuple<Destination,Product,int>(fra,coils,39),
				new Tuple<Destination,Product,int>(fra,plate,41),

				new Tuple<Destination,Product,int>(det,bands,10),
				new Tuple<Destination,Product,int>(det,coils,14),
				new Tuple<Destination,Product,int>(det,plate,15),

				new Tuple<Destination,Product,int>(lan,bands,8),
				new Tuple<Destination,Product,int>(lan,coils,11),
				new Tuple<Destination,Product,int>(lan,plate,12),

				new Tuple<Destination,Product,int>(win,bands,10),
				new Tuple<Destination,Product,int>(win,coils,14),
				new Tuple<Destination,Product,int>(win,plate,16),

				new Tuple<Destination,Product,int>(stl,bands,11),
				new Tuple<Destination,Product,int>(stl,coils,16),
				new Tuple<Destination,Product,int>(stl,plate,17),

				new Tuple<Destination,Product,int>(fre,bands,71),
				new Tuple<Destination,Product,int>(fre,coils,82),
				new Tuple<Destination,Product,int>(fre,plate,86),

				new Tuple<Destination,Product,int>(laf,bands,6),
				new Tuple<Destination,Product,int>(laf,coils,8),
				new Tuple<Destination,Product,int>(laf,plate,8)
			};

			clev.Cost = new List<Tuple<Destination, Product, int>>
			{
				new Tuple<Destination,Product,int>(fra,bands,22),
				new Tuple<Destination,Product,int>(fra,coils,27),
				new Tuple<Destination,Product,int>(fra,plate,29),

				new Tuple<Destination,Product,int>(det,bands,7),
				new Tuple<Destination,Product,int>(det,coils,9),
				new Tuple<Destination,Product,int>(det,plate,9),

				new Tuple<Destination,Product,int>(lan,bands,10),
				new Tuple<Destination,Product,int>(lan,coils,12),
				new Tuple<Destination,Product,int>(lan,plate,13),

				new Tuple<Destination,Product,int>(win,bands,7),
				new Tuple<Destination,Product,int>(win,coils,9),
				new Tuple<Destination,Product,int>(win,plate,9),

				new Tuple<Destination,Product,int>(stl,bands,21),
				new Tuple<Destination,Product,int>(stl,coils,26),
				new Tuple<Destination,Product,int>(stl,plate,28),

				new Tuple<Destination,Product,int>(fre,bands,82),
				new Tuple<Destination,Product,int>(fre,coils,95),
				new Tuple<Destination,Product,int>(fre,plate,99),

				new Tuple<Destination,Product,int>(laf,bands,13),
				new Tuple<Destination,Product,int>(laf,coils,17),
				new Tuple<Destination,Product,int>(laf,plate,18)
			};

			pitt.Cost = new List<Tuple<Destination, Product, int>>
			{
				new Tuple<Destination,Product,int>(fra,bands,19),
				new Tuple<Destination,Product,int>(fra,coils,24),
				new Tuple<Destination,Product,int>(fra,plate,26),

				new Tuple<Destination,Product,int>(det,bands,11),
				new Tuple<Destination,Product,int>(det,coils,14),
				new Tuple<Destination,Product,int>(det,plate,14),

				new Tuple<Destination,Product,int>(lan,bands,12),
				new Tuple<Destination,Product,int>(lan,coils,17),
				new Tuple<Destination,Product,int>(lan,plate,17),

				new Tuple<Destination,Product,int>(win,bands,10),
				new Tuple<Destination,Product,int>(win,coils,13),
				new Tuple<Destination,Product,int>(win,plate,13),

				new Tuple<Destination,Product,int>(stl,bands,25),
				new Tuple<Destination,Product,int>(stl,coils,28),
				new Tuple<Destination,Product,int>(stl,plate,31),

				new Tuple<Destination,Product,int>(fre,bands,83),
				new Tuple<Destination,Product,int>(fre,coils,99),
				new Tuple<Destination,Product,int>(fre,plate,104),

				new Tuple<Destination,Product,int>(laf,bands,15),
				new Tuple<Destination,Product,int>(laf,coils,20),
				new Tuple<Destination,Product,int>(laf,plate,20)
			};

			multiModel.Destinations = destinations;
			multiModel.Origins = origins;
			multiModel.Products = products;

			#endregion

			#region Model

			/*
			 * mathematical Model
			 */

			var mathModel = new Model();
			var Transport = new VariableCollection<Origin, Destination, Product>(
				(x, y, z) => new StringBuilder("Product_").Append(z.Id).Append(" from Origin_").Append(x.Id).Append(" to Destination_").Append(y.Id),
				0,
				double.PositiveInfinity,
				VariableType.Integer,
				multiModel.Origins,
				multiModel.Destinations,
				multiModel.Products
				);

			mathModel.AddObjective(
				Expression.Sum(multiModel.Origins.SelectMany(orig => orig.Cost.Select(costlist => (costlist.Item3 * Transport[orig, costlist.Item1, costlist.Item2])))),
				"z"
				);

			// Supply
			foreach (Origin orig in multiModel.Origins)
			{
				foreach (Product prod in multiModel.Products)
				{
					var expression = Expression.Sum(multiModel.Destinations.Select(dest => Transport[orig, dest, prod]));
					mathModel.AddConstraint(expression == orig.Supply[prod]);
				}
			}

			// Demand
			foreach (Destination dest in multiModel.Destinations)
			{
				foreach (Product prod in multiModel.Products)
				{
					var expression = Expression.Sum(multiModel.Origins.Select(orig => Transport[orig, dest, prod]));
					mathModel.AddConstraint(expression == dest.Demand[prod]);
				}
			}

			// Limits
			foreach (Origin orig in multiModel.Origins)
			{
				foreach (Destination dest in multiModel.Destinations)
				{
					var expression = Expression.Sum(multiModel.Products.Select(prod => Transport[orig, dest, prod]));
					mathModel.AddConstraint(expression <= multiModel.Limit);
				}
			}

			return mathModel;

			#endregion
		}

		private static Solution SolveModel(Model mathModel)
		{
			ISolver solver = new GLPKSolver(Console.WriteLine);
			Solution solution = solver.Solve(mathModel);

			return solution;
		}
		
		/*
		 * Classes used for the definition of the data
		 */

		class MultiTransportModel
		{
			public int Limit;

			public List<Origin> Origins;

			public List<Destination> Destinations;

			public List<Product> Products;

			public MultiTransportModel()
			{
				Origins = new List<Origin>();
				Destinations = new List<Destination>();
				Products = new List<Product>();
			}
		}

		class Origin
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public Dictionary<Product, int> Supply { get; set; }
			public List<Tuple<Destination, Product, int>> Cost { get; set; }
		}

		class Destination
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public Dictionary<Product, int> Demand { get; set; }
		}

		class Product
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}
    }
}
