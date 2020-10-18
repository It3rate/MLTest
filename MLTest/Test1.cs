using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Probabilistic.Algorithms;
using Microsoft.ML.Probabilistic.Collections;
using Microsoft.ML.Probabilistic.Compiler.Visualizers;
using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Learners;
using Microsoft.ML.Probabilistic.Learners.Mappings;
using Microsoft.ML.Probabilistic.Models;
using Microsoft.ML.Probabilistic.Math;
using System.Xml.Linq;

namespace MLTest
{
    public class Point
    {
	    [LoadColumn(1)]
	    [ColumnName("Label")]
        public float X;
        [LoadColumn(2)]
        [ColumnName("Score")]
        public float Y;
        //[LoadColumn(3)]
        //public float Score;

        public Point(float x, float y)
	    {
		    X = x;
		    Y = y;
	    }
    }

    public class ClassifierMap : IClassifierMapping<IList<Vector>, int, IList<string>, string, Vector>
    {
	    public IEnumerable<int> GetInstances(IList<Vector> instanceSource)
	    {
		    for (int instance = 0; instance < instanceSource.Count; instance++)
		    {
			    yield return instance;
		    }
	    }

	    public Vector GetFeatures(int instance, IList<Vector> featureVectors)
	    {
		    return featureVectors[instance];
	    }

	    public string GetLabel(int instance, IList<Vector> featureVectors, IList<string> labels)
	    {
		    return labels[instance];
	    }

	    public IEnumerable<string> GetClassLabels(IList<Vector> featureVectors = null, IList<string> labels = null)
	    {
		    return new[] { "F", "M" };
	    }
    }
    public class Test1
    {
		Form _form;
	    public Test1(Form f)
	    {
		    _form = f;
            //Test();
            //Prob();
            //Trial1();
            //StingTest();
            //GameRank();
            //BayesLinReg();
            //BetweenTestInt();
            //BetweenTestGaussian();
            //LearnGaussian();
            Gaussian2();
        }
        public void Gaussian2()
        {
            var x0 = Gaussian.FromMeanAndVariance(10,1);
            var x1 = Gaussian.FromMeanAndVariance(20,1);
            var x2 = x1 * x0;
            var x3 = Variable.GaussianFromMeanAndVariance(x2.GetMean(), x2.GetVariance());
            Range dataRange = new Range(12).Named("n");
            VariableArray<double> cx0 = Variable.Array<double>(dataRange);
            cx0[dataRange] = Variable.GaussianFromMeanAndVariance(x0.Sample(), x1.Sample()).ForEach(dataRange);

            var inferenceEngine = new InferenceEngine();
            //inferenceEngine.NumberOfIterations = 1000;
            var result = inferenceEngine.Infer(x3);
            Console.WriteLine(result);
            Console.WriteLine(x2.Sample());
            Console.WriteLine(x2.Sample());
            Console.WriteLine(x2.Sample());
            Console.WriteLine(x2.Sample());
        }

        void LearnGaussian()
        {
            Variable<double> mean = Variable.GaussianFromMeanAndVariance(0, 10);
            Variable<double> precision = Variable.GammaFromShapeAndScale(1, 1);

            Range dataRange = new Range(12).Named("n");
            VariableArray<double> x = Variable.Array<double>(dataRange);
            x[dataRange] = Variable.GaussianFromMeanAndPrecision(mean, precision).ForEach(dataRange);
            x.ObservedValue = new double[] { 50.0, 50.0, 50.0, 50.0, 50.0, 50.0, 50.0, 50.0, 50.0, 50.0, 50.0, 50.0 };

            var inferenceEngine = new InferenceEngine();
            var meanResult = inferenceEngine.Infer(mean);
            var precResult = inferenceEngine.Infer(precision);
            Console.WriteLine(meanResult +"\n"+ precResult);
        }

        public void BetweenTestGaussian()
        {
            var x0 = Variable.GaussianFromMeanAndVariance(0.1, 0.04);
            var x1 = Variable.GaussianFromMeanAndVariance(1, 0.02);
            var cx0 = Variable.GaussianFromMeanAndVariance(.2, 0.03);

            var test = Variable.IsBetween(cx0, x0, x1);

            var inferenceEngine = new InferenceEngine();
            var result = inferenceEngine.Infer(test);
            Console.WriteLine(result);

            result = inferenceEngine.Infer(test);
            Console.WriteLine(result);
        }
        public void BetweenTestInt()
        {
            var x0 = Variable.New<double>();// Variable.New<double>();
            var x1 = Variable.New<double>();
            var cx0 = Variable.New<double>();

            x0.ObservedValue = 2.0;
            x1.ObservedValue = 3.0;
            cx0.ObservedValue = 2.5;// 12.5;

            var test = Variable.IsBetween(cx0, x0, x1);

            var inferenceEngine = new InferenceEngine();
            var result = inferenceEngine.Infer<Bernoulli>(test);
            Console.WriteLine(result);
            x0.ObservedValue = 2.6;

            result = inferenceEngine.Infer<Bernoulli>(test);
            Console.WriteLine(result);
        }
        public void BayesLinReg()
        {
            Vector[] data = new Vector[] { 
                Vector.FromArray(1.0, -3), Vector.FromArray(1.0, -2.1), Vector.FromArray(1.0, -1.3), 
                Vector.FromArray(1.0, 0.5), Vector.FromArray(1.0, 1.2), Vector.FromArray(1.0, 3.3), 
                Vector.FromArray(1.0, 4.4), Vector.FromArray(1.0, 5.5) };
            Range rows = new Range(data.Length);
            VariableArray<Vector> x = Variable.Constant(data, rows).Named("x");
            Variable<Vector> w = Variable.VectorGaussianFromMeanAndPrecision(Vector.FromArray(new double[] { 0, 0 }), PositiveDefiniteMatrix.Identity(2)).Named("w");
            VariableArray<double> y = Variable.Array<double>(rows);
            y[rows] = Variable.GaussianFromMeanAndVariance(Variable.InnerProduct(x[rows], w), 1.0);
            y.ObservedValue = new double[] { 30, 45, 40, 80, 70, 100, 130, 110 };
            InferenceEngine engine = new InferenceEngine(new VariationalMessagePassing());
            VectorGaussian postW = engine.Infer<VectorGaussian>(w);
            Console.WriteLine("Posterior over the weights: " + Environment.NewLine + postW);
        }
        public void GameRank()
        {
            var playerCount = Variable.New<int>();
            var players = new Range(playerCount);
            var gameCount = Variable.New<int>();
            var games = new Range(gameCount);

            var playerSkills = Variable.Array<double>(players);
            var playerSkillsPrior = Variable.Array<Gaussian>(players);
            using (Variable.ForEach(players))
            {
                playerSkills[players] = Variable<double>.Random(playerSkillsPrior[players]);
            }

            var redPlayers = Variable.Array<int>(games);
            var bluePlayers = Variable.Array<int>(games);
            var redPlayerWins = Variable.Array<bool>(games);
            const double noise = 1.0;
            using (Variable.ForEach(games))
            {
                var redPlayerSkill = playerSkills[redPlayers[games]];
                var redPlayerPerformance = Variable.GaussianFromMeanAndVariance(redPlayerSkill, noise);
                var bluePlayerSkill = playerSkills[bluePlayers[games]];
                var bluePlayerPerformance = Variable.GaussianFromMeanAndVariance(bluePlayerSkill, noise);
                redPlayerWins[games] = redPlayerPerformance > bluePlayerPerformance;
            }

            const int PlayerCount = 6;
            playerSkillsPrior.ObservedValue = Enumerable.Repeat(Gaussian.FromMeanAndVariance(6, 9), PlayerCount).ToArray();

            var hasWon = new[] { true, true, false, true, true, false, true, true, false, true, true, false };
            gameCount.ObservedValue = hasWon.Length;
            playerCount.ObservedValue = PlayerCount;
            redPlayers.ObservedValue = new[] { 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2 };
            bluePlayers.ObservedValue = new[] { 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3 };
            redPlayerWins.ObservedValue = hasWon;

            var inferenceEngine = new InferenceEngine();
            var inferredSkills = inferenceEngine.Infer<Gaussian[]>(playerSkills);

            playerSkillsPrior.ObservedValue = inferredSkills;
            gameCount.ObservedValue = 1;
            redPlayers.ObservedValue = new[] { 0 };
            bluePlayers.ObservedValue = new[] { 2 };
            redPlayerWins.ClearObservedValue();
            var player0Vs3 = inferenceEngine.Infer<Bernoulli[]>(redPlayerWins).First();
        }

        public void StingTest()
        {
            MLContext ctx = new MLContext();
            var pts = CreateData();
            IDataView data = ctx.Data.LoadFromEnumerable<Point>(pts);  //LoadFromTextFile("test.cvs");
            var result = ctx.Regression.Evaluate(data);
            Debug.WriteLine(result);


            Variable<string> str1 = Variable.StringUniform().Named("str1");
            Variable<string> str2 = Variable.StringUniform().Named("str2");
            Variable<string> text = (str1 + " " + str2).Named("text");
            text.ObservedValue = "Hello uncertain world";

            var engine = new InferenceEngine();

            if (engine.Algorithm is Microsoft.ML.Probabilistic.Algorithms.ExpectationPropagation)
            {
                Console.WriteLine("str1: {0}", engine.Infer(str1));
                Console.WriteLine("str2: {0}", engine.Infer(str2));

                var distOfStr1 = engine.Infer<StringDistribution>(str1);
                foreach (var s in new[] { "Hello", "Hello uncertain", "Hello uncertain world" })
                {
                    Console.WriteLine("P(str1 = '{0}') = {1}", s, distOfStr1.GetProb(s));
                }
            }
            else
            {
                Console.WriteLine("This example only runs with Expectation Propagation");
            }
        }
	    public void Trial1()
	    {
            var engine = new InferenceEngine(); 
            var a = Variable.Bernoulli(1.0 / 6.0);
		    var b = Variable.Bernoulli(2.0 / 6.0);
		    var c = a & b;
		    var d = a | b;
		    var infc = engine.Infer(c);
		    var infd = engine.Infer(d);

			var g0 = new Gaussian(3, 2);
            var s0 = g0.Sample();
            var s1 = g0.Sample();
            var s2 = g0.Sample();

            var gmv = Variable.GaussianFromMeanAndVariance(2.0, 1.0);
            var val = engine.Infer<Gaussian>(gmv).Sample();
            Console.WriteLine(val);
            gmv.ObservedValue = 4.0;
            var val2 = engine.Infer<Gaussian>(gmv).Sample();
            Console.WriteLine(val2);

            var vg0 = Variable.GaussianFromMeanAndVariance(3, 2);
			var vg0av = vg0.ArrayVariable;
            var ig0 = engine.Infer(vg0);

			engine.ShowFactorGraph = true;
			engine.SaveFactorGraphToFolder = ".";
        }

        public void Test()
	    {
			Gaussian g = new Gaussian(5, 2.0);
			var gs = new double[10];
			for (int i = 0; i < 10; i++)
			{
				gs[i] = g.Sample();
            }
		    var set = CreateBernoulliData();
		    string[] labels;
		    Vector[] vset;
            CreateVectorData(out vset, out labels);
		    var mapping = new ClassifierMap();
		    var classifier = BayesPointMachineClassifier.CreateBinaryClassifier(mapping);

		    classifier.Train(vset, labels);
			
		    var evaluatorMapping = mapping.ForEvaluation();
		    var evaluator = new ClassifierEvaluator<IList<Vector>, int, IList<string>, string>(evaluatorMapping);

		    var predictions = classifier.PredictDistribution(vset);
		    IEnumerable<string> estimates = classifier.Predict(vset);

		    double errorCount = evaluator.Evaluate(vset, labels, estimates, Metrics.ZeroOneError);

		    double areaUnderRocCurve = evaluator.AreaUnderRocCurve("F", vset, labels, predictions);

		    var rocCurve = evaluator.ReceiverOperatingCharacteristicCurve( "F", vset, predictions);
        }

	    public void CreateVectorData(out Vector[] features, out string[] labels)
	    {
		    int len = 1000;
		    var rnd = new Random(0);
		    features = new Vector[len];
		    labels = new string[len];
		    for (int i = 0; i < len; i++)
		    {
			    var rh = rnd.NextDouble();
			    var rw = rnd.NextDouble();
                if (rnd.NextDouble() < 0.5)
			    {
				    features[i] = Vector.FromArray(rh * 0.3 - 0.1, rw * 0.2 -.01);
				    labels[i] = "M";
			    }
			    else
			    {
				    features[i] = Vector.FromArray(rh * 0.2 + 0.1, rw * 0.2 -0.2);
                    labels[i] = "F";
                }
		    }
	    }
	    public Bernoulli[] CreateBernoulliData()
	    {
		    int len = 1000;
		    var rnd = new Random(0);
		    var result = new Bernoulli[len];
		    for (int i = 0; i < len; i++)
		    {
			    var r = rnd.NextDouble();
			    result[i] = new Bernoulli(r * 0.5f + 0.2f);
		    }
		    return result;
	    }

	    public Point[] CreateData()
	    {
		    int len = 1000;
			var rnd = new Random(0);
		    var result = new Point[len];
		    for (int i = 0; i < len; i++)
		    {
			    var r = rnd.Next(0, 1000) / 1000f;
			    result[i] = new Point(r, r*0.5f + 0.2f);
		    }
		    return result;
	    }

	    public void Prob()
	    {
		    // The winner and loser in each of 6 samples games
		    var winnerData = new[] { 0, 0, 0, 1, 3, 4 };
		    var loserData = new[] { 1, 3, 4, 2, 1, 2 };

		    // Define the statistical model as a probabilistic program
		    var game = new Range(winnerData.Length);
		    var player = new Range(winnerData.Concat(loserData).Max() + 1);
		    var playerSkills = Variable.Array<double>(player);
		    playerSkills[player] = Variable.GaussianFromMeanAndVariance(6, 9).ForEach(player);

		    var winners = Variable.Array<int>(game);
		    var losers = Variable.Array<int>(game);

		    using (Variable.ForEach(game))
		    {
			    // The player performance is a noisy version of their skill
			    var winnerPerformance = Variable.GaussianFromMeanAndVariance(playerSkills[winners[game]], 1.0);
			    var loserPerformance = Variable.GaussianFromMeanAndVariance(playerSkills[losers[game]], 1.0);

			    // The winner performed better in this game
			    Variable.ConstrainTrue(winnerPerformance > loserPerformance);
		    }

		    // Attach the data to the model
		    winners.ObservedValue = winnerData;
		    losers.ObservedValue = loserData;

		    // Run inference
		    var inferenceEngine = new InferenceEngine();
		    var inferredSkills = inferenceEngine.Infer<Gaussian[]>(playerSkills);

		    // The inferred skills are uncertain, which is captured in their variance
		    var orderedPlayerSkills = inferredSkills
			    .Select((s, i) => new { Player = i, Skill = s })
			    .OrderByDescending(ps => ps.Skill.GetMean());

		    foreach (var playerSkill in orderedPlayerSkills)
		    {
			    Console.WriteLine($"Player {playerSkill.Player} skill: {playerSkill.Skill}");
		    }
        }
    }
}
